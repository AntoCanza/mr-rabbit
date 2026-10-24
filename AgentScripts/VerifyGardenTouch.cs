using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MrRabbit;

public static class VerifyGardenTouch
{
    static async System.Threading.Tasks.Task Tap(RectTransform rect,GameObject handler)
    {
        await System.Threading.Tasks.Task.Delay(150);
        Canvas.ForceUpdateCanvases();
        var point=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
        var e=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
        if(hits.Count==0||ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)!=handler)
            throw new Exception("Touch blocked at "+rect.parent.name+"/"+rect.name+"; point "+point+" depth "+rect.GetComponent<Graphic>()?.depth+" culled "+rect.GetComponent<CanvasRenderer>()?.cull+" active "+rect.gameObject.activeInHierarchy+"; hit "+(hits.Count==0?"none":hits[0].gameObject.name));
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,e,ExecuteEvents.pointerClickHandler);
    }
    static void Finish(GardenController g){for(int i=0;i<300;i++)g.AdvanceActor(.02f);}
    static void Cue(GardenController.Widgets ui,GameObject expected,bool seeds=false)
    {
        int active=0;
        foreach(var cue in ui.view.GetComponentsInChildren<ActionCue>(true))
        {
            if(!cue.GuidedOnly)throw new Exception("Garden contains distracting idle pulse");
            if(!cue.Highlighted)continue;
            active++;
            if(seeds)
            {if(!Array.Exists(ui.seedChoices,b=>b.gameObject==cue.gameObject))throw new Exception("Non-seed action highlighted over choice panel");}
            else if(cue.gameObject!=expected)throw new Exception("Wrong next action: "+cue.name);
        }
        if(active!=(seeds?4:expected==null?0:1))throw new Exception("Wrong number of guided highlights: "+active);
        if(ui.arrow!=null&&ui.arrow.gameObject.activeInHierarchy)throw new Exception("Old guiding arrow still visible");
    }
    public static async System.Threading.Tasks.Task<string> Run()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();var a=s.GetComponent<OutdoorAdventure>();var g=s.GetComponent<GardenController>();
        var flags=BindingFlags.Instance|BindingFlags.NonPublic;
        var ui=(GardenController.Widgets)typeof(GardenController).GetField("ui",flags).GetValue(g);
        var field=typeof(GardenController).GetField("progress",flags);var original=g.Progress;
        bool saved=PlayerPrefs.HasKey(GardenController.SaveKey);string json=PlayerPrefs.GetString(GardenController.SaveKey);
        try
        {
            field.SetValue(g,new GardenProgress());s.ShowSelection();s.ChooseMale();
            var framing=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>().transform.parent.GetComponent<GameplayFraming>();
            await System.Threading.Tasks.Task.Delay(150);
        Canvas.ForceUpdateCanvases();framing.Apply();framing.FollowCharacter(10);
            var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>();
            await Tap((RectTransform)bed.transform.Find("Action Hint"),bed.gameObject);
            var w=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();
            for(int i=0;i<1500;i++){w.Advance(Vector2.zero,.02f);bed.Advance(.02f);}
            if(!bed.IsSleeping)throw new Exception("Sleep icon did not sleep");
            a.WalkOutOfHome();for(int i=0;i<1500;i++)w.Advance(Vector2.zero,.02f);a.ChooseShoes();
            // Use real ground pointer events across the extension, not direct movement calls.
            for(int step=0;step<9;step++)
            {
                await System.Threading.Tasks.Task.Delay(100);Canvas.ForceUpdateCanvases();
                var groundEvent=new PointerEventData(EventSystem.current){position=new Vector2(Screen.width*.5f,Screen.height*.06f),button=PointerEventData.InputButton.Left};
                var groundHits=new List<RaycastResult>();EventSystem.current.RaycastAll(groundEvent,groundHits);
                if(groundHits.Count==0||ExecuteEvents.GetEventHandler<IPointerDownHandler>(groundHits[0].gameObject)?.GetComponent<OutdoorPointerInput>()==null)
                    throw new Exception("Expanded lawn gesture blocked: "+a.Location+" / "+(groundHits.Count==0?"none":groundHits[0].gameObject.name));
                ExecuteEvents.ExecuteHierarchy(groundHits[0].gameObject,groundEvent,ExecuteEvents.pointerDownHandler);
                groundEvent.position+=Vector2.left*70;
                ExecuteEvents.ExecuteHierarchy(groundHits[0].gameObject,groundEvent,ExecuteEvents.dragHandler);
                var movement=groundHits[0].gameObject.GetComponentInParent<OutdoorPointerInput>();
                for(int i=0;i<350;i++){movement.AdvanceHold(.02f);a.Advance(0,.02f);}
                ExecuteEvents.ExecuteHierarchy(groundHits[0].gameObject,groundEvent,ExecuteEvents.pointerUpHandler);
            }
            if(a.Position>OutdoorAdventure.LeftEdge+.02f)throw new Exception("Touch route did not reach expanded left edge");
            // Return the garden gate to the visible viewport before testing its actual icon.
            var input=UnityEngine.Object.FindFirstObjectByType<OutdoorPointerInput>();
            var back=new PointerEventData(EventSystem.current){position=new Vector2(Screen.width*.4f,Screen.height*.14f),button=PointerEventData.InputButton.Left};
            input.OnPointerDown(back);back.position+=Vector2.right*70;input.OnDrag(back);
            for(int i=0;i<1000&&a.Position<GardenController.GateX-.25f;i++){input.AdvanceHold(.02f);a.Advance(0,.02f);}
            input.OnPointerUp(back);
            await Tap((RectTransform)ui.gate.transform.Find("Action Hint"),ui.gate.gameObject);
            for(int i=0;i<1200;i++)a.Advance(0,.02f);
            if(a.Location!=OutdoorAdventure.Place.Garden)throw new Exception("Garden icon did not enter");
            Cue(ui,ui.tools[0].gameObject);
            var hoeCue=ui.tools[0].GetComponent<ActionCue>();var scale=hoeCue.Picture.localScale;
            bool animated=false;
            for(int sample=0;sample<4;sample++){await System.Threading.Tasks.Task.Delay(100);animated|=(hoeCue.Picture.localScale-scale).sqrMagnitude>.000001f;}
            if(!animated)throw new Exception("Guided picture is not animating");
            await Tap((RectTransform)ui.tools[0].transform,ui.tools[0].gameObject);Cue(ui,null);Finish(g);Cue(ui,ui.beds[0].gameObject);
            await Tap((RectTransform)ui.beds[0].transform.Find("Action Hint"),ui.beds[0].gameObject);Finish(g);Cue(ui,ui.tools[1].gameObject);
            await Tap((RectTransform)ui.tools[1].transform,ui.tools[1].gameObject);Finish(g);Cue(ui,null,true);
            await Tap((RectTransform)ui.seedChoices[2].transform,ui.seedChoices[2].gameObject);
            Cue(ui,ui.beds[0].gameObject);
            await Tap((RectTransform)ui.beds[0].transform,ui.beds[0].gameObject);Finish(g);Cue(ui,ui.tools[2].gameObject);
            if(g.Progress.plots[0].crop!=2||!g.Progress.NeedsWater(0))throw new Exception("Seed picture action");
            await Tap((RectTransform)ui.tools[2].transform,ui.tools[2].gameObject);Finish(g);Cue(ui,ui.beds[0].gameObject);
            await Tap((RectTransform)ui.beds[0].transform.Find("Action Hint"),ui.beds[0].gameObject);Finish(g);
            await Tap((RectTransform)ui.exit.transform,ui.exit.gameObject);Finish(g);
            s.GetComponent<DayNightCycle>().Advance(60);
            typeof(GardenController).GetMethod("Refresh",flags).Invoke(g,null);
            await Tap((RectTransform)ui.notice.transform,ui.notice.gameObject);
            for(int i=0;i<1200;i++)a.Advance(0,.02f);
            if(a.Location!=OutdoorAdventure.Place.Garden)throw new Exception("Notification touch");
            Cue(ui,ui.tools[2].gameObject);
            g.Progress.plots[0].growth=GardenProgress.GrowDays[2];
            typeof(GardenController).GetMethod("Refresh",flags).Invoke(g,null);Cue(ui,ui.tools[3].gameObject);
            await Tap((RectTransform)ui.tools[3].transform,ui.tools[3].gameObject);Finish(g);Cue(ui,ui.beds[0].gameObject);
            await Tap((RectTransform)ui.beds[0].transform,ui.beds[0].gameObject);Finish(g);Cue(ui,ui.tools[0].gameObject);
            return "PASS: real UI touch route; exclusive animated highlights through hoe, soil, seed choices, sow, water, dry reminder, harvest and restart; no arrows; no distracting pulses; "+Screen.width+"x"+Screen.height;
        }
        finally
        {
            field.SetValue(g,original);typeof(GardenController).GetField("dirty",flags).SetValue(g,false);
            if(saved)PlayerPrefs.SetString(GardenController.SaveKey,json);else PlayerPrefs.DeleteKey(GardenController.SaveKey);
            PlayerPrefs.Save();s.ShowSelection();s.ChooseMale();
        }
    }
}
