using System;
using System.Linq;
using UnityEngine;
using MrRabbit;
public static class VerifyCityTravel
{
    static void Check(bool value,string message){if(!value)throw new Exception(message);}
    static void Walk(OutdoorAdventure a){for(int i=0;i<2000;i++)a.Advance(0,.02f);}
    static void CityWalk(CityController c){for(int i=0;i<700;i++)c.AdvanceCity(0,.02f);}
    public static string Run()
    {
        var s=UnityEngine.Object.FindFirstObjectByType<CharacterSelection>();var c=s.GetComponent<CityController>();var a=s.GetComponent<OutdoorAdventure>();var g=s.GetComponent<GardenController>();
        var rabbit=UnityEngine.Object.FindFirstObjectByType<RabbitWalker>(FindObjectsInactive.Include);
        string city=JsonUtility.ToJson(c.Progress),garden=JsonUtility.ToJson(g.Progress);
        bool hadCity=PlayerPrefs.HasKey(CityProgress.SaveKey),hadGarden=PlayerPrefs.HasKey(GardenController.SaveKey);
        string savedCity=PlayerPrefs.GetString(CityProgress.SaveKey),savedGarden=PlayerPrefs.GetString(GardenController.SaveKey);
        var random=UnityEngine.Random.state;
        try
        {
            var legacy=new CityProgress{crop=2,quantity=6,reward=12,accepted=true,coins=42};legacy.owned[0]=true;legacy.equipped[0]=1;legacy.Store();
            var migrated=CityProgress.Load();
            Check(migrated.accepted&&migrated.crop==2&&migrated.quantity==6&&migrated.reward==6&&migrated.coins==42&&migrated.owned[0]&&migrated.equipped[0]==1,"Repricing lost existing progress");
            for(int i=0;i<30;i++){migrated.NewOffer();Check(migrated.reward==migrated.quantity,"New offer does not pay one coin per unit");}
            foreach(bool female in new[]{false,true})foreach(bool rollers in new[]{false,true})
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(new CityProgress()),c.Progress);
                s.ShowSelection();if(female)s.ChooseFemale();else s.ChooseMale();
                rabbit.WalkToExit();for(int i=0;i<500;i++)rabbit.Advance(Vector2.zero,.02f);
                if(rollers)a.ChooseRollers();else a.ChooseShoes();c.GoToStation();Walk(a);
                Check(a.Location==OutdoorAdventure.Place.Train&&c.State==CityController.Phase.Map,"Station cannot be reached");
                c.ChooseDestination(false);Check(c.State==CityController.Phase.Map,"Travels to current location");
                c.CloseMap();Check(a.Location==OutdoorAdventure.Place.Meadow,"Boarding cancellation failed");
                c.GoToStation();Walk(a);c.ChooseDestination(true);c.AdvanceJourney(3);
                Check(c.State==CityController.Phase.Travelling&&c.JourneyTime==3&&c.UI.passenger.sprite!=null,"Missing passenger or journey completed too soon");
                c.AdvanceJourney(3);Check(a.Location==OutdoorAdventure.Place.City&&c.State==CityController.Phase.City,"City arrival failed");
                Check(s.GetComponent<DayNightCycle>().IsAdvancing,"City clock stopped");
                c.Approach(1);CityWalk(c);Check(c.State==CityController.Phase.Market,"Market unreachable");
                c.Progress.crop=3;c.Progress.quantity=3;c.Progress.reward=3;c.Progress.accepted=false;
                g.Progress.harvests[3]=2;c.AcceptOrder();int money=c.Progress.coins;
                Check(!c.Deliver()&&g.Progress.harvests[3]==2&&c.Progress.coins==money,"Insufficient delivery changed inventory");
                s.GetComponent<DayNightCycle>().Advance(240);
                Check(c.Progress.accepted&&c.Progress.crop==3&&c.Progress.quantity==3,"Accepted order expired");
                for(int i=0;i<4;i++)g.Progress.harvests[i]=8+i;
                Check(c.Deliver(),"Valid delivery failed");Check(c.Progress.coins==money+3&&g.Progress.harvests[3]==8,"Wrong sale quantity or reward");
                Check(g.Progress.harvests[0]==8&&g.Progress.harvests[1]==9&&g.Progress.harvests[2]==10,"Sold unrelated produce");
                Check(!c.Deliver(),"Double delivery granted money");
                c.CloseShop();c.Approach(2);CityWalk(c);Check(c.State==CityController.Phase.Wardrobe,"Boutique unreachable");
                int who=female?1:0;c.Progress.coins=0;c.Wear(1);Check(!c.Progress.owned[who],"Unaffordable purchase allowed");
                c.Progress.coins=59;c.Wear(1);Check(!c.Progress.owned[who]&&c.Progress.coins==59,"Outfit bought below 60 coins");
                c.Progress.coins=60;c.Wear(1);Check(c.Progress.owned[who]&&c.Progress.equipped[who]==1&&c.Progress.coins==0,"Purchase/equip failed");
                c.Wear(0);c.Wear(1);Check(c.Progress.equipped[who]==1&&c.Progress.coins==0,"Owned outfit charged again");
                Check(a.PlayerSprite==(female?c.UI.femaleOutfit:c.UI.maleOutfit),"Outfit did not reach player");
                var reload=CityProgress.Load();Check(reload.owned[who]&&reload.equipped[who]==1&&reload.coins==0,"Outfit save failed");
                c.CloseShop();c.Approach(0);CityWalk(c);c.CloseMap();Check(a.Location==OutdoorAdventure.Place.City,"City map cancellation failed");
                c.Approach(0);CityWalk(c);c.ChooseDestination(false);c.AdvanceJourney(6);
                Check(a.Location==OutdoorAdventure.Place.Meadow&&a.UsingRollers==rollers,"Return lost location or footwear");
                a.WalkHome();Walk(a);Check(a.Location==OutdoorAdventure.Place.Home,"Home entry failed");
                var h=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);h.ToggleTV();
                for(int i=0;i<1500;i++)rabbit.Advance(Vector2.zero,.02f);h.Advance(0);
                Check(h.IsWatching&&h.UI.seated.sprite==(female?c.UI.femaleCitySeated:c.UI.maleCitySeated),"Seated outfit changed");h.CancelActivity();
            }
            return "PASS: both characters and footwear, station and map cancellation, six-second travel with visible passenger, city movement, insufficient/exact/double deliveries, pinned orders, saved money and outfits, free re-equipping, return and home entry. Original saves restored.";
        }
        finally
        {
            JsonUtility.FromJsonOverwrite(city,c.Progress);JsonUtility.FromJsonOverwrite(garden,g.Progress);
            if(hadCity)PlayerPrefs.SetString(CityProgress.SaveKey,savedCity);else PlayerPrefs.DeleteKey(CityProgress.SaveKey);
            if(hadGarden)PlayerPrefs.SetString(GardenController.SaveKey,savedGarden);else PlayerPrefs.DeleteKey(GardenController.SaveKey);
            PlayerPrefs.Save();UnityEngine.Random.state=random;s.ShowSelection();s.ChooseMale();c.ApplyClothes();
        }
    }
}
