using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class PolishCityTravel
{
    static void Place(RectTransform r,float x,float y,float right,float top){r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(right,top);r.offsetMin=r.offsetMax=Vector2.zero;}
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play");
        foreach(var s in UnityEngine.Object.FindObjectsByType<CitySymbol>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            var fit=s.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit!=null)UnityEngine.Object.DestroyImmediate(fit);
            var r=s.rectTransform;
            switch(s.name)
            {
                case "Garden":Place(r,.70f,.10f,.95f,.43f);break;
                case "Becomes":Place(r,.43f,.53f,.59f,.77f);break;
                case "Price Coin":Place(r,.26f,.025f,.43f,.15f);break;
                case "Departing Home":Place(r,.08f,.035f,.14f,.13f);break;
                case "Destination City":Place(r,.86f,.035f,.92f,.13f);break;
                case "Reward Coin":Place(r,.78f,.55f,.90f,.74f);break;
                case "Equipped":Place(r,.39f,.02f,.61f,.16f);break;
                case "Boutique Sign":Place(r,.46f,.82f,.56f,.97f);break;
                case "Coins":Place(r,.025f,.08f,.32f,.92f);break;
                default:Place(r,0,0,1,1);break;
            }
        }
        var c=UnityEngine.Object.FindFirstObjectByType<CityController>(FindObjectsInactive.Include);var canvas=c.transform.parent;
        var sign=canvas.Find("Meadow Viewport/Meadow/Scrolling Landscape/Home Sign");
        Place((RectTransform)sign,(1.86f-OutdoorAdventure.WorldLeft)/OutdoorAdventure.WorldWidth,.34f,(2.01f-OutdoorAdventure.WorldLeft)/OutdoorAdventure.WorldWidth,.46f);
        var symbol=sign.GetComponentInChildren<CitySymbol>(true);symbol.symbol=CitySymbol.Kind.Arrow;symbol.SetAllDirty();
        // The original kitchen's text labels stay hidden; number labels remain visible.
        foreach(var t in canvas.GetComponentsInChildren<TMPro.TMP_Text>(true))if(t.name=="Kitchen Hint")t.gameObject.SetActive(false);
        EditorSceneManager.MarkSceneDirty(c.gameObject.scene);EditorSceneManager.SaveScene(c.gameObject.scene);return "Corrected icon proportions and aligned home entrance with the painted door.";
    }
}
