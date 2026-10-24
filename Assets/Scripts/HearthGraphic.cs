using UnityEngine;

namespace MrRabbit
{
    // Lightweight steam and a small serving dish, drawn directly in the UI.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HearthGraphic : UnityEngine.UI.MaskableGraphic
    {
        public bool steam;
        [SerializeField] private int dish;
        public void SetDish(int value){dish=value;SetVerticesDirty();}
        private void Update(){if(steam)SetVerticesDirty();}
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();
            if(steam)
            {
                for(int i=0;i<3;i++)for(int j=0;j<7;j++)
                {
                    float y=Mathf.Repeat(Time.time*.4f+j*.13f+i*.3f,1);
                    Ellipse(vh,.2f+i*.3f+Mathf.Sin(y*7+Time.time*2)*.05f,y,.035f,.065f,new Color(1,.98f,.89f,(1-y)*.7f));
                }
            }
            else
            {
                Ellipse(vh,.5f,.45f,.49f,.35f,new Color32(94,65,45,255));
                Ellipse(vh,.5f,.5f,.46f,.32f,new Color32(255,246,221,255));
                Color[] colours={new Color32(238,147,50,255),new Color32(203,141,69,255),new Color32(207,74,45,255),new Color32(193,53,76,255),new Color32(244,168,60,255),new Color32(92,76,159,255)};
                if(dish==1)
                {
                    Ellipse(vh,.5f,.55f,.35f,.24f,new Color32(163,99,43,255));
                    Ellipse(vh,.5f,.59f,.33f,.21f,new Color32(229,170,78,255));
                    for(int i=0;i<3;i++)Ellipse(vh,.32f+i*.18f,.62f,.025f,.10f,new Color32(255,220,146,255));
                }
                else if(dish==3||dish==5)
                {
                    Box(vh,.26f,.29f,.74f,.78f,new Color32(244,226,196,255));
                    Box(vh,.30f,.33f,.70f,.70f,colours[dish]);
                    Box(vh,.23f,.76f,.77f,.86f,new Color32(132,157,107,255));
                    Ellipse(vh,.5f,.53f,.12f,.12f,new Color32(255,236,197,255));
                    Ellipse(vh,.5f,.53f,.07f,.08f,colours[dish]);
                }
                else
                {
                    Ellipse(vh,.5f,.53f,.34f,.21f,colours[Mathf.Clamp(dish,0,5)]);
                    for(int i=0;i<3;i++)Ellipse(vh,.38f+i*.12f,.55f,.025f,.035f,new Color32(90,135,70,255));
                }
            }
        }
        private void Box(UnityEngine.UI.VertexHelper vh,float x,float y,float right,float top,Color c)
        {
            var r=rectTransform.rect;int n=vh.currentVertCount;
            vh.AddVert(new Vector3(r.x+x*r.width,r.y+y*r.height),c,Vector2.zero);vh.AddVert(new Vector3(r.x+right*r.width,r.y+y*r.height),c,Vector2.zero);
            vh.AddVert(new Vector3(r.x+right*r.width,r.y+top*r.height),c,Vector2.zero);vh.AddVert(new Vector3(r.x+x*r.width,r.y+top*r.height),c,Vector2.zero);
            vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);
        }
        private void Ellipse(UnityEngine.UI.VertexHelper vh,float x,float y,float rx,float ry,Color tint)
        {
            var r=rectTransform.rect;int n=vh.currentVertCount;
            vh.AddVert(new Vector3(r.x+x*r.width,r.y+y*r.height),tint,Vector2.zero);
            for(int i=0;i<=24;i++)
            {
                float a=i*Mathf.PI/12;
                vh.AddVert(new Vector3(r.x+(x+Mathf.Cos(a)*rx)*r.width,r.y+(y+Mathf.Sin(a)*ry)*r.height),tint,Vector2.zero);
                if(i>0)vh.AddTriangle(n,n+i,n+i+1);
            }
        }
    }
}
