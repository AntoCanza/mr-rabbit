# Mr Rabbit

A 2D home and meadow prototype for a children's tablet game, based on our drawings.

## Railway, city and continuous countryside — 18 October 2026

**0.8.2 (13)** simplifies movement at home, outside, in friends' houses and in the city:
touch any free background area, move the finger in the desired direction, keep holding
to walk, and lift to stop. Direction follows finger motion, regardless of the rabbit's
position; reversing needs only a small opposite motion, without crossing the touch origin.
At home, vertical gestures use the stairs near their landings. Tiny finger jitter and
stationary touches are ignored, second fingers cannot steal movement, and action icons
retain their normal taps. `VerifyRelativeMovement.cs` covers these cases, stairs, release
and icon takeover; the held-movement and actual-UI touch checks also pass at tablet sizes.
The combined 0.8.2 APK built with zero errors and was installed in place on the Galaxy
Tab A7 Lite (package version code 13). Actual touch swipes on empty space to the left
of the rabbit moved right and left as intended; the running app error log was empty.

Economy update **0.8.1 (12)**: outfits cost 60 coins; every delivered crop unit pays
exactly 1 coin (3 products → 3 coins, 6 products → 6 coins). Existing accepted requests
are repriced without losing their product/quantity, saved coins or owned outfits.

Version **0.8.0 (11)** adds a countryside station left of school, a visual two-stop map,
and a six-second train journey with the selected rabbit visible through the window.
The city supports walking, a produce buyer, and a clothing boutique. Accept one order,
collect the indicated quantity, and deliver only that crop for coins. Unaccepted offers
rotate each game-day; accepted orders remain until fulfilled. Coins, ownership, selected
outfits and accepted orders persist. Each rabbit has one purchasable outfit (60 coins),
including matching seated artwork for home and school; owned outfits can be worn again free.

The countryside now uses **one repeating mirrored landscape texture** for sky, hills,
grass and path across all 6.5 viewport widths. Cottages, garden, school and station are
transparent landmark layers. The player's own exterior house is visible with a door cue.
The garden gate has a larger contextual animated picture; growth, thirst and harvest notices
use pictures only. The kitchen uses ingredient/dish pictures, counts, a pot and a plate,
with the available next action highlighted.

New artwork and provenance: `Assets/Art/City/`, `Assets/Art/Outdoors/SharedLandscape.png`,
the four `*Cutout.png` landmarks, and `ArtSources/CityAndLandscape.prompts.txt`.
Original art and saves are preserved. Discarded outfit layout drafts live in `ArtSources/CityDrafts/`.

Verification: `VerifyCityTravel.cs` checks both rabbits/footwear, map cancellation, timing,
transactions, insufficient produce/funds, duplicate delivery protection, persistence,
free re-equipping, seated outfits, return and home entry; it restores original saves.
`VerifyCityTouch.cs` checks actual UI raycasts/pointer events for kitchen, stations, map,
market and boutique at **1340×800, 1920×1200, 2048×1536, 2266×1488 and 2340×1080**.
Existing neighborhood, home activity, garden and school suites also passed.
These are representative aspect-ratio/layout checks, not tests on every tablet model.

APK: `Builds/Android/MrRabbit-0.8.0.apk`.
Build succeeded with zero errors (59,025,173-byte APK). Installed in place on the connected
Samsung SM-T220 / Galaxy Tab A7 Lite; package confirms 0.8.0 / code 11. Device touch checks
covered character/footwear choice, countryside train boarding, the map, city arrival,
walking and opening the produce market and clothing boutique. Existing produce remained
present. The running app's filtered Unity/AndroidRuntime logs contained no errors.

## Neighborhood update — 18 October 2026

Version 0.3.0 adds the home camera follow, Coccodella and Zebrella's visitable cottages,
random solo/shared outings, and a Mr Rabbit face launcher icon. The original artwork is
preserved. New art is in `Assets/Art/Outdoors/Neighborhood.png`, `CoccodellaInterior.png`,
`ZebrellaInterior.png` and `Assets/Art/Characters/MrRabbitIcon.png`. Built-in image-generation
prompts and references are recorded in `ArtSources/Neighborhood.prompts.txt` and
`ArtSources/MrRabbitIcon.prompt.txt`.

`AgentScripts/VerifyNeighborhood.cs` checks both characters and both footwear choices,
unchanged home zoom while following upstairs, both visits and exits, indoor movement,
the clock during visits, all three outing groups, returns, and the absence of an outdoor
resident from her home. Passed at 1340×800 and 2340×1080 in live Editor Play mode;
the existing home-walking and meadow suites also passed. The player loop was confirmed
advancing. `PreviewNeighborhood.cs` provides repeatable visual preview states.

Built `Builds/Android/MrRabbit-0.3.0.apk` (39,194,826 bytes) successfully with zero build
errors and installed it as an in-place update on the connected Samsung Galaxy Tab A7 Lite
(SM-T220, Android 14). Package metadata confirms version 0.3.0 / code 3. Device touch checks
covered character selection, climbing to bed and sleeping with the camera following,
waking/descending, the exit and roller selection, both cottage visits, and returning to
the meadow. Random resident walking was observed on the device; absent hosts were correctly
hidden inside. The new face icon was visually verified in Samsung's app drawer and used to
reopen the game. Filtered Unity/AndroidRuntime error logs for the running app were empty.
This is a functional device check, not a sustained performance or battery benchmark.

## Play together

