using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddFriends
{
    private static TMPro.TMP_FontAsset font;
    private static readonly Color Ink = new Color32(63, 75, 67, 255);

    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before editing.");
        var safe = GameObject.Find("Safe Area");
        var world = safe.transform.Find("Meadow Viewport/Meadow/Scrolling Landscape");
        if (world == null || safe.GetComponent<MrRabbit.OutdoorAdventure>() == null)
            throw new InvalidOperationException("The existing meadow is required.");
        if (safe.GetComponentInChildren<MrRabbit.MeadowFriend>(true) != null)
            throw new InvalidOperationException("Friends already exist; edit in place.");
        font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null) throw new InvalidOperationException("Missing project font.");

        var friends = Rect("Meadow Friends",world.parent,Vector2.zero,
            new Vector2(MrRabbit.OutdoorAdventure.WorldWidth,1));
        friends.SetSiblingIndex(world.parent.Find("Outdoor Rabbit").GetSiblingIndex());
        var adventure = safe.GetComponent<MrRabbit.OutdoorAdventure>();
        adventure.SetFriendsLayer(friends);
        EditorUtility.SetDirty(adventure);
        CreateFriend(friends, "Zebralla", 1.42f, new Color32(243, 229, 255, 255),
            "Ciao! Sono Zebralla.\nChe bello vederti!");
        CreateFriend(friends, "Coccodella", .72f, new Color32(220, 245, 245, 255),
            "Ciao! Sono Coccodella.\nGiochiamo nel prato!");
        var hint = Image("Friends Hint", world.parent, new Vector2(.25f,.91f),
            new Vector2(.75f,.975f), new Color32(255,249,230,240));
        Text("Label",hint.transform,"Tocca le amiche per salutarle",26,Vector2.zero,Vector2.one);
        Canvas.ForceUpdateCanvases();
        EditorSceneManager.MarkSceneDirty(safe.scene);
        EditorSceneManager.SaveScene(safe.scene);
        AssetDatabase.SaveAssets();
        return "Zebralla and Coccodella are in the meadow with names and tap greetings.";
    }

    private static void CreateFriend(Transform world, string name, float x, Color cardColor, string words)
    {
        var root = Rect(name,world,new Vector2((x-.11f)/MrRabbit.OutdoorAdventure.WorldWidth,.22f),
            new Vector2((x+.11f)/MrRabbit.OutdoorAdventure.WorldWidth,.57f));
        var hit = root.gameObject.AddComponent<UnityEngine.UI.Image>();
        hit.color = Color.clear; hit.raycastTarget = true;
        var button = root.gameObject.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = hit;
        button.transition = UnityEngine.UI.Selectable.Transition.None;
        button.navigation = new UnityEngine.UI.Navigation { mode = UnityEngine.UI.Navigation.Mode.None };
        var portrait = Image("Portrait",root,Vector2.zero,Vector2.one,Color.white);
        portrait.sprite = Import("Assets/Art/Characters/" + name + ".png");
        portrait.preserveAspect = true;
        portrait.rectTransform.pivot = new Vector2(.5f,0);
        var plate = Image("Name",root,new Vector2(-.10f,1.02f),new Vector2(1.10f,1.19f),cardColor);
        // The nameplate is also part of the same generous touch target.
        plate.raycastTarget = true;
        Text("Label",plate.transform,name,30,Vector2.zero,Vector2.one,true);
        var bubble = Image("Greeting",root,new Vector2(-.28f,1.24f),new Vector2(1.28f,1.64f),
            new Color32(255,250,235,255));
        var message = Text("Words",bubble.transform,words,27,new Vector2(.04f,.06f),new Vector2(.96f,.94f));
        root.gameObject.AddComponent<MrRabbit.MeadowFriend>().Configure(
            name,button,portrait.rectTransform,bubble.gameObject,message,words);
        bubble.gameObject.SetActive(false);
    }

    private static Sprite Import(string path)
    {
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false; importer.isReadable = false;
        importer.wrapMode = TextureWrapMode.Clamp; importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 512;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        foreach (var platform in new[] { "Android", "iPhone" })
        {
            var settings = importer.GetPlatformTextureSettings(platform);
            settings.name = platform; settings.overridden = true;
            settings.maxTextureSize = 512; settings.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(settings);
        }
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max)
    {
        var rect = new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent,false); rect.anchorMin=min; rect.anchorMax=max;
        rect.offsetMin=rect.offsetMax=Vector2.zero; return rect;
    }
    private static UnityEngine.UI.Image Image(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {
        var image=Rect(name,parent,min,max).gameObject.AddComponent<UnityEngine.UI.Image>();
        image.color=color; image.raycastTarget=false; return image;
    }
    private static TMPro.TMP_Text Text(string name,Transform parent,string words,float size,Vector2 min,Vector2 max,bool bold=false)
    {
        var text=Rect(name,parent,min,max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        text.font=font; text.text=words; text.fontSize=size; text.color=Ink;
        text.alignment=TMPro.TextAlignmentOptions.Center;
        text.fontStyle=bold?TMPro.FontStyles.Bold:TMPro.FontStyles.Normal;
        text.enableAutoSizing=true; text.fontSizeMin=size*.7f; text.fontSizeMax=size;
        text.raycastTarget=false; return text;
    }
}
