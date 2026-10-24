using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class PrepareHoldMovement
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        var input=UnityEngine.Object.FindFirstObjectByType<OutdoorPointerInput>(FindObjectsInactive.Include);
        var adventure=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        var rabbit=adventure.PlayerRect.GetComponent<RabbitGraphic>();
        rabbit.raycastTarget=true;
        var arrow=input.transform.Find("Movement Direction");
        if(arrow==null)
        {
            arrow=new GameObject("Movement Direction",typeof(RectTransform),typeof(CanvasRenderer),typeof(MovementDirectionGraphic)).transform;
            arrow.SetParent(input.transform,false);
        }
        var graphic=arrow.GetComponent<MovementDirectionGraphic>();
        graphic.raycastTarget=false;graphic.enabled=false;input.SetArrow(graphic);
        PlayerSettings.bundleVersion="0.7.1";PlayerSettings.Android.bundleVersionCode=8;
        EditorSceneManager.MarkSceneDirty(input.gameObject.scene);EditorSceneManager.SaveScene(input.gameObject.scene);AssetDatabase.SaveAssets();
        return "Character captures holds; subtle direction pictogram configured; 0.7.1 (8)";
    }
}
