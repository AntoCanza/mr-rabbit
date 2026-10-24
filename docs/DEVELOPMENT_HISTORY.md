# Cronologia dello sviluppo di Mr Rabbit

Traduzione italiana delle note storiche del precedente README. Date, versioni,
risultati delle prove e limiti si riferiscono al momento di ciascuna nota; alcune
istruzioni sono ormai superate. Per l'uso attuale consultare il [README](../README.md)
e la [guida di configurazione](GETTING_STARTED.md). Queste note non attestano nuove
verifiche sulla versione corrente dei sorgenti.

Prototipo 2D di casa e prato per un gioco per bambini su tablet, basato sui nostri disegni.

## Ferrovia, città e campagna continua — 18 ottobre 2026

**0.8.2 (13)** semplifica il movimento in casa, all'aperto, nelle case delle amiche e in città:
toccare una zona libera dello sfondo, muovere il dito nella direzione desiderata,
tenere premuto per camminare e sollevare il dito per fermarsi. La direzione segue
il movimento del dito indipendentemente dalla posizione del coniglio; per invertirla
basta un piccolo movimento opposto, senza attraversare il punto iniziale del tocco.
In casa i gesti verticali usano le scale vicino ai pianerottoli. Piccoli tremolii e
tocchi fermi vengono ignorati, un secondo dito non può sottrarre il controllo del
movimento e le icone conservano i normali tocchi. `VerifyRelativeMovement.cs` copre
questi casi, scale, rilascio e passaggio alle icone; superate anche le prove di
movimento continuo e di tocco sull'interfaccia reale alle dimensioni dei tablet.
L'APK complessivo 0.8.2 è stato compilato con zero errori e installato come
aggiornamento sul Galaxy Tab A7 Lite (codice versione del pacchetto 13). Scorrimenti
reali su spazi vuoti a sinistra del coniglio lo hanno mosso correttamente a destra
e a sinistra; il registro errori dell'applicazione in esecuzione era vuoto.

Aggiornamento dell'economia **0.8.1 (12)**: i vestiti costano 60 monete; ogni unità
di raccolto consegnata rende esattamente 1 moneta (3 prodotti → 3 monete, 6 prodotti
→ 6 monete). Il compenso delle richieste già accettate viene aggiornato conservando
prodotto, quantità, monete salvate e vestiti posseduti.

La versione **0.8.0 (11)** aggiunge una stazione in campagna a sinistra della scuola,
una mappa illustrata a due fermate e un viaggio in treno di sei secondi, con il
coniglio scelto visibile dal finestrino. In città si può camminare e visitare un
acquirente di prodotti e una boutique. Accettare un ordine, raccogliere la quantità
indicata e consegnare soltanto quella coltura in cambio di monete. Le offerte non
accettate cambiano ogni giorno di gioco; gli ordini accettati rimangono fino alla
consegna. Monete, proprietà, vestiti selezionati e ordini accettati vengono salvati.
Ogni coniglio ha un vestito acquistabile (60 monete), con immagini coordinate da
seduto per casa e scuola; i vestiti posseduti si possono indossare di nuovo gratis.

La campagna usa ora **un'unica texture di paesaggio ripetuta e specchiata** per cielo,
colline, erba e sentiero lungo tutte le 6,5 larghezze della vista. Casette, orto,
scuola e stazione sono livelli trasparenti. L'esterno della casa del giocatore è
visibile con un'indicazione sulla porta. Il cancello dell'orto ha un'immagine
contestuale animata più grande; avvisi di crescita, sete e raccolto usano solo
disegni. La cucina usa immagini di ingredienti e piatti, quantità, pentola e piatto,
evidenziando la prossima azione disponibile.

Nuove immagini e provenienza: `Assets/Art/City/`, `Assets/Art/Outdoors/SharedLandscape.png`,
i quattro luoghi `*Cutout.png` e `ArtSources/CityAndLandscape.prompts.txt`.
Conservati immagini originali e salvataggi. Le bozze scartate dei vestiti sono in
`ArtSources/CityDrafts/`.

Verifiche: `VerifyCityTravel.cs` controlla entrambi i conigli e le calzature,
annullamento della mappa, tempi, transazioni, prodotti o fondi insufficienti,
protezione dalle consegne duplicate, salvataggi, cambio gratuito dei vestiti,
pose da seduti, ritorno e ingresso in casa; ripristina i salvataggi originali.
`VerifyCityTouch.cs` controlla veri raycast ed eventi del puntatore dell'interfaccia
per cucina, stazioni, mappa, mercato e boutique a **1340×800, 1920×1200, 2048×1536,
2266×1488 e 2340×1080**. Superate anche le suite esistenti per vicinato, attività
domestiche, orto e scuola. Sono controlli rappresentativi di proporzioni e disposizione,
non prove su ogni modello di tablet.

APK: `Builds/Android/MrRabbit-0.8.0.apk`.
Compilazione riuscita con zero errori (APK di 59.025.173 byte). Installato come
aggiornamento sul Samsung SM-T220 / Galaxy Tab A7 Lite collegato; il pacchetto
conferma 0.8.0 / codice 11. Le prove di tocco sul dispositivo hanno coperto scelta
del personaggio e delle calzature, imbarco sul treno in campagna, mappa, arrivo in
città, cammino e apertura del mercato dei prodotti e della boutique. I prodotti
esistenti sono rimasti disponibili. Nessun errore nei registri Unity/AndroidRuntime
filtrati dell'applicazione in esecuzione.

## Aggiornamento del vicinato — 18 ottobre 2026

La versione 0.3.0 aggiunge l'inseguimento del personaggio con la telecamera in casa,
le casette visitabili di Coccodella e Zebrella, uscite casuali singole o insieme e
un'icona di avvio con il volto di Mr Rabbit. Conservate le immagini originali.
Le nuove immagini sono `Assets/Art/Outdoors/Neighborhood.png`, `CoccodellaInterior.png`,
`ZebrellaInterior.png` e `Assets/Art/Characters/MrRabbitIcon.png`. Prompt e riferimenti
della generazione integrata sono registrati in `ArtSources/Neighborhood.prompts.txt`
e `ArtSources/MrRabbitIcon.prompt.txt`.

`AgentScripts/VerifyNeighborhood.cs` controlla entrambi i personaggi e le calzature,
lo zoom invariato mentre la telecamera segue al piano superiore, entrambe le visite
e le uscite, movimento interno, orologio durante le visite, tutti e tre i gruppi
di uscita, ritorni e assenza in casa dell'abitante che si trova fuori. Superato a
1340×800 e 2340×1080 nella modalità Play dell'Editor; superate anche le suite
esistenti di cammino in casa e prato. Confermato l'avanzamento del ciclo di gioco.
`PreviewNeighborhood.cs` offre stati ripetibili per l'anteprima visiva.

Compilato `Builds/Android/MrRabbit-0.3.0.apk` (39.194.826 byte) con zero errori e
installato come aggiornamento sul Samsung Galaxy Tab A7 Lite collegato (SM-T220,
Android 14). I metadati confermano versione 0.3.0 / codice 3. Le prove di tocco hanno
coperto scelta del personaggio, salita a letto e sonno con inseguimento della
telecamera, risveglio e discesa, uscita e scelta dei pattini, visite a entrambe le
casette e ritorno al prato. Osservato il cammino casuale delle abitanti sul
dispositivo; le padrone di casa assenti erano correttamente nascoste all'interno.
L'icona col volto è stata controllata nell'elenco delle applicazioni Samsung e
usata per riaprire il gioco. Nessun errore nei registri Unity/AndroidRuntime
filtrati dell'applicazione. È una verifica funzionale sul dispositivo, non una
misura prolungata di prestazioni o batteria.

## Giocare insieme

Le istruzioni seguenti descrivono il prototipo di allora; alcuni comandi sono
cambiati nelle versioni successive.

1. Aprire il progetto in Unity Hub con **Unity 6.3 LTS (6000.3.25f1)**.
2. Aprire `Assets/Scenes/Home.unity`.
3. Premere **Play** in alto in Unity. Entrambi i conigli appaiono nella schermata iniziale.
4. Fare clic o toccare **Mr Rabbit (Male)** o **Miss Rabbit (Female)** per entrare in casa.
5. Usare **sinistra / destra** per camminare sul piano attuale.
6. Vicino alla base della scala tenere premuto **su** per salire; vicino alla cima tenere premuto **giù** per scendere.
7. Sulla scala, **su / giù** invertono la direzione. Anche **destra / sinistra** fanno salire o scendere.
8. Camminare o toccare il pavimento al piano terra vicino alla porta sinistra per uscire automaticamente. Toccando la porta si raggiunge l'uscita da entrambi i piani.
9. Scegliere **Scarpe normali** o **Roller** per uscire. **Resta in casa** annulla la scelta.
10. Proseguire verso **sinistra** per esplorare il prato scorrevole. I pattini sono visibili e permettono di muoversi più velocemente.
11. Avvicinarsi all'ingresso della casa a destra per rientrare automaticamente. Il coniglio indossa scarpe normali e compare abbastanza all'interno da non uscire subito di nuovo.
12. Toccare il letto al piano superiore per raggiungerlo tramite le scale e dormire. Toccarlo di nuovo per svegliarsi, oppure toccare un'altra destinazione sul pavimento per svegliarsi e camminare. La scelta del personaggio appare solo all'avvio; durante il gioco non c'è un pulsante per cambiarlo.
13. Premere di nuovo **Play** per fermarsi. La sessione successiva riparte dalla scelta del personaggio.

