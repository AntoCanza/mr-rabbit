using UnityEngine;
using UnityEngine.UI;

namespace MrRabbit
{
    // Small resolution-independent pictograms, shared by the new journey and kitchen controls.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class CitySymbol : MaskableGraphic
    {
        public enum Kind { Train, Home, City, Market, Shirt, Coin, Check, Back, Arrow, Lock, Pin, Pot, Plate, Garden, Sound, Muted }
        public Kind symbol;
        VertexHelper mesh;
        static readonly Color ink = new Color32(72, 66, 69, 255);
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); mesh = vh;
            Color teal = new Color32(71, 158, 164, 255), gold = new Color32(246, 191, 72, 255);
            Color coral = new Color32(234, 123, 103, 255), cream = new Color32(255, 246, 219, 255);
            switch (symbol)
            {
                case Kind.Sound:
                case Kind.Muted:
                    Box(.12f,.35f,.34f,.65f,ink);
                    Poly(teal,new Vector2(.27f,.35f),new Vector2(.55f,.13f),new Vector2(.55f,.87f),new Vector2(.27f,.65f));
                    if(symbol==Kind.Muted){Line(.67f,.34f,.91f,.66f,.07f,coral);Line(.67f,.66f,.91f,.34f,.07f,coral);}
                    else {Line(.68f,.34f,.78f,.5f,.055f,ink);Line(.78f,.5f,.68f,.66f,.055f,ink);Line(.82f,.22f,.96f,.5f,.05f,ink);Line(.96f,.5f,.82f,.78f,.05f,ink);}break;
                case Kind.Coin:
                    Circle(.5f,.5f,.43f,ink);Circle(.5f,.5f,.37f,gold);Circle(.5f,.5f,.27f,new Color32(255,220,112,255));Box(.46f,.29f,.54f,.72f,gold);break;
                case Kind.Train:
                    Box(.12f,.22f,.85f,.71f,ink);Box(.16f,.29f,.81f,.67f,teal);Box(.1f,.67f,.87f,.76f,ink);
                    Box(.23f,.43f,.43f,.61f,cream);Box(.53f,.43f,.73f,.61f,cream);
                    Circle(.29f,.23f,.11f,ink);Circle(.68f,.23f,.11f,ink);Box(.1f,.09f,.89f,.14f,ink);break;
                case Kind.Home:
                    Box(.23f,.13f,.79f,.63f,ink);Box(.28f,.17f,.74f,.62f,cream);
                    Poly(coral,new Vector2(.12f,.59f),new Vector2(.5f,.94f),new Vector2(.89f,.59f));
                    Box(.44f,.17f,.60f,.47f,teal);Box(.31f,.44f,.40f,.54f,gold);break;
                case Kind.City:
                    for(int i=0;i<3;i++){float x=.1f+i*.28f,top=i==1?.91f:.69f;Box(x,.12f,x+.23f,top,ink);Box(x+.03f,.15f,x+.20f,top-.03f,i==1?teal:coral);for(int j=0;j<3;j++)Box(x+.07f,.23f+j*.16f,x+.15f,.31f+j*.16f,gold);}break;
                case Kind.Market:
                    Box(.18f,.16f,.82f,.52f,teal);Box(.1f,.53f,.9f,.72f,coral);
                    Poly(cream,new Vector2(.13f,.72f),new Vector2(.26f,.90f),new Vector2(.74f,.90f),new Vector2(.87f,.72f));
                    Circle(.35f,.52f,.12f,gold);Circle(.61f,.52f,.12f,coral);Box(.14f,.14f,.86f,.23f,ink);break;
                case Kind.Shirt:
                    Poly(ink,new Vector2(.28f,.85f),new Vector2(.4f,.9f),new Vector2(.5f,.78f),new Vector2(.6f,.9f),new Vector2(.72f,.85f),new Vector2(.96f,.59f),new Vector2(.79f,.43f),new Vector2(.7f,.52f),new Vector2(.7f,.12f),new Vector2(.3f,.12f),new Vector2(.3f,.52f),new Vector2(.21f,.43f),new Vector2(.04f,.59f));
                    Poly(new Color32(170,135,210,255),new Vector2(.29f,.80f),new Vector2(.4f,.83f),new Vector2(.5f,.71f),new Vector2(.6f,.83f),new Vector2(.71f,.80f),new Vector2(.87f,.59f),new Vector2(.79f,.52f),new Vector2(.65f,.63f),new Vector2(.65f,.17f),new Vector2(.35f,.17f),new Vector2(.35f,.63f),new Vector2(.21f,.52f),new Vector2(.13f,.59f));break;
                case Kind.Check: Line(.16f,.48f,.40f,.25f,.12f,teal);Line(.40f,.25f,.85f,.78f,.12f,teal);break;
                case Kind.Back: Line(.23f,.5f,.82f,.5f,.11f,ink);Line(.23f,.5f,.49f,.78f,.11f,ink);Line(.23f,.5f,.49f,.22f,.11f,ink);break;
                case Kind.Arrow: Line(.16f,.5f,.78f,.5f,.10f,teal);Line(.78f,.5f,.54f,.76f,.10f,teal);Line(.78f,.5f,.54f,.24f,.10f,teal);break;
                case Kind.Lock: Circle(.5f,.65f,.25f,ink);Circle(.5f,.65f,.15f,cream);Box(.19f,.14f,.81f,.61f,gold);Circle(.5f,.40f,.07f,ink);break;
                case Kind.Pin: Circle(.5f,.65f,.27f,coral);Poly(coral,new Vector2(.25f,.55f),new Vector2(.5f,.10f),new Vector2(.75f,.55f));Circle(.5f,.65f,.11f,cream);break;
                case Kind.Pot:
                    Box(.19f,.19f,.81f,.57f,teal);Box(.1f,.50f,.90f,.58f,ink);Box(.17f,.60f,.83f,.67f,ink);Circle(.5f,.70f,.06f,ink);Line(.33f,.77f,.38f,.91f,.035f,coral);Line(.59f,.77f,.64f,.94f,.035f,coral);break;
                case Kind.Plate: Circle(.5f,.45f,.40f,ink);Circle(.5f,.45f,.35f,cream);Circle(.5f,.45f,.23f,gold);Circle(.44f,.50f,.09f,coral);break;
                case Kind.Garden: Box(.15f,.16f,.85f,.33f,new Color32(163,112,74,255));Line(.5f,.28f,.5f,.75f,.06f,teal);Poly(teal,new Vector2(.5f,.52f),new Vector2(.19f,.84f),new Vector2(.45f,.83f));Poly(teal,new Vector2(.5f,.65f),new Vector2(.80f,.93f),new Vector2(.59f,.92f));break;
            }
        }
        void Box(float x,float y,float r,float t,Color c)=>Poly(c,new Vector2(x,y),new Vector2(r,y),new Vector2(r,t),new Vector2(x,t));
        void Circle(float x,float y,float radius,Color c)
        {for(int i=0;i<24;i++){float a=i*Mathf.PI/12,b=(i+1)*Mathf.PI/12;Poly(c,new Vector2(x,y),new Vector2(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius),new Vector2(x+Mathf.Cos(b)*radius,y+Mathf.Sin(b)*radius));}}
        void Line(float x,float y,float r,float t,float width,Color c)
        {var a=new Vector2(x,y);var b=new Vector2(r,t);var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*width*.5f;Poly(c,a+n,b+n,b-n,a-n);}
        void Poly(Color c,params Vector2[] points)
        {
            int start=mesh.currentVertCount;var rect=GetPixelAdjustedRect();float side=Mathf.Min(rect.width,rect.height);rect=new Rect(rect.center-Vector2.one*side*.5f,Vector2.one*side);
            foreach(var p in points)mesh.AddVert(new Vector3(rect.x+p.x*rect.width,rect.y+p.y*rect.height),c*color,Vector2.zero);
            for(int i=1;i<points.Length-1;i++)mesh.AddTriangle(start,start+i,start+i+1);
        }
    }
}
