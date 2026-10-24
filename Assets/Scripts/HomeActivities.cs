using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    // One owner for home activities; ingredients are committed only on completion.
    public sealed class HomeActivities : MonoBehaviour
    {
        public enum Activity { Idle, ToChair, Watching, ToKitchen, Kitchen, Cooking, ToTable, Eating }
        [Serializable] public sealed class Widgets
        {
            public UnityEngine.UI.Button chair, television, kitchen, close, cook, eat, stand, sound;
            public UnityEngine.UI.Button[] recipes;
            public TMPro.TMP_Text[] recipeLabels;
            public TMPro.TMP_Text status, soundLabel, feedback;
            public GameObject kitchenPanel, tvControls;
            public UnityEngine.UI.Image seated, screen, progress;
            public HearthGraphic steam, plate;
            public Sprite maleSeated, femaleSeated, femaleStanding;
            public Sprite[] programmes;
        }
        [SerializeField] private RabbitWalker rabbit;
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private GardenController garden;
        [SerializeField] private BedInteraction bed;
        [SerializeField] private Widgets ui;
        private RabbitGraphic standing;
        private AudioSource televisionAudio;
        private AudioClip televisionClip;
        private int recipe;
        private float elapsed, channelTime, feedbackTime;
        private bool muted;
        private static readonly Vector2 Chair = new Vector2(.785f,.118f);
        private static readonly Vector2 Kitchen = new Vector2(.365f,.118f);
        private static readonly Vector2 Table = new Vector2(.747f,.474f);
        public Activity State { get; private set; }
        public int Channel { get; private set; }
        public bool IsWatching => State == Activity.Watching;
        public bool SoundPlaying => televisionAudio != null && televisionAudio.isPlaying;
        public bool IsMuted => muted;
        public int SelectedRecipe => recipe;
        public Widgets UI => ui;

        public void Configure(RabbitWalker walker, OutdoorAdventure travel, GardenController crops,
            BedInteraction sleeping, Widgets widgets)
        { rabbit=walker;adventure=travel;garden=crops;bed=sleeping;ui=widgets; }

        private void Awake()
        {
            standing=rabbit.GetComponent<RabbitGraphic>();
            ui.chair.onClick.AddListener(ToggleTV);
            ui.television.onClick.AddListener(TapTV);
            ui.kitchen.onClick.AddListener(OpenKitchen);
            ui.close.onClick.AddListener(CancelActivity);
            ui.stand.onClick.AddListener(CancelActivity);
            ui.sound.onClick.AddListener(ToggleSound);
            ui.cook.onClick.AddListener(Cook);
            ui.eat.onClick.AddListener(Eat);
            for(int i=0;i<ui.recipes.Length;i++){int crop=i;ui.recipes[i].onClick.AddListener(()=>SelectRecipe(crop));}
            televisionAudio=gameObject.AddComponent<AudioSource>();
            televisionAudio.playOnAwake=false;televisionAudio.loop=true;televisionAudio.spatialBlend=0;
            televisionAudio.volume=.40f;
            televisionClip=CreateTVSound();televisionAudio.clip=televisionClip;
            muted=PlayerPrefs.GetInt("MrRabbit.TV.Muted",0)!=0;
            RefreshSound();
            ResetVisuals();
        }

        public void ToggleTV()
        {
            if(!AtHome)return;
            if(State==Activity.Watching||State==Activity.ToChair){CancelActivity();return;}
            BeginRoute(Activity.ToChair,Chair);
        }
        public void TapTV(){if(IsWatching)NextChannel();else ToggleTV();}
        public void OpenKitchen()
        {
            if(!AtHome)return;
            BeginRoute(Activity.ToKitchen,Kitchen);
        }
        private bool AtHome => adventure.Location==OutdoorAdventure.Place.Home;
        private void BeginRoute(Activity next,Vector2 destination)
        {
            CancelActivity();bed.Wake();
            State=next;rabbit.WalkTo(destination);ClearFocus();
        }
        public void SelectRecipe(int crop)
        {
            if(State!=Activity.Kitchen||crop<0||crop>=ui.recipes.Length)return;
            recipe=crop;RefreshKitchen();ClearFocus();
        }
        public void Cook()
        {
            if(State!=Activity.Kitchen||garden.Progress.harvests[recipe]<=0)return;
            State=Activity.Cooking;elapsed=0;ui.steam.gameObject.SetActive(true);
            RefreshKitchen();ClearFocus();
        }
        public void Eat()
        {
            if(State!=Activity.Kitchen||garden.Progress.meals[recipe]<=0)return;
            BeginRoute(Activity.ToTable,Table);
        }
        public void ToggleSound()
        {
            muted=!muted;PlayerPrefs.SetInt("MrRabbit.TV.Muted",muted?1:0);PlayerPrefs.Save();
            RefreshSound();
            if(muted)televisionAudio.Stop();else if(IsWatching)televisionAudio.Play();
            ClearFocus();
        }
        public void NextChannel()
        {
            if(!IsWatching||ui.programmes.Length==0)return;
            Channel=(Channel+1)%ui.programmes.Length;channelTime=0;ui.screen.sprite=ui.programmes[Channel];
        }
        private void Update()
        {
            var guide=adventure.GetComponent<GameGuide>();if(guide!=null&&(guide.IntroOpen||guide.MenuOpen))return;
            var k=Keyboard.current;
            if(State!=Activity.Idle&&k!=null&&(k.escapeKey.wasPressedThisFrame||k.leftArrowKey.isPressed||
                k.rightArrowKey.isPressed||k.upArrowKey.isPressed||k.downArrowKey.isPressed))CancelActivity();
            Advance(Time.deltaTime);
        }
        public void Advance(float seconds)
        {
            if(!AtHome){CancelActivity();return;}
            seconds=Mathf.Max(0,seconds);
            if(feedbackTime>0){feedbackTime-=seconds;if(feedbackTime<=0)ui.feedback.gameObject.SetActive(false);}
            bool approaching=State==Activity.ToChair||State==Activity.ToKitchen||State==Activity.ToTable;
            if(approaching&&!rabbit.HasDestination)
            {
                Vector2 target=State==Activity.ToChair?Chair:State==Activity.ToKitchen?Kitchen:Table;
                if(Vector2.Distance(rabbit.PositionNormalized,target)>.015f){CancelActivity();return;}
                rabbit.StopWalking();rabbit.MovementEnabled=false;elapsed=0;
                if(State==Activity.ToChair)
                {
                    State=Activity.Watching;ShowSeated(false);Channel=0;channelTime=0;
                    ui.screen.sprite=ui.programmes[0];ui.screen.color=Color.white;
                    ui.tvControls.SetActive(true);if(!muted)televisionAudio.Play();
                }
                else if(State==Activity.ToKitchen)
                {
                    State=Activity.Kitchen;ui.kitchenPanel.SetActive(true);RefreshKitchen();
                }
                else
                {
                    State=Activity.Eating;ShowSeated(true);ui.plate.gameObject.SetActive(true);
                    ui.plate.SetDish(recipe);Feedback(2);
                }
            }
            elapsed+=seconds;
            if(IsWatching)
            {
                channelTime+=seconds;if(channelTime>=3)NextChannel();
                ui.screen.color=Color.Lerp(Color.white,new Color(.86f,.94f,1),.5f+.5f*Mathf.Sin(elapsed*2));
                ui.seated.rectTransform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(elapsed*1.8f)*.7f);
            }
            else if(State==Activity.Cooking)
            {
                ui.progress.fillAmount=Mathf.Clamp01(elapsed/3);
                rabbit.transform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(elapsed*9)*2);
                if(elapsed>=3)
                {
                    garden.CookHarvest(recipe);State=Activity.Kitchen;
                    ui.steam.gameObject.SetActive(false);rabbit.transform.localRotation=Quaternion.identity;
                    RefreshKitchen();
                }
            }
            else if(State==Activity.Eating)
            {
                ui.seated.rectTransform.localRotation=Quaternion.Euler(0,0,Mathf.Sin(elapsed*8)*2);
                if(elapsed>=2.5f)
                {
                    bool eaten=garden.EatMeal(recipe);CancelActivity();
                    if(eaten)Feedback(2.5f);
                }
            }
        }
        private void ShowSeated(bool atTable)
        {
            standing.enabled=false;
            bool female=adventure.GetComponent<CharacterSelection>().SelectedCharacter=="Miss Rabbit";
            ui.seated.sprite=female?ui.femaleSeated:ui.maleSeated;
            var city=adventure.GetComponent<CityController>();if(city!=null)ui.seated.sprite=city.SeatedSprite(ui.seated.sprite);
            var r=ui.seated.rectTransform;
            r.anchorMin=atTable?new Vector2(.715f,.487f):new Vector2(.748f,.148f);
            r.anchorMax=atTable?new Vector2(.792f,.665f):new Vector2(.833f,.343f);
            r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=new Vector3(atTable?-1:1,1,1);
            ui.seated.gameObject.SetActive(true);
        }
        private void RefreshKitchen()
        {
            var p=garden.Progress;if(p==null)return;
            bool ready=State==Activity.Kitchen;
            for(int i=0;i<ui.recipes.Length;i++)
            {
                ui.recipeLabels[i].text="";
                ui.recipes[i].interactable=ready;
                ui.recipes[i].GetComponent<UnityEngine.UI.Image>().color=i==recipe?new Color32(245,209,134,255):new Color32(255,245,218,255);
            }
            ui.cook.interactable=ready&&p.harvests[recipe]>0;
            ui.eat.interactable=ready&&p.meals[recipe]>0;
            ui.status.text="";
            ui.progress.fillAmount=0;
        }
        public void CancelActivity()
        {
            if(State!=Activity.Idle)
            {
                rabbit.StopWalking();rabbit.MovementEnabled=AtHome;
                standing.enabled=true;rabbit.transform.localRotation=Quaternion.identity;
            }
            State=Activity.Idle;elapsed=0;ResetVisuals();
        }
        private void ResetVisuals()
        {
            if(ui==null||ui.seated==null)return;
            ui.seated.gameObject.SetActive(false);ui.kitchenPanel.SetActive(false);ui.tvControls.SetActive(false);
            ui.steam.gameObject.SetActive(false);ui.plate.gameObject.SetActive(false);
            ui.feedback.gameObject.SetActive(false);feedbackTime=0;
            ui.screen.sprite=null;ui.screen.color=new Color32(66,69,72,255);
            if(televisionAudio!=null)televisionAudio.Stop();
        }
        private void Feedback(float duration)
        {ui.feedback.text="";ui.feedback.gameObject.SetActive(true);feedbackTime=duration;}
        private void RefreshSound()
        {
            ui.soundLabel.text="";
            var icon=ui.sound.GetComponentInChildren<CitySymbol>(true);
            if(icon!=null){icon.symbol=muted?CitySymbol.Kind.Muted:CitySymbol.Kind.Sound;icon.SetAllDirty();}
        }
        private static void ClearFocus(){if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);}
        private void OnDisable(){CancelActivity();}
        private void OnApplicationPause(bool paused)
        {if(televisionAudio==null)return;if(paused)televisionAudio.Pause();else if(IsWatching&&!muted)televisionAudio.UnPause();}
        private void OnDestroy(){if(televisionClip!=null)Destroy(televisionClip);}

        // Original, quiet TV jingle; generated once, no external audio dependency.
        private static AudioClip CreateTVSound()
        {
            const int rate=22050;const float beat=.375f;int count=rate*6;
            var samples=new float[count];float[] notes={261.63f,329.63f,392,329.63f,293.66f,349.23f,440,392,329.63f,392,523.25f,440,349.23f,293.66f,261.63f,0};
            for(int i=0;i<count;i++)
            {
                float time=i/(float)rate,local=time%beat,f=notes[Mathf.Min(15,(int)(time/beat))];
                float envelope=Mathf.Min(1,local/.012f)*Mathf.Exp(-local*11)*Mathf.Clamp01((beat-local)/.025f);
                samples[i]=f==0?0:envelope*(Mathf.Sin(2*Mathf.PI*f*local)*.5f+Mathf.Sin(4*Mathf.PI*f*local)*.12f);
            }
            var clip=AudioClip.Create("Rabbit TV - original jingle",count,1,rate,false);clip.SetData(samples,0);return clip;
        }
    }
}
