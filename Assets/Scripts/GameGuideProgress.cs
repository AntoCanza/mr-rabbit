using System;
using System.Collections.Generic;
using UnityEngine;

namespace MrRabbit
{
    // Tutorial memory is independent of school rewards, inventory and character choice.
    [Serializable]
    public sealed class GameGuideProgress
    {
        public const string SaveKey="MrRabbit.Guide.it-IT.v1";
        public int version=1,introPage;
        public bool introFinished,paused;
        public List<string> completed=new List<string>();
        public bool Done(string id)=>completed.Contains(id);
        public bool Complete(string id)
        {if(string.IsNullOrEmpty(id)||Done(id))return false;completed.Add(id);return true;}
        public void Store(){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(this));PlayerPrefs.Save();}
        public static GameGuideProgress Load()
        {
            GameGuideProgress p=null;
            try{p=JsonUtility.FromJson<GameGuideProgress>(PlayerPrefs.GetString(SaveKey,""));}catch(ArgumentException){}
            if(p==null||p.version!=1)return new GameGuideProgress();
            p.introPage=Mathf.Clamp(p.introPage,0,3);
            if(p.completed==null)p.completed=new List<string>();
            p.completed.RemoveAll(s=>string.IsNullOrEmpty(s)||s.Length>64);
            p.completed=new List<string>(new HashSet<string>(p.completed));
            return p;
        }
    }
}
