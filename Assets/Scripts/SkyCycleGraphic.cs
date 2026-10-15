using UnityEngine;

namespace MrRabbit
{
    // Small vector sky decorations: a single UI mesh with no bitmap or particle effects.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SkyCycleGraphic : UnityEngine.UI.MaskableGraphic
    {
        [SerializeField] private bool aroundHouse;
        private float phase = .12f, night;
        private static readonly Vector2[] LawnStars = {
            new Vector2(.07f,.90f),new Vector2(.22f,.80f),new Vector2(.35f,.94f),
            new Vector2(.48f,.77f),new Vector2(.62f,.91f),new Vector2(.76f,.81f),
            new Vector2(.91f,.95f),new Vector2(.13f,.68f),new Vector2(.40f,.66f),
            new Vector2(.69f,.69f),new Vector2(.87f,.73f),new Vector2(.55f,.96f)
        };
        private static readonly Vector2[] HouseStars = {
            new Vector2(.04f,.77f),new Vector2(.05f,.96f),new Vector2(.23f,.95f),
            new Vector2(.28f,.88f),new Vector2(.75f,.96f),new Vector2(.90f,.88f),
            new Vector2(.96f,.97f),new Vector2(.95f,.77f)
        };
        public void Configure(bool home) { aroundHouse = home; raycastTarget = false; SetVerticesDirty(); }
        public void SetTime(float value,float darkness)
        {
            if (Mathf.Abs(phase-value)<.0001f && Mathf.Abs(night-darkness)<.0001f) return;
            phase=value; night=darkness; SetVerticesDirty();
        }
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();
            Rect r=GetPixelAdjustedRect();
            if (r.width<=0 || r.height<=0) return;
            var stars=aroundHouse?HouseStars:LawnStars;
            for(int i=0;i<stars.Length;i++)
            {
                Vector2 p=Point(r,stars[i]);
                float size=r.height*(i%3==0?.006f:.0038f);
                Color c=new Color(1,.96f,.76f,night*.88f);
                Diamond(vh,p,size,c);
            }
            float sunProgress=phase<.5f?phase/.5f:0;
            float moonProgress=phase>=.5f?(phase-.5f)/.5f:0;
            Vector2 sun=Point(r,aroundHouse?new Vector2(.11f,.86f):Arc(sunProgress));
            Vector2 moon=Point(r,aroundHouse?new Vector2(.11f,.86f):Arc(moonProgress));
            float radius=r.height*(aroundHouse?.035f:.043f);
            if(night<.999f) Sun(vh,sun,radius,1-night);
            if(night>.001f) Moon(vh,moon,radius*1.25f,night);
        }
        private static Vector2 Arc(float t)=>new Vector2(Mathf.Lerp(.13f,.87f,t),.76f+.13f*Mathf.Sin(t*Mathf.PI));
        private static Vector2 Point(Rect r,Vector2 p)=>new Vector2(r.xMin+p.x*r.width,r.yMin+p.y*r.height);
        private static void Vertex(UnityEngine.UI.VertexHelper vh,Vector2 p,Color c)=>vh.AddVert(p,c,Vector2.zero);
        private static void Diamond(UnityEngine.UI.VertexHelper vh,Vector2 p,float s,Color c)
        {
            int k=vh.currentVertCount;
            Vertex(vh,p+new Vector2(0,s),c);Vertex(vh,p+new Vector2(s*.6f,0),c);
            Vertex(vh,p-new Vector2(0,s),c);Vertex(vh,p-new Vector2(s*.6f,0),c);
            vh.AddTriangle(k,k+1,k+2);vh.AddTriangle(k,k+2,k+3);
        }
        private static void Disc(UnityEngine.UI.VertexHelper vh,Vector2 p,float radius,Color c)
        {
            const int sides=32;
            int k=vh.currentVertCount;Vertex(vh,p,c);
            for(int i=0;i<=sides;i++)
            {
                float a=i*Mathf.PI*2/sides;
                Vertex(vh,p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,c);
                if(i>0) vh.AddTriangle(k,k+i,k+i+1);
            }
        }
        private static void Sun(UnityEngine.UI.VertexHelper vh,Vector2 p,float r,float alpha)
        {
            Disc(vh,p,r*1.55f,new Color(1,.82f,.36f,.10f*alpha));
            for(int i=0;i<8;i++)
            {
                float angle=i*Mathf.PI/4;
                Vector2 direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                Vector2 side=new Vector2(-direction.y,direction.x)*r*.12f;
                int k=vh.currentVertCount;Color c=new Color(1,.73f,.20f,alpha);
                Vertex(vh,p+direction*r*1.65f,c);Vertex(vh,p+direction*r*1.22f+side,c);
                Vertex(vh,p+direction*r*1.22f-side,c);vh.AddTriangle(k,k+1,k+2);
            }
            Disc(vh,p,r,new Color(1,.79f,.23f,alpha));
            Disc(vh,p+new Vector2(-r*.12f,r*.12f),r*.78f,new Color(1,.89f,.44f,alpha));
        }
        private static void Moon(UnityEngine.UI.VertexHelper vh,Vector2 p,float r,float alpha)
        {
            Disc(vh,p,r*1.5f,new Color(.81f,.90f,1,.07f*alpha));
            const int sides=32;int start=vh.currentVertCount;
            for(int i=0;i<=sides;i++)
            {
                float a=i*Mathf.PI/sides;
                float s=Mathf.Sin(a),y=Mathf.Cos(a);
                Color c=new Color(1,.97f,.77f,alpha);
                Vertex(vh,p+new Vector2(.35f-s,y)*r,c);
                Vertex(vh,p+new Vector2(.35f-.25f*s,y)*r,c);
                if(i>0)
                {
                    int k=start+(i-1)*2;
                    vh.AddTriangle(k,k+1,k+2);vh.AddTriangle(k+1,k+3,k+2);
                }
            }
        }
    }
}
