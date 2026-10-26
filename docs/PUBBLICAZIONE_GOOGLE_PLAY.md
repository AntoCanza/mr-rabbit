# Pubblicazione su Google Play

Preparazione del 24 ottobre 2026, ricontrollata il 26 ottobre 2026. **Mr Rabbit non è ancora pubblicato su Google Play.**

## Dati approvati

| Campo | Valore |
| --- | --- |
| Nome dell'app | Mr Rabbit |
| Sviluppatore pubblico | SofiaAntonio Games |
| Assistenza e privacy | antoniocanza@gmail.com |
| Distribuzione | Gratuita, senza pubblicità né acquisti in-app |
| Lingua e pubblico | Italiano, bambini di 6–8 anni |
| Versione preparata | 0.10.1, codice Android 18 |
| Identificatore | com.mrrabbit.home |
| Piattaforma | Android, ARM64 / IL2CPP, API minima 25 e destinazione 36 |

La [scheda italiana](SCHEDA_GOOGLE_PLAY_IT.md) contiene nome, descrizione breve,
descrizione completa e note della prima versione. La scuola è il fulcro del gioco:
il percorso segue la prima elementare italiana, con la copertura attuale limitata
a competenze selezionate di italiano e matematica.

## Account e creazione dell'app

Il 26 ottobre 2026 è stata creata l'app nell'account personale **SofiaAntonio Games**,
con nome **Mr Rabbit**, lingua **Italiano — it-IT**, tipo **Gioco**, distribuzione
**Senza costi** e pacchetto **com.mrrabbit.home**. Il titolare ha approvato
esplicitamente le dichiarazioni di conformità, i termini di Play App Signing e
la dichiarazione sulle leggi statunitensi di esportazione prima della creazione.

Completate la scheda italiana e le dichiarazioni della console: nessuna pubblicità,
nessun ID pubblicità, nessuna raccolta o condivisione di dati, nessun accesso
riservato, pubblico esclusivamente di **6–8 anni**, nessuna funzione governativa,
finanziaria o sanitaria. Selezionata la categoria **Gioco → Educativi** e impostati
email pubblica e sito HTTPS. Mostrato l'impegno alle Norme per le famiglie nella
sezione Sicurezza dei dati. Non richiesta l'adesione facoltativa al programma
App approvate dagli insegnanti.

Con ulteriore approvazione esplicita del titolare per i termini IARC e il contatto
pubblico, il questionario ha assegnato **PEGI 3** in Europa e **Everyone / Per tutti**
per ESRB. Queste classificazioni non cambiano il pubblico educativo di 6–8 anni.

Per gli account personali creati dopo il 13 novembre 2023 Google richiede, prima
della domanda di accesso alla produzione, un test chiuso con almeno 12 tester
iscritti ininterrottamente per 14 giorni. Le verifiche locali non lo sostituiscono.
Nessun tester è stato reclutato o iscritto.