Si può anche fare clic o toccare un pavimento o una scala per raggiungerli. Il
coniglio segue i percorsi collegati e usa la scala per cambiare piano. Trascinare
modifica la destinazione; le frecce della tastiera assumono subito il controllo.
Entrambi i conigli si fermano alle pareti, tranne all'uscita sinistra del piano terra,
e non attraversano pavimento o soffitto lontano dalla scala. Il prato supporta
anche destinazioni con clic o tocco e ha un limite sinistro. Ogni uscita chiede
nuovamente quali calzature usare.

Il gioco riempie gli schermi orizzontali con paesaggi ingranditi e ritagliati in
proporzione. La telecamera della casa segue dolcemente il coniglio sui due assi,
conservando la scala esistente e fermandosi ai bordi del dipinto. Salendo si vede
la camera da letto, scendendo il piano terra. Non ci sono barra del titolo,
indicatore del tempo, pulsante di cambio personaggio o istruzioni fisse in basso.
Se la vista Game appare tagliata, ridurre il cursore Scale per adattarla alla finestra.

Due amiche hanno le proprie case nel prato: **Zebrella**, zebra con vestito lilla,
e **Coccodella**, coccodrilla verde con vestito blu. Toccare una casetta per
raggiungerne la porta e visitarla. Gli interni hanno temi distinti lilla e a righe,
oppure verdi con ninfee. Toccare il pavimento per camminare, la padrona di casa
per salutarla e la porta sinistra, oppure camminare a sinistra, per tornare al prato.
I pattini rimangono fuori e vengono rimessi al ritorno sul prato.
Entrambe iniziano in casa. Dopo 8–15 secondi di gioco attivo in casa o nel prato,
eventi casuali ogni 12–23 secondi possono far uscire Coccodella, Zebrella o entrambe.
Le uscite durano 14–26 secondi, poi ciascuna torna alla propria porta. Le abitanti
si fermano per salutare e mantengono la posizione tra casa e prato; chi è fuori
non compare nel proprio interno. I timer si fermano durante visite, scelta di
calzature o personaggio e sospensione dell'applicazione.
Fare clic o toccare un'amica o il suo nome mostra un saluto italiano e una piccola
oscillazione amichevole. Il saluto dura 4,5 secondi; un altro tocco lo riavvia.
Si può continuare a camminare. La nuvoletta resta nella vista e si chiude quando
l'amica esce dallo schermo, si torna a casa o si cambia coniglio.

Giorno, tramonto, notte e alba si alternano automaticamente. Un ciclo completo dura
**120 secondi** di gioco: 48 di giorno, 12 di tramonto, 48 di notte e 12 di alba.
Una nuova sessione Play inizia durante il giorno; la prima notte arriva dopo circa
46 secondi. La luce indica il periodo senza etichette dell'orario. Sole, luna e
stelle appaiono nel cielo della casa e sul prato. Il paesaggio cambia gradualmente,
mentre personaggi e comandi restano leggibili. Lo stesso orologio continua tra
casa e prato e si ferma durante le scelte di personaggio o calzature e la
sospensione dell'applicazione. Durante il sonno continua ad avanzare.

Come prima attività insieme, scegliere un nome per ogni stanza e decidere cosa
farvi fare al coniglio. All'epoca la casa era un'unica immagine di sfondo; il
coniglio camminava davanti ai mobili. Il letto permetteva sonno e risveglio;
altre interazioni con i mobili, suoni e obiettivi erano passi futuri.
Entrambi i conigli usano sprite trasparenti adattati dai nostri disegni, con
una piccola oscillazione mentre camminano e orientamento automatico destra/sinistra.

## Contenuti del progetto

- `Assets/Scenes/Home.unity`: scena iniziale e unica scena abilitata nella compilazione.
- `Assets/Art/Home/House.png`: immagine pulita per il gioco ricreata dal disegno originale.
- `ArtSources/Home.original.jpg`: disegno sorgente conservato.
- `ArtSources/House.prompt.txt`: prompt della generazione integrata e provenienza.
- `Assets/Art/Characters/MrRabbit.png` e `MissRabbit.png`: sprite trasparenti dei personaggi.
- `ArtSources/MrRabbit.original.jpg` e `MissRabbit.original.jpg`: disegni dei personaggi conservati.
- `ArtSources/MrRabbit.prompt.txt` e `MissRabbit.prompt.txt`: prompt della generazione integrata e provenienza.
- `Assets/Scripts/CharacterSelection.cs`: scelta del personaggio, accesso al gioco e ritorno alla selezione.
- `Assets/Scripts/RabbitGraphic.cs`: visualizzazione dello sprite e oscillazione del cammino.
- `Assets/Scripts/RabbitWalker.cs`: percorsi collegati tra piani e scala, frecce e percorsi da tocco.
- `Assets/Scripts/HousePointerInput.cs`: destinazioni tramite mouse e tocco su tali percorsi.
- `Assets/Scripts/OutdoorAdventure.cs`: uscita, calzature, movimento e telecamera nel prato, ritorno con scarpe.
- `Assets/Scripts/OutdoorPointerInput.cs`: destinazioni tramite mouse e tocco nel prato.
- `Assets/Scripts/DayNightCycle.cs`: orologio condiviso, transizioni della luce e pause per menu e sospensione.
- `Assets/Scripts/SkyCycleGraphic.cs`: sole, falce di luna e stelle vettoriali in una mesh dell'interfaccia per luogo.
- `Assets/Art/Outdoors/Meadow.png` e `Roller.png`: panorama e sprite trasparente del pattino.
- `ArtSources/Roller.original.jpg`: disegno conservato del pattino rosso con ruote gialle.
- `ArtSources/Outdoors.prompts.txt`: prompt della generazione integrata per entrambe le nuove risorse.
- `Assets/Scripts/SafeAreaLayout.cs`: disposizione nell'area sicura e obiettivo di 60 FPS.
- `AgentScripts/`: strumenti per Unity CLI, esterni ad Assets ed esclusi dalle compilazioni del gioco.

Il progetto usa URP 2D, uGUI e Input System. Lo sfondo non ha mipmap o copia leggibile
dalla CPU e usa impostazioni mobili ASTC 6x6. La scena non ha post-elaborazione né
simulazione fisica. Consumo effettivo di memoria e prestazioni richiedono ancora
misurazioni sui dispositivi. Ogni texture dei personaggi è limitata a 512 pixel
sul lato maggiore, usa ASTC 6x6 mobile e non ha mipmap o copia leggibile dalla CPU.
I PNG sorgente a piena risoluzione rimangono su disco. Ritratti del menu e giocatore
condividono gli sprite. La casa è inattiva finché non viene fatta una scelta.
Il prato usa un panorama fino a 2048 pixel e riutilizza uno sprite del pattino da
512 pixel per entrambi i piedi e la scheda di scelta. Entrambi usano ASTC 6x6 mobile
senza mipmap o copie leggibili dalla CPU. È attiva solo la vista attuale di casa
o prato; questa funzionalità non usa fisica o creazione di oggetti a ogni fotogramma.
Giorno e notte riutilizzano i dipinti con tinte e due piccole mesh dell'interfaccia.
Non aggiungono texture bitmap, luci dinamiche o post-elaborazione. Gli aggiornamenti
di cielo e tinta sono limitati a 20 Hz; il movimento mantiene l'obiettivo di 60 FPS.
Modificare **Cycle Seconds** su **Safe Area / Day Night Cycle** per cambiare la durata
del ciclo. Le prestazioni vanno ancora misurate sull'hardware reale.

I percorsi usano coordinate relative all'immagine e si ridimensionano con la casa.
Selezionare **Player Rabbit** sotto **House Viewport / House** nella scena salvata
(o il personaggio scelto durante il gioco), con casa attiva e Gizmos della vista
Scene abilitati, per vedere i percorsi verdi dei piani e quello arancione della scala.
Il componente Rabbit Walker espone velocità, limiti delle pareti, portata dei
pianerottoli e punti della scala per regolazioni successive.

## Versioni e compilazioni

All'epoca la cronologia Git locale iniziava su `main`, senza repository remoto
configurato. Cache Unity, soluzioni generate, acquisizioni e compilazioni sono
escluse. Conservare in Git i file `.meta` delle risorse.

Android è configurato per ARM64 / IL2CPP. L'identificatore è `com.mrrabbit.home`
sia per Android sia per iOS, con versione `0.3.0` (codice versione Android 3);
confermare l'identificatore finale prima della configurazione degli store. La
versione 0.1.0 è stata installata e avviata su Samsung Galaxy Tab A7 Lite (SM-T220,
Android 14) il 17 ottobre 2026. Android si può compilare su questo PC Windows;
un rilascio iOS richiederà macOS/Xcode e configurazione della firma Apple.

