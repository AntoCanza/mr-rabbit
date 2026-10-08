using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    public sealed class DraggableRabbit : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform board;
        private RectTransform body;
        private Vector2 grabOffset;
        private int activePointer = int.MinValue;

        public void Configure(RectTransform room) => board = room;

        private void Awake() => body = (RectTransform)transform;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (activePointer != int.MinValue || board == null) return;
            if (body == null) body = (RectTransform)transform;
            activePointer = eventData.pointerId;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(board,
                eventData.position, eventData.pressEventCamera, out Vector2 pointer))
                grabOffset = (Vector2)body.localPosition - pointer;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == activePointer) body.localScale = Vector3.one * 1.04f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != activePointer || board == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(board,
                eventData.position, eventData.pressEventCamera, out Vector2 pointer)) return;
            Vector2 position = pointer + grabOffset;
            Rect bounds = board.rect;
            float halfWidth = body.rect.width * .5f;
            position.x = Mathf.Clamp(position.x, bounds.xMin + halfWidth, bounds.xMax - halfWidth);
            position.y = Mathf.Clamp(position.y, bounds.yMin, bounds.yMax - body.rect.height);
            body.localPosition = position;
        }

        public void OnPointerUp(PointerEventData eventData) => Release(eventData.pointerId);
        public void OnEndDrag(PointerEventData eventData) => Release(eventData.pointerId);

        private void Release(int pointerId)
        {
            if (pointerId != activePointer) return;
            body.localScale = Vector3.one;
            activePointer = int.MinValue;
        }

        private void OnDisable()
        {
            if (body != null) body.localScale = Vector3.one;
            activePointer = int.MinValue;
        }
    }
}
