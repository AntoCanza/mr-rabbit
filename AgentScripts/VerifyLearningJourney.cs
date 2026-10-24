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

public static class VerifyLearningJourney
{
    static readonly BindingFlags F=BindingFlags.Instance|BindingFlags.NonPublic;
    static T Get<T>(object o,string key)=>(T)o.GetType().GetField(key,F).GetValue(o);
    static int buttons;
    static void Check(bool ok,string label){if(!ok)throw new Exception(label);}
    static void Walk(OutdoorAdventure a){for(int i=0;i<3500;i++)a.Advance(0,.02f);}
    static void CityWalk(CityController c){for(int i=0;i<1800;i++)c.AdvanceCity(0,.02f);}
    static void Work(GardenController g){for(int i=0;i<600;i++)g.AdvanceActor(.02f);}
    static PointerEventData Pointer(HandwritingPad pad,Vector2 point)
    {var r=pad.rectTransform;return new PointerEventData(EventSystem.current){pointerId=67,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(new Vector2(r.rect.x+point.x*r.rect.width,r.rect.y+point.y*r.rect.height)))};}
    static HandwritingRecognizer.Glyph Glyph(char c)=>HandwritingRecognizer.Templates.Concat(HandwritingRecognizer.Consonants).First(t=>t.Symbol==c);
    static void Draw(HandwritingPad pad,char c)
    {foreach(var stroke in Glyph(c).Strokes){pad.OnPointerDown(Pointer(pad,stroke[0]));foreach(var p in stroke)pad.OnDrag(Pointer(pad,p));pad.OnPointerUp(Pointer(pad,stroke.Last()));}}
    static void Ray(Button b)
    {
        Check(b.gameObject.activeInHierarchy,"Hidden control "+b.name);var r=(RectTransform)b.transform;var point=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center));
        Check(point.x>0&&point.y>0&&point.x<Screen.width&&point.y<Screen.height,"Offscreen "+b.name);
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
        Check(hits.Count>0&&ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)==b.gameObject,"Covered "+b.name+" by "+(hits.Count>0?hits[0].gameObject.name:"nothing"));buttons++;
    }
    static void Tap(Button b)
    {
        Ray(b);Check(b.interactable,"Disabled "+b.name);var r=(RectTransform)b.transform;var e=new PointerEventData(EventSystem.current){pointerId=67,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};
        ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerDownHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerUpHandler);ExecuteEvents.Execute(b.gameObject,e,ExecuteEvents.pointerClickHandler);
    }
    static async Task Answer(LearningJourney j,bool correct=true)
    {
        await Task.Delay(40);Canvas.ForceUpdateCanvases();
        if(j.Lesson.Writing){j.Erase();for(int i=0;i<j.Lesson.Answer.Length;i++)Draw(j.UI.pads[i],correct?j.Lesson.Answer[i]:j.Lesson.Alphabet?(j.Lesson.Answer=="L"?'T':'L'):j.Lesson.Vowels?(j.Lesson.Answer=="A"?'E':'A'):(j.Lesson.Answer[i]=='0'?'1':'0'));Check(j.SubmitWriting()==correct,"Unexpected ink result for "+j.Lesson.Answer+": "+string.Join(",",j.UI.pads.Select(p=>p.Recognized)));}
        else{int before=j.Score;Canvas.ForceUpdateCanvases();Tap(j.UI.choices[correct?j.Lesson.CorrectIndex:(j.Lesson.CorrectIndex+1)%3]);Check(j.Score==before+(correct?1:0)&&j.Answered==(j.Assessment||correct),"Choice touch grading");}
    }
    static void School(OutdoorAdventure a,SchoolController s,CharacterSelection select)
    {
        select.ShowSelection();select.ChooseMale();s.PackAtHome();s.Pack(0);s.Pack(1);s.Pack(2);
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();a.WalkOutOfHome();for(int i=0;i<2500;i++)r.Advance(Vector2.zero,.02f);a.ChooseRollers();s.GoToSchool();Walk(a);s.Sit();s.Advance(2);s.Listen();s.Advance(2);
    }
    public static string Logic()
    {
        var random=new System.Random(20261018);int generated=0;
        for(int skill=0;skill<LearningProgress.SkillCount;skill++)
        {
            for(int run=0;run<20;run++)
            {
                var seen=new HashSet<string>();for(int n=0;n<5;n++)
                {var q=LearningLesson.Next(skill,random,seen);Check(seen.Count==n+1,"Repeated test question");Check(q.Answer.Length>0&&(!q.Writing||q.Answer.Length<=2),"Answer size");if(q.Choices!=null)Check(q.Choices.Distinct().Count()==3&&q.Choices[q.CorrectIndex]==q.Answer,"Choice uniqueness");if(skill>=7&&skill<=9)Check(int.Parse(q.Answer)==q.Left+q.Right&&int.Parse(q.Answer)<=(skill==7?5:skill==8?10:20),"Sum range");generated++;}
            }
        }
        var old=GardenProgress.FromJson("{\"version\":1,\"plots\":[{\"prepared\":true,\"planted\":true,\"crop\":3,\"growth\":0.3,\"water\":0.2},{},{}],\"harvests\":[2,3,4,5],\"meals\":[4,3,2,1]}");
        Check(old.harvests.SequenceEqual(new[]{2,3,4,5,0,0})&&old.meals.SequenceEqual(new[]{4,3,2,1,0,0})&&old.plots[0].crop==3&&old.plots[0].planted,"Legacy garden migration");
        int letters=0;
        foreach(var glyph in HandwritingRecognizer.Consonants)
        {
            var ink=glyph.Strokes.Reverse().Select(s=>s.Reverse().Select((p,i)=>new Vector2(.14f+p.x*.69f+Mathf.Sin(i*2.3f)*.004f,.11f+p.y*.73f)).ToList()).ToList();
            char c=HandwritingRecognizer.RecognizeAlphabet(ink,out _);Check(c==glyph.Symbol,"Alphabet variation "+glyph.Symbol+" classified as "+c);letters++;
        }
        // Independently specified, uneven first-primary strokes (not generated from templates).
        var samples=new Dictionary<char,Vector2[][]>{
            ['H']=new[]{Points(.23f,.85f,.21f,.64f,.22f,.35f,.20f,.14f),Points(.77f,.88f,.79f,.57f,.76f,.12f),Points(.22f,.48f,.49f,.52f,.78f,.5f)},
            ['L']=new[]{Points(.25f,.86f,.23f,.63f,.24f,.40f,.23f,.16f,.52f,.14f,.79f,.17f)},
            ['M']=new[]{Points(.17f,.14f,.16f,.84f,.33f,.66f,.50f,.49f,.69f,.69f,.83f,.85f,.85f,.14f)},
            ['N']=new[]{Points(.21f,.14f,.22f,.86f,.40f,.63f,.60f,.38f,.79f,.15f,.78f,.88f)},
            ['T']=new[]{Points(.16f,.83f,.45f,.86f,.83f,.84f),Points(.51f,.84f,.49f,.62f,.50f,.35f,.48f,.13f)},
            ['V']=new[]{Points(.17f,.86f,.31f,.55f,.49f,.15f,.66f,.53f,.84f,.85f)},
            ['Z']=new[]{Points(.19f,.85f,.47f,.86f,.82f,.84f,.58f,.55f,.20f,.14f,.52f,.16f,.83f,.15f)}
        };
        foreach(var s in samples){var ink=s.Value.Select(line=>line.ToList()).ToList();Check(HandwritingRecognizer.RecognizeAlphabet(ink,out _)==s.Key,"Freehand "+s.Key);}
        return generated+" curriculum prompts; 16 distorted consonants + 7 independent hand-drawn samples; old garden inventory/plots preserved.";
    }
    static Vector2[] Points(params float[] xy){var v=new Vector2[xy.Length/2];for(int i=0;i<v.Length;i++)v[i]=new Vector2(xy[i*2],xy[i*2+1]);return v;}
    public static async Task<string> Run()
    {
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();var j=a.GetComponent<LearningJourney>();var school=a.GetComponent<SchoolController>();var select=a.GetComponent<CharacterSelection>();var g=a.GetComponent<GardenController>();var c=a.GetComponent<CityController>();var voice=a.GetComponent<ItalianVoice>();
        string learning=JsonUtility.ToJson(j.Progress),garden=JsonUtility.ToJson(g.Progress),city=JsonUtility.ToJson(c.Progress),pupil=JsonUtility.ToJson(Get<SchoolController.Progress>(school,"progress"));
        var keys=new[]{LearningProgress.SaveKey,GardenController.SaveKey,CityProgress.SaveKey,SchoolController.SaveKey};var saved=keys.ToDictionary(k=>k,k=>(PlayerPrefs.HasKey(k),PlayerPrefs.GetString(k)));bool wasMuted=voice.Muted,auto=j.AutoOpen;int questions=0;buttons=0;
        try
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new LearningProgress()),j.Progress);j.AutoOpen=true;School(a,school,select);await Task.Delay(200);Canvas.ForceUpdateCanvases();
            Check(j.State==LearningJourney.Page.Notebook,"School does not lead to learning");j.ChangeTab(0);Tap(j.UI.cards[0]);Ray(j.UI.test);Check(!j.UI.test.interactable,"Test bypassed practice");Tap(j.UI.practice);await Task.Delay(150);Canvas.ForceUpdateCanvases();
            Check(!j.SubmitWriting()&&j.QuestionNumber==0,"Blank counted");
            // Unknown ink is neither a wrong solution nor a completed question.
            var pad=j.UI.pads[0];pad.OnPointerDown(Pointer(pad,new Vector2(.1f,.1f)));for(int i=0;i<120;i++)pad.OnDrag(Pointer(pad,new Vector2(i%2==0?.1f:.9f,.1f+i*.006f)));pad.OnPointerUp(Pointer(pad,new Vector2(.9f,.82f)));
            Check(pad.Recognized=='\0'&&!j.SubmitWriting()&&!j.Answered&&j.QuestionNumber==0&&j.Score==0,"Uncertain ink scored");
            for(int n=0;n<3;n++){await Answer(j);Check(j.Answered,"Practice complete");Check(!j.SubmitWriting(),"Duplicate practice");j.Next();questions++;}
            Check(j.Progress.CanTest(0),"Practice target");
            // An unsuccessful check does not award a medal.
            j.StartSession(true);for(int n=0;n<5;n++){await Answer(j,n<3);j.Next();questions++;}Check(j.State==LearningJourney.Page.Result&&!j.Progress.medals[0],"3/5 awarded medal");j.OpenNotebook();j.StartSession(true);j.Help();Check(!j.Assessment&&!j.Progress.medals[0],"Help bypassed exam");j.Back();
            foreach(int skill in new[]{0,1,10,2,3,4,5,6,7,8,9})
            {
                j.SelectSkill(skill);Check(j.SelectedSkill==skill&&j.Progress.Available(skill),"Prerequisites for "+skill);
                if(!j.Progress.CanTest(skill)){j.StartSession(false);for(int n=0;n<3;n++){await Answer(j);j.Next();questions++;}}
                j.StartSession(true);var signatures=new HashSet<string>();for(int n=0;n<5;n++){Check(signatures.Add(j.Lesson.Signature),"Duplicate assessment question");await Answer(j,n!=0);Check(!j.SubmitWriting(),"Repeated submit");j.Next();questions++;}
                Check(j.State==LearningJourney.Page.Result&&j.Score==4&&j.Progress.medals[skill],"4/5 failed skill "+skill);j.OpenNotebook();
            }
            Check(j.Progress.CropUnlocked(4)&&j.Progress.CropUnlocked(5)&&j.Progress.OutfitUnlocked(2)&&j.Progress.OutfitUnlocked(3),"Reward dependencies");
            j.Close();school.Leave();a.WalkHome();Walk(a);await Task.Delay(200);Canvas.ForceUpdateCanvases();Tap(j.UI.openWall);await Task.Delay(150);Canvas.ForceUpdateCanvases();Tap(j.UI.medalChoices[0]);Tap(j.UI.wallSlots[3]);Check(j.Progress.wall[3]==0,"Medal not hung");j.SelectMedal(0);Check(j.Hang(9)&&j.Progress.wall[3]==-1&&j.Progress.wall[9]==0,"Medal move duplicated it");
            var reload=LearningProgress.Load();Check(reload.wall[9]==0&&reload.medals.All(x=>x),"Learning save reload");j.Close();
            a.WalkOutOfHome();var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();for(int i=0;i<2000;i++)r.Advance(Vector2.zero,.02f);a.ChooseRollers();
            for(int crop=4;crop<6;crop++)
            {
                g.Progress.plots[0]=new GardenProgress.Plot();g.GoToGarden();Walk(a);g.PickTool(0);Work(g);g.TapBed(0);Work(g);g.PickTool(1);Work(g);await Task.Delay(200);Canvas.ForceUpdateCanvases();var gu=Get<GardenController.Widgets>(g,"ui");Tap(gu.seedChoices[crop]);g.TapBed(0);Work(g);Check(g.Progress.plots[0].planted&&g.Progress.plots[0].crop==crop,"Reward crop sowing");
                for(int i=0;i<6;i++){g.Progress.Water(0);g.Progress.AdvanceDays(.4f);}Check(g.Progress.GetStage(0)==GardenProgress.Stage.Ripe,"Reward crop growth");int before=g.Progress.harvests[crop];g.PickTool(3);Work(g);g.TapBed(0);Work(g);Check(g.Progress.harvests[crop]==before+6,"Reward crop harvest");g.RequestExit();Work(g);
            }
            c.GoToStation();Walk(a);c.ChooseDestination(true);c.AdvanceJourney(6);c.Approach(1);CityWalk(c);
            for(int crop=4;crop<6;crop++){c.Progress.crop=crop;c.Progress.quantity=3;c.Progress.reward=3;c.Progress.accepted=true;g.Progress.harvests[crop]=3;int coins=c.Progress.coins;Check(c.Deliver()&&c.Progress.coins==coins+3&&g.Progress.harvests[crop]==0,"One coin per reward crop");}
            c.CloseShop();c.Approach(2);CityWalk(c);c.Progress.coins=1000;
            foreach(bool female in new[]{false,true})
            {
                // Character selection cannot be changed in a shop; configure only the selected avatar field for the isolated wardrobe check.
                typeof(CharacterSelection).GetProperty("SelectedCharacter").SetValue(select,female?"Miss Rabbit":"Mr Rabbit");
                for(int outfit=2;outfit<4;outfit++){int before=c.Progress.coins;c.Wear(outfit);int who=female?1:0;Check(c.Progress.equipped[who]==outfit&&c.Progress.coins==before-CityController.OutfitCost(outfit),"Outfit purchase "+who+"/"+outfit);c.Wear(outfit);Check(c.Progress.coins==before-CityController.OutfitCost(outfit),"Charged twice");}
            }
            Check(voice.clips.Length>=100&&voice.clips.All(x=>x.clip!=null&&x.clip.length>.2f),"Voice assets missing");await Task.Delay(200);if(voice.Muted)voice.ToggleMute();voice.Say("water");await Task.Delay(100);Check(voice.LastKey=="water"&&voice.IsSpeaking,"Italian playback: "+voice.LastKey+"; "+voice.IsSpeaking);voice.ToggleMute();Check(!voice.IsSpeaking,"Mute did not stop audio");voice.Say("harvest");Check(!voice.IsSpeaking,"Automatic voice ignored mute");voice.Repeat();await Task.Delay(100);Check(voice.IsSpeaking&&voice.LastKey=="harvest","Explicit replay unavailable while muted");voice.Stop();
            return questions+" real exercise answers; 11 medals, 3/5 rejection, 4/5 passes, no duplicate rewards, help/uncertain ink, wall movement/reload, sow/grow/harvest/sell both crops, four outfit purchases, Italian audio/mute/replay; "+buttons+" touch raycasts.";
        }
        finally
        {
            j.Close();select.ShowSelection();JsonUtility.FromJsonOverwrite(learning,j.Progress);JsonUtility.FromJsonOverwrite(garden,g.Progress);JsonUtility.FromJsonOverwrite(city,c.Progress);JsonUtility.FromJsonOverwrite(pupil,Get<SchoolController.Progress>(school,"progress"));typeof(GardenController).GetField("dirty",F).SetValue(g,false);j.RefreshHome();j.AutoOpen=auto;if(voice.Muted!=wasMuted)voice.ToggleMute();
            foreach(var p in saved){if(p.Value.Item1)PlayerPrefs.SetString(p.Key,p.Value.Item2);else PlayerPrefs.DeleteKey(p.Key);}PlayerPrefs.Save();
        }
    }
}
