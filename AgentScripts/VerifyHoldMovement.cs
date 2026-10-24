using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using MrRabbit;

public static class VerifyHoldMovement
{
    static void Check(bool ok,string why){if(!ok)throw new Exception(why);}
    static PointerEventData Pointer(Vector2 p,int id=41)=>new PointerEventData(EventSystem.current){pointerId=id,position=p,button=PointerEventData.InputButton.Left};
    static void RunFor(OutdoorPointerInput input,OutdoorAdventure travel,float seconds)
    {for(int i=0;i<Mathf.CeilToInt(seconds/.02f);i++){input.AdvanceHold(.02f);travel.Advance(0,.02f);}}
    public static async Task<string> Run()
    {
        var input=UnityEngine.Object.FindFirstObjectByType<OutdoorPointerInput>(FindObjectsInactive.Include);
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var choice=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var home=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        foreach(bool female in new[]{false,true})
        foreach(bool rollers in new[]{false,true})
        {
            choice.ShowSelection();if(female)choice.ChooseFemale();else choice.ChooseMale();
            travel.WalkOutOfHome();for(int i=0;i<2000&&travel.Location==OutdoorAdventure.Place.Home;i++)home.Advance(Vector2.zero,.02f);
            if(rollers)travel.ChooseRollers();else travel.ChooseShoes();
            Canvas.ForceUpdateCanvases();await Task.Delay(180);
            var player=travel.PlayerRect;var center=RectTransformUtility.WorldToScreenPoint(null,player.TransformPoint(player.rect.center));
            var e=Pointer(center);var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
            Check(hits.Count>0&&hits[0].gameObject==player.gameObject,"Character must receive touch over scenery/actions");
            e.pointerPressRaycast=hits[0];
            ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,e,ExecuteEvents.pointerDownHandler);
            Check(input.IsHolding&&travel.TouchHeld,"Capture gesture");
            float start=travel.Position;RunFor(input,travel,.5f);Check(Mathf.Abs(start-travel.Position)<.001f,"Centered finger should rest");
            e.position=center+Vector2.left*100;ExecuteEvents.Execute(input.gameObject,e,ExecuteEvents.dragHandler);
            RunFor(input,travel,10);
            float speed=rollers?.32f:.18f;
            Check(Mathf.Abs(start-travel.Position-speed*10)<.005f,"Stationary held finger stopped during scrolling");
            Check(input.Arrow.enabled&&input.Arrow.rectTransform.localScale.x<0&&!input.Arrow.raycastTarget,"Left arrow");
            var other=Pointer(center+Vector2.right*200,42);input.OnPointerDown(other);input.OnDrag(other);input.OnPointerUp(other);
            Check(input.IsHolding&&input.Direction<0,"Second finger stole movement");
            float before=travel.Position;e.position=center+Vector2.right*100;input.OnDrag(e);RunFor(input,travel,1);
            Check(travel.Position>before+.15f&&input.Direction>0,"Reverse without lifting");
            input.OnPointerUp(e);float stopped=travel.Position;RunFor(input,travel,2);
            Check(Mathf.Abs(stopped-travel.Position)<.001f&&!input.Arrow.enabled&&!travel.HasDestination,"Release must stop immediately");
            // A gesture begins anywhere; holding without moving is always inert.
            center=RectTransformUtility.WorldToScreenPoint(null,player.TransformPoint(player.rect.center));
            var beside=Pointer(center+Vector2.left*Screen.width*.25f);
            input.OnPointerDown(beside);RunFor(input,travel,.08f);input.OnPointerUp(beside);
            Check(Mathf.Abs(stopped-travel.Position)<.001f,"Background tap still queues a destination");
            input.OnPointerDown(beside);RunFor(input,travel,.5f);Check(Mathf.Abs(travel.Position-stopped)<.001f,"Stationary background hold moved");
            beside.position+=Vector2.right*80;input.OnDrag(beside);RunFor(input,travel,.5f);Check(travel.Position>stopped+.02f,"Right swipe on left side did not move right");
            beside.position+=Vector2.left*40;input.OnDrag(beside);Check(input.Direction<0,"Reversal required crossing the initial touch point");
            input.SendMessage("OnApplicationPause",true);stopped=travel.Position;RunFor(input,travel,1);
            Check(!input.IsHolding&&Mathf.Abs(stopped-travel.Position)<.001f,"Pause leaves stuck movement");
            input.OnPointerDown(beside);beside.position+=Vector2.left*80;input.OnDrag(beside);RunFor(input,travel,.5f);input.SendMessage("OnApplicationFocus",false);
            Check(!input.IsHolding&&!travel.TouchHeld,"Focus loss leaves stuck movement");
            input.OnPointerDown(beside);beside.position+=Vector2.left*80;input.OnDrag(beside);RunFor(input,travel,40);
            Check(Mathf.Abs(travel.Position-OutdoorAdventure.LeftEdge)<.001f,"World boundary");
            input.OnPointerUp(beside);
            // Taking an icon route while another finger is held safely supersedes manual travel.
            center=RectTransformUtility.WorldToScreenPoint(null,player.TransformPoint(player.rect.center));
            e=Pointer(center);input.OnPointerDown(e);e.position=center+Vector2.right*100;input.OnDrag(e);
            var school=UnityEngine.Object.FindFirstObjectByType<SchoolController>();school.GoToSchool();input.AdvanceHold(.02f);
            Check(!input.IsHolding&&travel.HasDestination,"Icon route must replace a hold");
            for(int i=0;i<2000&&travel.Location==OutdoorAdventure.Place.Meadow;i++)travel.Advance(0,.02f);
            Check(travel.Location==OutdoorAdventure.Place.School&&!input.IsHolding&&!input.Arrow.enabled,"School entry cancels hold");
            if(school.State==SchoolController.Phase.Packing)school.ClosePacking();else school.Leave();
            var garden=UnityEngine.Object.FindFirstObjectByType<GardenController>();garden.GoToGarden();
            for(int i=0;i<2000&&travel.Location==OutdoorAdventure.Place.Meadow;i++)travel.Advance(0,.02f);
            Check(travel.Location==OutdoorAdventure.Place.Garden,"Garden action icon route");
            garden.RequestExit();for(int i=0;i<300;i++)garden.AdvanceActor(.02f);
            Check(travel.Location==OutdoorAdventure.Place.Meadow&&!travel.TouchHeld,"Garden exit clean");
        }
        return "Both rabbits, shoes/rollers: real character raycast, 10-second stationary hold across scrolling, reversal, dead zone, immediate release, second-finger isolation, background hold, pause/focus cancellation, boundary, school/garden routes passed at "+Screen.width+"x"+Screen.height;
    }
}
