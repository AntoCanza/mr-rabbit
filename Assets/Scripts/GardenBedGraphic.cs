using UnityEngine;

namespace MrRabbit
{
    // Small reusable meshes for interactive soil, no per-frame textures.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GardenBedGraphic : UnityEngine.UI.MaskableGraphic
    {
        public bool prepared, wet, selected, seeds;
        [SerializeField] private UnityEngine.UI.Image paintedSoil;
        public void SetPainting(UnityEngine.UI.Image value){paintedSoil=value;SetVerticesDirty();}
        public void Show(bool tilled,bool watered,bool active,bool sown)
        {
            if(prepared==tilled&&wet==watered&&selected==active&&seeds==sown)return;
            prepared=tilled;wet=watered;selected=active;seeds=sown;SetVerticesDirty();
            if(paintedSoil!=null)paintedSoil.color=wet?new Color(.67f,.64f,.59f):Color.white;
        }
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;
            if(paintedSoil==null)
            {
                Quad(vh,r,new Color32(92,60,37,255));
                var inside=new Rect(r.x+5,r.y+5,r.width-10,r.height-10);
                Quad(vh,inside,wet?new Color32(103,69,44,255):new Color32(164,113,69,255));
            }
            for(int row=0;row<4;row++)
            {
                float y=r.y+r.height*(.19f+row*.19f);
                if(prepared)for(int segment=0;segment<14;segment++)
                    Quad(vh,new Rect(r.x+r.width*(.08f+segment*.06f),y+Mathf.Sin(segment*2+row)*2,r.width*.065f,3),new Color32(88,56,33,180));
                if(seeds)for(int j=0;j<5;j++)Quad(vh,new Rect(r.x+r.width*(.14f+j*.17f),y+6,5,4),new Color32(242,208,118,255));
            }
            if(selected)
            {
                Color c=new Color32(255,213,82,255);
                Quad(vh,new Rect(r.x,r.y,r.width,4),c);Quad(vh,new Rect(r.x,r.yMax-4,r.width,4),c);
                Quad(vh,new Rect(r.x,r.y,4,r.height),c);Quad(vh,new Rect(r.xMax-4,r.y,4,r.height),c);
            }
        }
        private static void Quad(UnityEngine.UI.VertexHelper vh,Rect r,Color c)
        {
            int n=vh.currentVertCount;
            vh.AddVert(new Vector3(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector3(r.xMin,r.yMax),c,Vector2.zero);
            vh.AddVert(new Vector3(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector3(r.xMax,r.yMin),c,Vector2.zero);
            vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);
        }
    }
}
