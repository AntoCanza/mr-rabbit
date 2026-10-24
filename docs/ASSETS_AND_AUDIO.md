# Assets and audio

## Drawings and illustrations

`ArtSources` preserves original reference drawings, image-generation prompts, earlier visual variants and voice-generation provenance. Runtime content is under `Assets/Art`, divided into `Characters`, `Home`, `Outdoors`, `Garden`, `School` and `City`.

The game artwork was developed from original drawings with generated illustration work. Prompt files document the intended appearance and source process; they are not blanket third-party license grants. Keep original references and existing notices when changing assets. Do not move or replace Unity `.meta` files independently of their assets.

The README gallery contains three selected historical 0.10.0 screenshots copied from local QA captures: home, guided vowel handwriting and train travel. The full capture archive and device logs remain ignored.

## Offline Italian narration

| Source | Purpose |
| --- | --- |
| `ArtSources/ItalianVoice.profile.json` | Approved `it-IT-ElsaNeural` voice, Italian locale, rate −5%, volume −5%, natural pitch and 24 kHz output |
| `ArtSources/ItalianVoice.lines.json` | Canonical spoken script |
| `ArtSources/VoiceAuditions/Guida-naturale-elsa-prova.mp3` | Approved comparison sample |
| `ArtSources/ItalianVoiceNeural/manifest.json` | Synthetic audio provenance, generation version and clip hashes |
| `Assets/Audio/Italian/` | Included mono 16-bit PCM WAV clips used by the game |
| `AgentScripts/PrepareItalianVoice.py` | Optional preparation, validation and installation tool |

The game plays the included WAV clips offline. It does not call a runtime TTS service or need a microphone. Keep the approved Elsa neural voice for guidance, lessons and notifications; do not substitute the rejected desktop voice or another speaker.

### Optional audio regeneration

Audio preparation requires Python and the `edge-tts` and `imageio-ffmpeg` development dependencies. The preparation script documents their use. Use a separate virtual environment or the project's ignored development-tool location.

```sh
python -m venv .venv
# Activate the environment using the command appropriate for your shell.
python -m pip install edge-tts imageio-ffmpeg
python AgentScripts/PrepareItalianVoice.py
```

The last command prepares and validates the staging batch under `ArtSources/ItalianVoiceNeural`. Review the result before installing it:

```sh
python AgentScripts/PrepareItalianVoice.py --install
```

Installation validates the complete batch and preserves the Unity WAV paths and GUIDs. Generation uses an online Microsoft speech service through `edge-tts`; the generated game playback remains offline. Review the service terms that apply to generation and distribution before using regenerated audio commercially. `PrepareGuideLines.py` updates guide text and should only be run when intentionally revising the script. The PowerShell voice helper delegates to the Python tool.

## Third-party content and licensing

Unity dependencies retain their package licenses and notices. Imported TextMesh Pro resources and fonts retain the notices included under `Assets/TextMesh Pro`. Check those files and the restored package contents before redistribution.

The repository does not introduce a project-wide reuse license. Original project content remains subject to its owners' rights; public hosting does not by itself select MIT, Apache, Creative Commons or another license. Do not remove third-party notices, imply ownership of third-party material or infer redistribution permissions from a file's presence.
