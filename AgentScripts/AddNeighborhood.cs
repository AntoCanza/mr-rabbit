using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class AddNeighborhood
{
    private static TMPro.TMP_FontAsset font;
    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play first.");
        var safe = GameObject.Find("Safe Area");
        var canvas = safe.transform.parent;
        var house = canvas.Find("House Viewport/House");
        var meadow = canvas.Find("Meadow Viewport/Meadow");
        if (house == null || meadow == null) throw new InvalidOperationException("Existing home and meadow required.");
        if (safe.GetComponent<Neighborhood>() != null) throw new InvalidOperationException("Neighborhood already installed; edit in place.");
        font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        var framing = house.GetComponent<GameplayFraming>();
        framing.Follow(house.GetComponentInChildren<RabbitWalker>(true));
        EditorUtility.SetDirty(framing);
        var world = meadow.Find("Scrolling Landscape");
        world.Find("Meadow Painting").GetComponent<UnityEngine.UI.Image>().sprite = Import("Neighborhood");
        var friendsLayer = meadow.Find("Meadow Friends");
        var cocco = friendsLayer.Find("Coccodella").GetComponent<MeadowFriend>();
        var zebra = friendsLayer.Find("Zebralla").GetComponent<MeadowFriend>();
        zebra.name = "Zebrella";
        zebra.transform.Find("Name/Label").GetComponent<TMPro.TMP_Text>().text = "Zebrella";
        zebra.Configure("Zebrella", zebra.GetComponent<UnityEngine.UI.Button>(),
            (RectTransform)zebra.transform.Find("Portrait"), zebra.transform.Find("Greeting").gameObject,
            zebra.transform.Find("Greeting/Words").GetComponent<TMPro.TMP_Text>(), "Ciao! Sono Zebrella.\nChe bello vederti!");
        EditorUtility.SetDirty(zebra);

        // Match the generated doors in the panorama; broad touch targets include the facades.
        var doors = new UnityEngine.UI.Button[2];
        string[] names = { "Coccodella", "Zebrella" };
        float[] centers = { .332f, .646f };
        for (int i = 0; i < 2; i++)
        {
            doors[i] = Button("Casa di " + names[i], world,
                new Vector2(centers[i] - .105f, .27f), new Vector2(centers[i] + .105f, .78f));
            var plate = Image("Nameplate", doors[i].transform, new Vector2(.23f, .08f),
                new Vector2(.77f, .18f), new Color32(255, 248, 222, 245));
            Text("Name", plate.transform, names[i], 28, Vector2.zero, Vector2.one);
        }
        var neighborhood = safe.AddComponent<Neighborhood>();
        var view = Rect("Friend House Viewport", canvas, Vector2.zero, Vector2.one);
        view.SetSiblingIndex(safe.transform.GetSiblingIndex());
        view.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        var room = Image("Room", view, Vector2.zero, Vector2.one, Color.white);
        room.sprite = Import("CoccodellaInterior"); room.raycastTarget = true;
        room.gameObject.AddComponent<GameplayFraming>().Configure(4f/3f, new Rect(0,-.16f,1,1));
        room.gameObject.AddComponent<VisitPointerInput>().Configure(neighborhood);
        var exit = Button("Exit Door", room.transform, new Vector2(0,.28f), new Vector2(.19f,.86f));
        var hostButton = Button("Resident",room.transform,new Vector2(.65f,.12f),new Vector2(.82f,.46f));
        var host = Image("Portrait",hostButton.transform,Vector2.zero,Vector2.one,Color.white);
        host.preserveAspect = true;
        var visitorRect = Rect("Visitor",room.transform,new Vector2(.14f,.10f),new Vector2(.30f,.43f));
        visitorRect.pivot = new Vector2(.5f,0);
        var visitor = visitorRect.gameObject.AddComponent<RabbitGraphic>();
        visitor.preserveAspect = true; visitor.raycastTarget = false;
        var titlePlate = Image("House Name",view,new Vector2(.31f,.90f),new Vector2(.69f,.97f),new Color32(255,248,230,235));
        var title = Text("Title",titlePlate.transform,"",30,Vector2.zero,Vector2.one);
        var words = Text("Welcome",view,"",30,new Vector2(.34f,.77f),new Vector2(.91f,.87f));
        words.color = new Color32(54,45,66,255);
        var welcomePanel = words.gameObject.AddComponent<UnityEngine.UI.Shadow>();
        welcomePanel.effectColor = new Color(1,1,1,.8f);
        words.gameObject.SetActive(false);
        neighborhood.Configure(safe.GetComponent<OutdoorAdventure>(),safe.GetComponent<DayNightCycle>(),
            new[] { cocco,zebra }, doors,new[] { room.sprite,Import("ZebrellaInterior") },
            view.gameObject,room,host,visitor,title,words,exit,hostButton);
        view.gameObject.SetActive(false);
        PlayerSettings.bundleVersion = "0.3.0";
        PlayerSettings.Android.bundleVersionCode = 3;
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.MarkSceneDirty(safe.scene);
        EditorSceneManager.SaveScene(safe.scene);
        AssetDatabase.SaveAssets();
        return "Following home camera, two themed visitable houses, resident outings installed; version 0.3.0.";
    }

    private static Sprite Import(string name)
    {
        string path = "Assets/Art/Outdoors/" + name + ".png";
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.mipmapEnabled = false; importer.isReadable = false;
        importer.wrapMode = TextureWrapMode.Clamp; importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        foreach (string platform in new[] { "Android", "iPhone" })
        {
            var settings = importer.GetPlatformTextureSettings(platform);
            settings.name = platform; settings.overridden = true; settings.maxTextureSize = 2048;
            settings.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(settings);
        }
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
    private static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max)
    {
        var rect = new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent,false);rect.anchorMin=min;rect.anchorMax=max;
        rect.offsetMin=rect.offsetMax=Vector2.zero;return rect;
    }
    private static UnityEngine.UI.Image Image(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {
        var image=Rect(name,parent,min,max).gameObject.AddComponent<UnityEngine.UI.Image>();
        image.color=color;image.raycastTarget=false;return image;
    }
    private static UnityEngine.UI.Button Button(string name,Transform parent,Vector2 min,Vector2 max)
    {
        var hit=Image(name,parent,min,max,Color.clear);hit.raycastTarget=true;
        var button=hit.gameObject.AddComponent<UnityEngine.UI.Button>();button.targetGraphic=hit;
        button.transition=UnityEngine.UI.Selectable.Transition.None;
        button.navigation=new UnityEngine.UI.Navigation { mode=UnityEngine.UI.Navigation.Mode.None };
        return button;
    }
    private static TMPro.TMP_Text Text(string name,Transform parent,string value,float size,Vector2 min,Vector2 max)
    {
        var text=Rect(name,parent,min,max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        text.font=font;text.text=value;text.fontSize=size;text.color=new Color32(63,68,60,255);
        text.alignment=TMPro.TextAlignmentOptions.Center;text.enableAutoSizing=true;
        text.fontSizeMin=size*.7f;text.fontSizeMax=size;text.raycastTarget=false;return text;
    }
}
