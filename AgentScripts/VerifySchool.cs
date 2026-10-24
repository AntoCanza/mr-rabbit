using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using MrRabbit;

public static class VerifySchool
{
    static void Check(bool b,string message){if(!b)throw new Exception(message);}
    public static string Logic()
    {
        var lesson=new SchoolLesson(12345);int questions=0;
        foreach(SchoolLesson.Subject mode in Enum.GetValues(typeof(SchoolLesson.Subject)))
        {
            string last=null;var seen=new HashSet<string>();
            for(int i=0;i<500;i++)
            {
                lesson.Next(mode);Check(last!=lesson.Question,"Immediate repeat");last=lesson.Question;seen.Add(last);
                Check(lesson.IsCorrect(lesson.Answer)&&!lesson.IsCorrect("X"),"Answer validation");
                if(mode==SchoolLesson.Subject.Vowels)Check(lesson.Question.Replace("_",lesson.Answer)==lesson.CompleteWord&&"AEIOU".Contains(lesson.Answer),"Vowel replacement");
                else{int n=int.Parse(lesson.Answer);Check(n>=0&&n<=10,"Natural number range");Check(n==lesson.Left+lesson.Right&&!lesson.Question.Contains("−"),"Addition only");}
                questions++;
            }
            Check(seen.Count>20,"Question variety");
        }
        int recognized=0;
        foreach(var glyph in HandwritingRecognizer.Templates)
        {
            var ink=glyph.Strokes.Reverse().Select(stroke=>stroke.Reverse().Select((p,i)=>new Vector2(p.x*.72f+.11f+Mathf.Sin(i*2.3f)*.008f,p.y*.68f+.12f+Mathf.Cos(i*1.7f)*.008f)).ToList()).ToList();
            var c=HandwritingRecognizer.Recognize(ink,char.IsLetter(glyph.Symbol),out _);
            Check(c==glyph.Symbol,"Perturbed/reversed glyph "+glyph.Symbol+" got "+c);recognized++;
        }
        Check(HandwritingRecognizer.Recognize(new List<List<Vector2>>(),true,out _)=='\0',"Blank accepted");
        var scribble=new List<List<Vector2>>{Enumerable.Range(0,150).Select(i=>new Vector2(i%2==0?.1f:.9f,.1f+i/150f*.8f)).ToList()};
        Check(HandwritingRecognizer.Recognize(scribble,false,out _)=='\0',"Scribble accepted");
        return questions+" random questions; "+recognized+" distorted, reversed and reordered glyphs; blank/scribble rejection passed";
    }
    public static async Task<string> Run()
    {
        Check(Application.isPlaying,"Enter Play");
        var saved=PlayerPrefs.GetString(SchoolController.SaveKey,"");
        var s=UnityEngine.Object.FindFirstObjectByType<SchoolController>();
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var choice=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var walker=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        var field=typeof(SchoolController).GetField("progress",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
        var progress=JsonUtility.ToJson(field.GetValue(s));
        try
        {
            foreach(bool female in new[]{false,true})
            {
                choice.ShowSelection();await Task.Delay(100);if(female)choice.ChooseFemale();else choice.ChooseMale();
                s.PackAtHome();s.Pack(0);s.Pack(1);s.Pack(2);Check(s.Packed==7,"Pack kit");
                a.WalkOutOfHome();for(int i=0;i<3000&&a.Location==OutdoorAdventure.Place.Home;i++)walker.Advance(Vector2.zero,.02f);
                a.ChooseRollers();s.GoToSchool();for(int i=0;i<3000&&a.Location==OutdoorAdventure.Place.Meadow;i++)a.Advance(0,.02f);
                Check(s.State==SchoolController.Phase.Desk,"Reach school");
                Check(s.UI.deskCue.Highlighted,"Seat cue missing");
                Check(Mathf.Abs(a.Position-SchoolController.SchoolX)<.03f,"School position");
                s.Sit();s.Advance(2);Check(s.State==SchoolController.Phase.Listen&&s.UI.seated.gameObject.activeSelf,"Seated");
                Check(s.UI.listenCue.Highlighted&&!s.UI.desk.gameObject.activeSelf,"Listening cue");
                Check(s.UI.seated.sprite==(female?s.UI.femaleSeated:s.UI.maleSeated),"Selected pupil");
                s.Listen();s.Advance(2);
                Check(s.UI.lessons.Length==2&&s.UI.lessonCues.Length==2,"School must offer exactly two activities");
                s.ChooseSubject(2);Check(s.State==SchoolController.Phase.Subjects,"Removed activity was accessible");
                for(int subject=0;subject<2;subject++)
                {
                    s.ChooseSubject(subject);for(int i=0;i<200;i++)s.Advance(.03f);
                    Check(s.State==SchoolController.Phase.Paper&&s.UI.delivered.All(x=>x.gameObject.activeSelf),"Teacher distributed sheets");
                    Check(s.UI.paperCue.Highlighted,"Worksheet cue");
                    await Task.Delay(150);await Tap(s.UI.paper.gameObject);await Task.Delay(150);
                    Check(s.State==SchoolController.Phase.Writing,"Paper touch");
                    var pad=s.UI.pads[0];Draw(pad,HandwritingRecognizer.Templates[0]);Check(pad.PointCount==0,"Pencil gate");
                    await Tap(s.UI.pencil.gameObject);
                    int stars=s.Stars;Check(!s.Submit()&&s.Stars==stars,"Blank rewarded");
                    char wrong=subject==0?(s.Lesson.Answer=="A"?'E':'A'):(s.Lesson.Answer=="0"?'1':'0');
                    Draw(pad,HandwritingRecognizer.Templates.First(x=>x.Symbol==wrong));
                    Check(!s.Submit()&&s.Stars==stars,"Wrong rewarded");await Tap(s.UI.erase.gameObject);
                    Check(pad.PointCount==0,"Eraser touch");
                    for(int i=0;i<s.Lesson.Answer.Length;i++)
                    {
                        var target=s.UI.pads[i];var e=Pointer(target,new Vector2(.5f,.5f));var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
                        Check(hits.Count>0&&hits[0].gameObject==target.gameObject,"Blank ink pad raycast blocked");
                        Draw(target,HandwritingRecognizer.Templates.First(x=>x.Symbol==s.Lesson.Answer[i]));
                        Check(target.Recognized==s.Lesson.Answer[i],"Finger recognition");
                    }
                    await Tap(s.UI.submit.gameObject);Check(s.State==SchoolController.Phase.Reward&&s.Stars==stars+1,"Reward touch");
                    Check(!s.Submit()&&s.Stars==stars+1,"Duplicate reward");
                    s.ClosePaper();
                }
                s.Leave();Check(a.Location==OutdoorAdventure.Place.Meadow&&!s.UI.view.activeSelf,"Return to meadow");
            }
            return "Both pupils; packing, school walking, seating/listening, vowels and addition only, teacher delivery, actual UI raycasts and finger events, erase, wrong/blank rejection, single rewards, exit passed at "+Screen.width+"x"+Screen.height;
        }
        finally
        {
            field.SetValue(s,JsonUtility.FromJson<SchoolController.Progress>(progress));
            if(saved.Length==0)PlayerPrefs.DeleteKey(SchoolController.SaveKey);else PlayerPrefs.SetString(SchoolController.SaveKey,saved);PlayerPrefs.Save();
        }
    }
    static PointerEventData Pointer(HandwritingPad pad,Vector2 p)
    {
        var r=pad.rectTransform;var local=new Vector2(r.rect.x+p.x*r.rect.width,r.rect.y+p.y*r.rect.height);
        return new PointerEventData(EventSystem.current){pointerId=91,button=PointerEventData.InputButton.Left,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(local))};
    }
    static void Draw(HandwritingPad pad,HandwritingRecognizer.Glyph glyph)
    {
        foreach(var stroke in glyph.Strokes)
        {
            ExecuteEvents.Execute(pad.gameObject,Pointer(pad,stroke[0]),ExecuteEvents.pointerDownHandler);
            foreach(var p in stroke)ExecuteEvents.Execute(pad.gameObject,Pointer(pad,p),ExecuteEvents.dragHandler);
            ExecuteEvents.Execute(pad.gameObject,Pointer(pad,stroke[stroke.Length-1]),ExecuteEvents.pointerUpHandler);
        }
    }
    static async Task Tap(GameObject target)
    {
        Canvas.ForceUpdateCanvases();await Task.Delay(100);
        var r=(RectTransform)target.transform;var e=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center)),button=PointerEventData.InputButton.Left};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);
        Check(hits.Count>0&&ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject)==target,"Touch blocked: "+target.name);
        ExecuteEvents.Execute(target,e,ExecuteEvents.pointerClickHandler);
    }
}
