using UnityEngine;
using UnityEngine.EventSystems;

namespace MrRabbit
{
    // Two fixed meadow friends share this lightweight tap-to-greet behavior.
    public sealed class MeadowFriend : MonoBehaviour
    {
        [SerializeField] private string friendName;
        [SerializeField] private UnityEngine.UI.Button greetButton;
        [SerializeField] private RectTransform portrait;
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private TMPro.TMP_Text speech;
        [SerializeField, TextArea] private string greeting;
        [SerializeField, Min(1)] private float greetingSeconds = 4.5f;
        private float remaining;
        private RectTransform viewport, bubbleRect;
        public string FriendName => friendName;
        public bool IsGreeting => remaining > 0;

        public void Configure(string name, UnityEngine.UI.Button button, RectTransform body,
            GameObject bubble, TMPro.TMP_Text words, string message)
        {
            friendName = name; greetButton = button; portrait = body;
            speechBubble = bubble; speech = words; greeting = message;
        }

        private void Awake()
        {
            viewport = (RectTransform)transform.parent.parent;
            bubbleRect = (RectTransform)speechBubble.transform;
            greetButton.onClick.AddListener(Greet);
        }

        public void Greet()
        {
            if (!isActiveAndEnabled) return;
            remaining = greetingSeconds;
            speech.text = greeting;
            speechBubble.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }

        private void Update() => Advance(Time.deltaTime);

        private void LateUpdate()
        {
            if (!IsGreeting) return;
            // Keep the words inside the meadow when a friend is near a screen edge.
            var body = (RectTransform)transform;
            float friendX = viewport.InverseTransformPoint(body.TransformPoint(body.rect.center)).x;
            if (friendX < viewport.rect.xMin - body.rect.width * .5f ||
                friendX > viewport.rect.xMax + body.rect.width * .5f)
            {
                ResetGreeting();
                return;
            }
            var position = bubbleRect.anchoredPosition;
            position.x = 0;
            bubbleRect.anchoredPosition = position;
            float center = viewport.InverseTransformPoint(bubbleRect.TransformPoint(bubbleRect.rect.center)).x;
            float halfWidth = bubbleRect.rect.width * .5f;
            position.x = Mathf.Clamp(center, viewport.rect.xMin + halfWidth + 12,
                viewport.rect.xMax - halfWidth - 12) - center;
            bubbleRect.anchoredPosition = position;
        }

        public void Advance(float seconds)
        {
            if (!IsGreeting) return;
            remaining = Mathf.Max(0, remaining - Mathf.Max(0, seconds));
            if (!IsGreeting) { ResetGreeting(); return; }
            // A brief friendly sway; no Animator, particles or per-frame allocations.
            float elapsed = greetingSeconds - remaining;
            float wave = Mathf.Clamp01(1 - elapsed / 1.2f);
            portrait.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(elapsed * 12) * 4 * wave);
        }

        private void ResetGreeting()
        {
            remaining = 0;
            if (speechBubble != null) speechBubble.SetActive(false);
            if (portrait != null) portrait.localRotation = Quaternion.identity;
        }

        private void OnDisable() => ResetGreeting();
        private void OnDestroy()
        {
            if (greetButton != null) greetButton.onClick.RemoveListener(Greet);
        }
    }
}