1. Open this project in Unity Hub with **Unity 6.3 LTS (6000.3.25f1)**.
2. Open `Assets/Scenes/Home.unity`.
3. Press **Play** at the top of Unity. Both rabbits appear on the starting screen.
4. Click or tap **Mr Rabbit (Male)** or **Miss Rabbit (Female)** to enter the house.
5. Use **Left / Right** to walk along the current floor.
6. Near the bottom of the stairs, hold **Up** to climb. Near the top, hold **Down** to descend.
7. While on the stairs, **Up / Down** change direction. **Right / Left** also climb / descend.
8. Walk or tap the downstairs floor near the left door to leave automatically. Tapping the door still routes there from either floor.
9. Choose **Scarpe normali** or **Roller** to go outside. **Resta in casa** cancels the choice.
10. Keep going **Left** to explore the scrolling meadow. Roller skates are visible and move faster than walking.
11. Approach the right-hand house entrance to return automatically. The rabbit wears normal shoes indoors and appears far enough inside to avoid immediately exiting again.
12. Tap the upstairs bed to walk there via the stairs and sleep. Tap it again to wake, or tap another floor destination to wake and walk. Character selection is shown only when the game starts; there is no in-game change-character button.
13. Press **Play** again to stop. The next play session starts with the character choices.

You can also click or tap a floor or stair to walk there. The chosen rabbit follows the connected
walkways and uses the staircase to change floors. Dragging adjusts the destination;
arrow keys take over immediately. Both rabbits stop at the walls except for the downstairs left exit,
and cannot move through the floor/ceiling away from the stairs. The meadow also supports click/tap
destinations and has a left boundary. Every trip outside asks about footwear again.

The game fills landscape screens with enlarged, proportionally cropped scenery. The home camera now follows the rabbit smoothly on both axes while keeping the existing scale; it stops at the edges of the painting. Climbing reveals the bedroom, and descending reveals the ground floor. Gameplay has no persistent title bar, time badge, character-switch button or footer instructions.
If the Game view looks cropped, reduce its Scale slider to fit the window.

Two friends have their own houses in the meadow: **Zebrella**, a zebra in a lilac dress, and
**Coccodella**, a green crocodile in a blue dress. Tap either cottage to walk to its door and visit.
Their interiors have distinct lilac/striped and green/water-lily themes. Tap the floor to walk,
tap the host to greet her, and tap the left door or walk left to return to the lawn.
Rollers remain outside and are restored when returning to the lawn.
Both friends begin inside. After 8–15 seconds of active home/meadow play, random events every
12–23 seconds can send Coccodella, Zebrella, or both outside. Outings last 14–26 seconds before
the walk back to their own doors. Residents pause to greet, and their whereabouts persist
between home and meadow; a friend who is outside is absent from her interior. Outing timers
pause during visits, footwear/character choices and app suspension.
Click or tap a friend or her name to see an Italian greeting and a small friendly sway.
The greeting lasts 4.5 seconds; tapping again restarts it. You can keep walking during
the greeting. The bubble stays within the viewport and closes when the friend goes
offscreen, you return home, or you change rabbits.

Day, sunset, night and dawn now alternate automatically. One full cycle takes **120 seconds**
of play: 48 seconds of daylight, 12 of sunset, 48 of night and 12 of dawn. A new Play session
starts partway through daylight, so the first night arrives after roughly 46 seconds of play.
Lighting indicates the current period without a time label. Sun, moon and stars appear in
the house sky and over the meadow. Scenery changes gently while characters and controls
remain readable. The same clock continues between the house and meadow; it pauses during
character/footwear choices and app suspension. Sleeping leaves the clock running.

For a first activity together, choose a name for each room and decide what the rabbit
should do there. The house is currently one background picture; the rabbit walks in
front of the furniture. The upstairs bed supports sleep and wake; other furniture interactions, sound, and objectives are future steps.
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
- `Assets/Scripts/DayNightCycle.cs`: shared clock, smooth light transitions and menu/suspension pauses.
- `Assets/Scripts/SkyCycleGraphic.cs`: vector sun, crescent moon and stars in one UI mesh per location.
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
Day/night reuses the existing paintings with color tints and two small UI meshes. It adds
no bitmap textures, dynamic lights or post-processing. Sky/tint updates are limited to 20 Hz;
movement still targets 60 FPS. Adjust **Cycle Seconds** on **Safe Area / Day Night Cycle** to
change the cycle duration. Device performance still needs measurement on actual hardware.

The walkways use coordinates relative to the picture, so they resize with the house.
Select **Player Rabbit** under **House Viewport / House** in the saved scene (or the chosen
character while playing), with the house active and Scene view Gizmos enabled, to see the
green floor paths and orange stair path. The Rabbit Walker component exposes walking
speed, wall limits, landing reach, and stair points for later adjustments.

## Versions and builds

Local Git history starts on `main`; no remote is configured. Unity caches, generated
solutions, captures, and build outputs are ignored. Keep asset `.meta` files in Git.

Android is configured for ARM64 / IL2CPP. The application identifier is
`com.mrrabbit.home` for both Android and iOS, with version `0.3.0` (Android version code 3); confirm the final
identifier before store setup. Version 0.1.0 was installed and launched on a Samsung Galaxy Tab A7 Lite (SM-T220, Android 14) on 17 October 2026. Android builds can be made on this Windows PC; an iOS release will need
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

Day/night was verified on 15 October 2026 with `AgentScripts/VerifyDayNight.cs`
(`VerifyDayNight.Run`): full-cycle timing and wrap, gradual color at all period boundaries,
readable characters, sky graphics that do not intercept taps, and clock preservation/pause
across location changes, both menus, character changes and application suspension. Live
Update frames also crossed sunset into night and dawn into day. A simulated mouse click
on the exit door worked at night. Both home/meadow movement suites still passed.
Day, sunset and night were visually checked at tablet/phone sizes. The cream UI retains
its contrast throughout the transitions. Compilation and the final Console had no errors
or warnings. No publication or physical-device validation is implied by these Editor checks.

## Meadow friends

Added on 17 October 2026. Both portraits are new designs generated with the built-in
image generator, using Miss Rabbit as a visual style reference. Their source PNGs are
in `Assets/Art/Characters/`; the exact prompts are in `ArtSources/Friends.prompts.txt`.
Each sprite imports at a maximum of 512 pixels with Android/iOS ASTC 6x6, without
mipmaps or a CPU-readable copy. The feature adds no physics, Animator or particles.

