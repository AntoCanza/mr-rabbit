using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using MrRabbit;

public static class AddCityTravel
{
    static TMPro.TMP_FontAsset font;
    static Texture2D crops;
    static readonly Color cream=new Color32(255,247,226,255),ink=new Color32(65,68,76,255),mint=new Color32(200,231,215,255);
    static Sprite round;
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first");
        var a=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        if(a.GetComponent<CityController>()!=null)throw new Exception("Already installed. Edit existing widgets in place.");
        AssetDatabase.Refresh();
        foreach(string name in new[]{"CityPanorama","CountryStation","Train","HomeExterior","MrRabbitCity","MissRabbitCity","MrRabbitCitySeated","MissRabbitCitySeated"})
        {
            var path="Assets/Art/City/"+name+".png";
            var imp=(TextureImporter)AssetImporter.GetAtPath(path);
            if(imp==null)throw new Exception("Missing art: "+path);
            imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.alphaIsTransparency=true;
            imp.mipmapEnabled=false;imp.isReadable=false;imp.maxTextureSize=2048;imp.textureCompression=TextureImporterCompression.Compressed;
            imp.SetPlatformTextureSettings(new TextureImporterPlatformSettings{name="Android",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6});
            imp.SaveAndReimport();
        }
        font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        crops=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/GardenAtlas.png");
        round=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        var canvas=a.transform.parent;var safe=a.transform;
        var meadow=canvas.Find("Meadow Viewport/Meadow");
        var world=(RectTransform)meadow.Find("Scrolling Landscape");var friends=(RectTransform)meadow.Find("Meadow Friends");
        // Extend the existing world by exactly one viewport on the left; preserve every old world position.
        foreach(var layer in new[]{world,friends})
        {
            for(int i=0;i<layer.childCount;i++)
            {
                var child=layer.GetChild(i) as RectTransform;if(child==null)continue;
                var min=child.anchorMin;var max=child.anchorMax;
                min.x=(min.x*5.5f+1)/6.5f;max.x=(max.x*5.5f+1)/6.5f;child.anchorMin=min;child.anchorMax=max;
            }
            layer.anchorMin=new Vector2(-4.25f,0);layer.anchorMax=new Vector2(2.25f,1);layer.offsetMin=layer.offsetMax=Vector2.zero;
        }
        var ui=new CityController.Widgets();
        ui.countryPainting=Image("Countryside Station",world,0,0,1.28f/6.5f,1,Color.white);ui.countryPainting.sprite=Art("CountryStation");
        ui.countryPainting.transform.SetSiblingIndex(world.Find("School Meadow").GetSiblingIndex()+1);
        ui.station=Button("Board Country Train",world,World(-3.94f),.28f,World(-3.50f),.72f,Color.clear);
        AddCue(ui.station,CitySymbol.Kind.Train,.25f,.24f,.75f,.60f,true);
        ui.stationSign=Button("Train Sign",safe,.15f,.82f,.25f,.96f,cream);AddCue(ui.stationSign,CitySymbol.Kind.Train,.12f,.08f,.88f,.92f,false);

        var house=Image("Our House Exterior",world,World(1.73f),.26f,World(2.25f),.89f,Color.white);house.sprite=Art("HomeExterior");house.preserveAspect=true;
        ui.homeExterior=house;
        house.transform.SetSiblingIndex(world.Find("Home Sign").GetSiblingIndex());
        var sign=world.Find("Home Sign").GetComponent<UnityEngine.UI.Button>();
        foreach(Transform child in sign.transform)child.gameObject.SetActive(false);
        Place((RectTransform)sign.transform,World(1.99f),.28f,World(2.22f),.52f);sign.GetComponent<UnityEngine.UI.Image>().color=Color.clear;
        var homeCue=sign.GetComponent<ActionCue>();if(homeCue!=null)UnityEngine.Object.DestroyImmediate(homeCue);
        AddCue(sign,CitySymbol.Kind.Home,.12f,.06f,.88f,.94f,false);
        var post=world.Find("Signpost");if(post!=null)post.gameObject.SetActive(false);

