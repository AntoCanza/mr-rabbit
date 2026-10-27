# Sito pubblico e indicizzazione su Google

Il sito del gioco è **https://antocanza.github.io/mr-rabbit/**. GitHub Pages pubblica la cartella `docs` del ramo `main`. Il file `docs/.nojekyll` conserva HTML e risorse senza trasformazioni. I contenuti sono leggibili senza JavaScript, accesso al gioco o servizi esterni per i caratteri tipografici.

Il repository GitHub rimane il riferimento per codice e documentazione tecnica. La sezione About e il README collegano il sito pubblico. GitHub controlla HTML, intestazioni e regole dei robot su `github.com`: i file di questo repository non possono modificarli.

## Elementi inclusi

- Titolo, descrizione e contenuti in italiano, un solo titolo principale e collegamenti HTML navigabili.
- Indirizzo canonico HTTPS, URL Open Graph coerente e anteprime con immagini locali.
- Dati strutturati WebSite, WebPage, VideoGame e SoftwareApplication, senza recensioni, prezzi, collegamenti allo store o disponibilità inventati.
- FAQ sul riutilizzo open source, collegamento alla licenza MIT nel piè di pagina e proprietà `license` nei dati strutturati del gioco, tutti riferiti al [testo canonico della licenza](../LICENSE) nel repository.
- [Mappa del sito](sitemap.xml) con la pagina principale e l'[informativa privacy](privacy/). Le date di modifica sono omesse per non segnalare aggiornamenti inesistenti.
- Pagina di errore con `noindex`; GitHub Pages restituisce HTTP 404 per i percorsi inesistenti.
- Impaginazione adattabile a telefoni e tablet, dimensioni delle immagini dichiarate e copie WebP leggere delle schermate esistenti.

Eseguire `python tools/check_site.py` dopo modifiche a pagine, immagini, percorsi o mappa del sito. Il controllo viene eseguito anche dal flusso di verifica del repository e non avvia Unity. Le verifiche funzionali e di rilascio del gioco restano quelle descritte in [VERIFICATION.md](VERIFICATION.md).

Il testo canonico dell'informativa resta `Assets/Resources/InformativaPrivacy.txt`. Dopo una modifica eseguire `python tools/export_privacy.py`: l'esportatore conserva anche indirizzo canonico, metadati e collegamento al sito.

## Configurare Google Search Console

Il proprietario deve verificare il sito con il proprio account Google. L'accesso al repository non concede automaticamente l'accesso a Search Console.

1. In [Google Search Console](https://search.google.com/search-console/) aggiungere la proprietà **Prefisso URL** `https://antocanza.github.io/mr-rabbit/`. Non usare una proprietà DNS per il dominio `github.io`.
2. Scegliere la verifica tramite file HTML o meta tag. Salvare il file effettivo fornito da Google in `docs/`, oppure il meta tag esatto in `docs/index.html`, quindi pubblicare la modifica. Conservare la verifica anche dopo la conferma.
3. Inviare `https://antocanza.github.io/mr-rabbit/sitemap.xml` nella sezione Sitemap.
4. Ispezionare la pagina principale, eseguire il test dell'URL pubblicato e richiedere l'indicizzazione. Controllare successivamente il rapporto di indicizzazione delle pagine.

Un file `/mr-rabbit/robots.txt` non controllerebbe la scansione: i robot leggono soltanto `https://antocanza.github.io/robots.txt`, alla radice dell'host. Questo repository non pubblica quella radice. L'assenza del file permette la scansione; ricontrollare eventuali regole se in futuro viene creato un sito dell'account.

Google decide se e quando indicizzare una pagina idonea. La mappa e i metadati non garantiscono indicizzazione o posizione nei risultati. L'idoneità ai risultati avanzati è distinta dall'indicizzazione generale; il gioco non ha ancora un'offerta pubblica o recensioni.

## Manutenzione

Mantenere coerenti la FAQ open source, il collegamento alla licenza nel piè di pagina e la proprietà `license` dei dati strutturati con [LICENSE](../LICENSE), [README](../README.md#crediti-e-licenze) e [Risorse grafiche e audio](ASSETS_AND_AUDIO.md#contenuti-di-terzi-e-licenze). La concessione MIT riguarda i materiali originali del progetto; conservare le condizioni dei componenti di terzi e della generazione vocale.

Conservare l'italiano e la distinzione tra obiettivi educativi e copertura attuale: competenze selezionate di italiano e matematica, con la scuola al centro. Non dichiarare la copertura dell'intero programma né la disponibilità su Google Play prima della conferma. Se cambia l'indirizzo del sito, aggiornare insieme indirizzi canonici, mappa, dati strutturati, anteprime e sito nella sezione About. Collegare ogni nuova pagina dalle pagine esistenti e aggiungere soltanto i suoi indirizzi canonici alla mappa.

Fonti: [requisiti tecnici di Google](https://developers.google.com/search/docs/essentials/technical), [collegamenti scansionabili](https://developers.google.com/search/docs/crawling-indexing/links-crawlable), [mappe del sito](https://developers.google.com/search/docs/crawling-indexing/sitemaps/build-sitemap), [posizione di robots.txt](https://developers.google.com/crawling/docs/robots-txt/create-robots-txt) e [sorgenti di pubblicazione GitHub Pages](https://docs.github.com/en/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site).
