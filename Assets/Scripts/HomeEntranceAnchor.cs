using UnityEngine;

namespace MrRabbit
{
    // Follow the painted doorway, including the Image's aspect-ratio letterboxing.
    public sealed class HomeEntranceAnchor : MonoBehaviour
    {
        public UnityEngine.UI.Image house;
        public RectTransform entrance;
        public void Apply()
        {
            if(house==null||entrance==null||house.sprite==null)return;
            var r=house.GetPixelAdjustedRect();float aspect=house.sprite.rect.width/house.sprite.rect.height;
            if(house.preserveAspect)
            {
                if(r.width/r.height>aspect){float w=r.height*aspect;r.x+=(r.width-w)*.5f;r.width=w;}
                else{float h=r.width/aspect;r.y+=(r.height-h)*.5f;r.height=h;}
            }
            Vector3 door=house.rectTransform.TransformPoint(new Vector3(r.x+r.width*.335f,r.y+r.height*.245f));
            entrance.anchorMin=entrance.anchorMax=new Vector2(.5f,.5f);
            entrance.sizeDelta=Vector2.one*r.height*.20f;
            entrance.position=door;
        }
        void LateUpdate()=>Apply();
    }
}
