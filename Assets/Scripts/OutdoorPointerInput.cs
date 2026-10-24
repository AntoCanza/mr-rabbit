using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    // A captured gesture sets a direction, never a world point that the camera can overtake.
    public sealed class OutdoorPointerInput : MonoBehaviour, IPointerDownHandler, IDragHandler,
        IInitializePotentialDragHandler, IBeginDragHandler, IPointerUpHandler, IEndDragHandler, ICancelHandler
    {
        [SerializeField] private OutdoorAdventure adventure;
        [SerializeField] private MovementDirectionGraphic arrow;
        private readonly RelativeDragGesture gesture=new RelativeDragGesture();
        public bool IsHolding=>gesture.Active;
        public float Direction=>adventure.TouchDirection;
        public MovementDirectionGraphic Arrow=>arrow;
        public void Configure(OutdoorAdventure value)=>adventure=value;
        public void SetArrow(MovementDirectionGraphic value)=>arrow=value;
        public void OnPointerDown(PointerEventData data)
        {
            if(adventure.Location!=OutdoorAdventure.Place.Meadow||!gesture.Begin(data))return;
            adventure.BeginTouchMovement();ShowArrow();
        }
        public void OnInitializePotentialDrag(PointerEventData data)=>data.useDragThreshold=false;
        public void OnBeginDrag(PointerEventData data)=>OnDrag(data);
        public void OnDrag(PointerEventData data)
        {
            if(gesture.Drag(data))adventure.SetTouchDirection(gesture.Direction.x);
            ShowArrow();
        }
        private void Update()=>AdvanceHold(Time.unscaledDeltaTime);
        public void AdvanceHold(float seconds)
        {
            if(!IsHolding)return;
            if(adventure.Location!=OutdoorAdventure.Place.Meadow||!adventure.TouchHeld){CancelHold();return;}
            ShowArrow();
        }
        private void LateUpdate()=>ShowArrow();
        private void ShowArrow()
        {
            if(arrow==null)return;
            bool visible=IsHolding&&adventure.TouchHeld&&adventure.TouchDirection!=0&&adventure.Location==OutdoorAdventure.Place.Meadow;
            arrow.enabled=visible;
            if(!visible)return;
            float direction=adventure.TouchDirection;
            var body=adventure.PlayerRect;var r=arrow.rectTransform;
            float x=(body.anchorMin.x+body.anchorMax.x)*.5f+direction*.105f;
            r.anchorMin=new Vector2(x-.035f,.115f);r.anchorMax=new Vector2(x+.035f,.16f);
            r.offsetMin=r.offsetMax=Vector2.zero;r.localScale=new Vector3(direction,1,1);
        }
        public void OnPointerUp(PointerEventData data)=>Release(data.pointerId);
        public void OnEndDrag(PointerEventData data)=>Release(data.pointerId);
        private void Release(int id){if(gesture.Release(id))CancelHold();}
        public void CancelHold(){gesture.Cancel();if(adventure!=null)adventure.EndTouchMovement();if(arrow!=null)arrow.enabled=false;}
        public void OnCancel(BaseEventData data)=>CancelHold();
        private void OnDisable()=>CancelHold();
        private void OnApplicationPause(bool paused){if(paused)CancelHold();}
        private void OnApplicationFocus(bool focused){if(!focused)CancelHold();}
    }
}