`MeadowFriend` handles greetings; the `Meadow Friends` layer scrolls with the landscape
and renders above the sky decorations. `AgentScripts/AddFriends.cs` is a guarded
one-time scene setup helper. `AgentScripts/VerifyFriends.cs` checks both rabbits with
shoes and rollers, greeting timing/repeats, night readability, movement during greetings,
and cleanup on returning home or changing characters.

Verified in Unity Play mode at 1440 x 1080 and 2340 x 1080: actual Input System mouse
and simulated touch events opened the correct greetings without moving the rabbit.
Bubble bounds were checked at both edges, live frames confirmed timeout and offscreen
cleanup, and both names and portraits were inspected by day and night. Existing meadow
and day/night checks passed. Compilation and the final Console had no errors or warnings.
These are Editor checks; physical-device performance and publishing remain separate work.

## Tablet update 0.2.0 — 17 October 2026

Gameplay now fills the display: `GameplayFraming` crops decorative margins while preserving
art proportions and walkway coordinates. Character selection appears at startup only.
`BedInteraction` routes either rabbit up the stairs, shows a breathing horizontal sleeping
pose on the painted bed, and wakes on a second bed tap or a new floor destination.
Door proximity works with ordinary floor taps and arrow movement. Return/cancel positions
prevent an immediate second exit. Shoes/rollers selection remains part of leaving home.

Editor checks passed at 1340x800, 1440x1080 and 2340x1080, including visible floor/head bounds.
`VerifyTabletExperience.Run` checks both characters, bed hit testing, stair routing, sleep,
wake/cancel, proximity transitions, and no doorway bounce. Existing home, meadow, friends
and day/night suites also passed. The Android ARM64/IL2CPP build succeeded with zero errors.

Installed version 0.2.0 (code 2) over 0.1.0 on Samsung SM-T220 / Android 14. Device input
verified startup selection, fullscreen home, tapping the bed to walk upstairs and sleep,
tapping the floor to wake and approach the exit, rollers selection, and returning by
approaching the house without tapping its sign. No Unity/AndroidRuntime errors appeared
in the app's log during these checks. Captures are stored in `Captures/Android/v020-*.png`.

## Neighborhood and garden — 0.4.0, 18 October 2026

The home camera follows the rabbit upstairs without changing the artwork's proportions.
Coccodella and Zebrella have separate visitable homes and randomly walk outside, individually
or together. The Android launcher icon uses Mr Rabbit's face.

The meadow now extends left to a distant vegetable garden. Touch its gate icon to walk there
and enter the separate garden view. Three independent beds follow a pictorial guide: take
the hoe, prepare a bed, select carrot/wheat/tomato/strawberry seeds, plant, water repeatedly,
then take the basket and harvest. Each harvest gives three items and resets that bed to
unprepared soil. Tools, beds, entrances, greetings, sleep and menu choices have visual icons;
labels are secondary. New artwork uses simple crayon/pencil drawings inspired by the original
rabbit. Sources and exact prompts: `Assets/Art/Garden/` and `ArtSources/Garden.prompts.txt`.

Growth consumes the shared 120-second game-day cycle. Water lasts 0.4 game-days; carrots
mature in 1 day, wheat/strawberries in 1.25, tomatoes in 1.5. Dry plants pause without dying.
Seed, sprout, growing and ripe states are shown. A clickable meadow notification indicates
new growth, thirst or harvest; simultaneous water/harvest needs have both icons. All beds,
water, growth and harvested counts persist in `MrRabbit.Garden.v1` PlayerPrefs. Progress is
saved after actions, every five seconds of growth and on suspension. Closed-app time does
not grow plants. Selection and footwear menus pause the game clock.

`VerifyGarden.Run` checks every crop, watering limits, dry pauses, independent beds, saving,
harvesting/reset, both rabbits/footwear, travel, all notifications and menu pause.
`VerifyGardenTouch.Run` awaits rendered frames and verifies UI raycasts/pointer events for
sleep, the garden gate, tools, bed badges, seed pictures, exit and notices. These passed at
1340x800 and 2340x1080. `VerifyNeighborhood.Run` also passed after meadow expansion.
The live frame counter advanced and the final Editor console had no runtime errors.

Scene authoring helpers are outside Assets: `AddGarden.Run` is guarded one-time setup;
`PolishGarden.Run` applies final atlas bounds, soil painting and icon layout to that scene.
`GardenArt` samples individual organic drawing bounds rather than assuming a rigid grid.

## Unified illustration style and action highlights — 0.5.0, 18 October 2026

The original `Assets/Art/Home/House.png` is now the visual reference for all active artwork:
warm brown outlines, soft painted pastel fills and simple picture-book forms. The four
characters keep their original silhouettes, clothes and colors. Both friends' interiors,
the neighborhood, garden scenery, soil, tools, crops, action pictures, rollers and launcher
icon have been restyled using the built-in image generator. The original house is unchanged.
Exact prompts are in `ArtSources/UnifiedStyle.prompts.txt`; the previous images are retained
under `ArtSources/BeforeStyleUpdate/`. `GardenArtSlot` preserves icon identity across atlas
updates and `GardenArt` uses the new drawings' measured bounds.

`ActionCue` replaces the garden's guiding arrow with a warm pulsing highlight and gentle
picture movement. Only the current step is emphasized: tool, then target bed; the seed
selection presents four equally valid choices. Cues stop during walking/working and switch
after completion. Harvest returns the guide to soil preparation. Other clickable pictograms
have subtle idle movement; garden controls stay quiet unless they are the next step.
The touch targets remain stationary. `PrepareActionCues.Run` configures the existing UI
without replacing it; the legacy guide arrow stays inactive.

