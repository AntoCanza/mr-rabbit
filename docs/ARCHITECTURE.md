# Architecture

## Project and scene

The repository root is a complete Unity project. `Assets/Scenes/Home.unity` is the single enabled gameplay build scene. Much of the illustrated world is implemented through uGUI views, images and custom graphics rather than conventional physics-based world geometry. URP uses the checked-in 2D renderer settings.

Runtime scripts are in `Assets/Scripts`, primarily in the `MrRabbit` namespace. `AgentScripts` contains development-only scene setup, inspection and regression helpers outside Unity's normal player compilation. Preserve this boundary.

## System map

| System | Main source files | Responsibility |
| --- | --- | --- |
| Character and home movement | `CharacterSelection`, `RabbitGraphic`, `RabbitWalker`, `HousePointerInput` | Rabbit choice, appearance, connected floor/stair paths and home input |
| Shared movement input | `RelativeDragGesture`, `OutdoorPointerInput`, `CityPointerInput`, `VisitPointerInput` | Gesture direction, release behavior and location-specific pointer routing |
| Countryside and friends | `OutdoorAdventure`, `Neighborhood`, `MeadowFriend`, `SharedMeadowLandscape` | Leaving home, footwear, scrolling landmarks, visits and residents |
| Day/night and framing | `DayNightCycle`, `SkyCycleGraphic`, `GameplayFraming`, `SafeAreaLayout`, `HomeEntranceAnchor` | Shared time, readable scenery, safe areas and doorway alignment |
| School and handwriting | `SchoolController`, `SchoolLesson`, `SchoolWordPicture`, `HandwritingPad`, `HandwritingRecognizer`, `SchoolFeedback` | Classroom flow, illustrated prompts, stroke input, independent classification and feedback |
| Learning progression | `LearningJourney`, `LearningLesson`, `LearningProgress`, `MedalGraphic` | Eleven skills, practice/checks, saved medals and reward progression |
| Garden and kitchen | `GardenController`, `GardenProgress`, `GardenArt`, `HomeActivities`, `KitchenVisuals`, `BedInteraction` | Crop cycle, ingredients, cooking/eating and home activities |
| City economy | `CityController`, `CityProgress`, `CitySymbol`, `RewardArt` | Train travel, produce orders, delivery, coins and outfits |
| Guidance and voice | `GameGuide`, `GameGuideProgress`, `ItalianVoice`, `ActionCue`, `GuideHandGraphic`, `GuideFocusGraphic` | Resumable introduction, contextual hints, offline speech and decorative cues |

Entries name C# files without the `.cs` suffix. Read the actual components and serialized scene bindings before changing a flow; UI and controller references are stored together in the scene.

## Progress and save compatibility

The game stores local progress in versioned `PlayerPrefs` keys, including:

| Key | Purpose |
| --- | --- |
| `MrRabbit.School.v1` | Classroom progress |
| `MrRabbit.Garden.v1` | Garden and crop progress |
| `MrRabbit.City.v1` | Coins, orders and owned/selected outfits |
| `MrRabbit.Learning.it-IT.v1` | Italian skill progression, earned medals and related rewards |
| `MrRabbit.Guide.it-IT.v1` | Introduction page and contextual guide state |
| `MrRabbit.Voice.Muted` | Narration mute preference |
| `MrRabbit.TV.Muted` | TV sound preference |

Use the constants in the source as the authority. Changes must preserve old saves, rewards, inventory and owned items. Replaying or skipping guidance must not grant rewards or reset learning. Guide state remains separate from progression.

## Content and narration

Original drawings, generation prompts, approved speech settings and audio preparation receipts live in `ArtSources`. Runtime sprites and audio live under `Assets`. `ItalianVoice` plays packaged clips; development-time generation uses the canonical lines and approved voice profile. See [Assets and audio](ASSETS_AND_AUDIO.md).

## Repository boundaries

- Commit runtime assets together with their `.meta` files and shared project/package settings.
- Keep `Library`, bulk captures, device logs, generated IDE files, build outputs and real-save snapshots local.
- Keep a small curated gallery in `docs/images`; these are copies of recorded 0.10.0 screens.
- The repository integrity workflow runs without a Unity license. Actual compilation, gameplay and release regression require the Editor and the appropriate device checks.
