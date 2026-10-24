using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using MrRabbit;
public static class VerifyCityTouch
{
    static void Check(bool b,string message){if(!b)throw new Exception(message);}
    static async Task Tap(UnityEngine.UI.Button b)
    {
        await Task.Delay(250);Canvas.ForceUpdateCanvases();var r=(RectTransform)b.transform;
        var p=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));
        Check(p.x>=0&&p.x<=Screen.width&&p.y>=0&&p.y<=Screen.height,"Offscreen target: "+b.name);
        var e=new PointerEventData(EventSystem.current){position=p,button=PointerEventData.InputButton.Left,pointerId=-1};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
        Check(hits.Count>0,"No touch target: "+b.name);
        var handler=ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
        Check(handler==b.gameObject,"Covered target "+b.name+" by "+hits[0].gameObject.name);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,e,ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,e,ExecuteEvents.pointerUpHandler);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,e,ExecuteEvents.pointerClickHandler);
    }
    static void WalkCity(CityController c){for(int i=0;i<1000;i++)c.AdvanceCity(0,.02f);}
    public static async Task<string> Run()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();var a=s.GetComponent<OutdoorAdventure>();var c=s.GetComponent<CityController>();var h=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        h.CancelActivity();s.ShowSelection();s.ChooseMale();
        h.OpenKitchen();for(int i=0;i<1500;i++){r.Advance(Vector2.zero,.02f);h.Advance(.02f);}
        Check(h.State==HomeActivities.Activity.Kitchen,"Kitchen failed to open");
        for(int i=0;i<4;i++){await Tap(h.UI.recipes[i]);Check(h.SelectedRecipe==i,"Wrong recipe selected");}
        await Tap(h.UI.close);Check(h.State==HomeActivities.Activity.Idle,"Kitchen close failed");
        r.WalkToExit();for(int i=0;i<1000;i++)r.Advance(Vector2.zero,.02f);a.ChooseShoes();
        await Tap(c.UI.stationSign);for(int i=0;i<2000;i++)a.Advance(0,.02f);
        Check(c.State==CityController.Phase.Map,"Train sign failed");
        await Tap(c.UI.mapCity);Check(c.State==CityController.Phase.Travelling,"Map destination tap failed");c.AdvanceJourney(6);
        // Walk into the next viewport, then use the visible market entrance.
        c.WalkToScreen(1);WalkCity(c);await Tap(c.UI.marketDoor);WalkCity(c);Check(c.State==CityController.Phase.Market,"Market door failed");
        await Tap(c.UI.closeMarket);Check(c.State==CityController.Phase.City,"Market back failed");
        c.WalkToScreen(1);WalkCity(c);await Tap(c.UI.boutiqueDoor);WalkCity(c);Check(c.State==CityController.Phase.Wardrobe,"Boutique door failed");
        await Tap(c.UI.closeWardrobe);Check(c.State==CityController.Phase.City,"Boutique back failed");
        c.WalkToScreen(0);WalkCity(c);c.WalkToScreen(0);WalkCity(c);await Tap(c.UI.cityStation);WalkCity(c);
        await Tap(c.UI.mapHome);c.AdvanceJourney(6);Check(a.Location==OutdoorAdventure.Place.Meadow,"Home destination failed");
        a.WalkHome();for(int i=0;i<2000;i++)a.Advance(0,.02f);Check(a.Location==OutdoorAdventure.Place.Home,"Return home failed");
        return "PASS: real UI raycasts and pointer events for four kitchen cards, back, train sign, both map destinations, market, boutique and city station. "+Screen.width+"×"+Screen.height;
    }
}