`VerifyGarden.Run` passed with the new art. `VerifyGardenTouch.Run` also checks exclusive
highlights, actual picture animation, quiet unrelated actions, no guiding arrows, and the
complete hoe/seed/water/harvest/restart interaction sequence through real UI raycasts.
Both 1340x800 and 2340x1080 passed in fresh Play sessions. Preview helpers intentionally
disable world updates for screenshots; restart Play before running interaction checks.

The Android build succeeded with zero errors and the expected warning that the Editor's
Pipeline tooling is disabled in players. `Builds/Android/MrRabbit-0.5.0.apk` was installed
over the existing app on Samsung SM-T220 (Android 14), and version 0.5.0/code 5 was confirmed.
The saved carrot bed survived the update. On-device touches verified the water-tool-only
highlight, its transfer to the bed, watering, real-time ripening, the clickable harvest
notice in the meadow, basket pickup, harvest (count 3), and restart with the hoe highlighted.
No Unity/AndroidRuntime errors were logged during these checks. Device screenshots are
stored in `Captures/tablet-050-*.png`; Editor art previews are in `Assets/Captures/*-050*.png`.

## Home activities — 18 October 2026

Tap the armchair to walk over and watch television with a dedicated seated pose for
Mr Rabbit or Miss Rabbit. The screen cycles through three landscape programmes every
three seconds; tapping the television changes the programme immediately. An original
quiet synthesized jingle plays while watching. Audio can be muted (preference saved),
and Alzati, floor movement, the bed and the exit door release the activity and stop sound.

The new painted cooking station beside the fireplace opens four recipes: carrot soup,
warm bread, tomato soup and strawberry compote. One harvested item makes one dish after
three seconds. Mangia carries a prepared dish upstairs to the existing dining table,
where the selected rabbit sits and eats. Harvests and prepared dishes share the garden
save; older saves retain their harvests and start with no prepared dishes. Ingredients
and meals are consumed only on completed actions, so cancelling does not lose food.

HomeActivities owns these mutually exclusive interactions. GardenProgress stores dishes
and validates inventory transactions; GardenController commits successful changes.
AddHomeActivities.Run is guarded one-time scene authoring. Generated transparent art is
in Assets/Art/Home/{MrRabbitSeated,MissRabbitSeated,CookingStation}.png. Built-in image
generation prompts and reference provenance are in ArtSources/HomeActivities.prompts.txt.

Verified in Unity Play mode: both characters, every harvest/cook/eat recipe, persistence,
old-save migration, empty inventory, repeated taps, cancellation and bed/door/selection
transitions. VerifyHomeActivities.CheckTouch passed at 1340x800 and 2340x1080 after waiting
for rendered UI frames. Existing VerifyTabletExperience.Run passed for both characters.
TV, cooking/steam and seated dining were visually inspected; after the renderer fix,
compilation and the final runtime checks reported no new errors. Screenshots are under
Assets/Screenshots/home-*.png. This change has not been built or installed on Android.

## Tablet installation — 0.6.0, 18 October 2026

Built Builds/Android/MrRabbit-0.6.0.apk (44,108,971 bytes) successfully with zero errors.
Warnings concern disabled Pipeline player tooling and legacy/round launcher icon deprecation.
Installed as an in-place update on Samsung SM-T220; Android confirms version 0.6.0/code 6.
Launch and device touches verified character selection, kitchen recipes and seating at the TV.
The pantry displays the existing harvests (6 carrots, 3 wheat, 3 tomatoes, 3 strawberries);
no ingredients were consumed during device verification. No Unity/AndroidRuntime errors were
reported for the running app. Screenshots: Captures/Android/v060-{launch,home,kitchen,tv}.png.
The app was left open with Miss Rabbit watching television.

## School and finger handwriting — 0.7.0, 18 October 2026

The meadow now spans 5.5 viewport widths (previously 3.5). Existing houses and the
garden retain their world positions; a school sits beyond the garden. The animated
book icon leads there. Pack a backpack, snack and water bottle at home or on arrival.
Mr/Miss Rabbit sits with Coccodella and Zebrella, listens to the owl teacher, and
chooses missing vowels, addition or subtraction. The teacher visits all three desks
and delivers worksheets. A pencil enables finger writing, an eraser clears it, a
book reveals a faint model, and the check mark submits. Correct answers award a
saved star; mistakes invite another attempt without penalties. Actions use pictures.

SchoolLesson generates varied Italian vowel questions and arithmetic with results
0–10, including two separate writing boxes for 10. HandwritingPad records strokes;
HandwritingRecognizer independently matches offline shape templates for vowels and
digits, with uppercase/lowercase vowel variants, flexible stroke order/direction,
and rejection of ambiguous marks. This is a small teaching recognizer, not general
handwriting OCR. School progress has its own versioned save and does not consume
garden inventory. Lessons pause the world clock.

Art was created with built-in image generation and saved in Assets/Art/School;
the complete prompts and reference provenance are in ArtSources/School.prompts.txt.
AddSchool is guarded one-time scene authoring. VerifySchool checks 1,500 generated
questions, 27 distorted/reversed/reordered glyph variants, blank/scribble rejection,
both pupils, packing, travel, sitting/listening cues, delivery, UI raycasts and finger
events, erasure, wrong-answer rejection, single rewards, and return to the meadow.
The interaction checks passed at 1340x800 and 2340x1080; after cue/layout adjustments
they passed again at tablet resolution. VerifyHomeActivities, VerifyNeighborhood,
VerifyGarden and VerifyTabletExperience also passed. The latter now waits for UI
frames after selecting a character before checking the bed raycast, avoiding a
stale-layout result. Compilation has zero errors.
Editor previews: Assets/Captures/school-*.png.

