using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class AddGarden
{
    static TMPro.TMP_FontAsset font;
    static Texture2D atlas,icons;
    static Sprite circle;
    static Color cream=new Color32(255,248,222,255),ink=new Color32(66,69,48,255);
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var safe=GameObject.Find("Safe Area");
        if(safe==null||safe.GetComponent<GardenController>()!=null)throw new InvalidOperationException("Expected existing scene without garden.");
        var canvas=safe.transform.parent;
        var meadow=canvas.Find("Meadow Viewport/Meadow");
        var world=(RectTransform)meadow.Find("Scrolling Landscape");
        var friends=(RectTransform)meadow.Find("Meadow Friends");
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        circle=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        var approachSprite=Import("GardenApproach",true);
        var background=Import("GardenBackground",true);
        Import("GardenAtlas",false);Import("ActionIcons",false);
        atlas=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/GardenAtlas.png");
        icons=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/ActionIcons.png");
        if(atlas==null||icons==null||background==null||approachSprite==null)throw new InvalidOperationException("Missing garden artwork.");
        // Extend to the left while keeping every existing landmark at exactly its old world coordinate.
        RemapChildren(world);RemapChildren(friends);
        foreach(var layer in new[]{world,friends})
        {layer.anchorMin=new Vector2(OutdoorAdventure.WorldLeft,0);layer.anchorMax=new Vector2(2.25f,1);layer.offsetMin=layer.offsetMax=Vector2.zero;}
        var approach=Image("Distant Garden",world,new Vector2(0,0),new Vector2(1.41f/3.5f,1),Color.white);
        approach.sprite=approachSprite;approach.raycastTarget=true;
        approach.transform.SetSiblingIndex(world.Find("Meadow Painting").GetSiblingIndex()+1);
        approach.gameObject.AddComponent<MeadowBlend>();
        var gate=Button("Garden Gate",world,new Vector2(.055f,.32f),new Vector2(.29f,.72f),Color.clear);
        Badge(gate.transform,4,new Vector2(.50f,1.05f),95);
        var controller=safe.AddComponent<GardenController>();
        var view=Rect("Garden Viewport",canvas,Vector2.zero,Vector2.one);
        view.SetSiblingIndex(safe.transform.GetSiblingIndex());view.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        var bg=Image("Garden Painting",view,Vector2.zero,Vector2.one,Color.white);bg.sprite=background;
        bg.gameObject.AddComponent<GameplayFraming>().Configure(4f/3f,new Rect(0,0,1,1));
        // Controls use the safe area, independent of decorative cropping in the painting.
        var controls=Rect("Garden Controls",view,Vector2.zero,Vector2.one);controls.gameObject.AddComponent<SafeAreaLayout>();
        var ui=new GardenController.Widgets {view=view.gameObject,background=bg,approach=approach,gate=gate};
        ui.exit=Button("Back To Meadow",controls,new Vector2(.025f,.875f),new Vector2(.14f,.975f),cream);
        Cell("Back",ui.exit.transform,icons,8,new Vector2(.24f,.06f),new Vector2(.76f,.94f));
        var guidePanel=Image("Guide",controls,new Vector2(.19f,.835f),new Vector2(.76f,.975f),cream);
        ui.guide=Text("Instruction",guidePanel.transform,"",34,new Vector2(.03f,.42f),new Vector2(.97f,.96f));
        ui.guide.fontStyle=TMPro.FontStyles.Bold;
        ui.detail=Text("Help",guidePanel.transform,"",20,new Vector2(.03f,.07f),new Vector2(.97f,.44f));
        var countPanel=Image("Harvest Count",controls,new Vector2(.79f,.875f),new Vector2(.975f,.975f),cream);
        Cell("Basket",countPanel.transform,icons,6,new Vector2(.02f,.10f),new Vector2(.36f,.90f));
        ui.count=Text("Count",countPanel.transform,"",24,new Vector2(.35f,0),Vector2.one);
        ui.tools=new UnityEngine.UI.Button[4];
        string[] toolNames={"Zappa","Semi","Acqua","Cestino"};int[] cells={0,2,1,3};
        for(int i=0;i<4;i++)
        {
            float y=.64f-i*.155f;
            var button=Button(toolNames[i],controls,new Vector2(.025f,y),new Vector2(.16f,y+.135f),cream);
            ui.tools[i]=button;
            Cell("Tool",button.transform,atlas,cells[i],new Vector2(.10f,.19f),new Vector2(.90f,.98f));
            Text("Name",button.transform,toolNames[i],18,new Vector2(.03f,.01f),new Vector2(.97f,.22f));
        }
        ui.beds=new UnityEngine.UI.Button[3];ui.soil=new GardenBedGraphic[3];
        ui.plants=new UnityEngine.UI.RawImage[9];ui.bedLabels=new TMPro.TMP_Text[3];
        ui.progress=new UnityEngine.UI.Image[3];ui.bedIcons=new UnityEngine.UI.RawImage[3];
        for(int i=0;i<3;i++)
        {
            float x=.275f+i*.23f;
            var bed=Button("Aiuola "+(i+1),controls,new Vector2(x,.31f),new Vector2(x+.205f,.58f),Color.clear);
            ui.beds[i]=bed;
            var soil=Rect("Soil",bed.transform,Vector2.zero,new Vector2(1,.59f)).gameObject.AddComponent<GardenBedGraphic>();
            soil.raycastTarget=false;ui.soil[i]=soil;
            for(int j=0;j<3;j++)ui.plants[i*3+j]=Cell("Plant "+j,bed.transform,atlas,8,
                new Vector2(.01f+j*.29f,.08f),new Vector2(.40f+j*.29f,1.03f));
            var plate=Image("Status",bed.transform,new Vector2(0,-.25f),new Vector2(1,-.04f),cream);
            ui.bedLabels[i]=Text("Words",plate.transform,"",20,Vector2.zero,Vector2.one);
            var track=Image("Growth Track",bed.transform,new Vector2(.05f,-.36f),new Vector2(.95f,-.30f),new Color32(228,219,190,255));
            var fill=Image("Growth",track.transform,Vector2.zero,Vector2.one,new Color32(101,164,87,255));
            fill.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            fill.type=UnityEngine.UI.Image.Type.Filled;fill.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;fill.fillAmount=0;
            ui.progress[i]=fill;
            ui.bedIcons[i]=Badge(bed.transform,11,new Vector2(.50f,1.06f),65);
        }
        var rabbitRect=Rect("Garden Rabbit",controls,new Vector2(.085f,.13f),new Vector2(.215f,.40f));
        rabbitRect.pivot=new Vector2(.5f,0);
        ui.rabbit=rabbitRect.gameObject.AddComponent<RabbitGraphic>();ui.rabbit.preserveAspect=true;ui.rabbit.raycastTarget=false;
        ui.heldIcon=Cell("Held Tool",rabbitRect,atlas,0,new Vector2(.55f,.20f),new Vector2(1.12f,.70f));ui.heldIcon.gameObject.SetActive(false);
        var arrowRect=Rect("Guiding Arrow",controls,new Vector2(.5f,.5f),new Vector2(.5f,.5f));
        arrowRect.pivot=new Vector2(.5f,0);arrowRect.sizeDelta=new Vector2(48,58);
        ui.arrow=arrowRect.gameObject.AddComponent<GardenGuideArrow>();ui.arrow.color=new Color32(255,184,47,255);ui.arrow.raycastTarget=false;
        var shadow=arrowRect.gameObject.AddComponent<UnityEngine.UI.Shadow>();shadow.effectColor=new Color32(93,62,33,255);shadow.effectDistance=new Vector2(2,-2);
        ui.seedPanel=Image("Choose Seeds",controls,Vector2.zero,Vector2.one,new Color32(45,67,46,235)).gameObject;
        ui.seedPanel.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;
        Text("Title",ui.seedPanel.transform,"Quali semi piantiamo?",38,new Vector2(.15f,.77f),new Vector2(.85f,.88f));
        ui.seedPanel.transform.Find("Title").GetComponent<TMPro.TMP_Text>().color=cream;
        ui.seedChoices=new UnityEngine.UI.Button[4];int[] cropCells={4,6,5,7};
        for(int i=0;i<4;i++)
        {
            float left=.075f+i*.22f;
            var card=Button(GardenProgress.CropNames[i],ui.seedPanel.transform,new Vector2(left,.28f),new Vector2(left+.19f,.70f),cream);
            ui.seedChoices[i]=card;
            Cell("Crop",card.transform,atlas,cropCells[i],new Vector2(.06f,.23f),new Vector2(.94f,.94f));
            Text("Name",card.transform,GardenProgress.CropNames[i],29,new Vector2(.04f,.04f),new Vector2(.96f,.22f));
        }
        ui.cancelSeeds=Button("Back",ui.seedPanel.transform,new Vector2(.43f,.08f),new Vector2(.57f,.20f),cream);
        Cell("Back Icon",ui.cancelSeeds.transform,icons,8,new Vector2(.24f,.06f),new Vector2(.76f,.94f));
        ui.seedPanel.SetActive(false);
        // Notification stays in the safe area while the meadow scrolls behind it.
        ui.notice=Button("Garden Notice",safe.transform,new Vector2(.24f,.86f),new Vector2(.76f,.975f),cream);
        ui.noticeIcon=Cell("Event Icon",ui.notice.transform,icons,5,new Vector2(.02f,.03f),new Vector2(.16f,.97f));
        ui.secondaryNoticeIcon=Cell("Water Too",ui.notice.transform,icons,5,new Vector2(.17f,.03f),new Vector2(.31f,.97f));
        ui.noticeText=Text("Message",ui.notice.transform,"",28,new Vector2(.33f,.08f),new Vector2(.97f,.92f));
        ui.notice.gameObject.SetActive(false);
        controller.Configure(safe.GetComponent<OutdoorAdventure>(),safe.GetComponent<DayNightCycle>(),ui);
        view.gameObject.SetActive(false);
        InstallActionIcons(safe.transform,canvas,meadow);
        PlayerSettings.bundleVersion="0.4.0";PlayerSettings.Android.bundleVersionCode=4;
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(safe.scene);EditorSceneManager.SaveScene(safe.scene);AssetDatabase.SaveAssets();
        return "Garden, guided crop cycle, persistence, extended meadow and pictorial action hints installed; 0.4.0 (4).";
    }
    static void InstallActionIcons(Transform safe,Transform canvas,Transform meadow)
    {
        var world=meadow.Find("Scrolling Landscape");
        var sign=world.Find("Home Sign");
        sign.Find("Label").gameObject.SetActive(false);sign.GetComponent<UnityEngine.UI.Image>().color=Color.clear;
        Badge(sign,0,new Vector2(.5f,.5f),90);
        world.Find("Signpost").gameObject.SetActive(false);
        foreach(string name in new[]{"Coccodella","Zebrella"})
        {
            var door=world.Find("Casa di "+name);door.Find("Nameplate").gameObject.SetActive(false);
            Badge(door,1,new Vector2(.5f,.20f),80);
            var friend=meadow.Find("Meadow Friends/"+name);friend.Find("Name").gameObject.SetActive(false);
            Badge(friend,3,new Vector2(.5f,1.08f),72);
        }
        var house=canvas.Find("House Viewport/House");
        Badge(house.Find("Exit Door"),1,new Vector2(.5f,.66f),80);
        Badge(house.Find("Bed"),2,new Vector2(.72f,.30f),90);
        var visit=canvas.Find("Friend House Viewport/Room");
        Badge(visit.Find("Exit Door"),8,new Vector2(.5f,.45f),80);
        Badge(visit.Find("Resident"),3,new Vector2(.5f,1.05f),70);
        var cancel=safe.Find("Footwear Choice/Stay Home");
        cancel.Find("Label").gameObject.SetActive(false);Badge(cancel,0,new Vector2(.5f,.5f),70);
        // Character/footwear choices already have large pictures; add a check to signal selection.
        foreach(var selection in safe.GetComponentsInChildren<UnityEngine.UI.Button>(true))
            if(selection.name=="Choose Mr Rabbit"||selection.name=="Choose Miss Rabbit"||selection.name=="Choose Shoes"||selection.name=="Choose Rollers")
                Badge(selection.transform,9,new Vector2(.88f,.13f),52);
    }
    static void RemapChildren(RectTransform parent)
    {
        for(int i=0;i<parent.childCount;i++)
        {
            var r=parent.GetChild(i) as RectTransform;if(r==null)continue;
            r.anchorMin=new Vector2((r.anchorMin.x*2.25f+1.25f)/3.5f,r.anchorMin.y);
            r.anchorMax=new Vector2((r.anchorMax.x*2.25f+1.25f)/3.5f,r.anchorMax.y);
        }
    }
    static Sprite Import(string name,bool sprite)
    {
        string path="Assets/Art/Garden/"+name+".png";AssetDatabase.ImportAsset(path);
        var i=(TextureImporter)AssetImporter.GetAtPath(path);if(i==null)throw new InvalidOperationException(path);
        i.textureType=sprite?TextureImporterType.Sprite:TextureImporterType.Default;i.spriteImportMode=SpriteImportMode.Single;
        i.alphaIsTransparency=true;i.mipmapEnabled=false;i.isReadable=false;i.wrapMode=TextureWrapMode.Clamp;
        i.maxTextureSize=2048;i.textureCompression=TextureImporterCompression.CompressedHQ;
        foreach(string p in new[]{"Android","iPhone"}){var s=i.GetPlatformTextureSettings(p);s.name=p;s.overridden=true;s.maxTextureSize=2048;s.format=TextureImporterFormat.ASTC_6x6;i.SetPlatformTextureSettings(s);}
        i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max)
    {var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;return r;}
    static UnityEngine.UI.Image Image(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {var i=Rect(name,parent,min,max).gameObject.AddComponent<UnityEngine.UI.Image>();i.color=color;i.raycastTarget=false;return i;}
    static UnityEngine.UI.Button Button(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {var i=Image(name,parent,min,max,color);i.raycastTarget=true;var b=i.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=i;b.transition=UnityEngine.UI.Selectable.Transition.None;b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;}
    static TMPro.TMP_Text Text(string name,Transform parent,string words,float size,Vector2 min,Vector2 max)
    {var t=Rect(name,parent,min,max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();t.font=font;t.text=words;t.fontSize=size;t.enableAutoSizing=true;t.fontSizeMin=size*.75f;t.fontSizeMax=size;t.color=ink;t.alignment=TMPro.TextAlignmentOptions.Center;t.raycastTarget=false;return t;}
    static UnityEngine.UI.RawImage Cell(string name,Transform parent,Texture2D texture,int cell,Vector2 min,Vector2 max)
    {
        var slot=Rect(name,parent,min,max);var r=Rect("Art",slot,Vector2.zero,Vector2.one);
        var fit=r.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;
        var image=r.gameObject.AddComponent<UnityEngine.UI.RawImage>();image.texture=texture;GardenArt.Apply(image,cell);image.raycastTarget=false;return image;
    }
    static UnityEngine.UI.RawImage Badge(Transform parent,int cell,Vector2 anchor,float size)
    {
        var r=Rect("Action Hint",parent,anchor,anchor);r.sizeDelta=new Vector2(size,size);
        var bg=r.gameObject.AddComponent<UnityEngine.UI.Image>();bg.sprite=circle;bg.color=cream;bg.raycastTarget=true;
        var outline=r.gameObject.AddComponent<UnityEngine.UI.Outline>();outline.effectColor=new Color32(117,144,91,255);outline.effectDistance=new Vector2(2,-2);
        return Cell("Symbol",r,icons,cell,new Vector2(.04f,.04f),new Vector2(.96f,.96f));
    }
}
