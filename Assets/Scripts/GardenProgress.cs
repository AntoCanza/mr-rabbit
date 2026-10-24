using System;
using UnityEngine;

namespace MrRabbit
{
    // Pure saved game state. Growth consumes game-days, never wall-clock/offline time.
    [Serializable]
    public sealed class GardenProgress
    {
        public enum Stage { Empty, Prepared, Seeds, Sprout, Growing, Ripe }
        [Serializable] public sealed class Plot
        {
            public bool prepared, planted;
            public int crop = -1;
            public float growth, water;
            public int seenStage;
        }
        public int version = 1;
        public Plot[] plots = { new Plot(), new Plot(), new Plot() };
        public int[] harvests = new int[6];
        public int[] meals = new int[6];
        public int mealsEaten;
        public static readonly string[] RecipeNames = { "Zuppa di carote", "Pane caldo", "Zuppa di pomodori", "Composta di fragole", "Zuppa di zucca", "Composta di mirtilli" };
        public static readonly string[] CropNames = { "Carote", "Grano", "Pomodori", "Fragole", "Zucche", "Mirtilli" };
        public static readonly float[] GrowDays = { 1f, 1.25f, 1.5f, 1.25f, 1.75f, 1.5f };
        public const float WaterDays = .40f;
        public static int Yield(int crop)=>crop>=4?6:3;

        public Stage GetStage(int i)
        {
            var p = plots[i];
            if (!p.prepared) return Stage.Empty;
            if (!p.planted) return Stage.Prepared;
            float ratio = p.growth / GrowDays[p.crop];
            return ratio >= 1 ? Stage.Ripe : ratio >= .60f ? Stage.Growing : ratio >= .20f ? Stage.Sprout : Stage.Seeds;
        }
        public bool NeedsWater(int i) => plots[i].planted && GetStage(i) != Stage.Ripe && plots[i].water <= .00001f;
        public bool HasGrowthNews(int i) => plots[i].planted && (int)GetStage(i) > plots[i].seenStage;
        public int TotalHarvests { get { int total=0;foreach(int count in harvests)total+=count;return total; } }

        public bool Hoe(int i)
        {
            if (GetStage(i) != Stage.Empty) return false;
            plots[i].prepared = true; return true;
        }
        public bool Choose(int i, int crop)
        {
            if (GetStage(i) != Stage.Prepared || crop < 0 || crop >= 6) return false;
            plots[i].crop = crop; return true;
        }
        public bool Sow(int i)
        {
            var p=plots[i];
            if (GetStage(i)!=Stage.Prepared || p.crop<0) return false;
            p.planted=true;p.growth=p.water=0;p.seenStage=(int)Stage.Seeds;return true;
        }
        public bool Water(int i)
        {
            if (!NeedsWater(i)) return false;
            plots[i].water=WaterDays;return true;
        }
        public bool Harvest(int i)
        {
            if(GetStage(i)!=Stage.Ripe)return false;
            int crop=plots[i].crop;harvests[crop]=Mathf.Min(1000000,harvests[crop]+Yield(crop));
            plots[i]=new Plot();return true;
        }
        public bool Cook(int crop)
        {
            if (crop < 0 || crop >= 6 || harvests[crop] <= 0 || meals[crop] >= 1000000) return false;
            harvests[crop]--; meals[crop]++; return true;
        }
        public bool Eat(int crop)
        {
            if (crop < 0 || crop >= 6 || meals[crop] <= 0) return false;
            meals[crop]--; mealsEaten = Mathf.Min(1000000, mealsEaten + 1); return true;
        }
        public bool AdvanceDays(float days)
        {
            if(float.IsNaN(days)||float.IsInfinity(days)||days<=0)return false;
            bool changed=false;
            for(int i=0;i<plots.Length;i++)
            {
                var p=plots[i];
                if(!p.planted||GetStage(i)==Stage.Ripe||p.water<=0)continue;
                float used=Mathf.Min(days,p.water,GrowDays[p.crop]-p.growth);
                p.growth+=used;p.water=Mathf.Max(0,p.water-used);changed|=used>0;
            }
            return changed;
        }
        public void MarkSeen() {for(int i=0;i<plots.Length;i++)plots[i].seenStage=(int)GetStage(i);}

        public static GardenProgress FromJson(string json)
        {
            try
            {
                var value=JsonUtility.FromJson<GardenProgress>(json);
                if(value==null||value.version!=1||value.plots==null||value.plots.Length!=3||
                    value.harvests==null||(value.harvests.Length!=4&&value.harvests.Length!=6))return new GardenProgress();
                Array.Resize(ref value.harvests,6);Array.Resize(ref value.meals,6);
                for(int i=0;i<3;i++)
                {
                    var p=value.plots[i];
                    if(p==null||p.crop < -1||p.crop>5||p.planted&&(!p.prepared||p.crop<0)||
                        float.IsNaN(p.growth)||float.IsInfinity(p.growth)||float.IsNaN(p.water)||float.IsInfinity(p.water))
                    {value.plots[i]=new Plot();continue;}
                    p.growth=Mathf.Clamp(p.growth,0,p.crop>=0?GrowDays[p.crop]:0);
                    p.water=Mathf.Clamp(p.water,0,WaterDays);
                    p.seenStage=Mathf.Clamp(p.seenStage,0,(int)value.GetStage(i));
                }
                for(int i=0;i<6;i++)value.harvests[i]=Mathf.Clamp(value.harvests[i],0,1000000);
                for(int i=0;i<6;i++)value.meals[i]=Mathf.Clamp(value.meals[i],0,1000000);
                value.mealsEaten=Mathf.Clamp(value.mealsEaten,0,1000000);
                return value;
            }
            catch(ArgumentException) {return new GardenProgress();}
        }
    }
}
