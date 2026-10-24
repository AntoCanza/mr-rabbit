using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
public static class PrepareTabletRelease
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play before preparing release.");
        const string path="Assets/Art/Characters/MrRabbitIcon.png";
        AssetDatabase.ImportAsset(path);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType=TextureImporterType.Default;importer.maxTextureSize=1024;
        importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        var icon=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        PlayerSettings.SetIcons(NamedBuildTarget.Unknown,new[]{icon},IconKind.Any);
        foreach(var kind in PlayerSettings.GetSupportedIconKinds(NamedBuildTarget.Android))
        {
            // Android wraps the legacy/round artwork safely when no layered icon is supplied.
            if(kind.ToString().Contains("Adaptive"))continue;
            var slots=PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android,kind);
            foreach(var slot in slots)slot.SetTexture(icon);
            PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android,kind,slots);
        }
        var safe=GameObject.Find("Safe Area");
        var world=safe.transform.parent.Find("Meadow Viewport/Meadow/Scrolling Landscape");
        foreach(string name in new[]{"Coccodella","Zebrella"})
        {
            var plate=(RectTransform)world.Find("Casa di "+name+"/Nameplate");
            plate.anchorMin=new Vector2(.23f,.08f);plate.anchorMax=new Vector2(.77f,.18f);
            plate.offsetMin=plate.offsetMax=Vector2.zero;
        }
        PlayerSettings.bundleVersion="0.7.3";PlayerSettings.Android.bundleVersionCode=10;
        EditorUserBuildSettings.buildAppBundle=false;
        EditorSceneManager.MarkSceneDirty(safe.scene);EditorSceneManager.SaveScene(safe.scene);
        AssetDatabase.SaveAssets();
        return "0.7.3 (10), audible TV and school feedback, four-attempt help, continuous held movement and finger handwriting, saved scene, APK output.";
    }
}

