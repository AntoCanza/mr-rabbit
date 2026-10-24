using UnityEngine;
using MrRabbit;

public static class PreviewWordPictures
{
    public static string Run()
    {
        var s=Object.FindFirstObjectByType<SchoolController>();
        var canvas=s.GetComponentInParent<Canvas>();
        var root=new GameObject("Vocabulary Preview",typeof(RectTransform),typeof(CanvasRenderer),typeof(UnityEngine.UI.Image));
        var r=(RectTransform)root.transform;r.SetParent(canvas.transform,false);r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
        root.GetComponent<UnityEngine.UI.Image>().color=new Color32(255,247,223,255);
        for(int i=0;i<s.UI.wordPicture.Entries.Count;i++)
        {
            var entry=s.UI.wordPicture.Entries[i];int col=i%6,row=i/6;
            var slot=new GameObject(entry.word,typeof(RectTransform));var cell=(RectTransform)slot.transform;cell.SetParent(r,false);
            cell.anchorMin=new Vector2(col/6f+.015f,1-(row+1)/4f+.04f);cell.anchorMax=new Vector2((col+1)/6f-.015f,1-row/4f-.015f);cell.offsetMin=cell.offsetMax=Vector2.zero;
            var child=new GameObject("Art",typeof(RectTransform),typeof(CanvasRenderer),typeof(UnityEngine.UI.RawImage),typeof(UnityEngine.UI.AspectRatioFitter));child.transform.SetParent(cell,false);
            var image=child.GetComponent<UnityEngine.UI.RawImage>();image.texture=entry.atlas;image.uvRect=entry.uv;image.raycastTarget=false;
            var fit=child.GetComponent<UnityEngine.UI.AspectRatioFitter>();fit.aspectMode=UnityEngine.UI.AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=entry.uv.width*entry.atlas.width/(entry.uv.height*entry.atlas.height);
            var label=new GameObject("Label",typeof(RectTransform),typeof(TMPro.TextMeshProUGUI));var text=label.GetComponent<TMPro.TextMeshProUGUI>();text.transform.SetParent(cell,false);text.rectTransform.anchorMin=new Vector2(0,-.16f);text.rectTransform.anchorMax=new Vector2(1,0);text.rectTransform.offsetMin=text.rectTransform.offsetMax=Vector2.zero;text.font=s.UI.question.font;text.fontSize=24;text.alignment=TMPro.TextAlignmentOptions.Center;text.text=entry.word;text.color=Color.black;
        }
        return "Temporary Play-mode contact sheet; disappears on exiting Play";
    }
}
