"""Check Unity source-repository integrity without starting the Editor."""

from collections import defaultdict
import json
from pathlib import Path, PurePosixPath
import re
import subprocess
import sys
from urllib.parse import unquote, urlsplit


ROOT = Path(__file__).resolve().parents[1]
errors = []


def check(condition, message):
    if not condition:
        errors.append(message)


def read(path):
    return (ROOT / path).read_text(encoding="utf-8-sig")


def main():
    try:
        result = subprocess.run(
            ["git", "ls-files", "-z", "--cached", "--others", "--exclude-standard"],
            cwd=ROOT, check=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
        )
    except (OSError, subprocess.CalledProcessError) as error:
        print(f"Run this check inside a Git checkout with Git installed: {error}")
        return 1

    files = set(result.stdout.decode("utf-8").split("\0")) - {""}
    required = (
        "Packages/manifest.json", "Packages/packages-lock.json",
        "ProjectSettings/ProjectVersion.txt", "ProjectSettings/EditorSettings.asset",
        "ProjectSettings/VersionControlSettings.asset",
        "ProjectSettings/EditorBuildSettings.asset", "Assets/Scenes/Home.unity",
        "README.md", ".gitignore", ".gitattributes", "CONTRIBUTING.md",
    )
    for path in required:
        check(path in files and (ROOT / path).is_file(), f"Missing required file: {path}")
    if errors:
        return finish(len(files), 0)

    forbidden_roots = {
        "library", "temp", "obj", "build", "builds", "logs", "usersettings",
        "captures", "memorycaptures", "recordings", ".utmp", ".vs", ".idea",
        ".venv", "testresults", "coverage",
    }
    forbidden_suffixes = {
        ".apk", ".aab", ".keystore", ".jks", ".p12", ".pfx",
        ".mobileprovision", ".csproj", ".sln", ".slnx", ".pyc",
    }
    case_paths = defaultdict(list)
    for path in sorted(files):
        item = PurePosixPath(path)
        case_paths[path.casefold()].append(path)
        check(item.parts[0].casefold() not in forbidden_roots,
              f"Generated/local folder is included: {path}")
        check(item.suffix.lower() not in forbidden_suffixes,
              f"Generated file or signing material is included: {path}")
        check(not (item.name == ".env" or
                   (item.name.startswith(".env.") and item.name != ".env.example")),
              f"Local environment file is included: {path}")
        check("__pycache__" not in item.parts, f"Python cache is included: {path}")
        check(not path.startswith("Assets/Captures/"), f"Bulk QA capture is included: {path}")
        check((ROOT / path).is_file(), f"Included file is missing on disk: {path}")
        if (ROOT / path).is_file():
            check((ROOT / path).stat().st_size < 100 * 1024 * 1024,
                  f"File exceeds the regular GitHub blob limit: {path}")
    for paths in case_paths.values():
        check(len(paths) == 1, f"Case-sensitive path collision: {paths}")

    asset_files = {p for p in files if p.startswith("Assets/")}
    guids = defaultdict(list)
    folders = set()
    for path in sorted(asset_files):
        item = PurePosixPath(path)
        folders.update(str(p) for p in item.parents if str(p).startswith("Assets/"))
        if item.suffix == ".meta":
            target = path[:-5]
            check(target in files or (ROOT / target).is_dir(), f"Orphan metadata: {path}")
            if not (ROOT / path).is_file():
                continue
            match = re.search(r"^guid: ([0-9a-f]{32})\s*$", read(path), re.MULTILINE)
            check(match is not None, f"Missing or invalid GUID: {path}")
            if match:
                guids[match.group(1)].append(path)
        else:
            check(path + ".meta" in files, f"Asset is missing metadata: {path}")
    for folder in sorted(folders):
        check(folder + ".meta" in files, f"Asset folder is missing metadata: {folder}")
    for guid, paths in guids.items():
        check(len(paths) == 1, f"Duplicate asset GUID {guid}: {paths}")

    try:
        manifest = json.loads(read("Packages/manifest.json"))["dependencies"]
        locked = json.loads(read("Packages/packages-lock.json"))["dependencies"]
        for name, version in manifest.items():
            check(name in locked, f"Direct package is absent from lock file: {name}")
            if re.fullmatch(r"\d+\.\d+\.\d+(?:[-+][\w.-]+)?", version):
                check(locked.get(name, {}).get("version") == version,
                      f"Manifest/lock version mismatch: {name}")
            else:
                check(not re.search(r"(?:latest|\*|^[~^])", version),
                      f"Unpinned package version: {name}={version}")
    except (ValueError, KeyError, TypeError) as error:
        errors.append(f"Invalid package configuration: {error}")

    check("m_Mode: Visible Meta Files" in read("ProjectSettings/VersionControlSettings.asset"),
          "Unity must use Visible Meta Files")
    check("m_SerializationMode: 2" in read("ProjectSettings/EditorSettings.asset"),
          "Unity must use Force Text serialization")
    check(re.search(r"^m_EditorVersion: \d+\.\d+\.\d+\w\d+\s*$",
                    read("ProjectSettings/ProjectVersion.txt"), re.MULTILINE) is not None,
          "A concrete Unity Editor version must be recorded")
    scenes = re.findall(r"- enabled: (\d+)\s+path: (.+)",
                        read("ProjectSettings/EditorBuildSettings.asset"))
    check(any(enabled == "1" and path.strip() == "Assets/Scenes/Home.unity"
              for enabled, path in scenes), "Home must be an enabled build scene")
    for enabled, path in scenes:
        if enabled == "1":
            check(path.strip() in files, f"Enabled build scene is missing: {path}")

    # Check links in current documentation. The preserved development history is archival.
    docs = [p for p in files if p.endswith(".md") and
            (p in {"README.md", "CONTRIBUTING.md"} or p.startswith("docs/")) and
            p != "docs/DEVELOPMENT_HISTORY.md"]
    for doc in docs:
        content = read(doc)
        links = re.findall(r"\]\(([^)]+)\)", content)
        links += re.findall(r'(?:src|href)="([^"]+)"', content)
        for link in links:
            parsed = urlsplit(link)
            if parsed.scheme or parsed.netloc or not parsed.path:
                continue
            target = ((ROOT / doc).parent / unquote(parsed.path)).resolve()
            check(target.is_relative_to(ROOT) and target.exists(),
                  f"Broken local documentation link in {doc}: {link}")

    return finish(len(files), len(guids))


def finish(file_count, guid_count):
    if errors:
        for message in errors:
            print(f"ERROR: {message}")
        print(f"Repository check failed: {len(errors)} problem(s).")
        return 1
    print(f"Repository integrity passed: {file_count} files, {guid_count} unique asset GUIDs.")
    print("Unity compilation, gameplay and release/device regression are separate checks.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
