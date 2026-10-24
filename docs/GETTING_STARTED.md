# Setup and builds

## First import

Clone `https://github.com/AntoCanza/mr-rabbit.git`, add its root folder to Unity Hub and use **6000.3.25f1**, recorded in `ProjectSettings/ProjectVersion.txt`. Install Android Build Support, SDK/NDK and OpenJDK only if building Android.

Let Package Manager restore `Packages/manifest.json` and `Packages/packages-lock.json`. First import rebuilds the ignored `Library` cache. The current binary assets are ordinary Git blobs, so a normal clone contains them without a Git LFS step.

Open `Assets/Scenes/Home.unity` and enter Play mode. `SampleScene.unity` and the URP scene templates are not the game entry point. Narration WAV files are already included under `Assets/Audio/Italian`; voice generation is optional.

## Android build

Use **File → Build Profiles**, choose Android and enable `Assets/Scenes/Home.unity`. Review these checked-in settings before building:

| Setting | Current value |
| --- | --- |
| Product | Mr Rabbit |
| Source version | 0.10.0 |
| Android version code | 17 |
| Package identifier | `com.mrrabbit.home` |
| Scripting backend / architecture | IL2CPP / ARM64 |
| Orientation | Landscape |
| Minimum SDK | API 25 |
| Target SDK | Automatic in Unity |

Use the SDK, NDK and JDK installed for this Editor. Write development APKs to `Builds/Android/`, which is ignored. Release signing keys, passwords, store credentials and machine-specific paths stay outside Git. Store requirements must be reviewed separately at publication time.

The iOS identifier is also `com.mrrabbit.home`, but a real iOS build needs macOS, Xcode and Apple signing. The repository's history does not establish an iOS release.

## Optional Unity CLI automation

The project pins `com.unity.pipeline` for development-time Editor automation. The normal Unity Editor workflow works without installing the CLI. If using it, install the official Unity CLI and explicitly target this checkout when multiple Editors are open.

`AgentScripts/*.cs` are source files intended to be evaluated through a compatible Editor scripting tool. They are outside `Assets`, so their classes do not automatically appear in player assemblies or Unity menu commands. Read each helper before running it: several `Add*`, `Install*`, `Polish*` and `Prepare*` scripts are historical migrations that change scenes, art or version numbers. **A fresh clone already contains the completed scene; do not replay every helper.** In particular, the historical `PrepareTabletRelease.cs` sets an older version.

For feature checks and save protection, follow [Verification](VERIFICATION.md). For optional development-time speech preparation, follow [Assets and audio](ASSETS_AND_AUDIO.md).

## Common setup issues

| Symptom | Check |
| --- | --- |
| Empty template scene | Open `Assets/Scenes/Home.unity`. |
| Package or compile errors on first open | Confirm the exact Editor version, network access for the first restore and Package Manager diagnostics. Preserve the two package files when troubleshooting. |
| Missing scripts, materials or references | Check that the asset and its original `.meta` file were both cloned. Do not regenerate GUIDs as a first fix. |
| Game view appears cropped | Choose a landscape aspect ratio and reduce the Game view Scale slider to fit the panel. Some painting crop is intentional. |
| Narration is silent | Check the saved mute preference, game/device volume and packaged `Assets/Audio/Italian` files. |
| Android platform is unavailable | Install Android Build Support and its SDK, NDK and OpenJDK for the pinned Editor in Unity Hub. |
| Old progress appears during Play mode | Progress uses local PlayerPrefs. Back it up before QA; do not clear real saves merely to reproduce a fresh install. |
