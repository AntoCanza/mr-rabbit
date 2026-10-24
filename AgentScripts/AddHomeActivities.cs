using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class AddHomeActivities
{
    static TMPro.TMP_FontAsset font;
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first.");
        var pointer=UnityEngine.Object.FindFirstObjectByType<HousePointerInput>(FindObjectsInactive.Include);
        var house=(RectTransform)pointer.transform;
        if(house.GetComponent<HomeActivities>()!=null)throw new InvalidOperationException("Home activities already installed.");
        var rabbit=house.GetComponentInChildren<RabbitWalker>(true);
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        var garden=UnityEngine.Object.FindFirstObjectByType<GardenController>(FindObjectsInactive.Include);
        var bed=house.GetComponentInChildren<BedInteraction>(true);
        if(rabbit==null||travel==null||garden==null||bed==null)throw new InvalidOperationException("Home dependencies missing.");
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        var ui=new HomeActivities.Widgets();
        ui.maleSeated=Sprite("MrRabbitSeated");ui.femaleSeated=Sprite("MissRabbitSeated");
        ui.femaleStanding=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/MissRabbit.png");
        ui.programmes=new[]{AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Outdoors/Meadow.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Garden/GardenBackground.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Outdoors/Neighborhood.png")};

        ui.screen=Image("TV Screen",house,.550f,.240f,.606f,.287f,new Color32(66,69,72,255));
        ui.screen.raycastTarget=false;
        ui.television=Button("Television",house,.537f,.218f,.621f,.307f,Color.clear);
        ui.chair=Button("Armchair",house,.721f,.135f,.854f,.344f,Color.clear);
        Label("TV Hint",house,.724f,.105f,.849f,.147f,"Guarda la TV",18);
        ui.kitchen=Button("Cooking Station",house,.310f,.105f,.421f,.280f,Color.clear);
        var furniture=Image("Cooking Furniture",ui.kitchen.transform,0,0,1,1,Color.white);
        furniture.sprite=Sprite("CookingStation");furniture.preserveAspect=true;furniture.raycastTarget=false;
        Label("Kitchen Hint",house,.313f,.100f,.421f,.140f,"Cucina",18);
        ui.steam=Rect("Cooking Steam",house,.325f,.262f,.370f,.323f).gameObject.AddComponent<HearthGraphic>();
        ui.steam.steam=true;ui.steam.raycastTarget=false;ui.steam.gameObject.SetActive(false);
        // The walker renders in front of the new worktable.
        rabbit.transform.SetAsLastSibling();
        rabbit.GetComponent<RabbitGraphic>().raycastTarget=false;
        ui.seated=Image("Seated Rabbit",house,.748f,.148f,.833f,.343f,Color.white);
        ui.seated.preserveAspect=true;ui.seated.raycastTarget=false;ui.seated.gameObject.SetActive(false);
        ui.plate=Rect("Dinner Plate",house,.790f,.567f,.838f,.600f).gameObject.AddComponent<HearthGraphic>();
        ui.plate.raycastTarget=false;ui.plate.gameObject.SetActive(false);
        ui.feedback=Label("Meal Feedback",house,.665f,.675f,.850f,.725f,"",22);
        ui.feedback.gameObject.SetActive(false);

        var controls=Rect("TV Controls",house.parent,.62f,.025f,.97f,.105f);
        ui.tvControls=controls.gameObject;
        ui.stand=Button("Stand Up",controls,0,0,.47f,1,new Color32(255,235,188,255));
        Label("Label",ui.stand.transform,0,0,1,1,"Alzati",22);
        ui.sound=Button("TV Sound",controls,.50f,0,1,1,new Color32(214,238,222,255));
        ui.soundLabel=Label("Label",ui.sound.transform,0,0,1,1,"Audio: sì",22);
        controls.gameObject.SetActive(false);

        var panel=Image("Kitchen Menu",house.parent,.10f,.65f,.90f,.97f,new Color32(255,249,231,252));
        ui.kitchenPanel=panel.gameObject;
        var shadow=panel.gameObject.AddComponent<UnityEngine.UI.Shadow>();shadow.effectColor=new Color(0,0,0,.20f);shadow.effectDistance=new Vector2(0,-5);
        Label("Title",panel.transform,.03f,.82f,.84f,.98f,"Dall'orto alla tavola",26);
        ui.close=Button("Close Kitchen",panel.transform,.89f,.80f,.99f,.99f,new Color32(237,211,174,255));
        Label("Label",ui.close.transform,0,0,1,1,"Chiudi",18);
        ui.recipes=new UnityEngine.UI.Button[4];ui.recipeLabels=new TMPro.TMP_Text[4];
        for(int i=0;i<4;i++)
        {
            float x=.02f+i*.245f;
            ui.recipes[i]=Button("Recipe "+i,panel.transform,x,.35f,x+.23f,.79f,new Color32(255,245,218,255));
            ui.recipeLabels[i]=Label("Label",ui.recipes[i].transform,.04f,.04f,.96f,.55f,GardenProgress.RecipeNames[i],18);
            var slot=Rect("Ingredient Slot",ui.recipes[i].transform,.30f,.54f,.70f,.98f);
            var icon=Rect("Ingredient",slot,0,0,1,1).gameObject.AddComponent<UnityEngine.UI.RawImage>();
            icon.texture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/GardenAtlas.png");icon.raycastTarget=false;
            icon.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>().aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;
            GardenArt.Apply(icon,new[]{4,6,5,7}[i]);
        }
        ui.status=Label("Status",panel.transform,.03f,.20f,.97f,.34f,"",19);
        ui.cook=Button("Cook",panel.transform,.20f,.025f,.47f,.19f,new Color32(244,188,92,255));
        Label("Label",ui.cook.transform,0,0,1,1,"Cucina",21);
        ui.eat=Button("Eat",panel.transform,.51f,.025f,.78f,.19f,new Color32(154,205,173,255));
        Label("Label",ui.eat.transform,0,0,1,1,"Mangia",21);
        ui.progress=Image("Cooking Progress",panel.transform,.025f,.005f,.975f,.018f,new Color32(86,150,118,255));
        // A filled image needs a sprite; the UI's built-in white sprite is sufficient.
        ui.progress.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        ui.progress.type=UnityEngine.UI.Image.Type.Filled;ui.progress.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;ui.progress.fillAmount=0;ui.progress.raycastTarget=false;
        panel.gameObject.SetActive(false);
        var activities=house.gameObject.AddComponent<HomeActivities>();activities.Configure(rabbit,travel,garden,bed,ui);
        if(UnityEngine.Object.FindFirstObjectByType<AudioListener>()==null)
        {
            var camera=UnityEngine.Object.FindFirstObjectByType<Camera>();
            if(camera==null)throw new InvalidOperationException("Missing camera for audio listener.");
            camera.gameObject.AddComponent<AudioListener>();
        }
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.MarkSceneDirty(house.gameObject.scene);EditorSceneManager.SaveScene(house.gameObject.scene);AssetDatabase.SaveAssets();
        return "TV, seated poses, original TV jingle, kitchen with four recipes, saved dishes, and dining table connected.";
    }
    static Sprite Sprite(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Home/"+name+".png");
    static RectTransform Rect(string name,Transform parent,float x,float y,float xmax,float ymax)
    {
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);
        r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(xmax,ymax);r.offsetMin=r.offsetMax=Vector2.zero;return r;
    }
    static UnityEngine.UI.Image Image(string name,Transform parent,float x,float y,float xmax,float ymax,Color colour)
    {var im=Rect(name,parent,x,y,xmax,ymax).gameObject.AddComponent<UnityEngine.UI.Image>();im.color=colour;return im;}
    static UnityEngine.UI.Button Button(string name,Transform parent,float x,float y,float xmax,float ymax,Color colour)
    {
        var im=Image(name,parent,x,y,xmax,ymax,colour);var b=im.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=im;
        b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;
    }
    static TMPro.TMP_Text Label(string name,Transform parent,float x,float y,float xmax,float ymax,string words,int size)
    {
        var t=Rect(name,parent,x,y,xmax,ymax).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        t.font=font;t.text=words;t.color=new Color32(83,56,39,255);t.fontSize=size;
        t.enableAutoSizing=true;t.fontSizeMin=12;t.fontSizeMax=size;t.alignment=TMPro.TextAlignmentOptions.Center;
        t.raycastTarget=false;return t;
    }
}
