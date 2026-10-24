using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class PrepareArtUpdate
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        int count=0;
        foreach(var image in UnityEngine.Object.FindObjectsByType<RawImage>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            if(image.texture==null||(image.texture.name!="GardenAtlas"&&image.texture.name!="ActionIcons"))continue;
            var existing=image.GetComponent<GardenArtSlot>();
            if(existing!=null){existing.SetCell(existing.cell);EditorUtility.SetDirty(image);continue;}
            int cell=GardenArt.FindCell(image);
            image.gameObject.AddComponent<GardenArtSlot>().SetCell(cell);count++;
        }
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        return "Bound "+count+" atlas images to semantic cells before updating artwork.";
    }
}