L'integrazione beta ufficiale di Codex usa `com.unity.pipeline` 0.8.0-exp.1.
Indicare esplicitamente questo progetto nei comandi Unity CLI se un altro Editor è aperto.

## Verifiche del movimento

Controlli in modalità Play di Unity dell'8 ottobre 2026:

- Compilazione e Console senza errori o avvisi dopo la configurazione della scena.
- Dimensioni effettive della vista Game: tablet 1440 x 1080 e telefono orizzontale 2340 x 1080.
- Casa completa, titolo, personaggio e suggerimento visibili senza deformare le immagini.
- Prove automatiche di entrambe le pareti esterne sui due piani, blocco del movimento verticale lontano dalle scale, salita, discesa, inversione a metà scala e tempi lunghi dei fotogrammi.
- I percorsi da tocco usano le scale tra i piani, si fermano sulle destinazioni della scala e non teletrasportano il personaggio.
- Le destinazioni fuori limite restano sui percorsi; le frecce annullano il percorso da tocco.
- Frecce simulate con Input System salgono le scale e raggiungono una parete in veri fotogrammi Update.
- Un tocco simulato nella disposizione per telefono porta dal piano superiore a una destinazione al piano terra.

Per ripetere i controlli delle regole di movimento durante Play nella scena Home,
scegliere prima un personaggio, poi usare il comando Unity CLI `run_script` con
`AgentScripts/VerifyHomeWalking.cs` e punto di ingresso `VerifyHomeWalking.Run`.

Sono prove nell'Editor, non verifiche di tocco su dispositivi fisici, prestazioni o store.

Il 9 ottobre 2026 il personaggio è stato sostituito con Miss Rabbit. Le prove del
movimento sono state nuovamente superate; sprite trasparente, proporzioni, orientamento
destra/sinistra e posizione su piani e scala controllati nella vista Game 1440 x 1080.
La Console non presentava errori.

Il 9 ottobre 2026 è stato verificato anche il flusso di scelta del personaggio a
1440 x 1080 e 2340 x 1080. Entrambi i ritratti erano visibili; la casa rimaneva
inattiva prima della scelta. Un clic simulato ha scelto Mr Rabbit; un tocco simulato
è tornato al menu e ha scelto Miss Rabbit. Il cambio annulla la destinazione del
cammino e riposiziona il nuovo personaggio al piano terra. Superate le prove del
movimento per entrambe le scelte, senza errori in Console.

Il 15 ottobre 2026 `AgentScripts/VerifyMeadow.cs` (`VerifyMeadow.Run`) ha controllato
entrambi i conigli con entrambe le calzature: i percorsi verso la porta dal piano
superiore usano le scale, la parete in alto a sinistra non fa uscire, il menu delle
calzature blocca il movimento, l'annullamento torna in sicurezza, il cammino a
sinistra scorre il prato, il limite sinistro arresta il movimento, i tocchi funzionano,
i tasti sostituiscono le destinazioni da tocco e il ritorno toglie i pattini e
riprende il cammino interno. Cambiare coniglio azzera lo stato esterno.
`VerifyHomeWalking.Run` è stato superato con il nuovo comportamento dell'uscita al piano terra.

Gli eventi Input System hanno verificato anche uscita con freccia sinistra, scelta
dei pattini con mouse, ritorno con freccia destra e scarpe automatiche, tocco sulla
porta d'uscita, scelta delle scarpe tramite tocco e tocco sul cartello di ritorno.
Il contatore dei fotogrammi avanzava. Ispezionate le viste tablet (1440 x 1080) e
telefono orizzontale (2340 x 1080). Compilazione e Console finale senza errori o
avvisi. Queste prove si svolgono nell'Editor; non è stata prodotta una compilazione
per dispositivo fisico né una misura delle prestazioni.

Giorno e notte verificati il 15 ottobre 2026 con `AgentScripts/VerifyDayNight.cs`
(`VerifyDayNight.Run`): durata e ripetizione del ciclo completo, gradualità dei
colori ai confini di ogni periodo, leggibilità dei personaggi, grafica del cielo
che non intercetta i tocchi e conservazione o pausa dell'orologio attraverso cambi
di luogo, entrambi i menu, cambi di personaggio e sospensione dell'applicazione.
Veri fotogrammi Update hanno attraversato tramonto/notte e alba/giorno. Un clic
simulato sulla porta d'uscita ha funzionato di notte. Ancora superate entrambe le
suite di movimento in casa e nel prato. Giorno, tramonto e notte controllati
visivamente alle dimensioni tablet e telefono. L'interfaccia color crema mantiene
il contrasto durante le transizioni. Compilazione e Console finale senza errori o
avvisi. Queste prove nell'Editor non attestano pubblicazione o validazione su dispositivo fisico.

## Amiche del prato

Aggiunte il 17 ottobre 2026. Entrambi i ritratti sono nuovi disegni realizzati con
il generatore integrato, usando Miss Rabbit come riferimento stilistico. I PNG
sorgente sono in `Assets/Art/Characters/`; i prompt esatti in `ArtSources/Friends.prompts.txt`.
Ogni sprite viene importato fino a 512 pixel con ASTC 6x6 Android/iOS, senza mipmap
o copia leggibile dalla CPU. La funzionalità non aggiunge fisica, Animator o particelle.

`MeadowFriend` gestisce i saluti; il livello `Meadow Friends` scorre con il paesaggio
e viene disegnato sopra le decorazioni del cielo. `AgentScripts/AddFriends.cs`
configura la scena una sola volta, con protezioni. `AgentScripts/VerifyFriends.cs`
controlla entrambi i conigli con scarpe e pattini, tempi e ripetizioni dei saluti,
leggibilità notturna, movimento durante i saluti e pulizia al ritorno in casa o
al cambio di personaggio.

Verificato in modalità Play a 1440 x 1080 e 2340 x 1080: veri eventi mouse di Input
System e tocchi simulati hanno aperto i saluti corretti senza muovere il coniglio.
Controllati i limiti delle nuvolette a entrambi i bordi; fotogrammi attivi hanno
confermato scadenza e chiusura fuori schermo. Nomi e ritratti ispezionati di giorno
e di notte. Superate le prove esistenti di prato e ciclo giorno/notte. Compilazione
e Console finale senza errori o avvisi. Sono prove nell'Editor; prestazioni sui
dispositivi fisici e pubblicazione restano attività separate.

## Aggiornamento tablet 0.2.0 — 17 ottobre 2026

Il gioco riempie ora lo schermo: `GameplayFraming` ritaglia i margini decorativi
conservando proporzioni delle immagini e coordinate dei percorsi. La scelta del
personaggio appare solo all'avvio. `BedInteraction` conduce entrambi i conigli
su per le scale, mostra sul letto dipinto una posa orizzontale con respiro e li
sveglia al secondo tocco sul letto o scegliendo una destinazione sul pavimento.
La vicinanza alla porta funziona con normali tocchi sul pavimento e frecce. Le
posizioni di ritorno o annullamento evitano una seconda uscita immediata. La scelta
tra scarpe e pattini rimane parte dell'uscita da casa.

Superate le prove nell'Editor a 1340x800, 1440x1080 e 2340x1080, compresi i limiti
visibili di pavimento e testa. `VerifyTabletExperience.Run` controlla entrambi i
personaggi, bersaglio del letto, percorsi sulle scale, sonno, risveglio e annullamento,
transizioni per vicinanza e assenza di rimbalzi sulla porta. Superate anche le suite
esistenti di casa, prato, amiche e giorno/notte. Compilazione Android ARM64/IL2CPP
riuscita con zero errori.

Installata 0.2.0 (codice 2) sopra 0.1.0 su Samsung SM-T220 / Android 14. I comandi
sul dispositivo hanno verificato scelta iniziale, casa a schermo intero, tocco sul
letto per salire e dormire, tocco sul pavimento per svegliarsi e avvicinarsi
all'uscita, scelta dei pattini e ritorno avvicinandosi alla casa senza toccare il
cartello. Nessun errore Unity/AndroidRuntime nei registri dell'applicazione durante
le prove. Acquisizioni in `Captures/Android/v020-*.png`.

## Vicinato e orto — 0.4.0, 18 ottobre 2026

La telecamera della casa segue il coniglio al piano superiore senza cambiare le
proporzioni dell'immagine. Coccodella e Zebrella hanno case separate visitabili e
camminano casualmente fuori, da sole o insieme. L'icona Android usa il volto di Mr Rabbit.

Il prato si estende ora a sinistra fino a un orto lontano. Toccare l'icona del
cancello per raggiungerlo ed entrare nella vista dedicata. Tre aiuole indipendenti
seguono una guida illustrata: prendere la zappa, preparare un'aiuola, scegliere semi
di carota, grano, pomodoro o fragola, seminare, annaffiare ripetutamente, poi prendere
il cesto e raccogliere. Ogni raccolto dà tre prodotti e riporta l'aiuola a terreno
non preparato. Attrezzi, aiuole, ingressi, saluti, sonno e scelte dei menu hanno
icone; le etichette sono secondarie. Le nuove immagini usano semplici disegni a
pastello o matita ispirati al coniglio originale. Sorgenti e prompt esatti:
`Assets/Art/Garden/` e `ArtSources/Garden.prompts.txt`.

