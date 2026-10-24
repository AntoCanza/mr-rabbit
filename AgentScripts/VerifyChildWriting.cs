using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using MrRabbit;

public static class VerifyChildWriting
{
    public static string Run(bool strict=false)
    {
        int correct=0,wrong=0,unknown=0;var failures=new List<string>();var watch=Stopwatch.StartNew();
        var random=new System.Random(7519);
        foreach(var glyph in HandwritingRecognizer.Templates)
        for(int trial=0;trial<12;trial++)
        {
            float angle=(trial%3-1)*11*Mathf.Deg2Rad,sx=.55f+(trial%4)*.23f,sy=.78f,slant=(trial%3-1)*.16f;
            var strokes=new List<List<Vector2>>();
            foreach(var line in glyph.Strokes)
            {
                var points=new List<Vector2>();
                for(int i=1;i<line.Length;i++)
                {
                    int pieces=Mathf.Max(1,Mathf.CeilToInt(Vector2.Distance(line[i-1],line[i])*24));
                    for(int j=0;j<pieces;j++)
                    {
                        var p=Vector2.Lerp(line[i-1],line[i],j/(float)pieces)-Vector2.one*.5f;
                        p=new Vector2(p.x*sx+p.y*slant,p.y*sy);
                        p=new Vector2(p.x*Mathf.Cos(angle)-p.y*Mathf.Sin(angle),p.x*Mathf.Sin(angle)+p.y*Mathf.Cos(angle));
                        p+=Vector2.one*.48f+new Vector2((float)random.NextDouble()-.5f,(float)random.NextDouble()-.5f)*.026f;
                        points.Add(p);
                    }
                }
                if(points.Count==0)points.Add(line[0]);
                if(trial%2==1)points.Reverse();
                // Finger lifts split a line without changing its geometry.
                if(trial%4==3&&points.Count>12){int middle=points.Count/2;strokes.Add(points.Take(middle+1).ToList());strokes.Add(points.Skip(middle).ToList());}
                else strokes.Add(points);
            }
            if(trial%2==1)strokes.Reverse();
            var found=HandwritingRecognizer.Recognize(strokes,char.IsLetter(glyph.Symbol),out _);
            if(found==glyph.Symbol)correct++;else {if(found=='\0')unknown++;else wrong++;if(failures.Count<45)failures.Add(glyph.Symbol+"/"+trial+"="+(found=='\0'?"?":found.ToString()));}
        }
        var blank=new List<List<Vector2>>();
        var dot=new List<List<Vector2>>{new List<Vector2>{new Vector2(.4f,.4f),new Vector2(.405f,.41f)}};
        var bar=new List<List<Vector2>>{new List<Vector2>{new Vector2(.1f,.5f),new Vector2(.9f,.5f)}};
        var scribble=new List<List<Vector2>>{Enumerable.Range(0,120).Select(i=>new Vector2(i%2==0?.1f:.9f,.1f+i/150f*.8f)).ToList()};
        foreach(bool vowel in new[]{false,true})foreach(var bad in new[]{blank,dot,bar,scribble})
            if(HandwritingRecognizer.Recognize(bad,vowel,out _)!='\0')throw new Exception("Blank/dot/bar/scribble accepted");
        if(strict&&(correct<310||wrong>1))throw new Exception("Handwriting regression: "+correct+" correct, "+wrong+" wrong, "+unknown+" uncertain");
        return correct+" correct / "+(correct+wrong+unknown)+"; "+wrong+" wrong; "+unknown+" uncertain; "+watch.ElapsedMilliseconds+" ms; "+string.Join(", ",failures);
    }
}
