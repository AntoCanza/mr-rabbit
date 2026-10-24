using System;
using System.Reflection;
using UnityEngine;
using MrRabbit;

public static class VerifyGarden
{
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);}
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    static void Refresh(GardenController g)=>typeof(GardenController).GetMethod("Refresh",Flags).Invoke(g,null);
    static void Finish(GardenController g){for(int i=0;i<300;i++)g.AdvanceActor(.02f);}
    static void Travel(OutdoorAdventure a){for(int i=0;i<1200;i++)a.Advance(0,.02f);}
    public static string Run()
    {
        // Verify every crop through repeated watering, dry pauses, harvest and a fresh cycle.
        for(int crop=0;crop<6;crop++)
        {
            var p=new GardenProgress();
            Check(!p.Sow(0)&&!p.Water(0)&&!p.Harvest(0)&&!p.Choose(0,crop),"Out of order action accepted");
            Check(p.Hoe(0)&&p.Choose(0,crop)&&p.Sow(0),"Plant sequence");
            p.AdvanceDays(99);Check(p.plots[0].growth==0&&p.NeedsWater(0),"Dry seed grew");
            Check(p.Water(0)&&!p.Water(0),"Initial water guard");
            p.AdvanceDays(GardenProgress.GrowDays[crop]*.21f);
            Check(p.GetStage(0)==GardenProgress.Stage.Sprout&&p.HasGrowthNews(0),"Sprout stage/news");
            p.MarkSeen();Check(!p.HasGrowthNews(0),"News acknowledgement");
            p.AdvanceDays(99);float dryGrowth=p.plots[0].growth;
            p.AdvanceDays(99);Check(p.plots[0].growth==dryGrowth&&p.NeedsWater(0),"Dry growth failed to pause");
            Check(p.plots[1].growth==0&&p.GetStage(1)==GardenProgress.Stage.Empty,"Other plot changed");
            bool growing=p.GetStage(0)==GardenProgress.Stage.Growing;
            for(int i=0;i<6&&p.GetStage(0)!=GardenProgress.Stage.Ripe;i++)
            {p.Water(0);p.AdvanceDays(.4f);growing|=p.GetStage(0)==GardenProgress.Stage.Growing;}
            Check(growing&&p.GetStage(0)==GardenProgress.Stage.Ripe&&!p.NeedsWater(0),"Growth/ripe cycle");
            var restored=GardenProgress.FromJson(JsonUtility.ToJson(p));
            Check(restored.GetStage(0)==GardenProgress.Stage.Ripe&&restored.plots[0].crop==crop,"Reload state");
            Check(restored.Harvest(0)&&restored.harvests[crop]==GardenProgress.Yield(crop)&&restored.GetStage(0)==GardenProgress.Stage.Empty,"Harvest/reset");
            Check(!restored.Sow(0)&&restored.Hoe(0),"Next cycle must hoe");
        }
        Check(GardenProgress.FromJson("broken").GetStage(0)==GardenProgress.Stage.Empty,"Malformed save fallback");
        Check(GardenProgress.FromJson("").plots.Length==3,"Empty save fallback");
        var selection=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var a=selection.GetComponent<OutdoorAdventure>();var g=selection.GetComponent<GardenController>();
        var clock=selection.GetComponent<DayNightCycle>();
        var field=typeof(GardenController).GetField("progress",Flags);var original=g.Progress;
        bool saved=PlayerPrefs.HasKey(GardenController.SaveKey);string json=PlayerPrefs.GetString(GardenController.SaveKey);
        try
        {
            foreach(bool female in new[]{false,true})
            foreach(bool rollers in new[]{false,true})
            {
                field.SetValue(g,new GardenProgress());
                selection.ShowSelection();if(female)selection.ChooseFemale();else selection.ChooseMale();
                a.WalkOutOfHome();var walker=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();
                for(int i=0;i<900;i++)walker.Advance(Vector2.zero,.02f);
                Check(a.Location==OutdoorAdventure.Place.FootwearChoice,"Exit house");
                if(rollers)a.ChooseRollers();else a.ChooseShoes();
                g.GoToGarden();Travel(a);Check(a.Location==OutdoorAdventure.Place.Garden,"Garden routing");
                Check(clock.IsAdvancing,"Garden clock");
                g.PickTool(0);Finish(g);Check(g.HeldTool==GardenController.Tool.Hoe,"Pick hoe");
                g.TapBed(0);Finish(g);Check(g.Progress.GetStage(0)==GardenProgress.Stage.Prepared,"Hoe soil");
                g.PickTool(1);Finish(g);g.ChooseSeeds(0);g.TapBed(0);Finish(g);
                Check(g.Progress.NeedsWater(0),"Sow seeds");
                g.PickTool(2);Finish(g);g.TapBed(0);Finish(g);
                Check(!g.Progress.NeedsWater(0),"Water plot");
                g.RequestExit();Finish(g);Check(a.Location==OutdoorAdventure.Place.Meadow&&a.UsingRollers==rollers,"Return/footwear");
                clock.Advance(clock.CycleSeconds*.21f);Refresh(g);
                Check(g.NoticeVisible&&g.NoticeSymbol==7&&g.NoticeText=="","Growth picture notification");
                clock.Advance(clock.CycleSeconds*.2f);Refresh(g);
                Check(g.NoticeVisible&&g.NoticeSymbol==5&&g.NoticeText=="","Water picture notification");
                g.GoToGarden();Travel(a);Check(g.SelectedPlot==0,"Guide selects thirsty plot");
                for(int cycle=0;cycle<3&&g.Progress.GetStage(0)!=GardenProgress.Stage.Ripe;cycle++)
                {g.PickTool(2);Finish(g);g.TapBed(0);Finish(g);clock.Advance(clock.CycleSeconds*.4f);}
                Check(g.Progress.GetStage(0)==GardenProgress.Stage.Ripe,"Clock ripening");
                g.RequestExit();Finish(g);Refresh(g);Check(g.NoticeVisible&&g.NoticeSymbol==6&&g.NoticeText=="","Harvest picture notification");
                g.GoToGarden();Travel(a);g.PickTool(3);Finish(g);g.TapBed(0);Finish(g);
                Check(g.Progress.TotalHarvests==3&&g.Progress.GetStage(0)==GardenProgress.Stage.Empty,"Harvest interaction");
                Check(GardenProgress.FromJson(PlayerPrefs.GetString(GardenController.SaveKey)).TotalHarvests==3,"Persistent harvest");
                g.RequestExit();Finish(g);a.WalkHome();Travel(a);Check(a.Location==OutdoorAdventure.Place.Home,"Home from expanded lawn");
                selection.ShowSelection();float before=clock.NormalizedTime;clock.Advance(99);Check(clock.NormalizedTime==before,"Menu clock must pause");
            }
        return "PASS: six crops; ordered actions; stages; repeated watering; no dry/offline growth; independent beds; save/reload; harvest/reset; both rabbits/footwear; garden travel; all meadow alerts; menu pause.";
        }
        finally
        {
            field.SetValue(g,original);typeof(GardenController).GetField("dirty",Flags).SetValue(g,false);
            if(saved)PlayerPrefs.SetString(GardenController.SaveKey,json);else PlayerPrefs.DeleteKey(GardenController.SaveKey);
            PlayerPrefs.Save();selection.ShowSelection();selection.ChooseMale();Refresh(g);
        }
    }
    public static string Preview(int mode)
    {
        var s=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();var a=s.GetComponent<OutdoorAdventure>();
        var g=s.GetComponent<GardenController>();
        if(a.Location!=OutdoorAdventure.Place.Garden)
        {s.ShowSelection();s.ChooseMale();a.WalkOutOfHome();var w=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();for(int i=0;i<900;i++)w.Advance(Vector2.zero,.02f);a.ChooseShoes();g.GoToGarden();Travel(a);}
        s.GetComponent<DayNightCycle>().PreviewTime(.12f);
        if(mode==1){g.PickTool(0);Finish(g);g.TapBed(0);Finish(g);g.PickTool(1);Finish(g);}
        if(mode==2){g.RequestExit();Finish(g);}
        Refresh(g);return "Preview mode "+mode;
    }
}
