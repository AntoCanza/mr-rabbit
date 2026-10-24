using UnityEngine;
namespace MrRabbit
{
    // Resolution-independent medal and notebook drawings, with the same warm palette as the HUD.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MedalGraphic : UnityEngine.UI.MaskableGraphic
    {
        public bool book,earned=true;
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();Color edge=new Color32(133,98,61,255),gold=earned?new Color32(247,195,73,255):new Color32(193,192,177,255);
            if(book){Box(vh,.10f,.15f,.9f,.86f,edge);Box(vh,.16f,.21f,.48f,.80f,new Color32(255,248,224,255));Box(vh,.52f,.21f,.84f,.80f,new Color32(255,248,224,255));for(int i=0;i<3;i++){Box(vh,.21f,.37f+i*.13f,.42f,.40f+i*.13f,gold);Box(vh,.58f,.37f+i*.13f,.79f,.40f+i*.13f,gold);}return;}
            Box(vh,.28f,.05f,.46f,.50f,new Color32(88,162,163,255));Box(vh,.54f,.05f,.72f,.50f,new Color32(226,126,107,255));
            Circle(vh,.5f,.59f,.37f,edge);Circle(vh,.5f,.59f,.325f,gold);Circle(vh,.5f,.59f,.25f,earned?new Color32(255,226,132,255):new Color32(221,219,201,255));
        }
        void Box(UnityEngine.UI.VertexHelper v,float x,float y,float X,float Y,Color c){int n=v.currentVertCount;Point(v,x,y,c);Point(v,x,Y,c);Point(v,X,Y,c);Point(v,X,y,c);v.AddTriangle(n,n+1,n+2);v.AddTriangle(n,n+2,n+3);}
        void Circle(UnityEngine.UI.VertexHelper v,float x,float y,float r,Color c){int n=v.currentVertCount;Point(v,x,y,c);for(int i=0;i<=40;i++){float a=i*Mathf.PI/20;Point(v,x+Mathf.Cos(a)*r,y+Mathf.Sin(a)*r,c);if(i>0)v.AddTriangle(n,n+i,n+i+1);}}
        void Point(UnityEngine.UI.VertexHelper v,float x,float y,Color c){var r=rectTransform.rect;v.AddVert(new Vector3(r.x+x*r.width,r.y+y*r.height),c*color,Vector2.zero);}
    }
}
