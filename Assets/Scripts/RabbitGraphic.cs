using UnityEngine;

namespace MrRabbit
{
    // The supplied character art is rendered as one sprite, with a small walking sway.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RabbitGraphic : UnityEngine.UI.Image
    {
        private float stride;
        private int facing = 1;

        public void SetGait(float phase, int direction, bool walking)
        {
            float next = walking ? Mathf.Sin(phase) : 0;
            if (Mathf.Approximately(next, stride) && direction == facing) return;
            stride = next;
            facing = direction;
            // Pivot is at the feet: the image turns and sways without leaving the walkway.
            rectTransform.localScale = new Vector3(facing, 1 - Mathf.Abs(stride) * .015f, 1);
            rectTransform.localRotation = Quaternion.Euler(0, 0, stride * facing * 1.5f);
        }

        protected override void OnDisable()
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            stride = 0;
            facing = 1;
            base.OnDisable();
        }
    }
}