        var city=Image("City Viewport",canvas,0,0,1,1,Color.white);city.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();ui.city=city.gameObject;
        ui.cityWorld=Rect("City Streets",city.transform,0,0,1.9f,1);
        ui.cityPainting=Image("Sparkling City",ui.cityWorld,0,0,1,1,Color.white);ui.cityPainting.sprite=Art("CityPanorama");
        var ground=Image("Walk Along Pavement",city.transform,0,0,1,.28f,Color.clear);ground.raycastTarget=true;
        ui.cityStation=Door("City Station",ui.cityWorld,.19f,CitySymbol.Kind.Train);
        ui.marketDoor=Door("Market",ui.cityWorld,.515f,CitySymbol.Kind.Market);
        ui.boutiqueDoor=Door("Boutique",ui.cityWorld,.832f,CitySymbol.Kind.Shirt);
        ui.cityRabbit=Rect("City Rabbit",city.transform,.10f,.15f,.26f,.51f).gameObject.AddComponent<RabbitGraphic>();ui.cityRabbit.preserveAspect=true;ui.cityRabbit.raycastTarget=false;

        var map=Image("Railway Map",canvas,0,0,1,1,new Color32(239,242,219,255));map.raycastTarget=true;ui.map=map.gameObject;
        var mapClose=Button("Leave Train",map.transform,.025f,.82f,.12f,.96f,cream);AddCue(mapClose,CitySymbol.Kind.Back,.12f,.12f,.88f,.88f,false);ui.mapClose=mapClose;
        // The two destinations are visual postcards, with only their short names below.
        ui.mapHome=Button("Village Destination",map.transform,.07f,.27f,.43f,.73f,cream);
        ui.mapCity=Button("City Destination",map.transform,.61f,.27f,.94f,.73f,cream);
        var homePicture=Rect("Village Pictures",ui.mapHome.transform,.03f,.03f,.97f,.97f);
        var green=Image("Village Lawn",homePicture,0,0,1,1,new Color32(198,222,164,255));green.sprite=round;green.type=UnityEngine.UI.Image.Type.Sliced;
        var homeMini=Image("Home",homePicture,.30f,.35f,.67f,.98f,Color.white);homeMini.sprite=Art("HomeExterior");homeMini.preserveAspect=true;
        var neighbours=Image("Two Cottages",homePicture,.02f,.01f,.58f,.40f,Color.white);neighbours.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Outdoors/Neighborhood.png");
        var school=Image("School",homePicture,.03f,.41f,.29f,.86f,Color.white);school.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/School/SchoolMeadow.png");school.preserveAspect=true;
        Symbol("Garden",homePicture,CitySymbol.Kind.Garden,.70f,.10f,.95f,.43f);
        var cityPicture=Image("City Picture",ui.mapCity.transform,.03f,.03f,.97f,.97f,Color.white);cityPicture.sprite=Art("CityPanorama");
        Cue(ui.mapHome,homePicture,true);Cue(ui.mapCity,cityPicture.rectTransform,true);
        Text("Home Name",map.transform,.11f,.20f,.39f,.27f,"Casa",32);Text("City Name",map.transform,.65f,.20f,.91f,.27f,"Città",32);
        for(int i=0;i<19;i++){float x=.25f+i*.028f;Image("Track Sleeper",map.transform,x,.12f,x+.009f,.17f,new Color32(164,142,109,255));}
        Image("Rail",map.transform,.24f,.13f,.79f,.138f,ink);Image("Rail",map.transform,.24f,.155f,.79f,.163f,ink);
        ui.mapPin=Image("Current Stop",map.transform,.18f,.68f,.24f,.79f,Color.clear);Symbol("Pin",ui.mapPin.transform,CitySymbol.Kind.Pin,0,0,1,1);

        var journey=Image("Train Journey",canvas,0,0,1,1,new Color32(173,219,235,255));journey.raycastTarget=true;journey.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();ui.journey=journey.gameObject;
        ui.travellingScenery=Rect("Passing Landscape",journey.transform,0,0,3,1);
        var field=Image("Passing Village",ui.travellingScenery,0,0,.55f,1,Color.white);field.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Outdoors/Neighborhood.png");
        var skyline=Image("Approaching City",ui.travellingScenery,.55f,0,1,1,Color.white);skyline.sprite=Art("CityPanorama");
        Image("Track Bed",journey.transform,0,.16f,1,.22f,new Color32(147,135,117,255));
        for(int i=0;i<28;i++){float x=i/28f;Image("Sleeper",journey.transform,x,.15f,x+.015f,.20f,new Color32(102,91,76,255));}
        Image("Travel Rail",journey.transform,0,.20f,1,.207f,ink);
        ui.train=Rect("Passenger Train",journey.transform,.055f,.18f,.945f,.67f);
        var trainArt=Image("Train Painting",ui.train,0,0,1,1,Color.white);trainArt.sprite=Art("Train");
        var window=Image("Passenger Window",ui.train,.236f,.503f,.466f,.772f,Color.clear);window.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        ui.passenger=Image("Our Passenger",window.transform,.17f,-.95f,.78f,.98f,Color.white);ui.passenger.preserveAspect=true;
        var travelProgress=Image("Journey Progress Track",journey.transform,.17f,.075f,.83f,.09f,cream);
        ui.journeyProgress=Image("Journey Progress",travelProgress.transform,0,0,1,1,new Color32(67,154,150,255));ui.journeyProgress.sprite=round;ui.journeyProgress.type=UnityEngine.UI.Image.Type.Filled;ui.journeyProgress.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;
        Symbol("Departing Home",journey.transform,CitySymbol.Kind.Home,.08f,.035f,.14f,.13f);Symbol("Destination City",journey.transform,CitySymbol.Kind.City,.86f,.035f,.92f,.13f);

