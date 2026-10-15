using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddDayNight
{
    public static string Run()
    {
        if(EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play before editing.");
        var safe=GameObject.Find("Safe Area").GetComponent<RectTransform>();
        if(safe.GetComponent<MrRabbit.DayNightCycle>()!=null) throw new InvalidOperationException("Day/night already exists; edit in place.");
        var house=safe.Find("House Viewport/House");
        var meadow=safe.Find("Meadow Viewport/Meadow");
        var painting=meadow.Find("Scrolling Landscape/Meadow Painting").GetComponent<UnityEngine.UI.Image>();
        var paper=safe.parent.Find("Paper").GetComponent<UnityEngine.UI.Image>();
        var title=safe.Find("Title").GetComponent<TMPro.TMP_Text>();
        var hint=safe.Find("Touch Hint").GetComponent<TMPro.TMP_Text>();
        var font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        var travel=safe.GetComponent<MrRabbit.OutdoorAdventure>();
        if(font==null || travel==null) throw new InvalidOperationException("Home font/adventure missing.");
        var homeSky=Rect("Home Sky",house).gameObject.AddComponent<MrRabbit.SkyCycleGraphic>();
        homeSky.Configure(true);homeSky.transform.SetAsFirstSibling();
        var lawnSky=Rect("Meadow Sky",meadow).gameObject.AddComponent<MrRabbit.SkyCycleGraphic>();
        lawnSky.Configure(false);lawnSky.transform.SetSiblingIndex(1);
        var badge=Rect("Time Of Day",safe);
        badge.anchorMin=badge.anchorMax=badge.pivot=new Vector2(0,1);
        badge.sizeDelta=new Vector2(240,60);badge.anchoredPosition=new Vector2(24,-12);
        var face=badge.gameObject.AddComponent<UnityEngine.UI.Image>();
        face.color=new Color32(246,236,208,255);face.raycastTarget=false;
        var label=Rect("Label",badge).gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        label.font=font;label.fontSize=28;label.fontStyle=TMPro.FontStyles.Bold;
        label.alignment=TMPro.TextAlignmentOptions.Center;label.color=new Color32(63,88,69,255);
        label.raycastTarget=false;label.text="Giorno";
        var clock=safe.gameObject.AddComponent<MrRabbit.DayNightCycle>();
        clock.Configure(travel,house.GetComponent<UnityEngine.UI.Image>(),painting,paper,
            homeSky,lawnSky,title,hint,label,badge.gameObject);
        clock.PreviewTime(.12f);
        // Keep the footwear modal above the badge as well as the existing game UI.
        safe.Find("Footwear Choice").SetAsLastSibling();
        EditorUtility.SetDirty(clock);
        EditorSceneManager.MarkSceneDirty(safe.gameObject.scene);
        EditorSceneManager.SaveScene(safe.gameObject.scene);AssetDatabase.SaveAssets();
        return "Shared 120-second day/night cycle, sky meshes, readable tints and Italian time label added.";
    }
    private static RectTransform Rect(string name,Transform parent)
    {
        var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(parent,false);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;
        r.offsetMin=r.offsetMax=Vector2.zero;return r;
    }
}
