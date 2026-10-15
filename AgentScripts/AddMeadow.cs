using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddMeadow
{
    private static TMPro.TMP_FontAsset font;
    private static readonly Color Ink = new Color32(63, 88, 69, 255);
    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before editing.");
        var safe = GameObject.Find("Safe Area").GetComponent<RectTransform>();
        if (safe.GetComponent<MrRabbit.OutdoorAdventure>() != null)
            throw new InvalidOperationException("Meadow already exists; edit it in place.");
        var home = (RectTransform)safe.Find("House Viewport");
        var house = (RectTransform)home.Find("House");
        var player = home.GetComponentInChildren<MrRabbit.RabbitWalker>(true);
        var selection = safe.GetComponent<MrRabbit.CharacterSelection>();
        font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (player == null || selection == null || font == null) throw new InvalidOperationException("Missing Home references.");
        var meadow = Import("Assets/Art/Outdoors/Meadow.png", 2048, false);
        var roller = Import("Assets/Art/Outdoors/Roller.png", 512, true);

        var door = Button("Exit Door", house, new Vector2(.165f,.118f), new Vector2(.235f,.32f), new Color32(100,65,43,255));
        var panel = Image("Door Panel", door.transform, new Vector2(.10f,.02f), new Vector2(.90f,.97f), new Color32(188,120,68,255));
        Image("Window Frame", panel.transform, new Vector2(.13f,.58f),new Vector2(.87f,.90f),new Color32(102,71,48,255));
        Image("Door Window", panel.transform, new Vector2(.21f,.62f),new Vector2(.79f,.86f),new Color32(177,223,223,255));
        Image("Handle", panel.transform, new Vector2(.74f,.38f),new Vector2(.92f,.45f),new Color32(255,217,108,255));
        Text("Exit Label",door.transform,"< Esci",26,new Vector2(-.35f,1.01f),new Vector2(1.35f,1.23f),true);
        door.transform.SetSiblingIndex(player.transform.GetSiblingIndex());

        var outdoors = Rect("Meadow Viewport", safe, home.anchorMin, home.anchorMax);
        outdoors.offsetMin = home.offsetMin; outdoors.offsetMax = home.offsetMax;
        var area = Rect("Meadow",outdoors,Vector2.zero,Vector2.one);
        var aspect = area.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();
        aspect.aspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;
        aspect.aspectRatio = 4f / 3f;
        area.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        var world = Rect("Scrolling Landscape",area,Vector2.zero,new Vector2(MrRabbit.OutdoorAdventure.WorldWidth,1));
        var background = Image("Meadow Painting",world,Vector2.zero,Vector2.one,Color.white);
        background.sprite = meadow; background.raycastTarget = true;
        var sign = Button("Home Sign",world,new Vector2(.905f,.38f),new Vector2(.989f,.48f),new Color32(255,241,201,255));
        Text("Label",sign.transform,"Casa >",29,Vector2.zero,Vector2.one,true);
        Image("Signpost",world,new Vector2(.941f,.18f),new Vector2(.953f,.38f),new Color32(147,101,62,255));
        var rabbitRect = Rect("Outdoor Rabbit",area,new Vector2(.63f,.18f),new Vector2(.83f,.56f));
        rabbitRect.pivot = new Vector2(.5f,0);
        var rabbit = rabbitRect.gameObject.AddComponent<MrRabbit.RabbitGraphic>();
        rabbit.sprite = player.GetComponent<MrRabbit.RabbitGraphic>().sprite;
        rabbit.preserveAspect = true;
        rabbit.raycastTarget = false;
        var pair = Rect("Roller Pair",rabbitRect,Vector2.zero,Vector2.one);
        var left = Image("Left Roller",pair,Vector2.zero,Vector2.one,Color.white);
        var right = Image("Right Roller",pair,Vector2.zero,Vector2.one,Color.white);
        left.sprite = right.sprite = roller;
        pair.gameObject.SetActive(false);

        var modal = Image("Footwear Choice",safe,Vector2.zero,Vector2.one,new Color32(250,247,234,255));
        modal.raycastTarget = true;
        Text("Title",modal.transform,"Usciamo a giocare?",52,new Vector2(.08f,.83f),new Vector2(.92f,.94f),true);
        Text("Subtitle",modal.transform,"Scegli cosa mettere ai piedi",29,new Vector2(.08f,.76f),new Vector2(.92f,.83f));
        var row = Rect("Footwear Cards",modal.transform,new Vector2(.17f,.23f),new Vector2(.83f,.73f));
        var layout = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        layout.spacing = 32; layout.childControlHeight = layout.childControlWidth = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = true;
        var shoes = Button("Choose Shoes",row,Vector2.zero,Vector2.one,new Color32(230,241,235,255));
        var preview = Image("Shoes Preview",shoes.transform,new Vector2(.15f,.27f),new Vector2(.85f,.95f),Color.white);
        preview.sprite = rabbit.sprite; preview.preserveAspect = true;
        Text("Label",shoes.transform,"Scarpe normali",34,new Vector2(.03f,.06f),new Vector2(.97f,.23f),true);
        var skates = Button("Choose Rollers",row,Vector2.zero,Vector2.one,new Color32(250,230,212,255));
        var skateIcon = Image("Roller Preview",skates.transform,new Vector2(.14f,.29f),new Vector2(.86f,.94f),Color.white);
        skateIcon.sprite = roller; skateIcon.preserveAspect = true;
        Text("Label",skates.transform,"Roller",34,new Vector2(.05f,.06f),new Vector2(.95f,.23f),true);
        var cancel = Button("Stay Home",modal.transform,new Vector2(.34f,.10f),new Vector2(.66f,.19f),new Color32(224,231,211,255));
        Text("Label",cancel.transform,"Resta in casa",27,Vector2.zero,Vector2.one);
        shoes.navigation = new UnityEngine.UI.Navigation {mode=UnityEngine.UI.Navigation.Mode.Explicit,selectOnRight=skates,selectOnDown=cancel};
        skates.navigation = new UnityEngine.UI.Navigation {mode=UnityEngine.UI.Navigation.Mode.Explicit,selectOnLeft=shoes,selectOnDown=cancel};
        cancel.navigation = new UnityEngine.UI.Navigation {mode=UnityEngine.UI.Navigation.Mode.Explicit,selectOnUp=shoes};

        var adventure = safe.gameObject.AddComponent<MrRabbit.OutdoorAdventure>();
        var hint = safe.Find("Touch Hint").GetComponent<TMPro.TMP_Text>();
        hint.enableAutoSizing = true; hint.fontSizeMin = 16; hint.fontSizeMax = 26;
        adventure.Configure(player,home.gameObject,outdoors.gameObject,modal.gameObject,world,rabbit,
            pair.gameObject,left.rectTransform,right.rectTransform,door,shoes,skates,cancel,sign,hint,preview);
        area.gameObject.AddComponent<MrRabbit.OutdoorPointerInput>().Configure(adventure);
        selection.SetAdventure(adventure);
        outdoors.gameObject.SetActive(false); modal.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        EditorUtility.SetDirty(selection); EditorUtility.SetDirty(adventure);
        EditorSceneManager.MarkSceneDirty(safe.gameObject.scene);
        EditorSceneManager.SaveScene(safe.gameObject.scene); AssetDatabase.SaveAssets();
        return "Left exit door, footwear menu, scrolling meadow and automatic shoes on return configured.";
    }

    private static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max)
    {
        var r = new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(parent,false); r.anchorMin=min; r.anchorMax=max; r.offsetMin=r.offsetMax=Vector2.zero; return r;
    }
    private static UnityEngine.UI.Image Image(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {
        var image=Rect(name,parent,min,max).gameObject.AddComponent<UnityEngine.UI.Image>();
        image.color=color; image.raycastTarget=false; return image;
    }
    private static UnityEngine.UI.Button Button(string name,Transform parent,Vector2 min,Vector2 max,Color color)
    {
        var face=Image(name,parent,min,max,color); face.raycastTarget=true;
        var b=face.gameObject.AddComponent<UnityEngine.UI.Button>(); b.targetGraphic=face;
        b.navigation=new UnityEngine.UI.Navigation {mode=UnityEngine.UI.Navigation.Mode.None};
        var colors=b.colors; colors.selectedColor=new Color(1,.91f,.69f); colors.pressedColor=new Color(.8f,.85f,.7f); b.colors=colors;
        return b;
    }
    private static void Text(string name,Transform parent,string words,float size,Vector2 min,Vector2 max,bool bold=false)
    {
        var t=Rect(name,parent,min,max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        t.font=font;t.text=words;t.fontSize=size;t.color=Ink;t.alignment=TMPro.TextAlignmentOptions.Center;
        t.fontStyle=bold?TMPro.FontStyles.Bold:TMPro.FontStyles.Normal;
        t.enableAutoSizing=true;t.fontSizeMin=size*.65f;t.fontSizeMax=size;t.raycastTarget=false;
    }
    private static Sprite Import(string path,int max,bool alpha)
    {
        AssetDatabase.ImportAsset(path);
        var i=(TextureImporter)AssetImporter.GetAtPath(path);
        i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;
        i.alphaSource=alpha?TextureImporterAlphaSource.FromInput:TextureImporterAlphaSource.None;
        i.alphaIsTransparency=alpha;i.mipmapEnabled=false;i.isReadable=false;
        i.wrapMode=TextureWrapMode.Clamp;i.filterMode=FilterMode.Bilinear;i.maxTextureSize=max;
        i.textureCompression=TextureImporterCompression.CompressedHQ;
        foreach(var platform in new[]{"Android","iPhone"})
        {
            var s=i.GetPlatformTextureSettings(platform);s.name=platform;s.overridden=true;
            s.maxTextureSize=max;s.format=TextureImporterFormat.ASTC_6x6;i.SetPlatformTextureSettings(s);
        }
        i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
