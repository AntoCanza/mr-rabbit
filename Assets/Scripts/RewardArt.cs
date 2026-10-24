using UnityEngine;
namespace MrRabbit
{
    public sealed class RewardArt : MonoBehaviour
    {
        public Texture2D atlas,garden;
        static RewardArt instance;
        void Awake(){instance=this;}
        public static void Apply(UnityEngine.UI.RawImage image,int crop,bool plant=false)
        {
            if(instance==null)return;image.texture=instance.atlas;int cell=(plant?2:0)+crop-4;
            image.uvRect=new Rect(cell%2*.5f,cell<2?.5f:0,.5f,.5f);var fit=image.GetComponent<UnityEngine.UI.AspectRatioFitter>();if(fit!=null)fit.aspectRatio=1;
        }
        public static void RestoreGarden(UnityEngine.UI.RawImage image){if(instance!=null)image.texture=instance.garden;}
        public static void Crop(UnityEngine.UI.RawImage image,int crop)
        {if(crop>=4)Apply(image,crop);else{RestoreGarden(image);GardenArt.Apply(image,CityController.CropCell(crop));}}
        void OnDestroy(){if(instance==this)instance=null;}
    }
}
