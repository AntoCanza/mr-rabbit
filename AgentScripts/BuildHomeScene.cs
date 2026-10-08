using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;

public static class BuildHomeScene
{
    public static string Run()
    {
        const string scenePath = "Assets/Scenes/Home.unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
            throw new InvalidOperationException("Home already exists; inspect and edit it instead of rebuilding.");

        var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null) throw new InvalidOperationException("Import TMP essentials before building the scene.");
        const string housePath = "Assets/Art/Home/House.png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(housePath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.isReadable = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        foreach (string platform in new[] { "Android", "iPhone" })
        {
            var settings = importer.GetPlatformTextureSettings(platform);
            settings.name = platform;
            settings.overridden = true;
            settings.maxTextureSize = 2048;
            settings.format = TextureImporterFormat.ASTC_6x6;
            settings.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SetPlatformTextureSettings(settings);
        }
        importer.SaveAndReimport();
        var house = AssetDatabase.LoadAssetAtPath<Sprite>(housePath);
        if (house == null) throw new InvalidOperationException("House sprite was not imported.");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 0, -10);
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(255, 250, 237, 255);
        camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;

        var canvasObject = new GameObject("Home UI", typeof(RectTransform), typeof(Canvas),
            typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600, 1200);
        scaler.matchWidthOrHeight = .5f;

        var paper = Rect("Paper", canvas.transform);
        Stretch(paper);
        var paperImage = paper.gameObject.AddComponent<UnityEngine.UI.Image>();
        paperImage.color = camera.backgroundColor;
        paperImage.raycastTarget = false;

        var safeArea = Rect("Safe Area", canvas.transform);
        Stretch(safeArea);
        safeArea.gameObject.AddComponent<MrRabbit.SafeAreaLayout>();

        var title = Label("Title", safeArea, "Mr Rabbit", font, 44,
            new Color32(63, 88, 69, 255), TMPro.FontStyles.Bold);
        title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(.5f, 1);
        title.rectTransform.pivot = new Vector2(.5f, 1);
        title.rectTransform.sizeDelta = new Vector2(600, 72);
        title.rectTransform.anchoredPosition = new Vector2(0, -12);

        var viewport = Rect("House Viewport", safeArea);
        Stretch(viewport);
        viewport.offsetMin = new Vector2(40, 72);
        viewport.offsetMax = new Vector2(-40, -90);
        var board = Rect("House", viewport);
        board.anchorMin = board.anchorMax = new Vector2(.5f, .5f);
        var houseImage = board.gameObject.AddComponent<UnityEngine.UI.Image>();
        houseImage.sprite = house;
        houseImage.preserveAspect = true;
        houseImage.raycastTarget = false;
        var aspect = board.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();
        aspect.aspectMode = UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;
        aspect.aspectRatio = house.rect.width / house.rect.height;

        var rabbit = Rect("Mr Rabbit (Placeholder)", board);
        rabbit.anchorMin = new Vector2(.344f, .116f);
        rabbit.anchorMax = new Vector2(.436f, .286f);
        rabbit.pivot = new Vector2(.5f, 0);
        rabbit.sizeDelta = Vector2.zero;
        rabbit.gameObject.AddComponent<MrRabbit.RabbitGraphic>().raycastTarget = true;
        rabbit.gameObject.AddComponent<MrRabbit.DraggableRabbit>().Configure(board);

        var hint = Label("Touch Hint", safeArea, "Drag Mr Rabbit around his home", font,
            24, new Color32(114, 98, 75, 255), TMPro.FontStyles.Normal);
        hint.rectTransform.anchorMin = new Vector2(0, 0);
        hint.rectTransform.anchorMax = new Vector2(1, 0);
        hint.rectTransform.pivot = new Vector2(.5f, 0);
        hint.rectTransform.sizeDelta = new Vector2(-40, 52);
        hint.rectTransform.anchoredPosition = new Vector2(0, 12);

        var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
        EditorSettings.serializationMode = SerializationMode.ForceText;
        PlayerSettings.productName = "Mr Rabbit";
        PlayerSettings.companyName = "Mr Rabbit";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.mrrabbit.home");
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.mrrabbit.home");
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.defaultScreenWidth = 1440;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.runInBackground = false;
        int mobile = Array.FindIndex(QualitySettings.names, name => name.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) >= 0);
        QualitySettings.SetQualityLevel(mobile >= 0 ? mobile : 0, true);
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;
        Canvas.ForceUpdateCanvases();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = canvasObject;
        return scenePath + " | URP 2D | " + string.Join(", ", QualitySettings.names);
    }

    private static RectTransform Rect(string name, Transform parent)
    {
        var result = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        result.SetParent(parent, false);
        return result;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static TMPro.TextMeshProUGUI Label(string name, Transform parent, string text,
        TMPro.TMP_FontAsset font, float size, Color color, TMPro.FontStyles style)
    {
        var label = Rect(name, parent).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        label.text = text;
        label.font = font;
        label.fontSize = size;
        label.fontStyle = style;
        label.color = color;
        label.alignment = TMPro.TextAlignmentOptions.Center;
        label.raycastTarget = false;
        label.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
        return label;
    }
}
