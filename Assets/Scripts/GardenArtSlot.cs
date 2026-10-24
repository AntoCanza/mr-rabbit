using UnityEngine;
using UnityEngine.UI;
namespace MrRabbit
{
    // Retains the meaning of static atlas pictures when their hand-painted sheet is updated.
    [ExecuteAlways,RequireComponent(typeof(RawImage))]
    public sealed class GardenArtSlot : MonoBehaviour
    {
        public int cell;
        private void OnEnable()=>GardenArt.Apply(GetComponent<RawImage>(),cell);
        public void SetCell(int value){cell=value;GardenArt.Apply(GetComponent<RawImage>(),cell);}
    }
}
