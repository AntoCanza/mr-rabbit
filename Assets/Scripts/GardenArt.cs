using UnityEngine;
using UnityEngine.UI;
namespace MrRabbit
{
    // Hand-drawn sheets have organic spacing: use each complete drawing's bounds.
    public static class GardenArt
    {
        static readonly Rect[] Tools={
            new Rect(35,42,267,283),new Rect(320,89,338,228),new Rect(682,90,217,235),new Rect(970,56,259,267),
            new Rect(64,336,249,298),new Rect(359,383,245,240),new Rect(666,342,250,293),new Rect(1010,388,195,241),
            new Rect(57,714,242,212),new Rect(332,644,293,284),new Rect(642,650,309,280),new Rect(976,635,257,296),
            new Rect(34,961,261,255),new Rect(331,923,285,296),new Rect(628,974,294,246),new Rect(941,968,300,249)};
        static readonly Rect[] Actions={
            new Rect(36,57,288,282),new Rect(370,58,290,294),new Rect(656,94,299,255),new Rect(958,89,280,261),
            new Rect(16,403,360,231),new Rect(402,383,199,271),new Rect(654,381,293,272),new Rect(967,422,255,224),
            new Rect(35,689,301,246),new Rect(368,693,270,240),new Rect(667,679,256,261),new Rect(980,676,253,277),
            new Rect(32,981,340,222),new Rect(382,958,224,252),new Rect(647,997,297,197),new Rect(971,996,271,197)};
        public static void Apply(RawImage image,int index)
        {
            Rect pixels=(image.texture.name=="ActionIcons"?Actions:Tools)[index];
            var uv=new Rect(pixels.x/1254f,1-pixels.yMax/1254f,pixels.width/1254f,pixels.height/1254f);
            if(image.uvRect!=uv)image.uvRect=uv;
            var fit=image.GetComponent<AspectRatioFitter>();
            if(fit!=null)fit.aspectRatio=pixels.width/pixels.height;
        }
        public static int FindCell(RawImage image)
        {
            var bounds=image.texture.name=="ActionIcons"?Actions:Tools;
            int best=0;float distance=float.MaxValue;
            for(int i=0;i<bounds.Length;i++)
            {
                var p=bounds[i];var center=new Vector2(p.center.x/1254f,1-p.center.y/1254f);
                float delta=(center-image.uvRect.center).sqrMagnitude;
                if(delta<distance){distance=delta;best=i;}
            }
            return best;
        }
    }
}
