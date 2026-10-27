# Guida operativa del repository

## Avvio e fonti pertinenti

La radice è un progetto Unity completo. Usare **Unity 6000.3.25f1**, come fissato
in `ProjectSettings/ProjectVersion.txt`, e aprire `Assets/Scenes/Home.unity`.
Per Android servono i moduli Android Build Support, SDK, NDK e OpenJDK della stessa
versione dell'Editor. Le risorse sono in Git ordinario; Git LFS non è necessario.

Usare `README.md` e `docs/GETTING_STARTED.md` per la configurazione,
`CONTRIBUTING.md` per i contributi, `docs/ARCHITECTURE.md` quando si modificano
sistemi o salvataggi e `docs/ASSETS_AND_AUDIO.md` per grafica e narrazione.
Consultare `docs/VERIFICATION.md` per scegliere le prove pertinenti e preparare
una distribuzione. Le note in `docs/DEVELOPMENT_HISTORY.md` sono evidenze storiche,
non una sequenza di configurazione da rieseguire.

## Comandi di controllo

Eseguire dalla radice, con Git e Python 3.10 o successivo:

```powershell
python tools/check_repository.py
python tools/check_site.py
git diff --check
```

I due script Python usano la libreria standard e sono quelli del flusso
`.github/workflows/repository-checks.yml`. Controllano integrità dei sorgenti e
sito pubblico; non compilano Unity né verificano gioco o dispositivi.
Gli strumenti `AgentScripts/Verify*.cs` vanno scelti e letti secondo precondizioni
e punti di ingresso descritti nella guida alle verifiche. Non avviare in blocco
gli strumenti storici `Add*`, `Install*` o `Prepare*`: possono modificare scene
o versioni. Dichiarare sempre quali prove sono state eseguite e quali restano
da eseguire, con le relative cause.

## Integrità delle risorse e del codice

- Conservare insieme `Assets`, `Packages` e `ProjectSettings`. Spostare o rinominare
  le risorse tramite Unity insieme ai relativi `.meta`, mantenendo i GUID.
- Tenere il codice eseguito nel gioco in `Assets/Scripts`, principalmente nel
  namespace `MrRabbit`; l'automazione dell'Editor resta in `AgentScripts` fuori
  dalle compilazioni del gioco. Ogni `MonoBehaviour` ha il proprio file omonimo.
- Mantenere coerenti `Packages/manifest.json` e `Packages/packages-lock.json`.
  Usare `.editorconfig`, Visible Meta Files e serializzazione Force Text.
- Evitare riformattazioni estese e rinomine dei campi serializzati senza gestire
  i riferimenti della scena. Cache, compilazioni, registri, acquisizioni massive,
  copie dei salvataggi e materiali di firma restano esclusi da Git.
- L'identità Git e l'autenticazione restano nelle istruzioni globali e negli
  strumenti locali; non copiarle in questo file.

## Regole di revisione

- Segnalare perdita di GUID o riferimenti serializzati quando cambiano risorse
  e scene; preservare i `.meta` e verificare la scena Home.
- Segnalare modifiche che azzerano salvataggi, ricompense o progressi della guida;
  preservare le chiavi esistenti o gestire esplicitamente una migrazione compatibile.
- Segnalare rete, raccolta di dati o permessi Android incompatibili con il gioco
  offline e la privacy inclusa; mantenere le dichiarazioni coerenti con il codice.

Aggiornare comandi e vincoli quando cambia il progetto. Le istruzioni seguenti
conservano i requisiti educativi, vocali e di rilascio già presenti nel repository.
Riferimento: [documentazione ufficiale OpenAI su AGENTS.md](https://learn.chatgpt.com/docs/agent-configuration/agents-md).

# Documentazione e obiettivi educativi

Tutta la documentazione del progetto deve essere in italiano: README, guide,
cronologia, note di rilascio, istruzioni per gli agenti e modelli per segnalazioni
e richieste di integrazione. Conservare percorsi, identificatori tecnici, comandi
e diciture dell'Editor necessari al loro utilizzo. Licenze di terzi e materiali
sorgente originali conservano il proprio testo per tutelarne integrità e provenienza.

Il README deve spiegare chiaramente che il gioco segue il programma scolastico
italiano della prima elementare (classe prima della scuola primaria) e che la
scuola è il fulcro dell'esperienza e dell'avanzamento. Distinguere questo obiettivo
dalla copertura attuale: competenze selezionate di italiano e matematica, senza
affermare che tutte le materie o l'intero programma siano già implementati.

# Verifiche di rilascio

L'utente richiede una regressione funzionale completa prima di ogni rilascio.
Prima di creare il pacchetto o installare una versione, provare tutte le funzionalità
disponibili, la navigazione, i comandi, i progressi e la compatibilità dei salvataggi.
Ispezionare le schermate renderizzate per individuare anomalie, immagini tagliate
o mancanti, interfacce sovrapposte e comandi irraggiungibili o fuori schermo.
Includere rapporti d'aspetto rappresentativi dei tablet (4:3, 16:10 e il tablet
collegato) e verificare i veri bersagli dei tocchi e i raycast dell'interfaccia,
oltre alle chiamate dirette ai metodi. Correggere i problemi e ricontrollare i
percorsi interessati prima della distribuzione. Preservare i salvataggi reali;
le prove che modificano i progressi devono ripristinarli. Dopo l'installazione,
verificare versione installata, avvio e registri del dispositivo. Annotare nelle
note di rilascio copertura, correzioni e limiti. Non dichiarare una verifica su
dispositivo fisico quando è stata eseguita soltanto una simulazione.

