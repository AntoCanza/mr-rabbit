using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    // Mouse/touch use the same constrained movement as the arrow keys.
    public sealed class HousePointerInput : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,IInitializePotentialDragHandler,ICancelHandler
    {
        [SerializeField] private RabbitWalker rabbit;
        [SerializeField] private BedInteraction bed;
        readonly RelativeDragGesture gesture=new RelativeDragGesture();
        bool started;
        public Vector2 Direction=>gesture.Direction;

        public void Configure(RabbitWalker walker) => rabbit = walker;
        public void SetBed(BedInteraction value) => bed = value;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(gesture.Begin(eventData))started=false;
        }

        public void OnInitializePotentialDrag(PointerEventData e)=>e.useDragThreshold=false;
        public void OnBeginDrag(PointerEventData eventData)=>OnDrag(eventData);
        public void OnDrag(PointerEventData eventData)
        {
            if(!gesture.Drag(eventData,true))return;
            if(!started)
            {
                bed?.Wake();GetComponent<HomeActivities>()?.CancelActivity();
                rabbit.StopWalking();rabbit.BeginTouchMovement();started=true;
            }
            rabbit.SetTouchDirection(gesture.Direction);
        }

        public void OnPointerUp(PointerEventData eventData) => Release(eventData.pointerId);
        public void OnEndDrag(PointerEventData eventData) => Release(eventData.pointerId);
        private void OnDisable()=>CancelHold();
        private void Release(int pointer)
        {
            if(gesture.Release(pointer))CancelHold();
        }

        void Update(){if(started&&!rabbit.TouchHeld)CancelHold();}
        public void CancelHold(){gesture.Cancel();started=false;if(rabbit!=null)rabbit.EndTouchMovement();}
        public void OnCancel(BaseEventData e)=>CancelHold();
        void OnApplicationPause(bool paused){if(paused)CancelHold();}
        void OnApplicationFocus(bool focused){if(!focused)CancelHold();}
    }
}
