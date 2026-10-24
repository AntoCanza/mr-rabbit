using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MrRabbit;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public static class VerifyHomeExterior
{
    public static async Task<string> Run()
    {
        await Task.Delay(300);var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();var s=a.GetComponent<CharacterSelection>();
        s.ShowSelection();s.ChooseMale();a.GetComponent<DayNightCycle>().PreviewTime(.12f);
        var w=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();a.WalkOutOfHome();for(int i=0;i<2000;i++)w.Advance(Vector2.zero,.02f);a.ChooseShoes();
        await Task.Delay(200);Canvas.ForceUpdateCanvases();
        var button=(Button)typeof(OutdoorAdventure).GetField("homeSign",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(a);var r=(RectTransform)button.transform;
        var point=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));
        var e=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
        if(hits.Count==0||ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)!=button.gameObject)throw new Exception("House entry icon is covered");
        var h=a.GetComponent<CityController>().UI.homeExterior;
        if(h.sprite.name!="HomeExteriorHarmonized")throw new Exception("House palette asset missing");
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"Captures","Release-"+Application.version,Screen.width+"x"+Screen.height);Directory.CreateDirectory(folder);
        await Awaitable.EndOfFrameAsync();var picture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,"11-meadow-home.png"),picture.EncodeToPNG());UnityEngine.Object.Destroy(picture);
        ExecuteEvents.Execute(button.gameObject,e,ExecuteEvents.pointerClickHandler);for(int i=0;i<1000;i++)a.Advance(0,.02f);
        if(a.Location!=OutdoorAdventure.Place.Home)throw new Exception("House entry tap failed");
        return Screen.width+"x"+Screen.height+": matching house art, visible entrance icon and actual tap-to-enter passed.";
    }
}
