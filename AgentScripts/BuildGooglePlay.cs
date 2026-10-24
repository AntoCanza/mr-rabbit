using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildGooglePlay
{
    public static string Run()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Arrestare Play prima di compilare.");
        string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codex", "secrets", "mr-rabbit");
        string key = Path.Combine(directory, "google-play-upload.keystore");
        string protectedPassword = Path.Combine(directory, "upload-password.dpapi");
        if (!File.Exists(key) || !File.Exists(protectedPassword)) throw new InvalidOperationException("Chiave di caricamento o password protetta non disponibili su questo PC.");
        string encoded = File.ReadAllText(protectedPassword).Trim();
        byte[] encrypted = Enumerable.Range(0, encoded.Length / 2).Select(i => Convert.ToByte(encoded.Substring(i * 2, 2), 16)).ToArray();
        var security = Assembly.Load("System.Security");
        var protection = security.GetType("System.Security.Cryptography.ProtectedData", true);
        var scope = security.GetType("System.Security.Cryptography.DataProtectionScope", true);
        byte[] clear = (byte[])protection.GetMethod("Unprotect", new[] { typeof(byte[]), typeof(byte[]), scope })
            .Invoke(null, new object[] { encrypted, null, Enum.Parse(scope, "CurrentUser") });
        string password = Encoding.Unicode.GetString(clear);
        Array.Clear(clear, 0, clear.Length);
        bool custom = PlayerSettings.Android.useCustomKeystore;
        string oldKey = PlayerSettings.Android.keystoreName, oldAlias = PlayerSettings.Android.keyaliasName;
        string oldPassword = PlayerSettings.Android.keystorePass, oldAliasPassword = PlayerSettings.Android.keyaliasPass;
        bool bundle = EditorUserBuildSettings.buildAppBundle, development = EditorUserBuildSettings.development;
        string output = Path.Combine("Builds", "Android", "MrRabbit-" + PlayerSettings.bundleVersion + ".aab");
        try
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = key;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = "mrrabbit-upload";
            PlayerSettings.Android.keyaliasPass = password;
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.development = false;
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.DetailedBuildReport
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Compilazione Google Play non riuscita: " + report.summary.result + "; errori: " + report.summary.totalErrors);
            return output + "; byte: " + new FileInfo(output).Length + "; errori: " + report.summary.totalErrors + "; avvisi: " + report.summary.totalWarnings;
        }
        finally
        {
            PlayerSettings.Android.useCustomKeystore = custom;
            PlayerSettings.Android.keystoreName = oldKey;
            PlayerSettings.Android.keyaliasName = oldAlias;
            PlayerSettings.Android.keystorePass = oldPassword;
            PlayerSettings.Android.keyaliasPass = oldAliasPassword;
            EditorUserBuildSettings.buildAppBundle = bundle;
            EditorUserBuildSettings.development = development;
            password = null;
            AssetDatabase.SaveAssets();
        }
    }
}
