using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class UnifyMeadow
{
    static Sprite Art(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Outdoors/"+name+".png");
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play");
        AssetDatabase.Refresh();
        foreach(var name in new[]{"SharedLandscape","CottagesCutout","SchoolCutout","GardenCutout","StationCutout"})
        {
            var path="Assets/Art/Outdoors/"+name+".png";var i=(TextureImporter)AssetImporter.GetAtPath(path);if(i==null)throw new Exception("Missing "+path);
            i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.alphaIsTransparency=true;i.mipmapEnabled=false;i.isReadable=false;i.maxTextureSize=2048;
            i.wrapModeU=name=="SharedLandscape"?TextureWrapMode.Mirror:TextureWrapMode.Clamp;i.wrapModeV=TextureWrapMode.Clamp;
            i.SetPlatformTextureSettings(new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6});i.SaveAndReimport();
        }
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);var c=a.GetComponent<CityController>();
        var world=a.transform.parent.Find("Meadow Viewport/Meadow/Scrolling Landscape");
        if(world.Find("One Shared Landscape")!=null)throw new Exception("Shared landscape already installed; edit in place");
        var rect=new GameObject("One Shared Landscape",typeof(RectTransform)).GetComponent<RectTransform>();rect.SetParent(world,false);
        rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;rect.SetAsFirstSibling();
        var painting=rect.gameObject.AddComponent<UnityEngine.UI.RawImage>();painting.texture=Art("SharedLandscape").texture;painting.uvRect=new Rect(0,0,OutdoorAdventure.WorldWidth/1.8f,1);painting.raycastTarget=true;
        a.gameObject.AddComponent<SharedMeadowLandscape>().Configure(a.GetComponent<DayNightCycle>(),painting);
        world.Find("Meadow Painting").GetComponent<UnityEngine.UI.Image>().sprite=Art("CottagesCutout");
        world.Find("Distant Garden").GetComponent<UnityEngine.UI.Image>().sprite=Art("GardenCutout");
        world.Find("School Meadow").GetComponent<UnityEngine.UI.Image>().sprite=Art("SchoolCutout");
        c.UI.countryPainting.sprite=Art("StationCutout");
        var gardenPainting=(RectTransform)world.Find("Distant Garden");var min=gardenPainting.anchorMin;var max=gardenPainting.anchorMax;float width=max.x-min.x;
        gardenPainting.anchorMin=new Vector2(min.x+width*.134f,.17f);gardenPainting.anchorMax=new Vector2(min.x+width*(.134f+.63f),.80f);gardenPainting.offsetMin=gardenPainting.offsetMax=Vector2.zero;
        var station=c.UI.countryPainting.rectTransform;var stationMin=station.anchorMin;var stationWidth=station.anchorMax.x-stationMin.x;
        station.anchorMin=new Vector2(stationMin.x+stationWidth*.11f,.22f);station.anchorMax=new Vector2(stationMin.x+stationWidth*.89f,.94f);station.offsetMin=station.offsetMax=Vector2.zero;
        // Transparent landmarks need no background feathering; their original ground is gone.
        foreach(string name in new[]{"Distant Garden","School Meadow"})
        {var blend=world.Find(name).GetComponent<MeadowBlend>();if(blend!=null)blend.enabled=false;}
        c.UI.travellingScenery.Find("Passing Village").GetComponent<UnityEngine.UI.Image>().sprite=Art("SharedLandscape");
        var map=c.UI.map.transform;map.Find("Village Destination/Village Pictures/Two Cottages").GetComponent<UnityEngine.UI.Image>().sprite=Art("CottagesCutout");
        map.Find("Village Destination/Village Pictures/School").GetComponent<UnityEngine.UI.Image>().sprite=Art("SchoolCutout");
        var garden=a.GetComponent<GardenController>();var ui=(GardenController.Widgets)typeof(GardenController).GetField("ui",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(garden);
        var gateHint=(RectTransform)ui.gate.transform.Find("Action Hint");gateHint.anchorMin=gateHint.anchorMax=new Vector2(.5f,.20f);gateHint.anchoredPosition=Vector2.zero;gateHint.sizeDelta=new Vector2(130,130);
        var gateGlow=Glow(gateHint);gateHint.GetComponent<ActionCue>().Configure((RectTransform)gateHint.Find("Symbol"),gateGlow,true);
        ui.notice.transform.SetParent(ui.gate.transform,false);var notice=(RectTransform)ui.notice.transform;notice.anchorMin=notice.anchorMax=new Vector2(.5f,.20f);notice.anchoredPosition=Vector2.zero;notice.sizeDelta=new Vector2(130,130);
        ui.notice.GetComponent<UnityEngine.UI.Image>().color=Color.clear;ui.noticeText.text="";ui.noticeText.gameObject.SetActive(false);
        var icon=(RectTransform)ui.noticeIcon.transform.parent;icon.anchorMin=new Vector2(.05f,.05f);icon.anchorMax=new Vector2(.95f,.95f);icon.offsetMin=icon.offsetMax=Vector2.zero;
        var secondary=(RectTransform)ui.secondaryNoticeIcon.transform.parent;secondary.anchorMin=new Vector2(.65f,-.05f);secondary.anchorMax=new Vector2(1.05f,.4f);secondary.offsetMin=secondary.offsetMax=Vector2.zero;
        ui.notice.GetComponent<ActionCue>().Configure(icon,Glow(notice),true);
        EditorSceneManager.MarkSceneDirty(a.gameObject.scene);EditorSceneManager.SaveScene(a.gameObject.scene);AssetDatabase.SaveAssets();
        return "One mirrored, continuous sky/hills/grass/path texture across all 6.5 viewport widths; all four landmarks use transparent cutouts. Existing movement and door positions preserved.";
    }
    static UnityEngine.UI.Image Glow(Transform parent)
    {
        var r=new GameObject("Available Action Glow",typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=new Vector2(-.05f,-.05f);r.anchorMax=new Vector2(1.05f,1.05f);r.offsetMin=r.offsetMax=Vector2.zero;r.SetAsFirstSibling();
        var image=r.gameObject.AddComponent<UnityEngine.UI.Image>();image.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");image.preserveAspect=true;image.raycastTarget=false;return image;
    }
}
