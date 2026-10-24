using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using MrRabbit;

public static class VerifyWordPictures
{
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);}
    public static string Vocabulary()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<SchoolController>(FindObjectsInactive.Include);
        var art=s.UI.wordPicture;
        string[] words=(string[])typeof(SchoolLesson).GetField("Words",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
        Check(art.Entries.Count==words.Length&&art.Entries.Select(e=>e.word).Distinct().Count()==words.Length,"Vocabulary coverage");
        foreach(string word in words)
        {
            Check(art.Show(word)&&art.ShownWord==word&&art.Picture.texture!=null,"Missing picture: "+word);
            var uv=art.Picture.uvRect;Check(uv.xMin>=-.001f&&uv.yMin>=-.001f&&uv.xMax<=1.001f&&uv.yMax<=1.001f,"UV bounds");
            Check(!art.Picture.raycastTarget,"Illustration must not intercept finger writing");
        }
        var lesson=new SchoolLesson(71);
        for(int i=0;i<400;i++){lesson.Next(SchoolLesson.Subject.Vowels);Check(art.Show(lesson.CompleteWord),"Random question lost illustration");}
        lesson.Next(SchoolLesson.Subject.Addition);Check(!art.Show(lesson.CompleteWord)&&!art.gameObject.activeSelf&&art.Picture.texture==null,"Stale image in mathematics");
        return "All 24 words mapped to unique cells; 400 random vowel questions covered; arithmetic hides/clears pictures";
    }
    public static async Task<string> Run()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<SchoolController>();
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>();
        var c=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();
        var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        c.ShowSelection();c.ChooseFemale();a.WalkOutOfHome();for(int i=0;i<2000&&a.Location==OutdoorAdventure.Place.Home;i++)r.Advance(Vector2.zero,.02f);
        a.ChooseRollers();s.GoToSchool();for(int i=0;i<2000&&a.Location==OutdoorAdventure.Place.Meadow;i++)a.Advance(0,.02f);
        if(s.State==SchoolController.Phase.Packing){s.Pack(0);s.Pack(1);s.Pack(2);}
        s.Sit();s.Advance(2);s.Listen();s.Advance(2);
        foreach(int subject in new[]{0,1,0})
        {
            s.ChooseSubject(subject);for(int i=0;i<200;i++)s.Advance(.03f);s.OpenPaper();await Task.Delay(180);
            var art=s.UI.wordPicture;
            Check(art.gameObject.activeInHierarchy==(subject==0),"Picture visibility");
            if(subject==0)
            {
                Check(art.ShownWord==s.Lesson.CompleteWord,"Picture/word mismatch");
                var corners=new Vector3[4];((RectTransform)art.transform).GetWorldCorners(corners);
                var padCorners=new Vector3[4];s.UI.pads[0].rectTransform.GetWorldCorners(padCorners);
                Check(corners[2].x<padCorners[0].x,"Picture overlaps handwriting");
                var labelCorners=new Vector3[4];s.UI.readback[0].rectTransform.GetWorldCorners(labelCorners);
                var helpCorners=new Vector3[4];((RectTransform)s.UI.help.transform).GetWorldCorners(helpCorners);
                Check(labelCorners[0].y>helpCorners[2].y,"Recognized letter overlaps help icon");
            }
            else Check(Mathf.Abs(((RectTransform)s.UI.padSlots[0].transform).anchorMin.x-(s.Lesson.Answer.Length==1?.335f:.16f))<.001f,"Math layout changed");
            s.ClosePaper();
        }
        return "Vowels/math/vowels transitions, correct semantic image, image/ink separation and unchanged math layout passed at "+Screen.width+"x"+Screen.height;
    }
}
