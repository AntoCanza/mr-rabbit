using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MrRabbit;

// Captures real rendered states and checks their fixed controls; never ships in a player.
public static class ReleaseVisualPass
{
    const BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
    static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,F).GetValue(owner);
    static void Drain(RabbitWalker r,HomeActivities h=null,BedInteraction bed=null)
    {for(int i=0;i<2000&&r.HasDestination;i++){r.Advance(Vector2.zero,.02f);h?.Advance(.02f);bed?.Advance(.02f);}h?.Advance(0);bed?.Advance(0);}
    static void Walk(OutdoorAdventure a){for(int i=0;i<2000;i++)a.Advance(0,.02f);}
    static void Work(GardenController g){for(int i=0;i<400;i++)g.AdvanceActor(.02f);}
    static void CityWalk(CityController c){for(int i=0;i<1000;i++)c.AdvanceCity(0,.02f);}
    static PointerEventData Pointer(HandwritingPad pad,Vector2 p)
    {var r=pad.rectTransform;return new PointerEventData(EventSystem.current){pointerId=91,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(new Vector2(r.rect.x+p.x*r.rect.width,r.rect.y+p.y*r.rect.height)))};}
    static void Draw(HandwritingPad pad,char answer)
    {
        foreach(var stroke in HandwritingRecognizer.Templates.First(x=>x.Symbol==answer).Strokes)
        {
            pad.OnPointerDown(Pointer(pad,stroke[0]));foreach(var p in stroke)pad.OnDrag(Pointer(pad,p));pad.OnPointerUp(Pointer(pad,stroke[stroke.Length-1]));
        }
    }
    public static async Task<string> Run()
    {
        await Task.Delay(300);Canvas.ForceUpdateCanvases();
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();var s=a.GetComponent<CharacterSelection>();var g=a.GetComponent<GardenController>();
        var c=a.GetComponent<CityController>();var n=a.GetComponent<Neighborhood>();var school=a.GetComponent<SchoolController>();var clock=a.GetComponent<DayNightCycle>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);var h=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>(FindObjectsInactive.Include);
        var gu=Field<GardenController.Widgets>(g,"ui");var su=school.UI;
        var keys=new[]{GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey,"MrRabbit.TV.Muted"};
        var disk=keys.ToDictionary(k=>k,k=>(PlayerPrefs.HasKey(k),k=="MrRabbit.TV.Muted"?PlayerPrefs.GetInt(k).ToString():PlayerPrefs.GetString(k)));
        string garden=JsonUtility.ToJson(g.Progress),city=JsonUtility.ToJson(c.Progress),pupil=JsonUtility.ToJson(Field<SchoolController.Progress>(school,"progress"));
        float time=clock.NormalizedTime;bool mute=h.IsMuted;
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"Captures","Release-"+Application.version,Screen.width+"x"+Screen.height);Directory.CreateDirectory(folder);
        var issues=new List<string>();int captures=0,buttons=0;
        async Task Capture(string name,params Button[] expected)
        {
            await Task.Delay(170);Canvas.ForceUpdateCanvases();
            foreach(var b in expected)
            {
                if(!b.gameObject.activeInHierarchy){issues.Add(name+": inactive "+b.name);continue;}
                var rect=(RectTransform)b.transform;var point=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
                if(point.x<0||point.y<0||point.x>Screen.width||point.y>Screen.height){issues.Add(name+": outside screen "+b.name);continue;}
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                if(hits.Count==0||ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)!=b.gameObject)issues.Add(name+": covered "+b.name+" by "+(hits.Count>0?hits[0].gameObject.name:"none"));
                buttons++;
            }
            foreach(var text in a.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>(false))
                if(text.isActiveAndEnabled&&!string.IsNullOrEmpty(text.text)&&text.isTextOverflowing)issues.Add(name+": overflowing text "+text.name);
            if(gu.count.gameObject.activeInHierarchy&&c.UI.wallet.activeInHierarchy)
            {
                var one=new Vector3[4];var two=new Vector3[4];((RectTransform)gu.count.transform.parent).GetWorldCorners(one);((RectTransform)c.UI.wallet.transform).GetWorldCorners(two);
                if(new Rect(one[0],one[2]-one[0]).Overlaps(new Rect(two[0],two[2]-two[0])))issues.Add(name+": basket/wallet overlap");
            }
            var voice=a.GetComponent<ItalianVoice>();if(voice!=null)
            {
                Rect Bounds(Transform t){var corners=new Vector3[4];((RectTransform)t).GetWorldCorners(corners);return new Rect(corners[0],corners[2]-corners[0]);}
                var hud=new[]{gu.count.transform.parent,c.UI.wallet.transform,c.UI.orderBadge.transform,voice.repeat.transform,voice.mute.transform}.Where(t=>t.gameObject.activeInHierarchy).ToArray();
                for(int i=0;i<hud.Length;i++)for(int j=i+1;j<hud.Length;j++)if(Bounds(hud[i]).Overlaps(Bounds(hud[j])))issues.Add(name+": HUD overlap "+hud[i].name+" / "+hud[j].name);
                foreach(var text in a.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>(false))if(text.isActiveAndEnabled)
                foreach(var letter in text.textInfo.characterInfo.Take(text.textInfo.characterCount))if(letter.isVisible)
                {
                    Vector2 lo=text.transform.TransformPoint(letter.bottomLeft),hi=text.transform.TransformPoint(letter.topRight);var glyph=new Rect(lo,hi-lo);
                    if(new[]{voice.repeat.transform,voice.mute.transform}.Any(t=>t.gameObject.activeInHierarchy&&Bounds(t).Overlaps(glyph)))issues.Add(name+": voice covers text "+text.name);
                }
            }
            await Awaitable.EndOfFrameAsync();var image=ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());UnityEngine.Object.Destroy(image);captures++;
        }
        void Home(){s.ShowSelection();s.ChooseMale();clock.PreviewTime(.12f);}
        void Outdoors(){a.WalkOutOfHome();Drain(r);a.ChooseRollers();}
        void Exercise(int subject){school.ChooseSubject(subject);for(int i=0;i<200;i++)school.Advance(.03f);school.OpenPaper();}
        try
        {
            s.ShowSelection();await Capture("01-selection",Field<Button>(s,"maleButton"),Field<Button>(s,"femaleButton"));Home();await Capture("02-home");
            bed.ToggleSleep();Drain(r,null,bed);await Capture("03-sleep");bed.Wake();h.ToggleTV();Drain(r,h);await Capture("04-TV",h.UI.stand,h.UI.sound);h.ToggleSound();await Capture("05-TV-toggle",h.UI.stand,h.UI.sound);
            h.OpenKitchen();Drain(r,h);await Capture("06-kitchen",h.UI.close);
            for(int i=0;i<4;i++){g.Progress.harvests[i]=3;g.Progress.meals[i]=1;}h.SelectRecipe(0);await Capture("07-kitchen-ready",h.UI.cook,h.UI.eat);
            h.Cook();h.Advance(1);await Capture("08-cooking",h.UI.close);h.Advance(3);h.Eat();Drain(r,h);await Capture("09-eating");h.CancelActivity();
            Outdoors();a.ReturnHome();a.WalkOutOfHome();Drain(r);await Capture("10-footwear",Field<Button>(a,"shoesButton"),Field<Button>(a,"rollersButton"),Field<Button>(a,"cancelButton"));a.ChooseRollers();await Capture("11-meadow-home",Field<Button>(a,"homeSign"));
            n.ResetResidents();n.VisitCoccodella();Walk(a);n.GreetHost();await Capture("12-coccodella",Field<Button>(n,"leaveButton"));n.WalkToExit();for(int i=0;i<500;i++)n.AdvanceVisitor(0,.02f);
            n.VisitZebrella();Walk(a);n.GreetHost();await Capture("13-zebrella",Field<Button>(n,"leaveButton"));n.WalkToExit();for(int i=0;i<500;i++)n.AdvanceVisitor(0,.02f);
            for(int i=0;i<3;i++)g.Progress.plots[i]=new GardenProgress.Plot();g.GoToGarden();Walk(a);await Capture("14-garden",gu.exit,gu.tools[0]);
            g.PickTool(0);Work(g);g.TapBed(0);Work(g);g.PickTool(1);Work(g);await Capture("15-seeds",gu.cancelSeeds,gu.seedChoices[0],gu.seedChoices[3]);
            g.ChooseSeeds(0);g.TapBed(0);Work(g);await Capture("16-garden-water",gu.tools[2]);g.RequestExit();Work(g);await Capture("17-garden-notice",gu.notice);
            school.GoToSchool();Walk(a);if(school.State!=SchoolController.Phase.Packing){school.Leave();Field<SchoolController.Progress>(school,"progress").packed=0;school.GoToSchool();Walk(a);}
            await Capture("18-schoolbag",su.kit[0],su.kit[1],su.kit[2]);school.Pack(0);school.Pack(1);school.Pack(2);await Capture("19-classroom",su.desk);
            school.Sit();school.Advance(2);school.Listen();school.Advance(2);await Capture("20-subjects",su.lessons[0],su.lessons[1]);Exercise(0);await Capture("21-vowels",su.pencil,su.erase,su.submit);
            school.SelectPen();for(int i=0;i<4;i++){school.Erase();Draw(su.pads[0],school.Lesson.Answer=="A"?'E':'A');school.Submit();}await Capture("22-help-available",su.help);school.ToggleHelp();await Capture("23-help-model",su.help);
            school.Erase();for(int i=0;i<school.Lesson.Answer.Length;i++)Draw(su.pads[i],school.Lesson.Answer[i]);school.Submit();await Capture("24-completion",su.next);
            school.ClosePaper();Exercise(1);await Capture("25-addition",su.pencil,su.erase,su.submit);school.SelectPen();for(int i=0;i<school.Lesson.Answer.Length;i++)Draw(su.pads[i],school.Lesson.Answer[i]);school.Submit();await Capture("26-star",su.next);school.Leave();
            c.GoToStation();Walk(a);await Capture("27-map-country",c.UI.mapCity,c.UI.mapClose);c.ChooseDestination(true);c.AdvanceJourney(2);await Capture("28-train");c.AdvanceJourney(6);await Capture("29-city-station",c.UI.cityStation);
            c.Approach(1);CityWalk(c);c.Progress.accepted=false;c.Refresh();await Capture("30-market",c.UI.accept,c.UI.closeMarket);c.AcceptOrder();await Capture("31-order",c.UI.closeMarket);
            c.CloseShop();c.Approach(2);CityWalk(c);c.Progress.coins=0;c.Progress.owned[0]=false;c.Progress.equipped[0]=0;c.Refresh();await Capture("32-wardrobe-locked",c.UI.closeWardrobe);
            c.Progress.coins=65;c.Refresh();await Capture("33-wardrobe-ready",c.UI.clothes[1]);c.Wear(1);await Capture("34-wardrobe-owned",c.UI.clothes[0],c.UI.closeWardrobe);
            c.CloseShop();c.Approach(0);CityWalk(c);await Capture("35-map-city",c.UI.mapHome);c.ChooseDestination(false);c.AdvanceJourney(2);await Capture("36-return-train");c.AdvanceJourney(6);
            clock.PreviewTime(.65f);await Capture("37-meadow-night");a.WalkHome();Walk(a);s.ShowSelection();s.ChooseFemale();await Capture("38-female-home");
            File.WriteAllLines(Path.Combine(folder,"checks.txt"),issues);
            return captures+" screenshots; "+buttons+" control centers checked at "+Screen.width+"x"+Screen.height+"; issues: "+string.Join(" | ",issues.Distinct());
        }
        finally
        {
            h.CancelActivity();bed.Wake();s.ShowSelection();
            JsonUtility.FromJsonOverwrite(garden,g.Progress);JsonUtility.FromJsonOverwrite(city,c.Progress);JsonUtility.FromJsonOverwrite(pupil,Field<SchoolController.Progress>(school,"progress"));
            typeof(GardenController).GetField("dirty",F).SetValue(g,false);if(h.IsMuted!=mute)h.ToggleSound();clock.PreviewTime(time);
            foreach(var pair in disk){if(!pair.Value.Item1)PlayerPrefs.DeleteKey(pair.Key);else if(pair.Key=="MrRabbit.TV.Muted")PlayerPrefs.SetInt(pair.Key,int.Parse(pair.Value.Item2));else PlayerPrefs.SetString(pair.Key,pair.Value.Item2);}PlayerPrefs.Save();
        }
    }
}
