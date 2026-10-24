using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;

public static class SimplifyVisualUI
{
    const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
    static Texture2D actions;
    static T Field<T>(object owner,string name)=>(T)owner.GetType().GetField(name,Private).GetValue(owner);
    static void Hide(Transform root,params string[] names)
    {foreach(var text in root.GetComponentsInChildren<TMPro.TMP_Text>(true))if(names.Contains(text.name))text.gameObject.SetActive(false);}
    static RectTransform Rect(string name,Transform parent,float x,float y,float right,float top)
    {
        var old=parent.Find(name);var r=old!=null?(RectTransform)old:new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
        if(old==null)r.SetParent(parent,false);Place(r,x,y,right,top);return r;
    }
    static void Place(RectTransform r,float x,float y,float right,float top)
    {r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(right,top);r.offsetMin=r.offsetMax=Vector2.zero;}
    static void Art(string name,Transform parent,int cell,float x=.12f,float y=.12f,float right=.88f,float top=.88f)
    {
        var slot=Rect(name,parent,x,y,right,top);var r=Rect("Art",slot,0,0,1,1);
        var image=r.GetComponent<RawImage>()??r.gameObject.AddComponent<RawImage>();image.texture=actions;image.raycastTarget=false;
        var fit=r.GetComponent<AspectRatioFitter>()??r.gameObject.AddComponent<AspectRatioFitter>();
        fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;GardenArt.Apply(image,cell);
    }
    static void Symbol(Transform parent,CitySymbol.Kind kind)
    {
        var r=Rect("Action Picture",parent,.12f,.12f,.88f,.88f);
        var icon=r.GetComponent<CitySymbol>()??r.gameObject.AddComponent<CitySymbol>();icon.raycastTarget=false;icon.symbol=kind;icon.SetAllDirty();
    }
    static void InFrontOfActors(Transform action)
    {
        var layer=action.GetComponent<Canvas>();if(layer==null)layer=action.gameObject.AddComponent<Canvas>();
        layer.overrideSorting=true;layer.sortingOrder=10;
        if(action.GetComponent<GraphicRaycaster>()==null)action.gameObject.AddComponent<GraphicRaycaster>();
        if(action.GetComponent<WorldActionLayer>()==null)action.gameObject.AddComponent<WorldActionLayer>();
    }
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
        actions=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/ActionIcons.png");
        var travel=UnityEngine.Object.FindFirstObjectByType<OutdoorAdventure>(FindObjectsInactive.Include);
        var canvas=travel.transform.parent;var school=travel.GetComponent<SchoolController>();
        // The continuous ground must also be the input surface across every landmark.
        canvas.Find("Meadow Viewport/Meadow/Scrolling Landscape/One Shared Landscape").GetComponent<RawImage>().raycastTarget=true;
        var ui=school.UI;var removed=ui.subjects.transform.Find("Lesson 2");
        if(removed!=null)UnityEngine.Object.DestroyImmediate(removed.gameObject);
        ui.lessons=ui.lessons.Take(2).ToArray();ui.lessonCues=ui.lessonCues.Take(2).ToArray();
        for(int i=0;i<2;i++)Place((RectTransform)ui.lessons[i].transform,.15f+i*.4f,.04f,.45f+i*.4f,.96f);
        EditorUtility.SetDirty(school);
        var city=travel.GetComponent<CityController>();
        foreach(var door in new[]{city.UI.station,city.UI.cityStation,city.UI.marketDoor,city.UI.boutiqueDoor})
        {
            var cue=door.GetComponent<ActionCue>();var halo=Field<Image>(cue,"halo");
            halo.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");halo.preserveAspect=true;
            var p=cue.Picture;Place(halo.rectTransform,p.anchorMin.x-.025f,p.anchorMin.y-.025f,p.anchorMax.x+.025f,p.anchorMax.y+.025f);
        }

