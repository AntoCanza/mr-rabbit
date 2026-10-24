<div align="center">
  <img src="Assets/Art/Characters/MrRabbitIcon.png" alt="Mr Rabbit" width="120" />
  <h1>Mr Rabbit</h1>
  <p><strong>A little world to explore. A gentle place to learn.</strong></p>
  <p>An illustrated Italian learning adventure for children aged 6–8, built with Unity.</p>
  <p>
    <a href="ProjectSettings/ProjectVersion.txt"><img alt="Unity 6000.3.25f1" src="https://img.shields.io/badge/Unity-6000.3.25f1-222222?logo=unity" /></a>
    <a href="ProjectSettings/ProjectSettings.asset"><img alt="Version 0.10.0" src="https://img.shields.io/badge/version-0.10.0-D99A45" /></a>
    <img alt="Android tablets" src="https://img.shields.io/badge/Android-tablets-87A878?logo=android&amp;logoColor=white" />
    <img alt="Italian" src="https://img.shields.io/badge/language-Italian-639D9C" />
    <a href="https://github.com/AntoCanza/mr-rabbit/actions/workflows/repository-checks.yml"><img alt="Repository checks" src="https://github.com/AntoCanza/mr-rabbit/actions/workflows/repository-checks.yml/badge.svg?branch=main" /></a>
  </p>
  <p><a href="#get-started">Get started</a> · <a href="#inside-the-game">The game</a> · <a href="#repository-layout">Repository layout</a> · <a href="docs/ARCHITECTURE.md">Architecture</a> · <a href="CONTRIBUTING.md">Contributing</a></p>
</div>

![Mr Rabbit at home, with the kitchen, staircase and medal wall](docs/images/home.png)

## A world built around learning

Choose Mr Rabbit or Miss Rabbit and step into a warm, painted world inspired by original drawings. Visit friends, grow food, cook at home and take the train into town. At the centre is school: short, varied Italian literacy and mathematics activities earn medals and unlock rewards to enjoy elsewhere.

The game is designed for landscape tablets and young readers. Recognizable illustrations, animated action cues and a calm Italian voice help children find their next step. Children can explore at their own pace, replay an explanation or pause the guide.

**Current source version: 0.10.0, Android version code 17.** This repository contains the Unity project and development tools. The development history records earlier build and device checks; publishing this source does not create a new game release.

## Inside the game

| Place | What children can do |
| --- | --- |
| **School** | Practise vowels, letter sounds, handwriting, syllables, words, short reading prompts, counting, number sequences and addition. Earn eleven milestone medals through practice and short checks. |
| **Home** | Walk between floors, hang and rearrange earned medals, cook harvested ingredients, eat, watch TV and rest upstairs. |
| **Countryside** | Explore a continuous landscape, choose shoes or roller skates, experience day and night, and visit Zebrella and Coccodella. |
| **Garden** | Prepare soil, choose seeds, sow, water and harvest. School rewards unlock additional crops. |
| **Train and city** | Travel to town, accept produce orders, deliver crops for coins and buy outfits. |
| **Elsa's guide** | Follow a resumable illustrated introduction and contextual spoken hints. Replay, mute, pause or skip guidance without losing earned progress. |

<table>
  <tr>
    <td width="50%"><img src="docs/images/school.png" alt="Italian vowel exercise, handwriting pad and contextual finger hint" /></td>
    <td width="50%"><img src="docs/images/train.png" alt="Miss Rabbit travelling to the city by train" /></td>
  </tr>
  <tr>
    <td align="center"><em>Write, listen and learn through short activities.</em></td>
    <td align="center"><em>Explore a world connected to school rewards.</em></td>
  </tr>
</table>

The learning sequence covers selected first-primary skills in Italian. It does not claim complete national-curriculum coverage. Subtraction is intentionally excluded. Handwriting recognition runs locally; synthetic regression results are not a measurement of real children's handwriting accuracy.

## Get started

### Requirements

