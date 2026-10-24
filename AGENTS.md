# Release verification

The user requires a full feature and functional regression before every release.
Before packaging/installing a release, exercise all available gameplay features,
navigation, inputs, progress and save compatibility. Inspect rendered screens for
glitches, clipped/missing art, overlapping UI and unreachable/off-screen controls.
Include representative tablet aspect ratios (4:3, 16:10 and the connected tablet),
and verify actual UI raycasts/touch targets, not only direct method calls.
Fix issues and recheck affected paths before distributing. Preserve real player
saves; tests that modify progress must restore it. After installation, verify the
installed version, launch and device logs. Record coverage, fixes and any limits in
the release notes. Do not claim a physical-device check when only simulated.

The primary audience is children aged 6–8. Keep essential reading exercise text,
short orientation labels and numbers; communicate other actions with recognizable
drawings/icons and contextual highlights.

School learning is the primary progression. Future places, city activities and
economy should support it. The game is optimized for Italian and the Italian
first-primary learning sequence; do not add multilingual UI or a language picker.
Milestone medals are earned through practice and short varied checks, unlock
rewards, and can be hung and rearranged in the house. Preserve earned medals,
existing items, currencies and old saves. Subtraction remains excluded. Do not
claim coverage of the entire national curriculum: these are selected literacy
and mathematics skills, with prerequisites and child-paced progression.
Provide concise offline Italian voice guidance, with replay and a saved mute
preference; avoid repeated automatic announcements and overlapping HUD controls.
The onboarding in 0.10.0 is a guided first-run wizard followed by contextual spoken
directions and moving/highlighted action icons throughout the game.
Its Italian female voice, and general game narration, should feel calm, warm,
comfortable and reassuring, like a mother. Keep the school central, show one useful
action at a time, and advance from actual successful actions, never speech ending
or timers. Keep tutorial progress separate from earned school rewards and inventory.
Support replay, pause, skip and resuming an unfinished introduction. Never instruct
an unavailable purchase, harvest, recipe or delivery. Decorative hands/highlights
must not intercept touches. The home entrance icon must track the painted doorway
including aspect-ratio fitting on different tablet screens.
The user approved it-IT-ElsaNeural and explicitly requires her for every spoken
line: guidance, lessons, notifications, future activities and onboarding. Preserve
the approved -5% rate, -5% volume and natural pitch in
ArtSources/ItalianVoice.profile.json. The approved comparison sample is
ArtSources/VoiceAuditions/Guida-naturale-elsa-prova.mp3. Do not substitute Windows
Elsa Desktop (rejected as mechanical), Isabella or another voice. Retain offline
playback; generate audio at development time with AgentScripts/PrepareItalianVoice.py.
Notifications should also use spoken Italian alongside the relevant icon, including
the garden's need for water. Keep reminders calm, contextual and rate-limited.
Handwriting must tolerate children's variations while classifying independently
of the expected solution. Keep wrong/blank/scribble rejection and add varied
freehand regression examples when changing recognition.
