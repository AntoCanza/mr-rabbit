using UnityEngine;
using UnityEngine.EventSystems;
namespace MrRabbit
{
    // Read finger motion, independently of the rabbit or camera. Hold to keep moving.
    public sealed class RelativeDragGesture
    {
        int pointer=int.MinValue;
        Vector2 anchor;
        public bool Active=>pointer!=int.MinValue;
        public Vector2 Direction {get;private set;}
        public bool Begin(PointerEventData e)
        {
            if(Active||e.button!=PointerEventData.InputButton.Left)return false;
            var hit=e.pointerPressRaycast.gameObject;
            if(hit!=null&&hit.GetComponentInParent<UnityEngine.UI.Selectable>()!=null)return false;
            pointer=e.pointerId;anchor=e.position;Direction=Vector2.zero;return true;
        }
        public bool Drag(PointerEventData e,bool vertical=false)
        {
            if(!Active||e.pointerId!=pointer)return false;
            var delta=e.position-anchor;
            float threshold=Mathf.Max(12,Mathf.Min(Screen.width,Screen.height)*.018f);
            float extent=vertical?Mathf.Max(Mathf.Abs(delta.x),Mathf.Abs(delta.y)):Mathf.Abs(delta.x);
            if(extent<threshold)return false;
            Direction=vertical&&Mathf.Abs(delta.y)>Mathf.Abs(delta.x)?new Vector2(0,Mathf.Sign(delta.y)):new Vector2(Mathf.Sign(delta.x),0);
            anchor=e.position;e.eligibleForClick=false;return true;
        }
        public bool Release(int id){if(id!=pointer)return false;Cancel();return true;}
        public void Cancel(){pointer=int.MinValue;Direction=Vector2.zero;}
    }
}
