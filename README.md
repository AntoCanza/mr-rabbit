# Mr Rabbit

A 2D home and meadow prototype for a children's tablet game, based on our drawings.

## Play together

1. Open this project in Unity Hub with **Unity 6.3 LTS (6000.3.25f1)**.
2. Open `Assets/Scenes/Home.unity`.
3. Press **Play** at the top of Unity. Both rabbits appear on the starting screen.
4. Click or tap **Mr Rabbit (Male)** or **Miss Rabbit (Female)** to enter the house.
5. Use **Left / Right** to walk along the current floor.
6. Near the bottom of the stairs, hold **Up** to climb. Near the top, hold **Down** to descend.
7. While on the stairs, **Up / Down** change direction. **Right / Left** also climb / descend.
8. Walk **Left** into the downstairs exit door, or tap **< Esci**. From upstairs, tapping the door walks down the stairs first.
9. Choose **Scarpe normali** or **Roller** to go outside. **Resta in casa** cancels the choice.
10. Keep going **Left** to explore the scrolling meadow. Roller skates are visible and move faster than walking.
11. Go **Right** to the **Casa >** sign (or tap it) to return home. The rabbit automatically wears normal shoes indoors.
12. Use **Choose rabbit** at the top right to return to the starting screen and switch characters.
13. Press **Play** again to stop. The next play session starts with the character choices.

You can also click or tap a floor or stair to walk there. The chosen rabbit follows the connected
walkways and uses the staircase to change floors. Dragging adjusts the destination;
arrow keys take over immediately. Both rabbits stop at the walls except for the downstairs left exit,
and cannot move through the floor/ceiling away from the stairs. The meadow also supports click/tap
destinations and has a left boundary. Every trip outside asks about footwear again.

The first scene is designed for landscape screens.
If the Game view looks cropped, reduce its Scale slider to fit the window.

For a first activity together, choose a name for each room and decide what the rabbit
should do there. The house is currently one background picture; the rabbit walks in
front of the furniture. Furniture interaction, sound, and objectives are future steps.
Both rabbits use transparent sprites adapted from our character drawings, with a small
walking sway and automatic left/right facing.

## Project contents

- `Assets/Scenes/Home.unity`: the starting scene and only enabled build scene.
- `Assets/Art/Home/House.png`: clean game art recreated from the original drawing.
- `ArtSources/Home.original.jpg`: preserved source drawing.
- `ArtSources/House.prompt.txt`: the built-in image-generation prompt and provenance.
- `Assets/Art/Characters/MrRabbit.png` and `MissRabbit.png`: the transparent character sprites.
- `ArtSources/MrRabbit.original.jpg` and `MissRabbit.original.jpg`: preserved character drawings.
- `ArtSources/MrRabbit.prompt.txt` and `MissRabbit.prompt.txt`: built-in image-generation prompts and provenance.
- `Assets/Scripts/CharacterSelection.cs`: character choices, gameplay gating, and switching back.
- `Assets/Scripts/RabbitGraphic.cs`: the character's sprite rendering and walking sway.
- `Assets/Scripts/RabbitWalker.cs`: connected floor/stair paths, arrow movement, and tap routing.
- `Assets/Scripts/HousePointerInput.cs`: mouse/touch destinations on those paths.
- `Assets/Scripts/OutdoorAdventure.cs`: exit, footwear choice, meadow movement/camera, and return with shoes.
- `Assets/Scripts/OutdoorPointerInput.cs`: mouse/touch destinations on the meadow.
- `Assets/Art/Outdoors/Meadow.png` and `Roller.png`: panorama and transparent skate sprite.
- `ArtSources/Roller.original.jpg`: preserved red skate/yellow wheel drawing.
- `ArtSources/Outdoors.prompts.txt`: built-in image-generation prompts for both new assets.
- `Assets/Scripts/SafeAreaLayout.cs`: safe-area layout and 60 FPS target.
- `AgentScripts/`: Unity CLI helpers; these are outside Assets and excluded from player builds.

