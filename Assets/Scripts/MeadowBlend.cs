using UnityEngine;
using UnityEngine.UI;
namespace MrRabbit
{
    // Feather the overlapping artwork at the join, without an extra shader or texture.
    [RequireComponent(typeof(Image))]
    public sealed class MeadowBlend : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper mesh)
        {
            if(!IsActive()||mesh.currentVertCount!=4)return;
            UIVertex bottomLeft=default,topLeft=default,topRight=default,bottomRight=default;
            mesh.PopulateUIVertex(ref bottomLeft,0);mesh.PopulateUIVertex(ref topLeft,1);
            mesh.PopulateUIVertex(ref topRight,2);mesh.PopulateUIVertex(ref bottomRight,3);
            mesh.Clear();
            for(int i=0;i<=48;i++)
            {
                float t=i/48f,opacity=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(1.25f/1.41f,1,t));
                var bottom=bottomLeft;bottom.position=Vector3.Lerp(bottomLeft.position,bottomRight.position,t);bottom.uv0=Vector4.Lerp(bottomLeft.uv0,bottomRight.uv0,t);
                var top=topLeft;top.position=Vector3.Lerp(topLeft.position,topRight.position,t);top.uv0=Vector4.Lerp(topLeft.uv0,topRight.uv0,t);
                bottom.color.a=(byte)(bottom.color.a*opacity);top.color.a=(byte)(top.color.a*opacity);
                mesh.AddVert(bottom);mesh.AddVert(top);
                if(i>0){int n=i*2;mesh.AddTriangle(n-2,n-1,n+1);mesh.AddTriangle(n-2,n+1,n);}
            }
        }
    }
}
