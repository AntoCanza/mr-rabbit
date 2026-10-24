# Verifiche e controlli di rilascio

## Integrità del repository

Eseguire da una copia Git con Python 3.10 o successivo:

```sh
python tools/check_repository.py
```

Il controllo usa soltanto la libreria standard di Python e Git. Verifica le cartelle Unity obbligatorie, la validità dei JSON dei pacchetti e le versioni fissate, i metadati visibili e la serializzazione testuale, la scena Home abilitata nella compilazione, l'abbinamento tra risorse e file `.meta`, l'unicità dei GUID e l'esclusione di file generati locali e materiali di firma. `.github/workflows/repository-checks.yml` lo esegue a ogni invio e richiesta di integrazione.

È un controllo dell'integrità dei sorgenti. Non avvia né compila Unity, non dimostra il comportamento del gioco, non esegue tutti gli strumenti di regressione e non valida una versione distribuibile.

## Proteggere prima i salvataggi reali

Leggere [AGENTS.md](../AGENTS.md) prima delle verifiche di gioco. Le prove devono preservare i progressi reali. Con la modalità Play arrestata, lo strumento esistente `AgentScripts/GuideSaveSafety.cs` può copiare tutte le chiavi correnti del gioco, della guida e delle preferenze audio in un nuovo file `Library/MrRabbitQA/guide-original-*.json`. Conservare il percorso restituito. Dopo le prove, arrestare Play e ripristinare la stessa copia. Non includerla mai in Git.

Il precedente `ReleaseSaveSnapshot.cs` copre un insieme storico più limitato e non deve sostituire la copia attuale che comprende la guida. Alcuni strumenti di verifica ripristinano anche le proprie chiavi, ma questo non elimina la necessità di una copia complessiva.

## Verifiche esistenti delle funzionalità

Si tratta di sorgenti da eseguire nell'Editor sotto `AgentScripts`, non di raccolte di test individuate automaticamente da Unity Test Framework. La maggior parte richiede la scena Home aperta e alcuni richiedono la modalità Play. Leggere punti di ingresso pubblici e precondizioni prima di eseguirli tramite l'interfaccia di scripting compatibile con l'Editor.

| Area | Strumenti rappresentativi |
| --- | --- |
| Movimento e casa | `VerifyHomeWalking`, `VerifyHoldMovement`, `VerifyRelativeMovement`, `VerifyHomeExterior`, `VerifyHomeActivities` |
| Mondo e amiche | `VerifyMeadow`, `VerifyFriends`, `VerifyNeighborhood`, `VerifyDayNight` |
| Orto e città | `VerifyGarden`, `VerifyGardenTouch`, `VerifyCityTravel`, `VerifyCityTouch` |
| Scuola e scrittura | `VerifySchool`, `VerifySchoolHints`, `VerifyWordPictures`, `VerifyChildWriting`, `VerifyFreeWriting` |
| Progressione e guida | `VerifyLearningJourney`, `VerifyGameGuide` |
| Interfaccia e presentazione sui tablet | `VerifyTabletExperience`, `ReleaseVisualPass`, `LearningVisualPass` |

Gli strumenti storici `Add*`, `Install*` e `Prepare*` possono modificare scene o reimpostare versioni. Non costituiscono una sequenza di configurazione o una suite di regressione da eseguire indiscriminatamente.

## Prima di creare il pacchetto o installare una versione

I requisiti completi di rilascio sono in [AGENTS.md](../AGENTS.md). Coprire tutte le funzionalità disponibili, i percorsi di navigazione, le modalità di comando e i salvataggi; verificare la compatibilità con i vecchi salvataggi. Ispezionare le schermate renderizzate per immagini mancanti o tagliate, interfacce sovrapposte, comandi fuori schermo e raggiungibilità effettiva mediante raycast e tocchi.

Includere 4:3, 16:10 e il rapporto d'aspetto reale del tablet collegato. Provare entrambi i conigli, calzature, casa e scale, giorno e notte, amiche e visite, colture, annaffiatura e raccolto, cucina e pasti, TV e sonno, viaggi in treno, consegne al mercato, vestiti, pratica e verifiche scolastiche, rifiuto e incertezza nel riconoscimento della scrittura, tutti i percorsi di medaglie e ricompense, riascolto, pausa, salto, ripresa e silenziamento della guida.

Ripristinare i salvataggi originali, correggere i problemi trovati e ricontrollare i percorsi interessati. Dopo l'installazione verificare la versione effettivamente installata, l'avvio e i registri del dispositivo. Registrare commit provato, risoluzioni, dispositivi, risultati, correzioni e limiti residui. Descrivere separatamente le prove su dispositivi fisici e quelle simulate.

## Evidenze storiche

La [Cronologia dello sviluppo](DEVELOPMENT_HISTORY.md) conserva, in traduzione italiana, i contenuti del precedente README e le sue note di implementazione e rilascio. Sono registrazioni storiche e includono istruzioni intermedie ormai superate. Per la configurazione attuale usare il README e la guida di configurazione correnti. Le evidenze storiche non costituiscono nuovi risultati per l'attuale pubblicazione del repository; l'archivio locale completo delle acquisizioni e dei registri non è incluso.