        var market=Image("Market Order",canvas,.19f,.19f,.81f,.86f,cream);market.raycastTarget=true;ui.market=market.gameObject;
        ui.closeMarket=Button("Leave Market",market.transform,.015f,.80f,.13f,.97f,Color.clear);AddCue(ui.closeMarket,CitySymbol.Kind.Back,.05f,.05f,.95f,.95f,false);
        ui.customer=Image("Fruit Buyer",market.transform,.04f,.20f,.31f,.76f,Color.white);ui.customer.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/Coccodella.png");ui.customer.preserveAspect=true;
        ui.orderCrop=Crop("Requested Produce",market.transform,0,.33f,.49f,.53f,.84f);
        ui.orderQuantity=Text("Requested Amount",market.transform,.52f,.53f,.75f,.78f,"× 3",55);
        Symbol("Reward Coin",market.transform,CitySymbol.Kind.Coin,.78f,.55f,.90f,.74f);
        ui.orderReward=Text("Order Reward",market.transform,.79f,.35f,.89f,.53f,"6",40);
        ui.orderStock=Text("Basket Quantity",market.transform,.35f,.29f,.70f,.46f,"0 / 3",38);
        ui.accept=Button("Accept Request",market.transform,.40f,.055f,.66f,.25f,mint);AddCue(ui.accept,CitySymbol.Kind.Check,.20f,.03f,.80f,.97f,true);
        ui.deliver=Button("Deliver Requested Produce",market.transform,.40f,.055f,.66f,.25f,mint);AddCue(ui.deliver,CitySymbol.Kind.Arrow,.20f,.03f,.80f,.97f,true);

        var wardrobe=Image("Clothing Boutique",canvas,.16f,.13f,.84f,.89f,cream);wardrobe.raycastTarget=true;ui.wardrobe=wardrobe.gameObject;
        ui.closeWardrobe=Button("Leave Boutique",wardrobe.transform,.015f,.82f,.12f,.97f,Color.clear);AddCue(ui.closeWardrobe,CitySymbol.Kind.Back,.05f,.05f,.95f,.95f,false);
        Symbol("Boutique Sign",wardrobe.transform,CitySymbol.Kind.Shirt,.46f,.82f,.56f,.97f);
        ui.clothes=new UnityEngine.UI.Button[2];ui.clothesPictures=new UnityEngine.UI.Image[2];ui.prices=new TMPro.TMP_Text[2];
        for(int i=0;i<2;i++)
        {
            float x=.13f+i*.41f;var b=Button("Outfit "+i,wardrobe.transform,x,.075f,x+.33f,.79f,i==0?mint:new Color32(228,216,240,255));ui.clothes[i]=b;
            var preview=Image("Outfit Preview",b.transform,.09f,.17f,.91f,.97f,Color.white);preview.preserveAspect=true;ui.clothesPictures[i]=preview;Cue(b,preview.rectTransform,true);
            Symbol("Price Coin",b.transform,CitySymbol.Kind.Coin,.26f,.025f,.43f,.15f);ui.prices[i]=Text("Price",b.transform,.47f,.015f,.71f,.15f,"6",36);
            Symbol("Equipped",b.transform,CitySymbol.Kind.Check,.39f,.02f,.61f,.16f);
        }