        var selection=travel.transform.Find("Character Selection");Hide(selection,"Question","Selection Hint","Gender");
        // The game title and the two character names remain useful orientation for readers.
        var choices=selection.Find("Rabbit Choices");if(choices!=null)Place((RectTransform)choices,.14f,.15f,.86f,.84f);
        var shoes=Field<Button>(travel,"shoesButton");var shoePreview=Field<Image>(travel,"shoesPreview");
        shoePreview.enabled=false;Art("Shoe Picture",shoes.transform,14,.12f,.14f,.88f,.90f);
        var footwear=travel.transform.Find("Footwear Choice");Hide(footwear,"Title","Subtitle","Label");
        var roller=footwear.GetComponentsInChildren<Image>(true).FirstOrDefault(i=>i.name=="Roller Preview");
        if(roller!=null)Place(roller.rectTransform,.14f,.14f,.86f,.90f);
        Hide(canvas,"Touch Hint","TV Hint","Friends Hint");

        var garden=travel.GetComponent<GardenController>();var g=Field<GardenController.Widgets>(garden,"ui");
        InFrontOfActors(g.gate.transform.Find("Action Hint"));InFrontOfActors(g.notice.transform);
        g.guide.transform.parent.gameObject.SetActive(false);
        foreach(var label in g.bedLabels)label.transform.parent.gameObject.SetActive(false);
        foreach(var button in g.tools)
        {Hide(button.transform,"Name");var tool=button.transform.Find("Tool");if(tool!=null)Place((RectTransform)tool,.10f,.06f,.90f,.94f);}
        Hide(g.seedPanel.transform,"Title","Name");
        foreach(var button in g.seedChoices)
        {var crop=button.transform.Find("Crop");if(crop!=null)Place((RectTransform)crop,.08f,.08f,.92f,.92f);}
        g.count.fontSizeMax=34;g.count.fontSize=34;
        Place((RectTransform)g.count.transform.parent,.64f,.875f,.81f,.975f);

        var home=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include);var h=home.UI;
        Hide(h.tvControls.transform,"Label");Symbol(h.stand.transform,CitySymbol.Kind.Back);Symbol(h.sound.transform,CitySymbol.Kind.Sound);
        // Square actions stay large enough to tap without turning them into wide empty bars.
        var controls=(RectTransform)h.tvControls.transform;Place(controls,.72f,.025f,.97f,.15f);
        Place((RectTransform)h.stand.transform,0,0,.44f,1);Place((RectTransform)h.sound.transform,.55f,0,.99f,1);
        h.feedback.enabled=false;h.feedback.raycastTarget=false;
        var feedback=h.feedback.rectTransform;Place(feedback,.70f,.68f,.79f,.81f);Art("Happy Meal",feedback,9);

        foreach(var friend in canvas.GetComponentsInChildren<MeadowFriend>(true))
        {
            var bubble=(RectTransform)Field<GameObject>(friend,"speechBubble").transform;
            foreach(var text in bubble.GetComponentsInChildren<TMPro.TMP_Text>(true))text.enabled=false;
            bubble.anchorMin=bubble.anchorMax=new Vector2(.5f,1.02f);bubble.pivot=new Vector2(.5f,0);bubble.sizeDelta=new Vector2(108,100);bubble.anchoredPosition=Vector2.zero;
            Art("Wave",bubble,3,.14f,.12f,.86f,.88f);
        }
        var neighborhood=travel.GetComponent<Neighborhood>();var welcome=Field<TMPro.TMP_Text>(neighborhood,"hostWords");
        welcome.enabled=false;welcome.raycastTarget=false;Place(welcome.rectTransform,.66f,.65f,.77f,.82f);Art("Wave",welcome.transform,3,0,0,1,1);
        PlayerSettings.bundleVersion="0.8.3";PlayerSettings.Android.bundleVersionCode=14;
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(travel.gameObject.scene);EditorSceneManager.SaveScene(travel.gameObject.scene);AssetDatabase.SaveAssets();
        return "0.8.3 (14): two school subjects; pictorial garden, footwear, TV and greetings; reading exercises and essential numbers preserved.";
    }
}
