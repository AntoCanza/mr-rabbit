using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using MrRabbit;

public static class VerifyHomeActivities
{
    static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    static void Arrive(RabbitWalker r,HomeActivities a)
    {for(int i=0;i<4000&&r.HasDestination;i++)r.Advance(Vector2.zero,1f/60);Require(!r.HasDestination,"Route stuck");a.Advance(0);}
    public static string[] Run()
    {
        Require(Application.isPlaying,"Enter Play first");
        var choose=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var garden=UnityEngine.Object.FindFirstObjectByType<GardenController>();
        var a=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>(FindObjectsInactive.Include);
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        bool hadSave=PlayerPrefs.HasKey(GardenController.SaveKey),hadMute=PlayerPrefs.HasKey("MrRabbit.TV.Muted");
        string saved=PlayerPrefs.GetString(GardenController.SaveKey),runtime=JsonUtility.ToJson(garden.Progress);
        int mute=PlayerPrefs.GetInt("MrRabbit.TV.Muted");
        var passed=new List<string>();
        try
        {
            foreach(var graphic in a.GetComponentsInChildren<HearthGraphic>(true))
                Require(graphic.GetComponent<CanvasRenderer>()!=null,"Missing renderer for "+graphic.name);
            var old=GardenProgress.FromJson("{\"version\":1,\"plots\":[{},{},{}],\"harvests\":[2,3,4,5]}");
            Require(old.harvests[0]==2&&old.meals.Length==6&&old.meals[0]==0,"Old-save migration lost harvests");
            Require(!old.Cook(-1)&&!old.Eat(4)&&!old.Eat(0),"Invalid transaction accepted");
            passed.Add("Old-save migration and invalid inventory operations");
            foreach(bool female in new[]{false,true})
            {
                a.CancelActivity();choose.ShowSelection();if(female)choose.ChooseFemale();else choose.ChooseMale();
                a.ToggleTV();Arrive(r,a);
                Require(a.IsWatching&&!r.MovementEnabled&&!r.GetComponent<RabbitGraphic>().enabled,"TV seating");
                var pose=a.transform.Find("Seated Rabbit").GetComponent<UnityEngine.UI.Image>();
                Require(pose.sprite.name==(female?"MissRabbitSeated":"MrRabbitSeated"),"Wrong seated character");
                int channel=a.Channel;a.Advance(3.1f);Require(a.Channel!=channel,"TV did not advance");
                a.TapTV();Require(a.Channel==(channel+2)%3,"Tap TV did not change channel");
                if(a.IsMuted)a.ToggleSound();Require(a.SoundPlaying,"TV audio did not start");
                a.ToggleSound();Require(!a.SoundPlaying,"Mute failed");a.ToggleSound();
                a.ToggleTV();Require(r.MovementEnabled&&!a.SoundPlaying,"TV did not release player/audio");
                a.ToggleTV();Arrive(r,a);bed.ToggleSleep();
                for(int i=0;i<4000&&r.HasDestination;i++)r.Advance(Vector2.zero,1f/60);
                bed.Advance(0);Require(bed.IsSleeping&&!a.IsWatching&&!a.SoundPlaying,"TV to bed transition");
                a.OpenKitchen();Arrive(r,a);Require(!bed.IsSleeping&&a.State==HomeActivities.Activity.Kitchen,"Bed to kitchen");
                for(int crop=0;crop<4;crop++)
                {
                    var p=garden.Progress;p.harvests[crop]=0;p.meals[crop]=0;
                    a.SelectRecipe(crop);a.Cook();a.Eat();Require(a.State==HomeActivities.Activity.Kitchen,"Empty pantry accepted");
                    p.plots[0]=new GardenProgress.Plot();p.Hoe(0);p.Choose(0,crop);p.Sow(0);
                    for(int j=0;j<6;j++){p.Water(0);p.AdvanceDays(.4f);}
                    Require(p.Harvest(0)&&p.harvests[crop]==3,"Harvest workflow failed");
                    a.Cook();a.Cook();a.Advance(1);a.CancelActivity();Require(p.harvests[crop]==3&&p.meals[crop]==0,"Cancelled cooking consumed food");
                    a.OpenKitchen();Arrive(r,a);a.SelectRecipe(crop);a.Cook();a.Advance(3.1f);
                    Require(p.harvests[crop]==2&&p.meals[crop]==1,"Cooking inventory transaction");
                    var disk=GardenProgress.FromJson(PlayerPrefs.GetString(GardenController.SaveKey));
                    Require(disk.harvests[crop]==2&&disk.meals[crop]==1,"Cooked meal not saved");
                    a.Eat();Arrive(r,a);Require(a.State==HomeActivities.Activity.Eating&&r.Surface==RabbitWalker.WalkSurface.UpperFloor,"Dining route");
                    a.CancelActivity();Require(p.meals[crop]==1,"Cancelled eating lost meal");
                    a.OpenKitchen();Arrive(r,a);a.SelectRecipe(crop);int eaten=p.mealsEaten;
                    a.Eat();a.Eat();Arrive(r,a);a.Advance(2.6f);
                    Require(p.meals[crop]==0&&p.mealsEaten==eaten+1&&a.State==HomeActivities.Activity.Idle&&r.MovementEnabled,"Eating completion");
                    disk=GardenProgress.FromJson(PlayerPrefs.GetString(GardenController.SaveKey));
                    Require(disk.meals[crop]==0&&disk.mealsEaten==eaten+1,"Eating not saved");
                    a.OpenKitchen();Arrive(r,a);
                }
                a.ToggleTV();Arrive(r,a);travel.WalkOutOfHome();Arrive(r,a);
                Require(travel.Location==OutdoorAdventure.Place.FootwearChoice&&!a.SoundPlaying,"TV to exit door");
                travel.ReturnHome();
                a.OpenKitchen();a.CancelActivity();r.StopWalking();a.Advance(0);
                Require(a.State==HomeActivities.Activity.Idle&&r.MovementEnabled,"Cancelled approach resumed");
                a.ToggleTV();Arrive(r,a);choose.ShowSelection();
                Require(!a.SoundPlaying&&a.State==HomeActivities.Activity.Idle,"Selection left sound playing");
                passed.Add((female?"Miss Rabbit":"Mr Rabbit")+": TV/3 channels/audio/mute, bed/door/selection transitions; all 4 harvest-cook-eat recipes, cancellation, double taps and persistence");
            }
            choose.ChooseMale();return passed.ToArray();
        }
        finally
        {
            a.CancelActivity();bed.Wake();
            typeof(GardenController).GetField("progress",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(garden,GardenProgress.FromJson(runtime));
            if(hadSave)PlayerPrefs.SetString(GardenController.SaveKey,saved);else PlayerPrefs.DeleteKey(GardenController.SaveKey);
            if(hadMute)PlayerPrefs.SetInt("MrRabbit.TV.Muted",mute);else PlayerPrefs.DeleteKey("MrRabbit.TV.Muted");
            PlayerPrefs.Save();
        }
    }

    public static async System.Threading.Tasks.Task<string> CheckTouch()
    {
        var a=UnityEngine.Object.FindFirstObjectByType<HomeActivities>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        a.CancelActivity();travel.ReturnHome();
        r.WalkTo(new Vector2(.70f,.118f));Arrive(r,a);
        await Tap(a.transform.Find("Armchair").gameObject);Arrive(r,a);Require(a.IsWatching,"Chair tap missed");
        int old=a.Channel;await Tap(a.transform.Find("Television").gameObject);Require(a.Channel!=old,"TV tap missed");
        await Tap(a.transform.parent.Find("TV Controls/Stand Up").gameObject);Require(a.State==HomeActivities.Activity.Idle,"Stand tap missed");
        r.WalkTo(new Vector2(.365f,.118f));Arrive(r,a);
        await Tap(a.transform.Find("Cooking Station").gameObject);Arrive(r,a);Require(a.State==HomeActivities.Activity.Kitchen,"Kitchen tap missed");
        for(int i=0;i<4;i++)await Tap(a.transform.parent.Find("Kitchen Menu/Recipe "+i).gameObject);
        Require(a.SelectedRecipe==3,"Recipe selection missed");
        await Tap(a.transform.parent.Find("Kitchen Menu/Close Kitchen").gameObject);Require(a.State==HomeActivities.Activity.Idle,"Close tap missed");
        return Screen.width+"x"+Screen.height+": raycasts and pointer events passed for chair, screen, stand, kitchen, four recipes, close";
    }
    static async System.Threading.Tasks.Task Tap(GameObject target)
    {
        await System.Threading.Tasks.Task.Delay(150);Canvas.ForceUpdateCanvases();var rect=(RectTransform)target.transform;
        var point=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
        var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left,pointerId=-1};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
        Require(hits.Count>0,"No hit: "+target.name);
        var handler=ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
        Require(handler==target,"Covered touch target: "+target.name+" by "+hits[0].gameObject.name);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,data,ExecuteEvents.pointerDownHandler);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,data,ExecuteEvents.pointerUpHandler);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,data,ExecuteEvents.pointerClickHandler);
    }
}

