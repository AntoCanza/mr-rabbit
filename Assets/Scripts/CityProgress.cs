using System;
using UnityEngine;

namespace MrRabbit
{
    [Serializable]
    public sealed class CityProgress
    {
        public const string SaveKey = "MrRabbit.City.v1";
        public int version = 1, coins, crop, quantity = 3, reward = 3;
        public bool accepted;
        public float offerDays;
        public bool[] owned = new bool[2];
        public int[] equipped = new int[2];

        public static CityProgress Load()
        {
            CityProgress p = null;
            try { p = JsonUtility.FromJson<CityProgress>(PlayerPrefs.GetString(SaveKey, "")); }
            catch (ArgumentException) { }
            if (p == null || p.version != 1) p = new CityProgress();
            if (p.owned == null || p.owned.Length != 2) p.owned = new bool[2];
            if (p.equipped == null || p.equipped.Length != 2) p.equipped = new int[2];
            p.coins = Mathf.Clamp(p.coins, 0, 1000000);
            var learning=LearningProgress.Current??LearningProgress.Load();
            for (int i = 0; i < 2; i++){int e=p.equipped[i];p.equipped[i]=e==1&&p.owned[i]||e>=2&&e<=3&&learning.rewardOwned[(e-2)*2+i]?e:0;}
            if (p.crop < 0 || p.crop > 5 || (p.quantity != 3 && p.quantity != 6))
            { p.accepted = false; p.NewOffer(); }
            // Reprice existing orders without losing an accepted request or other progress.
            p.reward = p.quantity;
            if (float.IsNaN(p.offerDays) || float.IsInfinity(p.offerDays)) p.offerDays = 0;
            p.offerDays = Mathf.Clamp(p.offerDays, 0, 1);
            return p;
        }

        public void NewOffer()
        {
            // Rotate away from the previous product, while keeping every crop reachable.
            var learning=LearningProgress.Current??LearningProgress.Load();var possible=new System.Collections.Generic.List<int>();
            for(int i=0;i<6;i++)if(i!=crop&&learning.CropUnlocked(i))possible.Add(i);
            crop=possible[UnityEngine.Random.Range(0,possible.Count)];
            quantity = UnityEngine.Random.Range(0, 2) == 0 ? 3 : 6;
            reward = quantity; accepted = false; offerDays = 0;
        }
        public void Store(bool flush = true)
        { PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(this)); if (flush) PlayerPrefs.Save(); }
    }
}
