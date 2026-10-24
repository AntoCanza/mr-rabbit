using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MrRabbit;
public static class VerifyRelativeMovement
{
    static void Check(bool b,string m){if(!b)throw new Exception(m);}
    static PointerEventData Finger(float x=.15f,float y=.75f)=>new PointerEventData(EventSystem.current){pointerId=91,button=PointerEventData.InputButton.Left,position=new Vector2(Screen.width*x,Screen.height*y)};
    public static async System.Threading.Tasks.Task<string> Run()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();var a=s.GetComponent<OutdoorAdventure>();var c=s.GetComponent<CityController>();var n=s.GetComponent<Neighborhood>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);var h=r.GetComponentInParent<HousePointerInput>(true);
        s.ShowSelection();s.ChooseMale();
        var e=Finger();float x=r.PositionNormalized.x;h.OnPointerDown(e);r.Advance(Vector2.zero,.5f);Check(r.PositionNormalized.x==x,"Touch alone moved at home");
        e.position+=Vector2.right*100;h.OnDrag(e);r.Advance(Vector2.zero,1);Check(r.PositionNormalized.x>x+.1f,"Right swipe left of rabbit did not move right at home");
        e.position+=Vector2.left*50;h.OnDrag(e);x=r.PositionNormalized.x;r.Advance(Vector2.zero,.5f);Check(r.PositionNormalized.x<x,"Home reversal required crossing origin");
        h.OnPointerUp(e);x=r.PositionNormalized.x;r.Advance(Vector2.zero,1);Check(r.PositionNormalized.x==x&&!r.TouchHeld,"Home release drift");
        r.WalkTo(new Vector2(.43f,.118f));for(int i=0;i<500;i++)r.Advance(Vector2.zero,.02f);
        e=Finger();h.OnPointerDown(e);e.position+=Vector2.up*120;h.OnDrag(e);for(int i=0;i<50;i++)r.Advance(Vector2.zero,.02f);
        Check(r.Surface==RabbitWalker.WalkSurface.Stairs,"Up gesture failed on stairs");float y=r.PositionNormalized.y;
        e.position+=Vector2.down*60;h.OnDrag(e);for(int i=0;i<25;i++)r.Advance(Vector2.zero,.02f);Check(r.PositionNormalized.y<y,"Stair gesture did not reverse");h.OnPointerUp(e);
        r.ResetToStart();r.WalkToExit();for(int i=0;i<1000;i++)r.Advance(Vector2.zero,.02f);a.ChooseShoes();
        n.VisitZebrella();for(int i=0;i<1500;i++)a.Advance(0,.02f);
        var v=UnityEngine.Object.FindFirstObjectByType<VisitPointerInput>(FindObjectsInactive.Include);e=Finger();x=n.VisitorPosition;v.OnPointerDown(e);e.position+=Vector2.right*100;v.OnDrag(e);n.AdvanceVisitor(0,1);Check(n.VisitorPosition>x+.1f,"Visit swipe failed");
        v.OnPointerUp(e);x=n.VisitorPosition;n.AdvanceVisitor(0,1);Check(n.VisitorPosition==x,"Visit release drift");
        n.WalkToExit();for(int i=0;i<500;i++)n.AdvanceVisitor(0,.02f);c.GoToStation();for(int i=0;i<2000;i++)a.Advance(0,.02f);c.ChooseDestination(true);c.AdvanceJourney(6);
        var input=c.UI.city.GetComponent<CityPointerInput>();await System.Threading.Tasks.Task.Delay(200);Canvas.ForceUpdateCanvases();e=Finger(.5f,.85f);var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
        Check(hits.Count>0&&ExecuteEvents.GetEventHandler<IPointerDownHandler>(hits[0].gameObject)==input.gameObject,"City sky does not receive movement gestures: "+c.State+" hits="+string.Join(",",hits.ConvertAll(hit=>hit.gameObject.name)));
        e.pointerPressRaycast=hits[0];input.OnPointerDown(e);x=c.CityX;c.AdvanceCity(0,.5f);Check(c.CityX==x,"Touch alone moved in city");
        e.position+=Vector2.right*100;input.OnDrag(e);c.AdvanceCity(0,1);Check(c.CityX>x+.2f,"City swipe failed");
        e.position+=Vector2.left*50;input.OnDrag(e);x=c.CityX;c.AdvanceCity(0,.5f);Check(c.CityX<x,"City reversal failed");input.OnPointerUp(e);x=c.CityX;c.AdvanceCity(0,1);Check(c.CityX==x,"City release drift");
        e=Finger();input.OnPointerDown(e);e.position+=Vector2.right*100;input.OnDrag(e);c.Approach(1);Check(!c.TouchHeld,"City icon did not replace manual gesture");input.CancelHold();
        s.ShowSelection();s.ChooseMale();
        return "PASS: origin-independent direction, reversal without crossing origin, stationary touch, release, home stairs up/down, house visits, full-city background raycast and icon takeover.";
    }
}
