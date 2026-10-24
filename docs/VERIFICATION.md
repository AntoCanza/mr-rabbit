# Verification and release checks

## Repository integrity

Run from a Git checkout with Python 3.10 or newer:

```sh
python tools/check_repository.py
```

The check uses only the Python standard library and Git. It verifies required Unity roots, valid and pinned package JSON, visible metadata and text serialization, the enabled Home build scene, asset/meta pairing, GUID uniqueness, and exclusion of local generated files and signing material. `.github/workflows/repository-checks.yml` runs it on pushes and pull requests.

This is a source integrity check. It does not launch or compile Unity, prove gameplay behavior, run every regression helper or validate a distributable release.

## Protect real saves first

Read [AGENTS.md](../AGENTS.md) before gameplay QA. Tests must preserve real progress. With Play mode stopped, the existing `AgentScripts/GuideSaveSafety.cs` helper can capture all current game/guide and mute keys into a fresh `Library/MrRabbitQA/guide-original-*.json` snapshot. Keep that returned path. After testing, stop Play mode and restore that same snapshot. Never commit it.

The older `ReleaseSaveSnapshot.cs` has narrower historical coverage and should not replace the current guide-aware backup. Some feature helpers also restore their own keys, but that does not remove the need for an encompassing backup.

## Existing feature checks

These are Editor evaluation sources under `AgentScripts`, not automatically discovered Unity Test Framework assemblies. Most need the open Home scene and some need Play mode. Read their public entry points and preconditions before running them through the compatible Editor scripting interface.

| Area | Representative helpers |
| --- | --- |
| Movement and home | `VerifyHomeWalking`, `VerifyHoldMovement`, `VerifyRelativeMovement`, `VerifyHomeExterior`, `VerifyHomeActivities` |
| World and friends | `VerifyMeadow`, `VerifyFriends`, `VerifyNeighborhood`, `VerifyDayNight` |
| Garden and city | `VerifyGarden`, `VerifyGardenTouch`, `VerifyCityTravel`, `VerifyCityTouch` |
| School and writing | `VerifySchool`, `VerifySchoolHints`, `VerifyWordPictures`, `VerifyChildWriting`, `VerifyFreeWriting` |
| Progression and guidance | `VerifyLearningJourney`, `VerifyGameGuide` |
| Tablet UI and presentation | `VerifyTabletExperience`, `ReleaseVisualPass`, `LearningVisualPass` |

Historical `Add*`, `Install*` and `Prepare*` helpers can modify scenes or reset versions. They are not a setup sequence or a regression suite to run indiscriminately.

## Before packaging or installing a release

The complete release requirements live in [AGENTS.md](../AGENTS.md). Cover every available gameplay feature, navigation route, input mode and persistence path; check old-save compatibility. Inspect rendered screens for missing/clipped art, overlapping UI, controls outside the screen and actual UI raycast/touch reachability.

Include 4:3, 16:10 and the connected tablet's real aspect ratio. Exercise both rabbits, footwear, home/stairs, day/night, friends/visits, crops/watering/harvest, cooking/eating, TV/sleep, train travel, market delivery, outfits, school practice/checks, handwriting rejection/uncertainty, all medal/reward paths and guide replay/pause/skip/resume/mute.

Restore original saves, fix discovered issues, and recheck affected paths. After installation verify the actual installed version, launch and device logs. Record the tested commit, resolutions, devices, results, fixes and remaining limits. Physical-device checks and simulated checks must be described separately.

## Historical evidence

[Development history](DEVELOPMENT_HISTORY.md) preserves the former README and its implementation and release notes. Those notes are historical records and include obsolete intermediate instructions. Use the current README and setup guide for present configuration. Historical evidence is not a new result for the current repository publication, and the full local capture/log archive is not included.