Riferimenti: [registrazione](https://support.google.com/googleplay/android-developer/answer/6112435)
e [requisiti di test dei nuovi account personali](https://support.google.com/googleplay/android-developer/answer/14151465).

## Privacy

L'informativa italiana è disponibile nel gioco dalla scelta del personaggio,
tramite **Per i genitori · Privacy**. Le quattro pagine sono leggibili offline,
bloccano i tocchi ai personaggi sottostanti e sospendono la guida fino alla chiusura.
Non chiedono dati, consensi o invii di messaggi.

Il testo canonico è [InformativaPrivacy.txt](../Assets/Resources/InformativaPrivacy.txt).
La [pagina pubblica sorgente](privacy/index.html) deriva dallo stesso testo:
rigenerarla con `python tools/export_privacy.py` in caso di modifiche.
L'[informativa pubblica su GitHub Pages](https://antocanza.github.io/mr-rabbit/privacy/)
è ora attiva ed è l'URL da usare nel campo privacy della console. Il ricontrollo
del 26 ottobre conferma HTTP 200 senza autenticazione e HTML identico ai sorgenti.
Anche il [sito del gioco](https://antocanza.github.io/mr-rabbit/), immagini,
foglio di stile e mappa del sito rispondono correttamente. Il testo canonico
rimane disponibile anche in
[sola lettura](https://raw.githubusercontent.com/AntoCanza/mr-rabbit/main/Assets/Resources/InformativaPrivacy.txt).

Il nuovo manifesto Android rimuove INTERNET e ACCESS_NETWORK_STATE e imposta
allowBackup=false. Progressi e riconoscimento della scrittura rimangono locali;
la voce è inclusa nel gioco e pubblicità, acquisti in-app e analisi sono assenti. Il controllo del
pacchetto finale ha confermato questi valori. Il vecchio APK 0.10.0
includeva INTERNET: questa differenza riguarda il nuovo pacchetto.

Le dichiarazioni salvate in Sicurezza dei dati, Pubblico e contenuti, Pubblicità,
Accesso all'app e Classificazione dei contenuti rispecchiano il prodotto verificato.
Il pubblico di 6–8 anni comporta le norme per le famiglie. Ogni futuro cambiamento
a dati, servizi o pubblico richiede di riesaminare le dichiarazioni.
Riferimenti: [Sicurezza dei dati](https://support.google.com/googleplay/android-developer/answer/10787469)
e [pubblico e contenuti](https://support.google.com/googleplay/android-developer/answer/9867159).

## Firma e compilazione

La chiave di caricamento RSA a 3072 bit è stata generata per SofiaAntonio Games,
con alias `mrrabbit-upload`, e conservata fuori dal repository nel profilo Windows,
sotto `.codex/secrets/mr-rabbit`. La password è protetta con DPAPI per l'account
Windows corrente. Non è stata caricata una chiave privata sul repository o inviata
alla console. La protezione DPAPI non sostituisce una copia sicura e recuperabile
della chiave e della password gestita dal titolare.

`AgentScripts/BuildGooglePlay.cs` legge i materiali locali protetti, prepara l'AAB
e ripristina le impostazioni di firma al termine. Il metodo da eseguire con lo
strumento dell'Editor è `BuildGooglePlay.Run`; deve partire fuori da Play.
Il pacchetto previsto è `Builds/Android/MrRabbit-0.10.1.aab`.
Compilazione riuscita con zero errori e tre avvisi; dimensione 65.281.281 byte.
Gli avvisi riguardano Pipeline di sviluppo disabilitato nel gioco e i due formati
di icone tradizionali/rotonde deprecati da Unity.
Impronta SHA256: `AC96BE4B36273DA12B1AA621F169C41321D6CE197C57F38DD45557EC7C7E777C`.
La validazione con bundletool è superata e la firma JAR è verificata.
Il controllo del manifesto finale conferma versione 0.10.1/codice 18, API 36,
assenza di INTERNET e ACCESS_NETWORK_STATE, allowBackup=false e compilazione
non di sviluppo. Tutte le sette librerie native ARM64 hanno segmenti allineati
a 16 KB; anche la configurazione dell'AAB dichiara PAGE_ALIGNMENT_16K.

Il verificatore JDK segnala il certificato autofirmato senza marca temporale e
l'ordine META-INF del contenitore Gradle nella lettura sequenziale JarInputStream.
La verifica tramite JarFile è positiva. Il 26 ottobre Play Console ha accettato
l'AAB e mostrato versione 18 (0.10.1), API minima 25, destinazione 36 e ABI
arm64-v8a, dopo l'accettazione approvata dei termini di Play App Signing.
Non inserire password nei comandi o nei documenti.

La firma distribuita da Play può differire da quella delle precedenti
installazioni locali. Prima di aggiornare il tablet verificare la compatibilità,
preservando i progressi: non disinstallare o cancellare i dati per aggirare un
conflitto di firma.

Riferimenti: [caricamento e firma](https://developer.android.com/studio/publish/upload-bundle)
e [API di destinazione richieste](https://support.google.com/googleplay/android-developer/answer/11926878).

## Materiali grafici

I sorgenti [GooglePlayFeature.svg](../ArtSources/GooglePlayFeature.svg) e
[GooglePlayIcon.svg](../ArtSources/GooglePlayIcon.svg) usano le immagini esistenti
del progetto e una schermata effettiva della classe. Non introducono scene o
funzionalità immaginarie. L'esportatore `tools/export_store_graphics.cjs` richiede
Node.js e il modulo di sviluppo `sharp` e salva in `Builds/GooglePlay/0.10.1/`:

- icona PNG RGBA a 32 bit, 512×512 e inferiore a 1 MB;
- immagine principale PNG RGB 1024×500;
- sei schermate RGB per smartphone a 1920×1080, acquisite il 26 ottobre 2026;
- sei schermate RGB a 1920×1200 e sei a 2048×1536.

Le schermate provengono dalle acquisizioni dell'Editor della versione 0.10.1;
non sono fotografie o nuove prove su tablet fisici. I file esportati restano
locali, fuori da Git, come gli altri risultati di compilazione.

## Verifiche eseguite il 24 ottobre 2026

La regressione funzionale completa ha superato le 19 verifiche esistenti di casa,
movimento, amiche, giorno/notte, orto, cucina, città, scuola, medaglie e comandi.
Superate anche le prove della logica didattica e della scrittura: 1.100 consegne
del percorso, 1.000 domande scolastiche, vocabolario e rifiuto di risposte errate,
vuote o scarabocchiate. La suite sintetica della scrittura mantiene 316/324 corretti,
1 errato e 7 incerti; non misura l'accuratezza sui bambini reali.

Le prove visive a 1340×800, 1920×1200 e 2048×1536 coprono 73 stati del gioco e
40 della guida per risoluzione, più quattro pagine privacy per ciascun formato:
351 schermate, tutte ispezionate attraverso 21 tavole riepilogative. Superati
raycast reali, percorsi guidati, controlli del testo e delle sovrapposizioni.
Verificati anche alunni con vestiti equipaggiati e guida della cucina quando
la ricetta precedentemente selezionata non ha ingredienti.

I sette salvataggi e preferenze originali del PC sono stati ripristinati dalla
copia protetta creata prima delle prove. Evidenze locali in
`Captures/GooglePlay-2026-10-24`, `Captures/Guide-0.10.1` e `Captures/Release-0.10.1`.
Nessun tablet era collegato: non è stata eseguita una nuova installazione o
verifica su dispositivo fisico. Seguire [AGENTS.md](../AGENTS.md) e
[Verifiche](VERIFICATION.md) per i controlli successivi all'installazione.

## Passaggi residui

1. Attendere i controlli preliminari e la revisione Google del test chiuso Alpha, rispondendo agli eventuali rilievi effettivi della console.
2. Reclutare tester reali, aggiungerli alla mailing list o al gruppo autorizzato e farli aderire con il proprio account Google. Al momento non è configurato alcun elenco di tester.
3. Raccogliere feedback d'uso e mantenere almeno 12 tester iscritti continuativamente per 14 giorni. La prova eseguita dal solo titolare non sostituisce questo requisito.
4. Richiedere l'accesso alla produzione quando la console conferma i requisiti e presentare la successiva versione pubblica alla revisione.
5. Verificare l'effettiva disponibilità pubblica prima di aggiungere al sito un pulsante di installazione o dichiarare il gioco pubblicato.

Il link di adesione previsto dalla console è
`https://play.google.com/apps/testing/com.mrrabbit.home`; al momento dell'invio il
pulsante per copiarlo risulta disabilitato. Non va presentato come un test già
accessibile. L'email per i feedback è **antoniocanza@gmail.com**.

Google suggerisce di reclutare tramite contatti personali e professionali e
comunità rappresentative del pubblico. Per Mr Rabbit sono particolarmente utili
genitori adulti che parlano italiano e possiedono un dispositivo Android; non
servono dati personali dei bambini. Non sono stati inviati inviti o pubblicati
annunci di reclutamento.

## Ricontrollo del 26 ottobre 2026

- Il ricontrollo è partito dal commit pubblico `3640ecd`; i controlli GitHub e la distribuzione Pages di quel commit risultano riusciti.
- Il controllo locale del repository supera 1.343 file e 342 GUID unici.
- Il pacchetto AAB conserva dimensione e SHA256 riportati sopra. Da `e6183c5`, commit della preparazione 0.10.1, non risultano modifiche a gioco, pacchetti, impostazioni Unity o strumenti dell'Editor. Non è stata eseguita una nuova compilazione né una nuova installazione o prova su dispositivo fisico.
- Eseguite nuovamente le due suite visive a 1920×1080: 38 stati con 50 centri di controllo e 35 stati didattici con 255 raycast effettivi di comandi e scrittura, senza anomalie nei risultati finali. Ispezionate tutte le 73 schermate attraverso quattro tavole. La suite storica richiede `LearningJourney.AutoOpen=false` a runtime per verificare i propri comandi; la suite didattica copre il percorso attuale. Nessuna impostazione di gioco è stata salvata nei sorgenti.
- Verificato l'avanzamento del gioco nell'Editor, quindi ripristinati i sette salvataggi e preferenze originali tramite la copia protetta acquisita prima di Play. Non è stata ripetuta l'intera suite funzionale del 24 ottobre; il pacchetto già verificato è invariato.
- Ispezionati icona, immagine principale e diciotto schermate nei tre formati. Corretto l'esportatore dell'icona per produrre PNG RGBA a 32 bit, preservandone l'aspetto originale, e aggiunta l'esportazione delle sei schermate 1920×1080. L'immagine principale e le schermate rimangono RGB.
- Scheda italiana entro i limiti: nome 9/30, descrizione breve 78/80, descrizione completa 2.052/4.000, note del test 201/500 caratteri. Il testo espone attività, limiti della copertura didattica, pubblico, guida offline, salvataggi locali e assenza di pubblicità e acquisti.
- Salvata la dichiarazione degli asset AI prevista dal flusso della console, coerente con la provenienza assistita dall'AI delle illustrazioni. I materiali rimangono schermate reali del gioco, non scene promozionali inventate.
- Creato il test chiuso **Alpha**, con release **0.10.1 (18) - Test chiuso** e disponibilità nei 178 paesi/regioni proposti dalla console. Disattivata l'adesione automatica a Google Play Games on PC: questa preparazione non certifica una versione PC.
- L'unico errore iniziale di convalida della release era l'assenza dei paesi; risolto impostando la disponibilità del canale. Il controllo preliminare ha poi richiesto la dichiarazione ID pubblicità, completata con «No» sulla base del manifesto e degli SDK presenti.
- Restano avvisi non bloccanti per assenza di tester, file di deoffuscamento R8/ProGuard e simboli nativi. Non sono stati generati nuovi pacchetti per eliminare avvisi facoltativi.
- Inviate **14 modifiche per la revisione**. La console conferma **«Modifiche in fase di revisione»**, con controlli rapidi ancora in corso e successivo inoltro alla revisione al loro termine. Nessun tester risulta iscritto e nessuna disponibilità pubblica è stata confermata. Non è stato creato un rilascio di produzione.

Evidenze locali del ricontrollo: `Captures/GooglePlay-2026-10-26`.
