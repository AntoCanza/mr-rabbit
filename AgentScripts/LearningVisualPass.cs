using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MrRabbit;
public static class LearningVisualPass
{
    static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
    static T Get<T>(object o,string n)=>(T)o.GetType().GetField(n,F).GetValue(o);
    static void Check(bool ok,string text){if(!ok)throw new Exception(text);}
    static void Walk(OutdoorAdventure a){for(int i=0;i<3000;i++)a.Advance(0,.02f);}
    static void Work(GardenController g){for(int i=0;i<600;i++)g.AdvanceActor(.02f);}
    static void CityWalk(CityController c){for(int i=0;i<1500;i++)c.AdvanceCity(0,.02f);}
    static void Drain(RabbitWalker r,HomeActivities h=null){for(int i=0;i<2500;i++){r.Advance(Vector2.zero,.02f);h?.Advance(.02f);}}
    static void Draw(LearningJourney j)
    {
        for(int i=0;i<j.Lesson.Answer.Length;i++)
        {
            var pad=j.UI.pads[i];var glyph=HandwritingRecognizer.Templates.Concat(HandwritingRecognizer.Consonants).First(g=>g.Symbol==j.Lesson.Answer[i]);
            PointerEventData Event(Vector2 p){var r=pad.rectTransform;return new PointerEventData(EventSystem.current){pointerId=23,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(new Vector2(r.rect.x+p.x*r.rect.width,r.rect.y+p.y*r.rect.height)))};}
            foreach(var s in glyph.Strokes){pad.OnPointerDown(Event(s[0]));foreach(var p in s)pad.OnDrag(Event(p));pad.OnPointerUp(Event(s.Last()));}
        }
    }
    public static async Task<string> Run()
    {
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();var j=a.GetComponent<LearningJourney>();var s=a.GetComponent<SchoolController>();var g=a.GetComponent<GardenController>();var c=a.GetComponent<CityController>();var select=a.GetComponent<CharacterSelection>();var clock=a.GetComponent<DayNightCycle>();var voice=a.GetComponent<ItalianVoice>();
        var gu=Get<GardenController.Widgets>(g,"ui");var home=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        string learning=JsonUtility.ToJson(j.Progress),garden=JsonUtility.ToJson(g.Progress),city=JsonUtility.ToJson(c.Progress),school=JsonUtility.ToJson(Get<SchoolController.Progress>(s,"progress"));
        var keys=new[]{LearningProgress.SaveKey,GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey};var disk=keys.ToDictionary(k=>k,k=>(PlayerPrefs.HasKey(k),PlayerPrefs.GetString(k)));bool auto=j.AutoOpen;float time=clock.NormalizedTime;
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"Captures","Release-"+Application.version,Screen.width+"x"+Screen.height);Directory.CreateDirectory(folder);int captures=0,rays=0;
        var issues=new List<string>();
        void Ray(GameObject target,bool ink=false)
        {
            var r=(RectTransform)target.transform;Vector2 p=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));if(p.x<0||p.y<0||p.x>Screen.width||p.y>Screen.height){issues.Add("Offscreen "+target.name);return;}
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=p},hits);var got=hits.Count==0?null:ink?ExecuteEvents.GetEventHandler<IPointerDownHandler>(hits[0].gameObject):ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
            if(got!=target)issues.Add("Covered "+target.name+" by "+(hits.Count==0?"nothing":hits[0].gameObject.name));rays++;
        }
        async Task Capture(string name,params Button[] expected)
        {
            await Task.Delay(160);Canvas.ForceUpdateCanvases();
            foreach(var b in expected){if(!b.gameObject.activeInHierarchy)issues.Add("Hidden "+b.name);else Ray(b.gameObject);}
            if(j.IsOpen)
            {
                foreach(var b in j.UI.root.GetComponentsInChildren<Button>(false))if(b.isActiveAndEnabled)Ray(b.gameObject);
                if(j.State==LearningJourney.Page.Exercise&&j.Lesson.Writing&&!j.Answered)foreach(var pad in j.UI.pads)if(pad.gameObject.activeInHierarchy)Ray(pad.gameObject,true);
            }
            Ray(voice.repeat.gameObject);Ray(voice.mute.gameObject);
            foreach(var t in a.transform.parent.GetComponentsInChildren<TMPro.TMP_Text>(false))if(t.isActiveAndEnabled&&!string.IsNullOrEmpty(t.text))
            {
                if(t.isTextOverflowing)issues.Add("Overflow "+name+" / "+t.name+" / "+t.text);
                if(t.textInfo.characterInfo.Take(t.textInfo.characterCount).Any(ch=>ch.isVisible&&ch.textElement!=null&&ch.textElement.unicode==0x25A1)&&!t.text.Contains("□"))issues.Add("Missing glyph "+name+" / "+t.name);
            }
            await Awaitable.EndOfFrameAsync();var shot=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,name+".png"),shot.EncodeToPNG());UnityEngine.Object.Destroy(shot);captures++;
        }
        try
        {
            j.Close();JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new LearningProgress()),j.Progress);j.AutoOpen=true;select.ShowSelection();select.ChooseMale();clock.PreviewTime(.12f);await Capture("39-medal-wall-empty",j.UI.openWall);
            s.PackAtHome();s.Pack(0);s.Pack(1);s.Pack(2);a.WalkOutOfHome();Drain(rabbit);a.ChooseRollers();s.GoToSchool();Walk(a);s.Sit();s.Advance(2);s.Listen();s.Advance(2);j.ChangeTab(0);
            await Capture("40-italian-path-locked",j.UI.practice,j.UI.italian,j.UI.maths);j.StartSession(false);await Capture("41-practice-vowel",j.UI.submit,j.UI.help,j.UI.erase);j.Help();await Capture("42-free-model",j.UI.help);Draw(j);Check(j.SubmitWriting(),"Model ink");await Capture("43-practice-next",j.UI.next);j.Back();
            j.Progress.practice[0]=3;j.OpenNotebook();j.StartSession(true);await Capture("44-assessment",j.UI.help,j.UI.submit);
            for(int i=0;i<5;i++){Draw(j);Check(j.SubmitWriting(),"Assessment ink");j.Next();}await Capture("45-earned-medal",j.UI.resultNext);
            j.OpenNotebook();for(int i=0;i<11;i++){j.Progress.medals[i]=true;j.Progress.practice[i]=3;}j.ChangeTab(0);await Capture("46-italian-path-earned");j.ChangeTab(1);await Capture("47-maths-path");
            foreach(int skill in new[]{1,10,2,3,4,5,6,7,8,9})
            {
                j.SelectSkill(skill);j.StartSession(false);
                if(skill==5||skill==6||skill==9){for(int tries=0;tries<70&&(skill==5?j.Lesson.Left!=10:skill==6?int.Parse(j.Lesson.Answer)<10:int.Parse(j.Lesson.Answer)<15);tries++){j.Back();j.StartSession(false);}}
                await Capture((48+Array.IndexOf(new[]{1,10,2,3,4,5,6,7,8,9},skill))+"-skill-"+skill);j.Back();
            }
            j.Close();s.Leave();a.WalkHome();Walk(a);for(int i=0;i<11;i++)j.Progress.Hang(i,i);j.RefreshHome();await Capture("58-home-medals",j.UI.openWall);j.OpenWall();await Capture("59-medal-collection");j.SelectMedal(8);await Capture("60-choose-frame");j.Hang(11);j.Close();
            home.OpenKitchen();Drain(rabbit,home);g.Progress.harvests[4]=g.Progress.harvests[5]=6;g.Progress.meals[4]=g.Progress.meals[5]=1;home.SelectRecipe(4);await Capture("61-pumpkin-recipe",home.UI.recipes[4],home.UI.recipes[5],home.UI.cook,home.UI.eat);
            foreach(int crop in new[]{4,5}){home.SelectRecipe(crop);int before=g.Progress.harvests[crop];home.Cook();home.Advance(4);Check(g.Progress.harvests[crop]==before-1,"Reward recipe cooking");home.Eat();Drain(rabbit,home);home.Advance(3);Check(home.State==HomeActivities.Activity.Idle,"Reward recipe eating");if(crop==4){home.OpenKitchen();Drain(rabbit,home);}}
            a.WalkOutOfHome();Drain(rabbit);a.ChooseRollers();g.GoToGarden();Walk(a);
            for(int i=0;i<2;i++){g.Progress.plots[i]=new GardenProgress.Plot();g.Progress.Hoe(i);g.Progress.Choose(i,i+4);g.Progress.Sow(i);for(int n=0;n<6;n++){g.Progress.Water(i);g.Progress.AdvanceDays(.4f);}}
            await Capture("62-reward-garden",gu.tools[3]);g.Progress.plots[2]=new GardenProgress.Plot();g.TapBed(2);g.PickTool(0);Work(g);g.TapBed(2);Work(g);g.PickTool(1);Work(g);j.Progress.medals[8]=false;await Capture("63-locked-blueberries",gu.seedChoices[0],gu.seedChoices[4]);j.Progress.medals[8]=true;await Capture("64-all-six-seeds",gu.seedChoices[4],gu.seedChoices[5]);g.CancelSeeds();g.RequestExit();Work(g);
            c.GoToStation();Walk(a);c.ChooseDestination(true);c.AdvanceJourney(6);c.Approach(1);CityWalk(c);c.Progress.accepted=true;c.Progress.crop=5;c.Progress.quantity=c.Progress.reward=6;c.Refresh();await Capture("65-blueberry-request",c.UI.deliver,c.UI.closeMarket);c.CloseShop();c.Approach(2);CityWalk(c);c.Progress.coins=1000;j.Progress.medals[10]=j.Progress.medals[9]=false;c.Refresh();await Capture("66-school-clothes-locked",c.UI.closeWardrobe);
            j.Progress.medals[10]=j.Progress.medals[9]=true;c.Refresh();await Capture("67-school-clothes-unlocked",c.UI.clothes[2],c.UI.clothes[3]);c.Wear(2);await Capture("68-golden-outfit",c.UI.clothes[0]);c.Wear(3);await Capture("69-emerald-outfit",c.UI.clothes[2]);c.CloseShop();c.Approach(0);CityWalk(c);c.ChooseDestination(false);c.AdvanceJourney(2);await Capture("70-reward-train-passenger");c.AdvanceJourney(6);a.WalkHome();Walk(a);
            select.ShowSelection();select.ChooseFemale();j.RefreshHome();await Capture("71-female-medals",j.UI.openWall);a.WalkOutOfHome();Drain(rabbit);a.ChooseRollers();c.GoToStation();Walk(a);c.ChooseDestination(true);c.AdvanceJourney(6);c.Approach(2);CityWalk(c);c.Progress.coins=1000;c.Wear(3);await Capture("72-female-emerald",c.UI.clothes[0]);c.CloseShop();c.Approach(0);CityWalk(c);c.ChooseDestination(false);c.AdvanceJourney(2);await Capture("73-female-reward-train");c.AdvanceJourney(6);
            File.WriteAllLines(Path.Combine(folder,"learning-checks.txt"),issues.Distinct());return captures+" learning screenshots; "+rays+" actual control/ink raycasts; issues: "+string.Join(" | ",issues.Distinct());
        }
        finally
        {
            j.Close();home.CancelActivity();select.ShowSelection();JsonUtility.FromJsonOverwrite(learning,j.Progress);JsonUtility.FromJsonOverwrite(garden,g.Progress);JsonUtility.FromJsonOverwrite(city,c.Progress);JsonUtility.FromJsonOverwrite(school,Get<SchoolController.Progress>(s,"progress"));typeof(GardenController).GetField("dirty",F).SetValue(g,false);j.AutoOpen=auto;j.RefreshHome();clock.PreviewTime(time);
            foreach(var p in disk){if(p.Value.Item1)PlayerPrefs.SetString(p.Key,p.Value.Item2);else PlayerPrefs.DeleteKey(p.Key);}PlayerPrefs.Save();
        }
    }
}
