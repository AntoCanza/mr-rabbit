# Risorse grafiche e audio

## Disegni e illustrazioni

`ArtSources` conserva disegni di riferimento originali, prompt di generazione delle immagini, varianti visive precedenti e provenienza della generazione vocale. I contenuti usati nel gioco sono in `Assets/Art`, suddivisi in `Characters`, `Home`, `Outdoors`, `Garden`, `School` e `City`.

Le illustrazioni del gioco derivano da disegni originali, sviluppati con strumenti di generazione delle immagini. I file dei prompt documentano l'aspetto desiderato e il processo di origine; non costituiscono concessioni generali di licenze di terzi. Conservare riferimenti originali e note esistenti quando si modificano le risorse. Non spostare o sostituire i file Unity `.meta` indipendentemente dalle rispettive risorse. I prompt originali sono materiali sorgente: se redatti in un'altra lingua, ne viene conservato il testo per documentare esattamente la generazione effettuata.

La galleria del README comprende tre schermate storiche selezionate della versione 0.10.0, copiate dalle acquisizioni locali delle verifiche: casa, scrittura guidata delle vocali e viaggio in treno. L'archivio completo delle acquisizioni e i registri dei dispositivi restano esclusi da Git.

## Narrazione italiana offline

| Sorgente | Contenuto |
| --- | --- |
| `ArtSources/ItalianVoice.profile.json` | Voce approvata `it-IT-ElsaNeural`, lingua italiana, velocità −5%, volume −5%, intonazione naturale e uscita a 24 kHz |
| `ArtSources/ItalianVoice.lines.json` | Copione canonico delle frasi pronunciate |
| `ArtSources/VoiceAuditions/Guida-naturale-elsa-prova.mp3` | Campione di confronto approvato |
| `ArtSources/ItalianVoiceNeural/manifest.json` | Provenienza dell'audio sintetico, versione della generazione e impronte delle registrazioni |
| `Assets/Audio/Italian/` | Registrazioni WAV mono PCM a 16 bit incluse e usate dal gioco |
| `AgentScripts/PrepareItalianVoice.py` | Strumento facoltativo di preparazione, validazione e installazione |

Il gioco riproduce offline i file WAV inclusi. Non chiama servizi di sintesi vocale durante il gioco e non richiede un microfono. Conservare la voce neurale Elsa approvata per guida, lezioni e notifiche; non sostituirla con la voce desktop rifiutata o con un'altra voce.

### Rigenerazione audio facoltativa

La preparazione audio richiede Python e le dipendenze di sviluppo `edge-tts` e `imageio-ffmpeg`. Lo script di preparazione ne documenta l'utilizzo. Usare un ambiente virtuale separato o la posizione degli strumenti di sviluppo del progetto esclusa da Git.

```sh
python -m venv .venv
# Attivare l'ambiente con il comando appropriato per la propria shell.
python -m pip install edge-tts imageio-ffmpeg
python AgentScripts/PrepareItalianVoice.py
```

L'ultimo comando prepara e valida il lotto provvisorio in `ArtSources/ItalianVoiceNeural`. Esaminare il risultato prima di installarlo:

```sh
python AgentScripts/PrepareItalianVoice.py --install
```

L'installazione valida l'intero lotto e conserva percorsi e GUID dei WAV Unity. La generazione usa un servizio vocale Microsoft online tramite `edge-tts`; la riproduzione nel gioco rimane offline. Prima di usare commercialmente l'audio rigenerato, esaminare le condizioni del servizio applicabili a generazione e distribuzione. `PrepareGuideLines.py` aggiorna il testo della guida e va eseguito soltanto quando si vuole modificarne il copione. Lo strumento vocale PowerShell richiama quello Python.

## Contenuti di terzi e licenze

Le dipendenze Unity conservano le licenze e le note dei rispettivi pacchetti. Le risorse TextMesh Pro e i caratteri importati conservano le note incluse in `Assets/TextMesh Pro`. Controllare questi file e i contenuti dei pacchetti ripristinati prima della ridistribuzione. Il testo originale delle licenze e delle attribuzioni di terzi viene preservato.

Il repository non introduce una licenza generale di riutilizzo del progetto. I contenuti originali restano soggetti ai diritti dei rispettivi titolari; la pubblicazione non seleziona automaticamente MIT, Apache, Creative Commons o altre licenze. Non rimuovere note di terzi, non attribuirsi la proprietà dei loro materiali e non dedurre permessi di ridistribuzione dalla sola presenza di un file.
