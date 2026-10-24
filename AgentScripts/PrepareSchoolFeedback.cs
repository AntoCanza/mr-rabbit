using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using MrRabbit;
public static class PrepareSchoolFeedback
{
    public static string Run()
    {
        if(EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
        var s=Object.FindFirstObjectByType<SchoolController>();var u=s.UI;
        var f=s.GetComponent<SchoolFeedback>();if(f==null)f=s.gameObject.AddComponent<SchoolFeedback>();
        f.rewardPicture=u.rewardCue.Picture.GetComponentInChildren<RawImage>(true);
        f.starUv=f.rewardPicture.uvRect;f.checkUv=Uv(420,445,300,280);
        f.helpCue=u.help.GetComponent<ActionCue>();f.helpCue.Configure(f.helpCue.Picture,u.help.transform.Find("Glow").GetComponent<Image>(),true);
        if(f.helpCost==null)
        {
            var cost=Rect("Help Costs One Star",u.help.transform,.52f,.65f,1.1f,1.02f);f.helpCost=cost.gameObject;
            Icon(cost,.40f,0,1,1,f.rewardPicture.texture,f.starUv);Text(cost,0,0,.46f,1,"−1",u.question,24);
            var panel=Rect("Teacher Reminder",u.worksheet.transform,.25f,.28f,.75f,.77f);
            var bg=panel.gameObject.AddComponent<Image>();bg.color=new Color32(255,246,215,250);bg.raycastTarget=true;
            var outline=panel.gameObject.AddComponent<Outline>();outline.effectColor=new Color32(203,174,123,255);outline.effectDistance=new Vector2(3,3);
            var teacher=Rect("Teacher",panel,.04f,.05f,.55f,.95f);f.portrait=teacher;
            var image=teacher.gameObject.AddComponent<Image>();image.sprite=u.teacher.sprite;image.preserveAspect=true;image.raycastTarget=false;
            Icon(panel,.58f,.38f,.90f,.84f,f.rewardPicture.texture,f.starUv);
            f.delta=Text(panel,.57f,.08f,.92f,.37f,"−1",u.question,52);
            f.reminder=panel.gameObject;f.reminder.SetActive(false);
        }
        FixIcon(f.helpCost.transform,.48f,0,1,1);
        FixIcon(f.reminder.transform,.58f,.38f,.90f,.84f);
        EditorUtility.SetDirty(f);EditorSceneManager.MarkSceneDirty(s.gameObject.scene);EditorSceneManager.SaveScene(s.gameObject.scene);
        PlayerSettings.bundleVersion="0.7.3";PlayerSettings.Android.bundleVersionCode=10;AssetDatabase.SaveAssets();
        return "Teacher reminder, visible one-star cost, assisted checkmark, offline audio; 0.7.3 (10) saved";
    }
    static Rect Uv(float x,float y,float w,float h)=>new Rect(x/1448,1-(y+h)/1086,w/1448,h/1086);
    static void FixIcon(Transform parent,float x,float y,float r,float t)
    {
        var icon=parent.Find("Star");if(icon==null)return;
        var slot=Rect("Star Slot",parent,x,y,r,t);icon.SetParent(slot,false);
        var rt=(RectTransform)icon;rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;
    }
    static RectTransform Rect(string n,Transform p,float x,float y,float r,float t)
    {var a=new GameObject(n,typeof(RectTransform)).GetComponent<RectTransform>();a.SetParent(p,false);a.anchorMin=new Vector2(x,y);a.anchorMax=new Vector2(r,t);a.offsetMin=a.offsetMax=Vector2.zero;return a;}
    static void Icon(Transform p,float x,float y,float r,float t,Texture texture,Rect uv)
    {var a=Rect("Star",p,x,y,r,t).gameObject.AddComponent<RawImage>();a.texture=texture;a.uvRect=uv;a.raycastTarget=false;var fit=a.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=295f/285;}
    static TMPro.TMP_Text Text(Transform p,float x,float y,float r,float t,string text,TMPro.TMP_Text sample,int size)
    {var a=Rect("Count",p,x,y,r,t).gameObject.AddComponent<TMPro.TextMeshProUGUI>();a.font=sample.font;a.text=text;a.color=new Color32(119,76,46,255);a.fontSize=size;a.alignment=TMPro.TextAlignmentOptions.Center;a.raycastTarget=false;return a;}
}