La crescita segue il ciclo condiviso di 120 secondi per giorno di gioco. L'acqua
dura 0,4 giorni; le carote maturano in 1 giorno, grano e fragole in 1,25, pomodori
in 1,5. Le piante asciutte si fermano senza morire. Sono visibili seme, germoglio,
crescita e maturazione. Una notifica cliccabile nel prato indica nuova crescita,
sete o raccolto; se servono acqua e raccolta contemporaneamente compaiono entrambe
le icone. Aiuole, acqua, crescita e quantità raccolte vengono salvate in
`MrRabbit.Garden.v1` PlayerPrefs. I progressi si salvano dopo le azioni, ogni cinque
secondi di crescita e alla sospensione. Il tempo con applicazione chiusa non fa
crescere le piante. I menu di scelta e calzature fermano l'orologio di gioco.

`VerifyGarden.Run` controlla ogni coltura, limiti dell'acqua, pause per terreno
asciutto, indipendenza delle aiuole, salvataggi, raccolta e ripristino, entrambi
i conigli e le calzature, spostamenti, notifiche e pausa nei menu.
`VerifyGardenTouch.Run` attende fotogrammi renderizzati e controlla raycast ed eventi
del puntatore per sonno, cancello, attrezzi, simboli delle aiuole, semi, uscita e
avvisi. Superati a 1340x800 e 2340x1080. Superato anche `VerifyNeighborhood.Run`
dopo l'espansione del prato. Contatore dei fotogrammi in avanzamento e Console
finale dell'Editor senza errori di esecuzione.

Gli strumenti di creazione della scena sono fuori da Assets: `AddGarden.Run`
configura una sola volta con protezioni; `PolishGarden.Run` applica limiti finali
dell'atlante, terreno e disposizione delle icone. `GardenArt` campiona i contorni
dei singoli disegni organici senza presumere una griglia rigida.

## Stile illustrato uniforme e azioni evidenziate — 0.5.0, 18 ottobre 2026

L'originale `Assets/Art/Home/House.png` è ora il riferimento visivo per tutte le
immagini attive: contorni marroni caldi, colori pastello morbidi e forme semplici
da libro illustrato. I quattro personaggi conservano sagome, abiti e colori
originali. Interni delle amiche, vicinato, orto, terreno, attrezzi, colture, immagini
delle azioni, pattini e icona di avvio sono stati ridisegnati con il generatore
integrato. La casa originale è invariata. Prompt esatti in
`ArtSources/UnifiedStyle.prompts.txt`; immagini precedenti conservate in
`ArtSources/BeforeStyleUpdate/`. `GardenArtSlot` conserva l'identità delle icone
tra aggiornamenti dell'atlante; `GardenArt` usa i contorni misurati dei nuovi disegni.

`ActionCue` sostituisce la freccia dell'orto con un'evidenziazione calda pulsante
e un lieve movimento dell'immagine. Si evidenzia solo il passo attuale: prima
l'attrezzo, poi l'aiuola bersaglio; la scelta dei semi mostra quattro opzioni
ugualmente valide. Le indicazioni si fermano mentre si cammina o lavora e cambiano
a completamento. Dopo la raccolta la guida torna alla preparazione del terreno.
Gli altri pittogrammi cliccabili si muovono lievemente quando inattivi; i comandi
dell'orto restano fermi se non indicano il passo successivo. I bersagli dei tocchi
non si muovono. `PrepareActionCues.Run` configura l'interfaccia esistente senza
sostituirla; la vecchia freccia rimane inattiva.

Superato `VerifyGarden.Run` con le nuove immagini. `VerifyGardenTouch.Run` controlla
anche evidenziazioni esclusive, animazione effettiva, inattività delle altre azioni,
assenza di frecce guida e sequenza completa zappa, semi, acqua, raccolto e ripartenza
attraverso veri raycast dell'interfaccia. Superato a 1340x800 e 2340x1080 in nuove
sessioni Play. Gli strumenti di anteprima disabilitano intenzionalmente gli
aggiornamenti del mondo per le schermate; riavviare Play prima delle prove di interazione.

Compilazione Android riuscita con zero errori e l'avviso previsto sugli strumenti
Pipeline dell'Editor disabilitati nel gioco. `Builds/Android/MrRabbit-0.5.0.apk`
installato sopra l'applicazione esistente su Samsung SM-T220 (Android 14), confermata
0.5.0/codice 5. L'aiuola di carote salvata è sopravvissuta all'aggiornamento. Tocchi
sul dispositivo hanno verificato evidenziazione del solo annaffiatoio, passaggio
all'aiuola, annaffiatura, maturazione in tempo reale, avviso di raccolto cliccabile
nel prato, presa del cesto, raccolta (quantità 3) e ripartenza con la zappa evidenziata.
Nessun errore Unity/AndroidRuntime durante le prove. Schermate del dispositivo in
`Captures/tablet-050-*.png`; anteprime delle immagini nell'Editor in
`Assets/Captures/*-050*.png`.

## Attività domestiche — 18 ottobre 2026

Toccare la poltrona per raggiungerla e guardare la televisione con una posa seduta
dedicata a Mr Rabbit o Miss Rabbit. Lo schermo alterna tre programmi di paesaggi
ogni tre secondi; toccare la TV cambia subito programma. Durante la visione si sente
un breve motivo originale sintetizzato e tranquillo. L'audio si può silenziare
(preferenza salvata); Alzati, movimento sul pavimento, letto e porta interrompono
l'attività e il suono.

La nuova postazione di cucina dipinta accanto al camino apre quattro ricette:
zuppa di carote, pane caldo, zuppa di pomodoro e composta di fragole. Un prodotto
raccolto diventa un piatto dopo tre secondi. Mangia porta il piatto al tavolo da
pranzo esistente al piano superiore, dove il coniglio scelto si siede e mangia.
Raccolti e piatti preparati condividono il salvataggio dell'orto; i salvataggi vecchi
mantengono i raccolti e iniziano senza piatti pronti. Ingredienti e pasti si
consumano solo ad azione completata, quindi annullare non fa perdere cibo.

HomeActivities gestisce queste interazioni reciprocamente esclusive. GardenProgress
conserva i piatti e valida le transazioni dell'inventario; GardenController salva
le modifiche riuscite. AddHomeActivities.Run crea la scena una sola volta, con
protezioni. Immagini trasparenti generate in
Assets/Art/Home/{MrRabbitSeated,MissRabbitSeated,CookingStation}.png. Prompt della
generazione integrata e provenienza dei riferimenti in ArtSources/HomeActivities.prompts.txt.

Verificati in modalità Play entrambi i personaggi, tutte le ricette dalla raccolta
al pasto, salvataggi, migrazione dei vecchi salvataggi, inventario vuoto, tocchi
ripetuti, annullamento e passaggi a letto, porta e scelta del personaggio.
VerifyHomeActivities.CheckTouch superato a 1340x800 e 2340x1080 dopo aver atteso i
fotogrammi dell'interfaccia renderizzata. VerifyTabletExperience.Run superato per
entrambi i personaggi. TV, cucina con vapore e pasto da seduti ispezionati visivamente;
dopo la correzione del renderer, compilazione e controlli finali non hanno segnalato
nuovi errori. Schermate in Assets/Screenshots/home-*.png. Questa modifica non era
stata compilata o installata su Android al momento della nota.

## Installazione sul tablet — 0.6.0, 18 ottobre 2026

Compilato Builds/Android/MrRabbit-0.6.0.apk (44.108.971 byte) con zero errori.
Gli avvisi riguardano Pipeline disabilitato nel gioco e formati deprecati delle
icone di avvio tradizionali e rotonde. Installato come aggiornamento su Samsung
SM-T220; Android conferma 0.6.0/codice 6. Avvio e tocchi sul dispositivo hanno
verificato scelta del personaggio, ricette e seduta davanti alla TV. La dispensa
mostra i raccolti esistenti (6 carote, 3 grano, 3 pomodori, 3 fragole); nessun
ingrediente consumato durante le prove sul dispositivo. Nessun errore
Unity/AndroidRuntime nell'applicazione in esecuzione. Schermate:
Captures/Android/v060-{launch,home,kitchen,tv}.png. L'applicazione è stata lasciata
aperta con Miss Rabbit davanti alla televisione.

## Scuola e scrittura con il dito — 0.7.0, 18 ottobre 2026

