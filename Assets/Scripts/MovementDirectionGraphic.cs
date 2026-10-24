using UnityEngine;

namespace MrRabbit
{
    // Small, translucent pictogram below the feet. It never intercepts touches.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MovementDirectionGraphic : UnityEngine.UI.MaskableGraphic
    {
        protected override void Awake(){base.Awake();raycastTarget=false;}
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;
            Shape(vh,r,new Color32(255,248,218,155),1);
            Shape(vh,r,new Color32(91,104,71,130),.73f);
        }
        static void Shape(UnityEngine.UI.VertexHelper vh,Rect r,Color32 c,float scale)
        {
            var points=new[]{new Vector2(-.48f,-.12f),new Vector2(.12f,-.12f),new Vector2(.12f,-.38f),new Vector2(.48f,0),new Vector2(.12f,.38f),new Vector2(.12f,.12f),new Vector2(-.48f,.12f)};
            int start=vh.currentVertCount;
            foreach(var p in points)vh.AddVert(r.center+new Vector2(p.x*r.width,p.y*r.height)*scale,c,Vector2.zero);
            vh.AddTriangle(start,start+1,start+5);vh.AddTriangle(start,start+5,start+6);
            vh.AddTriangle(start+2,start+3,start+4);
        }
    }
}
