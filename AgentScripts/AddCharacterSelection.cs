using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddCharacterSelection
{
    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before editing the scene.");
        var safe = GameObject.Find("Safe Area").GetComponent<RectTransform>();
        if (safe.Find("Character Selection") != null) throw new InvalidOperationException("Selection already exists; edit it in place.");
        var home = safe.Find("House Viewport").gameObject;
        var hint = safe.Find("Touch Hint").gameObject;
        var player = home.GetComponentInChildren<MrRabbit.RabbitWalker>(true);
        var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        var male = Import("Assets/Art/Characters/MrRabbit.png");
        var female = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/MissRabbit.png");
        if (font == null || male == null || female == null || player == null)
            throw new InvalidOperationException("Home needs both character sprites, a font, and the player.");

        var panel = Rect("Character Selection", safe, Vector2.zero, Vector2.one);
        Text("Question", panel, "Who will play today?", font, 48,
            new Vector2(.1f, .82f), new Vector2(.9f, .91f), TMPro.FontStyles.Bold);
        var cards = Rect("Rabbit Choices", panel, new Vector2(.14f, .16f), new Vector2(.86f, .79f));
        var row = cards.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        row.spacing = 32;
        row.childControlWidth = row.childControlHeight = true;
        row.childForceExpandWidth = row.childForceExpandHeight = true;
        var maleButton = Card("Choose Mr Rabbit", cards, male, "Mr Rabbit", "Male", font,
            new Color32(230, 241, 235, 255));
        var femaleButton = Card("Choose Miss Rabbit", cards, female, "Miss Rabbit", "Female", font,
            new Color32(250, 234, 216, 255));
        maleButton.navigation = new UnityEngine.UI.Navigation
            { mode = UnityEngine.UI.Navigation.Mode.Explicit, selectOnRight = femaleButton };
        femaleButton.navigation = new UnityEngine.UI.Navigation
            { mode = UnityEngine.UI.Navigation.Mode.Explicit, selectOnLeft = maleButton };
        Text("Selection Hint", panel, "Tap a rabbit to enter the house", font, 26,
            new Vector2(.1f, .05f), new Vector2(.9f, .12f));

        var change = Rect("Choose Rabbit Button", safe, Vector2.one, Vector2.one);
        change.pivot = Vector2.one;
        change.sizeDelta = new Vector2(240, 60);
        change.anchoredPosition = new Vector2(-24, -12);
        var changeImage = change.gameObject.AddComponent<UnityEngine.UI.Image>();
        changeImage.color = new Color32(226, 237, 215, 255);
        var changeButton = change.gameObject.AddComponent<UnityEngine.UI.Button>();
        changeButton.targetGraphic = changeImage;
        changeButton.navigation = new UnityEngine.UI.Navigation { mode = UnityEngine.UI.Navigation.Mode.None };
        Text("Label", change, "Choose rabbit", font, 24, Vector2.zero, Vector2.one);

        var controller = safe.gameObject.AddComponent<MrRabbit.CharacterSelection>();
        controller.Configure(panel.gameObject, home, hint, player, male, female,
            maleButton, femaleButton, changeButton);
        player.gameObject.name = "Player Rabbit";
        home.SetActive(false);
        hint.SetActive(false);
        change.gameObject.SetActive(false);
        EditorUtility.SetDirty(controller);
        Canvas.ForceUpdateCanvases();
        var scene = safe.gameObject.scene;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        return "Home opens with both rabbits; choosing one starts play.";
    }

    private static Sprite Import(string path)
    {
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.isReadable = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 512;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        foreach (string platform in new[] { "Android", "iPhone" })
        {
            var settings = importer.GetPlatformTextureSettings(platform);
            settings.name = platform;
            settings.overridden = true;
            settings.maxTextureSize = 512;
            settings.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(settings);
        }
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static UnityEngine.UI.Button Card(string name, Transform parent, Sprite sprite,
        string characterName, string gender, TMPro.TMP_FontAsset font, Color normal)
    {
        var card = Rect(name, parent, Vector2.zero, Vector2.one);
        card.sizeDelta = new Vector2(400, 600);
        var border = card.gameObject.AddComponent<UnityEngine.UI.Image>();
        border.color = new Color32(99, 126, 105, 255);
        var inside = Rect("Card Face", card, Vector2.zero, Vector2.one);
        inside.offsetMin = new Vector2(4, 4);
        inside.offsetMax = new Vector2(-4, -4);
        var face = inside.gameObject.AddComponent<UnityEngine.UI.Image>();
        face.raycastTarget = false;
        var button = card.gameObject.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = face;
        var colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = new Color32(251, 246, 224, 255);
        colors.selectedColor = new Color32(249, 231, 184, 255);
        colors.pressedColor = new Color32(237, 214, 157, 255);
        colors.fadeDuration = .1f;
        button.colors = colors;
        var portrait = Rect("Portrait", inside, new Vector2(.07f, .26f), new Vector2(.93f, .95f));
        portrait.pivot = new Vector2(.5f, 0);
        var image = portrait.gameObject.AddComponent<UnityEngine.UI.Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        Text("Name", inside, characterName, font, 38, new Vector2(.05f, .14f), new Vector2(.95f, .24f), TMPro.FontStyles.Bold);
        Text("Gender", inside, gender, font, 26, new Vector2(.05f, .06f), new Vector2(.95f, .13f));
        return button;
    }

    private static RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return rect;
    }

    private static void Text(string name, Transform parent, string words, TMPro.TMP_FontAsset font,
        float size, Vector2 min, Vector2 max, TMPro.FontStyles style = TMPro.FontStyles.Normal)
    {
        var text = Rect(name, parent, min, max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = words;
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = new Color32(63, 88, 69, 255);
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        text.raycastTarget = false;
    }
}
