using UnityEngine;

namespace MrRabbit
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GuideFocusGraphic : UnityEngine.UI.MaskableGraphic
    {
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;float w=Mathf.Min(r.width,r.height)*.045f;
            Quad(vh,r.xMin,r.yMin,r.xMax,r.yMin+w);Quad(vh,r.xMin,r.yMax-w,r.xMax,r.yMax);
            Quad(vh,r.xMin,r.yMin+w,r.xMin+w,r.yMax-w);Quad(vh,r.xMax-w,r.yMin+w,r.xMax,r.yMax-w);
        }
        void Quad(UnityEngine.UI.VertexHelper v,float x,float y,float r,float t)
        {int s=v.currentVertCount;v.AddVert(new Vector3(x,y),color,Vector2.zero);v.AddVert(new Vector3(r,y),color,Vector2.zero);v.AddVert(new Vector3(r,t),color,Vector2.zero);v.AddVert(new Vector3(x,t),color,Vector2.zero);v.AddTriangle(s,s+1,s+2);v.AddTriangle(s,s+2,s+3);}
    }
}
