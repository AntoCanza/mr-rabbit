using System;
using UnityEngine;

namespace MrRabbit
{
    // Stable IDs keep earned achievements and wall positions compatible with future lessons.
    public enum LearningSkill { Vowels, Sounds, Syllables, Words, Sentences, Count10, Numbers20, Add5, Add10, Add20, Letters }

    [Serializable]
    public sealed class LearningProgress
    {
        public const string SaveKey="MrRabbit.Learning.it-IT.v1";
        public const int SkillCount=11, PracticeTarget=3, TestLength=5, PassScore=4;
        public static LearningProgress Current {get;set;}
        public int version=1;
        public bool[] medals=new bool[SkillCount];
        public int[] practice=new int[SkillCount];
        public int[] best=new int[SkillCount];
        public int[] wall={-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
        public bool[] rewardOwned=new bool[4]; // two additional outfits, one per avatar
        public static readonly string[] Names={"Vocali","Suoni e lettere","Sillabe","Parole","Piccole frasi","Numeri fino a 10","Numeri fino a 20","Somme fino a 5","Somme fino a 10","Somme fino a 20","Scrivo le lettere"};
        public static readonly string[] Marks={"AE","ABC","MA","MELA","A B","10","20","+5","+10","+20","ABC"};
        public static readonly int[][] Prerequisites={new int[0],new[]{0},new[]{1},new[]{2},new[]{3},new int[0],new[]{5},new[]{5},new[]{7},new[]{6,8},new[]{1}};
        public bool Available(int id)
        {if(id<0||id>=SkillCount)return false;foreach(int p in Prerequisites[id])if(!medals[p])return false;return true;}
        public bool CanTest(int id)=>Available(id)&&practice[id]>=PracticeTarget;
        public bool RecordPractice(int id)
        {if(!Available(id)||practice[id]>=PracticeTarget)return false;practice[id]++;Store();return true;}
        public bool CompleteTest(int id,int score)
        {
            if(!CanTest(id)||score<0||score>TestLength)return false;
            best[id]=Mathf.Max(best[id],score);bool earned=score>=PassScore&&!medals[id];
            if(score>=PassScore)medals[id]=true;Store();return earned;
        }
        public bool CropUnlocked(int crop)=>crop>=0&&crop<6&&(crop<4||medals[crop==4?0:8]);
        public bool OutfitUnlocked(int outfit)=>outfit>=0&&outfit<=3&&(outfit<2||medals[outfit==2?10:9]);
        public bool Hang(int medal,int slot)
        {
            if(medal<0||medal>=SkillCount||!medals[medal]||slot<0||slot>=wall.Length)return false;
            for(int i=0;i<wall.Length;i++)if(wall[i]==medal)wall[i]=-1;
            wall[slot]=medal;Store();return true;
        }
        public static LearningProgress Load()
        {
            LearningProgress p=null;try{p=JsonUtility.FromJson<LearningProgress>(PlayerPrefs.GetString(SaveKey,""));}catch(ArgumentException){}
            if(p==null||p.version!=1)p=new LearningProgress();
            Array.Resize(ref p.medals,SkillCount);Array.Resize(ref p.practice,SkillCount);Array.Resize(ref p.best,SkillCount);Array.Resize(ref p.rewardOwned,4);
            if(p.wall==null||p.wall.Length!=12)p.wall=new[]{-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
            var seen=new bool[SkillCount];for(int i=0;i<p.wall.Length;i++){int n=p.wall[i];if(n<0||n>=SkillCount||!p.medals[n]||seen[n])p.wall[i]=-1;else seen[n]=true;}
            for(int i=0;i<SkillCount;i++){p.practice[i]=Mathf.Clamp(p.practice[i],0,PracticeTarget);p.best[i]=Mathf.Clamp(p.best[i],0,TestLength);}
            return p;
        }
        public void Store(bool flush=true){PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(this));if(flush)PlayerPrefs.Save();}
    }
}
