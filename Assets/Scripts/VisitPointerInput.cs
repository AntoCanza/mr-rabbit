using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class VisitPointerInput : MonoBehaviour, IPointerDownHandler, IDragHandler,IBeginDragHandler,IInitializePotentialDragHandler,IPointerUpHandler,IEndDragHandler,ICancelHandler
    {
        [SerializeField] private Neighborhood neighborhood;
        public void Configure(Neighborhood value) => neighborhood = value;
        readonly RelativeDragGesture gesture=new RelativeDragGesture();
        public void OnPointerDown(PointerEventData e){if(neighborhood.Visiting>=0&&gesture.Begin(e))neighborhood.BeginTouchMovement();}
        public void OnInitializePotentialDrag(PointerEventData e)=>e.useDragThreshold=false;
        public void OnBeginDrag(PointerEventData e)=>OnDrag(e);
        public void OnDrag(PointerEventData e){if(gesture.Drag(e))neighborhood.SetTouchDirection(gesture.Direction.x);}
        public void OnPointerUp(PointerEventData e){if(gesture.Release(e.pointerId))CancelHold();}
        public void OnEndDrag(PointerEventData e)=>OnPointerUp(e);
        public void CancelHold(){gesture.Cancel();if(neighborhood!=null)neighborhood.EndTouchMovement();}
        public void OnCancel(BaseEventData e)=>CancelHold();
        void Update(){if(gesture.Active&&(!neighborhood.TouchHeld||neighborhood.Visiting<0))CancelHold();}
        void OnDisable()=>CancelHold();
        void OnApplicationPause(bool paused){if(paused)CancelHold();}
        void OnApplicationFocus(bool focused){if(!focused)CancelHold();}
    }
}
