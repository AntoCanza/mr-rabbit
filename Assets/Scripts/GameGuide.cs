using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    // Contextual teaching observes real state. It never awards progress or performs actions.
    [DefaultExecutionOrder(100)]
    public sealed class GameGuide : MonoBehaviour
    {
        [Serializable] public sealed class Widgets
        {
            public RectTransform root;
            public GameObject intro,menu;
            public GameObject[] introPictures;
            public TMPro.TMP_Text introTitle,pauseLabel;
            public UnityEngine.UI.Image[] pips;
            public UnityEngine.UI.Button next,back,skip,introRepeat,introMute,help,restart,repeat,pause,skipStep,menuClose;
            public CitySymbol introMuteSymbol,nextSymbol;
            public RectTransform hand;
            public GuideFocusGraphic[] focus;
        }
        sealed class Step
        {
            public string id,voice;public RectTransform[] targets;public bool drag,vertical;public float direction;
        }
        public Widgets UI;
        public GameGuideProgress Progress {get;private set;}
        public bool IntroOpen=>enabled&&UI!=null&&UI.intro.activeSelf;
        public bool MenuOpen=>enabled&&UI!=null&&UI.menu.activeSelf;
        public bool HasGuidance=>enabled&&!suspended&&(IntroOpen||step!=null);
        public string CurrentStep=>IntroOpen?"intro_"+Progress.introPage:step?.id;
        public bool EnabledGuide=>enabled&&Progress!=null&&!Progress.paused;
        public RectTransform[] Targets=>!enabled||IntroOpen||MenuOpen||Progress.paused?Array.Empty<RectTransform>():step?.targets??Array.Empty<RectTransform>();
        OutdoorAdventure adventure;
        CharacterSelection selection;
        SchoolController school;
        GardenController garden;
        CityController city;
        LearningJourney learning;
        HomeActivities home;
        RabbitWalker rabbit;
        BedInteraction bed;
        Neighborhood neighbors;
        ItalianVoice voice;
        GardenController.Widgets gu;
        Step step;
        bool pendingSpeech,suspended,manual,dirty;
        float entered,poll,layoutPoll;
        Vector2 previousHome;
        int previousQuestion=-1,previousMeals,previousCoins;
        bool previousAnswered,previousAccepted,previousSleeping,previousWatching;
        static readonly string[] Titles={"Benvenuto!","Impara e conquista","Coltiva e prenditi cura","Esplora e scegli"};
        void Awake()
        {
            Progress=GameGuideProgress.Load();adventure=GetComponent<OutdoorAdventure>();selection=GetComponent<CharacterSelection>();school=GetComponent<SchoolController>();
            garden=GetComponent<GardenController>();city=GetComponent<CityController>();learning=GetComponent<LearningJourney>();neighbors=GetComponent<Neighborhood>();voice=GetComponent<ItalianVoice>();
            rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);home=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);bed=UnityEngine.Object.FindFirstObjectByType<BedInteraction>(FindObjectsInactive.Include);
            gu=garden.UI;previousHome=rabbit.PositionNormalized;
            UI.next.onClick.AddListener(NextIntro);UI.back.onClick.AddListener(PreviousIntro);UI.skip.onClick.AddListener(FinishIntro);
            UI.introRepeat.onClick.AddListener(Replay);UI.introMute.onClick.AddListener(()=>voice.ToggleMute());UI.help.onClick.AddListener(OpenMenu);
            UI.restart.onClick.AddListener(ReplayIntro);UI.repeat.onClick.AddListener(ReplayContext);UI.pause.onClick.AddListener(TogglePause);UI.skipStep.onClick.AddListener(SkipCurrent);UI.menuClose.onClick.AddListener(CloseMenu);
            UI.intro.SetActive(false);UI.menu.SetActive(false);HideFocus();
        }
        void Start()
        {
            previousMeals=garden.Progress.mealsEaten;previousCoins=city.Progress.coins;previousAccepted=city.Progress.accepted;
            if(!Progress.introFinished&&!Progress.paused)ShowIntro();
        }
        public void Record(string id){if(Progress!=null&&Progress.Complete(id))dirty=true;}
        public void NextIntro()
        {if(!IntroOpen)return;if(Progress.introPage>=3)FinishIntro();else{Progress.introPage++;Progress.Store();ShowIntro();}}
        public void PreviousIntro(){if(!IntroOpen||Progress.introPage<=0)return;Progress.introPage--;Progress.Store();ShowIntro();}
        public void ReplayIntro(){UI.menu.SetActive(false);Progress.introPage=0;Progress.paused=false;Progress.Store();ShowIntro();}
        void ShowIntro()
        {
            step=null;manual=false;HideFocus();UI.intro.SetActive(true);UI.help.gameObject.SetActive(false);
            for(int i=0;i<4;i++){UI.introPictures[i].SetActive(i==Progress.introPage);UI.pips[i].color=i==Progress.introPage?new Color32(68,153,149,255):new Color32(218,212,190,255);}
            UI.introTitle.text=Titles[Progress.introPage];UI.back.interactable=Progress.introPage>0;UI.nextSymbol.symbol=Progress.introPage==3?CitySymbol.Kind.Check:CitySymbol.Kind.Arrow;UI.nextSymbol.SetAllDirty();
            UI.next.GetComponent<ActionCue>()?.SetHighlighted(true);
            StopMovement();voice.Stop();voice.Say("guide_intro_"+Progress.introPage);ClearSelection();
        }
        public void FinishIntro()
        {
            if(!IntroOpen)return;Progress.introFinished=true;Progress.introPage=0;Progress.Store();UI.intro.SetActive(false);voice.Stop();step=null;poll=0;ClearSelection();Tick();
        }
        public void OpenMenu(){if(IntroOpen)return;StopMovement();HideFocus();UI.menu.SetActive(true);voice.Say("guide_menu",true);ClearSelection();}
        public void CloseMenu(){UI.menu.SetActive(false);if(voice.LastKey=="guide_menu")voice.Stop();pendingSpeech=step!=null;entered=Time.unscaledTime;poll=0;ClearSelection();}
        public void TogglePause()
        {Progress.paused=!Progress.paused;Progress.Store();step=null;manual=false;voice.Stop();UI.menu.SetActive(false);HideFocus();poll=0;Tick();}
        public void SkipCurrent(){if(step!=null)Record(step.id);manual=false;step=null;UI.menu.SetActive(false);voice.Stop();poll=0;Tick();}
        public void Replay()
        {if(IntroOpen)voice.Say("guide_intro_"+Progress.introPage,true);else if(step!=null){voice.Say(step.voice,true);pendingSpeech=false;}else voice.Repeat();}
        public void ReplayContext()
        {UI.menu.SetActive(false);if(Progress.paused){Progress.paused=false;Progress.Store();}manual=true;step=null;poll=0;Tick();Replay();ClearSelection();}
        void StopMovement(){rabbit.EndTouchMovement();adventure.EndTouchMovement();city.EndTouchMovement();neighbors.EndTouchMovement();rabbit.StopWalking();}
        void LateUpdate(){Tick();}
        public void Tick()
        {
            if(Progress==null||UI==null)return;
            UI.root.gameObject.SetActive(enabled);
            UI.introMuteSymbol.symbol=voice.Muted?CitySymbol.Kind.Muted:CitySymbol.Kind.Sound;UI.introMuteSymbol.SetVerticesDirty();
            UI.pauseLabel.text=Progress.paused?"Riprendi":"Pausa";
            if(suspended){HideFocus();return;}
            if(IntroOpen){HideFocus();return;}
            Observe();if(dirty){Progress.Store();dirty=false;}
            UI.help.gameObject.SetActive(!UI.menu.activeSelf);
            if(UI.menu.activeSelf){HideFocus();return;}
            if(Progress.paused){step=null;HideFocus();return;}
            poll-=Time.unscaledDeltaTime;
            if(poll<=0)
            {
                poll=.10f;var next=FindStep();
                if(next?.id!=step?.id)
                {
                    if(manual&&step!=null){manual=false;next=FindStep();}
                    if(voice.LastKey!=null&&voice.LastKey.StartsWith("guide_")&&voice.IsSpeaking)voice.Stop();
                    step=next;entered=Time.unscaledTime;pendingSpeech=next!=null;
                }
                else if(next!=null)step=next;
            }
            if(step!=null&&pendingSpeech&&!voice.IsSpeaking&&Time.unscaledTime-entered>.65f)
            {voice.Say(step.voice);pendingSpeech=false;}
            PlaceHelp();AnimateFocus();
        }
        void Observe()
        {
            if(!selection.IsChoosing)Record("choose");
            if(adventure.Location==OutdoorAdventure.Place.Home)
            {
                if(rabbit.TouchHeld&&Vector2.Distance(previousHome,rabbit.PositionNormalized)>.001f)Record("move");
                if(rabbit.Surface==RabbitWalker.WalkSurface.UpperFloor)Record("stairs");
                if(bed.IsSleeping)Record("sleep");if(home.IsWatching)Record("tv");
            }
            previousHome=rabbit.PositionNormalized;
            if(previousSleeping&&!bed.IsSleeping)Record("wake");previousSleeping=bed.IsSleeping;
            if(previousWatching&&!home.IsWatching)Record("tv_controls");previousWatching=home.IsWatching;
            if(step!=null&&step.id=="growing"&&adventure.Location==OutdoorAdventure.Place.Meadow)Record("growing");
            if(school.State==SchoolController.Phase.Packing)Record("bag");
            for(int i=0;i<3;i++)if((school.Packed&(1<<i))!=0)Record("pack_"+i);
            if(adventure.Location==OutdoorAdventure.Place.FootwearChoice)Record("exit");
            if(adventure.Location==OutdoorAdventure.Place.Meadow)Record("shoes");
            if(adventure.Location==OutdoorAdventure.Place.School&&school.State!=SchoolController.Phase.Packing)Record("school");
            if(school.State==SchoolController.Phase.Listen||school.State==SchoolController.Phase.Listening||school.State==SchoolController.Phase.Subjects)Record("sit");
            if(school.State==SchoolController.Phase.Subjects)Record("listen");
            if(learning.State==LearningJourney.Page.Notebook)Record("notebook");
            if(learning.State==LearningJourney.Page.Exercise){Record("practice");if(learning.Assessment)Record("assessment");if(learning.Answered){if(learning.Lesson.Writing){Record("write");Record("submit");}else Record("answer");}}
            if(previousAnswered&&(learning.QuestionNumber!=previousQuestion||learning.State!=LearningJourney.Page.Exercise))Record("next");
            previousQuestion=learning.QuestionNumber;previousAnswered=learning.State==LearningJourney.Page.Exercise&&learning.Answered;
            if(learning.Progress.wall.Any(i=>i>=0))Record("hang");
            if(learning.State==LearningJourney.Page.Wall)Record("wall");
            if(learning.SelectedMedal>=0)Record("medal_choice");
            if(adventure.Location==OutdoorAdventure.Place.Garden)Record("garden");
            if(gu.seedPanel.activeSelf)Record("seed_tool");
            if(garden.HeldTool==GardenController.Tool.Hoe)Record("hoe_tool");
            if(garden.HeldTool==GardenController.Tool.Water)Record("water_tool");
            if(garden.HeldTool==GardenController.Tool.Basket)Record("harvest_tool");
            if(garden.HeldTool==GardenController.Tool.Seeds)Record("sow_tool");
            for(int i=0;i<3;i++)
            {
                var p=garden.Progress.plots[i];if(p.prepared)Record("hoe");if(p.crop>=0)Record("seeds");if(p.planted)Record("sow");if(p.water>0)Record("water");
            }
            if(garden.Progress.TotalHarvests>0)Record("harvest");
            if(garden.Progress.meals.Any(n=>n>0))Record("cook");
            if(garden.Progress.mealsEaten>previousMeals)Record("eat");previousMeals=garden.Progress.mealsEaten;
            if(home.State==HomeActivities.Activity.Kitchen)Record("kitchen");
            if(city.State==CityController.Phase.Map)Record("station");
            if(adventure.Location==OutdoorAdventure.Place.City)Record("train");
            if(city.State==CityController.Phase.Market)Record("market");
            if(city.Progress.accepted)Record("accept");
            if(previousAccepted&&!city.Progress.accepted&&city.Progress.coins>previousCoins)Record("deliver");
            previousAccepted=city.Progress.accepted;previousCoins=city.Progress.coins;
            if(city.State==CityController.Phase.Wardrobe)Record("boutique");
            if(city.Progress.equipped.Any(n=>n>0))Record("outfit");
            if(adventure.Location==OutdoorAdventure.Place.Visit)Record("visit");
        }
        bool Want(string id)=>manual||!Progress.Done(id);
        Step Hint(string id,string audio,params RectTransform[] targets)
        {return Want(id)?new Step{id=id,voice="guide_"+audio,targets=targets.Where(t=>t!=null).ToArray()}:null;}
        public static RectTransform ActionTarget(UnityEngine.UI.Button b)
        {
            if(b==null)return null;
            foreach(string name in new[]{"Action Hint","Action Picture","Painted Entrance"}){var child=b.transform.Find(name);if(child!=null&&child.gameObject.activeInHierarchy)return (RectTransform)child;}
            var picture=b.GetComponent<ActionCue>()?.Picture;
            return picture!=null&&picture.gameObject.activeInHierarchy?picture:(RectTransform)b.transform;
        }
        Step Button(string id,string audio,UnityEngine.UI.Button b)=>Hint(id,audio,ActionTarget(b));
        Step Route(string id,string audio,UnityEngine.UI.Button b,float direction)
        {
            if(b!=null&&Visible(ActionTarget(b)))return Button(id,audio,b);
            return Want(id)?new Step{id=id,voice="guide_walk",targets=Array.Empty<RectTransform>(),drag=true,direction=direction}:null;
        }
        Step FindStep()
        {
            var place=adventure.Location;
            if(selection.IsChoosing)return Hint("choose","choose",Find("Choose Mr Rabbit"),Find("Choose Miss Rabbit"));
            if(school.State==SchoolController.Phase.Packing)
            {for(int i=0;i<3;i++)if((school.Packed&(1<<i))==0)return Button("pack_"+i,"pack_"+i,school.UI.kit[i]);return null;}
            if(learning.IsOpen)return LearningStep();
            if(place==OutdoorAdventure.Place.FootwearChoice)return Hint("shoes","shoes",Find("Choose Shoes"),Find("Choose Rollers"));
            if(place==OutdoorAdventure.Place.Home)
            {
                if(home.State==HomeActivities.Activity.Kitchen)return KitchenStep();
                if(home.IsWatching)return Hint("tv_controls","tv_controls",ActionTarget(home.UI.television),ActionTarget(home.UI.sound),ActionTarget(home.UI.stand));
                if(home.State!=HomeActivities.Activity.Idle||bed.IsApproaching||rabbit.HasDestination)return null;
                if(bed.IsSleeping)return Hint("wake","wake",(RectTransform)bed.transform);
                if(Want("move"))return new Step{id="move",voice="guide_move",targets=Array.Empty<RectTransform>(),drag=true,direction=1};
                if(school.Packed!=7&&Want("bag"))return Button("bag","bag",school.UI.homeBag);
                if(learning.Progress.medals.Any(x=>x)&&!Progress.Done("hang"))return Button("wall","wall",learning.UI.openWall);
                if(!Progress.Done("school"))return Button("exit","exit",(UnityEngine.UI.Button)Find("Exit Door",true)?.GetComponent<UnityEngine.UI.Button>());
                if(garden.Progress.TotalHarvests>0||garden.Progress.meals.Any(n=>n>0))
                {var k=Button("kitchen","kitchen",home.UI.kitchen);if(k!=null)return k;}
                if(Want("tv"))return Button("tv","tv",home.UI.chair);
                if(Want("sleep"))
                {
                    if(!Visible((RectTransform)bed.transform))return Want("stairs_route")?new Step{id="stairs_route",voice="guide_stairs",targets=Array.Empty<RectTransform>(),drag=true,vertical=rabbit.PositionNormalized.x>=.42f&&rabbit.PositionNormalized.x<=.60f,direction=rabbit.PositionNormalized.x>.60f?-1:1}:null;
                    return Hint("sleep","sleep",(RectTransform)bed.transform);
                }
                return Button("home_explore","home_explore",(UnityEngine.UI.Button)Find("Exit Door",true)?.GetComponent<UnityEngine.UI.Button>());
            }
            if(place==OutdoorAdventure.Place.Meadow)
            {
                if(adventure.HasDestination||adventure.TouchHeld)return null;
                if(Want("school"))return Button("school","school",school.UI.sign);
                if(learning.Progress.medals.Any(x=>x)&&!Progress.Done("hang"))return Button("return_medal","return_medal",(UnityEngine.UI.Button)Find("Home Sign")?.GetComponent<UnityEngine.UI.Button>());
                bool needs=garden.Progress.plots.Any(p=>p.planted&&(p.water<=0||p.growth>=GardenProgress.GrowDays[p.crop]));
                if(needs||!Progress.Done("garden"))return Route("garden","garden",gu.gate,Mathf.Sign(GardenController.GateX-adventure.Position));
                if(!Progress.Done("train"))return Button("station","station",city.UI.stationSign);
                if(!Progress.Done("visit"))return Route("visit","visit",(UnityEngine.UI.Button)Find("Casa di Coccodella")?.GetComponent<UnityEngine.UI.Button>(),Mathf.Sign(.747f-adventure.Position));
                return null;
            }
            if(place==OutdoorAdventure.Place.Garden)return GardenStep();
            if(place==OutdoorAdventure.Place.School)
            {
                if(school.State==SchoolController.Phase.Desk)return Button("sit","sit",school.UI.desk);
                if(school.State==SchoolController.Phase.Listen)return Button("listen","listen",school.UI.listen);
                if(school.State==SchoolController.Phase.Subjects)return Button("notebook","notebook",learning.UI.openBook);
                // Legacy worksheets remain accessible, with their own exercise-specific voices.
                if(school.State==SchoolController.Phase.Paper)return Button("paper","paper",school.UI.paper);
                if(school.State==SchoolController.Phase.Writing)return Button("pencil","pencil",school.UI.pencil);
                return null;
            }
            if(place==OutdoorAdventure.Place.Train)
            {if(city.State==CityController.Phase.Map)return Button("destination","destination",city.UI.mapCity.interactable?city.UI.mapCity:city.UI.mapHome);return null;}
            if(place==OutdoorAdventure.Place.City)return CityStep();
            if(place==OutdoorAdventure.Place.Visit)
            {var greet=Find("Resident");return Want("greet")?Hint("greet","greet",greet):Hint("visit_exit","visit_exit",Find("Exit Door"));}
            return null;
        }
        Step LearningStep()
        {
            if(learning.State==LearningJourney.Page.Notebook)
            {if(learning.Progress.CanTest(learning.SelectedSkill)&&Want("assessment"))return Button("assessment","assessment",learning.UI.test);return Button("practice","practice",learning.UI.practice);}
            if(learning.State==LearningJourney.Page.Result)return Button("result","result",learning.UI.resultNext);
            if(learning.State==LearningJourney.Page.Wall)
            {
                if(!learning.Progress.medals.Any(x=>x))return Button("wall_empty","wall_empty",learning.UI.close);
                if(learning.SelectedMedal<0){int id=Array.FindIndex(learning.Progress.medals,x=>x);return Button("medal_choice","medal_choice",learning.UI.medalChoices[id]);}
                int slot=Array.FindIndex(learning.Progress.wall,x=>x<0);return Button("hang","hang",learning.UI.wallSlots[slot<0?0:slot]);
            }
            if(learning.State==LearningJourney.Page.Exercise)
            {
                if(learning.Answered)return Button("next","next",learning.UI.next);
                if(learning.Lesson.Writing)
                {
                    var blank=learning.UI.pads.Where(p=>p.gameObject.activeInHierarchy&&p.PointCount==0).Select(p=>p.rectTransform).ToArray();
                    if(blank.Length>0)return Hint("write","write",blank);
                    return Button("submit","submit",learning.UI.submit);
                }
                return Hint("answer","answer",learning.UI.choices.Select(b=>(RectTransform)b.transform).ToArray());
            }
            return null;
        }
        Step GardenStep()
        {
            if(garden.IsMoving||garden.IsWorking)return null;
            if(gu.seedPanel.activeSelf)return Hint("seeds","seeds",gu.seedChoices.Take(4).Select(b=>(RectTransform)b.transform).ToArray());
            int plot=Array.FindIndex(garden.Progress.plots,p=>p.planted&&p.growth>=GardenProgress.GrowDays[p.crop]);
            int tool=3;string action="harvest";
            if(plot<0){plot=Enumerable.Range(0,3).Where(i=>garden.Progress.NeedsWater(i)).DefaultIfEmpty(-1).First();tool=2;action="water";}
            if(plot<0&&Want("growing")&&garden.Progress.plots.Any(p=>p.planted&&p.water>0))return Button("growing","growing",gu.exit);
            if(plot<0){plot=garden.SelectedPlot;var p=garden.Progress.plots[plot];if(!p.prepared){tool=0;action="hoe";}else if(!p.planted){if(p.crop<0)return Button("seed_tool","seeds_tool",gu.tools[1]);tool=1;action="sow";}else return Button("growing","growing",gu.exit);}
            if((int)garden.HeldTool!=tool)return Button(action+"_tool",tool==1?"seeds_tool":action+"_tool",gu.tools[tool]);
            return Button(action,action,gu.beds[plot]);
        }
        Step KitchenStep()
        {
            int current=home.SelectedRecipe;
            if(garden.Progress.meals[current]>0)return Button("eat","eat",home.UI.eat);
            if(garden.Progress.harvests[current]>0)return Button("cook","cook",home.UI.cook);
            int crop=Enumerable.Range(0,6).Where(i=>garden.Progress.meals[i]>0||garden.Progress.harvests[i]>0).DefaultIfEmpty(-1).First();
            if(crop>=0)return Button("recipe","recipe",home.UI.recipes[crop]);
            return Button("no_food","no_food",home.UI.close);
        }
        Step CityStep()
        {
            if(city.State==CityController.Phase.Market)
            {
                if(!city.Progress.accepted)return Button("accept","accept",city.UI.accept);
                if(city.CanDeliver)return Button("deliver","deliver",city.UI.deliver);
                return Button("collect_order","collect_order",city.UI.closeMarket);
            }
            if(city.State==CityController.Phase.Wardrobe)
            {
                var available=city.UI.clothes.Skip(1).FirstOrDefault(b=>b.interactable);
                if(available!=null)return Button("outfit","outfit",available);
                return Button("clothes_later","clothes_later",city.UI.closeWardrobe);
            }
            if(city.State==CityController.Phase.City)
            {
                if(!Progress.Done("accept"))return Route("market","market",city.UI.marketDoor,1);
                if(!Progress.Done("boutique"))return Route("boutique","boutique",city.UI.boutiqueDoor,1);
                return Route("city_return","city_return",city.UI.cityStation,-1);
            }
            return null;
        }
        RectTransform Find(string name,bool homeOnly=false)
        {return transform.parent.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(t=>t.name==name&&t.gameObject.activeInHierarchy&&(!homeOnly||t.IsChildOf(home.transform)));}
        static Rect Bounds(RectTransform target)
        {var corners=new Vector3[4];target.GetWorldCorners(corners);Vector2 lo=RectTransformUtility.WorldToScreenPoint(null,corners[0]),hi=RectTransformUtility.WorldToScreenPoint(null,corners[2]);return new Rect(lo,hi-lo);}
        static bool Visible(RectTransform target)
        {if(target==null||!target.gameObject.activeInHierarchy)return false;var r=Bounds(target);return r.width>2&&r.height>2&&r.center.x>8&&r.center.x<Screen.width-8&&r.center.y>8&&r.center.y<Screen.height-8;}
        void PlaceHelp()
        {
            layoutPoll-=Time.unscaledDeltaTime;if(layoutPoll>0)return;layoutPoll=.4f;
            var controls=transform.parent.GetComponentsInChildren<UnityEngine.UI.Selectable>(false).Where(b=>b.isActiveAndEnabled&&!b.transform.IsChildOf(UI.root)).Select(b=>Bounds((RectTransform)b.transform)).ToArray();
            var rect=(RectTransform)UI.help.transform;
            float w=.068f,h=.10f;Vector2[] candidates={new Vector2(.012f,.015f),new Vector2(.012f,.75f),new Vector2(.012f,.40f),new Vector2(.91f,.015f),new Vector2(.012f,.14f),new Vector2(.44f,.012f),new Vector2(.12f,.015f),new Vector2(.91f,.75f)};
            Vector2 best=candidates[0];float score=float.MaxValue;
            foreach(var p in candidates)
            {
                var box=new Rect(p.x*Screen.width,p.y*Screen.height,w*Screen.width,h*Screen.height);float cost=0;
                foreach(var b in controls)if(box.Overlaps(b)){float dx=Mathf.Min(box.xMax,b.xMax)-Mathf.Max(box.xMin,b.xMin),dy=Mathf.Min(box.yMax,b.yMax)-Mathf.Max(box.yMin,b.yMin);cost+=dx*dy;}
                if(cost<score){score=cost;best=p;if(cost==0)break;}
            }
            rect.anchorMin=best;rect.anchorMax=best+new Vector2(w,h);rect.offsetMin=rect.offsetMax=Vector2.zero;
        }
        void AnimateFocus()
        {
            HideFocus();if(step==null)return;var targets=step.targets.Where(Visible).Take(UI.focus.Length).ToArray();
            float pulse=(1-Mathf.Cos(Time.unscaledTime*3))*.5f;
            for(int i=0;i<targets.Length;i++)
            {
                var box=Bounds(targets[i]);box.xMin-=4;box.yMin-=4;box.xMax+=4;box.yMax+=4;
                var focus=UI.focus[i];focus.gameObject.SetActive(true);focus.color=new Color(1,.68f,.13f,.58f+pulse*.30f);ScreenRect(focus.rectTransform,box);
            }
            Vector2 at;
            if(targets.Length>0)
            {
                var box=Bounds(targets[(int)(Time.unscaledTime/2)%targets.Length]);at=new Vector2(box.center.x,box.center.y+Mathf.Sin(Time.unscaledTime*3)*5);
            }
            else if(step.drag)at=step.vertical?new Vector2(Screen.width*.53f,Screen.height*(.33f+Mathf.Sin(Time.unscaledTime*1.8f)*.10f)):new Vector2(Screen.width*(.43f+Mathf.Sin(Time.unscaledTime*1.8f)*.10f*step.direction),Screen.height*.23f);
            else return;
            float size=Mathf.Min(Screen.width,Screen.height)*.115f;
            at.x=Mathf.Clamp(at.x,size*.5f,Screen.width-size*.5f);at.y=Mathf.Clamp(at.y,size,Screen.height-size*.15f);
            UI.hand.gameObject.SetActive(true);ScreenRect(UI.hand,new Rect(at.x-size*.37f,at.y-size*.92f,size,size));
        }
        void ScreenRect(RectTransform rect,Rect screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(UI.root,screen.min,null,out var lo);RectTransformUtility.ScreenPointToLocalPointInRectangle(UI.root,screen.max,null,out var hi);
            rect.anchorMin=rect.anchorMax=UI.root.pivot;rect.pivot=Vector2.zero;rect.anchoredPosition=lo;rect.sizeDelta=hi-lo;
        }
        void HideFocus(){if(UI==null)return;foreach(var f in UI.focus)f.gameObject.SetActive(false);UI.hand.gameObject.SetActive(false);}
        static void ClearSelection(){if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);}
        void OnApplicationPause(bool pause)
        {suspended=pause;Progress?.Store();if(pause){StopMovement();voice.Stop();}else{pendingSpeech=step!=null;entered=Time.unscaledTime;}}
        void OnDisable(){if(UI!=null){UI.root.gameObject.SetActive(false);HideFocus();}if(voice!=null&&voice.LastKey!=null&&voice.LastKey.StartsWith("guide_"))voice.Stop();}
        void OnDestroy(){if(dirty)Progress?.Store();}
    }
}
