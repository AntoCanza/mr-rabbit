using System;
using UnityEngine;
using MrRabbit;

public static class VerifyNeighborhood
{
    static void Check(bool value,string message) { if (!value) throw new Exception(message); }
    public static string Run()
    {
        var selection=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var adventure=selection.GetComponent<OutdoorAdventure>();
        var neighborhood=selection.GetComponent<Neighborhood>();
        var clock=selection.GetComponent<DayNightCycle>();
        var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var frame=rabbit.transform.parent.GetComponent<GameplayFraming>();
        var rect=(RectTransform)frame.transform;
        var random=UnityEngine.Random.state;
        try
        {
            foreach (bool female in new[]{false,true})
            foreach (bool rollers in new[]{false,true})
            {
                selection.ShowSelection();
                if(female) selection.ChooseFemale(); else selection.ChooseMale();
                Canvas.ForceUpdateCanvases();frame.Apply();frame.FollowCharacter(10);
                var size=rect.rect.size;float lower=rect.anchoredPosition.y;
                rabbit.WalkTo(new Vector2(.70f,.474f));
                for(int i=0;i<700;i++) {rabbit.Advance(Vector2.zero,.02f);frame.FollowCharacter(.02f);}
                Check(rabbit.Surface==RabbitWalker.WalkSurface.UpperFloor,"Upper floor unreachable");
                Check(rect.anchoredPosition.y<lower-20,"Camera did not pan upward");
                Check((size-rect.rect.size).sqrMagnitude<.01f,"Camera changed proportions/zoom");
                var viewport=(RectTransform)rect.parent;
                var body=(RectTransform)rabbit.transform;
                var center=viewport.InverseTransformPoint(body.TransformPoint(body.rect.center));
                Check(viewport.rect.Contains(center),"Player outside viewport upstairs");
                rabbit.WalkToExit();for(int i=0;i<700;i++)rabbit.Advance(Vector2.zero,.02f);
                Check(adventure.Location==OutdoorAdventure.Place.FootwearChoice,"Door route failed");
                if(rollers)adventure.ChooseRollers();else adventure.ChooseShoes();
                neighborhood.ResetResidents();
                for(int home=0;home<2;home++)
                {
                    if(home==0)neighborhood.VisitCoccodella();else neighborhood.VisitZebrella();
                    for(int i=0;i<700;i++)adventure.Advance(0,.02f);
                    Check(adventure.Location==OutdoorAdventure.Place.Visit && neighborhood.Visiting==home,"Visit entry failed");
                    Check(clock.IsAdvancing,"Clock stopped during visit");
                    neighborhood.WalkInside(.80f);for(int i=0;i<200;i++)neighborhood.AdvanceVisitor(0,.02f);
                    Check(neighborhood.VisitorPosition>.75f,"Visitor cannot walk in room");
                    neighborhood.GreetHost();
                    neighborhood.WalkToExit();for(int i=0;i<250;i++)neighborhood.AdvanceVisitor(0,.02f);
                    Check(adventure.Location==OutdoorAdventure.Place.Meadow,"Visit exit failed");
                    Check(adventure.UsingRollers==rollers,"Visit lost footwear choice");
                }
                for(int group=0;group<3;group++)
                {
                    neighborhood.ResetResidents();neighborhood.StartOuting(group);
                    for(int friend=0;friend<2;friend++)
                        Check((neighborhood.ResidentActivity(friend)!=Neighborhood.Activity.AtHome)==(group==2||group==friend),"Wrong outing participants");
                    float start=neighborhood.ResidentPosition(group==1?1:0);
                    neighborhood.AdvanceResidents(1);
                    Check(Mathf.Abs(neighborhood.ResidentPosition(group==1?1:0)-start)>.001f,"Resident did not walk");
                }
                // Prevent the scheduler creating a new outing while verifying the current return.
                typeof(Neighborhood).GetField("nextEvent",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(neighborhood,1000f);
                for(int i=0;i<2500;i++)neighborhood.AdvanceResidents(.02f);
                Check(neighborhood.ResidentActivity(0)==Neighborhood.Activity.AtHome&&neighborhood.ResidentActivity(1)==Neighborhood.Activity.AtHome,"Residents did not return home");
                neighborhood.StartOuting(0);
                neighborhood.VisitCoccodella();for(int i=0;i<700;i++)adventure.Advance(0,.02f);
                var host=GameObject.Find("Friend House Viewport/Room/Resident");
                Check(host==null || !host.activeInHierarchy,"Resident duplicated at home while outside");
                neighborhood.WalkToExit();for(int i=0;i<200;i++)neighborhood.AdvanceVisitor(0,.02f);
                adventure.WalkHome();for(int i=0;i<900;i++)adventure.Advance(0,.02f);
                Check(adventure.Location==OutdoorAdventure.Place.Home,"Return to own house failed");
            }
            selection.ShowSelection();selection.ChooseMale();neighborhood.ResetResidents();
            return "PASS: both rabbits, both footwear choices, camera follows without zoom, both visits, indoor movement/exit, clock, all three outing groups, returns and absent hosts.";
        }
        finally {UnityEngine.Random.state=random;}
    }
}
