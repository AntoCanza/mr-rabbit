using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class PolishLearningJourney
{
    static RectTransform Wrap(UnityEngine.UI.Graphic g)
    {
        if(g.transform.parent.name.EndsWith(" Holder"))return (RectTransform)g.transform.parent;
        var old=g.rectTransform;var holder=new GameObject(g.name+" Holder",typeof(RectTransform)).GetComponent<RectTransform>();holder.SetParent(old.parent,false);holder.SetSiblingIndex(old.GetSiblingIndex());
        holder.anchorMin=old.anchorMin;holder.anchorMax=old.anchorMax;holder.offsetMin=old.offsetMin;holder.offsetMax=old.offsetMax;old.SetParent(holder,false);old.anchorMin=Vector2.zero;old.anchorMax=Vector2.one;old.offsetMin=old.offsetMax=Vector2.zero;old.localScale=Vector3.one;
        var fit=g.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit==null)fit=g.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;return holder;
    }
    static void Place(RectTransform r,float x,float y,float X,float Y){r.anchorMin=new Vector2(x,y);r.anchorMax=new Vector2(X,Y);r.offsetMin=r.offsetMax=Vector2.zero;}
    static void CopyPicture(UnityEngine.UI.Button source,UnityEngine.UI.Button destination,float x,float y,float X,float Y)
    {
        foreach(var graphic in destination.GetComponentsInChildren<UnityEngine.UI.RawImage>(true))graphic.gameObject.SetActive(false);
        var original=source.GetComponentInChildren<UnityEngine.UI.RawImage>(true);var holder=new GameObject("Painted Action Holder",typeof(RectTransform)).GetComponent<RectTransform>();holder.SetParent(destination.transform,false);Place(holder,x,y,X,Y);
        var copy=UnityEngine.Object.Instantiate(original,holder);copy.name="Painted Action";copy.gameObject.SetActive(true);copy.raycastTarget=false;Place(copy.rectTransform,0,0,1,1);
        var fit=copy.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit==null)fit=copy.gameObject.AddComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;
        var cue=destination.GetComponent<ActionCue>();if(cue!=null){var halo=destination.transform.Find("Learning Glow").GetComponent<UnityEngine.UI.Image>();cue.Configure(copy.rectTransform,halo,true);}
    }
    public static string Run()
    {
        if(Application.isPlaying)throw new Exception("Edit mode required");var j=UnityEngine.Object.FindFirstObjectByType<LearningJourney>(FindObjectsInactive.Include);var w=j.UI;var s=j.GetComponent<SchoolController>();
        foreach(var m in j.transform.parent.GetComponentsInChildren<MedalGraphic>(true))Wrap(m);
        foreach(var count in w.counts){var p=Wrap(count);Place(p,0,0,.06f,.08f);count.raycastTarget=false;}
        foreach(var b in w.cards.Concat(w.wallSlots).Concat(w.medalChoices).Concat(new[]{w.practice,w.test,w.openBook,w.resultNext,w.submit,w.next}))
        {
            var cue=b.GetComponent<ActionCue>();var glow=b.transform.Find("Learning Glow");if(cue==null||glow==null)continue;
            Place((RectTransform)glow,0,0,1,1);var medal=b.GetComponentInChildren<MedalGraphic>(true);var icon=b.GetComponentInChildren<CitySymbol>(true);
            var raw=b.GetComponentsInChildren<UnityEngine.UI.RawImage>(true).FirstOrDefault(x=>x.name=="Painted Action");cue.Configure(medal!=null?medal.rectTransform:icon!=null?icon.rectTransform:raw!=null?raw.rectTransform:(RectTransform)b.transform,glow.GetComponent<UnityEngine.UI.Image>(),true);b.transform.localScale=Vector3.one;
        }
        foreach(var t in w.erase.GetComponentsInChildren<TMPro.TMP_Text>(true))t.gameObject.SetActive(false);
        if(w.erase.transform.Find("Painted Action Holder")==null)CopyPicture(s.UI.erase,w.erase,.08f,.08f,.92f,.92f);
        if(w.practice.transform.Find("Painted Action Holder")==null)CopyPicture(s.UI.pencil,w.practice,.06f,.09f,.39f,.91f);
        foreach(var t in w.homeLabels){t.fontSizeMin=6;t.fontSizeMax=10;}
        // A slash on the automatic-announcement toggle distinguishes it from explicit replay.
        var voice=j.GetComponent<ItalianVoice>();voice.muteSymbol.symbol=CitySymbol.Kind.Muted;voice.muteSymbol.SetAllDirty();
        foreach(var b in new[]{voice.repeat,voice.mute})if(b.GetComponent<WorldActionLayer>()==null)b.gameObject.AddComponent<WorldActionLayer>();
        if(voice.repeat.transform.Find("Replay Arrow")==null){var arrow=new GameObject("Replay Arrow",typeof(RectTransform)).GetComponent<RectTransform>();arrow.SetParent(voice.repeat.transform,false);Place(arrow,.35f,0,.70f,.25f);var symbol=arrow.gameObject.AddComponent<CitySymbol>();symbol.symbol=CitySymbol.Kind.Back;symbol.raycastTarget=false;}
        var gu=(GardenController.Widgets)typeof(GardenController).GetField("ui",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(j.GetComponent<GardenController>());
        Place((RectTransform)gu.cancelSeeds.transform,.025f,.845f,.12f,.98f);
        Place((RectTransform)gu.count.transform.parent,.46f,.89f,.625f,.975f);
        Place((RectTransform)j.GetComponent<CityController>().UI.orderBadge.transform,.28f,.89f,.435f,.975f);
        var recipes=UnityEngine.Object.FindFirstObjectByType<HomeActivities>(FindObjectsInactive.Include).UI.recipes;
        var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Garden/SchoolRewardCrops.png");
        for(int i=4;i<6;i++)foreach(var raw in new[]{gu.seedChoices[i].GetComponentInChildren<UnityEngine.UI.RawImage>(true),recipes[i].transform.Find("Ingredient Picture").GetComponentInChildren<UnityEngine.UI.RawImage>(true)})
        {
            var old=raw.GetComponent<GardenArtSlot>();if(old!=null)UnityEngine.Object.DestroyImmediate(old);raw.texture=atlas;raw.uvRect=new Rect((i-4)*.5f,.5f,.5f,.5f);var fit=raw.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit!=null)fit.aspectRatio=1;
        }
        Canvas.ForceUpdateCanvases();EditorSceneManager.MarkSceneDirty(j.gameObject.scene);EditorSceneManager.SaveScene(j.gameObject.scene);AssetDatabase.SaveAssets();return "Contained icon animation and illustrations in square holders; painted pencil/eraser; touch panels retain their bounds.";
    }
}