Il prato misura ora 5,5 larghezze della vista (prima 3,5). Case e orto mantengono
le posizioni nel mondo; oltre l'orto si trova una scuola, raggiungibile con l'icona
animata del libro. Preparare zaino, merenda e borraccia a casa o all'arrivo.
Mr/Miss Rabbit si siede con Coccodella e Zebrella, ascolta la maestra gufo e sceglie
vocali mancanti, addizioni o sottrazioni. La maestra visita tutti e tre i banchi
e distribuisce le schede. Una matita abilita la scrittura col dito, una gomma
cancella, un libro mostra un modello tenue e la spunta conferma. Le risposte
corrette assegnano una stella salvata; gli errori invitano a riprovare senza
penalità. Le azioni usano immagini. Le sottrazioni, presenti in questa versione
storica, sono state rimosse nella 0.8.3.

SchoolLesson genera domande variate sulle vocali italiane e operazioni con risultati
0–10, con due aree di scrittura separate per il 10. HandwritingPad registra i
tratti; HandwritingRecognizer confronta indipendentemente modelli locali di vocali
e cifre, con varianti maiuscole e minuscole delle vocali, ordine e direzione dei
tratti flessibili e rifiuto dei segni ambigui. È un piccolo riconoscitore didattico,
non un OCR generale. I progressi scolastici hanno un salvataggio versionato
separato e non consumano l'inventario dell'orto. Le lezioni fermano l'orologio del mondo.

Immagini create con il generatore integrato e salvate in Assets/Art/School;
prompt completi e provenienza dei riferimenti in ArtSources/School.prompts.txt.
AddSchool configura la scena una sola volta, con protezioni. VerifySchool controlla
1.500 domande generate, 27 varianti di simboli deformati, invertiti o con tratti
riordinati, rifiuto di vuoto e scarabocchi, entrambi gli alunni, preparazione dello
zaino, viaggio, indicazioni per sedersi e ascoltare, distribuzione delle schede,
raycast ed eventi del dito, cancellazione, rifiuto delle risposte errate, ricompense
singole e ritorno al prato. Prove di interazione superate a 1340x800 e 2340x1080;
dopo modifiche a indicazioni e disposizione sono state superate di nuovo alla
risoluzione tablet. Superati anche VerifyHomeActivities, VerifyNeighborhood,
VerifyGarden e VerifyTabletExperience. Quest'ultimo ora attende i fotogrammi
dell'interfaccia dopo la scelta del personaggio prima del raycast del letto,
evitando risultati su una disposizione non aggiornata. Compilazione con zero errori.
Anteprime nell'Editor: Assets/Captures/school-*.png.

Rilascio Android: Builds/Android/MrRabbit-0.7.0.apk (54.860.017 byte), compilato con
zero errori e l'avviso previsto su Pipeline nel gioco. Installato come aggiornamento
su Samsung SM-T220; Android riporta 0.7.0/codice 7. I tocchi hanno verificato
preparazione dei tre oggetti, scorciatoia per la scuola e percorso esteso nel prato,
seduta, ascolto, scelta della materia e consegna della scheda. Tratti sullo schermo
Android hanno scritto una I errata per NAS_ (rifiutata), l'hanno cancellata e hanno
scritto O (riconosciuta e premiata). Un 8 scritto con due tratti ha risposto a 7 + 1
e ottenuto la seconda stella. Nessun errore Unity o AndroidRuntime nell'applicazione.
Dati dell'orto esistenti conservati dopo l'aggiornamento; nessun raccolto o piatto
consumato durante le prove. Il tablet è rimasto aperto in classe con due stelle.
Acquisizioni in Captures/Android/v070-*.png, comprese handwritten-o, handwritten-8
e math-reward.

## Movimento continuo al tocco — 0.7.1, 18 ottobre 2026

Il movimento nel prato usa ora una pressione mantenuta e acquisita, invece di
impostare ripetutamente una destinazione. Prima un dito fermo non generava più
eventi di trascinamento, quindi il coniglio raggiungeva l'ultimo punto e si fermava
anche tenendo premuto. Toccare il coniglio, scorrere a destra o sinistra e tenere
premuto per muoversi continuamente; attraversare il punto iniziale per invertire
o sollevare per fermarsi. Una piccola freccia traslucida sotto i piedi indica la
direzione. Un breve tocco sullo sfondo non agisce; tenere premuto accanto al
coniglio lo fa camminare in quella direzione dopo un breve ritardo di intenzione.
Le icone mantengono i percorsi automatici verso scuola, orto e case. Nessun nuovo
testo nell'interfaccia.

OutdoorPointerInput gestisce un solo puntatore, ignora le altre dita, acquisisce
il coniglio sopra i pulsanti di sfondo e rilascia alla fine del tocco, annullamento,
disabilitazione, perdita del fuoco o pausa dell'applicazione. OutdoorAdventure
combina la direzione mantenuta con il modello esistente di movimento limitato e
telecamera; i percorsi delle azioni sostituiscono in sicurezza il controllo manuale.
MovementDirectionGraphic è un pittogramma procedurale non interattivo.

VerifyHoldMovement superato a 1340x800 e 2340x1080 per entrambi i conigli e le
calzature: raycast reali sul personaggio, dieci secondi di pressione ferma durante
lo scorrimento, inversione, zona neutra, arresto immediato, isolamento del secondo
dito, pressione sullo sfondo, pausa e fuoco, bordo del mondo e percorsi verso
scuola e orto. VerifySchool superato dopo la modifica. Controllo visivo nell'Editor:
Assets/Captures/hold-direction.png.

Compilato Builds/Android/MrRabbit-0.7.1.apk (54.863.413 byte) con zero errori e
installato come aggiornamento su Samsung SM-T220; confermata 0.7.1/codice 8.
Un solo DOWN e MOVE sullo schermo, seguito da sei secondi di pressione ferma, ha
mantenuto lo scorrimento dalle case all'orto. Un MOVE oltre il punto iniziale ha
invertito la direzione senza sollevare il dito; UP ha tolto la freccia e due
acquisizioni successive hanno confermato la posizione ferma. Superati ingresso e
uscita dall'orto e scorciatoia della scuola con le icone esistenti. Nessun errore
Unity/AndroidRuntime. Inventario salvato dell'orto e corredo scolastico ancora
disponibili. Captures/Android/v071-{hold1,hold2,reverse,release,stopped,garden,school}.png
documenta le prove. L'applicazione è rimasta nel prato fuori dalla scuola.

## Vocabolario illustrato — 0.7.2, 18 ottobre 2026

Ciascuna delle 24 parole con vocale mancante ha ora un grande disegno corrispondente
accanto all'area di scrittura. SchoolWordPicture associa la parola italiana completa
a un rettangolo misurato dell'atlante: cambiare la vocale mancante non cambia il
soggetto. Le immagini non sono interattive; matita, gomma, aiuto e conferma mantengono
icone e bersagli dei tocchi. L'area di scrittura conserva le dimensioni precedenti.
L'aritmetica mantiene la disposizione centrale con una o due aree e toglie
l'illustrazione del vocabolario. Nessuna nuova etichetta o indicazione scritta nel gioco.

Creato con il generatore integrato usando SchoolIcons.png come riferimento stilistico.
Risorse finali: Assets/Art/School/WordPictures1.png e WordPictures2.png.
Prompt finali completi: ArtSources/WordPictures.prompts.txt. Tutti i soggetti e i
rispettivi ritagli nel gioco ispezionati in Assets/Captures/word-pictures-all.png;
Assets/Captures/word-picture-apple.png mostra la disposizione della scheda.

VerifyWordPictures superato: tutte le voci del vocabolario, 400 domande casuali,
rimozione delle immagini per l'aritmetica, cambi di materia e nessuna sovrapposizione
con la scrittura a 1340x800 e 2340x1080. VerifySchool superato dopo il cambio di
disposizione, compresi veri raycast dell'area di scrittura, tratti del dito,
cancellazione e ricompense per risposte corrette o errate. Il riscontro della
lettera riconosciuta appare sopra l'area della vocale, lontano dall'icona del libro
di aiuto; quello matematico mantiene la posizione precedente. Superato il controllo
di regressione sulle sovrapposizioni dopo questa regolazione finale.

Compilazione Android finale: Builds/Android/MrRabbit-0.7.2.apk, 59.262.713 byte,
zero errori. Installato come aggiornamento su Samsung SM-T220 con 0.7.2/codice 9.
La verifica sul dispositivo ha mostrato il disegno del mare per M_RE; tre tratti
sullo schermo hanno scritto A, riconosciuta con assegnazione di una stella. La
rifinitura finale della disposizione è stata ricompilata e reinstallata, poi è
stata riaperta la scheda illustrata. Nessun errore Unity/AndroidRuntime. Conservati
corredo, stelle e salvataggio dell'orto. Acquisizioni: Captures/Android/v072-*.png.

## 0.7.3 — aiuto scolastico e riscontri sonori

Il libro delle risposte rimane nascosto fino a quattro invii manoscritti errati
sullo stesso esercizio. Risposte vuote e conferme ripetute senza nuovi tratti non
contano. Il libro mostra il costo di una stella prima dell'uso; il primo utilizzo
la sottrae una sola volta senza scendere sotto zero ed esclude la ricompensa per
quell'esercizio. Il completamento assistito mostra una spunta. Un breve movimento
di diniego della testa della maestra gufo e un suono discendente segnalano l'aiuto.
Il promemoria si chiude automaticamente; nessuna spiegazione aggiuntiva o finestra
di conferma. Ascolto a scuola, nuovi tentativi e completamenti hanno ora suoni
sintetizzati offline. Volume TV aumentato da 0,12 a 0,40; conservato il comando audio.