Android release: Builds/Android/MrRabbit-0.7.0.apk (54,860,017 bytes), built successfully
with zero errors and the expected Pipeline-player warning. Installed with an in-place
update on Samsung SM-T220; Android reports version 0.7.0/code 7. Device touches verified
packing all three items, the school shortcut and extended meadow route, sitting,
listening, subject choice and worksheet delivery. Android touchscreen strokes drew
an incorrect I for NAS_ (rejected), erased it, and drew O (recognized and rewarded).
A two-stroke handwritten 8 answered 7 + 1 and earned the second star. No Unity or
AndroidRuntime errors were logged for the running app. Existing garden data remained
present after the update; no harvests or dishes were consumed during device checks.
The tablet was left open in class at the two-star reward. Device captures are in
Captures/Android/v070-*.png, including handwritten-o, handwritten-8 and math-reward.

## Continuous touch movement — 0.7.1, 18 October 2026

Meadow movement now uses a captured hold instead of repeatedly setting a destination.
Previously a stationary finger stopped generating drag events, so the rabbit reached
the last world point and stopped even while the screen continued to be held.
Touch the rabbit, slide left/right and keep holding to move continuously; move across
the initial touch point to reverse, or lift to stop. A small translucent arrow below
the feet shows the active direction. A brief background tap is inert; holding beside
the rabbit also walks in that direction after a short intent delay. Action icons
retain their automatic routes to the school, garden and homes. No new text UI.

OutdoorPointerInput owns one pointer, ignores secondary fingers, captures the rabbit
above background buttons, and releases on pointer-up, cancellation, disable, loss of
focus or app pause. OutdoorAdventure combines the held direction with its existing
bounded movement/camera model; action routes supersede manual control safely.
MovementDirectionGraphic is a non-interactive procedural UI pictogram.

VerifyHoldMovement passed at 1340x800 and 2340x1080 for both rabbits and footwear:
real character raycasts, ten seconds of stationary holding across scrolling, reversal,
neutral zone, immediate stop, second-finger isolation, background hold, pause/focus,
world edge and school/garden routes. VerifySchool also passed after the change.
Editor visual check: Assets/Captures/hold-direction.png.

Built Builds/Android/MrRabbit-0.7.1.apk (54,863,413 bytes) with zero errors and installed
in place on Samsung SM-T220; Android confirms 0.7.1/code 8. A single touchscreen DOWN
and MOVE followed by a stationary six-second hold kept scrolling from the homes to
the garden. A MOVE across the original point reversed direction without lifting;
UP removed the arrow and two later captures confirmed the world position stayed fixed.
Garden entry/exit and the school shortcut also passed using their existing icons.
No Unity/AndroidRuntime errors appeared. Saved garden inventory and school kit remained
available. Captures/Android/v071-{hold1,hold2,reverse,release,stopped,garden,school}.png
record the device checks. The app was left in the meadow outside the school.

## Illustrated vocabulary — 0.7.2, 18 October 2026

Every one of the 24 missing-vowel words now has a large matching drawing beside the
handwriting box. SchoolWordPicture maps the complete Italian word to a measured atlas
rectangle, so changing the missing vowel cannot change the subject. Pictures remain
non-interactive; pencil, eraser, help and submit retain their icons and touch targets.
The handwriting box keeps its previous size. Arithmetic keeps its centered one/two-box
layout and clears the vocabulary illustration. No additional labels or written hints
were added to gameplay.

Created with built-in image generation using SchoolIcons.png as a style reference.
Final assets: Assets/Art/School/WordPictures1.png and WordPictures2.png.
Full final prompts: ArtSources/WordPictures.prompts.txt. All subjects and their cropped
in-game display were visually inspected in Assets/Captures/word-pictures-all.png;
Assets/Captures/word-picture-apple.png shows the worksheet layout.

VerifyWordPictures passed: all vocabulary entries, 400 randomized questions, image
clearing for arithmetic, subject transitions and no overlap with handwriting at
1340x800 and 2340x1080. VerifySchool passed including actual writing-area raycasts,
finger strokes, erasing and correct/incorrect answer rewards after the layout change.
The recognized-letter feedback appears above the vowel writing box, clear of the
help-book icon; math feedback retains its previous location. The overlap regression
check passed after this final adjustment.

Final Android build: Builds/Android/MrRabbit-0.7.2.apk, 59,262,713 bytes, zero errors.
Installed in place on Samsung SM-T220 as 0.7.2/code 9. Device verification showed the
sea drawing for M_RE; three touchscreen strokes wrote A, recognized it and earned a
star. The final layout refinement was rebuilt and reinstalled, then the illustrated
worksheet was opened again. No Unity/AndroidRuntime errors were logged. Existing kit,
stars and garden save were preserved. Captures: Captures/Android/v072-*.png.

## 0.7.3 — school help and audible feedback

The answer book is hidden until four incorrect handwritten submissions on the same
exercise. Empty answers and repeated confirmation without fresh ink do not count.
The book shows a one-star cost before use; first use deducts once, floors stars at
zero, and disqualifies that exercise from a reward. Assisted completion shows a
checkmark. A brief owl-teacher head shake and descending chime mark use of help.
The reminder closes automatically; no extra prose or dialog confirmation is added.
School listening, retries and completions now have synthesized offline chimes.
TV volume increased from 0.12 to 0.40; its existing mute control is preserved.

Audio diagnosis: tablet media volume 7/15, unmuted, routed to speaker. Unity audio
was enabled with one active listener and master volume 1. Previously only TV had
sound; school had no audio. Physical speaker audibility cannot be assessed remotely.
VerifySchool and VerifySchoolHints exercised both pupils and all three subjects,
including blank/duplicate rejection, the four-attempt threshold, repeated help,
zero-star floor, and assisted completion without rewards. VerifyHomeActivities
passed TV playback, mute/unmute and activity transitions. Teacher reminder and help
cost were visually checked in the Game view.

