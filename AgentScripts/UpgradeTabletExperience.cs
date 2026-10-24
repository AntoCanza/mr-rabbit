using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class UpgradeTabletExperience
{
    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play first.");
        var safe = GameObject.Find("Safe Area").transform;
        var home = safe.Find("House Viewport") ?? safe.parent.Find("House Viewport");
        var outdoors = safe.Find("Meadow Viewport") ?? safe.parent.Find("Meadow Viewport");
        if (home == null || outdoors == null) throw new InvalidOperationException("Missing viewports.");
        var house = (RectTransform)home.Find("House");
        var meadow = (RectTransform)outdoors.Find("Meadow");
        var rabbit = house.GetComponentInChildren<RabbitWalker>(true);
        var selection = safe.Find("Character Selection");
        if (rabbit == null || selection == null) throw new InvalidOperationException("Missing player or selection.");

        foreach (var view in new[] { home, outdoors })
        {
            view.SetParent(safe.parent, false);
            view.SetSiblingIndex(safe.GetSiblingIndex());
            Stretch((RectTransform)view);
            if (view.GetComponent<UnityEngine.UI.RectMask2D>() == null)
                view.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        }
        Frame(house, new Rect(.13f, .07f, .74f, .64f));
        Frame(meadow, new Rect(0, -.05f, 1, 1));
        var title = safe.Find("Title");
        if (title != null) title.SetParent(selection, false);
        safe.Find("Touch Hint").gameObject.SetActive(false);
        safe.Find("Choose Rabbit Button").gameObject.SetActive(false);
        safe.Find("Time Of Day").gameObject.SetActive(false);
        house.Find("Exit Door/Exit Label").gameObject.SetActive(false);
        meadow.Find("Friends Hint").gameObject.SetActive(false);

        // Transparent hit target covers the painted bed, with a separate pose above it.
        var bed = house.Find("Bed") as RectTransform;
        if (bed == null) bed = Rect("Bed", house, new Vector2(.185f, .485f), new Vector2(.385f, .675f));
        var hit = bed.GetComponent<UnityEngine.UI.Image>() ?? bed.gameObject.AddComponent<UnityEngine.UI.Image>();
        hit.color = Color.clear;
        hit.raycastTarget = true;
        var pose = house.Find("Sleeping Rabbit") as RectTransform;
        if (pose == null) pose = Rect("Sleeping Rabbit", house, new Vector2(.251f,.484f), new Vector2(.331f,.654f));
        pose.pivot = new Vector2(.5f,.5f);
        pose.localRotation = Quaternion.Euler(0,0,90);
        var sleeping = pose.GetComponent<UnityEngine.UI.Image>() ?? pose.gameObject.AddComponent<UnityEngine.UI.Image>();
        sleeping.sprite = rabbit.GetComponent<RabbitGraphic>().sprite;
        sleeping.preserveAspect = true;
        sleeping.raycastTarget = false;
        pose.gameObject.SetActive(false);
        var mark = house.Find("Sleep Mark") as RectTransform;
        if (mark == null) mark = Rect("Sleep Mark", house, new Vector2(.21f,.65f),new Vector2(.29f,.72f));
        var text = mark.GetComponent<TMPro.TextMeshProUGUI>() ?? mark.gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        text.font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        text.text = "z Z";
        text.fontSize = 28;
        text.color = new Color32(73,64,94,255);
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.raycastTarget = false;
        mark.gameObject.SetActive(false);
        var interaction = bed.GetComponent<BedInteraction>() ?? bed.gameObject.AddComponent<BedInteraction>();
        interaction.Configure(rabbit,safe.GetComponent<OutdoorAdventure>(),sleeping,mark.gameObject);
        house.GetComponent<HousePointerInput>().SetBed(interaction);

        PlayerSettings.bundleVersion = "0.2.0";
        PlayerSettings.Android.bundleVersionCode = 2;
        Canvas.ForceUpdateCanvases();
        house.GetComponent<GameplayFraming>().Apply();
        meadow.GetComponent<GameplayFraming>().Apply();
        EditorSceneManager.MarkSceneDirty(safe.gameObject.scene);
        EditorSceneManager.SaveScene(safe.gameObject.scene);
        AssetDatabase.SaveAssets();
        return "Fullscreen framed play areas, startup-only character selection, painted bed interaction; version 0.2.0.";
    }

    private static void Frame(RectTransform area, Rect bounds)
    {
        var aspect = area.GetComponent<UnityEngine.UI.AspectRatioFitter>();
        if (aspect != null) aspect.enabled = false;
        var framing = area.GetComponent<GameplayFraming>() ?? area.gameObject.AddComponent<GameplayFraming>();
        framing.Configure(4f/3f,bounds);
    }
    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one;
        rect.offsetMin=rect.offsetMax=Vector2.zero;
    }
    private static RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max)
    {
        var rect=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent,false);rect.anchorMin=min;rect.anchorMax=max;
        rect.offsetMin=rect.offsetMax=Vector2.zero;
        return rect;
    }
}
