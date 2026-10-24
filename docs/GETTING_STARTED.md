# Configurazione e compilazione

## Prima importazione

Clonare `https://github.com/AntoCanza/mr-rabbit.git`, aggiungere la cartella radice a Unity Hub e usare **6000.3.25f1**, indicata in `ProjectSettings/ProjectVersion.txt`. Installare Android Build Support, SDK/NDK e OpenJDK solo se si vuole compilare per Android.

Lasciare che Package Manager ripristini `Packages/manifest.json` e `Packages/packages-lock.json`. La prima importazione ricostruisce la cache `Library`, esclusa da Git. Le risorse binarie attuali sono normali file Git: una clonazione ordinaria le comprende senza ulteriori passaggi con Git LFS.

Aprire `Assets/Scenes/Home.unity` e avviare la modalità Play. `SampleScene.unity` e i modelli di scena URP non sono il punto di ingresso del gioco. I file WAV della narrazione sono già inclusi in `Assets/Audio/Italian`; la generazione della voce è facoltativa.

## Compilazione Android

Usare **File → Build Profiles**, scegliere Android e abilitare `Assets/Scenes/Home.unity`. Prima di compilare, controllare queste impostazioni presenti nel repository:

| Impostazione | Valore attuale |
| --- | --- |
| Prodotto | Mr Rabbit |
| Versione dei sorgenti | 0.10.0 |
| Codice versione Android | 17 |
| Identificatore del pacchetto | `com.mrrabbit.home` |
| Backend di compilazione / architettura | IL2CPP / ARM64 |
| Orientamento | Orizzontale |
| SDK minimo | API 25 |
| SDK di destinazione | Automatico in Unity |

Usare SDK, NDK e JDK installati per questa versione dell'Editor. Salvare gli APK di sviluppo in `Builds/Android/`, esclusa da Git. Chiavi di firma per il rilascio, password, credenziali degli store e percorsi specifici della macchina restano fuori da Git. I requisiti degli store vanno verificati separatamente al momento della pubblicazione.

Anche l'identificatore iOS è `com.mrrabbit.home`, ma una vera compilazione iOS richiede macOS, Xcode e firma Apple. La cronologia del repository non documenta un rilascio iOS.

## Automazione facoltativa con Unity CLI

Il progetto fissa la versione di `com.unity.pipeline` per l'automazione dell'Editor durante lo sviluppo. Il normale utilizzo di Unity Editor non richiede l'installazione della CLI. Se la si usa, installare Unity CLI ufficiale e indicare esplicitamente questa copia del progetto quando sono aperti più Editor.

`AgentScripts/*.cs` contiene sorgenti da eseguire mediante uno strumento di scripting compatibile con l'Editor. Si trovano fuori da `Assets`: le loro classi non compaiono automaticamente nelle compilazioni del gioco o nei menu di Unity. Leggere ogni strumento prima di eseguirlo: diversi script `Add*`, `Install*`, `Polish*` e `Prepare*` sono migrazioni storiche che cambiano scene, immagini o numeri di versione. **Una nuova clonazione contiene già la scena completa: non rieseguire tutti gli strumenti.** In particolare, il vecchio `PrepareTabletRelease.cs` imposta una versione precedente.

Per le prove delle funzionalità e la tutela dei salvataggi, seguire [Verifiche](VERIFICATION.md). Per la preparazione vocale facoltativa durante lo sviluppo, seguire [Risorse grafiche e audio](ASSETS_AND_AUDIO.md).

## Problemi comuni di configurazione

| Sintomo | Controllo |
| --- | --- |
| Scena di esempio vuota | Aprire `Assets/Scenes/Home.unity`. |
| Errori dei pacchetti o di compilazione al primo avvio | Verificare la versione esatta dell'Editor, l'accesso alla rete per il primo ripristino e la diagnostica di Package Manager. Conservare entrambi i file dei pacchetti durante la risoluzione del problema. |
| Script, materiali o riferimenti mancanti | Verificare che siano stati clonati sia la risorsa sia il suo file `.meta` originale. Non rigenerare i GUID come primo tentativo di correzione. |
| Vista Game apparentemente tagliata | Scegliere un rapporto d'aspetto orizzontale e ridurre il cursore Scale della vista Game per adattarla al pannello. Parte del ritaglio del paesaggio è intenzionale. |
| Narrazione silenziosa | Controllare la preferenza audio salvata, il volume del gioco e del dispositivo e i file inclusi in `Assets/Audio/Italian`. |
| Piattaforma Android non disponibile | Installare Android Build Support e i relativi SDK, NDK e OpenJDK per la versione fissata dell'Editor in Unity Hub. |
| Progressi precedenti visibili in modalità Play | I progressi usano PlayerPrefs locali. Farne una copia prima delle verifiche; non cancellare salvataggi reali solo per simulare una nuova installazione. |
