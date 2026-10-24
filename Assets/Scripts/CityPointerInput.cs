using UnityEngine;
using UnityEngine.EventSystems;
namespace MrRabbit
{
    public sealed class CityPointerInput : MonoBehaviour,IPointerDownHandler,IDragHandler,IBeginDragHandler,IInitializePotentialDragHandler,IPointerUpHandler,IEndDragHandler,ICancelHandler
    {
        [SerializeField] CityController city;
        public void Configure(CityController value)=>city=value;
        readonly RelativeDragGesture gesture=new RelativeDragGesture();
        public void OnPointerDown(PointerEventData e){if(city.State==CityController.Phase.City&&gesture.Begin(e))city.BeginTouchMovement();}
        public void OnInitializePotentialDrag(PointerEventData e)=>e.useDragThreshold=false;
        public void OnBeginDrag(PointerEventData e)=>OnDrag(e);
        public void OnDrag(PointerEventData e){if(gesture.Drag(e))city.SetTouchDirection(gesture.Direction.x);}
        public void OnPointerUp(PointerEventData e){if(gesture.Release(e.pointerId))CancelHold();}
        public void OnEndDrag(PointerEventData e)=>OnPointerUp(e);
        public void CancelHold(){gesture.Cancel();if(city!=null)city.EndTouchMovement();}
        public void OnCancel(BaseEventData e)=>CancelHold();
        void Update(){if(gesture.Active&&(!city.TouchHeld||city.State!=CityController.Phase.City))CancelHold();}
        void OnDisable()=>CancelHold();
        void OnApplicationPause(bool paused){if(paused)CancelHold();}
        void OnApplicationFocus(bool focused){if(!focused)CancelHold();}
    }
}
