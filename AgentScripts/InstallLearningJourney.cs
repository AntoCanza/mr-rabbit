using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class InstallLearningJourney
{
    static TMPro.TMP_FontAsset font;
    static Sprite rounded;
    static Texture2D garden,actions,rewards;
    static Color cream=new Color32(255,248,226,255),ink=new Color32(69,70,61,255),mint=new Color32(195,228,212,255),gold=new Color32(245,205,112,255);
    static T Get<T>(object owner,string name)=>(T)owner.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(owner);
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play before installation");
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        if(a==null)throw new Exception("Missing adventure");
        AssetDatabase.Refresh();
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");rounded=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        garden=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/GardenAtlas.png");actions=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/ActionIcons.png");
        var rewardImporter=(TextureImporter)AssetImporter.GetAtPath("Assets/Art/Garden/SchoolRewardCrops.png");rewardImporter.alphaIsTransparency=true;rewardImporter.mipmapEnabled=false;rewardImporter.maxTextureSize=1536;rewardImporter.textureCompression=TextureImporterCompression.CompressedHQ;rewardImporter.SaveAndReimport();
        rewards=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/SchoolRewardCrops.png");
        if(font==null||garden==null||actions==null||rewards==null)throw new Exception("Missing learning assets");
        if(a.GetComponent<LearningJourney>()!=null)return Finish(a);
        var safe=a.transform;var house=safe.parent.Find("House Viewport/House");var school=a.GetComponent<SchoolController>();
        var w=new LearningJourney.Widgets();
        w.openBook=Button("Learning Notebook",school.UI.view.transform,.78f,.66f,.91f,.86f,mint);Book(w.openBook.transform,.09f,.09f,.91f,.91f);Cue(w.openBook);
        var board=Button("Medals On Wall",house,.33f,.29f,.45f,.41f,new Color32(168,119,77,255));w.openWall=board;w.homeBoard=board.gameObject;
        w.homeMedals=new MedalGraphic[12];w.homeLabels=new TMPro.TMP_Text[12];
        for(int i=0;i<12;i++){float x=.04f+i%4*.24f,y=.07f+i/4*.30f;w.homeMedals[i]=Medal("Hung Medal "+i,board.transform,x,y,x+.21f,y+.28f);w.homeLabels[i]=Text("Medal Mark",w.homeMedals[i].transform,.13f,.34f,.87f,.77f,"",12);}
        var wallHint=Medal("Hang A Medal",board.transform,.31f,.24f,.69f,.78f);Cue(board,wallHint.rectTransform);
        // The small invitation stays behind earned medals and never blocks a touch.
        wallHint.transform.SetAsFirstSibling();
        var decoration=Image("Reading Pennant",house,.55f,.31f,.62f,.42f,new Color32(175,217,201,255));Book(decoration.transform,.13f,.13f,.87f,.87f);w.decoration=decoration.gameObject;
        var root=Image("Italian Learning",safe,0,0,1,1,new Color(1,.98f,.92f,.97f));root.raycastTarget=true;w.root=root.gameObject;
        var panel=Image("Learning Paper",root.transform,.045f,.04f,.955f,.865f,cream);
        w.close=Button("Close Learning",root.transform,.025f,.885f,.10f,.98f,cream);Symbol(w.close.transform,CitySymbol.Kind.Back);
        w.title=Text("Learning Title",root.transform,.15f,.88f,.64f,.98f,"Prima primaria",34);
        w.notebook=Rect("Notebook",panel.transform,0,0,1,1).gameObject;
        w.italian=Button("Italiano",w.notebook.transform,.25f,.86f,.46f,.98f,mint);Text("ABC",w.italian.transform,0,0,1,1,"ABC",38);
        w.maths=Button("Matematica",w.notebook.transform,.54f,.86f,.75f,.98f,Color.white);Text("123",w.maths.transform,0,0,1,1,"123",38);
        w.cards=new UnityEngine.UI.Button[6];w.cardLabels=new TMPro.TMP_Text[6];w.cardSymbols=new TMPro.TMP_Text[6];w.cardCounts=new TMPro.TMP_Text[6];w.cardMarks=new UnityEngine.UI.Image[6];w.cardMedals=new MedalGraphic[6];
        for(int i=0;i<6;i++)
        {
            float x=.045f+i%3*.31f,y=.50f-i/3*.31f;var b=Button("Learning Step "+i,w.notebook.transform,x,y,x+.29f,y+.28f,Color.white);w.cards[i]=b;
            w.cardMarks[i]=Image("Selected",b.transform,.01f,.01f,.99f,.035f,new Color32(70,153,134,255));
            w.cardMedals[i]=Medal("Achievement",b.transform,.04f,.22f,.34f,.91f);w.cardSymbols[i]=Text("Skill Picture",b.transform,.37f,.40f,.95f,.90f,"ABC",42);
            w.cardLabels[i]=Text("Skill Name",b.transform,.04f,.04f,.96f,.26f,"",25);w.cardCounts[i]=Text("Practice Count",b.transform,.62f,.24f,.94f,.43f,"",22);
            var locked=Rect("Locked",b.transform,.72f,.52f,.95f,.88f);Symbol(locked,CitySymbol.Kind.Lock);Cue(b);
        }
        w.practice=Button("Practice Skill",w.notebook.transform,.24f,.025f,.45f,.155f,mint);Art("Practice Pencil",w.practice.transform,actions,15,.08f,.12f,.43f,.88f);Text("Practice",w.practice.transform,.43f,.04f,.97f,.96f,"Prova",27);Cue(w.practice);
        w.test=Button("Verify Skill",w.notebook.transform,.55f,.025f,.78f,.155f,gold);Medal("Test Medal",w.test.transform,.05f,.03f,.39f,.97f);Text("Verify",w.test.transform,.40f,.04f,.97f,.96f,"Verifica",27);Cue(w.test);

        w.exercise=Rect("School Challenge",panel.transform,0,0,1,1).gameObject;
        w.question=Text("Challenge Question",w.exercise.transform,.08f,.72f,.84f,.94f,"",52);w.counter=Text("Challenge Number",w.exercise.transform,.06f,.04f,.22f,.14f,"",24);
        var pictureHolder=Rect("Exercise Drawing",w.exercise.transform,.07f,.25f,.39f,.65f);var picture=Raw("Picture",pictureHolder,0,0,1,1);Fit(picture.gameObject);w.wordPicture=pictureHolder.gameObject.AddComponent<SchoolWordPicture>();w.wordPicture.Configure(picture,school.UI.wordPicture.Entries.ToArray());
        w.pads=new HandwritingPad[2];w.padSlots=new GameObject[2];w.readback=new TMPro.TMP_Text[2];w.guides=new TMPro.TMP_Text[2];
        for(int i=0;i<2;i++)
        {
            var slot=Rect("Learning Ink Slot "+i,w.exercise.transform,.48f,.25f,.77f,.64f);w.padSlots[i]=slot.gameObject;
            var square=Image("Paper Square",slot,0,0,1,1,new Color32(255,254,246,255));Fit(square.gameObject);
            var line=square.gameObject.AddComponent<UnityEngine.UI.Outline>();line.effectColor=new Color32(185,180,157,255);line.effectDistance=new Vector2(2,2);
            Image("Baseline",square.transform,.07f,.14f,.93f,.148f,new Color32(198,216,218,255));w.guides[i]=Text("Free Model",square.transform,.08f,.08f,.92f,.92f,"",140);w.guides[i].color=new Color(.36f,.56f,.60f,.22f);
            w.pads[i]=Rect("Learning Finger Ink",square.transform,.04f,.04f,.96f,.96f).gameObject.AddComponent<HandwritingPad>();w.pads[i].color=new Color32(45,67,102,255);w.pads[i].raycastTarget=true;
            w.readback[i]=Text("Read Ink",slot,.28f,-.23f,.72f,-.025f,"",29);
        }
        w.choices=new UnityEngine.UI.Button[3];w.choiceLabels=new TMPro.TMP_Text[3];w.choicePictures=new UnityEngine.UI.RawImage[3];w.choiceMarks=new UnityEngine.UI.Image[3];
        for(int i=0;i<3;i++)
        {
            float x=.08f+i*.25f;var b=Button("Answer Choice "+i,w.exercise.transform,x,.16f,x+.22f,.43f,Color.white);w.choices[i]=b;
            w.choiceLabels[i]=Text("Answer",b.transform,.03f,.12f,.97f,.88f,"",42);w.choicePictures[i]=Raw("Answer Drawing",b.transform,.08f,.08f,.92f,.92f);Fit(w.choicePictures[i].gameObject);
            w.choiceMarks[i]=Image("Correct Choice",b.transform,.02f,.02f,.98f,.06f,new Color32(78,161,126,255));
        }
        w.counts=new UnityEngine.UI.RawImage[20];for(int i=0;i<20;i++)w.counts[i]=Art("Counted Carrot "+i,w.exercise.transform,garden,4,0,0,.06f,.08f);
        w.pips=new UnityEngine.UI.Image[5];for(int i=0;i<5;i++){float x=.34f+i*.065f;w.pips[i]=Image("Test Step "+i,w.exercise.transform,x,.045f,x+.045f,.085f,gold);}
        w.submit=Button("Submit Learning Ink",w.exercise.transform,.86f,.27f,.97f,.42f,mint);Symbol(w.submit.transform,CitySymbol.Kind.Check);Cue(w.submit);
        w.erase=Button("Erase Learning Ink",w.exercise.transform,.86f,.46f,.97f,.61f,Color.white);Text("Eraser",w.erase.transform,0,0,1,1,"⌫",42);
        w.help=Button("Learning Help",w.exercise.transform,.86f,.65f,.97f,.80f,Color.white);Book(w.help.transform,.08f,.08f,.92f,.92f);
        w.next=Button("Next Learning Question",w.exercise.transform,.86f,.06f,.97f,.21f,mint);Symbol(w.next.transform,CitySymbol.Kind.Arrow);Cue(w.next);
        w.feedbackSymbol=Symbol(Rect("Learning Feedback",w.exercise.transform,.65f,.04f,.75f,.16f),CitySymbol.Kind.Check);
        w.result=Rect("Assessment Result",panel.transform,0,0,1,1).gameObject;
        w.resultMedal=Medal("Earned Medal",w.result.transform,.35f,.36f,.65f,.85f);w.resultSymbol=Symbol(Rect("Try Again",w.result.transform,.39f,.43f,.61f,.77f),CitySymbol.Kind.Back);
        w.resultScore=Text("Result Score",w.result.transform,.35f,.25f,.65f,.38f,"",42);w.rewardName=Text("Result Name",w.result.transform,.15f,.16f,.85f,.26f,"",30);
        w.rewardSymbol=Symbol(Rect("New Reward",w.result.transform,.68f,.46f,.83f,.70f),CitySymbol.Kind.Garden);
        w.resultNext=Button("Return To Notebook",w.result.transform,.43f,.025f,.57f,.145f,mint);Book(w.resultNext.transform,.08f,.08f,.92f,.92f);Cue(w.resultNext);
        w.wall=Rect("Medal Collection",panel.transform,0,0,1,1).gameObject;
        var wallWood=Image("Medal Board",w.wall.transform,.05f,.34f,.95f,.96f,new Color32(202,161,109,255));
        w.wallSlots=new UnityEngine.UI.Button[12];w.wallMedals=new MedalGraphic[12];w.wallLabels=new TMPro.TMP_Text[12];w.wallMarks=new UnityEngine.UI.Image[12];
        for(int i=0;i<12;i++)
        {
            float x=.035f+i%4*.245f,y=.67f-i/4*.325f;var b=Button("Wall Frame "+i,wallWood.transform,x,y,x+.22f,y+.29f,cream);w.wallSlots[i]=b;
            w.wallMedals[i]=Medal("Displayed Medal",b.transform,.18f,.03f,.82f,.97f);w.wallLabels[i]=Text("Skill",b.transform,.27f,.39f,.73f,.70f,"",21);
            w.wallMarks[i]=Image("Hang Here",b.transform,.02f,.02f,.98f,.06f,new Color32(66,153,130,255));Cue(b);
        }
        w.medalChoices=new UnityEngine.UI.Button[11];w.trayMedals=new MedalGraphic[11];w.trayLabels=new TMPro.TMP_Text[11];
        for(int i=0;i<11;i++)
        {
            float x=.065f+i%6*.15f,y=.18f-i/6*.145f;var b=Button("Choose Medal "+i,w.wall.transform,x,y,x+.12f,y+.135f,Color.white);w.medalChoices[i]=b;
            w.trayMedals[i]=Medal("Medal",b.transform,.13f,0,.87f,1);w.trayLabels[i]=Text("Medal Skill",b.transform,.22f,.38f,.78f,.71f,LearningProgress.Marks[i],21);Cue(b);
        }
        foreach(var go in new[]{w.root,w.decoration})go.SetActive(false);
        var learning=a.gameObject.AddComponent<LearningJourney>();learning.Configure(w);
        return Finish(a);
    }
    static string Finish(OutdoorAdventure a)
    {
        InstallRewards(a);InstallVoice(a);PlayerSettings.bundleVersion="0.9.0";PlayerSettings.Android.bundleVersionCode=16;
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(a.gameObject.scene);EditorSceneManager.SaveScene(a.gameObject.scene);AssetDatabase.SaveAssets();
        return "Installed 11 Italian learning milestones, practice/exams, persistent medal wall, 2 new crops, 2 outfit variants, 6 recipes and offline Italian voice. 0.9.0 (16), PC only.";
    }
    static void InstallRewards(OutdoorAdventure a)
    {
        var rewardArt=a.GetComponent<RewardArt>();if(rewardArt==null)rewardArt=a.gameObject.AddComponent<RewardArt>();rewardArt.atlas=rewards;rewardArt.garden=garden;
        var g=a.GetComponent<GardenController>();var gu=Get<GardenController.Widgets>(g,"ui");
        Array.Resize(ref gu.seedChoices,6);var seedParent=gu.seedChoices[0].transform.parent;
        for(int i=0;i<6;i++)
        {
            if(i>=4&&gu.seedChoices[i]==null){gu.seedChoices[i]=UnityEngine.Object.Instantiate(gu.seedChoices[0],seedParent);gu.seedChoices[i].name="Reward Seed "+i;var raw=gu.seedChoices[i].GetComponentInChildren<UnityEngine.UI.RawImage>(true);raw.texture=rewards;raw.uvRect=new Rect((i-4)*.5f,.5f,.5f,.5f);Fit(raw.gameObject);var l=Rect("School Lock",gu.seedChoices[i].transform,.61f,.61f,.96f,.97f);Symbol(l,CitySymbol.Kind.Lock);}
            float x=.05f+i%3*.31f,y=.50f-i/3*.39f;Place((RectTransform)gu.seedChoices[i].transform,x,y,x+.28f,y+.33f);
        }
        var c=a.GetComponent<CityController>();var cu=c.UI;Array.Resize(ref cu.clothes,4);Array.Resize(ref cu.clothesPictures,4);Array.Resize(ref cu.prices,4);
        for(int i=0;i<4;i++)
        {
            if(i>=2)
            {
                if(cu.clothes[i]==null)cu.clothes[i]=UnityEngine.Object.Instantiate(cu.clothes[1],cu.clothes[1].transform.parent);cu.clothes[i].name="School Outfit "+i;
                cu.clothesPictures[i]=cu.clothes[i].transform.Find("Outfit Preview").GetComponent<UnityEngine.UI.Image>();
                cu.prices[i]=cu.clothes[i].GetComponentsInChildren<TMPro.TMP_Text>(true).First();if(cu.clothes[i].transform.Find("School Lock")==null){var l=Rect("School Lock",cu.clothes[i].transform,.60f,.73f,.95f,.95f);Symbol(l,CitySymbol.Kind.Lock);}
            }
            float x=.045f+i*.24f;Place((RectTransform)cu.clothes[i].transform,x,.20f,x+.21f,.83f);
        }
        cu.goldOutfit=Material("Golden School Clothes",new Color(1,.72f,.18f));cu.emeraldOutfit=Material("Emerald School Clothes",new Color(.16f,.76f,.48f));
        var home=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var hu=home.UI;Array.Resize(ref hu.recipes,6);Array.Resize(ref hu.recipeLabels,6);
        var visual=home.GetComponent<KitchenVisuals>();var ingredients=Get<TMPro.TMP_Text[]>(visual,"ingredients");var meals=Get<TMPro.TMP_Text[]>(visual,"meals");var marks=Get<UnityEngine.UI.Image[]>(visual,"selected");var dish=Get<UnityEngine.UI.Image>(visual,"dish");
        Array.Resize(ref ingredients,6);Array.Resize(ref meals,6);Array.Resize(ref marks,6);
        for(int i=0;i<6;i++)
        {
            if(i>=4&&hu.recipes[i]==null)
            {
                hu.recipes[i]=UnityEngine.Object.Instantiate(hu.recipes[0],hu.recipes[0].transform.parent);hu.recipes[i].name="Reward Recipe "+i;
                hu.recipeLabels[i]=hu.recipes[i].GetComponentsInChildren<TMPro.TMP_Text>(true).First(x=>x.name==hu.recipeLabels[0].name);
                ingredients[i]=hu.recipes[i].transform.Find("Ingredient Count").GetComponent<TMPro.TMP_Text>();meals[i]=hu.recipes[i].transform.Find("Meal Count").GetComponent<TMPro.TMP_Text>();marks[i]=hu.recipes[i].transform.Find("Selected Recipe").GetComponent<UnityEngine.UI.Image>();
                var raw=hu.recipes[i].transform.Find("Ingredient Picture").GetComponentInChildren<UnityEngine.UI.RawImage>(true);raw.texture=rewards;raw.uvRect=new Rect((i-4)*.5f,.5f,.5f,.5f);Fit(raw.gameObject);
                hu.recipes[i].GetComponentInChildren<HearthGraphic>(true).SetDish(i);
            }
            float x=.05f+i%3*.30f,y=.53f-i/3*.29f;Place((RectTransform)hu.recipes[i].transform,x,y,x+.27f,y+.26f);
        }
        visual.Configure(home,g,ingredients,meals,marks,dish);
        Place((RectTransform)hu.cook.transform,.26f,.015f,.45f,.20f);Place((RectTransform)hu.eat.transform,.55f,.015f,.74f,.20f);
    }
    static Material Material(string name,Color colour)
    {string path="Assets/Art/City/"+name.Replace(" ","")+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("MrRabbit/RewardClothes"));m.name=name;AssetDatabase.CreateAsset(m,path);}m.SetColor("_Tint",colour);return m;}
    static void InstallVoice(OutdoorAdventure a)
    {
        var voice=a.GetComponent<ItalianVoice>();if(voice!=null)return;voice=a.gameObject.AddComponent<ItalianVoice>();voice.clips=AssetDatabase.FindAssets("t:AudioClip",new[]{"Assets/Audio/Italian"}).Select(AssetDatabase.GUIDToAssetPath).Select(path=>
        {
            var importer=(AudioImporter)AssetImporter.GetAtPath(path);var sample=importer.defaultSampleSettings;sample.compressionFormat=AudioCompressionFormat.Vorbis;sample.quality=.55f;sample.loadType=AudioClipLoadType.CompressedInMemory;importer.defaultSampleSettings=sample;importer.forceToMono=true;importer.SaveAndReimport();
            return new ItalianVoice.Entry{key=System.IO.Path.GetFileNameWithoutExtension(path),clip=AssetDatabase.LoadAssetAtPath<AudioClip>(path)};
        }).ToArray();
        voice.repeat=Button("Repeat Italian Voice",a.transform,.66f,.89f,.735f,.975f,cream);Symbol(voice.repeat.transform,CitySymbol.Kind.Sound);
        voice.mute=Button("Mute Italian Voice",a.transform,.75f,.89f,.825f,.975f,cream);voice.muteSymbol=Symbol(voice.mute.transform,CitySymbol.Kind.Sound);
    }
    static RectTransform Rect(string n,Transform p,float x,float y,float X,float Y){var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);Place(r,x,y,X,Y);return r;}
    static void Place(RectTransform r,float x,float y,float X,float Y){r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(X,Y);r.offsetMin=r.offsetMax=Vector2.zero;}
    static UnityEngine.UI.Image Image(string n,Transform p,float x,float y,float X,float Y,Color c){var im=Rect(n,p,x,y,X,Y).gameObject.AddComponent<UnityEngine.UI.Image>();im.color=c;im.sprite=rounded;im.type=UnityEngine.UI.Image.Type.Sliced;im.raycastTarget=false;return im;}
    static UnityEngine.UI.Button Button(string n,Transform p,float x,float y,float X,float Y,Color c){var im=Image(n,p,x,y,X,Y,c);im.raycastTarget=true;var b=im.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=im;b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;}
    static TMPro.TMP_Text Text(string n,Transform p,float x,float y,float X,float Y,string t,int size){var v=Rect(n,p,x,y,X,Y).gameObject.AddComponent<TMPro.TextMeshProUGUI>();v.font=font;v.text=t;v.fontSize=size;v.enableAutoSizing=true;v.fontSizeMin=14;v.fontSizeMax=size;v.alignment=TMPro.TextAlignmentOptions.Center;v.color=ink;v.raycastTarget=false;return v;}
    static UnityEngine.UI.RawImage Raw(string n,Transform p,float x,float y,float X,float Y){var im=Rect(n,p,x,y,X,Y).gameObject.AddComponent<UnityEngine.UI.RawImage>();im.raycastTarget=false;return im;}
    static UnityEngine.UI.RawImage Art(string n,Transform p,Texture2D atlas,int index,float x,float y,float X,float Y){var holder=Rect(n+" Holder",p,x,y,X,Y);var raw=Raw(n,holder,0,0,1,1);raw.texture=atlas;Fit(raw.gameObject);GardenArt.Apply(raw,index);return raw;}
    static CitySymbol Symbol(Transform p,CitySymbol.Kind kind){var g=Rect("Symbol",p,.10f,.10f,.90f,.90f).gameObject.AddComponent<CitySymbol>();g.symbol=kind;g.raycastTarget=false;return g;}
    static MedalGraphic Medal(string n,Transform p,float x,float y,float X,float Y){var holder=Rect(n+" Holder",p,x,y,X,Y);var m=Rect(n,holder,0,0,1,1).gameObject.AddComponent<MedalGraphic>();m.raycastTarget=false;Fit(m.gameObject);return m;}
    static MedalGraphic Book(Transform p,float x,float y,float X,float Y){var m=Medal("Book",p,x,y,X,Y);m.book=true;return m;}
    static void Fit(GameObject go){var fit=go.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit==null)fit=go.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;}
    static void Cue(UnityEngine.UI.Button button,RectTransform picture=null){var glow=Image("Learning Glow",button.transform,0,0,1,1,new Color(1,.78f,.24f,.35f));glow.transform.SetAsFirstSibling();glow.enabled=false;var c=button.gameObject.AddComponent<ActionCue>();if(picture==null){var icon=button.GetComponentInChildren<MedalGraphic>();picture=icon!=null?icon.rectTransform:button.GetComponentInChildren<CitySymbol>()?.rectTransform;}c.Configure(picture??(RectTransform)button.transform,glow,true);}
}
