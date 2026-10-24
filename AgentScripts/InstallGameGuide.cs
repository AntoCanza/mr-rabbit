using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class InstallGameGuide
{
    static TMPro.TMP_FontAsset font;
    static Sprite rounded;
    static Color cream=new Color32(255,248,226,255),mint=new Color32(194,229,216,255),ink=new Color32(72,66,69,255),gold=new Color32(248,210,129,255);
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first");
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        if(a==null)throw new Exception("Missing adventure");
        var c=a.GetComponent<CityController>();var voice=a.GetComponent<ItalianVoice>();
        var sign=a.transform.parent.GetComponentsInChildren<UnityEngine.UI.Button>(true).Single(b=>b.name=="Home Sign");
        var anchor=c.UI.homeExterior.GetComponent<HomeEntranceAnchor>();if(anchor==null)anchor=c.UI.homeExterior.gameObject.AddComponent<HomeEntranceAnchor>();anchor.house=c.UI.homeExterior;anchor.entrance=(RectTransform)sign.transform;anchor.Apply();EditorUtility.SetDirty(anchor);
        var clips=AssetDatabase.FindAssets("t:AudioClip",new[]{"Assets/Audio/Italian"}).Select(AssetDatabase.GUIDToAssetPath).OrderBy(p=>p).ToArray();
        voice.clips=clips.Select(p=>new ItalianVoice.Entry{key=System.IO.Path.GetFileNameWithoutExtension(p),clip=AssetDatabase.LoadAssetAtPath<AudioClip>(p)}).ToArray();EditorUtility.SetDirty(voice);
        if(a.GetComponent<GameGuide>()!=null){IntroCue(a.GetComponent<GameGuide>().UI);PolishPictures(a.GetComponent<GameGuide>().UI);return Save(a,"Refreshed guide audio and doorway anchor");}
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");rounded=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        var w=new GameGuide.Widgets();w.root=Rect("Elsa Guide",a.transform,0,0,1,1);var canvas=w.root.gameObject.AddComponent<Canvas>();canvas.overrideSorting=true;canvas.sortingOrder=30;w.root.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        var intro=Image("First Steps With Elsa",w.root,0,0,1,1,new Color(.96f,.94f,.86f,.98f));intro.raycastTarget=true;w.intro=intro.gameObject;
        var paper=Image("Welcome Paper",intro.transform,.07f,.13f,.93f,.86f,cream);
        w.introTitle=Text("Welcome Title",paper.transform,.08f,.82f,.92f,.97f,"Benvenuto!",42);
        w.introPictures=new GameObject[4];for(int i=0;i<4;i++)w.introPictures[i]=Rect("Welcome Chapter "+i,paper.transform,.07f,.24f,.93f,.80f).gameObject;
        var view=w.introPictures[0].transform;
        Picture("Our Home",view,c.UI.homeExterior.sprite,.26f,.02f,.74f,.97f);
        Picture("Mr Rabbit",view,c.UI.male,.03f,0,.26f,.78f);Picture("Miss Rabbit",view,c.UI.female,.74f,0,.97f,.78f);
        view=w.introPictures[1].transform;
        Book(view,.08f,.08f,.48f,.92f);var medal=Rect("School Medal",view,.55f,.13f,.86f,.88f).gameObject.AddComponent<MedalGraphic>();medal.earned=true;medal.raycastTarget=false;Fit(medal.gameObject);
        view=w.introPictures[2].transform;
        Symbol(Rect("Grow",view,.04f,.24f,.32f,.94f),CitySymbol.Kind.Garden);
        Art(view,"Water",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/GardenAtlas.png"),1,.37f,.25f,.62f,.9f);
        Symbol(Rect("Cooking",view,.68f,.2f,.96f,.92f),CitySymbol.Kind.Pot);
        view=w.introPictures[3].transform;
        Symbol(Rect("Train",view,.01f,.2f,.33f,.9f),CitySymbol.Kind.Train);Symbol(Rect("Market",view,.36f,.2f,.65f,.9f),CitySymbol.Kind.Market);Symbol(Rect("Clothes",view,.69f,.2f,.99f,.9f),CitySymbol.Kind.Shirt);
        w.pips=new UnityEngine.UI.Image[4];for(int i=0;i<4;i++)w.pips[i]=Image("Welcome Step "+i,paper.transform,.40f+i*.055f,.16f,.43f+i*.055f,.185f,gold);
        w.back=Button("Welcome Previous",paper.transform,.06f,.035f,.19f,.19f,Color.white);Symbol(w.back.transform,CitySymbol.Kind.Back);
        w.next=Button("Welcome Next",paper.transform,.81f,.035f,.94f,.19f,mint);w.nextSymbol=Symbol(w.next.transform,CitySymbol.Kind.Arrow);
        w.skip=Button("Skip Welcome",intro.transform,.025f,.89f,.20f,.975f,cream);Text("Skip",w.skip.transform,0,0,1,1,"Salta",26);
        w.introRepeat=Button("Welcome Replay Elsa",intro.transform,.66f,.89f,.735f,.975f,cream);Symbol(w.introRepeat.transform,CitySymbol.Kind.Sound);
        w.introMute=Button("Welcome Mute Elsa",intro.transform,.75f,.89f,.825f,.975f,cream);w.introMuteSymbol=Symbol(w.introMute.transform,CitySymbol.Kind.Sound);
        w.help=Button("Elsa Guide Help",w.root,.012f,.015f,.08f,.115f,mint);Book(w.help.transform,.08f,.08f,.92f,.92f);Text("Question",w.help.transform,.28f,.24f,.72f,.86f,"?",38);
        var menu=Image("Elsa Help Menu",w.root,0,0,1,1,new Color(.96f,.94f,.86f,.92f));menu.raycastTarget=true;w.menu=menu.gameObject;
        var menuPaper=Image("Help Paper",menu.transform,.23f,.20f,.77f,.80f,cream);Text("Help Title",menuPaper.transform,.13f,.82f,.87f,.99f,"Elsa",36);
        w.restart=MenuButton("Discover The Game",menuPaper.transform,.06f,.43f,.47f,.81f,"Guida",CitySymbol.Kind.Home);
        w.repeat=MenuButton("Explain Here",menuPaper.transform,.53f,.43f,.94f,.81f,"Ascolta",CitySymbol.Kind.Sound);
        w.pause=MenuButton("Pause Guidance",menuPaper.transform,.06f,.03f,.47f,.41f,"Pausa",CitySymbol.Kind.Muted);w.pauseLabel=w.pause.GetComponentInChildren<TMPro.TMP_Text>();
        w.skipStep=MenuButton("Skip This Step",menuPaper.transform,.53f,.03f,.94f,.41f,"Salta",CitySymbol.Kind.Arrow);
        w.menuClose=Button("Close Elsa Menu",menu.transform,.025f,.885f,.10f,.98f,cream);Symbol(w.menuClose.transform,CitySymbol.Kind.Back);
        w.focus=new GuideFocusGraphic[4];for(int i=0;i<4;i++){w.focus[i]=Rect("Elsa Highlight "+i,w.root,0,0,.1f,.1f).gameObject.AddComponent<GuideFocusGraphic>();w.focus[i].raycastTarget=false;w.focus[i].gameObject.SetActive(false);}
        w.hand=Rect("Elsa Moving Finger",w.root,0,0,.1f,.1f);var hand=w.hand.gameObject.AddComponent<GuideHandGraphic>();hand.raycastTarget=false;w.hand.gameObject.SetActive(false);
        w.intro.SetActive(false);w.menu.SetActive(false);
        var guide=a.gameObject.AddComponent<GameGuide>();guide.UI=w;EditorUtility.SetDirty(guide);
        IntroCue(w);
        PolishPictures(w);
        return Save(a,"Installed introductory wizard, contextual action guide, animated nonblocking finger/highlights, replay/pause/skip, separate saved tutorial progress and doorway anchor");
    }
    static string Save(OutdoorAdventure a,string message){Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(a.gameObject.scene);EditorSceneManager.SaveScene(a.gameObject.scene);AssetDatabase.SaveAssets();return message;}
    static void IntroCue(GameGuide.Widgets w){if(w.next.GetComponent<ActionCue>()!=null)return;var halo=w.next.gameObject.AddComponent<UnityEngine.UI.Outline>();halo.effectColor=new Color(1,.72f,.20f,.65f);halo.effectDistance=new Vector2(3,3);w.next.gameObject.AddComponent<ActionCue>().Configure(w.nextSymbol.rectTransform,null,false);}
    static void PolishPictures(GameGuide.Widgets w)
    {
        var parent=w.introPictures[1].transform;
        foreach(var drawing in parent.GetComponentsInChildren<MedalGraphic>(true))
        {
            if(drawing.transform.parent!=parent)continue;
            var holder=drawing.book?Rect("School Book Slot",parent,.08f,.08f,.48f,.92f):Rect("School Medal Slot",parent,.55f,.13f,.86f,.88f);
            drawing.transform.SetParent(holder,false);var r=drawing.rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
        }
    }
    static RectTransform Rect(string n,Transform p,float x,float y,float X,float Y){var r=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(p,false);r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(X,Y);r.offsetMin=r.offsetMax=Vector2.zero;return r;}
    static UnityEngine.UI.Image Image(string n,Transform p,float x,float y,float X,float Y,Color c){var i=Rect(n,p,x,y,X,Y).gameObject.AddComponent<UnityEngine.UI.Image>();i.sprite=rounded;i.type=UnityEngine.UI.Image.Type.Sliced;i.color=c;i.raycastTarget=false;return i;}
    static UnityEngine.UI.Button Button(string n,Transform p,float x,float y,float X,float Y,Color c){var i=Image(n,p,x,y,X,Y,c);i.raycastTarget=true;var b=i.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=i;b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;}
    static UnityEngine.UI.Button MenuButton(string n,Transform p,float x,float y,float X,float Y,string label,CitySymbol.Kind kind){var b=Button(n,p,x,y,X,Y,mint);Symbol(Rect("Menu Icon",b.transform,.28f,.33f,.72f,.97f),kind);Text("Menu Label",b.transform,.04f,.02f,.96f,.34f,label,27);return b;}
    static TMPro.TMP_Text Text(string n,Transform p,float x,float y,float X,float Y,string text,int size){var t=Rect(n,p,x,y,X,Y).gameObject.AddComponent<TMPro.TextMeshProUGUI>();t.font=font;t.text=text;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=18;t.fontSizeMax=size;t.color=ink;t.alignment=TMPro.TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
    static CitySymbol Symbol(Transform p,CitySymbol.Kind kind){var s=Rect("Illustration",p,.08f,.08f,.92f,.92f).gameObject.AddComponent<CitySymbol>();s.symbol=kind;s.raycastTarget=false;return s;}
    static void Book(Transform p,float x,float y,float X,float Y){var m=Rect("Guide Book",p,x,y,X,Y).gameObject.AddComponent<MedalGraphic>();m.book=true;m.raycastTarget=false;Fit(m.gameObject);}
    static void Picture(string n,Transform p,Sprite sprite,float x,float y,float X,float Y){var i=Rect(n,p,x,y,X,Y).gameObject.AddComponent<UnityEngine.UI.Image>();i.sprite=sprite;i.preserveAspect=true;i.raycastTarget=false;}
    static void Fit(GameObject go){var f=go.AddComponent<UnityEngine.UI.AspectRatioFitter>();f.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;f.aspectRatio=1;}
    static void Art(Transform p,string n,Texture2D atlas,int cell,float x,float y,float X,float Y){var raw=Rect(n,p,x,y,X,Y).gameObject.AddComponent<UnityEngine.UI.RawImage>();raw.texture=atlas;raw.raycastTarget=false;GardenArt.Apply(raw,cell);Fit(raw.gameObject);}
}