Diagnosi audio: volume multimediale del tablet 7/15, non silenziato, diretto
all'altoparlante. Audio Unity abilitato con un ascoltatore attivo e volume principale
1. Prima solo la TV aveva suono; la scuola non aveva audio. L'udibilità fisica
dell'altoparlante non si può valutare da remoto. VerifySchool e VerifySchoolHints
hanno provato entrambi gli alunni e tutte e tre le materie, compresi rifiuto di
vuoti e duplicati, soglia dei quattro tentativi, aiuto ripetuto, minimo zero stelle
e completamento assistito senza ricompense. VerifyHomeActivities ha superato
riproduzione TV, silenziamento e riattivazione e passaggi tra attività. Promemoria
della maestra e costo dell'aiuto controllati visivamente nella vista Game.

APK di rilascio: Builds/Android/MrRabbit-0.7.3.apk (59.267.557 byte), compilazione
riuscita con zero errori; installato con -r su SM-T220 con 0.7.3 / codice 10.
L'ispezione della TV sul dispositivo ha inoltre rilevato la preferenza persistente
di silenziamento attiva (pulsante Audio: no). Cambiata in Audio: si e verificato
il nuovo stato. Android ha riportato la sessione AAudio di com.mrrabbit.home avviata,
non silenziata, deviceId 2; silenziamento principale disattivato e volume 1.
Salvataggi conservati. Acquisizioni del dispositivo v073-tv*.png.
Prova di scrittura sul dispositivo: MEL_ con disegno della mela; scritta I e inviata
tre volte separatamente, comprese conferme duplicate. Libro rimasto nascosto.
Dopo cancellazione e quarto invio di I, libro apparso col costo di una stella.
Cancellati i tratti di prova e lasciata pronta la scheda; non consumate le stelle
dell'utente. Nessun errore Unity/AndroidRuntime. Acquisizioni:
Captures/Android/v073-three-attempts.png e v073-four-attempts.png.

## 0.8.3 — interfaccia illustrata per bambini di 6–8 anni

La scuola offre ora solo vocali e addizioni. Rimossi materia, generatore, pulsante
e ascoltatori delle sottrazioni; gli indici di materia non validi vengono rifiutati.
Restano le 24 parole italiane illustrate, scrittura a mano, ricompense e aiuto.

L'orto usa attrezzi evidenziati e disegni delle aiuole al posto di passi scritti,
frasi sullo stato del terreno ed etichette di colture o attrezzi. Le quantità del
raccolto usano cesto e numero, separati dal portamonete. Le scelte delle calzature
mostrano una grande scarpa o un pattino. La selezione conserva titolo e nomi dei
personaggi; istruzioni ed etichette di genere sono nascoste. I saluti mostrano una
mano che saluta, la TV usa simboli di ritorno e altoparlante o silenzio, il riscontro
del pasto una spunta. Le schede della cucina conservano le immagini esistenti da
ingrediente a piatto e le quantità, senza testo ridondante.

Il rilascio mantiene treno e città, vestiti a 60 monete, prodotti a una moneta per
unità e movimento relativo al dito introdotti nelle versioni 0.8.0–0.8.2.
All'epoca non erano abilitate altre lingue. La nota storica proponeva che una futura
localizzazione traducesse le etichette residue e fornisse per ogni lingua parole
con vocali, risposte valide e immagini coordinate, evitando una traduzione meccanica
degli esercizi italiani. Questo indirizzo è stato superato dalla scelta italiana
della versione 0.9.0 e dalle istruzioni attuali del progetto.

Verifiche prima del pacchetto Android: 1.000 domande scolastiche casuali,
riconoscimento dei simboli e rifiuto di errori o vuoti; due alunni e due materie,
protezione contro la materia rimossa; aiuto dopo quattro tentativi; cicli completi
di orto, cucina, pasti e salvataggio; audio TV e cambi di attività; saluti illustrati
e visite. Veri raycast superati per scuola, cucina, mappa del treno, mercato e
vestiti a 1340x800, 1920x1200 e 2048x1536. Superate copertura del vocabolario
illustrato e transizioni matematica/vocali. Acquisizioni finali di orto, selezione,
calzature, cucina e scuola controllate visivamente. Script per riprodurre la
modifica: AgentScripts/SimplifyVisualUI.cs (modalità modifica, idempotente).
Destinazione Android: Builds/Android/MrRabbit-0.8.3.apk, codice versione 14.

La regressione completa ha inoltre trovato e corretto tre problemi funzionali o
visivi: lo sfondo condiviso della campagna riceve i gesti in tutto il mondo esteso;
i canvas delle azioni e notifiche dell'orto restano sopra il coniglio e accettano
tocchi quando si è al cancello; il treno di ritorno specchia le immagini senza
scalare negativamente il RectMask2D del passeggero, che rimane visibile in entrambe
le direzioni. Le evidenziazioni di stazione e negozi circondano le immagini delle
azioni senza creare un grande rettangolo giallo sull'edificio.

La scrittura attenua ora piccole oscillazioni, campiona ogni percorso a distanze
uniformi, normalizza le proporzioni bidimensionali conservando i tratti lineari
I/1 e confronta piccole correzioni di inclinazione. Copertura della forma, lunghezza
relativa, proporzioni e ambiguità continuano a rifiutare tratti incerti. Il
riconoscimento non riceve mai la risposta attesa. HandwritingPad memorizza i
risultati finché non cambiano tratti o modalità, evitando confronti ripetuti su
scrittura invariata. Non vengono usati riconoscimento o servizi remoti e non si
raccolgono campioni di scrittura.

