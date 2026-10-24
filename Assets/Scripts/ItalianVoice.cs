using System;
using UnityEngine;

namespace MrRabbit
{
    // Pre-recorded Italian clips ship with the player. No speech service or network at runtime.
    public sealed class ItalianVoice : MonoBehaviour
    {
        [Serializable] public sealed class Entry {public string key;public AudioClip clip;}
        public const string MuteKey="MrRabbit.Voice.Muted";
        public Entry[] clips;
        public UnityEngine.UI.Button repeat,mute;
        public CitySymbol muteSymbol;
        AudioSource source;
        OutdoorAdventure adventure;
        GardenController garden;
        SchoolController school;
        LearningJourney learning;
        GameGuide guide;
        OutdoorAdventure.Place lastPlace=(OutdoorAdventure.Place)(-1);
        string lastKey,lastNotice;
        float poll,noticeTime;
        bool suspended;
        public bool Muted {get;private set;}
        public string LastKey=>lastKey;
        public bool IsSpeaking=>source!=null&&source.isPlaying;
        public AudioSource Source=>source;
        void Awake()
        {
            adventure=GetComponent<OutdoorAdventure>();garden=GetComponent<GardenController>();school=GetComponent<SchoolController>();learning=GetComponent<LearningJourney>();
            guide=GetComponent<GameGuide>();
            source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.volume=.8f;
            Muted=PlayerPrefs.GetInt(MuteKey,0)!=0;repeat.onClick.AddListener(Repeat);mute.onClick.AddListener(ToggleMute);Refresh();
        }
        public bool HasClip(string key){foreach(var e in clips)if(e.key==key&&e.clip!=null)return true;return false;}
        public bool Say(string key,bool requested=false)
        {
            bool guideLine=key!=null&&key.StartsWith("guide_");
            if(!requested&&!guideLine&&guide!=null)
            {
                if(guide.IntroOpen||guide.MenuOpen)return false;
                if(guide.HasGuidance&&(key.StartsWith("tool_")||key=="home"||key=="garden"||key=="school"||key=="city"||key=="train"||key=="notebook"||key=="wardrobe"||key=="wall"))return false;
            }
            foreach(var e in clips)if(e.key==key&&e.clip!=null)
            {
                lastKey=key;if(suspended||Muted&&!requested)return false;
                source.Stop();source.clip=e.clip;source.Play();return true;
            }
            return false;
        }
        public void Repeat(){if(lastKey!=null)Say(lastKey,true);}
        public void Stop(){source?.Stop();}
        public void ToggleMute()
        {Muted=!Muted;PlayerPrefs.SetInt(MuteKey,Muted?1:0);PlayerPrefs.Save();if(Muted)Stop();Refresh();}
        void Refresh(){if(muteSymbol!=null){muteSymbol.symbol=Muted?CitySymbol.Kind.Muted:CitySymbol.Kind.Sound;muteSymbol.SetAllDirty();}}
        void Update()
        {
            bool visible=(adventure.Location!=OutdoorAdventure.Place.Selection||(guide!=null&&guide.HasGuidance))&&!(guide!=null&&guide.IntroOpen);
            repeat.gameObject.SetActive(visible);mute.gameObject.SetActive(visible);
            if(suspended||!visible)return;
            if(adventure.Location!=lastPlace)
            {
                lastPlace=adventure.Location;Stop();
                if((guide==null||!guide.EnabledGuide)&&(learning==null||!learning.IsOpen))
                {
                    string key=lastPlace==OutdoorAdventure.Place.Home?"home":lastPlace==OutdoorAdventure.Place.Garden?"garden":lastPlace==OutdoorAdventure.Place.School?"school":lastPlace==OutdoorAdventure.Place.City?"city":lastPlace==OutdoorAdventure.Place.Train?"train":null;
                    if(key!=null)Say(key);
                }
            }
            poll-=Time.unscaledDeltaTime;if(poll>0)return;poll=1;
            if(guide!=null&&guide.HasGuidance)return;
            if(IsSpeaking||learning!=null&&learning.IsOpen||school.State==SchoolController.Phase.Packing)return;
            if(adventure.Location!=OutdoorAdventure.Place.Meadow&&adventure.Location!=OutdoorAdventure.Place.Garden)return;
            bool water=false,ripe=false;for(int i=0;i<3;i++){water|=garden.Progress.NeedsWater(i);ripe|=garden.Progress.GetStage(i)==GardenProgress.Stage.Ripe;}
            string notice=ripe?"harvest":water?"water":null;
            if(notice==null){lastNotice=null;return;}
            // A reminder is contextual and rate-limited, never a narration of every frame.
            if(notice!=lastNotice&&Time.unscaledTime>=noticeTime){Say(notice);lastNotice=notice;noticeTime=Time.unscaledTime+60;}
        }
        void OnApplicationPause(bool pause){suspended=pause;if(pause)Stop();}
        void OnDestroy(){if(repeat!=null)repeat.onClick.RemoveListener(Repeat);if(mute!=null)mute.onClick.RemoveListener(ToggleMute);}
    }
}