This uses URP 2D, uGUI and the Input System. The background has no mipmaps or CPU-readable
copy and uses ASTC 6x6 mobile texture overrides. The scene has no post-processing or
physics simulation. Actual memory use and performance still need device measurements.
Each character texture is limited to 512 pixels on its longest side, uses mobile ASTC 6x6,
and has no mipmaps or CPU-readable copy. Full-resolution source PNGs remain on disk.
Menu portraits and the player share these sprites. The house is inactive until a choice is made.
The lawn uses one panorama (maximum 2048 pixels) and reuses one 512-pixel skate sprite for
both feet and the selection card. Both use ASTC 6x6 mobile overrides without mipmaps or
CPU-readable copies. Only the current home/meadow view is active; there is no physics simulation
or per-frame object creation for this feature.

The walkways use coordinates relative to the picture, so they resize with the house.
Select **Player Rabbit** under **House Viewport / House** in the saved scene (or the chosen
character while playing), with the house active and Scene view Gizmos enabled, to see the
green floor paths and orange stair path. The Rabbit Walker component exposes walking
speed, wall limits, landing reach, and stair points for later adjustments.

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

## Movement verification

Checked in Unity Play mode on 8 October 2026:

- Compilation and Console: no errors or warnings after scene setup.
- Actual Game view sizes: 1440 x 1080 tablet and 2340 x 1080 landscape phone.
- Full house, title, character, and hint remain visible without distorting the art.
- Automated movement checks cover both outside walls on both floors, blocked vertical
  movement away from stairs, ascent, descent, mid-stair reversal, and large frame times.
- Tap routes use the stairs between floors, stop at stair destinations, and never teleport.
- Out-of-bounds destinations stay on walkways, and arrow input cancels a tap route.
- Simulated Input System arrow keys climb the stairs and walk to a wall in real Update frames.
- A simulated touch on the phone layout routes from upstairs to a downstairs destination.

To repeat the movement-rule checks while Home is playing, first choose a character, then use the Unity CLI's
`run_script` command with file `AgentScripts/VerifyHomeWalking.cs` and entry
`VerifyHomeWalking.Run`.

These are Editor checks, not physical-device touch, performance, or store validation.

On 9 October 2026, the character was replaced with Miss Rabbit. The movement-rule checks
passed again; the transparent sprite, preserved proportions, left/right facing, floor and
stair placement were checked in the 1440 x 1080 Game view. The Console had no errors.

The character-selection flow was also checked on 9 October 2026 at tablet (1440 x 1080)
and landscape-phone (2340 x 1080) sizes. Both portraits are visible; the house stays inactive
before selection. A simulated mouse click chose Mr Rabbit; a simulated touch returned to
the menu and chose Miss Rabbit. Switching cancels any walk destination and resets the
new character downstairs. Movement-rule checks passed for both choices, with no Console errors.

On 15 October 2026, `AgentScripts/VerifyMeadow.cs` (`VerifyMeadow.Run`) checked both rabbits
with both footwear choices: upstairs door routes use stairs, the upper left wall does not exit,
the footwear menu blocks movement, cancel returns safely, left travel scrolls the meadow,
the left boundary clamps movement, taps work, keys override tap destinations, and returning
removes rollers and resumes walking indoors. Changing rabbits clears outdoor state.
`VerifyHomeWalking.Run` passed with the new downstairs exit behavior.

Input System events also verified left-arrow exit, mouse selection of rollers, right-arrow
return with automatic shoes, touch on the exit door, touch selection of shoes, and touch on
the return sign. The live frame counter advanced. Tablet (1440 x 1080) and landscape phone
(2340 x 1080) views were inspected. Compilation and Console finished without errors or warnings.
These checks run in the Editor; no physical-device build or performance measurement was made.
