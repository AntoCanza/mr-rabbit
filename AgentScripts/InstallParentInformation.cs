using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class InstallParentInformation
{
    public static string Run()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Arrestare Play prima di modificare la scena.");
        var selection = UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        if (selection == null) throw new InvalidOperationException("Aprire la scena Home.");
        if (selection.GetComponent<ParentInformation>() != null) return "Informativa per i genitori già installata.";
        var source = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/InformativaPrivacy.txt");
        var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        var parent = (GameObject)typeof(CharacterSelection).GetField("selectionPanel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(selection);
        if (source == null || font == null || parent == null) throw new InvalidOperationException("Mancano testo, font o pannello di selezione.");
        var open = Button("Parent Information", parent.transform, "Per i genitori · Privacy", font, new Vector2(.32f,.025f), new Vector2(.68f,.11f));
        var layer = Rect("Privacy Layer", selection.transform, Vector2.zero, Vector2.one);
        var canvas = layer.gameObject.AddComponent<Canvas>(); canvas.overrideSorting = true; canvas.sortingOrder = 50;
        layer.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        layer.gameObject.AddComponent<UnityEngine.UI.Image>().color = new Color(0,0,0,.55f);
        var paper = Rect("Privacy Paper", layer, new Vector2(.055f,.055f), new Vector2(.945f,.945f));
        paper.gameObject.AddComponent<UnityEngine.UI.Image>().color = new Color32(255,250,237,255);
        var heading = Text("Privacy Title", paper, font, 42, new Vector2(.06f,.82f), new Vector2(.94f,.95f));
        heading.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
        var body = Text("Privacy Text", paper, font, 31, new Vector2(.06f,.20f), new Vector2(.94f,.79f));
        body.alignment = TMPro.TextAlignmentOptions.TopLeft;
        body.enableAutoSizing = true; body.fontSizeMin = 25; body.fontSizeMax = 31;
        var previous = Button("Privacy Previous", paper, "Indietro", font, new Vector2(.06f,.055f), new Vector2(.24f,.15f));
        var count = Text("Privacy Page", paper, font, 27, new Vector2(.25f,.055f), new Vector2(.37f,.15f));
        var next = Button("Privacy Next", paper, "Avanti", font, new Vector2(.39f,.055f), new Vector2(.57f,.15f));
        var close = Button("Privacy Close", paper, "Chiudi", font, new Vector2(.74f,.055f), new Vector2(.94f,.15f));
        selection.gameObject.AddComponent<ParentInformation>().Configure(source, layer.gameObject, open, close, previous, next, heading, body, count);
        layer.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(selection.gameObject.scene);
        EditorSceneManager.SaveScene(selection.gameObject.scene);
        AssetDatabase.SaveAssets();
        return "Informativa offline installata nella scelta del personaggio.";
    }

    static RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false); rect.anchorMin = min; rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return rect;
    }

    static TMPro.TextMeshProUGUI Text(string name, Transform parent, TMPro.TMP_FontAsset font, float size, Vector2 min, Vector2 max)
    {
        var text = Rect(name, parent, min, max).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        text.font = font; text.fontSize = size; text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = new Color32(63,88,69,255); text.raycastTarget = false;
        text.textWrappingMode = TMPro.TextWrappingModes.Normal;
        return text;
    }

    static UnityEngine.UI.Button Button(string name, Transform parent, string label, TMPro.TMP_FontAsset font, Vector2 min, Vector2 max)
    {
        var rect = Rect(name, parent, min, max);
        var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color = new Color32(222,235,217,255);
        var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = image;
        var navigation = button.navigation; navigation.mode = UnityEngine.UI.Navigation.Mode.None; button.navigation = navigation;
        var text = Text("Label", rect, font, 29, new Vector2(.03f,.04f), new Vector2(.97f,.96f));
        text.text = label; text.enableAutoSizing = true; text.fontSizeMin = 21; text.fontSizeMax = 29;
        return button;
    }
}
