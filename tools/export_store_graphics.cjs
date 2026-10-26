// Esporta i sorgenti SVG e le schermate reali nei formati per Google Play.
const fs = require('node:fs/promises');
const path = require('node:path');
const sharp = require('sharp');

const root = path.resolve(__dirname, '..');
const version = '0.10.1';
const output = path.join(root, 'Builds', 'GooglePlay', version);

async function render(name, destination, alpha = false) {
  const file = path.join(root, 'ArtSources', name);
  let svg = await fs.readFile(file, 'utf8');
  const matches = [...svg.matchAll(/href="([^"]+)"/g)];
  for (const match of matches) {
    const asset = path.resolve(path.dirname(file), match[1]);
    if (!asset.startsWith(root + path.sep) || path.extname(asset) !== '.png')
      throw new Error('Riferimento grafico non valido: ' + match[1]);
    const image = await fs.readFile(asset);
    svg = svg.replace(match[0], 'href="data:image/png;base64,' + image.toString('base64') + '"');
  }
  const image = sharp(Buffer.from(svg)).flatten({ background: '#fff9e9' });
  if (alpha) image.ensureAlpha();
  else image.removeAlpha();
  await image.png().toFile(path.join(output, destination));
}

async function main() {
  await fs.mkdir(output, { recursive: true });
  await render('GooglePlayIcon.svg', 'icona-512.png', true);
  await render('GooglePlayFeature.svg', 'immagine-principale-1024x500.png');
  const captures = ['19-classroom.png', '41-practice-vowel.png', '47-maths-path.png',
    '58-home-medals.png', '14-garden.png', '28-train.png'];
  for (const size of ['1920x1080', '1920x1200', '2048x1536']) {
    const folder = path.join(output, 'schermate-' + size);
    await fs.mkdir(folder, { recursive: true });
    for (let index = 0; index < captures.length; index++) {
      const input = path.join(root, 'Captures', 'Release-' + version, size, captures[index]);
      await sharp(input).flatten({ background: '#fff9e9' }).removeAlpha().png()
        .toFile(path.join(folder, String(index + 1).padStart(2, '0') + '-' + captures[index].slice(3)));
    }
  }
  console.log('Materiali Google Play esportati: icona 512×512, immagine 1024×500 e 18 schermate per smartphone e tablet.');
}

main().catch(error => { console.error(error.message); process.exitCode = 1; });
