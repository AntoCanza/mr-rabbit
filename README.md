# Mr Rabbit

A first 2D home scene for a children's tablet game, based on our house drawing.

## Play together

1. Open this project in Unity Hub with **Unity 6.3 LTS (6000.3.25f1)**.
2. Open `Assets/Scenes/Home.unity`.
3. Press **Play** at the top of Unity, then drag the rabbit with the mouse.
4. Press **Play** again to stop. The rabbit returns to its starting position next time.

The rabbit also accepts touch input. The first scene is designed for landscape screens.
If the Game view looks cropped, reduce its Scale slider to fit the window.

For a first activity together, choose a name for each room and decide what the rabbit
should do there. The house is currently one background picture; furniture interaction,
walking, stairs, sound, and objectives are future steps. The rabbit is a temporary
character drawn with simple shapes.

## Project contents

- `Assets/Scenes/Home.unity`: the starting scene and only enabled build scene.
- `Assets/Art/Home/House.png`: clean game art recreated from the original drawing.
- `ArtSources/Home.original.jpg`: preserved source drawing.
- `ArtSources/House.prompt.txt`: the built-in image-generation prompt and provenance.
- `Assets/Scripts/RabbitGraphic.cs`: the lightweight placeholder character.
- `Assets/Scripts/DraggableRabbit.cs`: mouse and touch dragging within the picture.
- `Assets/Scripts/SafeAreaLayout.cs`: safe-area layout and 60 FPS target.
- `AgentScripts/`: Unity CLI helpers; these are outside Assets and excluded from player builds.

This uses URP 2D, uGUI and the Input System. The background has no mipmaps or CPU-readable
copy and uses ASTC 6x6 mobile texture overrides. The scene has no post-processing or
physics simulation. Actual memory use and performance still need device measurements.

## Versions and builds

Local Git history starts on `main`; no remote is configured. Unity caches, generated
solutions, captures, and build outputs are ignored. Keep asset `.meta` files in Git.

Android is configured for ARM64 / IL2CPP. The application identifier is
`com.mrrabbit.home` for both Android and iOS, with version `0.1.0`; confirm the final
identifier before store setup. This prototype has not been built or tested on a phone
or tablet yet. Android builds can be made on this Windows PC; an iOS release will need
a macOS/Xcode build environment and Apple signing setup.

The official beta Codex integration uses `com.unity.pipeline` 0.8.0-exp.1. Target this
project explicitly in Unity CLI commands if another Editor is open.

## Initial verification

Checked in Unity Play mode on 8 October 2026:

- Compilation and Console: no errors or warnings after scene setup.
- Actual Game view sizes: 1440 x 1080 tablet and 2340 x 1080 landscape phone.
- Full house, title, character, and hint remain visible without distorting the art.
- Simulated Input System mouse and touch drags move the rabbit through the UI module.
- Dragging beyond the picture clamps the position; releasing restores normal scale.

These are Editor checks, not physical-device touch, performance, or store validation.