Release APK: Builds/Android/MrRabbit-0.7.3.apk (59,267,557 bytes), build succeeded
with zero errors; installed with -r on SM-T220 as version 0.7.3 / code 10.
On-device TV inspection additionally found the persisted TV mute preference ON
(button displayed Audio: no). Toggled it to Audio: si and verified the new state.
Android reported com.mrrabbit.home's AAudio session started, unmuted, deviceId 2;
master mute off and volume 1. Existing saves retained. Device captures v073-tv*.png.
Device handwriting check: MEL_ with apple illustration; wrote I and submitted three
separate times, including duplicate confirm taps. Book remained hidden. After erasing
and submitting a fourth I, book appeared with one-star cost. Erased the test ink and
left worksheet ready; did not consume the user's stars. No Unity/AndroidRuntime errors.
Captures: Captures/Android/v073-three-attempts.png and v073-four-attempts.png.

## 0.8.3 — picture-led interface, ages 6–8

School now offers only vowels and addition. The subtraction subject, generator,
button and listeners have been removed; invalid subject indices are rejected.
The 24 illustrated Italian vocabulary words, handwriting, rewards and help remain.

The garden uses highlighted tools and plot pictures instead of written steps,
bed-status sentences and crop/tool labels. Harvest counts use a basket and number,
positioned separately from the coin wallet. Footwear choices show a large shoe or
roller. Selection retains the title and character names; instructions and gender
labels are hidden. Greetings show a waving hand, TV controls use back and speaker/
mute symbols, and meal feedback uses a checkmark. Kitchen cards retain the existing
ingredient-to-dish pictures and counts, with no redundant prose.

This release retains the train journey/city, wardrobe at 60 coins, produce at one
coin per unit, and finger-relative movement introduced in 0.8.0–0.8.2.
No additional language is enabled yet. Future localization should translate the
remaining navigation labels and provide per-language vowel words, valid answers
and matching illustrations, rather than mechanically translating Italian exercises.

Verification before Android packaging: 1,000 randomized school questions, glyph
recognition and wrong/blank rejection; two pupils/two subjects and removed-subject
guard; four-attempt help; complete garden/cooking/eating/save flows; TV mute and
activity transitions; visual greetings and neighborhood visits. Real UI raycasts
passed for school, kitchen, train map, market and wardrobe at 1340x800, 1920x1200
and 2048x1536. Illustrated vocabulary coverage and math/vowels transitions passed.
Final garden, selection, footwear, kitchen and school captures were visually checked.
Reproduction script: AgentScripts/SimplifyVisualUI.cs (edit mode, idempotent).
Android target: Builds/Android/MrRabbit-0.8.3.apk, version code 14.

Full release regression additionally found and fixed three functional/visual bugs:
the shared countryside background now receives gestures throughout the expanded
world; garden action/notice canvases stay above the rabbit and accept taps when
standing at the gate; the return train mirrors its artwork without negatively
scaling the passenger's RectMask2D, so the rider remains visible both ways.
Station/shop highlights now surround their action pictures instead of creating a
large yellow rectangle over the building.

Handwriting now smooths small wobble, samples each path at uniform distances,
normalizes two-dimensional proportions while preserving line-like I/1 strokes,
and compares small tilt/slant corrections. Shape coverage, relative path length,
aspect and ambiguity checks still reject uncertain ink. Recognition never receives
the expected exercise answer. HandwritingPad caches results until ink/mode changes,
avoiding repeated matching for unchanged handwriting. No remote recognition/service
or collected handwriting is involved.