I 324 campioni variati deterministici di VerifyChildWriting sono migliorati da
262 corretti, 6 classificazioni errate e 56 incerti a 316 corretti, 1 errato e
7 incerti. VerifyFreeWriting riconosce 15 esempi disegnati separatamente (anelli
aperti, angoli morbidi, barre staccate). Sono esempi sintetici o dello sviluppatore,
non misure dell'accuratezza sui bambini; la variabilità reale richiede ancora prove
di gioco. Superati rifiuto di vuoto, punto, barra orizzontale e scarabocchi,
simboli originali, rifiuto delle risposte sbagliate e percorsi di ricompense,
aiuto e salvataggi. Riferimento consultato:
[principi del riconoscimento a nuvole di punti](https://depts.washington.edu/acelab/proj/dollar/pdollar.html);
l'implementazione rimane il riconoscitore del gioco.

ReleaseVisualPass attraversa 38 stati renderizzati di gioco o menu a ciascuna
risoluzione 1340x800, 1920x1200 e 2048x1536: 114 acquisizioni e 147 raycast al
centro dei comandi, senza ulteriori comandi fuori schermo o coperti, testo attivo
traboccante o sovrapposizioni tra cesto e portamonete. Ispezionate visivamente tutte
le nove tavole riepilogative. Acquisizioni e checks.txt per risoluzione in
Captures/Release-0.8.3. Generare le tavole con AgentScripts/MakeReleaseContactSheets.ps1.
AGENTS.md del progetto richiede ora regressione funzionale e visiva completa prima
di ogni rilascio.

Pacchetto Android finale 0.8.3 riuscito con zero errori (APK di 59.029.089 byte).
Su richiesta dell'utente le prove finali restano su PC; questa versione NON è stata
installata sul tablet, che mantiene 0.8.2. Scrittura sul dispositivo e verifiche
dopo l'installazione rinviate a una nuova richiesta di installazione. La proposta
di riprogettare traguardi e ricompense intorno alla scuola resta una proposta;
questo rilascio migliora scuola e riconoscitore esistenti.

## 0.8.4 — casa più distanziata e colori coerenti delle casette

Casa di Mr Rabbit e destinazione d'ingresso spostate a destra di 0,8 larghezze
della vista. Campagna estesa da 6,5 a 7,3 larghezze, conservando coordinate originali
di case delle amiche, abitanti, orto, scuola e stazione. Lo sfondo ripetuto continua
nel prato aggiunto. L'altezza visualizzata della casa rientra ora sotto il bordo
superiore delle viste tablet. Un simbolo d'ingresso dipinto è accanto alla porta,
con priorità sui comandi del personaggio e senza coprirlo. La mappa del treno usa
lo stesso nuovo sprite della casa.

HomeExteriorHarmonized.png conserva l'architettura originale con pareti giallo
crema, finestre ciano chiare, ombre e contorni più delicati e tetto corallo
coordinato alla tavolozza delle amiche. Immagini originali conservate. Generato
con la modifica immagini integrata; riferimenti e prompt completo:
ArtSources/HomeExteriorHarmonized.prompt.txt. Aggiornamento idempotente della scena:
AgentScripts/SpaceAndHarmonizeHome.cs.

Superata la regressione funzionale completa: movimento in casa e scale, limiti
del prato, giorno e notte, sonno, risveglio e telecamera, entrambi i personaggi e
le calzature, visite e saluti, crescita, acqua e raccolto con guida ai tocchi reali,
cucina, pasti e TV, salvataggi e transazioni di treno, città, mercato e vestiti,
lettura, addizioni e aiuto scolastico, scrittura e copertura del vocabolario.
Percorso visivo: 38 stati a ciascuna risoluzione 1340x800, 1920x1200 e 2048x1536;
114 schermate e 150 raycast su comandi fissi, nessun problema di ritaglio,
sovrapposizione o comandi. Tutte le tavole ispezionate. Posizione finale dell'ingresso
verificata anche con veri raycast e tocco per entrare alle tre dimensioni
(VerifyHomeExterior). Acquisizioni: Captures/Release-0.8.4.

Modifiche pronte nell'anteprima Unity su PC. Installazione sul tablet ancora
rinviata dall'utente. Nessun APK 0.8.4 creato; compilarlo da questo stato verificato
quando verrà chiesta l'installazione. L'APK 0.8.3 precedente resta disponibile separatamente.

## 0.9.0 — apprendimento italiano, medaglie in casa e guida vocale

La maestra apre ora un quaderno visivo con due percorsi paralleli. Il gioco segue
letto-scrittura e matematica della prima primaria italiana; non ha selettore della
lingua o servizio di localizzazione. Si tratta di competenze selezionate, senza
dichiarare l'implementazione di tutte le materie o dell'intero programma nazionale.
La nota di sviluppo indica come riferimento le Indicazioni nazionali MIM
(D.M. 221/2025, prime classi dal 2026/27), tradotte nella sequenza del gioco al
ritmo del bambino:

| Percorso | Traguardi |
| --- | --- |
| Italiano | Vocali; nomi delle lettere pronunciati; scrittura delle maiuscole; sillabe; lettura di immagini e parole; brevi istruzioni scritte con risposte illustrate |
| Matematica | Contare e scrivere 0–10; numeri consecutivi fino a 20; addizioni entro 5, 10 e 20 |

Tre esercizi di pratica completati autonomamente abilitano una verifica di cinque
domande. Almeno quattro risposte corrette al primo tentativo assegnano la medaglia
permanente della competenza. Le verifiche usano domande casuali distinte, non hanno
timer e si possono ripetere. Una risposta errata fa avanzare dopo il riscontro;
scrittura incerta o vuota non conta come errore didattico. Chiedere un modello
trasforma la verifica in pratica libera; le risposte assistite non assegnano la
medaglia. Ripetere verifiche completate non duplica le ricompense. Anche la vecchia
pratica non sottrae più stelle per l'aiuto.

Le medaglie conquistate si possono scegliere, appendere in una delle dodici
cornici di casa, spostare senza duplicarle e vedere sulla parete effettiva. La
disposizione sopravvive al ricaricamento. Mantenuto il modello esistente di
progressi condivisi per entrambi i personaggi.

Le ricompense sono legate alle competenze: le vocali sbloccano i semi di zucca;
le somme entro dieci i mirtilli; la scrittura delle consonanti maiuscole il vestito
dorato; le somme entro venti la variante smeraldo; la lettura di brevi istruzioni
una decorazione per la casa. I nuovi vestiti costano 120/180 monete per personaggio
dopo lo sblocco; restano disponibili quello da 60 monete e tutti gli oggetti già
posseduti. Entrambe le colture speciali danno sei unità per raccolto. Ogni unità
si vende ancora per una moneta esatta. Le offerte del mercato includono le colture
disponibili e le richieste accettate restano fissate. Tutte e sei le colture si
possono cucinare e mangiare. Gli inventari dell'orto passano da quattro a sei voci,
conservando raccolti, pasti e aiuole in crescita.

101 registrazioni vocali italiane offline (Microsoft Elsa Desktop, it-IT) coprono
navigazione, attrezzi, acqua e raccolto, richieste del mercato, consegne delle
lezioni, lettere, aiuto e medaglie. I comandi superiori ripetono l'ultimo messaggio
e attivano o disattivano la voce automatica; la preferenza è salvata. Il riascolto
esplicito funziona anche con la voce automatica silenziata. I promemoria dell'orto
sono contestuali e limitati nella frequenza; non servono API vocali durante il
gioco, internet o voci installate sul tablet. Copione e preparazione riproducibile:
ArtSources/ItalianVoice.lines.json e AgentScripts/PrepareItalianVoice.ps1.
La voce desktop descritta qui è quella storica della 0.9.0, sostituita nella 0.10.0.

La scrittura supporta ora anche le consonanti maiuscole italiane in una modalità
alfabeto separata; il classificatore esistente per vocali e cifre viene conservato.
Il riconoscimento classifica sempre i tratti indipendentemente dalla soluzione attesa.

Nuove illustrazioni delle colture: Assets/Art/Garden/SchoolRewardCrops.png, generate
con ImageGen integrato usando l'atlante dell'orto come riferimento stilistico.
Prompt finale completo e modalità: ArtSources/SchoolRewardCrops.prompt.txt.
Medaglia e libro usano geometria dell'interfaccia indipendente dalla risoluzione.
I materiali dorato e smeraldo ricolorano i vestiti esistenti conservando pelo,
occhi e contorni, comprese pose sedute e sul treno. Strumenti di installazione e
rifinitura: InstallLearningJourney.cs e PolishLearningJourney.cs in AgentScripts;
modifiche della scena salvate tramite Unity.

Regressione funzionale completa superata per movimento, scale, sonno e risveglio,
giorno e notte, entrambi i personaggi e le calzature, amiche e visite, orto, cucina,
pasti e TV, treno e mappa, città, mercato e vestiti, scuola e aiuto, scrittura e
vocabolario esistenti. Nuova copertura: 1.100 consegne didattiche generate;
93 risposte effettive; tutte le undici medaglie; rifiuto con 3/5 e superamento con
4/5; protezioni per aiuto e tratti incerti; prevenzione dei duplicati; riordino e
ricaricamento delle medaglie; entrambi i nuovi cicli di coltura e vendite; tutti e
quattro i nuovi acquisti di vestiti (due varianti per personaggio); riproduzione,
silenziamento e riascolto in italiano. Il nuovo percorso controlla anche veri eventi
del puntatore per scelta delle risposte e collocazione delle medaglie.

La suite intensiva invariata per vocali e cifre mantiene 316 corretti, 1 errato e
7 incerti su 324 variazioni sintetiche; superati i 15 esempi indipendenti. L'estensione
alfabeto supera 16 consonanti deformate o invertite e 7 esempi disegnati in modo
indipendente. Sono prove deterministiche su PC, non misure dell'accuratezza della
scrittura dei bambini o prove sul tablet fisico.

Le verifiche renderizzate attraversano 73 stati a ciascuna risoluzione 1340x800,
1920x1200 e 2048x1536: 219 schermate, oltre 900 raycast su comandi e aree di scrittura,
controlli del testo traboccante e delle sovrapposizioni dei comandi a schermo.
Ispezionate tutte le quindici tavole riepilogative. Correzioni: pulsante indietro
dei semi sovrapposto alle nuove schede, rimappatura delle celle dell'atlante nelle
immagini clonate delle ricompense, separazione tra illustrazioni e scelte, animazioni
delle icone che alteravano i limiti delle schede, contatori di raccolto e ordini
sotto i comandi vocali e simbolo testuale di spunta non disponibile. Acquisizioni
e controlli in Captures/Release-0.9.0. Le prove ripristinano i progressi originali;
la copia di sicurezza è in Library/MrRabbitQA, fuori da Assets.

0.9.0 (codice versione Android 16) creata e installata su Samsung SM-T220 il
19 ottobre 2026. Compilazione Android completata con zero errori. Corretto prima
dell'installazione un problema di precisione del compilatore mobile in
RewardClothes.shader; il tablet reale mostra correttamente le anteprime dei vestiti
dorato e smeraldo. APK: Builds/Android/MrRabbit-0.9.0.apk (63.228.945 byte).
Non è stata prodotta una versione autonoma Windows.

Le prove rapide dopo l'installazione a 1340x800 orizzontale hanno coperto scelta
del personaggio da nuovo avvio, zero monete iniziali, casa, calzature, prato, mappa
della stazione, passeggero e viaggio visibili, movimento trascinato in città e
anteprime dei vestiti. Schermate ispezionate per disposizione e resa; nessuna voce
corrispondente a eccezioni Unity, arresti fatali o errori shader nel registro
dell'applicazione. Evidenze in Captures/Release-0.9.0/Tablet-2026-10-19. Sono prove
rapide sul dispositivo aggiuntive alla regressione completa su PC, non nuove misure
dell'accuratezza della scrittura sul tablet fisico. Dati dell'applicazione sul
tablet cancellati su richiesta dell'utente e cancellati di nuovo dopo le prove,
per far iniziare il bambino dalla scelta del personaggio senza progressi salvati.
I salvataggi PC non sono stati azzerati.

## 0.10.0 — introduzione di Elsa e guida contestuale

Il primo avvio apre ora un'introduzione illustrata di quattro pagine: casa ed
esplorazione, apprendimento scolastico e medaglie, orto e cucina, treno, città,
mercato e vestiti. Elsa legge ogni pagina; il bambino avanza con una freccia animata
e può tornare indietro, riascoltare, silenziare o saltare. Un'introduzione incompleta
riprende dalla pagina salvata. La fine della voce non fa mai avanzare pagina né
assegna progressi. La scuola rimane il percorso principale, con colture, vestiti e
parete delle medaglie presentati come sue ricompense.

Dopo l'introduzione, un dito animato e contorni pulsanti mostrano la prossima
azione utile nel contesto attuale. Non intercettano mai i tocchi. Le azioni sono
facoltative e il bambino può esplorare in un ordine diverso. La guida osserva
azioni reali riuscite, ricorda i passi appresi separatamente in MrRabbit.Guide.it-IT.v1
e copre scelta del personaggio, trascinamento, zaino, uscita e calzature, aula e
ascolto, pratica, scrittura e risposte a scelta, conferma, verifiche, medaglie e
cornici, ciclo dell'orto, cucina e pasti, TV, sonno e scale, mappa e treno, quantità
e consegne al mercato, vestiti, visite e saluti. I suggerimenti sulle risposte a
scelta evidenziano tutte le opzioni allo stesso modo senza svelare la risposta.
Valutazione dell'apprendimento e della scrittura esistenti invariate.

I suggerimenti rispettano i prerequisiti reali. Le piante crescono e hanno bisogno
d'acqua normalmente; la guida lascia esplorare durante la crescita. Ingredienti
mancanti, prodotti insufficienti, ricompense bloccate e vestiti troppo costosi
portano a spiegazioni e a un percorso di uscita o ritorno, mai a ricompense
inventate o acquisti automatici. Il libro guida con punto interrogativo apre
quattro brevi opzioni: introduzione, spiegazione del contesto attuale, pausa o
ripresa, salto del passo. Riascoltare la guida non azzera le ricompense.
Introduzione e menu d'aiuto bloccano tocchi e movimento sullo sfondo e fermano
l'orologio. La voce contestuale attende riscontri e istruzioni; le frasi superate
si fermano al cambio di contesto. I promemoria vocali di acqua e raccolto restano
limitati nella frequenza.

La voce it-IT-ElsaNeural approvata dall'utente viene usata per tutte le 168
registrazioni italiane offline, comprese 67 nuove frasi della guida, con velocità
−5%, volume −5% e intonazione naturale approvati. ArtSources/ItalianVoice.profile.json
è il profilo vocale unico; ArtSources/ItalianVoice.lines.json è il copione canonico.
Generare o aggiornare il testo con AgentScripts/PrepareGuideLines.py, poi l'audio
con AgentScripts/PrepareItalianVoice.py --install (il .ps1 richiama questo strumento).
La generazione valida un lotto completo prima dell'installazione e conserva i
percorsi WAV e i GUID Unity esistenti. Internet serve solo nella preparazione
audio; durante il gioco non occorrono servizi di sintesi vocale, voci del sistema
del tablet, microfono o rete. Provini sintetici e file di provenienza restano in
ArtSources/VoiceAuditions e ArtSources/ItalianVoiceNeural. La voce desktop Windows
è stata rifiutata e non è più usata. L'APK originale 0.9.0 mantiene il proprio audio storico.

L'icona d'ingresso segue ora la vera porta dipinta per entrambi i conigli, compreso
l'adattamento delle proporzioni del componente Image, invece di stare a sinistra
dell'edificio. HomeEntranceAnchor la mantiene allineata durante lo scorrimento e
al variare delle proporzioni dello schermo. InstallGameGuide.cs modifica la scena
tramite le API dell'Editor Unity.

Verifiche prima del rilascio: percorso guidato superato su 40 stati renderizzati
a ciascuna risoluzione 1340x800, 1920x1200 e 2048x1536, comprese 219 interazioni
mediante veri eventi del puntatore. Completa pratica scolastica reale e una verifica
di cinque risposte, conquista e appende la medaglia, esegue il ciclo effettivo
delle colture, cucina e mangia, dorme e guarda la TV, viaggia in entrambe le
direzioni, visita e saluta, prova le protezioni sulla disponibilità di mercato e
vestiti. Soltanto gli scenari espliciti di disponibilità di scorte o monete usano
inventari predisposti per le verifiche. Superata anche la regressione completa
esistente per entrambi i personaggi e le calzature, casa e scale, giorno e notte,
amiche e visite, orto, cucina e TV, treno e città, scuola e aiuto, scrittura,
tutte le undici medaglie, colture premio e varianti dei vestiti. La scrittura
mantiene il riferimento sintetico precedente (316/324 corretti, 1 errato, 7 incerti),
non una misura dell'accuratezza sui bambini.

Altri 73 stati di gioco e apprendimento per risoluzione hanno superato controlli
di raycast, testo traboccante e sovrapposizioni dei comandi: 219 acquisizioni dei
percorsi esistenti più 120 del percorso guidato. Ispezionate tutte le 24 tavole.
Problemi trovati e corretti: evidenziazione di interi luoghi invece delle icone
d'azione, sovrapposizione tra guida e menu, tra libro e medaglia, narrazione dello
zaino non corrispondente alle immagini reali di panino e borraccia. Le immagini
modificate dell'introduzione sono state ricontrollate alle tre risoluzioni. Tutte
le 168 impronte, i formati e i segnali WAV superano la validazione (787,392 secondi
totali); i riferimenti audio nella scena sono completi. Evidenze in
Captures/Guide-0.10.0 e Captures/Release-0.10.0. Salvataggi PC originali ripristinati
da una nuova copia in Library/MrRabbitQA. Codice versione Android 17.

Compilazione Android finale riuscita con zero errori. I tre avvisi residui
riguardano Pipeline intenzionalmente disabilitato nel gioco e formati deprecati
delle icone Android. Builds/Android/MrRabbit-0.10.0.apk misura 65.388.613 byte,
SHA256 6C7CDC4796D8417D795E8DD24C25957D544669A664872BA2BE222BB617B34F73.
Installato su Samsung SM-T220 il 19 ottobre 2026 come aggiornamento senza cancellare
i dati di gioco sul tablet. Versione 0.10.0/codice 17 e impronta dell'APK installato
corrispondono alla compilazione finale. Avvio a freddo riuscito; nessuna voce
corrispondente a eccezioni Unity, errori shader o arresti fatali nel registro del
processo. Le prove di tocco native hanno coperto tutte le quattro pagine introduttive
e i comandi per tornare indietro a 1340x800. Acquisizioni in
Captures/Release-0.10.0/Tablet-2026-10-19. Guida lasciata sulla prima pagina, ancora
incompleta, pronta per il bambino. Accuratezza fisica della scrittura infantile
non rimisurata. Una regressione finale mirata al riascolto conferma inoltre che le
spiegazioni contestuali richieste vengono riprodotte una volta senza duplicato automatico.


## 0.10.1 — preparazione Google Play e informativa per i genitori, 24 ottobre 2026

Preparata la distribuzione gratuita, senza pubblicità né acquisti in-app,
con nome pubblico SofiaAntonio Games e assistenza antoniocanza@gmail.com.
Il gioco non è ancora pubblicato: l'account sviluppatore deve essere creato e
l'iter Google di test e revisione non è iniziato.

Aggiunta l'informativa italiana offline alla scelta del personaggio, con quattro
pagine e comandi verificati tramite veri raycast. La consultazione blocca i tocchi
sottostanti e sospende temporaneamente la guida, preservando i progressi. Lo stesso
testo genera la pagina HTML pubblica. Il manifesto Android rimuove i permessi di
rete e disabilita il backup; API di destinazione fissata a 36, codice versione 18.
Il nome aziendale interno di Unity viene conservato per la compatibilità dei
PlayerPrefs su PC. La chiave di caricamento è esterna al repository, con password
protetta per l'account Windows.

Superata la regressione funzionale completa e la logica didattica, inclusi 93
risposte effettive del percorso, tutte le undici medaglie, ricompense, acquisti
con monete del gioco e salvataggi. Il riferimento sintetico della scrittura resta
316/324 corretti, 1 errato e 7 incerti. Prove visive a 1340×800, 1920×1200 e
2048×1536: 219 acquisizioni del gioco, 120 della guida e 12 della privacy, con
21 tavole ispezionate, raycast e controlli del testo superati. La guida della
cucina è verificata anche con una ricetta precedente priva di ingredienti;
le verifiche scolastiche riconoscono il personaggio con i vestiti equipaggiati.
Salvataggi PC originali ripristinati. Nessun tablet collegato: non sono state
eseguite nuove installazioni o prove su un dispositivo fisico.

Materiali italiani dello store, icona 512×512, immagine 1024×500 e dodici schermate
preparati dalle risorse e dalle acquisizioni reali. Dettagli e stato della
compilazione nella [guida di pubblicazione](PUBBLICAZIONE_GOOGLE_PLAY.md).
