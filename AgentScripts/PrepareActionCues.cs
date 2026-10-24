using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class PrepareActionCues
{
    static int count;
    static void Add(Transform owner,Transform picture,bool controlled)
    {
        if(owner==null||picture==null)throw new Exception("Missing action picture");
        var cue=owner.GetComponent<ActionCue>();
        if(cue==null)cue=owner.gameObject.AddComponent<ActionCue>();
        UnityEngine.UI.Image halo=null;
        if(controlled)
        {
            var existing=owner.Find("Action Glow");
            var r=existing as RectTransform;
            if(r==null){r=new GameObject("Action Glow",typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(owner,false);}
            r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(-5,-5);r.offsetMax=new Vector2(5,5);r.SetAsFirstSibling();
            halo=r.GetComponent<UnityEngine.UI.Image>();if(halo==null)halo=r.gameObject.AddComponent<UnityEngine.UI.Image>();
            halo.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            halo.type=UnityEngine.UI.Image.Type.Sliced;halo.raycastTarget=false;halo.enabled=false;
        }
        cue.Configure((RectTransform)picture,halo,controlled);EditorUtility.SetDirty(cue);count++;
    }
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first");
        count=0;
        var garden=UnityEngine.Object.FindFirstObjectByType<GardenController>(FindObjectsInactive.Include);
        var ui=(GardenController.Widgets)typeof(GardenController).GetField("ui",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(garden);
        foreach(var rect in UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            if(rect.name=="Action Hint"&&!rect.IsChildOf(ui.view.transform))Add(rect,rect.Find("Symbol"),false);
        for(int i=0;i<4;i++)
        {
            Add(ui.tools[i].transform,ui.tools[i].transform.Find("Tool"),true);
            Add(ui.seedChoices[i].transform,ui.seedChoices[i].transform.Find("Crop"),true);
        }
        foreach(var bed in ui.beds)Add(bed.transform,bed.transform.Find("Action Hint"),true);
        Add(ui.exit.transform,ui.exit.transform.Find("Back"),true);
        Add(ui.cancelSeeds.transform,ui.cancelSeeds.transform.Find("Back Icon"),true);
        Add(ui.notice.transform,ui.noticeIcon.transform.parent,false);
        if(ui.arrow!=null)ui.arrow.gameObject.SetActive(false);
        EditorUtility.SetDirty(garden);
        var scene=garden.gameObject.scene;EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        return "Installed "+count+" picture cues; garden uses exclusive guided highlights, fixed touch areas, no guiding arrow.";
    }
}
