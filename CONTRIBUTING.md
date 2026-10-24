# Contributing to Mr Rabbit

Read [README.md](README.md), [AGENTS.md](AGENTS.md) and the [architecture guide](docs/ARCHITECTURE.md) before changing the project. Use Unity **6000.3.25f1** and open the repository root in Unity Hub.

## Design expectations

- Design for Italian-speaking children aged 6–8. Keep essential exercise text and short orientation labels; use recognizable drawings and contextual highlights for actions.
- Keep school learning as the principal progression. World activities and rewards support it. Do not add subtraction, a language picker or claims of complete curriculum coverage.
- Preserve existing saves, earned medals, coins, inventory, outfits and unfinished tutorial progress. Keep guide state separate from earned rewards.
- Keep the approved calm `it-IT-ElsaNeural` narration, profile and offline playback. Hints must advance from successful actions and respect real availability.
- Decorative hints must not intercept touches. Check controls at tablet aspect ratios and respect the safe area.

## Working on a change

1. Create a focused branch from the current default branch, for example `feature/garden-feedback` or `fix/school-touch-target`.
2. Make asset moves and renames through Unity so their `.meta` files and GUIDs stay together. Keep runtime code under `Assets/Scripts` and Editor automation outside player assemblies.
3. Commit `Assets`, `Packages` and `ProjectSettings` changes together when required. Keep both package JSON files in sync. Do not commit caches, bulk captures, builds, signing keys or player-save backups.
4. Run `python tools/check_repository.py`, then the relevant Editor checks from [Verification](docs/VERIFICATION.md). Record what actually ran and its limits.
5. Open a pull request explaining the child-visible change, validation and any save-compatibility impact. Attach a few useful screenshots for visual changes.

Avoid broad source-formatting changes and unnecessary moves: scene references and serialized field names matter. Each `MonoBehaviour` should have its own matching C# file. The current compact style in existing gameplay files can be retained.

## Bugs and features

Use the issue templates. Include the source version, Unity version, device, resolution, reproducible steps and expected/actual behavior. Keep children's names, handwriting samples, real saves and other personal data out of public reports.

## Releases

Run the full feature and functional regression before packaging or installing a release, as required by [AGENTS.md](AGENTS.md). Restore real saves after testing. Clearly distinguish simulated tablet checks from actual device checks. Source-repository checks alone do not satisfy a release gate.

## Licensing

A project-wide reuse license has not been chosen. Do not replace existing notices or introduce new third-party assets without recording their origin and applicable terms. See [Assets and audio](docs/ASSETS_AND_AUDIO.md).
