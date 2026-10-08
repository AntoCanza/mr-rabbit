using UnityEngine;

namespace MrRabbit
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaLayout : MonoBehaviour
    {
        private Rect lastArea;
        private Vector2Int lastScreen;

        private void OnEnable()
        {
            if (Application.isPlaying) Application.targetFrameRate = 60;
            Apply();
        }

        private void Update()
        {
            if (lastScreen.x != Screen.width || lastScreen.y != Screen.height || lastArea != Screen.safeArea)
                Apply();
        }

        private void Apply()
        {
            if (Screen.width <= 0 || Screen.height <= 0) return;
            Rect area = Screen.safeArea;
            if (area.width <= 0 || area.height <= 0) area = new Rect(0, 0, Screen.width, Screen.height);
            var rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
            rect.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            lastArea = area;
            lastScreen = new Vector2Int(Screen.width, Screen.height);
        }
    }
}
