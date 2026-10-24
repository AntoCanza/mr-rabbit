using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HandwritingPad : UnityEngine.UI.MaskableGraphic,IPointerDownHandler,IDragHandler,IPointerUpHandler,IEndDragHandler
    {
        private readonly List<List<Vector2>> strokes=new List<List<Vector2>>();
        private int pointer=int.MinValue,pointCount;
        private bool recognitionDirty=true;
        private char recognition;
        private bool recognizedVowels;
        private bool recognizedAlphabet;
        public bool CanWrite {get;set;}
        public bool Vowels {get;set;}
        public bool Alphabet {get;set;}
        public int PointCount=>pointCount;
        public char Recognized {get {if(recognitionDirty||recognizedVowels!=Vowels||recognizedAlphabet!=Alphabet){recognition=Alphabet?HandwritingRecognizer.RecognizeAlphabet(strokes,out _):HandwritingRecognizer.Recognize(strokes,Vowels,out _);recognizedVowels=Vowels;recognizedAlphabet=Alphabet;recognitionDirty=false;}return recognition;}}
        public event System.Action Changed;
        public IReadOnlyList<List<Vector2>> Strokes=>strokes;
        public void Clear(){strokes.Clear();pointCount=0;pointer=int.MinValue;recognitionDirty=true;SetVerticesDirty();Changed?.Invoke();}
        public void OnPointerDown(PointerEventData e)
        {
            if(!CanWrite||pointer!=int.MinValue||e.button!=PointerEventData.InputButton.Left||strokes.Count>=12)return;
            pointer=e.pointerId;strokes.Add(new List<Vector2>());Add(e);
        }
        public void OnDrag(PointerEventData e){if(pointer==e.pointerId&&CanWrite)Add(e);}
        public void OnPointerUp(PointerEventData e){if(pointer!=e.pointerId)return;Add(e);pointer=int.MinValue;Changed?.Invoke();}
        public void OnEndDrag(PointerEventData e)=>OnPointerUp(e);
        private void Add(PointerEventData e)
        {
            if(pointCount>=1800||strokes.Count==0)return;
            var r=rectTransform.rect;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,e.position,e.pressEventCamera,out var p))return;
            p=new Vector2(Mathf.Clamp01((p.x-r.xMin)/r.width),Mathf.Clamp01((p.y-r.yMin)/r.height));
            var s=strokes[strokes.Count-1];if(s.Count>0&&Vector2.Distance(s[s.Count-1],p)<.004f)return;
            s.Add(p);pointCount++;recognitionDirty=true;SetVerticesDirty();
        }
        protected override void OnDisable(){pointer=int.MinValue;base.OnDisable();}
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;float half=Mathf.Max(2,r.width*.009f);
            foreach(var stroke in strokes)
            {
                for(int i=0;i<stroke.Count;i++)
                {
                    Vector2 p=new Vector2(r.x+stroke[i].x*r.width,r.y+stroke[i].y*r.height);
                    if(i==0){Quad(vh,p-new Vector2(half,half),p+new Vector2(-half,half),p+new Vector2(half,half),p+new Vector2(half,-half));continue;}
                    Vector2 a=new Vector2(r.x+stroke[i-1].x*r.width,r.y+stroke[i-1].y*r.height);
                    var d=(p-a).normalized;var n=new Vector2(-d.y,d.x)*half;Quad(vh,a-n,a+n,p+n,p-n);
                }
            }
            // Invisible quad keeps the blank paper a real raycast target.
            if(strokes.Count==0){int n=vh.currentVertCount;for(int i=0;i<4;i++)vh.AddVert(Vector3.zero,Color.clear,Vector2.zero);vh.AddTriangle(n,n+1,n+2);}
        }
        private void Quad(UnityEngine.UI.VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Vector2 d)
        {int n=vh.currentVertCount;vh.AddVert(a,color,Vector2.zero);vh.AddVert(b,color,Vector2.zero);vh.AddVert(c,color,Vector2.zero);vh.AddVert(d,color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);}
    }
}