VerifyChildWriting's deterministic 324 varied samples improved from 262 correct,
6 misclassified and 56 uncertain to 316 correct, 1 misclassified and 7 uncertain.
VerifyFreeWriting recognizes 15 separately drawn examples (open loops, soft corners,
detached bars). These are synthetic/developer examples, not measured child accuracy;
real-child variation still needs playtesting. Blank/dot/horizontal-bar/scribble
rejection, original glyphs, wrong-answer rejection, reward/help/save flows pass.
Reference consulted: https://depts.washington.edu/acelab/proj/dollar/pdollar.html
(point-cloud recognition principles; implementation remains the game's own matcher).

ReleaseVisualPass walks 38 rendered gameplay/menu states at each of 1340x800,
1920x1200 and 2048x1536: 114 captures and 147 control-center raycast checks, with no
remaining off-screen/covered controls, overflowing active text or basket/wallet
overlap reported. All nine contact sheets were visually inspected. Captures and
per-resolution checks.txt are in Captures/Release-0.8.3. Generate review sheets with
AgentScripts/MakeReleaseContactSheets.ps1. Project AGENTS.md now requires the complete
functional and visual regression before every release.

Final 0.8.3 Android packaging succeeded with zero errors (59,029,089-byte APK).
At the user's request the final checks remain on PC; this version has NOT been
installed on the tablet. The previously installed tablet version remains 0.8.2.
Device handwriting and post-install verification are deferred until installation
is requested again. The proposed school-centered milestone/reward redesign remains
a design proposal; this release improves the existing school and recognizer.

## 0.8.4 — spaced home and consistent cottage palette

Mr Rabbit's house and its entry destination moved 0.8 viewport widths to the right.
The countryside expanded from 6.5 to 7.3 widths, keeping the friends' houses,
residents, garden, school and station at their original world coordinates. The
repeating background extends continuously into the extra lawn. House display height
now fits below the top of tablet views. A painted entrance badge sits beside the
door, above actor input priority, clear of the rabbit. The train map uses the same
new house sprite.

HomeExteriorHarmonized.png keeps the original architecture with creamy yellow walls,
clear cyan windows, gentler shading and outlines, and a coral roof matching the
friends' illustration palette. Original art is preserved. Generated using built-in
image editing; references and complete prompt: ArtSources/HomeExteriorHarmonized.prompt.txt.
Idempotent scene update: AgentScripts/SpaceAndHarmonizeHome.cs.

Complete feature/functional regression passed: home movement/stairs, meadow bounds,
day/night, sleep/wake and camera, both characters/footwear, neighborhood visits and
greetings, garden growth/water/harvest and real touch guidance, cooking/eating/TV,
train/city/market/wardrobe saves and transactions, school reading/addition/help,
handwriting and vocabulary coverage. Visual traversal: 38 states in each of
1340x800, 1920x1200 and 2048x1536; 114 screenshots and 150 fixed-control raycasts,
no clipping/overlap/control problems reported. All review sheets inspected. Final
entrance artwork placement additionally passed actual raycasts and tap-to-enter at
all three sizes (VerifyHomeExterior). Captures: Captures/Release-0.8.4.

Changes are ready in the PC Unity preview. Tablet installation remains deferred by
the user. No 0.8.4 APK has been packaged; build it from this verified state when
installation is requested. The prior 0.8.3 APK remains available separately.

## 0.9.0 — Italian learning, medals at home and spoken guidance

The teacher now opens a visual learning notebook with two parallel paths. The
game targets Italian first-primary literacy and mathematics; it has no language
selector or localization service. These are selected learning skills, not a claim
to implement every subject or the complete national curriculum. The reference is
the MIM Indicazioni nazionali (D.M. 221/2025, first classes from 2026/27), translated
into the game's own child-paced sequence:

| Path | Milestones |
| --- | --- |
| Italian | Vowels; spoken letter names; uppercase letter writing; syllables; picture/word reading; short written instructions with picture answers |
| Mathematics | Count/write 0–10; consecutive numbers up to 20; additions within 5, 10 and 20 |

Three independently completed practice prompts enable a five-question check.
At least four correct first answers earn that skill's permanent medal. Checks use
distinct randomized questions, have no timer, and can be retried. A wrong answer
in a check advances after feedback; uncertain or blank handwriting does not count
as an educational error. Requesting a model switches the check to free practice;
assisted answers cannot earn the medal. Replaying completed checks does not create
duplicate rewards. Legacy practice also no longer deducts stars for help.

Earned medals can be selected, hung in any of twelve house frames, moved without
duplication and seen on the actual home wall. The arrangement survives reload.
The same existing shared player progress model is retained for both avatars.

Rewards are tied to skills: vowels unlock pumpkin seeds; sums within ten unlock
blueberries; uppercase consonant writing unlocks the golden clothing variant;
sums within twenty unlock the emerald variant; short reading instructions unlock
a home decoration. New outfits cost 120/180 coins per avatar after unlocking;
the existing 60-coin outfit and all previously owned items remain available.
Both special crops yield six units per harvest. Every unit still sells for exactly
one coin. Market offers include available crops, and accepted requests stay pinned.
All six crops can be cooked and eaten. Four-entry garden inventories migrate to
six entries while preserving harvests, meals and growing plots.

101 offline Italian speech clips (Microsoft Elsa Desktop, it-IT) cover navigation,
garden tools/water/harvest, market requests, lesson instructions, letters, help and
medals. The upper controls replay the latest message and toggle automatic speech;
the mute preference persists. Explicit replay works while automatic speech is muted.
Garden reminders are contextual and rate-limited; no runtime speech API, internet
or OS-installed tablet voice is needed. Audio lines and reproducible preparation:
ArtSources/ItalianVoice.lines.json and AgentScripts/PrepareItalianVoice.ps1.

Handwriting now also supports the Italian uppercase consonants in a separate
alphabet mode; the existing vowel/digit classifier is preserved. Recognition always
classifies the ink independently of the expected solution.

New crop illustrations: Assets/Art/Garden/SchoolRewardCrops.png, generated with
built-in ImageGen using the existing garden atlas as a style reference. Full final
prompt and mode: ArtSources/SchoolRewardCrops.prompt.txt. Medal/book drawings use
resolution-independent UI geometry. Gold/emerald clothing materials recolor the
existing clothing while preserving fur, eyes and outlines, including seated and
train poses. Scene installation/polish helpers: InstallLearningJourney.cs and
PolishLearningJourney.cs under AgentScripts; scene changes were saved through Unity.

Complete functional regression passed for movement, stairs, sleep/wake, day/night,
both characters and footwear, friends/visits, garden, cooking/eating/TV, train/map,
city/market/wardrobe, school/help and existing handwriting/vocabulary. New coverage:
1,100 generated curriculum prompts; 93 actual exercise answers; all eleven medals;
3/5 rejection and 4/5 passes; help and uncertain-ink guards; duplicate prevention;
medal rearrangement/reload; both new crop cycles and sales; all four new outfit
purchases (two variants per avatar); Italian playback/mute/replay. The new flow
also checks actual pointer events for answer choices and medal placement.

The unchanged vowel/digit stress set remains 316 correct, 1 misclassification and
7 uncertain out of 324 synthetic variations; the 15 independent examples pass.
The alphabet extension passes 16 distorted/reversed consonants and 7 independently
drawn examples. These are deterministic PC tests, not measured child handwriting
accuracy or a physical-tablet trial.

Rendered checks traverse 73 states at each of 1340x800, 1920x1200 and 2048x1536:
219 screenshots, over 900 control/ink raycasts, active-text overflow checks and HUD
overlap checks. All fifteen contact sheets were inspected. Fixes include a seed
back button overlapping new seed cards, atlas-cell remapping on cloned reward
art, illustration/choice separation, icon animation affecting card bounds, harvest
and order counters underneath voice controls, and an unavailable text check glyph.
Captures and checks are in Captures/Release-0.9.0. Tests restore original player
progress; the safety snapshot is kept under Library/MrRabbitQA, outside Assets.

0.9.0 (Android version code 16) was packaged and installed on the Samsung SM-T220
on 2026-10-19. The Android build completed with zero errors. A mobile compiler
precision issue in RewardClothes.shader was fixed before installing; the actual
tablet shows the gold and emerald clothing previews correctly. The APK is
Builds/Android/MrRabbit-0.9.0.apk (63,228,945 bytes). No Windows standalone was built.

Post-install smoke checks at the tablet's 1340x800 landscape resolution covered
fresh character selection, zero starting coins, home, footwear, meadow, the station
map, visible train passenger/journey, city drag movement and wardrobe previews.
Captured screens were inspected for layout and rendering issues; the app log had
no matching Unity exception, fatal crash or shader-error entries. Evidence is in
Captures/Release-0.9.0/Tablet-2026-10-19. This is a device smoke pass in addition to
the full PC regression above, not a new physical-tablet handwriting accuracy trial.
Tablet application data was cleared at the user's request, and cleared again after
the smoke pass so the child starts at character selection with no saved progress.
PC saves were not reset.

## 0.10.0 - Elsa's introduction and contextual guide

The first launch now opens a four-page illustrated introduction: home/exploration,
school learning and medals, garden/cooking, and train/city/market/clothes. Elsa
speaks each page; children advance using an animated arrow, can go back, replay,
mute or skip. An unfinished introduction resumes at its saved page. Speech ending
never advances a page or grants game progress. School remains the principal
progression, with crops, outfits and the medal wall presented as its rewards.

After the introduction, a moving finger and pulsing outlines show the next useful
action in the current context. They never intercept touches. Actions are optional;
children can explore in another order. The guide observes real successful actions,
remembers learned steps separately under MrRabbit.Guide.it-IT.v1, and covers choice
of character, drag movement, schoolbag, exit/footwear, classroom/listening, practice,
writing/choice answers, submission, checks, medals/frames, the garden cycle,
cooking/eating, TV/sleep/stairs, map/train, market quantities/delivery and clothes,
and visits/greetings. Choice-answer hints highlight every option equally, without
revealing the answer. Existing learning and handwriting evaluation are unchanged.

Hints respect real prerequisites. Plants grow and need water normally; the guide
lets the child explore during growth. Missing ingredients, insufficient produce,
locked rewards and unaffordable clothes lead to explanations and an exit/return
path, never a fabricated reward or automatic purchase. The guide book with a
question mark opens four short options: introduction, current-context explanation,
pause/resume and skip this step. Replaying guidance never resets rewards. The
introduction and help menu block background taps/movement and pause the game clock.
Context speech waits for feedback/instructions; obsolete guide lines stop when
context changes. Existing water/harvest voice reminders remain rate-limited.

The user-approved it-IT-ElsaNeural is used for all 168 offline Italian clips,
including 67 new guide lines, with the approved -5% rate, -5% volume and natural
pitch. ArtSources/ItalianVoice.profile.json is the single voice profile;
ArtSources/ItalianVoice.lines.json is the canonical script. Generate/update guide
text with AgentScripts/PrepareGuideLines.py, then audio with
AgentScripts/PrepareItalianVoice.py --install (the .ps1 delegates to it). Generation
validates a full batch before installation and preserves existing Unity WAV paths
and GUIDs. Internet is used only during audio preparation; no runtime TTS service,
OS tablet voice, microphone or network connection is required. Synthetic audition
and provenance files remain under ArtSources/VoiceAuditions and
ArtSources/ItalianVoiceNeural. The Windows desktop voice was rejected and is no
longer used. The original 0.9.0 APK retains its historical audio.

The home entrance icon now follows the actual painted doorway for both rabbits,
including Image aspect fitting, rather than sitting to the left of the building.
HomeEntranceAnchor keeps it aligned as the landscape scrolls and screen shape
changes. InstallGameGuide.cs makes scene changes through Unity's Editor API.

Pre-release verification: the guided flow passed 40 rendered states at each of
1340x800, 1920x1200 and 2048x1536, including 219 actual pointer-event interactions.
It completes real school practice and a five-answer assessment, earns/hangs the
medal, performs the actual crop cycle, cooks/eats, sleeps/watches TV, travels both
ways, visits/greets and exercises the market/clothing availability guards. Only
explicit stock/coin availability scenarios use seeded QA inventories. The complete
existing feature regression also passed for both characters/footwear, home/stairs,
day/night, friends/visits, garden, kitchen/TV, train/city, school/help, handwriting,
all eleven medals, reward crops and outfit variants. Handwriting retains its
previous synthetic baseline (316/324 correct, 1 wrong, 7 uncertain), not a measured
child-accuracy claim.

A further 73 gameplay/learning states at each resolution passed UI raycast,
overflow and HUD-overlap checks: 219 existing-flow captures plus 120 guided-flow
captures. All 24 contact sheets were inspected. Fixes found during this work include
highlighting entire landmarks instead of their action icons, guide/menu overlap,
book/medal illustration overlap, and schoolbag narration mismatching the actual
sandwich/water-bottle pictures. The changed introduction art was rechecked at all
three resolutions. All 168 WAV hashes/formats/signals pass validation (787.392s
total); scene audio references are complete. Test evidence is in
Captures/Guide-0.10.0 and Captures/Release-0.10.0. Original PC saves were restored
from a fresh backup under Library/MrRabbitQA. Android version code is 17.

The final Android build succeeded with zero errors. The remaining three warnings
concern the intentionally disabled runtime Pipeline and deprecated Android icon
formats. Builds/Android/MrRabbit-0.10.0.apk is 65,388,613 bytes, SHA256
6C7CDC4796D8417D795E8DD24C25957D544669A664872BA2BE222BB617B34F73.
It was installed on the Samsung SM-T220 on 2026-10-19 as an update, without clearing
tablet game data. Version 0.10.0/code 17 and the installed APK hash match the final
build. The cold launch succeeded; the app-process log has zero matching Unity
exceptions, shader errors or fatal crash entries. Native touch checks exercised
all four introduction pages and the previous-page controls at 1340x800. Captures
are under Captures/Release-0.10.0/Tablet-2026-10-19. The guide is left on its first
page, still unfinished, ready for the child. Physical child-handwriting accuracy
was not remeasured. A final focused replay regression also confirms requested
context explanations play once without an automatic duplicate.
