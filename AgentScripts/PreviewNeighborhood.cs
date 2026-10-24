using UnityEngine;
using MrRabbit;
public static class PreviewNeighborhood
{
    public static string Run(string stage)
    {
        var selection=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var a=selection.GetComponent<OutdoorAdventure>();
        var n=selection.GetComponent<Neighborhood>();
        var day=selection.GetComponent<DayNightCycle>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        day.PreviewTime(.12f);day.enabled=false;n.enabled=false;
        selection.ShowSelection();selection.ChooseMale();n.ResetResidents();
        if(stage=="upper"||stage=="bed")
        {
            var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>();
            if(stage=="bed")bed.ToggleSleep();else r.WalkTo(new Vector2(.29f,.474f));
            for(int i=0;i<600;i++){r.Advance(Vector2.zero,.02f);bed.Advance(.02f);}
            r.transform.parent.GetComponent<GameplayFraming>().FollowCharacter(10);
        }
        else if(stage!="lower")
        {
            r.WalkToExit();for(int i=0;i<100;i++)r.Advance(Vector2.zero,.02f);a.ChooseShoes();
            bool left=stage=="cocco"||stage=="left";
            if(left)n.VisitCoccodella();else n.VisitZebrella();
            for(int i=0;i<700;i++)a.Advance(0,.02f);
            if(stage=="left"||stage=="right"||stage=="friends")
            {
                n.WalkToExit();for(int i=0;i<100;i++)n.AdvanceVisitor(0,.02f);
                if(stage=="friends") {n.StartOuting(2);n.AdvanceResidents(3);}
            }
        }
        Canvas.ForceUpdateCanvases();
        return stage;
    }
}
