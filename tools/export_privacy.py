"""Esporta la pagina pubblica dal testo privacy canonico incluso nel gioco."""

from html import escape
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "Assets/Resources/InformativaPrivacy.txt"
OUTPUT = ROOT / "docs/privacy/index.html"


def main():
    sections = []
    for index, page in enumerate(SOURCE.read_text(encoding="utf-8-sig").split("\n---\n")):
        title, body = page.strip().split("\n", 1)
        level = "h1" if index == 0 else "h2"
        paragraphs = "\n".join("<p>" + escape(part).replace("\n", " ") + "</p>"
                               for part in body.strip().split("\n\n"))
        sections.append(f"<section><{level}>{escape(title)}</{level}>\n{paragraphs}</section>")
    document = '''<!doctype html>
<html lang="it">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta name="description" content="Informativa sulla privacy di Mr Rabbit, il gioco educativo di SofiaAntonio Games.">
<meta name="robots" content="index, follow">
<link rel="canonical" href="https://antocanza.github.io/mr-rabbit/privacy/">
<link rel="sitemap" type="application/xml" href="../sitemap.xml">
<meta property="og:type" content="website">
<meta property="og:locale" content="it_IT">
<meta property="og:title" content="Mr Rabbit — Informativa sulla privacy">
<meta property="og:description" content="Informativa sulla privacy di Mr Rabbit, il gioco educativo di SofiaAntonio Games.">
<meta property="og:url" content="https://antocanza.github.io/mr-rabbit/privacy/">
<title>Mr Rabbit — Informativa sulla privacy | SofiaAntonio Games</title>
<style>
:root{color-scheme:light}*{box-sizing:border-box}body{margin:0;background:#fffaf0;color:#293d31;font:1.08rem/1.7 system-ui,-apple-system,Segoe UI,sans-serif}main{max-width:54rem;margin:auto;padding:3rem 1.4rem 4rem}header{color:#53684e;border-bottom:1px solid #cbd5bf;padding-bottom:1rem;margin-bottom:2rem}h1{font-size:2.3rem;line-height:1.2}h2{font-size:1.5rem;line-height:1.35;margin-top:2.5rem}p{margin:1rem 0}footer{border-top:1px solid #cbd5bf;margin-top:3rem;padding-top:1rem;font-size:.95rem}a{color:#245c55;overflow-wrap:anywhere}@media(max-width:600px){main{padding-top:1.8rem}h1{font-size:1.9rem}}
</style>
</head>
<body><main>
<header><a href="../">Mr Rabbit</a> · SofiaAntonio Games</header>
''' + "\n".join(sections) + '''
<footer>Assistenza e privacy: <a href="mailto:antoniocanza@gmail.com">antoniocanza@gmail.com</a></footer>
</main></body></html>
'''
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    OUTPUT.write_text(document, encoding="utf-8", newline="\n")
    print("Pagina privacy aggiornata dal testo incluso nel gioco.")


if __name__ == "__main__":
    main()