Il pubblico principale è composto da bambini di 6–8 anni. Conservare il testo
essenziale degli esercizi di lettura, brevi indicazioni di orientamento e numeri;
comunicare le altre azioni con disegni o icone riconoscibili ed evidenziazioni
contestuali.

L'apprendimento scolastico costituisce il percorso principale. Luoghi futuri,
attività cittadine ed economia devono sostenerlo. Il gioco è ottimizzato per
l'italiano e per la sequenza di apprendimento della prima primaria italiana;
non aggiungere interfacce multilingue o un selettore della lingua. Le medaglie
si conquistano con la pratica e brevi verifiche variate, sbloccano ricompense
e si possono appendere e riordinare in casa. Preservare medaglie guadagnate,
oggetti esistenti, valute e vecchi salvataggi. Le sottrazioni restano escluse.
Non dichiarare la copertura dell'intero programma nazionale: le attività
riguardano competenze selezionate di letto-scrittura e matematica, con prerequisiti
e progressione al ritmo del bambino.

Fornire brevi indicazioni vocali italiane offline, con riascolto e preferenza audio
salvata; evitare annunci automatici ripetuti e sovrapposizioni dei comandi a schermo.
L'introduzione della versione 0.10.0 è una procedura guidata al primo avvio, seguita
da indicazioni vocali contestuali e icone di azione animate o evidenziate nel gioco.
La voce femminile italiana dell'introduzione e della narrazione deve risultare
calma, calda, confortevole e rassicurante, come quella di una mamma. Mantenere la
scuola centrale, mostrare un'azione utile alla volta e avanzare solo dopo azioni
effettivamente riuscite, mai alla fine della voce o tramite timer. Tenere i progressi
del tutorial separati dalle ricompense scolastiche conquistate e dall'inventario.
Supportare riascolto, pausa, salto e ripresa di un'introduzione incompleta. Non
suggerire mai acquisti, raccolti, ricette o consegne non disponibili. Mani e
evidenziazioni decorative non devono intercettare i tocchi. L'icona di ingresso
in casa deve seguire la porta dipinta, compreso l'adattamento dell'immagine ai
diversi rapporti d'aspetto dei tablet.

L'utente ha approvato it-IT-ElsaNeural e la richiede esplicitamente per ogni frase:
guida, lezioni, notifiche, attività future e introduzione. Preservare velocità −5%,
volume −5% e intonazione naturale approvati in ArtSources/ItalianVoice.profile.json.
Il campione di confronto approvato è
ArtSources/VoiceAuditions/Guida-naturale-elsa-prova.mp3. Non sostituirla con Windows
Elsa Desktop (rifiutata perché meccanica), Isabella o altre voci. Conservare la
riproduzione offline; generare l'audio durante lo sviluppo con
AgentScripts/PrepareItalianVoice.py. Anche le notifiche devono usare la voce italiana
insieme all'icona pertinente, comprese le richieste d'acqua dell'orto. Mantenere i
promemoria calmi, contestuali e limitati nella frequenza.

Il riconoscimento della scrittura deve tollerare le variazioni dei bambini e
classificare il segno indipendentemente dalla soluzione attesa. Conservare il
rifiuto di risposte sbagliate, spazi vuoti e scarabocchi; aggiungere esempi variati
a mano libera alle prove di regressione quando si modifica il riconoscimento.

# Pubblicazione e privacy

L'utente ha scelto Google Play, distribuzione gratuita senza pubblicità né acquisti
in-app, nome pubblico SofiaAntonio Games ed email pubblica antoniocanza@gmail.com.
La scuola rimane il fulcro. Non introdurre servizi di rete, analisi o raccolta di
campioni di scrittura. La configurazione Android offline e il backup disabilitato
devono corrispondere all'informativa inclusa nel gioco. Non modificare il nome
aziendale interno di Unity senza gestire la compatibilità dei PlayerPrefs su PC.

Il testo canonico della privacy è Assets/Resources/InformativaPrivacy.txt;
rigenerare la pagina pubblica con tools/export_privacy.py quando viene aggiornato.
Conservare chiavi di firma e password fuori dal repository. Non dichiarare la
pubblicazione completata finché Google Play non conferma la disponibilità.
