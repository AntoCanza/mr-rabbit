using UnityEngine;
using UnityEngine.UI;

namespace MrRabbit
{
    // World-space action pictures stay tappable even when the rabbit stands in front.
    [DisallowMultipleComponent, RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public sealed class WorldActionLayer : MonoBehaviour
    {
        Canvas layer;
        void OnEnable()
        {
            layer=GetComponent<Canvas>();
            layer.overrideSorting=true;layer.sortingOrder=10;
        }
    }
}
