using UnityEngine;
using UnityEngine.UI;

namespace MrRabbit
{
    // All feedback is local and offline. The teacher's reminder is a gentle head shake.
    public sealed class SchoolFeedback : MonoBehaviour
    {
        public enum Sound { Listen, Retry, Success, Help, Complete }
        public GameObject reminder,helpCost;
        public RectTransform portrait;
        public TMPro.TMP_Text delta;
        public RawImage rewardPicture;
        public ActionCue helpCue;
        public Rect starUv,checkUv;
        AudioSource source;
        AudioClip[] clips;
        float reminderTime;
        public bool SoundPlaying=>source!=null&&source.isPlaying;
        void Awake()
        {
            source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.volume=.42f;
            clips=new[]{Tone("Listen",523,659),Tone("Try again",330,294),Tone("Star",523,659,784),Tone("Teacher reminder",392,330,294),Tone("Completed",523,659)};
            ResetFeedback();
        }
        public void Play(Sound sound){if(source!=null)source.PlayOneShot(clips[(int)sound]);}
        public void RefreshHelp(bool used,bool available)
        {if(helpCost!=null)helpCost.SetActive(false);if(helpCue!=null)helpCue.SetHighlighted(available&&!used);}
        public void HelpUsed(int lost)
        {
            Play(Sound.Help);delta.text=lost>0?"−1":"0";
            reminderTime=2.3f;reminder.SetActive(true);
        }
        public void Complete(bool assisted)
        {
            HideReminder();rewardPicture.uvRect=assisted?checkUv:starUv;
            var fit=rewardPicture.GetComponent<AspectRatioFitter>();fit.aspectRatio=assisted?300f/280:295f/285;
            Play(assisted?Sound.Complete:Sound.Success);
        }
        public void ResetFeedback(){HideReminder();if(rewardPicture!=null)rewardPicture.uvRect=starUv;}
        public void HideReminder(){reminderTime=0;if(reminder!=null)reminder.SetActive(false);if(portrait!=null)portrait.localRotation=Quaternion.identity;}
        void Update()
        {
            if(reminderTime<=0)return;
            reminderTime-=Time.unscaledDeltaTime;
            portrait.localRotation=Quaternion.Euler(0,0,Mathf.Sin((2.3f-reminderTime)*11)*7);
            if(reminderTime<=0)HideReminder();
        }
        void OnApplicationPause(bool paused){if(paused)source?.Stop();}
        void OnDestroy(){if(clips!=null)foreach(var clip in clips)Destroy(clip);}
        static AudioClip Tone(string name,params float[] notes)
        {
            const int rate=22050;const float beat=.19f;
            var data=new float[Mathf.CeilToInt(rate*beat*notes.Length)];
            for(int i=0;i<data.Length;i++)
            {
                float t=i/(float)rate,local=t%beat,f=notes[Mathf.Min(notes.Length-1,(int)(t/beat))];
                float envelope=Mathf.Min(1,local/.012f)*Mathf.Exp(-local*10)*Mathf.Clamp01((beat-local)/.03f);
                data[i]=Mathf.Sin(2*Mathf.PI*f*local)*envelope*.65f;
            }
            var clip=AudioClip.Create(name,data.Length,1,rate,false);clip.SetData(data,0);return clip;
        }
    }
}
