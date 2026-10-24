using UnityEngine;

namespace MrRabbit
{
    // Animate only the picture: the surrounding touch area stays still and easy to hit.
    public sealed class ActionCue : MonoBehaviour
    {
        [SerializeField] private RectTransform picture;
        [SerializeField] private UnityEngine.UI.Image halo;
        [SerializeField] private bool guidedOnly;
        private Vector3 restingScale;
        private Quaternion restingRotation;
        private bool initialized, highlighted;
        public bool Highlighted => highlighted;
        public bool GuidedOnly => guidedOnly;
        public RectTransform Picture => picture;

        public void Configure(RectTransform visual, UnityEngine.UI.Image glow, bool controlled)
        { picture=visual; halo=glow; guidedOnly=controlled; Initialize(); }
        private void Initialize()
        {
            if(initialized||picture==null)return;
            restingScale=picture.localScale;restingRotation=picture.localRotation;initialized=true;
        }
        private void OnEnable(){Initialize();SetHighlighted(false);}
        public void SetHighlighted(bool value)
        {
            highlighted=value;
            if(halo!=null)halo.enabled=value;
            if(!value&&guidedOnly)ResetPicture();
        }
        private void LateUpdate()
        {
            if(!initialized||picture==null)return;
            if(guidedOnly&&!highlighted)return;
            float phase=Time.unscaledTime*(highlighted?3.8f:2.1f);
            float pulse=(1-Mathf.Cos(phase))*.5f;
            picture.localScale=restingScale*(1+pulse*(highlighted?.14f:.035f));
            picture.localRotation=restingRotation*Quaternion.Euler(0,0,Mathf.Sin(phase)*(highlighted?4f:1.5f));
            if(highlighted&&halo!=null)halo.color=new Color(1f,.80f,.29f,.30f+pulse*.35f);
        }
        private void ResetPicture()
        {if(initialized&&picture!=null){picture.localScale=restingScale;picture.localRotation=restingRotation;}}
        private void OnDisable(){highlighted=false;if(halo!=null)halo.enabled=false;ResetPicture();}
    }
}
