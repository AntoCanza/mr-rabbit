using System;
using UnityEditor;
using UnityEngine;

public static class PrepareGooglePlay
{
    public static string Run()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Arrestare Play prima della preparazione.");
        if (AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/InformativaPrivacy.txt") == null)
            throw new InvalidOperationException("Informativa privacy mancante.");
        if (!System.IO.File.Exists("Assets/Plugins/Android/LauncherManifest.xml"))
            throw new InvalidOperationException("Manifesto Android offline mancante.");
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
        var manifest = settings.FindProperty("useCustomLauncherManifest");
        if (manifest == null) throw new InvalidOperationException("Impostazione del manifesto non disponibile.");
        manifest.boolValue = true;
        settings.ApplyModifiedProperties();
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;
        PlayerSettings.Android.forceInternetPermission = false;
        PlayerSettings.bundleVersion = "0.10.1";
        PlayerSettings.Android.bundleVersionCode = 18;
        AssetDatabase.SaveAssets();
        return "0.10.1 / codice 18; API 36; manifesto offline con backup disabilitato. Firma di caricamento ancora da configurare.";
    }
}
