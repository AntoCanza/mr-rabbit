using UnityEngine;

namespace MrRabbit
{
    // A finger demonstration is decorative: every touch passes through to the game.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GuideHandGraphic : UnityEngine.UI.MaskableGraphic
    {
        UnityEngine.UI.VertexHelper mesh;
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();mesh=vh;
            Color outline=new Color32(94,75,57,255),skin=new Color32(255,222,177,255);
            Capsule(.37f,.45f,.37f,.89f,.095f,outline);Capsule(.37f,.45f,.37f,.89f,.065f,skin);
            Capsule(.54f,.39f,.54f,.62f,.09f,outline);Capsule(.54f,.39f,.54f,.62f,.060f,skin);
            Capsule(.69f,.36f,.69f,.55f,.085f,outline);Capsule(.69f,.36f,.69f,.55f,.055f,skin);
            Capsule(.82f,.32f,.82f,.48f,.08f,outline);Capsule(.82f,.32f,.82f,.48f,.05f,skin);
            Capsule(.27f,.28f,.13f,.49f,.10f,outline);Capsule(.27f,.28f,.13f,.49f,.07f,skin);
            Box(.28f,.15f,.88f,.48f,outline);Box(.31f,.17f,.85f,.49f,skin);
            Box(.27f,.03f,.86f,.20f,outline);Box(.30f,.05f,.83f,.17f,new Color32(77,167,169,255));
        }
        void Capsule(float x,float y,float r,float t,float radius,Color c)
        {var a=new Vector2(x,y);var b=new Vector2(r,t);var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*radius;Poly(c,a+n,b+n,b-n,a-n);Circle(a,radius,c);Circle(b,radius,c);}
        void Circle(Vector2 p,float radius,Color c)
        {for(int i=0;i<20;i++){float a=i*Mathf.PI/10,b=(i+1)*Mathf.PI/10;Poly(c,p,p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,p+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*radius);}}
        void Box(float x,float y,float r,float t,Color c)=>Poly(c,new Vector2(x,y),new Vector2(r,y),new Vector2(r,t),new Vector2(x,t));
        void Poly(Color c,params Vector2[] points)
        {int start=mesh.currentVertCount;var r=rectTransform.rect;foreach(var p in points)mesh.AddVert(new Vector3(r.x+p.x*r.width,r.y+p.y*r.height),c*color,Vector2.zero);for(int i=1;i<points.Length-1;i++)mesh.AddTriangle(start,start+i,start+i+1);}
    }
}
