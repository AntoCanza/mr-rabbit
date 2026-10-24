"""Generate audition files only; never replace shipped game audio.

Requires edge-tts (https://github.com/rany2/edge-tts). Generation uses Microsoft's
online service; the resulting audio files play offline. No child data is sent.
"""
import asyncio
import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT / "Library" / "VoiceTools"))
import edge_tts

TEXT = (
    "Ciao! Facciamo un piccolo passo insieme. "
    "Guarda la gocciolina: l'orto ha bisogno d'acqua. "
    "Prendi l'annaffiatoio, poi tocca la piantina. "
    "Bravissimo. Possiamo continuare con calma."
)


async def main():
    folder = ROOT / "ArtSources" / "VoiceAuditions"
    folder.mkdir(parents=True, exist_ok=True)
    manifest = {"status": "audition only; not included in game", "synthetic": True,
                "generator": "edge-tts", "generatorSource": "https://github.com/rany2/edge-tts",
                "text": TEXT, "samples": []}
    for label in ("Isabella", "Elsa"):
        voice = f"it-IT-{label}Neural"
        path = folder / f"Guida-naturale-{label.lower()}-prova.mp3"
        await edge_tts.Communicate(TEXT, voice, rate="-5%", volume="-5%").save(str(path))
        content = path.read_bytes()
        if len(content) < 10000:
            raise RuntimeError(f"Missing or incomplete speech: {path.name}")
        manifest["samples"].append({"voice": voice, "rate": "-5%", "volume": "-5%",
                                    "file": path.name, "bytes": len(content),
                                    "sha256": hashlib.sha256(content).hexdigest()})
        print(f"Ready: {path.name} ({len(content)} bytes)")
    (folder / "NaturalVoice-auditions.json").write_text(
        json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    asyncio.run(main())
