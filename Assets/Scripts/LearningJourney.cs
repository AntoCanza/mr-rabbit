using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    public sealed class LearningJourney : MonoBehaviour
    {
        public enum Page { Closed, Notebook, Exercise, Result, Wall }
        [Serializable] public sealed class Widgets
        {
            public GameObject root,notebook,exercise,result,wall,homeBoard,decoration;
            public UnityEngine.UI.Button openBook,openWall,close,italian,maths,practice,test,next,submit,erase,help,resultNext;
            public UnityEngine.UI.Button[] cards,choices,wallSlots,medalChoices;
            public UnityEngine.UI.Image[] cardMarks,pips,choiceMarks,wallMarks;
            public MedalGraphic[] cardMedals,wallMedals,trayMedals,homeMedals;
            public TMPro.TMP_Text title,question,counter,resultScore,rewardName;
            public TMPro.TMP_Text[] cardLabels,cardSymbols,cardCounts,choiceLabels,wallLabels,trayLabels,homeLabels,readback,guides;
            public GameObject[] padSlots;
            public HandwritingPad[] pads;
            public UnityEngine.UI.RawImage[] choicePictures,counts;
            public SchoolWordPicture wordPicture;
            public CitySymbol resultSymbol,feedbackSymbol,rewardSymbol;
            public MedalGraphic resultMedal;
        }
        public Widgets UI;
        public LearningProgress Progress {get;private set;}
        public LearningLesson Lesson {get;private set;}
        public Page State {get;private set;}
        public bool IsOpen=>State!=Page.Closed;
        public bool Assessment {get;private set;}
        public bool Answered=>answered;
        public int Score=>score;
        public int QuestionNumber=>number;
        public int SelectedSkill=>selected;
        public int SelectedMedal=>chosenMedal;
        public bool AutoOpen=true;
        OutdoorAdventure adventure;
        SchoolController school;
        ItalianVoice voice;
        readonly System.Random random=new System.Random();
        readonly HashSet<string> used=new HashSet<string>();
        readonly int[][] groups={new[]{0,1,10,2,3,4},new[]{5,6,7,8,9}};
        int tab,selected,number,score,practiceAnswers,chosenMedal=-1,inkRevision,lastSubmission=-1;
        bool answered,assisted,refreshing;
        public void Configure(Widgets widgets){UI=widgets;}
        void Awake()
        {
            adventure=GetComponent<OutdoorAdventure>();school=GetComponent<SchoolController>();voice=GetComponent<ItalianVoice>();
            Progress=LearningProgress.Load();LearningProgress.Current=Progress;
            UI.openBook.onClick.AddListener(OpenNotebook);UI.openWall.onClick.AddListener(OpenWall);UI.close.onClick.AddListener(Back);
            UI.italian.onClick.AddListener(()=>ChangeTab(0));UI.maths.onClick.AddListener(()=>ChangeTab(1));
            UI.practice.onClick.AddListener(()=>StartSession(false));UI.test.onClick.AddListener(()=>StartSession(true));
            UI.next.onClick.AddListener(Next);UI.submit.onClick.AddListener(()=>SubmitWriting());UI.erase.onClick.AddListener(Erase);UI.help.onClick.AddListener(Help);
            UI.resultNext.onClick.AddListener(OpenNotebook);
            for(int i=0;i<UI.cards.Length;i++){int n=i;UI.cards[i].onClick.AddListener(()=>SelectSkill(groups[tab][n]));}
            for(int i=0;i<3;i++){int n=i;UI.choices[i].onClick.AddListener(()=>AnswerChoice(n));}
            for(int i=0;i<UI.wallSlots.Length;i++){int n=i;UI.wallSlots[i].onClick.AddListener(()=>Hang(n));}
            for(int i=0;i<UI.medalChoices.Length;i++){int n=i;UI.medalChoices[i].onClick.AddListener(()=>SelectMedal(n));}
            foreach(var pad in UI.pads)pad.Changed+=InkChanged;
            SetPage(Page.Closed);RefreshHome();
        }
        void Update()
        {
            bool subjects=adventure.Location==OutdoorAdventure.Place.School&&school.State==SchoolController.Phase.Subjects;
            UI.openBook.gameObject.SetActive(subjects&&!IsOpen);UI.openBook.GetComponent<ActionCue>()?.SetHighlighted(subjects&&!IsOpen);
            UI.homeBoard.SetActive(adventure.Location==OutdoorAdventure.Place.Home);
            if(IsOpen&&((State==Page.Wall&&adventure.Location!=OutdoorAdventure.Place.Home)||(State!=Page.Wall&&adventure.Location!=OutdoorAdventure.Place.School)))Close();
            if(IsOpen&&Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame)Back();
        }
        void SetPage(Page page)
        {
            State=page;UI.root.SetActive(page!=Page.Closed);UI.notebook.SetActive(page==Page.Notebook);UI.exercise.SetActive(page==Page.Exercise);UI.result.SetActive(page==Page.Result);UI.wall.SetActive(page==Page.Wall);
            UI.title.text=page==Page.Notebook?"Prima primaria":page==Page.Wall?"Le mie medaglie":LearningProgress.Names[selected];
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
        }
        public void OpenNotebook()
        {
            if(adventure.Location!=OutdoorAdventure.Place.School||school.State!=SchoolController.Phase.Subjects)return;
            SetPage(Page.Notebook);RefreshNotebook();voice?.Say("notebook");
        }
        public void Close(){bool wall=State==Page.Wall;SetPage(Page.Closed);foreach(var p in UI.pads)p.CanWrite=false;voice?.Stop();if(wall){var r=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();if(r!=null)r.MovementEnabled=true;}}
        public void Back(){if(State==Page.Exercise||State==Page.Result)OpenNotebook();else Close();}
        public void ChangeTab(int index){if(State!=Page.Notebook||index<0||index>1)return;tab=index;selected=groups[tab][0];RefreshNotebook();}
        public void SelectSkill(int id)
        {
            if(State!=Page.Notebook||!Progress.Available(id))return;
            selected=id;tab=id<=4||id==10?0:1;RefreshNotebook();voice?.Say("skill_"+id);
        }
        void RefreshNotebook()
        {
            for(int i=0;i<UI.cards.Length;i++)
            {
                bool exists=i<groups[tab].Length;UI.cards[i].gameObject.SetActive(exists);if(!exists)continue;
                int id=groups[tab][i];bool unlocked=Progress.Available(id);
                UI.cards[i].interactable=unlocked;UI.cardLabels[i].text=LearningProgress.Names[id];UI.cardSymbols[i].text=LearningProgress.Marks[id];
                UI.cardMedals[i].earned=Progress.medals[id];UI.cardMedals[i].SetVerticesDirty();UI.cardCounts[i].text=unlocked&&!Progress.medals[id]?Progress.practice[id]+" / "+LearningProgress.PracticeTarget:"";
                UI.cards[i].transform.Find("Locked").gameObject.SetActive(!unlocked);
                UI.cardMarks[i].gameObject.SetActive(id==selected);UI.cards[i].GetComponent<ActionCue>()?.SetHighlighted(unlocked&&!Progress.medals[id]&&id==selected);
            }
            UI.italian.GetComponent<UnityEngine.UI.Image>().color=tab==0?new Color32(191,226,210,255):Color.white;
            UI.maths.GetComponent<UnityEngine.UI.Image>().color=tab==1?new Color32(191,226,210,255):Color.white;
            UI.practice.interactable=Progress.Available(selected);UI.test.interactable=Progress.CanTest(selected);
            UI.practice.GetComponent<ActionCue>()?.SetHighlighted(UI.practice.interactable&&!Progress.CanTest(selected));UI.test.GetComponent<ActionCue>()?.SetHighlighted(UI.test.interactable&&!Progress.medals[selected]);
        }
        public void StartSession(bool assessment)
        {
            if(State!=Page.Notebook||!Progress.Available(selected)||assessment&&!Progress.CanTest(selected))return;
            Assessment=assessment;number=score=practiceAnswers=0;used.Clear();SetPage(Page.Exercise);NewQuestion();
        }
        void NewQuestion()
        {
            Lesson=LearningLesson.Next(selected,random,used);answered=assisted=false;lastSubmission=-1;inkRevision=0;refreshing=true;
            UI.question.text=Lesson.Question;UI.counter.text=Assessment?(number+1)+" / "+LearningProgress.TestLength:"";
            UI.feedbackSymbol.gameObject.SetActive(false);UI.next.gameObject.SetActive(false);UI.help.gameObject.SetActive(true);
            bool picture=UI.wordPicture.Show(Lesson.Word);UI.wordPicture.Picture.transform.parent.gameObject.SetActive(picture);
            bool writing=Lesson.Writing;
            var pictureRect=(RectTransform)UI.wordPicture.transform;pictureRect.anchorMin=writing?new Vector2(.07f,.25f):new Vector2(.34f,.43f);pictureRect.anchorMax=writing?new Vector2(.39f,.65f):new Vector2(.66f,.73f);pictureRect.offsetMin=pictureRect.offsetMax=Vector2.zero;
            for(int i=0;i<2;i++)
            {
                bool show=writing&&i<Lesson.Answer.Length;UI.padSlots[i].SetActive(show);
                var rect=(RectTransform)UI.padSlots[i].transform;
                float left=Lesson.Answer.Length==1?.50f:.43f+i*.20f;
                rect.anchorMin=new Vector2(left,.25f);rect.anchorMax=new Vector2(left+(Lesson.Answer.Length==1?.29f:.18f),.64f);rect.offsetMin=rect.offsetMax=Vector2.zero;
                UI.pads[i].Vowels=Lesson.Vowels;UI.pads[i].Alphabet=Lesson.Alphabet;UI.pads[i].CanWrite=show;UI.pads[i].Clear();UI.readback[i].text="";
                UI.guides[i].text=show?Lesson.Answer[i].ToString():"";UI.guides[i].gameObject.SetActive(false);
            }
            UI.submit.gameObject.SetActive(writing);UI.submit.interactable=false;UI.erase.gameObject.SetActive(writing);
            for(int i=0;i<3;i++)
            {
                UI.choices[i].gameObject.SetActive(!writing);UI.choiceLabels[i].text=!writing?Lesson.Choices[i]:"";UI.choiceMarks[i].gameObject.SetActive(false);UI.choices[i].interactable=true;
                bool draw=!writing&&Lesson.PictureChoices!=null;
                UI.choicePictures[i].gameObject.SetActive(draw);UI.choiceLabels[i].gameObject.SetActive(!draw);
                if(draw)ApplyWord(UI.choicePictures[i],Lesson.PictureChoices[i]);
            }
            int count=Lesson.Kind==LearningLesson.Layout.Count?Lesson.Left:Lesson.Kind==LearningLesson.Layout.Sum?Lesson.Left+Lesson.Right:0;
            for(int i=0;i<UI.counts.Length;i++)
            {
                UI.counts[i].transform.parent.gameObject.SetActive(i<count);
                float x=.08f+(i%5)*.063f,y=.55f-(i/5)*.085f;
                if(Lesson.Kind==LearningLesson.Layout.Sum){int at=i<Lesson.Left?i:i-Lesson.Left;x=(i<Lesson.Left?.07f:.255f)+(at%3)*.047f;y=.55f-(at/3)*.054f;UI.counts[i].color=i<Lesson.Left?Color.white:new Color(.82f,1,.86f);}
                var r=(RectTransform)UI.counts[i].transform.parent;r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(x+.047f,y+.065f);r.offsetMin=r.offsetMax=Vector2.zero;
            }
            for(int i=0;i<UI.pips.Length;i++){UI.pips[i].gameObject.SetActive(Assessment);UI.pips[i].color=i<number?(i<score?new Color32(92,167,126,255):new Color32(222,171,123,255)):new Color32(224,220,202,255);}
            refreshing=false;voice?.Say(Lesson.Voice);
        }
        public void ApplyWord(UnityEngine.UI.RawImage target,string word)
        {
            foreach(var e in school.UI.wordPicture.Entries)if(e.word==word){target.texture=e.atlas;target.uvRect=e.uv;return;}
        }
        public bool AnswerChoice(int index)
        {if(State!=Page.Exercise||Lesson.Writing||answered||index<0||index>2)return false;return Evaluate(Lesson.Choices[index]==Lesson.Answer);}
        public bool SubmitWriting()
        {
            if(State!=Page.Exercise||!Lesson.Writing||answered||lastSubmission==inkRevision)return false;
            string answer="";for(int i=0;i<Lesson.Answer.Length;i++)
            {
                if(UI.pads[i].PointCount==0)return false;char c=UI.pads[i].Recognized;
                if(c=='\0'){lastSubmission=inkRevision;Feedback(false);voice?.Say("rewrite");return false;}answer+=c;
            }
            lastSubmission=inkRevision;return Evaluate(answer==Lesson.Answer);
        }
        bool Evaluate(bool correct)
        {
            Feedback(correct);
            if(!Assessment&&!correct){voice?.Say("retry");return false;}
            answered=true;if(correct){score++;practiceAnswers++;if(!Assessment&&!assisted)Progress.RecordPractice(selected);}
            if(Assessment||correct)
            {
                UI.next.gameObject.SetActive(true);UI.submit.interactable=false;UI.help.gameObject.SetActive(false);
                foreach(var pad in UI.pads)pad.CanWrite=false;
                for(int i=0;i<3;i++){UI.choices[i].interactable=false;UI.choiceMarks[i].gameObject.SetActive(Lesson.Choices!=null&&i==Lesson.CorrectIndex);}
            }
            voice?.Say(correct?"correct":"retry");return correct;
        }
        void Feedback(bool correct){UI.feedbackSymbol.gameObject.SetActive(true);UI.feedbackSymbol.symbol=correct?CitySymbol.Kind.Check:CitySymbol.Kind.Back;UI.feedbackSymbol.SetAllDirty();}
        void InkChanged()
        {
            if(refreshing||State!=Page.Exercise||Lesson==null||answered)return;inkRevision++;
            bool filled=true;for(int i=0;i<Lesson.Answer.Length;i++){char c=UI.pads[i].Recognized;UI.readback[i].text=UI.pads[i].PointCount==0?"":c=='\0'?"?":c.ToString();filled&=UI.pads[i].PointCount>0;}
            UI.submit.interactable=filled;UI.feedbackSymbol.gameObject.SetActive(false);
        }
        public void Erase(){if(State!=Page.Exercise||answered)return;foreach(var pad in UI.pads)pad.Clear();}
        public void Help()
        {
            if(State!=Page.Exercise||answered)return;
            // Asking for a model turns this into free practice, never an assisted exam pass.
            if(Assessment){Assessment=false;score=number=practiceAnswers=0;UI.counter.text="";foreach(var p in UI.pips)p.gameObject.SetActive(false);}
            assisted=true;foreach(var guide in UI.guides)guide.gameObject.SetActive(Lesson.Writing);
            if(Lesson.Choices!=null)UI.choiceMarks[Lesson.CorrectIndex].gameObject.SetActive(true);voice?.Say("help");
        }
        public void Next()
        {
            if(State!=Page.Exercise||!answered)return;number++;
            if(Assessment&&number>=LearningProgress.TestLength){Finish();return;}
            if(!Assessment&&practiceAnswers>=LearningProgress.PracticeTarget){OpenNotebook();voice?.Say(Progress.CanTest(selected)?"ready":"practice");return;}
            NewQuestion();
        }
        void Finish()
        {
            bool earned=Progress.CompleteTest(selected,score);bool passed=score>=LearningProgress.PassScore;
            SetPage(Page.Result);UI.resultMedal.gameObject.SetActive(passed);UI.resultMedal.earned=passed;UI.resultMedal.SetVerticesDirty();
            UI.resultSymbol.gameObject.SetActive(!passed);UI.resultSymbol.symbol=CitySymbol.Kind.Back;UI.resultSymbol.SetAllDirty();UI.resultScore.text=score+" / "+LearningProgress.TestLength;
            UI.rewardName.text=passed?LearningProgress.Names[selected]:"Riproviamo";
            UI.rewardSymbol.gameObject.SetActive(earned&&(selected==0||selected==8||selected==9||selected==10||selected==4));
            UI.rewardSymbol.symbol=selected==0||selected==8?CitySymbol.Kind.Garden:selected==4?CitySymbol.Kind.Home:CitySymbol.Kind.Shirt;UI.rewardSymbol.SetAllDirty();
            RefreshHome();voice?.Say(passed?(earned?"medal_"+selected:"passed"):"practice");
        }
        public void OpenWall()
        {
            if(adventure.Location!=OutdoorAdventure.Place.Home)return;
            var walker=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();walker?.StopWalking();
            UnityEngine.Object.FindFirstObjectByType<HomeActivities>()?.CancelActivity();
            if(walker!=null)walker.MovementEnabled=false;
            SetPage(Page.Wall);chosenMedal=-1;RefreshWall();voice?.Say("wall");
        }
        public void SelectMedal(int id)
        {if(State!=Page.Wall||id<0||id>=LearningProgress.SkillCount||!Progress.medals[id])return;chosenMedal=id;RefreshWall();voice?.Say("hang");}
        public bool Hang(int slot)
        {
            if(State!=Page.Wall)return false;
            if(chosenMedal<0){if(slot>=0&&slot<Progress.wall.Length&&Progress.wall[slot]>=0)SelectMedal(Progress.wall[slot]);return false;}
            if(!Progress.Hang(chosenMedal,slot))return false;chosenMedal=-1;RefreshWall();RefreshHome();voice?.Say("hung");return true;
        }
        void RefreshWall()
        {
            for(int i=0;i<UI.wallSlots.Length;i++)
            {
                int id=Progress.wall[i];UI.wallMedals[i].gameObject.SetActive(id>=0);UI.wallLabels[i].text=id>=0?LearningProgress.Marks[id]:"";
                UI.wallMarks[i].gameObject.SetActive(chosenMedal>=0);UI.wallSlots[i].GetComponent<ActionCue>()?.SetHighlighted(chosenMedal>=0);
            }
            for(int i=0;i<LearningProgress.SkillCount;i++)
            {UI.medalChoices[i].interactable=Progress.medals[i];UI.trayMedals[i].earned=Progress.medals[i];UI.trayMedals[i].SetVerticesDirty();UI.trayLabels[i].text=LearningProgress.Marks[i];UI.medalChoices[i].GetComponent<ActionCue>()?.SetHighlighted(Progress.medals[i]&&i==chosenMedal);}
        }
        public void RefreshHome()
        {
            for(int i=0;i<UI.homeMedals.Length;i++){int id=Progress.wall[i];UI.homeMedals[i].gameObject.SetActive(id>=0);UI.homeLabels[i].text=id>=0?LearningProgress.Marks[id]:"";}
            bool any=false;foreach(int id in Progress.wall)any|=id>=0;var invite=UI.homeBoard.transform.Find("Hang A Medal Holder");if(invite!=null)invite.gameObject.SetActive(!any);
            UI.decoration.SetActive(Progress.medals[4]);
        }
        void OnApplicationPause(bool pause){if(pause)Progress?.Store();}
        void OnDestroy(){foreach(var pad in UI.pads)if(pad!=null)pad.Changed-=InkChanged;if(LearningProgress.Current==Progress)LearningProgress.Current=null;}
    }
}
