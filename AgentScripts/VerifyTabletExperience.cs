using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrRabbit;
using UnityEngine;
using UnityEngine.EventSystems;

public static class VerifyTabletExperience
{
    public static async Task<string[]> Run()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play first.");
        var choice=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>(FindObjectsInactive.Include);
        var clock=UnityEngine.Object.FindFirstObjectByType<DayNightCycle>();
        var passed=new List<string>();
        foreach(bool female in new[]{false,true})
        {
            choice.ShowSelection();
            if(female) choice.ChooseFemale(); else choice.ChooseMale();
            Canvas.ForceUpdateCanvases();
            await Task.Delay(180);
            Require(!choice.IsChoosing,"Selection should close");
            foreach(string name in new[]{"Touch Hint","Choose Rabbit Button","Time Of Day"})
                Require(!choice.transform.Find(name).gameObject.activeSelf,"Unwanted HUD: "+name);
            Require(!choice.transform.Find("Character Selection/Title").gameObject.activeInHierarchy,"Title visible during play");

            var bedRect=(RectTransform)bed.transform;
            var point=RectTransformUtility.WorldToScreenPoint(null,bedRect.TransformPoint(bedRect.rect.center));
            var pointer=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};
            var hits=new List<RaycastResult>(); EventSystem.current.RaycastAll(pointer,hits);
            Require(hits.Count>0 && hits[0].gameObject==bed.gameObject,"Painted bed must receive touch");
            ExecuteEvents.Execute(bed.gameObject,pointer,ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(bed.gameObject,pointer,ExecuteEvents.pointerClickHandler);
            Require(bed.IsApproaching && rabbit.HasDestination,"Bed tap must start a route");
            bool usedStairs=false;
            for(int i=0;i<3000 && !bed.IsSleeping;i++)
            {
                var old=rabbit.PositionNormalized;
                rabbit.Advance(Vector2.zero,1f/60); bed.Advance(1f/60);
                usedStairs |= rabbit.Surface==RabbitWalker.WalkSurface.Stairs;
                Require(Vector2.Distance(old,rabbit.PositionNormalized)<.005f,"Sleep route teleported");
            }
            Require(usedStairs && bed.IsSleeping && !rabbit.MovementEnabled,"Sleep requires reaching bed via stairs");
            Require(!rabbit.GetComponent<RabbitGraphic>().enabled,"Standing rabbit remains visible in bed");
            var sleeping=bed.transform.parent.Find("Sleeping Rabbit").GetComponent<UnityEngine.UI.Image>();
            Require(sleeping.gameObject.activeSelf && sleeping.sprite==rabbit.GetComponent<RabbitGraphic>().sprite,"Wrong sleeping character");
            float before=clock.NormalizedTime;clock.Advance(1);
            Require(clock.NormalizedTime!=before,"Clock should continue during sleep");
            bed.ToggleSleep();
            Require(!bed.IsSleeping && rabbit.MovementEnabled && rabbit.GetComponent<RabbitGraphic>().enabled,"Bed tap must wake");
            bed.ToggleSleep();bed.Wake();rabbit.WalkTo(new Vector2(.75f,.474f));
            Drain(rabbit);bed.Advance(0);
            Require(!bed.IsSleeping && !bed.IsApproaching,"Cancelled bed route resumed");

            rabbit.WalkTo(new Vector2(.22f,.474f));Drain(rabbit);
            Require(travel.Location==OutdoorAdventure.Place.Home,"Upstairs proximity must not exit");
            // A normal floor destination NEAR the door is sufficient; no door click or special route.
            rabbit.WalkTo(new Vector2(.25f,.118f));Drain(rabbit);
            Require(travel.Location==OutdoorAdventure.Place.FootwearChoice,"Near-door floor tap must trigger exit");
            travel.ReturnHome();rabbit.Advance(Vector2.zero,10);
            Require(travel.Location==OutdoorAdventure.Place.Home,"Cancel must not retrigger the doorway");
            rabbit.WalkTo(new Vector2(.25f,.118f));Drain(rabbit);travel.ChooseRollers();
            travel.Advance(0,10);
            Require(travel.Location==OutdoorAdventure.Place.Meadow,"Exiting must not immediately return");
            travel.WalkToScreen(.84f);
            for(int i=0;i<3000 && travel.Location==OutdoorAdventure.Place.Meadow;i++) travel.Advance(0,1f/60);
            Require(travel.Location==OutdoorAdventure.Place.Home && !travel.UsingRollers,"Approaching the house must return without sign click");
            rabbit.Advance(Vector2.zero,10);
            Require(travel.Location==OutdoorAdventure.Place.Home,"Re-entry bounced back outside");
            passed.Add((female?"Miss Rabbit":"Mr Rabbit")+": hidden HUD, bed raycast/stair route/sleep/wake/cancel, near-door exit/entry and no bounce");
        }
        clock.PreviewTime(.12f);
        return passed.ToArray();
    }

    public static string CheckFraming()
    {
        Canvas.ForceUpdateCanvases();
        var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var house=(RectTransform)rabbit.transform.parent;
        house.GetComponent<GameplayFraming>().Apply();
        foreach(var p in new[]{new Vector2(.22f,.118f),new Vector2(.81f,.118f),new Vector2(.22f,.644f),new Vector2(.81f,.644f)})
        {
            var world=house.TransformPoint(new Vector2(house.rect.xMin+p.x*house.rect.width,house.rect.yMin+p.y*house.rect.height));
            var screen=RectTransformUtility.WorldToScreenPoint(null,world);
            Require(screen.x>=0 && screen.x<=Screen.width && screen.y>=0 && screen.y<=Screen.height,"Playable corner clipped: "+p+" => "+screen);
        }
        var view=(RectTransform)house.parent;
        Require(Mathf.Abs(view.rect.width*view.lossyScale.x-Screen.width)<2 && Mathf.Abs(view.rect.height*view.lossyScale.y-Screen.height)<2,"Viewport does not fill screen");
        return Screen.width+"x"+Screen.height+": full viewport and both floors including character heads visible";
    }

    private static void Drain(RabbitWalker rabbit)
    {for(int i=0;i<3000 && rabbit.HasDestination;i++)rabbit.Advance(Vector2.zero,1f/60);Require(!rabbit.HasDestination,"Route did not finish");}
    private static void Require(bool value,string message)
    {if(!value)throw new InvalidOperationException(message);}
}
