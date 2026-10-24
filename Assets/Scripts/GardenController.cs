using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    public sealed class GardenController : MonoBehaviour
    {
        public enum Tool { None=-1, Hoe=0, Seeds=1, Water=2, Basket=3 }
        private enum Action { None, TakeTool, Tend, Exit }
        [Serializable] public sealed class Widgets
        {
            public GameObject view, seedPanel;
            public UnityEngine.UI.Image background, approach;
            public UnityEngine.UI.Button gate, exit, cancelSeeds, notice;
            public UnityEngine.UI.Button[] tools, beds, seedChoices;
            public GardenBedGraphic[] soil;
            public UnityEngine.UI.RawImage[] plants;
            public UnityEngine.UI.RawImage[] bedIcons;
            public UnityEngine.UI.Image[] progress;
            public TMPro.TMP_Text[] bedLabels;
            public TMPro.TMP_Text guide, detail, count, noticeText;
            public UnityEngine.UI.RawImage noticeIcon, secondaryNoticeIcon, heldIcon;
            public RabbitGraphic rabbit;
            public GardenGuideArrow arrow;
        }
        public const string SaveKey="MrRabbit.Garden.v1";
        public const float GateX=-.65f;
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private DayNightCycle clock;
        [SerializeField] private Widgets ui;
        private GardenProgress progress;
        private int selected, pendingPlot;
        private Tool held=Tool.None,pendingTool;
        private Action action;
        private float x=.23f,targetX,gait,workTime,refreshTime,saveTime;
        private int facing=1;
        private bool moving,inGarden,suspended,dirty;
        private RectTransform guideTarget;
        private ActionCue[] gardenCues;
        public GardenProgress Progress=>progress;
        public Widgets UI=>ui;
        public int SelectedPlot=>selected;
        public Tool HeldTool=>held;
        public bool IsMoving=>moving;
        public bool IsWorking=>workTime>0;
        public bool NoticeVisible=>ui.notice.gameObject.activeSelf;
        public string NoticeText=>ui.noticeText.text;
        public int NoticeSymbol=>GardenArt.FindCell(ui.noticeIcon);

        public void Configure(OutdoorAdventure travel,DayNightCycle dayNight,Widgets widgets)
        {adventure=travel;clock=dayNight;ui=widgets;}

        private void Awake()
        {
            progress=GardenProgress.FromJson(PlayerPrefs.GetString(SaveKey,""));
            gardenCues=ui.view.GetComponentsInChildren<ActionCue>(true);
            if(ui.arrow!=null)ui.arrow.gameObject.SetActive(false);
            adventure.HouseReached+=OnArrival;clock.GameDaysAdvanced+=OnGameDays;
            ui.gate.onClick.AddListener(GoToGarden);ui.notice.onClick.AddListener(GoToGarden);
            ui.exit.onClick.AddListener(RequestExit);ui.cancelSeeds.onClick.AddListener(CancelSeeds);
            for(int i=0;i<4;i++) {int index=i;ui.tools[i].onClick.AddListener(()=>PickTool(index));ui.seedChoices[i].onClick.AddListener(()=>ChooseSeeds(index));}
            for(int i=4;i<ui.seedChoices.Length;i++){int index=i;ui.seedChoices[i].onClick.AddListener(()=>ChooseSeeds(index));}
            for(int i=0;i<3;i++) {int index=i;ui.beds[i].onClick.AddListener(()=>TapBed(index));}
            Refresh();
        }
        public void GoToGarden()=>adventure.WalkToHouse(2,GateX);
        private void OnArrival(int index)
        {
            if(index!=2||adventure.Location!=OutdoorAdventure.Place.Meadow)return;
            adventure.BeginGarden();inGarden=true;x=.23f;held=Tool.None;action=Action.None;moving=false;workTime=0;
            ui.rabbit.sprite=adventure.PlayerSprite;
            ui.view.SetActive(true);ui.seedPanel.SetActive(false);
            selected=FindNextPlot();progress.MarkSeen();dirty=true;
            ApplyRabbit(false);Refresh();Save();ClearFocus();
        }
        public void RequestExit()
        {
            if(!inGarden||workTime>0)return;
            ui.seedPanel.SetActive(false);Queue(Action.Exit,.08f);
        }
        private void Leave()
        {
            inGarden=false;moving=false;action=Action.None;workTime=0;
            ui.seedPanel.SetActive(false);ui.view.SetActive(false);Save();adventure.EndGarden();Refresh();
        }
        public void PickTool(int index)
        {
            if(!inGarden||index<0||index>3||workTime>0||ui.seedPanel.activeSelf)return;
            if(index==(int)Tool.Seeds&&progress.GetStage(selected)!=GardenProgress.Stage.Prepared) {Refresh();return;}
            pendingTool=(Tool)index;pendingPlot=selected;Queue(Action.TakeTool,.23f);
            GetComponent<ItalianVoice>()?.Say("tool_"+index);
        }
        public void TapBed(int index)
        {
            if(!inGarden||index<0||index>2||workTime>0||ui.seedPanel.activeSelf)return;
            selected=index;progress.plots[index].seenStage=(int)progress.GetStage(index);dirty=true;
            if(CanTend(index)) {pendingPlot=index;Queue(Action.Tend,BedX(index));}
            else {moving=false;action=Action.None;Refresh();}
        }
        private bool CanTend(int index)
        {
            var stage=progress.GetStage(index);
            return stage==GardenProgress.Stage.Empty&&held==Tool.Hoe||
                stage==GardenProgress.Stage.Prepared&&progress.plots[index].crop>=0&&held==Tool.Seeds||
                progress.NeedsWater(index)&&held==Tool.Water||stage==GardenProgress.Stage.Ripe&&held==Tool.Basket;
        }
        public void ChooseSeeds(int crop)
        {
            if(!inGarden||!ui.seedPanel.activeSelf||crop>=4&&!(LearningProgress.Current??LearningProgress.Load()).CropUnlocked(crop)||!progress.Choose(selected,crop))return;
            held=Tool.Seeds;ui.seedPanel.SetActive(false);dirty=true;Save();Refresh();ClearFocus();
        }
        public void CancelSeeds() {ui.seedPanel.SetActive(false);Refresh();ClearFocus();}
        private void Queue(Action next,float destination)
        {
            action=next;targetX=destination;moving=true;workTime=0;Refresh();ClearFocus();
        }
        private static float BedX(int index)=>.38f+index*.23f;

        private void Update()
        {
            if(inGarden&&adventure.Location!=OutdoorAdventure.Place.Garden)
            {
                inGarden=false;moving=false;workTime=0;action=Action.None;
                ui.view.SetActive(false);ui.seedPanel.SetActive(false);Save();
            }
            if(suspended)return;
            AdvanceActor(Time.deltaTime);
            refreshTime-=Time.deltaTime;
            if(refreshTime<=0) {refreshTime=.15f;Refresh();}
            if(dirty) {saveTime+=Time.deltaTime;if(saveTime>=5)Save();}
        }
        public void AdvanceActor(float seconds)
        {
            if(!inGarden||ui.seedPanel.activeSelf)return;
            seconds=Mathf.Max(0,seconds);
            if(workTime>0)
            {
                workTime=Mathf.Max(0,workTime-seconds);
                ui.heldIcon.rectTransform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(workTime*18)*22);
                if(workTime<=0) {ui.heldIcon.rectTransform.localRotation=Quaternion.identity;CompleteAction();}
                return;
            }
            if(!moving)return;
            float before=x;x=Mathf.MoveTowards(x,targetX,seconds*.34f);
            if(Mathf.Abs(x-before)>.00001f) {facing=x>before?1:-1;gait+=Mathf.Abs(x-before)*65;}
            ApplyRabbit(true);
            if(Mathf.Approximately(x,targetX))
            {
                moving=false;ApplyRabbit(false);
                if(action==Action.Exit)Leave();else workTime=.65f;
            }
        }
        private void CompleteAction()
        {
            var completed=action;action=Action.None;
            if(completed==Action.TakeTool)
            {
                held=pendingTool;
                if(held==Tool.Seeds&&progress.GetStage(pendingPlot)==GardenProgress.Stage.Prepared)
                {selected=pendingPlot;ui.seedPanel.SetActive(true);}
            }
            else if(completed==Action.Tend)
            {
                bool changed=false;
                switch(held)
                {
                    case Tool.Hoe:changed=progress.Hoe(pendingPlot);break;
                    case Tool.Seeds:changed=progress.Sow(pendingPlot);held=Tool.None;break;
                    case Tool.Water:changed=progress.Water(pendingPlot);if(changed)selected=FindNextPlot();break;
                    case Tool.Basket:changed=progress.Harvest(pendingPlot);held=Tool.None;break;
                }
                dirty|=changed;Save();
            }
            Refresh();
        }
        private void ApplyRabbit(bool walking)
        {
            var r=ui.rabbit.rectTransform;
            r.anchorMin=new Vector2(x-.065f,.13f);r.anchorMax=new Vector2(x+.065f,.40f);
            r.offsetMin=r.offsetMax=Vector2.zero;ui.rabbit.SetGait(gait,facing,walking);
        }
        private int FindNextPlot()
        {
            for(int i=0;i<3;i++)if(progress.GetStage(i)==GardenProgress.Stage.Ripe)return i;
            for(int i=0;i<3;i++)if(progress.NeedsWater(i))return i;
            for(int i=0;i<3;i++)if(!progress.plots[i].planted)return i;
            return selected;
        }
        private void OnGameDays(float days)
        {
            if(progress.AdvanceDays(days))dirty=true;
            if(inGarden)progress.MarkSeen();
        }

        // Rows in the hand-painted atlas are indexed from the top.
        public static Rect AtlasCell(int cell)=>new Rect((cell%4)*.25f,(3-cell/4)*.25f,.25f,.25f);
        private static int ToolCell(Tool tool)=>tool==Tool.Seeds?2:tool==Tool.Water?1:(int)tool;
        private static int PlantCell(int crop,GardenProgress.Stage stage)
        {
            if(stage==GardenProgress.Stage.Sprout)return crop==1?12:8;
            if(stage==GardenProgress.Stage.Growing)return crop==1?12:crop==3?14:9;
            return crop==0?10:crop==1?13:crop==2?11:15;
        }
        private void Refresh()
        {
            if(progress==null)return;
            int thirsty=0,ripe=0,news=0;
            for(int i=0;i<3;i++)
            {
                var p=progress.plots[i];var stage=progress.GetStage(i);
                bool needs=progress.NeedsWater(i);if(needs)thirsty++;if(stage==GardenProgress.Stage.Ripe)ripe++;
                if(progress.HasGrowthNews(i))news++;
                ui.soil[i].Show(p.prepared,p.water>0,false,stage==GardenProgress.Stage.Seeds);
                for(int j=0;j<3;j++)
                {
                    var plant=ui.plants[i*3+j];plant.gameObject.SetActive(stage>=GardenProgress.Stage.Sprout);
                    if(stage>=GardenProgress.Stage.Sprout){if(p.crop>=4&&stage==GardenProgress.Stage.Ripe)RewardArt.Apply(plant,p.crop,true);else{RewardArt.RestoreGarden(plant);GardenArt.Apply(plant,p.crop>=4?(stage==GardenProgress.Stage.Sprout?8:9):PlantCell(p.crop,stage));}}
                }
                ui.progress[i].fillAmount=p.planted?p.growth/GardenProgress.GrowDays[p.crop]:0;
                GardenArt.Apply(ui.bedIcons[i],stage==GardenProgress.Stage.Empty?11:stage==GardenProgress.Stage.Prepared?10:
                    stage==GardenProgress.Stage.Ripe?6:needs?5:7);
                ui.bedLabels[i].text="";
            }
            ui.count.text=progress.TotalHarvests.ToString();
            for(int i=4;i<ui.seedChoices.Length;i++){bool unlocked=(LearningProgress.Current??LearningProgress.Load()).CropUnlocked(i);ui.seedChoices[i].interactable=unlocked;var l=ui.seedChoices[i].transform.Find("School Lock");if(l!=null)l.gameObject.SetActive(!unlocked);}
            bool show=adventure.Location==OutdoorAdventure.Place.Meadow&&(ripe>0||thirsty>0||news>0);
            ui.notice.gameObject.SetActive(show);
            ui.noticeText.text="";ui.noticeText.gameObject.SetActive(false);
            GardenArt.Apply(ui.noticeIcon,ripe>0?6:thirsty>0?5:7);
            ui.secondaryNoticeIcon.gameObject.SetActive(ripe>0&&thirsty>0);
            ui.notice.GetComponent<ActionCue>()?.SetHighlighted(show);
            var gateHint=ui.gate.transform.Find("Action Hint");
            if(gateHint!=null)
            {
                gateHint.gameObject.SetActive(!show);
                bool work=false;for(int i=0;i<3;i++)work|=progress.GetStage(i)==GardenProgress.Stage.Empty||progress.GetStage(i)==GardenProgress.Stage.Prepared;
                gateHint.GetComponent<ActionCue>()?.SetHighlighted(work);
            }
            ui.approach.color=Color.Lerp(Color.white,new Color(.29f,.38f,.62f),clock.NightAmount);
            if(!inGarden)return;
            ui.background.color=Color.Lerp(Color.white,new Color(.53f,.59f,.75f),clock.NightAmount);
            ui.heldIcon.gameObject.SetActive(held!=Tool.None);
            if(held!=Tool.None)GardenArt.Apply(ui.heldIcon,ToolCell(held));
            Guide();
        }
        private void Guide()
        {
            guideTarget=null;var stage=progress.GetStage(selected);
            if(moving||workTime>0) { }
            else if(stage==GardenProgress.Stage.Empty)
            {guideTarget=held==Tool.Hoe?(RectTransform)ui.beds[selected].transform:(RectTransform)ui.tools[0].transform;}
            else if(stage==GardenProgress.Stage.Prepared)
            {
                bool ready=held==Tool.Seeds&&progress.plots[selected].crop>=0;
                guideTarget=ready?(RectTransform)ui.beds[selected].transform:(RectTransform)ui.tools[1].transform;
            }
            else if(stage==GardenProgress.Stage.Ripe)
            {guideTarget=held==Tool.Basket?(RectTransform)ui.beds[selected].transform:(RectTransform)ui.tools[3].transform;}
            else if(progress.NeedsWater(selected))
            {guideTarget=held==Tool.Water?(RectTransform)ui.beds[selected].transform:(RectTransform)ui.tools[2].transform;}
            else
            {
                int next=FindNextPlot();
                if(next!=selected) {guideTarget=(RectTransform)ui.beds[next].transform;}
                else guideTarget=(RectTransform)ui.exit.transform;
            }
            if(ui.seedPanel.activeSelf)guideTarget=null;
            ui.guide.text="";ui.detail.text="";
            foreach(var cue in gardenCues)
            {
                bool choice=ui.seedPanel.activeSelf&&Array.Exists(ui.seedChoices,b=>b.transform==cue.transform);
                var button=cue.GetComponent<UnityEngine.UI.Button>();cue.SetHighlighted(choice&&(button==null||button.interactable)||(!ui.seedPanel.activeSelf&&guideTarget==cue.transform));
            }
        }
        public void Save()
        {
            if(progress==null||!dirty)return;
            PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(progress));PlayerPrefs.Save();dirty=false;saveTime=0;
        }
        public bool CookHarvest(int crop)
        {
            if(progress==null||!progress.Cook(crop))return false;
            dirty=true;Save();Refresh();return true;
        }
        public bool DeliverHarvest(int crop,int amount,System.Action reward)
        {
            if(progress==null||crop<0||crop>=6||amount<=0||progress.harvests[crop]<amount||reward==null)return false;
            progress.harvests[crop]-=amount;
            // Stage both records before the one disk flush, keeping money and produce together.
            reward();dirty=true;Save();Refresh();return true;
        }
        public bool EatMeal(int crop)
        {
            if(progress==null||!progress.Eat(crop))return false;
            dirty=true;Save();Refresh();return true;
        }
        private static void ClearFocus(){if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);}
        private void OnApplicationPause(bool paused){suspended=paused;if(paused)Save();}
        private void OnApplicationQuit()=>Save();
        private void OnDestroy()
        {
            Save();if(clock!=null)clock.GameDaysAdvanced-=OnGameDays;if(adventure!=null)adventure.HouseReached-=OnArrival;
        }
    }
}
