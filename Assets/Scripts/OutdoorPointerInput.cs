using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class OutdoorPointerInput : MonoBehaviour, IPointerDownHandler, IDragHandler,
        IBeginDragHandler, IPointerUpHandler, IEndDragHandler
    {
        [SerializeField] private OutdoorAdventure adventure;
        private int activePointer = int.MinValue;
        public void Configure(OutdoorAdventure value) => adventure = value;
        public void OnPointerDown(PointerEventData data)
        {
            if (activePointer != int.MinValue || data.button != PointerEventData.InputButton.Left) return;
            activePointer = data.pointerId;
            SetDestination(data);
        }
        public void OnBeginDrag(PointerEventData data) { }
        public void OnDrag(PointerEventData data)
        {
            if (data.pointerId == activePointer) SetDestination(data);
        }
        public void OnPointerUp(PointerEventData data) => Release(data.pointerId);
        public void OnEndDrag(PointerEventData data) => Release(data.pointerId);
        private void Release(int id) { if (id == activePointer) activePointer = int.MinValue; }
        private void OnDisable() => activePointer = int.MinValue;
        private void SetDestination(PointerEventData data)
        {
            var area = (RectTransform)transform;
            if (area.rect.width > 0 && RectTransformUtility.ScreenPointToLocalPointInRectangle(area,
                data.position, data.pressEventCamera, out Vector2 point))
                adventure.WalkToScreen((point.x - area.rect.xMin) / area.rect.width);
        }
    }
}
