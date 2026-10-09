using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class UseMissRabbit
{
    public static string Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before changing the saved character.");
        const string path = "Assets/Art/Characters/MissRabbit.png";
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
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
            settings.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SetPlatformTextureSettings(settings);
        }
        importer.SaveAndReimport();
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        var walker = UnityEngine.Object.FindFirstObjectByType<MrRabbit.RabbitWalker>();
        if (sprite == null || walker == null) throw new InvalidOperationException("The sprite or Home character could not be found.");
        var graphic = walker.GetComponent<MrRabbit.RabbitGraphic>();
        graphic.sprite = sprite;
        graphic.type = UnityEngine.UI.Image.Type.Simple;
        graphic.preserveAspect = true;
        graphic.raycastTarget = true;
        graphic.color = Color.white;
        walker.gameObject.name = "Miss Rabbit";
        EditorUtility.SetDirty(graphic);
        EditorSceneManager.MarkSceneDirty(walker.gameObject.scene);
        EditorSceneManager.SaveScene(walker.gameObject.scene);
        AssetDatabase.SaveAssets();
        return "Miss Rabbit sprite assigned; walking paths retained.";
    }
}
