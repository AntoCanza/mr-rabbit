using System;
using System.Collections.Generic;
using System.Linq;
using MrRabbit;
using UnityEngine;
public static class VerifyFreeWriting
{
    static List<Vector2> Line(params float[] xy){var p=new List<Vector2>();for(int i=0;i<xy.Length;i+=2)p.Add(new Vector2(xy[i],xy[i+1]));return p;}
    public static string Run()
    {
        // Separately drawn examples: uneven baselines, open loops, detached bars and soft corners.
        var cases=new List<(char,List<List<Vector2>>)>{
            ('A',new List<List<Vector2>>{Line(.2f,.13f,.44f,.84f),Line(.44f,.84f,.83f,.18f),Line(.27f,.43f,.66f,.47f)}),
            ('E',new List<List<Vector2>>{Line(.23f,.12f,.20f,.89f),Line(.21f,.87f,.78f,.83f),Line(.23f,.49f,.68f,.51f),Line(.23f,.16f,.82f,.14f)}),
            ('I',new List<List<Vector2>>{Line(.44f,.87f,.47f,.7f,.43f,.52f,.45f,.32f,.46f,.13f)}),
            ('O',new List<List<Vector2>>{Line(.66f,.87f,.39f,.89f,.20f,.73f,.17f,.46f,.24f,.20f,.48f,.12f,.75f,.24f,.83f,.51f,.78f,.75f,.69f,.82f)}),
            ('U',new List<List<Vector2>>{Line(.21f,.87f,.23f,.56f,.25f,.26f,.35f,.15f,.54f,.11f,.72f,.27f,.79f,.56f,.78f,.86f)}),
            ('0',new List<List<Vector2>>{Line(.57f,.88f,.35f,.85f,.22f,.63f,.24f,.32f,.37f,.15f,.58f,.11f,.75f,.30f,.78f,.59f,.69f,.80f,.56f,.86f)}),
            ('1',new List<List<Vector2>>{Line(.3f,.69f,.43f,.81f,.53f,.91f,.55f,.61f,.54f,.29f,.54f,.12f)}),
            ('2',new List<List<Vector2>>{Line(.19f,.75f,.29f,.86f,.5f,.91f,.71f,.84f,.8f,.71f,.72f,.53f,.52f,.36f,.25f,.13f,.82f,.16f)}),
            ('3',new List<List<Vector2>>{Line(.23f,.81f,.41f,.9f,.65f,.87f,.76f,.72f,.69f,.58f,.48f,.5f,.68f,.44f,.81f,.28f,.7f,.13f,.44f,.10f,.2f,.2f)}),
            ('4',new List<List<Vector2>>{Line(.27f,.85f,.23f,.46f,.80f,.43f),Line(.68f,.88f,.70f,.12f)}),
            ('5',new List<List<Vector2>>{Line(.25f,.85f,.79f,.88f),Line(.26f,.84f,.25f,.54f,.47f,.56f,.71f,.48f,.79f,.29f,.66f,.13f,.43f,.1f,.22f,.21f)}),
            ('6',new List<List<Vector2>>{Line(.74f,.88f,.55f,.9f,.36f,.76f,.23f,.52f,.21f,.29f,.35f,.13f,.56f,.11f,.73f,.25f,.7f,.45f,.50f,.53f,.29f,.43f)}),
            ('7',new List<List<Vector2>>{Line(.22f,.88f,.8f,.87f,.62f,.58f,.43f,.15f),Line(.31f,.51f,.72f,.54f)}),
            ('8',new List<List<Vector2>>{Line(.5f,.5f,.28f,.7f,.36f,.86f,.56f,.9f,.73f,.76f,.67f,.58f,.5f,.5f,.25f,.28f,.34f,.13f,.55f,.09f,.76f,.23f,.68f,.4f,.5f,.5f)}),
            ('9',new List<List<Vector2>>{Line(.73f,.63f,.59f,.87f,.34f,.86f,.21f,.68f,.36f,.49f,.58f,.51f,.73f,.66f),Line(.73f,.82f,.74f,.52f,.64f,.27f,.44f,.13f)})
        };
        var failed=new List<string>();
        foreach(var item in cases)
        {
            var found=HandwritingRecognizer.Recognize(item.Item2,char.IsLetter(item.Item1),out _);
            if(found!=item.Item1)failed.Add(item.Item1+" -> "+(found=='\0'?"?":found.ToString()));
        }
        if(failed.Count>0)throw new Exception("Independent freehand examples: "+string.Join(", ",failed));
        return "15 independently drawn vowel/digit examples recognized, including open loops, uneven bars and soft corners.";
    }
}
