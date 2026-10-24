# Architettura

## Progetto e scena

La radice del repository è un progetto Unity completo. `Assets/Scenes/Home.unity` è l'unica scena di gioco abilitata nella compilazione. Gran parte del mondo illustrato è realizzata con viste uGUI, immagini e grafica personalizzata, anziché con geometria di gioco basata sulla fisica. URP usa le impostazioni del renderer 2D presenti nel repository.

Gli script eseguiti nel gioco sono in `Assets/Scripts`, principalmente nel namespace `MrRabbit`. `AgentScripts` contiene strumenti riservati allo sviluppo per configurazione delle scene, ispezione e regressione, fuori dalla normale compilazione Unity del gioco. Mantenere questa separazione.

## Mappa dei sistemi

| Sistema | File sorgente principali | Responsabilità |
| --- | --- | --- |
| Personaggio e movimento in casa | `CharacterSelection`, `RabbitGraphic`, `RabbitWalker`, `HousePointerInput` | Scelta e aspetto del coniglio, percorsi collegati tra piani e scale, comandi in casa |
| Comandi di movimento condivisi | `RelativeDragGesture`, `OutdoorPointerInput`, `CityPointerInput`, `VisitPointerInput` | Direzione dei gesti, comportamento al rilascio e instradamento del puntatore nei diversi luoghi |
| Campagna e amiche | `OutdoorAdventure`, `Neighborhood`, `MeadowFriend`, `SharedMeadowLandscape` | Uscita da casa, calzature, luoghi nel paesaggio scorrevole, visite e abitanti |
| Giorno/notte e inquadratura | `DayNightCycle`, `SkyCycleGraphic`, `GameplayFraming`, `SafeAreaLayout`, `HomeEntranceAnchor` | Tempo condiviso, leggibilità del paesaggio, aree sicure e allineamento della porta |
| Scuola e scrittura a mano | `SchoolController`, `SchoolLesson`, `SchoolWordPicture`, `HandwritingPad`, `HandwritingRecognizer`, `SchoolFeedback` | Attività in classe, consegne illustrate, acquisizione dei tratti, classificazione indipendente e riscontro |
| Percorso di apprendimento | `LearningJourney`, `LearningLesson`, `LearningProgress`, `MedalGraphic` | Undici competenze, pratica e verifiche, medaglie salvate e sblocco delle ricompense |
| Orto e cucina | `GardenController`, `GardenProgress`, `GardenArt`, `HomeActivities`, `KitchenVisuals`, `BedInteraction` | Ciclo delle colture, ingredienti, cucina, pasti e attività domestiche |
| Economia cittadina | `CityController`, `CityProgress`, `CitySymbol`, `RewardArt` | Viaggi in treno, richieste di prodotti, consegne, monete e vestiti |
| Guida e voce | `GameGuide`, `GameGuideProgress`, `ItalianVoice`, `ActionCue`, `GuideHandGraphic`, `GuideFocusGraphic` | Introduzione riprendibile, suggerimenti contestuali, voce offline e indicazioni decorative |

La tabella riporta i nomi dei file C# senza il suffisso `.cs`. Leggere i componenti effettivi e i collegamenti serializzati della scena prima di modificare un'attività: i riferimenti dell'interfaccia e dei controller sono conservati insieme nella scena.

## Progressi e compatibilità dei salvataggi

Il gioco conserva i progressi locali in chiavi `PlayerPrefs` versionate, tra cui:

| Chiave | Contenuto |
| --- | --- |
| `MrRabbit.School.v1` | Progressi in classe |
| `MrRabbit.Garden.v1` | Progressi dell'orto e delle colture |
| `MrRabbit.City.v1` | Monete, ordini e vestiti posseduti o selezionati |
| `MrRabbit.Learning.it-IT.v1` | Competenze di italiano e matematica, medaglie conquistate e ricompense collegate |
| `MrRabbit.Guide.it-IT.v1` | Pagina dell'introduzione e stato della guida contestuale |
| `MrRabbit.Voice.Muted` | Preferenza di silenziamento della narrazione |
| `MrRabbit.TV.Muted` | Preferenza audio della TV |

Fare riferimento alle costanti nel codice. Le modifiche devono preservare vecchi salvataggi, ricompense, inventario e oggetti posseduti. Rivedere o saltare la guida non deve assegnare ricompense né azzerare l'apprendimento. Lo stato della guida rimane separato dalla progressione.

## Contenuti e narrazione

Disegni originali, prompt di generazione, impostazioni vocali approvate e registrazioni della preparazione audio si trovano in `ArtSources`. Sprite e audio usati nel gioco sono in `Assets`. `ItalianVoice` riproduce le registrazioni incluse; la generazione durante lo sviluppo usa il copione canonico e il profilo vocale approvato. Consultare [Risorse grafiche e audio](ASSETS_AND_AUDIO.md).

## Confini del repository

- Includere le risorse del gioco insieme ai rispettivi file `.meta` e alle impostazioni condivise di progetto e pacchetti.
- Mantenere locali `Library`, acquisizioni massive, registri dei dispositivi, file generati degli IDE, compilazioni e copie dei salvataggi reali.
- Conservare una piccola galleria selezionata in `docs/images`: si tratta di copie di schermate registrate della versione 0.10.0.
- Il flusso di controllo dell'integrità del repository funziona senza una licenza Unity. Compilazione effettiva, prove di gioco e regressione di rilascio richiedono l'Editor e le verifiche appropriate sui dispositivi.
