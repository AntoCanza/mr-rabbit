"""Prepare the approved Elsa voice, preserving Unity WAV paths and GUIDs.

Install build-time dependencies: edge-tts, imageio-ffmpeg (also in Library/VoiceTools).
Run with --install to copy a completely validated batch into Assets. Generation
uses Microsoft's online service; game playback remains fully offline.
"""
import argparse
import asyncio
import hashlib
import json
import shutil
import subprocess
import sys
import wave
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "Library" / "VoiceTools"))
import edge_tts
import imageio_ffmpeg


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def validate(path, sample_rate):
    with wave.open(str(path), "rb") as audio:
        seconds = audio.getnframes() / audio.getframerate()
        if (audio.getnchannels(), audio.getsampwidth(), audio.getframerate()) != (1, 2, sample_rate):
            raise ValueError(f"Unexpected audio format: {path}")
        if not .2 < seconds < 45:
            raise ValueError(f"Unexpected duration {seconds}: {path}")
        return round(seconds, 3)


async def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--install", action="store_true")
    args = parser.parse_args()
    profile_path = ROOT / "ArtSources/ItalianVoice.profile.json"
    profile = json.loads(profile_path.read_text(encoding="utf-8-sig"))
    lines = json.loads((ROOT / "ArtSources/ItalianVoice.lines.json").read_text(encoding="utf-8-sig"))
    stage = ROOT / "ArtSources/ItalianVoiceNeural"
    stage.mkdir(parents=True, exist_ok=True)
    semaphore = asyncio.Semaphore(2)
    ffmpeg = imageio_ffmpeg.get_ffmpeg_exe()

    async def prepare(key, text):
        if not key.replace("_", "").isalnum() or not text.strip():
            raise ValueError(f"Invalid voice line {key}")
        fingerprint = hashlib.sha256(json.dumps([profile, text], sort_keys=True).encode()).hexdigest()
        wav_path = stage / f"{key}.wav"
        receipt = stage / f"{key}.json"
        async with semaphore:
            cache = json.loads(receipt.read_text()) if receipt.exists() else {}
            if not (wav_path.exists() and cache.get("fingerprint") == fingerprint
                    and cache.get("sha256") == digest(wav_path)):
                mp3_path = stage / f"{key}.mp3"
                for attempt in range(3):
                    try:
                        await edge_tts.Communicate(text, profile["voice"], rate=profile["rate"],
                                                  volume=profile["volume"], pitch=profile["pitch"]).save(str(mp3_path))
                        break
                    except Exception:
                        if attempt == 2:
                            raise
                        await asyncio.sleep(2 * (attempt + 1))
                subprocess.run([ffmpeg, "-v", "error", "-y", "-i", str(mp3_path), "-ac", "1",
                                "-ar", str(profile["sampleRate"]), "-c:a", "pcm_s16le", str(wav_path)], check=True)
                validate(wav_path, profile["sampleRate"])
                receipt.write_text(json.dumps({"fingerprint": fingerprint, "sha256": digest(wav_path)}))
            duration = validate(wav_path, profile["sampleRate"])
            print(f"Ready {key}: {duration}s", flush=True)
            return {"key": key, "text": text, "seconds": duration, "sha256": digest(wav_path)}

    entries = await asyncio.gather(*(prepare(key, text) for key, text in lines.items()))
    manifest = {"profile": profile, "synthetic": True, "generator": "edge-tts",
                "generatorVersion": __import__("importlib.metadata", fromlist=["version"]).version("edge-tts"),
                "source": "https://github.com/rany2/edge-tts", "clips": entries}
    (stage / "manifest.json").write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    if args.install:
        destination = ROOT / "Assets/Audio/Italian"
        backup = ROOT / "Library/MrRabbitQA/VoiceBeforeElsaNeural"
        backup.mkdir(parents=True, exist_ok=True)
        for entry in entries:
            target = destination / (entry["key"] + ".wav")
            if target.exists() and not Path(str(target) + ".meta").exists():
                raise ValueError(f"Expected existing Unity audio asset: {target}")
        for entry in entries:
            target = destination / (entry["key"] + ".wav")
            original = backup / target.name
            if target.exists() and not original.exists():
                shutil.copy2(target, original)
            shutil.copy2(stage / target.name, target)
            if digest(target) != entry["sha256"]:
                raise ValueError(f"Copy verification failed: {target}")
        print(f"Installed {len(entries)} validated Elsa clips; existing meta files preserved.")
    else:
        print(f"Prepared {len(entries)} clips. Use --install to update game audio.")


if __name__ == "__main__":
    asyncio.run(main())
