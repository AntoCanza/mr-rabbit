using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class AddWordPictures
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        string[][] words={
            new[]{"CASA","SOLE","LUNA","MARE","MELA","PERA","PANE","MANO","NASO","RANA","GATTO","PESCE"},
            new[]{"FIORE","TRENO","UVA","LUPO","MURO","NIDO","VISO","DITO","APE","ERBA","ORSO","ISOLA"}
        };
        var entries=new List<SchoolWordPicture.Entry>();
        // Tight measured bounds preserve the complete drawings despite irregular atlas spacing.
        Rect[][] bounds={
            new[]{new Rect(30,60,340,320),new Rect(395,55,330,325),new Rect(750,70,300,300),new Rect(1070,105,365,285),
                new Rect(50,400,280,310),new Rect(420,400,275,310),new Rect(735,450,340,245),new Rect(1100,395,310,315),
                new Rect(55,735,280,300),new Rect(380,730,330,310),new Rect(735,700,335,350),new Rect(1080,755,350,290)},
            new[]{new Rect(30,30,275,350),new Rect(360,85,375,280),new Rect(765,85,300,290),new Rect(1070,30,370,355),
                new Rect(25,425,325,280),new Rect(365,430,360,280),new Rect(740,395,310,315),new Rect(1145,380,260,340),
                new Rect(15,740,325,310),new Rect(360,750,350,295),new Rect(735,710,340,330),new Rect(1090,725,350,345)}
        };
        for(int page=0;page<2;page++)
        {
            string path="Assets/Art/School/WordPictures"+(page+1)+".png";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null)throw new InvalidOperationException("Missing atlas "+path);
            importer.textureType=TextureImporterType.Default;importer.maxTextureSize=2048;
            importer.alphaIsTransparency=true;importer.mipmapEnabled=false;
            importer.textureCompression=TextureImporterCompression.CompressedHQ;importer.SaveAndReimport();
            var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            for(int i=0;i<12;i++)
            {
                var b=bounds[page][i];
                entries.Add(new SchoolWordPicture.Entry{word=words[page][i],atlas=atlas,uv=new Rect(b.x/1448f,1-b.yMax/1086f,b.width/1448f,b.height/1086f)});
            }
        }
        var school=UnityEngine.Object.FindFirstObjectByType<SchoolController>(FindObjectsInactive.Include);
        var sheet=school.UI.worksheet.transform;
        var holder=sheet.Find("Word Picture");
        if(holder==null)
        {
            holder=new GameObject("Word Picture",typeof(RectTransform),typeof(SchoolWordPicture)).transform;
            holder.SetParent(sheet,false);
            var art=new GameObject("Illustration",typeof(RectTransform),typeof(CanvasRenderer),typeof(UnityEngine.UI.RawImage),typeof(UnityEngine.UI.AspectRatioFitter));
            art.transform.SetParent(holder,false);
        }
        var r=(RectTransform)holder;r.anchorMin=new Vector2(.11f,.24f);r.anchorMax=new Vector2(.45f,.73f);r.offsetMin=r.offsetMax=Vector2.zero;
        var image=holder.GetComponentInChildren<UnityEngine.UI.RawImage>(true);
        var fit=image.GetComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;
        image.color=Color.white;image.raycastTarget=false;
        var picture=holder.GetComponent<SchoolWordPicture>();picture.Configure(image,entries.ToArray());picture.Show(null);
        school.UI.wordPicture=picture;EditorUtility.SetDirty(school);
        PlayerSettings.bundleVersion="0.7.2";PlayerSettings.Android.bundleVersionCode=9;
        EditorSceneManager.MarkSceneDirty(school.gameObject.scene);EditorSceneManager.SaveScene(school.gameObject.scene);AssetDatabase.SaveAssets();
        return "24 semantic word pictures, preserved writing area and math layout; 0.7.2 (9)";
    }
}
