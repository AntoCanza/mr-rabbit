using UnityEngine;

namespace MrRabbit
{
    // All village landmarks sit on one continuous landscape, including its sky and path.
    public sealed class SharedMeadowLandscape : MonoBehaviour
    {
        [SerializeField] DayNightCycle clock;
        [SerializeField] UnityEngine.UI.RawImage landscape;
        public UnityEngine.UI.RawImage Landscape => landscape;
        public void Configure(DayNightCycle dayNight,UnityEngine.UI.RawImage painting)
        {clock=dayNight;landscape=painting;Refresh();}
        void LateUpdate()=>Refresh();
        void Refresh()
        {
            if(clock==null||landscape==null)return;
            Color tint=Color.Lerp(Color.white,new Color(.29f,.38f,.62f),clock.NightAmount);
            float time=clock.NormalizedTime;
            float dusk=time>=.4f&&time<.5f?Mathf.Sin((time-.4f)*Mathf.PI/.1f):time>=.9f?Mathf.Sin((time-.9f)*Mathf.PI/.1f):0;
            landscape.color=Color.Lerp(tint,new Color(1,.65f,.48f),dusk*.45f);
        }
    }
}
