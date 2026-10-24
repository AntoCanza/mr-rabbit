using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class SchoolController : MonoBehaviour
    {
        public enum Phase { Outside, Packing, Desk, Walking, Listen, Listening, Subjects, Delivering, Paper, Writing, Reward }
        [Serializable] public sealed class Progress {public int version=1,packed,stars;}
        [Serializable] public sealed class Widgets
        {
            public GameObject view,kitPanel,worksheet,subjects,reward;
            public UnityEngine.UI.Button entrance,sign,homeBag,exit,kitClose,desk,listen,paper,pencil,erase,submit,help,back,next;
            public UnityEngine.UI.Button[] kit,lessons;
            public UnityEngine.UI.Image[] kitChecks,delivered;
            public UnityEngine.UI.Image teacher,standing,seated,schoolPainting;
            public UnityEngine.UI.RawImage backpack,outdoorBackpack;
            public RectTransform teacherRect,standingRect,seatedRect;
            public TMPro.TMP_Text question,board,stars;
            public SchoolWordPicture wordPicture;
            public TMPro.TMP_Text[] readback,guides;
            public HandwritingPad[] pads;
            public GameObject[] padSlots;
            public Sprite maleSeated,femaleSeated,femaleStanding;
            public ActionCue[] kitCues,lessonCues;
            public ActionCue deskCue,listenCue,paperCue,pencilCue,submitCue,eraseCue,rewardCue,signCue;
        }
        public const string SaveKey="MrRabbit.School.v1";
        public const float SchoolX=-2.55f;
        [SerializeField] OutdoorAdventure adventure;
        [SerializeField] DayNightCycle clock;
        [SerializeField] Widgets ui;
        Progress progress;
        SchoolLesson lesson;
        Phase phase;
        float timer,actorX=.13f,teacherX=.16f;
        int delivery;
        bool goInsideAfterPacking,penSelected,hint,answered;
        bool usedHelp;
        int failedAttempts,inkRevision,lastSubmittedRevision=-1;
        SchoolFeedback feedback;
        public int FailedAttempts=>failedAttempts;
        public bool UsedHelp=>usedHelp;
        SchoolLesson.Subject subject;
        public Phase State=>phase;
        public int Stars=>progress.stars;
        public int Packed=>progress.packed;
        public SchoolLesson Lesson=>lesson;
        public Widgets UI=>ui;
        public void Configure(OutdoorAdventure travel,DayNightCycle dayNight,Widgets widgets)
        {adventure=travel;clock=dayNight;ui=widgets;}
        void Awake()
        {
            try{progress=JsonUtility.FromJson<Progress>(PlayerPrefs.GetString(SaveKey,""));}catch(ArgumentException){}
            if(progress==null||progress.version!=1)progress=new Progress();
            progress.packed&=7;progress.stars=Mathf.Clamp(progress.stars,0,1000000);
            lesson=new SchoolLesson(Environment.TickCount);
            feedback=GetComponent<SchoolFeedback>();
            adventure.HouseReached+=Arrived;
            ui.entrance.onClick.AddListener(GoToSchool);ui.sign.onClick.AddListener(GoToSchool);
            ui.homeBag.onClick.AddListener(PackAtHome);ui.kitClose.onClick.AddListener(ClosePacking);
            ui.exit.onClick.AddListener(Leave);ui.desk.onClick.AddListener(Sit);ui.listen.onClick.AddListener(Listen);
            ui.paper.onClick.AddListener(OpenPaper);ui.pencil.onClick.AddListener(SelectPen);
            ui.erase.onClick.AddListener(Erase);ui.submit.onClick.AddListener(()=>Submit());ui.help.onClick.AddListener(ToggleHelp);
            ui.back.onClick.AddListener(ClosePaper);ui.next.onClick.AddListener(Next);
            for(int i=0;i<3;i++){int k=i;ui.kit[i].onClick.AddListener(()=>Pack(k));}
            for(int i=0;i<ui.lessons.Length;i++){int k=i;ui.lessons[i].onClick.AddListener(()=>ChooseSubject(k));}
            foreach(var pad in ui.pads)pad.Changed+=InkChanged;
            ui.view.SetActive(false);ui.kitPanel.SetActive(false);ui.worksheet.SetActive(false);ui.reward.SetActive(false);
            UpdateKit();RefreshCues();
        }
        void Update()
        {
            if(phase!=Phase.Outside&&phase!=Phase.Packing&&adventure.Location!=OutdoorAdventure.Place.School)
                ResetRoom();
            if(phase==Phase.Packing&&adventure.Location==OutdoorAdventure.Place.Selection)ClosePacking();
            ui.sign.gameObject.SetActive(adventure.Location==OutdoorAdventure.Place.Meadow);
            ui.homeBag.gameObject.SetActive(adventure.Location==OutdoorAdventure.Place.Home);
            ui.outdoorBackpack.gameObject.SetActive(progress.packed==7);
            if(ui.schoolPainting!=null)ui.schoolPainting.color=Color.Lerp(Color.white,new Color(.29f,.38f,.62f),clock.NightAmount);
            Advance(Time.deltaTime);
        }
        public void GoToSchool(){if(adventure.Location==OutdoorAdventure.Place.Meadow)adventure.WalkToHouse(3,SchoolX);}
        void Arrived(int index)
        {
            if(index!=3||adventure.Location!=OutdoorAdventure.Place.Meadow)return;
            adventure.BeginSchool();
            if(progress.packed!=7){goInsideAfterPacking=true;phase=Phase.Packing;ui.kitPanel.SetActive(true);UpdateKit();RefreshCues();}
            else EnterRoom();
        }
        public void PackAtHome()
        {
            if(adventure.Location!=OutdoorAdventure.Place.Home)return;
            var player=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>();
            if(player!=null){player.GetComponentInParent<HomeActivities>()?.CancelActivity();player.transform.parent.GetComponentInChildren<BedInteraction>(true)?.Wake();player.StopWalking();}
            goInsideAfterPacking=false;phase=Phase.Packing;ui.kitPanel.SetActive(true);UpdateKit();RefreshCues();
        }
        public void Pack(int item)
        {
            if(phase!=Phase.Packing||item<0||item>2||(item>0&&(progress.packed&1)==0))return;
            progress.packed|=1<<item;Save();UpdateKit();RefreshCues();
            if(progress.packed==7){ui.kitPanel.SetActive(false);if(goInsideAfterPacking)EnterRoom();else{phase=Phase.Outside;RefreshCues();}}
            ClearFocus();
        }
        public void ClosePacking()
        {
            if(phase!=Phase.Packing)return;
            ui.kitPanel.SetActive(false);phase=Phase.Outside;
            if(goInsideAfterPacking)adventure.EndSchool();goInsideAfterPacking=false;RefreshCues();
        }
        void EnterRoom()
        {
            ui.kitPanel.SetActive(false);ui.view.SetActive(true);ui.worksheet.SetActive(false);ui.reward.SetActive(false);ui.subjects.SetActive(false);
            ui.standing.sprite=adventure.PlayerSprite;ui.seated.sprite=adventure.GetComponent<CharacterSelection>().SelectedCharacter=="Miss Rabbit"?ui.femaleSeated:ui.maleSeated;
            var city=adventure.GetComponent<CityController>();if(city!=null)ui.seated.sprite=city.SeatedSprite(ui.seated.sprite);
            ui.standing.gameObject.SetActive(true);ui.seated.gameObject.SetActive(false);ui.backpack.gameObject.SetActive(true);
            actorX=.13f;teacherX=.16f;phase=Phase.Desk;timer=0;
            foreach(var sheet in ui.delivered)sheet.gameObject.SetActive(false);
            ui.board.text="A E I O U";PlaceActors();RefreshCues();
        }
        public void Sit()
        {
            if(phase!=Phase.Desk)return;
            phase=Phase.Walking;timer=0;RefreshCues();ClearFocus();
        }
        public void Listen()
        {if(phase!=Phase.Listen)return;phase=Phase.Listening;timer=0;feedback?.Play(SchoolFeedback.Sound.Listen);RefreshCues();ClearFocus();}
        public void ChooseSubject(int index)
        {
            if(phase!=Phase.Subjects||index<0||index>1)return;
            GetComponent<LearningJourney>()?.Close();
            subject=(SchoolLesson.Subject)index;NewExercise();
        }
        void NewExercise()
        {
            lesson.Next(subject);answered=false;penSelected=false;hint=false;
            usedHelp=false;failedAttempts=0;inkRevision=0;lastSubmittedRevision=-1;feedback?.ResetFeedback();
            ui.subjects.SetActive(false);ui.reward.SetActive(false);ui.worksheet.SetActive(false);
            ui.board.text=subject==SchoolLesson.Subject.Vowels?"A E I O U":"1 + 1 = 2";
            foreach(var sheet in ui.delivered){sheet.gameObject.SetActive(false);sheet.color=Color.white;}
            delivery=0;timer=0;teacherX=.16f;phase=Phase.Delivering;RefreshCues();ClearFocus();
        }
        public void Advance(float seconds)
        {
            if(adventure.Location!=OutdoorAdventure.Place.School)return;
            seconds=Mathf.Max(0,seconds);timer+=seconds;
            if(phase==Phase.Walking)
            {
                actorX=Mathf.MoveTowards(actorX,.28f,seconds*.15f);
                ui.standingRect.localRotation=Quaternion.Euler(0,0,Mathf.Sin(timer*10)*2);
                if(Mathf.Approximately(actorX,.28f))
                {ui.standing.gameObject.SetActive(false);ui.seated.gameObject.SetActive(true);phase=Phase.Listen;timer=0;RefreshCues();}
            }
            else if(phase==Phase.Listening)
            {
                ui.teacherRect.localRotation=Quaternion.Euler(0,0,Mathf.Sin(timer*5)*3);
                if(timer>=1.5f){phase=Phase.Subjects;ui.subjects.SetActive(true);ui.teacherRect.localRotation=Quaternion.identity;RefreshCues();var journey=GetComponent<LearningJourney>();if(journey!=null&&journey.AutoOpen)journey.OpenNotebook();}
            }
            else if(phase==Phase.Delivering)
            {
                // Visit each desk; sheets appear only as the teacher reaches that pupil.
                float target=.30f+delivery*.235f;teacherX=Mathf.MoveTowards(teacherX,target,seconds*.40f);
                ui.teacherRect.localRotation=Quaternion.Euler(0,0,Mathf.Sin(timer*9)*2);
                if(Mathf.Approximately(teacherX,target))
                {
                    ui.delivered[delivery].gameObject.SetActive(true);delivery++;
                    if(delivery==3){phase=Phase.Paper;ui.teacherRect.localRotation=Quaternion.identity;RefreshCues();}
                }
            }
            else if(phase==Phase.Reward)
            {
                teacherX=Mathf.MoveTowards(teacherX,.30f,seconds*.40f);
                ui.seatedRect.localRotation=Quaternion.Euler(0,0,Mathf.Sin(timer*7)*2);
            }
            PlaceActors();
        }
        void PlaceActors()
        {
            SetX(ui.standingRect,actorX,.13f);SetX(ui.teacherRect,teacherX,.15f);
        }
        static void SetX(RectTransform r,float x,float width)
        {r.anchorMin=new Vector2(x-width*.5f,r.anchorMin.y);r.anchorMax=new Vector2(x+width*.5f,r.anchorMax.y);r.offsetMin=r.offsetMax=Vector2.zero;}
        public void OpenPaper()
        {
            if(phase!=Phase.Paper)return;
            phase=Phase.Writing;penSelected=false;ui.worksheet.SetActive(true);ui.question.text=lesson.Question;
            bool illustrated=ui.wordPicture!=null&&ui.wordPicture.Show(lesson.CompleteWord);
            for(int i=0;i<2;i++)
            {
                ui.padSlots[i].SetActive(i<lesson.Answer.Length);
                var slot=(RectTransform)ui.padSlots[i].transform;
                float left=illustrated?.57f:lesson.Answer.Length==1?.335f:.16f+i*.35f;
                slot.anchorMin=new Vector2(left,.20f);slot.anchorMax=new Vector2(left+.33f,.76f);
                slot.offsetMin=slot.offsetMax=Vector2.zero;
                var readback=ui.readback[i].rectTransform;
                readback.anchorMin=illustrated?new Vector2(.28f,1.01f):new Vector2(.28f,-.14f);
                readback.anchorMax=illustrated?new Vector2(.72f,1.10f):new Vector2(.72f,-.015f);
                readback.offsetMin=readback.offsetMax=Vector2.zero;
                ui.pads[i].Vowels=subject==SchoolLesson.Subject.Vowels;ui.pads[i].Alphabet=false;ui.pads[i].CanWrite=false;ui.pads[i].Clear();
                ui.guides[i].text=i<lesson.Answer.Length?lesson.Answer[i].ToString():"";ui.guides[i].gameObject.SetActive(false);
            }
            RefreshCues();ClearFocus();
        }
        public void SelectPen()
        {
            if(phase!=Phase.Writing)return;penSelected=true;
            foreach(var pad in ui.pads)pad.CanWrite=true;RefreshCues();ClearFocus();
        }
        public void Erase()
        {if(phase!=Phase.Writing)return;foreach(var pad in ui.pads)pad.Clear();RefreshCues();ClearFocus();}
        public void ToggleHelp()
        {
            if(phase!=Phase.Writing||failedAttempts<4)return;
            if(!usedHelp)
            {
                usedHelp=true;feedback?.Play(SchoolFeedback.Sound.Help);
            }
            hint=!hint;foreach(var guide in ui.guides)guide.gameObject.SetActive(hint);
            RefreshCues();ClearFocus();
        }
        void InkChanged()
        {
            if(phase!=Phase.Writing)return;
            inkRevision++;
            for(int i=0;i<2;i++)
            {
                char c=ui.pads[i].Recognized;
                ui.readback[i].text=ui.pads[i].PointCount==0?"":c=='\0'?"?":c.ToString();
            }
            RefreshCues();
        }
        public bool Submit()
        {
            if(phase!=Phase.Writing||!penSelected||answered)return false;
            for(int i=0;i<lesson.Answer.Length;i++)if(ui.pads[i].PointCount==0)return false;
            if(lastSubmittedRevision==inkRevision)return false;
            lastSubmittedRevision=inkRevision;
            string answer="";
            for(int i=0;i<lesson.Answer.Length;i++)
            {char c=ui.pads[i].Recognized;if(c=='\0'){GetComponent<ItalianVoice>()?.Say("rewrite");feedback?.Play(SchoolFeedback.Sound.Retry);return false;}answer+=c;}
            if(!lesson.IsCorrect(answer)){Retry();return false;}
            answered=true;if(!usedHelp)progress.stars=Mathf.Min(1000000,progress.stars+1);Save();
            feedback?.Complete(usedHelp);
            ui.worksheet.SetActive(false);ui.reward.SetActive(true);ui.stars.text=progress.stars.ToString();
            foreach(var sheet in ui.delivered)sheet.color=new Color32(255,221,107,255);
            ui.board.text=subject==SchoolLesson.Subject.Vowels?lesson.CompleteWord:lesson.Question.Replace("?",lesson.Answer);
            phase=Phase.Reward;timer=0;RefreshCues();return true;
        }
        void Retry()
        {
            failedAttempts++;RefreshCues();feedback?.Play(SchoolFeedback.Sound.Retry);
            ui.question.color=new Color32(176,101,45,255);ui.eraseCue.SetHighlighted(true);ClearFocus();
        }
        public void Next(){if(phase==Phase.Reward)NewExercise();}
        public void ClosePaper()
        {
            if(phase!=Phase.Writing&&phase!=Phase.Reward&&phase!=Phase.Paper)return;
            ui.worksheet.SetActive(false);ui.reward.SetActive(false);phase=Phase.Subjects;ui.subjects.SetActive(true);RefreshCues();
            feedback?.HideReminder();
        }
        public void Leave()
        {if(adventure.Location!=OutdoorAdventure.Place.School)return;ResetRoom();adventure.EndSchool();Save();}
        void ResetRoom()
        {
            GetComponent<LearningJourney>()?.Close();
            phase=Phase.Outside;ui.view.SetActive(false);ui.worksheet.SetActive(false);ui.reward.SetActive(false);
            feedback?.HideReminder();
            ui.seatedRect.localRotation=Quaternion.identity;foreach(var pad in ui.pads)pad.CanWrite=false;RefreshCues();
        }
        void UpdateKit()
        {
            for(int i=0;i<3;i++){bool packed=(progress.packed&(1<<i))!=0;ui.kitChecks[i].gameObject.SetActive(packed);ui.kit[i].interactable=!packed&&(i==0||(progress.packed&1)!=0);}
            ui.stars.text=progress.stars.ToString();
        }
        void RefreshCues()
        {
            ui.exit.gameObject.SetActive(phase!=Phase.Writing);
            ui.desk.gameObject.SetActive(phase==Phase.Desk||phase==Phase.Walking);
            ui.listen.gameObject.SetActive(phase==Phase.Listen||phase==Phase.Listening);
            ui.paper.gameObject.SetActive(phase==Phase.Paper||phase==Phase.Writing||phase==Phase.Reward);
            for(int i=0;i<3;i++)ui.kitCues[i].SetHighlighted(phase==Phase.Packing&&ui.kit[i].interactable);
            for(int i=0;i<ui.lessonCues.Length;i++)ui.lessonCues[i].SetHighlighted(phase==Phase.Subjects);
            ui.deskCue.SetHighlighted(phase==Phase.Desk);ui.listenCue.SetHighlighted(phase==Phase.Listen);
            ui.paperCue.SetHighlighted(phase==Phase.Paper);ui.pencilCue.SetHighlighted(phase==Phase.Writing&&!penSelected);
            bool ink=ui.pads[0].PointCount>0&&(lesson==null||lesson.Answer==null||lesson.Answer.Length==1||ui.pads[1].PointCount>0);
            ui.submit.interactable=phase==Phase.Writing&&penSelected&&ink;
            ui.submitCue.SetHighlighted(ui.submit.interactable);ui.eraseCue.SetHighlighted(false);
            ui.rewardCue.SetHighlighted(phase==Phase.Reward);ui.signCue.SetHighlighted(false);
            ui.paper.interactable=phase==Phase.Paper;
            ui.help.gameObject.SetActive(phase==Phase.Writing&&failedAttempts>=4);
            ui.help.interactable=phase==Phase.Writing&&failedAttempts>=4;
            feedback?.RefreshHelp(usedHelp,phase==Phase.Writing&&failedAttempts>=4);
            if(phase==Phase.Writing)ui.question.color=new Color32(65,64,48,255);
        }
        void Save(){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(progress));PlayerPrefs.Save();}
        static void ClearFocus(){if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);}
        void OnApplicationPause(bool paused){if(paused)Save();}
        void OnDestroy()
        {if(adventure!=null)adventure.HouseReached-=Arrived;foreach(var pad in ui.pads)if(pad!=null)pad.Changed-=InkChanged;}
    }
}
