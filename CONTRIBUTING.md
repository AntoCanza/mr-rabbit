# Contribuire a Mr Rabbit

Prima di modificare il progetto, leggere [README.md](README.md), [AGENTS.md](AGENTS.md) e la [guida all'architettura](docs/ARCHITECTURE.md). Usare Unity **6000.3.25f1** e aprire la radice del repository in Unity Hub.

## Principi di progettazione

- Progettare per bambini italofoni di 6–8 anni. Conservare il testo essenziale degli esercizi e brevi indicazioni di orientamento; usare disegni riconoscibili ed evidenziazioni contestuali per le azioni.
- Il gioco segue il programma scolastico italiano della prima elementare: la scuola è il fulcro dell'esperienza e dell'avanzamento. Attività nel mondo e ricompense sostengono l'apprendimento. I contenuti attuali coprono competenze selezionate di italiano e matematica; non dichiarare la copertura integrale del programma. Non aggiungere sottrazioni o un selettore della lingua.
- Scrivere tutta la documentazione in **italiano**, compresi README, guide, note di rilascio, cronologia, istruzioni e modelli per segnalazioni e richieste di integrazione. Conservare identificatori tecnici, percorsi, comandi e diciture dell'Editor necessari a usare il progetto. Le licenze di terzi e i materiali sorgente originali conservano il proprio testo.
- Preservare salvataggi esistenti, medaglie conquistate, monete, inventario, vestiti e progressi del tutorial incompleto. Tenere lo stato della guida separato dalle ricompense guadagnate.
- Conservare la narrazione calma approvata `it-IT-ElsaNeural`, il suo profilo e la riproduzione offline. I suggerimenti devono avanzare dopo azioni riuscite e rispettare la disponibilità reale delle attività.
- Le indicazioni decorative non devono intercettare i tocchi. Controllare i comandi nei rapporti d'aspetto dei tablet e rispettare l'area sicura.

## Lavorare a una modifica

1. Creare un ramo dedicato dal ramo predefinito attuale, per esempio `feature/garden-feedback` o `fix/school-touch-target`.
2. Spostare e rinominare le risorse tramite Unity, mantenendole insieme ai relativi file `.meta` e GUID. Conservare il codice del gioco in `Assets/Scripts` e l'automazione dell'Editor fuori dalle sue compilazioni.
3. Includere insieme le modifiche necessarie ad `Assets`, `Packages` e `ProjectSettings`. Mantenere sincronizzati entrambi i file JSON dei pacchetti. Non includere cache, acquisizioni massive, compilazioni, chiavi di firma o copie dei salvataggi dei giocatori.
4. Eseguire `python tools/check_repository.py`, poi le verifiche dell'Editor pertinenti descritte in [Verifiche](docs/VERIFICATION.md). Registrare cosa è stato effettivamente eseguito e i relativi limiti.
5. Aprire una richiesta di integrazione in italiano che spieghi il cambiamento visibile al bambino, le verifiche e l'eventuale impatto sulla compatibilità dei salvataggi. Allegare poche schermate utili per le modifiche visive.

Evitare riformattazioni estese dei sorgenti e spostamenti superflui: i riferimenti delle scene e i nomi dei campi serializzati sono rilevanti. Ogni `MonoBehaviour` deve avere il proprio file C# con nome corrispondente. Si può conservare lo stile compatto già usato nei file di gioco.

## Problemi e nuove funzionalità

Usare i modelli delle segnalazioni. Indicare versione dei sorgenti, versione Unity, dispositivo, risoluzione, passi per riprodurre il problema e comportamento atteso ed effettivo. Non inserire nomi di bambini, campioni di scrittura, salvataggi reali o altri dati personali nelle segnalazioni pubbliche.

## Rilasci

Eseguire la regressione completa delle funzionalità prima di creare il pacchetto o installare una versione, come richiesto da [AGENTS.md](AGENTS.md). Ripristinare i salvataggi reali dopo le prove. Distinguere chiaramente le simulazioni dei tablet dalle verifiche su dispositivi fisici. I soli controlli sui sorgenti non soddisfano i requisiti di rilascio.

## Licenze

Non è stata scelta una licenza generale di riutilizzo. Non sostituire le note esistenti e non introdurre nuove risorse di terzi senza registrarne origine e condizioni applicabili. Consultare [Risorse grafiche e audio](docs/ASSETS_AND_AUDIO.md).
