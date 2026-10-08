using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    // Mouse/touch use the same constrained movement as the arrow keys.
    public sealed class HousePointerInput : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RabbitWalker rabbit;
        private int activePointer = int.MinValue;

        public void Configure(RabbitWalker walker) => rabbit = walker;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (activePointer != int.MinValue || eventData.button != PointerEventData.InputButton.Left) return;
            activePointer = eventData.pointerId;
            SetDestination(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData) { }
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == activePointer) SetDestination(eventData);
        }

        public void OnPointerUp(PointerEventData eventData) => Release(eventData.pointerId);
        public void OnEndDrag(PointerEventData eventData) => Release(eventData.pointerId);
        private void OnDisable() => activePointer = int.MinValue;
        private void Release(int pointer)
        {
            if (pointer == activePointer) activePointer = int.MinValue;
        }

        private void SetDestination(PointerEventData eventData)
        {
            var house = (RectTransform)transform;
            if (rabbit == null || house.rect.width <= 0 || house.rect.height <= 0) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(house, eventData.position,
                eventData.pressEventCamera, out Vector2 point))
                rabbit.WalkTo(new Vector2((point.x - house.rect.xMin) / house.rect.width,
                    (point.y - house.rect.yMin) / house.rect.height));
        }
    }
}