- **Unity Hub** and **Unity Editor 6000.3.25f1**, matching [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
- **Git** to clone the project. Current assets are stored directly in Git; Git LFS is not required.
- For Android: install **Android Build Support**, including the SDK, NDK and OpenJDK, for this Editor through Unity Hub.

### Open and play

```sh
git clone https://github.com/AntoCanza/mr-rabbit.git
cd mr-rabbit
```

1. In Unity Hub, choose **Add project from disk** and select the cloned `mr-rabbit` folder. This is the Unity project root.
2. Open it with **6000.3.25f1**. Allow Unity to restore packages and import assets; the first import can take several minutes.
3. Open **`Assets/Scenes/Home.unity`** and press **Play**.
4. Follow or skip the introduction, choose a rabbit and explore. The illustrated help book reopens the guide.

**Home** is the only enabled game build scene. The sample scene and scene templates are retained. Python and the Unity CLI are optional development tools; they are not required to open or play the project.

### Controls

| Input | Action |
| --- | --- |
| **Drag on free background** | Move in the direction of the finger or mouse motion, keep holding to walk and release to stop. A small opposite motion reverses direction. |
| **Tap an illustrated action** | Use a door, interact with a friend or activity, choose an item or operate an exercise control. |
| **Arrow keys in the Editor** | Walk left/right; use up/down near stair landings to change floors. |
| **Finger or mouse on a writing pad** | Draw the requested letter or number, then submit with the checkmark. |
| **Speaker and guide controls** | Replay speech, save the mute preference, revisit the introduction or pause contextual guidance. |

## Repository layout

The repository root is the Unity project root, following [Unity's project directory conventions](https://docs.unity.com/en-us/engine/6000.3/manual/get-started/project-configuration/default-directories).

```text
mr-rabbit/
├── Assets/                     # Game content and matching .meta files
│   ├── Art/                    # Characters, home, school, garden, city and countryside
│   ├── Audio/Italian/          # Packaged offline Italian narration
│   ├── Scenes/Home.unity       # Game entry point and enabled build scene
│   ├── Scripts/                # Gameplay, learning, input, saves and UI
│   ├── Settings/               # URP 2D renderer and input configuration
│   ├── Screenshots/            # Existing in-project screenshots
│   └── TextMesh Pro/           # Imported text resources and notices
├── Packages/                   # manifest.json and packages-lock.json
├── ProjectSettings/            # Editor version and shared Unity configuration
├── AgentScripts/               # Optional Editor automation, QA and voice preparation
├── ArtSources/                 # Original drawings, prompts and voice provenance
├── docs/                       # Setup, architecture, QA, history and README images
├── tools/                      # Repository integrity checker
├── .github/                    # Repository checks and contribution templates
├── .gitattributes              # Text line endings and binary file handling
├── .gitignore                  # Unity caches, outputs, captures and local secrets
├── .editorconfig               # Shared editor formatting defaults
├── AGENTS.md                   # Gameplay and release constraints
└── CONTRIBUTING.md             # Contribution and review expectations
```

`Assets`, `Packages` and `ProjectSettings` are versioned together. Asset `.meta` files preserve Unity GUIDs and references. Generated `Library`, `Temp`, `Logs`, `UserSettings`, IDE projects, build packages and bulk QA captures stay local. The three README images are curated copies of existing 0.10.0 captures.

## Technology and conventions

- **C# / Unity 6**, **URP 2D**, **uGUI**, **TextMesh Pro** and the **Input System**.
- **Visible Meta Files** and **Force Text** serialization are already enabled.
- Packages are pinned in [manifest.json](Packages/manifest.json) and [packages-lock.json](Packages/packages-lock.json).
- Runtime scripts live in `Assets/Scripts`, primarily under the `MrRabbit` namespace. Editor helpers stay outside `Assets` and player builds.
- Versioned local `PlayerPrefs` keys store progress. Updates must preserve coins, owned items, crops, medals and unfinished guidance.
- Italian guidance uses packaged **it-IT-ElsaNeural** clips. Gameplay needs neither online speech generation nor a microphone.

See [Architecture](docs/ARCHITECTURE.md) for the main systems and [Assets and audio](docs/ASSETS_AND_AUDIO.md) for provenance and optional voice preparation.

## Build for Android

1. Open **File → Build Profiles**, select Android and switch the active platform if needed.
2. Confirm that `Assets/Scenes/Home.unity` is enabled in the scene list.
3. Check Player settings: identifier **`com.mrrabbit.home`**, landscape orientation, **IL2CPP** and **ARM64**. The minimum Android API is **25**; the target API uses Unity's automatic setting.
4. Build a local APK into **`Builds/Android/`**, using a versioned name such as `MrRabbit-0.10.0.apk`.

Signing keys and passwords stay outside Git. Store distribution needs separate signing and publication setup. iOS builds require macOS, Xcode and Apple signing; no iOS release is asserted here.

Read [Setup and builds](docs/GETTING_STARTED.md) before preparing a distributable build and follow the full [release verification requirements](docs/VERIFICATION.md).

## Checks and contribution

```sh
python tools/check_repository.py
```

The **Repository checks** GitHub workflow runs this same lightweight check on pushes and pull requests. It checks asset metadata and GUIDs, package pinning, the build scene and excluded local outputs. It does **not** compile Unity, run gameplay or certify a release. Existing `AgentScripts/Verify*.cs` helpers provide feature-specific Editor checks.

Contributions should keep school progression central, preserve saves and make interactions clear to children aged 6–8. Start with [CONTRIBUTING.md](CONTRIBUTING.md) and [AGENTS.md](AGENTS.md).

## Documentation

| Guide | Purpose |
| --- | --- |
| [Setup and builds](docs/GETTING_STARTED.md) | First import, Android configuration and troubleshooting |
| [Architecture](docs/ARCHITECTURE.md) | Runtime systems, scene organization and saved progress |
| [Assets and audio](docs/ASSETS_AND_AUDIO.md) | Drawings, generated art, offline narration and third-party content |
| [Verification](docs/VERIFICATION.md) | Repository checks, feature QA, save safety and release gates |
| [Development history](docs/DEVELOPMENT_HISTORY.md) | Preserved implementation notes and historical release evidence |

## Credits and licensing

Mr Rabbit grew from original drawings, developed into illustrated game assets with source prompts retained in `ArtSources`. Italian narration is synthesized during development and packaged for offline playback. Unity packages, TextMesh Pro resources, fonts and other third-party material retain their licenses and notices.

A project-wide reuse license has not been selected. Public availability alone does not grant a new license to the original code, drawings, artwork, characters or narration. See [Assets and audio](docs/ASSETS_AND_AUDIO.md) for provenance and retained notices.
