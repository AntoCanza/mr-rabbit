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

public static class VerifyGameGuide
{
    const BindingFlags F=BindingFlags.NonPublic|BindingFlags.Instance;
    static int checks,touches,captures;
    static GameGuide guide;
    static string folder;
    static void Check(bool ok,string label){checks++;if(!ok)throw new Exception(label);}
    static void Tick(){typeof(GameGuide).GetField("poll",F).SetValue(guide,0f);guide.Tick();Canvas.ForceUpdateCanvases();}
    static async Task Step(string id){await Task.Delay(140);Tick();Check(guide.CurrentStep==id,"Expected "+id+", found "+guide.CurrentStep);}
    static async Task Menu(){guide.OpenMenu();await Task.Delay(100);Tick();}
    static GameObject Hit(RectTransform r)
    {
        var point=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
        Check(hits.Count>0,"No ray hit "+r.name);return hits[0].gameObject;
    }
    static void Tap(Button b)
    {
        Check(b.gameObject.activeInHierarchy&&b.interactable,"Unavailable "+b.name);var r=GameGuide.ActionTarget(b);var hit=Hit(r);Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hit)==b.gameObject,"Covered "+b.name+" by "+hit.name);
        var e=new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center)),pointerId=81};
        ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerClickHandler);touches++;Tick();
    }
    static void BedTap(BedInteraction bed)
    {var r=(RectTransform)bed.transform;var hit=Hit(r);Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hit)==bed.gameObject,"Bed covered by "+hit.name);bed.OnPointerClick(new PointerEventData(EventSystem.current){button=PointerEventData.InputButton.Left});touches++;Tick();}
    static Button Find(string name)=>UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude,FindObjectsSortMode.None).First(b=>b.name==name);
    static void Walk(OutdoorAdventure a){for(int i=0;i<3000;i++)a.Advance(0,.02f);Tick();}
    static void Work(GardenController g){for(int i=0;i<650;i++)g.AdvanceActor(.02f);Tick();}
    static void Drain(RabbitWalker r,HomeActivities h=null,BedInteraction bed=null){for(int i=0;i<2200&&r.HasDestination;i++){r.Advance(Vector2.zero,.02f);h?.Advance(.02f);bed?.Advance(.02f);}h?.Advance(0);bed?.Advance(0);Tick();}
    static void CityWalk(CityController c){for(int i=0;i<1800;i++)c.AdvanceCity(0,.02f);Tick();}
    static async Task Capture(string name)
    {
        await Task.Delay(160);Tick();
        foreach(var t in guide.UI.root.GetComponentsInChildren<TMPro.TMP_Text>(false))Check(!t.isTextOverflowing,"Text overflow "+name+"/"+t.name);
        foreach(var b in guide.UI.root.GetComponentsInChildren<Button>(false))if(b.isActiveAndEnabled&&b.interactable){var r=(RectTransform)b.transform;var hit=Hit(r);Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hit)==b.gameObject,"Guide control covered "+b.name);}
        foreach(var target in guide.Targets)
        {
            if(!target.gameObject.activeInHierarchy)continue;var b=target.GetComponent<Button>();if(b!=null&&b.interactable){var hit=Hit(target);Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hit)==b.gameObject,"Finger/halo blocks "+b.name);}
        }
        foreach(var g in guide.UI.focus)Check(!g.raycastTarget,"Halo blocks touches");Check(!guide.UI.hand.GetComponent<GuideHandGraphic>().raycastTarget,"Hand blocks touches");
        await Awaitable.EndOfFrameAsync();var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());UnityEngine.Object.Destroy(image);captures++;
    }
    static PointerEventData Ink(HandwritingPad p,Vector2 point)
    {var r=p.rectTransform;return new PointerEventData(EventSystem.current){pointerId=82,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(new Vector2(r.rect.x+point.x*r.rect.width,r.rect.y+point.y*r.rect.height)))};}
    static void Draw(HandwritingPad p,char c)
    {foreach(var stroke in HandwritingRecognizer.Templates.Concat(HandwritingRecognizer.Consonants).First(t=>t.Symbol==c).Strokes){p.OnPointerDown(Ink(p,stroke[0]));foreach(var point in stroke)p.OnDrag(Ink(p,point));p.OnPointerUp(Ink(p,stroke.Last()));}}
    static async Task Answer(LearningJourney j)
    {await Task.Delay(80);if(j.Lesson.Writing){for(int i=0;i<j.Lesson.Answer.Length;i++)Draw(j.UI.pads[i],j.Lesson.Answer[i]);Tick();await Task.Delay(70);Tap(j.UI.submit);}else Tap(j.UI.choices[j.Lesson.CorrectIndex]);Check(j.Answered,"Real answer not accepted");await Task.Delay(60);}
    public static async Task<string> Introduction()
    {
        if(!Application.isPlaying)throw new Exception("Enter Play before guide QA");
        guide=UnityEngine.Object.FindFirstObjectByType<GameGuide>();folder=Path.Combine(Directory.GetCurrentDirectory(),"Captures/Guide-"+Application.version,Screen.width+"x"+Screen.height);Directory.CreateDirectory(folder);
        guide.ReplayIntro();guide.NextIntro();await Task.Delay(200);Tick();
        var drawings=guide.UI.introPictures[1].GetComponentsInChildren<MedalGraphic>();Check(drawings.Length==2&&drawings.Any(x=>x.book)&&drawings.Any(x=>!x.book),"Missing school/book illustration");
        Rect Box(RectTransform r){var corners=new Vector3[4];r.GetWorldCorners(corners);return new Rect(corners[0],corners[2]-corners[0]);}
        Check(!Box(drawings[0].rectTransform).Overlaps(Box(drawings[1].rectTransform)),"Book and medal overlap");
        await Capture("02-school-explained");guide.PreviousIntro();return "PASS: school book and medal visible, separate and raycast-safe at "+Screen.width+"x"+Screen.height;
    }
    public static async Task<string> ReplayOnce()
    {
        if(!Application.isPlaying)throw new Exception("Enter Play before guide QA");
        guide=UnityEngine.Object.FindFirstObjectByType<GameGuide>();var voice=guide.GetComponent<ItalianVoice>();
        if(guide.IntroOpen)guide.FinishIntro();guide.GetComponent<CharacterSelection>().ShowSelection();await Task.Delay(150);Tick();guide.ReplayContext();
        await Task.Delay(100);Check(voice.IsSpeaking&&voice.LastKey=="guide_choose","Requested explanation did not play");
        voice.Stop();await Task.Delay(850);Tick();Check(!voice.IsSpeaking,"Context replay repeats itself automatically");
        return "PASS: requested contextual explanation plays once, without an automatic duplicate";
    }
    public static async Task<string> Run()
    {
        if(!Application.isPlaying)throw new Exception("Enter Play before running guide QA");
        checks=touches=captures=0;var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();guide=a.GetComponent<GameGuide>();var s=a.GetComponent<SchoolController>();var g=a.GetComponent<GardenController>();var c=a.GetComponent<CityController>();var j=a.GetComponent<LearningJourney>();var select=a.GetComponent<CharacterSelection>();var clock=a.GetComponent<DayNightCycle>();var voice=a.GetComponent<ItalianVoice>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);var h=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>(FindObjectsInactive.Include);
        folder=Path.Combine(Directory.GetCurrentDirectory(),"Captures/Guide-"+Application.version,Screen.width+"x"+Screen.height);Directory.CreateDirectory(folder);
        clock.enabled=false;clock.PreviewTime(.12f);guide.enabled=true;j.Close();select.ShowSelection();h.CancelActivity();bed.Wake();
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new GameGuideProgress()),guide.Progress);JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new GardenProgress()),g.Progress);JsonUtility.FromJsonOverwrite("{\"version\":1,\"packed\":0,\"stars\":0}",typeof(SchoolController).GetField("progress",F).GetValue(s));JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new LearningProgress()),j.Progress);JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new CityProgress()),c.Progress);c.Refresh();j.RefreshHome();
        if(voice.Muted)voice.ToggleMute();guide.ReplayIntro();Tick();await Capture("01-welcome");Check(!clock.IsAdvancing,"Intro advances game time");
        Check(ExecuteEvents.GetEventHandler<IPointerClickHandler>(Hit((RectTransform)Find("Choose Mr Rabbit").transform))!=Find("Choose Mr Rabbit").gameObject,"Intro leaks selection touch");
        voice.Stop();Tick();Check(guide.IntroOpen&&guide.Progress.introPage==0,"Speech end auto-advances intro");
        Tap(guide.UI.next);await Capture("02-school-explained");Tap(guide.UI.next);Check(GameGuideProgress.Load().introPage==2&&!GameGuideProgress.Load().introFinished,"Intro resume not saved");await Capture("03-garden-explained");
        Tap(guide.UI.introMute);Check(voice.Muted,"Mute failed");Tap(guide.UI.introRepeat);Check(voice.IsSpeaking&&voice.LastKey=="guide_intro_2","Explicit muted replay failed");Tap(guide.UI.introMute);
        Tap(guide.UI.next);await Capture("04-city-explained");Tap(guide.UI.next);await Step("choose");await Capture("05-choose");
        Tap(Find("Choose Miss Rabbit"));await Step("move");await Capture("06-drag");Tick();Check(!guide.Progress.Done("move"),"Movement granted without action");
        var input=h.GetComponent<HousePointerInput>();var e=new PointerEventData(EventSystem.current){pointerId=83,button=PointerEventData.InputButton.Left,position=new Vector2(Screen.width*.40f,Screen.height*.15f)};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);if(hits[0].gameObject.GetComponentInParent<Selectable>()!=null){e.position=new Vector2(Screen.width*.60f,Screen.height*.07f);hits.Clear();EventSystem.current.RaycastAll(e,hits);}e.pointerPressRaycast=hits.First();Check(hits[0].gameObject.GetComponentInParent<Selectable>()==null,"QA drag starts on a button");input.OnPointerDown(e);e.position+=new Vector2(90,0);input.OnDrag(e);r.Advance(Vector2.zero,.2f);Tick();input.OnPointerUp(e);await Step("bag");
        Tap(s.UI.homeBag);await Step("pack_0");await Capture("07-backpack");for(int i=0;i<3;i++){await Step("pack_"+i);Tap(s.UI.kit[i]);}await Step("exit");await Capture("08-leave-home");
        Tap(Find("Exit Door"));Drain(r,h,bed);await Step("shoes");Tap(Find("Choose Rollers"));await Step("school");await Capture("09-school-route");
        var anchor=c.UI.homeExterior.GetComponent<HomeEntranceAnchor>();anchor.Apply();Check(Vector2.Distance(anchor.entrance.position,c.UI.homeExterior.rectTransform.position)<Screen.width*.25f,"Home icon detached");
        Tap(s.UI.sign);Walk(a);await Step("sit");await Capture("10-desk");Tap(s.UI.desk);s.Advance(2);await Step("listen");Tap(s.UI.listen);s.Advance(2);j.ChangeTab(0);await Step("practice");await Capture("11-practice");
        Tap(j.UI.practice);await Step("write");await Capture("12-finger-writing");Check(j.Score==0&&!j.Answered,"Guide answered school exercise");
        for(int i=0;i<3;i++){await Answer(j);if(i==0)await Capture("13-next-question");Tap(j.UI.next);}await Step("assessment");await Capture("14-check-ready");Tap(j.UI.test);
        for(int i=0;i<5;i++){await Answer(j);Tap(j.UI.next);}await Step("result");await Capture("15-earned-medal");Check(j.Progress.medals[0],"Medal not earned through real check");
        j.Close();s.Leave();a.WalkHome();Walk(a);await Step("wall");Tap(j.UI.openWall);await Step("medal_choice");Tap(j.UI.medalChoices[0]);await Step("hang");await Capture("16-hang-medal");Tap(j.UI.wallSlots[0]);Check(j.Progress.wall[0]==0,"Medal not hung");Tap(j.UI.close);
        a.WalkOutOfHome();Drain(r,h,bed);a.ChooseRollers();a.Advance(-1,10.7f);await Step("garden");Tap(g.UI.gate);Walk(a);await Step("hoe_tool");await Capture("17-garden-tool");Tap(g.UI.tools[0]);Work(g);await Step("hoe");Tap(g.UI.beds[0]);Work(g);await Step("seed_tool");Tap(g.UI.tools[1]);Work(g);await Step("seeds");await Capture("18-seeds");Tap(g.UI.seedChoices[0]);await Step("sow");Tap(g.UI.beds[0]);Work(g);await Step("water_tool");Tap(g.UI.tools[2]);Work(g);await Step("water");await Capture("19-water");Tap(g.UI.beds[0]);Work(g);await Step("growing");await Capture("20-grow-and-explore");
        Check(g.Progress.plots[0].growth==0,"Guide advances growth artificially");
        for(int i=0;i<3;i++){g.Progress.AdvanceDays(.4f);if(g.Progress.NeedsWater(0)){g.PickTool(2);Work(g);g.TapBed(0);Work(g);}}await Step("harvest_tool");Tap(g.UI.tools[3]);Work(g);await Step("harvest");Tap(g.UI.beds[0]);Work(g);Check(g.Progress.harvests[0]==3,"Real harvest missing");
        g.RequestExit();Work(g);a.ReturnHome();await Step("kitchen");Tap(h.UI.kitchen);Drain(r,h,bed);
        if(h.SelectedRecipe!=0){await Step("recipe");Tap(h.UI.recipes[0]);}
        await Step("cook");await Capture("21-cooking");Tap(h.UI.cook);h.Advance(3.1f);await Step("eat");await Capture("22-eating");Tap(h.UI.eat);Drain(r,h,bed);h.Advance(3);await Task.Delay(140);Tick();Check(g.Progress.mealsEaten==1,"Real meal missing");
        Tap(h.UI.chair);Drain(r,h,bed);await Capture("23-tv");h.CancelActivity();r.WalkTo(new Vector2(.586f,.474f));Drain(r,h,bed);await Task.Delay(140);Tick();BedTap(bed);Drain(r,h,bed);await Step("wake");await Capture("24-sleep");BedTap(bed);a.WalkOutOfHome();Drain(r,h,bed);a.ChooseRollers();await Task.Delay(140);Tick();Tap(c.UI.stationSign);Walk(a);await Step("destination");await Capture("25-map");Tap(c.UI.mapCity);c.AdvanceJourney(2);await Capture("26-train");Check(c.State==CityController.Phase.Travelling,"Trip skipped");c.AdvanceJourney(4);Tick();await Capture("27-city-route");
        c.AdvanceCity(1,2.6f);await Task.Delay(140);Tick();Tap(c.UI.marketDoor);CityWalk(c);await Step("accept");await Capture("28-market-request");Tap(c.UI.accept);await Step("collect_order");Check(!c.CanDeliver&&!guide.Targets.Contains((RectTransform)c.UI.deliver.transform),"Guide demands unavailable delivery");await Capture("29-need-more-produce");
        // Seed only this prerequisite scenario; the crop cycle above used actual actions.
        g.Progress.harvests[c.Progress.crop]=c.Progress.quantity;c.Refresh();await Step("deliver");await Capture("30-delivery-ready");int coins=c.Progress.coins,quantity=c.Progress.quantity;Tap(c.UI.deliver);Check(c.Progress.coins==coins+quantity,"Unit-price sale changed");Tap(c.UI.closeMarket);c.Approach(2);CityWalk(c);await Step("clothes_later");await Capture("31-clothes-unavailable");
        c.Progress.coins=60;c.Refresh();await Step("outfit");await Capture("32-clothes-affordable");Tap(c.UI.clothes[1]);Check(c.Progress.coins==0&&c.Progress.equipped[1]==1,"Outfit purchase failed");
        Tap(c.UI.closeWardrobe);c.Approach(0);CityWalk(c);await Step("destination");Tap(c.UI.mapHome);c.AdvanceJourney(6);Tick();
        var neighbors=a.GetComponent<Neighborhood>();neighbors.ResetResidents();neighbors.VisitCoccodella();Walk(a);await Step("greet");await Capture("36-visit-friend");Tap(Find("Resident"));await Step("visit_exit");await Capture("37-leave-friend");Tap(Find("Exit Door"));for(int i=0;i<1800;i++)neighbors.AdvanceVisitor(0,.02f);
        select.ShowSelection();select.ChooseMale();a.WalkOutOfHome();Drain(r,h,bed);a.ChooseRollers();await Capture("38-home-door-male");s.GoToSchool();Walk(a);s.Sit();s.Advance(2);s.Listen();s.Advance(2);j.SelectSkill(1);j.StartSession(false);await Step("answer");await Capture("39-choose-answer");Check(guide.Targets.Length==3,"Choice guidance singles out an answer");await Answer(j);j.Close();s.Leave();a.WalkHome();Walk(a);select.ShowSelection();select.ChooseFemale();a.WalkOutOfHome();Drain(r,h,bed);a.ChooseRollers();await Capture("40-home-door-female");
        await Menu();await Capture("33-help-menu");Tap(guide.UI.pause);Check(guide.Progress.paused&&!guide.HasGuidance&&GameGuideProgress.Load().paused,"Pause not saved");await Menu();Tap(guide.UI.pause);Check(!guide.Progress.paused,"Resume failed");
        await Menu();Tap(guide.UI.restart);await Capture("34-replay-intro");Tap(guide.UI.skip);Check(!guide.IntroOpen&&guide.Progress.introFinished,"Skip intro failed");
        await Menu();Tap(guide.UI.repeat);Check(voice.LastKey.StartsWith("guide_"),"Context replay failed");await Menu();Tap(guide.UI.skipStep);await Capture("35-skipped-step");
        Check(voice.clips.Length==168&&voice.clips.All(x=>x.clip!=null),"Missing Elsa clips");Check(guide.Progress.completed.Count>25,"Tutorial actions not remembered");
        string result=$"PASS: {checks} checks, {touches} pointer events, {captures} rendered states at {Screen.width}x{Screen.height}; intro/resume/mute/replay/skip, movement, school practice + actual assessment/medal, garden cycle, cooking/eating, sleep/TV, train, market prerequisites/sale, wardrobe prerequisites/purchase, help/pause. Seeded stock/coins only for explicit availability scenarios.";
        File.WriteAllText(Path.Combine(folder,"checks.txt"),result);clock.enabled=true;return result;
    }
}
