# Pubblicazione su Google Play

Preparazione del 24 ottobre 2026. **Mr Rabbit non è ancora pubblicato su Google Play.**

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

## Account da creare

Il titolare ha confermato che deve creare l'account sviluppatore. La console
attualmente aperta mostra la registrazione e richiede prima la verifica in due
passaggi. Il titolare deve accedere con l'account Google che intende usare,
completare registrazione, verifiche di identità e pagamento della quota unica di
25 USD. Nessun account è stato creato e nessun contratto o pagamento è stato
confermato durante questa preparazione.

Scegliere tra account personale e organizzazione in base al soggetto che pubblica.
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
L'[informativa pubblica in sola lettura](https://raw.githubusercontent.com/AntoCanza/mr-rabbit/main/Assets/Resources/InformativaPrivacy.txt) è stata verificata
con una richiesta senza autenticazione: HTTP 200, testo identico a quello del gioco.
Questo è l'URL preparato per il campo privacy della console. Il token GitHub
non consente di attivare GitHub Pages e l'account del browser non dispone delle
impostazioni del repository; non sono stati modificati permessi o credenziali.
La pagina HTML resta disponibile nei sorgenti per un eventuale hosting dedicato.

Il nuovo manifesto Android rimuove INTERNET e ACCESS_NETWORK_STATE e imposta
allowBackup=false. Progressi e riconoscimento della scrittura rimangono locali;
la voce è inclusa nel gioco e pubblicità, acquisti in-app e analisi sono assenti. Il controllo del
pacchetto finale ha confermato questi valori. Il vecchio APK 0.10.0
includeva INTERNET: questa differenza riguarda il nuovo pacchetto.

Compilare Sicurezza dei dati, Pubblico e contenuti, Pubblicità, Accesso all'app e
Classificazione dei contenuti sulla base del prodotto effettivo. Non è stata
assegnata una classificazione IARC e nessuna dichiarazione è stata inviata.
Il pubblico di 6–8 anni comporta le norme per le famiglie.
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
La verifica tramite JarFile è positiva; il caricamento in Play Console resta
necessario per la conferma definitiva dell'accettazione da parte dello store. Per una nuova app completare Play App Signing nella console.
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

- icona PNG RGB 512×512;
- immagine principale PNG RGB 1024×500;
- sei schermate RGB a 1920×1200 e sei a 2048×1536.

Le schermate provengono dalle acquisizioni dell'Editor della versione 0.10.1;
non sono fotografie o nuove prove su tablet fisici. I file esportati restano
locali, fuori da Git, come gli altri risultati di compilazione.

## Verifiche eseguite

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

1. Completare e verificare l'account sviluppatore.
2. Usare l'URL pubblico della privacy e il pacchetto AAB finale già verificati.
3. Creare l'app nella console, caricare il pacchetto e i materiali, compilare le dichiarazioni.
4. Eseguire il test chiuso obbligatorio se l'account personale vi è soggetto e richiedere l'accesso alla produzione.
5. Inviare la versione alla revisione Google e verificare la disponibilità effettiva prima di dichiararla pubblicata.
