using UnityEngine;

namespace MrRabbit
{
    // Crop decorative margins, preserving the painting and all walkway coordinates.
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public sealed class GameplayFraming : MonoBehaviour
    {
        [SerializeField] private float aspect = 4f / 3f;
        [SerializeField] private Rect playableArea = new Rect(.13f, .07f, .74f, .64f);
        private Vector2 lastSize;
        [SerializeField] private RabbitWalker followTarget;
        [SerializeField, Min(.01f)] private float followSeconds = .12f;

        public void Follow(RabbitWalker target) { followTarget = target; Apply(); }

        public void Configure(float imageAspect, Rect area)
        {
            aspect = imageAspect;
            playableArea = area;
            Apply();
        }

        private void OnEnable() => Apply();
        private void LateUpdate()
        {
            var parent = transform.parent as RectTransform;
            if (parent != null && parent.rect.size != lastSize) Apply();
            if (Application.isPlaying && followTarget != null) FollowCharacter(Time.deltaTime);
        }

        public void FollowCharacter(float seconds)
        {
            if (followTarget == null || lastSize.x <= 0) return;
            var rect = (RectTransform)transform;
            // Track the body's centre, not its feet; clamp only at the painting's edges.
            var focus = followTarget.PositionNormalized + new Vector2(0, .085f);
            var visible = lastSize / rect.rect.size;
            focus.x = Mathf.Clamp(focus.x, visible.x * .5f, 1 - visible.x * .5f);
            focus.y = Mathf.Clamp(focus.y, visible.y * .5f, 1 - visible.y * .5f);
            var target = Vector2.Scale(Vector2.one * .5f - focus, rect.rect.size);
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target,
                1 - Mathf.Exp(-Mathf.Max(0, seconds) / followSeconds));
        }

        public void Apply()
        {
            var rect = (RectTransform)transform;
            var parent = transform.parent as RectTransform;
            if (parent == null || parent.rect.width <= 0 || parent.rect.height <= 0) return;
            lastSize = parent.rect.size;
            float width = Mathf.Min(lastSize.x / playableArea.width,
                lastSize.y * aspect / playableArea.height);
            // The painting always covers the screen; no paper bars or stretched artwork.
            width = Mathf.Max(width, lastSize.x, lastSize.y * aspect);
            float height = width / aspect;
            var visible = new Vector2(lastSize.x / width, lastSize.y / height);
            var focus = playableArea.center;
            focus.x = Mathf.Clamp(focus.x, visible.x * .5f, 1 - visible.x * .5f);
            focus.y = Mathf.Clamp(focus.y, visible.y * .5f, 1 - visible.y * .5f);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2((.5f - focus.x) * width, (.5f - focus.y) * height);
            if (Application.isPlaying && followTarget != null) FollowCharacter(10);
        }
    }
}
