using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class FinishCityTravel
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new System.Exception("Stop Play");
        AssetDatabase.Refresh();
        var c=UnityEngine.Object.FindFirstObjectByType<CityController>(FindObjectsInactive.Include);
        foreach(string name in new[]{"MrRabbitCitySeated","MissRabbitCitySeated"})
        {
            var path="Assets/Art/City/"+name+".png";var i=(TextureImporter)AssetImporter.GetAtPath(path);
            i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.alphaIsTransparency=true;i.mipmapEnabled=false;i.isReadable=false;i.maxTextureSize=1024;
            i.SetPlatformTextureSettings(new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=1024,format=TextureImporterFormat.ASTC_6x6});i.SaveAndReimport();
        }
        c.UI.maleCitySeated=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/City/MrRabbitCitySeated.png");c.UI.femaleCitySeated=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/City/MissRabbitCitySeated.png");
        PlayerSettings.bundleVersion="0.8.0";PlayerSettings.Android.bundleVersionCode=11;EditorUserBuildSettings.buildAppBundle=false;
        EditorUtility.SetDirty(c);EditorSceneManager.MarkSceneDirty(c.gameObject.scene);EditorSceneManager.SaveScene(c.gameObject.scene);AssetDatabase.SaveAssets();
        return "0.8.0 (11), seated outfit variants imported, ready for APK build.";
    }
}
