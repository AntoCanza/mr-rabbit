using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class AddSchool
{
    static TMPro.TMP_FontAsset font;
    static Texture2D icons;
    static Color cream=new Color32(255,247,223,255),ink=new Color32(66,64,46,255);
    // Measured individual drawings; the painted atlas does not use tight cell boundaries.
    static readonly Rect[] bounds={new Rect(65,55,335,350),new Rect(415,85,330,325),new Rect(780,65,275,345),new Rect(1100,85,290,320),
        new Rect(55,470,345,250),new Rect(420,445,300,280),new Rect(770,435,290,290),new Rect(1080,400,310,335),
        new Rect(25,745,385,285),new Rect(410,745,330,290),new Rect(760,745,295,285),new Rect(1090,750,320,285)};
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        if(travel.GetComponent<SchoolController>()!=null)throw new InvalidOperationException("School already installed; edit in place");
        var canvas=travel.transform.parent;
        var meadow=canvas.Find("Meadow Viewport/Meadow");var house=canvas.Find("House Viewport/House");
        if(meadow==null||house==null)throw new InvalidOperationException("Missing home or meadow");
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        icons=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/School/SchoolIcons.png");
        var world=(RectTransform)meadow.Find("Scrolling Landscape");var friends=(RectTransform)meadow.Find("Meadow Friends");
        foreach(var layer in new[]{world,friends})
        {
            for(int i=0;i<layer.childCount;i++)
            {
                var child=(RectTransform)layer.GetChild(i);var a=child.anchorMin;var b=child.anchorMax;
                a.x=(a.x*3.5f+2)/5.5f;b.x=(b.x*3.5f+2)/5.5f;child.anchorMin=a;child.anchorMax=b;
            }
            layer.anchorMin=new Vector2(-3.25f,0);layer.anchorMax=new Vector2(2.25f,1);layer.offsetMin=layer.offsetMax=Vector2.zero;
        }
        var ui=new SchoolController.Widgets();
        ui.schoolPainting=Image("School Meadow",world,0,0,2.25f/5.5f,1,Color.white);
        ui.schoolPainting.sprite=Sprite("SchoolMeadow");ui.schoolPainting.gameObject.AddComponent<MeadowBlend>();
        ui.schoolPainting.transform.SetSiblingIndex(world.Find("Distant Garden").GetSiblingIndex()+1);
        ui.entrance=Button("School Entrance",world,.06f,.27f,.21f,.81f,Color.clear);
        CueIcon(ui.entrance,8,.30f,.35f,.62f,.65f,false);
        ui.sign=Button("School Sign",meadow.parent,.025f,.79f,.13f,.98f,cream);
        ui.signCue=CueIcon(ui.sign,8,.08f,.08f,.92f,.92f,false);
        ui.homeBag=Button("School Bag",house.parent,.025f,.80f,.13f,.98f,cream);
        CueIcon(ui.homeBag,0,.05f,.05f,.95f,.95f,false);
        ui.outdoorBackpack=Icon("Carried Backpack",meadow.Find("Outdoor Rabbit"),0,.10f,.12f,.44f,.42f);
        ui.outdoorBackpack.raycastTarget=false;

        var view=Rect("School Viewport",canvas,0,0,1,1);view.SetSiblingIndex(travel.transform.GetSiblingIndex());
        view.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();ui.view=view.gameObject;
        var room=Image("Classroom",view,0,0,1,1,Color.white);room.sprite=Sprite("Classroom");
        ui.board=Text("Board",view,.29f,.62f,.77f,.86f,"A E I O U",64);ui.board.color=cream;
        ui.teacher=Image("Teacher",view,.085f,.43f,.235f,.81f,Color.white);ui.teacher.sprite=Sprite("Teacher");ui.teacher.preserveAspect=true;ui.teacherRect=ui.teacher.rectTransform;
        ui.standing=Image("School Rabbit",view,.065f,.10f,.195f,.42f,Color.white);ui.standing.preserveAspect=true;ui.standingRect=ui.standing.rectTransform;
        ui.backpack=Icon("Backpack",ui.standing.transform,0,.08f,.12f,.45f,.42f);
        ui.seated=Image("Seated Pupil",view,.20f,.20f,.32f,.49f,Color.white);ui.seated.preserveAspect=true;ui.seatedRect=ui.seated.rectTransform;
        ui.seatedRect.localScale=new Vector3(-1,1,1);
        ui.maleSeated=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Home/MrRabbitSeated.png");
        ui.femaleSeated=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Home/MissRabbitSeated.png");
        ui.femaleStanding=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/MissRabbit.png");
        var cocco=Image("Coccodella At School",view,.435f,.20f,.555f,.49f,Color.white);cocco.sprite=Sprite("CoccodellaSeated");cocco.preserveAspect=true;
        var zebra=Image("Zebrella At School",view,.67f,.20f,.79f,.49f,Color.white);zebra.sprite=Sprite("ZebrellaSeated");zebra.preserveAspect=true;
        ui.delivered=new UnityEngine.UI.Image[3];
        for(int i=0;i<3;i++)
        {
            float x=.18f+i*.235f;
            var desk=Image("Desk "+i,view,x,.08f,x+.23f,.35f,Color.white);desk.sprite=Sprite("SchoolDesk");desk.preserveAspect=true;
            if(i==0)
            {
                ui.desk=Button("Sit At Desk",view,x,.08f,x+.23f,.40f,Color.clear);
                ui.deskCue=CueIcon(ui.desk,7,.24f,-.05f,.59f,.32f,true);
                ui.paper=Button("My Worksheet",view,x+.095f,.24f,x+.185f,.31f,cream);
                // Paper itself is the touch cue, while the desk artwork remains still.
                var symbol=Image("Paper Symbol",ui.paper.transform,.08f,.08f,.92f,.92f,Color.white);
                ui.paperCue=MakeCue(ui.paper,symbol.rectTransform,true);
            }
            ui.delivered[i]=Image("Delivered Sheet "+i,view,x+.095f,.245f,x+.17f,.285f,Color.white);
            ui.delivered[i].gameObject.SetActive(false);
        }
        foreach(var pupil in new[]{ui.seated,cocco,zebra})
        {
            var r=pupil.rectTransform;r.anchorMin=new Vector2(r.anchorMin.x,.12f);r.anchorMax=new Vector2(r.anchorMax.x,.41f);
            r.SetAsLastSibling();
        }
        ui.listen=Button("Listen To Teacher",view,.025f,.65f,.125f,.82f,cream);ui.listenCue=CueIcon(ui.listen,6,.05f,.05f,.95f,.95f,true);
        ui.exit=Button("Leave School",view,.02f,.84f,.12f,.98f,cream);CueIcon(ui.exit,11,.05f,.05f,.95f,.95f,false);
        var subjects=Image("Choose Lesson",view,.25f,.43f,.84f,.60f,cream);ui.subjects=subjects.gameObject;
        ui.lessons=new UnityEngine.UI.Button[2];ui.lessonCues=new ActionCue[2];
        for(int i=0;i<2;i++)
        {
            float x=.15f+i*.4f;ui.lessons[i]=Button("Lesson "+i,subjects.transform,x,.04f,x+.30f,.96f,cream);
            ui.lessonCues[i]=CueIcon(ui.lessons[i],i==0?8:9,.08f,.03f,.82f,.97f,true);
            if(i>0)Text("Operation",ui.lessons[i].transform,.70f,.30f,.98f,.80f,"+",45);
        }

        var reward=Image("Reward",view,.20f,.48f,.44f,.77f,cream);ui.reward=reward.gameObject;
        ui.next=Button("Reward Star",reward.transform,0,0,1,1,Color.clear);ui.rewardCue=CueIcon(ui.next,10,.08f,.20f,.92f,.98f,true);
        ui.stars=Text("Stars",reward.transform,.25f,.01f,.75f,.23f,"0",30);reward.gameObject.SetActive(false);

        var paper=Image("Writing Sheet",view,.10f,.055f,.90f,.95f,cream);paper.raycastTarget=true;ui.worksheet=paper.gameObject;
        var shadow=paper.gameObject.AddComponent<UnityEngine.UI.Shadow>();shadow.effectColor=new Color(0,0,0,.22f);shadow.effectDistance=new Vector2(4,-5);
        ui.question=Text("Question",paper.transform,.14f,.78f,.85f,.97f,"",66);
        ui.back=Button("Put Down Paper",paper.transform,.01f,.83f,.12f,.98f,Color.clear);CueIcon(ui.back,11,.05f,.05f,.95f,.95f,false);
        ui.pads=new HandwritingPad[2];ui.padSlots=new GameObject[2];ui.readback=new TMPro.TMP_Text[2];ui.guides=new TMPro.TMP_Text[2];
        for(int i=0;i<2;i++)
        {
            float left=.16f+i*.35f;
            var slot=Rect("Answer Slot "+i,paper.transform,left,.20f,left+.33f,.76f);ui.padSlots[i]=slot.gameObject;
            var square=Image("Answer Paper",slot,0,0,1,1,new Color32(255,253,244,255));
            var fit=square.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;
            var border=square.gameObject.AddComponent<UnityEngine.UI.Outline>();border.effectColor=new Color32(213,191,148,255);border.effectDistance=new Vector2(2,2);
            var baseline=Image("Baseline",square.transform,.06f,.14f,.94f,.145f,new Color32(201,211,216,255));
            ui.guides[i]=Text("Model",square.transform,.08f,.08f,.92f,.92f,"",180);ui.guides[i].color=new Color(.42f,.58f,.60f,.20f);
            ui.pads[i]=Rect("Finger Ink",square.transform,.04f,.04f,.96f,.96f).gameObject.AddComponent<HandwritingPad>();ui.pads[i].color=new Color32(42,67,108,255);
            ui.readback[i]=Text("Recognized",slot,.28f,-.14f,.72f,-.015f,"",34);
        }
        ui.pencil=Button("Pencil",paper.transform,.02f,.05f,.17f,.21f,Color.clear);ui.pencilCue=CueIcon(ui.pencil,3,.04f,.04f,.96f,.96f,true);
        ui.erase=Button("Eraser",paper.transform,.20f,.025f,.35f,.18f,Color.clear);ui.eraseCue=CueIcon(ui.erase,4,.04f,.04f,.96f,.96f,true);
        ui.help=Button("Show Model",paper.transform,.66f,.025f,.81f,.18f,Color.clear);CueIcon(ui.help,8,.04f,.04f,.96f,.96f,false);
        ui.submit=Button("Give To Teacher",paper.transform,.83f,.05f,.98f,.21f,Color.clear);ui.submitCue=CueIcon(ui.submit,5,.04f,.04f,.96f,.96f,true);
        paper.gameObject.SetActive(false);

        var kit=Image("Pack Schoolbag",canvas,0,0,1,1,new Color32(255,249,232,252));kit.raycastTarget=true;ui.kitPanel=kit.gameObject;
        ui.kitClose=Button("Close Schoolbag",kit.transform,.03f,.82f,.14f,.97f,Color.clear);CueIcon(ui.kitClose,11,.05f,.05f,.95f,.95f,false);
        ui.kit=new UnityEngine.UI.Button[3];ui.kitChecks=new UnityEngine.UI.Image[3];ui.kitCues=new ActionCue[3];
        for(int i=0;i<3;i++)
        {
            float x=.12f+i*.27f;ui.kit[i]=Button("Pack Item "+i,kit.transform,x,.28f,x+.22f,.73f,Color.clear);
            ui.kitCues[i]=CueIcon(ui.kit[i],i,.03f,.03f,.97f,.97f,true);
            var check=Image("Packed",ui.kit[i].transform,.67f,-.10f,.93f,.12f,new Color32(102,171,99,255));check.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");check.preserveAspect=true;ui.kitChecks[i]=check;
        }
        kit.gameObject.SetActive(false);view.gameObject.SetActive(false);
        var school=travel.gameObject.AddComponent<SchoolController>();school.Configure(travel,travel.GetComponent<DayNightCycle>(),ui);
        // Preserve existing destinations and saved garden data; only this new scene content is saved.
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(travel.gameObject.scene);EditorSceneManager.SaveScene(travel.gameObject.scene);AssetDatabase.SaveAssets();
        return "Meadow expanded from 3.5 to 5.5 widths; school, three desks, friends, teacher, schoolbag and handwriting worksheets installed.";
    }
    static Sprite Sprite(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/School/"+name+".png");
    static RectTransform Rect(string name,Transform parent,float x,float y,float right,float top)
    {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(right,top);r.offsetMin=r.offsetMax=Vector2.zero;return r;}
    static UnityEngine.UI.Image Image(string name,Transform parent,float x,float y,float right,float top,Color colour)
    {var im=Rect(name,parent,x,y,right,top).gameObject.AddComponent<UnityEngine.UI.Image>();im.color=colour;im.raycastTarget=false;return im;}
    static UnityEngine.UI.Button Button(string name,Transform parent,float x,float y,float right,float top,Color colour)
    {var im=Image(name,parent,x,y,right,top,colour);im.raycastTarget=true;var b=im.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=im;b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;}
    static TMPro.TMP_Text Text(string name,Transform parent,float x,float y,float right,float top,string words,int size)
    {var t=Rect(name,parent,x,y,right,top).gameObject.AddComponent<TMPro.TextMeshProUGUI>();t.font=font;t.text=words;t.color=ink;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=18;t.fontSizeMax=size;t.alignment=TMPro.TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
    static UnityEngine.UI.RawImage Icon(string name,Transform parent,int cell,float x,float y,float right,float top)
    {
        var slot=Rect(name,parent,x,y,right,top);var art=Rect("Art",slot,0,0,1,1).gameObject.AddComponent<UnityEngine.UI.RawImage>();
        art.texture=icons;var b=bounds[cell];art.uvRect=new Rect(b.x/1448f,1-b.yMax/1086f,b.width/1448f,b.height/1086f);art.raycastTarget=false;
        var fit=art.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=b.width/b.height;return art;
    }
    static ActionCue CueIcon(UnityEngine.UI.Button button,int cell,float x,float y,float right,float top,bool controlled)
    {var art=Icon("Picture",button.transform,cell,x,y,right,top);return MakeCue(button,(RectTransform)art.transform.parent,controlled);}
    static ActionCue MakeCue(UnityEngine.UI.Button button,RectTransform picture,bool controlled)
    {
        var halo=Image("Glow",button.transform,0,0,1,1,new Color(1,.8f,.3f,.3f));halo.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");halo.type=UnityEngine.UI.Image.Type.Sliced;halo.transform.SetAsFirstSibling();halo.enabled=false;
        var cue=button.gameObject.AddComponent<ActionCue>();cue.Configure(picture,halo,controlled);return cue;
    }
}
