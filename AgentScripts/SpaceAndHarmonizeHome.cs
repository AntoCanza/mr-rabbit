using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class SpaceAndHarmonizeHome
{
    public const string HousePath="Assets/Art/City/HomeExteriorHarmonized.png";
    static float World(float x)=>(x-OutdoorAdventure.WorldLeft)/OutdoorAdventure.WorldWidth;
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        var meadow=a.transform.parent.Find("Meadow Viewport/Meadow");
        var world=(RectTransform)meadow.Find("Scrolling Landscape");var friends=(RectTransform)meadow.Find("Meadow Friends");
        float previousWidth=world.anchorMax.x-world.anchorMin.x;
        bool expand=Mathf.Abs(previousWidth-OutdoorAdventure.WorldWidth)>.001f;
        if(expand&&Mathf.Abs(previousWidth-6.5f)>.001f)throw new InvalidOperationException("Unexpected world width "+previousWidth);
        if(expand)
        {
            foreach(var layer in new[]{world,friends})
            {
                for(int i=0;i<layer.childCount;i++)
                {
                    var r=(RectTransform)layer.GetChild(i);
                    if(r.name=="One Shared Landscape")continue;
                    var min=r.anchorMin;var max=r.anchorMax;
                    min.x=min.x*previousWidth/OutdoorAdventure.WorldWidth;max.x=max.x*previousWidth/OutdoorAdventure.WorldWidth;
                    r.anchorMin=min;r.anchorMax=max;
                }
                layer.anchorMin=new Vector2(OutdoorAdventure.WorldLeft,0);layer.anchorMax=new Vector2(OutdoorAdventure.WorldLeft+OutdoorAdventure.WorldWidth,1);layer.offsetMin=layer.offsetMax=Vector2.zero;
            }
            var post=(RectTransform)world.Find("Signpost");var lo=post.anchorMin;var hi=post.anchorMax;
            lo.x+=.8f/OutdoorAdventure.WorldWidth;hi.x+=.8f/OutdoorAdventure.WorldWidth;post.anchorMin=lo;post.anchorMax=hi;
        }
        var grass=world.Find("One Shared Landscape").GetComponent<RawImage>();grass.uvRect=new Rect(0,0,OutdoorAdventure.WorldWidth/1.8f,1);grass.raycastTarget=true;
        var c=a.GetComponent<CityController>();var house=c.UI.homeExterior.rectTransform;
        house.anchorMin=new Vector2(World(2.53f),.26f);house.anchorMax=new Vector2(World(3.05f),.76f);house.offsetMin=house.offsetMax=Vector2.zero;
        var door=(RectTransform)world.Find("Home Sign");door.anchorMin=new Vector2(World(2.49f),.32f);door.anchorMax=new Vector2(World(2.62f),.44f);door.offsetMin=door.offsetMax=Vector2.zero;
        if(door.GetComponent<WorldActionLayer>()==null)door.gameObject.AddComponent<WorldActionLayer>();
        foreach(var symbol in door.GetComponentsInChildren<CitySymbol>(true))symbol.enabled=false;
        var painted=door.Find("Painted Entrance");
        if(painted==null)
        {
            var rect=new GameObject("Painted Entrance",typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(door,false);
            rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
            var image=rect.gameObject.AddComponent<RawImage>();image.texture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/ActionIcons.png");image.raycastTarget=false;
            var fit=rect.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;GardenArt.Apply(image,1);
        }
        if(System.IO.File.Exists(HousePath))
        {
            AssetDatabase.ImportAsset(HousePath);
            var importer=(TextureImporter)AssetImporter.GetAtPath(HousePath);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.isReadable=false;importer.maxTextureSize=2048;
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6});importer.SaveAndReimport();
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(HousePath);c.UI.homeExterior.sprite=sprite;
            c.UI.map.transform.Find("Village Destination/Village Pictures/Home").GetComponent<Image>().sprite=sprite;
        }
        PlayerSettings.bundleVersion="0.8.4";PlayerSettings.Android.bundleVersionCode=15;
        EditorUtility.SetDirty(c);EditorSceneManager.MarkSceneDirty(a.gameObject.scene);EditorSceneManager.SaveScene(a.gameObject.scene);AssetDatabase.SaveAssets();
        return "Home moved +0.8 viewport; neighboring buildings and door coordinates preserved; repeating countryside extended; matching house/map artwork assigned when available; 0.8.4 (15).";
    }
}
