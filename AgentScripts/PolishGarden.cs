using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PolishGarden
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        int fixedCount=0;
        foreach(var graphic in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Graphic>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            if(graphic.GetComponent<CanvasRenderer>()==null){graphic.gameObject.AddComponent<CanvasRenderer>();fixedCount++;}
        foreach(var image in UnityEngine.Object.FindObjectsByType<UnityEngine.UI.RawImage>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            if(image.texture==null||(image.texture.name!="GardenAtlas"&&image.texture.name!="ActionIcons"))continue;
            if(Mathf.Abs(image.uvRect.width-.25f)>.0001f)continue;
            int index=Mathf.RoundToInt(image.uvRect.x*4)+(3-Mathf.RoundToInt(image.uvRect.y*4))*4;
            MrRabbit.GardenArt.Apply(image,index);
        }
        var safe=GameObject.Find("Safe Area");
        var controller=safe.GetComponent<MrRabbit.GardenController>();
        var widgets=(MrRabbit.GardenController.Widgets)typeof(MrRabbit.GardenController).GetField("ui",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(controller);
        widgets.approach.raycastTarget=true;
        widgets.approach.rectTransform.anchorMax=new Vector2(1.41f/3.5f,1);
        widgets.approach.transform.SetSiblingIndex(widgets.approach.transform.parent.Find("Meadow Painting").GetSiblingIndex()+1);
        if(widgets.approach.GetComponent<MrRabbit.MeadowBlend>()==null)widgets.approach.gameObject.AddComponent<MrRabbit.MeadowBlend>();
        widgets.seedPanel.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;
        var gateBadge=(RectTransform)widgets.gate.transform.GetChild(0);
        gateBadge.anchorMin=gateBadge.anchorMax=new Vector2(.5f,1.05f);
        var iconSlot=(RectTransform)widgets.noticeIcon.transform.parent;
        iconSlot.anchorMin=new Vector2(.02f,.03f);iconSlot.anchorMax=new Vector2(.16f,.97f);
        if(widgets.secondaryNoticeIcon==null)
        {
            var second=UnityEngine.Object.Instantiate(iconSlot,widgets.notice.transform);
            second.name="Water Too";second.anchorMin=new Vector2(.17f,.03f);second.anchorMax=new Vector2(.31f,.97f);
            widgets.secondaryNoticeIcon=second.GetComponentInChildren<UnityEngine.UI.RawImage>(true);
            MrRabbit.GardenArt.Apply(widgets.secondaryNoticeIcon,5);
        }
        widgets.noticeText.rectTransform.anchorMin=new Vector2(.33f,.08f);
        EditorUtility.SetDirty(controller);
        var house=safe.transform.parent.Find("House Viewport/House");
        var sleep=(RectTransform)house.Find("Bed/Action Hint");
        sleep.anchorMin=sleep.anchorMax=new Vector2(.72f,.30f);sleep.sizeDelta=new Vector2(90,90);
        ((RectTransform)house.Find("Exit Door/Action Hint")).sizeDelta=new Vector2(80,80);
        const string soilPath="Assets/Art/Garden/SoilBed.png";
        AssetDatabase.ImportAsset(soilPath);
        var importer=(TextureImporter)AssetImporter.GetAtPath(soilPath);
        importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
        importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.isReadable=false;importer.maxTextureSize=1024;
        var platform=importer.GetPlatformTextureSettings("Android");platform.name="Android";platform.overridden=true;platform.maxTextureSize=1024;platform.format=TextureImporterFormat.ASTC_6x6;importer.SetPlatformTextureSettings(platform);importer.SaveAndReimport();
        var soilSprite=AssetDatabase.LoadAssetAtPath<Sprite>(soilPath);
        foreach(var soil in widgets.soil)
        {
            var painting=soil.transform.parent.Find("Painted Soil") as RectTransform;
            if(painting==null)
            {
                painting=new GameObject("Painted Soil",typeof(RectTransform),typeof(UnityEngine.UI.Image)).GetComponent<RectTransform>();
                painting.SetParent(soil.transform.parent,false);painting.SetAsFirstSibling();
            }
            painting.anchorMin=new Vector2(-.01f,-.025f);painting.anchorMax=new Vector2(1.01f,.68f);painting.offsetMin=painting.offsetMax=Vector2.zero;
            var picture=painting.GetComponent<UnityEngine.UI.Image>();picture.sprite=soilSprite;picture.raycastTarget=false;
            soil.SetPainting(picture);EditorUtility.SetDirty(soil);
        }
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        return "Added missing renderers: "+fixedCount;
    }
}
