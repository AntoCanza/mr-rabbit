using UnityEngine;
namespace MrRabbit
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GardenGuideArrow : UnityEngine.UI.MaskableGraphic
    {
        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vh)
        {
            vh.Clear();var r=rectTransform.rect;
            Vector2[] points={new Vector2(.30f,1),new Vector2(.70f,1),new Vector2(.70f,.48f),
                new Vector2(1,.48f),new Vector2(.5f,0),new Vector2(0,.48f),new Vector2(.30f,.48f)};
            for(int i=0;i<points.Length;i++)vh.AddVert(new Vector3(r.x+points[i].x*r.width,r.y+points[i].y*r.height),color,Vector2.zero);
            vh.AddTriangle(0,1,2);vh.AddTriangle(0,2,6);vh.AddTriangle(5,3,4);
        }
    }
}
