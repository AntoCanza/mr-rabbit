using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MrRabbit
{
    public sealed class CityController : MonoBehaviour
    {
        public enum Phase { Outside, Map, Travelling, City, Market, Wardrobe }
        [Serializable] public sealed class Widgets
        {
            public GameObject map, journey, city, market, wardrobe, wallet, orderBadge;
            public UnityEngine.UI.Button station, stationSign, mapHome, mapCity, mapClose;
            public UnityEngine.UI.Button cityStation, marketDoor, boutiqueDoor, closeMarket, closeWardrobe, accept, deliver;
            public UnityEngine.UI.Button[] clothes;
            public RectTransform cityWorld, train, travellingScenery;
            public RabbitGraphic cityRabbit;
            public UnityEngine.UI.Image passenger, journeyProgress, mapPin;
            public UnityEngine.UI.Image[] clothesPictures;
            public TMPro.TMP_Text balance, orderQuantity, orderReward, orderStock, orderBadgeCount, feedback;
            public TMPro.TMP_Text[] prices;
            public UnityEngine.UI.RawImage orderCrop, badgeCrop;
            public UnityEngine.UI.Image customer;
            public Sprite male, female, maleOutfit, femaleOutfit, maleCitySeated, femaleCitySeated;
            public UnityEngine.UI.Image cityPainting, countryPainting, homeExterior;
            public Material goldOutfit,emeraldOutfit;
        }
        public const float StationX = -3.72f;
        public const float JourneySeconds = 6;
        public const int OutfitPrice = 60;
        [SerializeField] OutdoorAdventure adventure;
        [SerializeField] GardenController garden;
        [SerializeField] DayNightCycle clock;
        [SerializeField] CharacterSelection selection;
        [SerializeField] Widgets ui;
        CityProgress progress;
        Phase phase;
        bool originCity, targetCity, suspended, moving;
        float journeyTime, cityX=.36f, cityTarget, cameraLeft, gait, noticeTime, refreshTime;
        int pending=-1;
        RabbitGraphic[] actors;
        HomeActivities homeActivities;
        SchoolController schoolController;
        int appliedOutfit=-1;
        float touchDirection;
        public bool TouchHeld {get;private set;}
        public void BeginTouchMovement(){if(phase!=Phase.City)return;TouchHeld=true;touchDirection=0;moving=false;pending=-1;}
        public void SetTouchDirection(float value){if(TouchHeld)touchDirection=value;}
        public void EndTouchMovement(){TouchHeld=false;touchDirection=0;}
        public Phase State => phase;
        public CityProgress Progress => progress;
        public Widgets UI => ui;
        public float JourneyTime => journeyTime;
        public float CityX => cityX;
        public bool CanDeliver => progress.accepted && garden.Progress.harvests[progress.crop]>=progress.quantity && progress.coins<=1000000-progress.reward;
        int Character => selection.SelectedCharacter=="Miss Rabbit"?1:0;
        public void Configure(OutdoorAdventure a,GardenController g,DayNightCycle d,CharacterSelection s,Widgets w)
        {adventure=a;garden=g;clock=d;selection=s;ui=w;}
        void Awake()
        {
            progress=CityProgress.Load();
            actors=UnityEngine.Object.FindObjectsByType<RabbitGraphic>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            homeActivities=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);schoolController=GetComponent<SchoolController>();
            adventure.HouseReached+=Arrived;clock.GameDaysAdvanced+=Days;
            ui.station.onClick.AddListener(GoToStation);ui.stationSign.onClick.AddListener(GoToStation);
            ui.mapHome.onClick.AddListener(()=>ChooseDestination(false));ui.mapCity.onClick.AddListener(()=>ChooseDestination(true));ui.mapClose.onClick.AddListener(CloseMap);
            ui.cityStation.onClick.AddListener(()=>Approach(0));ui.marketDoor.onClick.AddListener(()=>Approach(1));ui.boutiqueDoor.onClick.AddListener(()=>Approach(2));
            ui.closeMarket.onClick.AddListener(CloseShop);ui.closeWardrobe.onClick.AddListener(CloseShop);
            ui.accept.onClick.AddListener(AcceptOrder);ui.deliver.onClick.AddListener(()=>Deliver());
            for(int i=0;i<ui.clothes.Length;i++){int k=i;ui.clothes[i].onClick.AddListener(()=>Wear(k));}
            HidePanels();Refresh();
        }
        void HidePanels(){ui.map.SetActive(false);ui.journey.SetActive(false);ui.city.SetActive(false);ui.market.SetActive(false);ui.wardrobe.SetActive(false);}
        public void GoToStation(){if(adventure.Location==OutdoorAdventure.Place.Meadow)adventure.WalkToHouse(4,StationX);}
        void Arrived(int index)
        {
            if(index!=4||adventure.Location!=OutdoorAdventure.Place.Meadow)return;
            adventure.BeginTrain();originCity=false;OpenMap();
        }
        void OpenMap()
        {
            phase=Phase.Map;ui.map.SetActive(true);ui.city.SetActive(false);
            ui.passenger.sprite=CurrentSprite();
            ui.mapPin.rectTransform.anchorMin=new Vector2(originCity?.76f:.18f,.68f);
            ui.mapPin.rectTransform.anchorMax=new Vector2(originCity?.82f:.24f,.79f);
            ui.mapPin.rectTransform.offsetMin=ui.mapPin.rectTransform.offsetMax=Vector2.zero;
            ui.mapHome.interactable=originCity;ui.mapCity.interactable=!originCity;
            Cue(ui.mapHome,originCity);Cue(ui.mapCity,!originCity);Refresh();ClearFocus();
        }
        public void CloseMap()
        {
            if(phase!=Phase.Map)return;
            ui.map.SetActive(false);adventure.ArriveByTrain(originCity);
            phase=originCity?Phase.City:Phase.Outside;ui.city.SetActive(originCity);Refresh();
        }
        public void ChooseDestination(bool city)
        {
            if(phase!=Phase.Map||city==originCity)return;
            targetCity=city;phase=Phase.Travelling;journeyTime=0;
            ui.map.SetActive(false);ui.journey.SetActive(true);ui.passenger.sprite=CurrentSprite();
            // Mirror artwork only: a negative scale on RectMask2D can cull the passenger.
            ui.train.localScale=Vector3.one;
            ui.train.Find("Train Painting").localScale=new Vector3(city?1:-1,1,1);
            var window=(RectTransform)ui.passenger.transform.parent;
            window.anchorMin=new Vector2(city?.236f:.534f,.503f);
            window.anchorMax=new Vector2(city?.466f:.764f,.772f);
            window.offsetMin=window.offsetMax=Vector2.zero;
            ui.passenger.rectTransform.localScale=new Vector3(city?1:-1,1,1);
            var departure=ui.journey.transform.Find("Departing Home").GetComponent<CitySymbol>();
            var arrival=ui.journey.transform.Find("Destination City").GetComponent<CitySymbol>();
            departure.symbol=city?CitySymbol.Kind.Home:CitySymbol.Kind.City;arrival.symbol=city?CitySymbol.Kind.City:CitySymbol.Kind.Home;
            departure.SetAllDirty();arrival.SetAllDirty();
            AdvanceJourney(0);Refresh();ClearFocus();
        }
        public void AdvanceJourney(float seconds)
        {
            if(phase!=Phase.Travelling||suspended)return;
            journeyTime=Mathf.Min(JourneySeconds,journeyTime+Mathf.Max(0,seconds));
            float fraction=journeyTime/JourneySeconds;
            float scroll=targetCity?fraction:1-fraction;
            ui.travellingScenery.anchorMin=new Vector2(-2*scroll,0);ui.travellingScenery.anchorMax=new Vector2(3-2*scroll,1);
            ui.travellingScenery.offsetMin=ui.travellingScenery.offsetMax=Vector2.zero;
            ui.train.anchoredPosition=new Vector2(0,Mathf.Sin(journeyTime*10)*2);
            ui.journeyProgress.type=UnityEngine.UI.Image.Type.Simple;
            ui.journeyProgress.rectTransform.anchorMin=Vector2.zero;
            ui.journeyProgress.rectTransform.anchorMax=new Vector2(fraction,1);
            ui.journeyProgress.rectTransform.offsetMin=ui.journeyProgress.rectTransform.offsetMax=Vector2.zero;
            if(journeyTime<JourneySeconds)return;
            ui.journey.SetActive(false);adventure.ArriveByTrain(targetCity);
            phase=targetCity?Phase.City:Phase.Outside;ui.city.SetActive(targetCity);
            if(targetCity){cityX=.36f;moving=false;pending=-1;ApplyCity(false);}
            Refresh();
        }
        public void Approach(int place)
        {
            if(phase!=Phase.City||adventure.Location!=OutdoorAdventure.Place.City||place<0||place>2)return;
            EndTouchMovement();
            pending=place;cityTarget=place==0?.36f:place==1?.98f:1.58f;moving=true;ClearFocus();
        }
        public void WalkToScreen(float x)
        {
            if(phase!=Phase.City)return;
            EndTouchMovement();
            cityTarget=Mathf.Clamp(cameraLeft+Mathf.Clamp01(x),.13f,1.78f);moving=true;pending=-1;
        }
        public void AdvanceCity(float direction,float seconds)
        {
            if(phase!=Phase.City||adventure.Location!=OutdoorAdventure.Place.City||suspended)return;
            if(TouchHeld)direction=touchDirection;
            float before=cityX,distance=Mathf.Max(0,seconds)*.24f;
            if(direction!=0){moving=false;pending=-1;cityX=Mathf.Clamp(cityX+Mathf.Sign(direction)*distance,.13f,1.78f);}
            else if(moving){cityX=Mathf.MoveTowards(cityX,cityTarget,distance);if(Mathf.Approximately(cityX,cityTarget))moving=false;}
            bool walked=Mathf.Abs(cityX-before)>.00001f;gait+=Mathf.Abs(cityX-before)*65;ApplyCity(walked,cityX>=before?1:-1);
            if(!moving&&pending>=0)
            {
                int place=pending;pending=-1;
                if(place==0){originCity=true;adventure.BoardCityTrain();OpenMap();}
                else {phase=place==1?Phase.Market:Phase.Wardrobe;ui.market.SetActive(place==1);ui.wardrobe.SetActive(place==2);GetComponent<ItalianVoice>()?.Say(place==1?"order_"+progress.crop+"_"+progress.quantity:"wardrobe");Refresh();}
            }
        }
        void ApplyCity(bool walking,int facing=1)
        {
            cameraLeft=Mathf.Clamp(cityX-.5f,0,.90f);
            ui.cityWorld.anchorMin=new Vector2(-cameraLeft,0);ui.cityWorld.anchorMax=new Vector2(1.90f-cameraLeft,1);ui.cityWorld.offsetMin=ui.cityWorld.offsetMax=Vector2.zero;
            ui.cityRabbit.rectTransform.anchorMin=new Vector2(cityX-cameraLeft-.08f,.15f);ui.cityRabbit.rectTransform.anchorMax=new Vector2(cityX-cameraLeft+.08f,.51f);
            ui.cityRabbit.rectTransform.offsetMin=ui.cityRabbit.rectTransform.offsetMax=Vector2.zero;ui.cityRabbit.sprite=CurrentSprite();ui.cityRabbit.SetGait(gait,facing,walking);
        }
        public void CloseShop()
        {
            if(phase!=Phase.Market&&phase!=Phase.Wardrobe)return;
            ui.market.SetActive(false);ui.wardrobe.SetActive(false);phase=Phase.City;Refresh();
        }
        public void AcceptOrder()
        {if(phase!=Phase.Market||progress.accepted)return;progress.accepted=true;progress.Store();Refresh();ClearFocus();}
        public bool Deliver()
        {
            if(phase!=Phase.Market||!CanDeliver)return false;
            int reward=progress.reward;
            bool sold=garden.DeliverHarvest(progress.crop,progress.quantity,()=>{progress.coins+=reward;progress.NewOffer();progress.Store(false);});
            if(sold){ui.feedback.text="+"+reward;noticeTime=2.5f;ui.feedback.gameObject.SetActive(true);}
            Refresh();ClearFocus();return sold;
        }
        public void Wear(int outfit)
        {
            if(phase!=Phase.Wardrobe||outfit<0||outfit>3)return;
            int who=Character;
            var learning=LearningProgress.Current??LearningProgress.Load();
            if(outfit>=2)
            {
                int index=(outfit-2)*2+who;
                if(!learning.rewardOwned[index]){if(!learning.OutfitUnlocked(outfit)||progress.coins<OutfitCost(outfit))return;progress.coins-=OutfitCost(outfit);learning.rewardOwned[index]=true;learning.Store(false);}
            }
            if(outfit==1&&!progress.owned[who])
            {if(progress.coins<OutfitPrice)return;progress.coins-=OutfitPrice;progress.owned[who]=true;}
            progress.equipped[who]=outfit;progress.Store();ApplyClothes();Refresh();ClearFocus();
        }
        public static int OutfitCost(int outfit)=>outfit<2?OutfitPrice:outfit==2?120:180;
        Material OutfitMaterial(int outfit)=>outfit==2?ui.goldOutfit:outfit==3?ui.emeraldOutfit:null;
        Sprite CurrentSprite()=>Character==1?(progress.equipped[1]>=1?ui.femaleOutfit:ui.female):(progress.equipped[0]>=1?ui.maleOutfit:ui.male);
        public Sprite SeatedSprite(Sprite original)
        {
            if(progress==null||progress.equipped[Character]==0)return original;
            var sprite=Character==1?ui.femaleCitySeated:ui.maleCitySeated;
            return sprite!=null?sprite:original;
        }
        public void ApplyClothes()
        {
            if(selection.IsChoosing)return;
            var sprite=CurrentSprite();
            bool changed=appliedOutfit!=progress.equipped[Character];appliedOutfit=progress.equipped[Character];var material=OutfitMaterial(appliedOutfit);
            foreach(var actor in actors)if(actor!=null){if(actor.sprite!=sprite)actor.sprite=sprite;if(changed)actor.material=material;}
            if(changed){ui.passenger.material=material;if(schoolController!=null){schoolController.UI.standing.material=material;schoolController.UI.seated.material=material;}if(homeActivities!=null)homeActivities.UI.seated.material=material;}
        }
        void Days(float days)
        {
            if(progress.accepted)return;
            progress.offerDays+=Mathf.Max(0,days);
            if(progress.offerDays>=1){progress.NewOffer();progress.Store();Refresh();}
        }
        void Update()
        {
            var guide=GetComponent<GameGuide>();if(guide!=null&&(guide.IntroOpen||guide.MenuOpen))return;
            if(suspended)return;
            if((phase!=Phase.Outside)&&adventure.Location!=OutdoorAdventure.Place.Train&&adventure.Location!=OutdoorAdventure.Place.City)
            {phase=Phase.Outside;HidePanels();}
            var keys=Keyboard.current;
            if(keys!=null&&keys.escapeKey.wasPressedThisFrame){if(phase==Phase.Map)CloseMap();else CloseShop();}
            float direction=keys==null?0:(keys.rightArrowKey.isPressed?1:0)-(keys.leftArrowKey.isPressed?1:0);
            AdvanceJourney(Time.deltaTime);AdvanceCity(direction,Time.deltaTime);
            if(noticeTime>0){noticeTime-=Time.deltaTime;if(noticeTime<=0)ui.feedback.gameObject.SetActive(false);}
            refreshTime-=Time.deltaTime;if(refreshTime<=0){refreshTime=.2f;Refresh();}
        }
        void LateUpdate()=>ApplyClothes();
        public void Refresh()
        {
            if(progress==null||garden.Progress==null)return;
            bool meadow=adventure.Location==OutdoorAdventure.Place.Meadow;
            ui.stationSign.gameObject.SetActive(meadow);
            Cue(ui.station,meadow);Cue(ui.cityStation,phase==Phase.City);Cue(ui.marketDoor,phase==Phase.City);Cue(ui.boutiqueDoor,phase==Phase.City);
            foreach(var door in new[]{ui.cityStation,ui.marketDoor,ui.boutiqueDoor})
            {door.interactable=phase==Phase.City;door.transform.Find("Action Picture").gameObject.SetActive(phase==Phase.City);}
            bool learningOpen=GetComponent<LearningJourney>()?.IsOpen??false;
            ui.wallet.SetActive(!learningOpen&&adventure.Location!=OutdoorAdventure.Place.Selection&&phase!=Phase.Travelling&&phase!=Phase.Map);
            ui.balance.text=progress.coins.ToString();
            ui.orderBadge.SetActive(!learningOpen&&progress.accepted&&phase!=Phase.Market&&phase!=Phase.Wardrobe&&phase!=Phase.Map&&phase!=Phase.Travelling&&adventure.Location!=OutdoorAdventure.Place.Selection);
            int stock=garden.Progress.harvests[progress.crop];
            RewardArt.Crop(ui.orderCrop,progress.crop);RewardArt.Crop(ui.badgeCrop,progress.crop);
            ui.orderQuantity.text="× "+progress.quantity;ui.orderReward.text=progress.reward.ToString();ui.orderStock.text=stock+" / "+progress.quantity;
            ui.orderBadgeCount.text=Mathf.Min(stock,progress.quantity)+" / "+progress.quantity;
            ui.accept.gameObject.SetActive(!progress.accepted);ui.deliver.gameObject.SetActive(progress.accepted);ui.deliver.interactable=CanDeliver;
            Cue(ui.accept,!progress.accepted);Cue(ui.deliver,CanDeliver);
            var learning=LearningProgress.Current??LearningProgress.Load();
            for(int i=0;i<ui.clothes.Length;i++)
            {
                ui.clothesPictures[i].sprite=Character==0?(i==0?ui.male:ui.maleOutfit):(i==0?ui.female:ui.femaleOutfit);ui.clothesPictures[i].material=OutfitMaterial(i);
                bool owned=i==0||(i==1?progress.owned[Character]:learning.rewardOwned[(i-2)*2+Character]),equipped=progress.equipped[Character]==i;
                bool unlocked=owned||learning.OutfitUnlocked(i);
                ui.prices[i].text=equipped?"":owned?"":OutfitCost(i).ToString();
                ui.clothes[i].interactable=!equipped&&unlocked&&(owned||progress.coins>=OutfitCost(i));
                Cue(ui.clothes[i],ui.clothes[i].interactable);
                var mark=ui.clothes[i].transform.Find("Equipped");if(mark!=null)mark.gameObject.SetActive(equipped);
                var cost=ui.clothes[i].transform.Find("Price Coin");if(cost!=null)cost.gameObject.SetActive(!owned);
                var locked=ui.clothes[i].transform.Find("School Lock");if(locked!=null)locked.gameObject.SetActive(!unlocked);
            }
            ui.cityPainting.color=Color.Lerp(Color.white,new Color(.62f,.65f,.86f),clock.NightAmount*.65f);
            ui.countryPainting.color=Color.Lerp(Color.white,new Color(.35f,.43f,.64f),clock.NightAmount);
            if(ui.homeExterior!=null)ui.homeExterior.color=ui.countryPainting.color;
        }
        public static int CropCell(int crop)=>crop==0?4:crop==1?6:crop==2?5:7;
        static void Cue(UnityEngine.UI.Button button,bool on){button.GetComponent<ActionCue>()?.SetHighlighted(on&&button.gameObject.activeSelf);}
        static void ClearFocus(){if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);}
        void OnApplicationPause(bool paused){suspended=paused;if(paused)progress?.Store();}
        void OnApplicationQuit()=>progress?.Store();
        void OnDestroy(){if(adventure!=null)adventure.HouseReached-=Arrived;if(clock!=null)clock.GameDaysAdvanced-=Days;}
    }
}