        var wallet=Image("Coin Wallet",safe,.84f,.89f,.98f,.975f,new Color(1,.97f,.88f,.90f));ui.wallet=wallet.gameObject;
        Symbol("Coins",wallet.transform,CitySymbol.Kind.Coin,.025f,.08f,.32f,.92f);ui.balance=Text("Balance",wallet.transform,.35f,.08f,.94f,.92f,"0",30);
        var badge=Image("Accepted Produce Request",safe,.68f,.89f,.825f,.975f,new Color(1,.97f,.88f,.90f));ui.orderBadge=badge.gameObject;
        ui.badgeCrop=Crop("Needed Crop",badge.transform,0,.02f,.06f,.34f,.94f);ui.orderBadgeCount=Text("Request Count",badge.transform,.36f,.06f,.98f,.94f,"0 / 3",26);
        ui.feedback=Text("Earned Coins",safe,.85f,.79f,.98f,.88f,"",36);ui.feedback.color=new Color32(60,126,92,255);ui.feedback.gameObject.SetActive(false);
        ui.male=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/MrRabbit.png");ui.female=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/MissRabbit.png");ui.maleOutfit=Art("MrRabbitCity");ui.femaleOutfit=Art("MissRabbitCity");
        ui.maleCitySeated=Art("MrRabbitCitySeated");ui.femaleCitySeated=Art("MissRabbitCitySeated");
        foreach(var view in new[]{ui.city,ui.map,ui.journey,ui.market,ui.wardrobe})view.transform.SetSiblingIndex(safe.GetSiblingIndex());
        foreach(var view in new[]{ui.city,ui.map,ui.journey,ui.market,ui.wardrobe})view.SetActive(false);
        var controller=a.gameObject.AddComponent<CityController>();controller.Configure(a,a.GetComponent<GardenController>(),a.GetComponent<DayNightCycle>(),a.GetComponent<CharacterSelection>(),ui);
        ground.gameObject.AddComponent<CityPointerInput>().Configure(controller);
        InstallKitchen(a);
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(a.gameObject.scene);EditorSceneManager.SaveScene(a.gameObject.scene);AssetDatabase.SaveAssets();
        return "Installed station, six-second visible train journey, two-stop visual map, walkable city, produce orders, saved coins and outfits; visible home exterior and visual kitchen.";
    }
    static void InstallKitchen(OutdoorAdventure a)
    {
        var home=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var ui=home.UI;
        Place((RectTransform)ui.kitchenPanel.transform,.07f,.38f,.93f,.96f);
        ui.status.gameObject.SetActive(false);
        Place((RectTransform)ui.close.transform,.91f,.83f,.99f,.98f);HideText(ui.close.transform);AddCue(ui.close,CitySymbol.Kind.Back,.10f,.06f,.90f,.94f,true);
        var counts=new TMPro.TMP_Text[4];var meals=new TMPro.TMP_Text[4];var selected=new UnityEngine.UI.Image[4];
        for(int i=0;i<4;i++)
        {
            float x=.025f+i*.245f;var button=ui.recipes[i];Place((RectTransform)button.transform,x,.34f,x+.225f,.81f);
            ui.recipeLabels[i].gameObject.SetActive(false);var old=button.transform.Find("Ingredient Slot");if(old!=null)old.gameObject.SetActive(false);
            var ingredient=Crop("Ingredient Picture",button.transform,i,.05f,.37f,.43f,.94f);
            var dish=Rect("Prepared Dish",button.transform,.58f,.41f,.95f,.90f).gameObject.AddComponent<HearthGraphic>();dish.SetDish(i);dish.raycastTarget=false;
            Symbol("Becomes",button.transform,CitySymbol.Kind.Arrow,.43f,.53f,.59f,.77f);
            counts[i]=Text("Ingredient Count",button.transform,.06f,.04f,.43f,.32f,"0",34);meals[i]=Text("Meal Count",button.transform,.59f,.04f,.95f,.32f,"0",34);
            selected[i]=Image("Selected Recipe",button.transform,.03f,.015f,.97f,.035f,new Color32(52,145,132,255));Cue(button,ingredient.rectTransform,true);
        }
        Place((RectTransform)ui.cook.transform,.22f,.055f,.44f,.28f);HideText(ui.cook.transform);AddCue(ui.cook,CitySymbol.Kind.Pot,.27f,.04f,.73f,.96f,true);
        Place((RectTransform)ui.eat.transform,.56f,.055f,.78f,.28f);HideText(ui.eat.transform);var serving=AddCue(ui.eat,CitySymbol.Kind.Plate,.27f,.04f,.73f,.96f,true).GetComponent<UnityEngine.UI.Image>();
        var explanation=ui.kitchenPanel.transform.Find("Explanation");if(explanation!=null)explanation.gameObject.SetActive(false);
        foreach(var text in ui.kitchenPanel.GetComponentsInChildren<TMPro.TMP_Text>(true))
            if(text!=counts[0]&&!counts.Contains(text)&&!meals.Contains(text))text.gameObject.SetActive(false);
        var picture=Image("Cooked Dish Glow",ui.cook.transform,.08f,.12f,.20f,.88f,Color.white);picture.enabled=false;
        home.gameObject.AddComponent<KitchenVisuals>().Configure(home,a.GetComponent<GardenController>(),counts,meals,selected,picture);
        var kitchenHint=home.transform.Find("Kitchen Hint");if(kitchenHint!=null)kitchenHint.gameObject.SetActive(false);
    }
    static void HideText(Transform root){foreach(var t in root.GetComponentsInChildren<TMPro.TMP_Text>(true))t.gameObject.SetActive(false);}
    static float World(float x)=>(x-OutdoorAdventure.WorldLeft)/OutdoorAdventure.WorldWidth;
    static Sprite Art(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/City/"+name+".png");
    static UnityEngine.UI.Button Door(string name,Transform parent,float center,CitySymbol.Kind symbol)
    {var b=Button(name,parent,center-.06f,.23f,center+.06f,.68f,Color.clear);AddCue(b,symbol,.23f,.48f,.77f,.95f,false);return b;}
    static RectTransform Rect(string name,Transform parent,float x,float y,float r,float t)
    {var o=new GameObject(name,typeof(RectTransform));var rect=o.GetComponent<RectTransform>();rect.SetParent(parent,false);Place(rect,x,y,r,t);return rect;}
    static void Place(RectTransform rect,float x,float y,float r,float t){rect.anchorMin=new Vector2(x,y);rect.anchorMax=new Vector2(r,t);rect.offsetMin=rect.offsetMax=Vector2.zero;}
    static UnityEngine.UI.Image Image(string name,Transform parent,float x,float y,float r,float t,Color c)
    {var im=Rect(name,parent,x,y,r,t).gameObject.AddComponent<UnityEngine.UI.Image>();im.color=c;im.raycastTarget=false;return im;}
    static UnityEngine.UI.Button Button(string name,Transform parent,float x,float y,float r,float t,Color c)
    {var im=Image(name,parent,x,y,r,t,c);im.raycastTarget=true;im.sprite=round;im.type=UnityEngine.UI.Image.Type.Sliced;var b=im.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=im;b.navigation=new UnityEngine.UI.Navigation{mode=UnityEngine.UI.Navigation.Mode.None};return b;}
    static TMPro.TMP_Text Text(string name,Transform parent,float x,float y,float r,float t,string words,int size)
    {var text=Rect(name,parent,x,y,r,t).gameObject.AddComponent<TMPro.TextMeshProUGUI>();text.font=font;text.text=words;text.color=ink;text.fontSize=size;text.enableAutoSizing=true;text.fontSizeMin=16;text.fontSizeMax=size;text.alignment=TMPro.TextAlignmentOptions.Center;text.raycastTarget=false;return text;}
    static CitySymbol Symbol(string name,Transform parent,CitySymbol.Kind kind,float x,float y,float r,float t)
    {var symbol=Rect(name,parent,x,y,r,t).gameObject.AddComponent<CitySymbol>();symbol.symbol=kind;symbol.raycastTarget=false;return symbol;}
    static ActionCue AddCue(UnityEngine.UI.Button b,CitySymbol.Kind symbol,float x,float y,float r,float t,bool controlled)
    {var slot=Rect("Action Picture",b.transform,x,y,r,t);Symbol("Symbol",slot,symbol,0,0,1,1);return Cue(b,slot,controlled);}
    static ActionCue Cue(UnityEngine.UI.Button b,RectTransform visual,bool controlled)
    {var glow=Image("Action Glow",b.transform,-.025f,-.025f,1.025f,1.025f,new Color(1,.8f,.3f,.3f));glow.sprite=round;glow.type=UnityEngine.UI.Image.Type.Sliced;glow.transform.SetAsFirstSibling();var cue=b.gameObject.AddComponent<ActionCue>();cue.Configure(visual,glow,controlled);return cue;}
    static UnityEngine.UI.RawImage Crop(string name,Transform parent,int crop,float x,float y,float r,float t)
    {var slot=Rect(name,parent,x,y,r,t);var im=Rect("Art",slot,0,0,1,1).gameObject.AddComponent<UnityEngine.UI.RawImage>();im.texture=crops;im.raycastTarget=false;var fit=im.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;GardenArt.Apply(im,CityController.CropCell(crop));return im;}
}
