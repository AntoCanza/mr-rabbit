using System;
using System.Collections.Generic;
using UnityEngine;

namespace MrRabbit
{
    // Offline shape matching for the small teaching alphabet. Stroke order/direction do not matter.
    // Recognition is independent of the exercise's expected answer; uncertain ink is never rewarded.
    public static class HandwritingRecognizer
    {
        public sealed class Glyph
        {
            public readonly char Symbol;
            public readonly Vector2[][] Strokes;
            internal readonly Vector2[] Cloud;
            internal readonly float Length;
            internal readonly float Aspect;
            public Glyph(char symbol,params Vector2[][] strokes)
            {Symbol=symbol;Strokes=strokes;Cloud=Normalize(strokes,out Length,out Aspect);}
        }
        static Vector2[] P(params float[] xy)
        {var p=new Vector2[xy.Length/2];for(int i=0;i<p.Length;i++)p[i]=new Vector2(xy[i*2],xy[i*2+1]);return p;}
        static Vector2[] Arc(float x,float y,float rx,float ry,float start,float end)
        {var p=new Vector2[33];for(int i=0;i<p.Length;i++){float a=Mathf.Lerp(start,end,i/32f)*Mathf.Deg2Rad;p[i]=new Vector2(x+Mathf.Cos(a)*rx,y+Mathf.Sin(a)*ry);}return p;}
        public static readonly Glyph[] Templates={
            new Glyph('A',P(.1f,.1f,.5f,.9f,.9f,.1f),P(.28f,.42f,.72f,.42f)),
            new Glyph('A',Arc(.45f,.42f,.28f,.3f,0,360),P(.73f,.72f,.73f,.12f,.85f,.12f)),
            new Glyph('E',P(.85f,.9f,.15f,.9f,.15f,.1f,.85f,.1f),P(.15f,.5f,.7f,.5f)),
            new Glyph('E',P(.15f,.50f,.80f,.50f,.78f,.68f,.65f,.78f,.4f,.78f,.2f,.65f,.13f,.43f,.22f,.22f,.43f,.14f,.67f,.15f,.82f,.28f)),
            new Glyph('I',P(.5f,.9f,.5f,.1f)),
            new Glyph('I',P(.2f,.9f,.8f,.9f),P(.5f,.9f,.5f,.1f),P(.2f,.1f,.8f,.1f)),
            new Glyph('I',P(.5f,.64f,.5f,.1f),P(.49f,.88f,.51f,.89f)),
            new Glyph('O',Arc(.5f,.5f,.35f,.42f,0,360)),
            new Glyph('U',P(.15f,.9f,.15f,.35f,.19f,.18f,.32f,.10f,.55f,.10f,.74f,.18f,.83f,.35f,.83f,.9f)),
            new Glyph('U',P(.15f,.8f,.15f,.32f,.2f,.16f,.4f,.1f,.66f,.19f,.78f,.4f,.78f,.8f,.78f,.12f)),
            new Glyph('0',Arc(.5f,.5f,.30f,.43f,0,360)),
            new Glyph('1',P(.5f,.9f,.5f,.1f)),
            new Glyph('1',P(.25f,.7f,.5f,.9f,.5f,.1f),P(.25f,.1f,.75f,.1f)),
            new Glyph('1',P(.25f,.7f,.5f,.9f,.5f,.1f)),
            new Glyph('2',P(.15f,.72f,.22f,.88f,.45f,.94f,.70f,.87f,.82f,.7f,.73f,.51f,.15f,.1f,.85f,.1f)),
            new Glyph('3',P(.16f,.83f,.4f,.94f,.68f,.9f,.82f,.74f,.70f,.57f,.45f,.5f,.70f,.45f,.85f,.27f,.72f,.1f,.42f,.06f,.15f,.16f)),
            new Glyph('3',P(.15f,.9f,.8f,.9f,.45f,.52f,.7f,.5f,.84f,.3f,.72f,.12f,.4f,.06f,.15f,.17f)),
            new Glyph('4',P(.7f,.92f,.15f,.35f,.9f,.35f),P(.7f,.92f,.7f,.08f)),
            new Glyph('4',P(.2f,.9f,.2f,.4f,.9f,.4f),P(.75f,.9f,.75f,.1f)),
            new Glyph('5',P(.8f,.9f,.2f,.9f,.18f,.52f,.52f,.55f,.77f,.46f,.84f,.28f,.71f,.1f,.4f,.07f,.15f,.18f)),
            new Glyph('6',P(.78f,.88f,.53f,.94f,.3f,.78f,.16f,.46f,.18f,.19f,.38f,.07f,.65f,.1f,.8f,.28f,.71f,.48f,.48f,.55f,.22f,.44f)),
            new Glyph('7',P(.15f,.9f,.85f,.9f,.37f,.1f)),
            new Glyph('7',P(.15f,.9f,.85f,.9f,.37f,.1f),P(.27f,.5f,.76f,.5f)),
            new Glyph('8',Arc(.5f,.72f,.26f,.22f,0,360),Arc(.5f,.28f,.31f,.24f,0,360)),
            new Glyph('8',P(.5f,.5f,.25f,.7f,.3f,.88f,.5f,.95f,.72f,.88f,.77f,.7f,.5f,.5f,.2f,.28f,.28f,.08f,.5f,.03f,.75f,.1f,.8f,.28f,.5f,.5f)),
            new Glyph('9',Arc(.48f,.7f,.29f,.24f,0,360),P(.77f,.70f,.75f,.4f,.6f,.12f,.3f,.05f)),
            new Glyph('9',Arc(.48f,.7f,.29f,.24f,0,360),P(.77f,.85f,.77f,.08f))
        };
        public static char Recognize(IReadOnlyList<List<Vector2>> ink,bool vowels,out float confidence)
            =>RecognizeCore(ink,vowels,false,out confidence);
        public static char RecognizeAlphabet(IReadOnlyList<List<Vector2>> ink,out float confidence)
            =>RecognizeCore(ink,true,true,out confidence);
        public static readonly Glyph[] Consonants={
            new Glyph('B',P(.18f,.1f,.18f,.9f,.55f,.9f,.76f,.79f,.7f,.61f,.2f,.51f,.6f,.51f,.82f,.37f,.73f,.15f,.18f,.1f)),
            new Glyph('C',Arc(.52f,.5f,.37f,.42f,45,315)),
            new Glyph('D',P(.18f,.1f,.18f,.9f,.48f,.9f,.75f,.76f,.84f,.51f,.77f,.25f,.5f,.1f,.18f,.1f)),
            new Glyph('F',P(.18f,.1f,.18f,.9f,.85f,.9f),P(.18f,.52f,.72f,.52f)),
            new Glyph('G',Arc(.5f,.5f,.37f,.42f,45,315),P(.77f,.2f,.82f,.49f,.53f,.49f)),
            new Glyph('H',P(.18f,.1f,.18f,.9f),P(.82f,.1f,.82f,.9f),P(.18f,.5f,.82f,.5f)),
            new Glyph('L',P(.18f,.9f,.18f,.1f,.85f,.1f)),
            new Glyph('M',P(.1f,.1f,.1f,.9f,.5f,.47f,.9f,.9f,.9f,.1f)),
            new Glyph('N',P(.18f,.1f,.18f,.9f,.82f,.1f,.82f,.9f)),
            new Glyph('P',P(.18f,.1f,.18f,.9f,.58f,.9f,.81f,.76f,.75f,.56f,.55f,.49f,.18f,.49f)),
            new Glyph('Q',Arc(.5f,.52f,.33f,.4f,0,360),P(.55f,.3f,.9f,.05f)),
            new Glyph('R',P(.18f,.1f,.18f,.9f,.58f,.9f,.81f,.76f,.75f,.56f,.55f,.49f,.18f,.49f),P(.5f,.49f,.88f,.1f)),
            new Glyph('S',P(.82f,.82f,.6f,.93f,.32f,.9f,.16f,.73f,.24f,.56f,.69f,.41f,.83f,.24f,.68f,.09f,.38f,.07f,.15f,.19f)),
            new Glyph('T',P(.1f,.9f,.9f,.9f),P(.5f,.9f,.5f,.1f)),
            new Glyph('V',P(.12f,.9f,.5f,.1f,.88f,.9f)),
            new Glyph('Z',P(.12f,.9f,.88f,.9f,.12f,.1f,.88f,.1f))
        };
        static char RecognizeCore(IReadOnlyList<List<Vector2>> ink,bool vowels,bool alphabet,out float confidence)
        {
            var strokes=new Vector2[ink.Count][];for(int i=0;i<ink.Count;i++)strokes[i]=ink[i].ToArray();
            var cloud=Normalize(strokes,out float length,out float aspect);confidence=0;
            if(cloud.Length<8||length<.3f||ink.Count>12)return '\0';
            var variants=new List<Vector2[]> {cloud};var lengths=new List<float>{length};
            var aspects=new List<float>{aspect};
            // Small alignment corrections tolerate a tilted/slanted hand without turning 6 into 9.
            foreach(float angle in new[]{-12f,0f,12f})foreach(float shear in new[]{-.18f,0f,.18f})
            {
                if(angle==0&&shear==0)continue;
                variants.Add(Normalize(strokes,out float variantLength,out float variantAspect,angle,shear));lengths.Add(variantLength);aspects.Add(variantAspect);
            }
            float best=float.MaxValue,second=float.MaxValue;char result='\0';
            var scores=new Dictionary<char,float>();
            foreach(var t in alphabet?AllLetters():Templates)
            {
                if(vowels!=char.IsLetter(t.Symbol))continue;
                float score=float.MaxValue;
                for(int v=0;v<variants.Count;v++)
                {
                    if(variants[v].Length<8)continue;
                    float ratio=lengths[v]/Mathf.Max(.001f,t.Length);if(ratio<.5f||ratio>2.1f)continue;
                    float candidate=(Distance(variants[v],t.Cloud)+Distance(t.Cloud,variants[v]))*.5f;
                    candidate+=Mathf.Max(0,Mathf.Abs(Mathf.Log(ratio))-.25f)*.04f;
                    candidate+=Mathf.Max(0,Mathf.Abs(Mathf.Log(aspects[v]/t.Aspect))-.4f)*.025f;
                    score=Mathf.Min(score,candidate);
                }
                if(!scores.TryGetValue(t.Symbol,out float old)||score<old)scores[t.Symbol]=score;
            }
            foreach(var pair in scores)
            {if(pair.Value<best){second=best;best=pair.Value;result=pair.Key;}else if(pair.Value<second)second=pair.Value;}
            confidence=Mathf.Clamp01(1-best/.16f);
            return best<=.115f&&second-best>=.006f?result:'\0';
        }
        static IEnumerable<Glyph> AllLetters(){foreach(var t in Templates)if(char.IsLetter(t.Symbol))yield return t;foreach(var t in Consonants)yield return t;}
        static float Distance(Vector2[] a,Vector2[] b)
        {
            float sum=0,worst=0;foreach(var p in a){float closest=float.MaxValue;foreach(var q in b)closest=Mathf.Min(closest,(p-q).sqrMagnitude);float d=Mathf.Sqrt(closest);sum+=d;worst=Mathf.Max(worst,d);}return sum/a.Length+worst*.10f;
        }
        static Vector2[] Normalize(Vector2[][] strokes,out float length,out float aspect,float angle=0,float shear=0)
        {
            length=0;aspect=1;var clean=new List<Vector2[]>();float radians=angle*Mathf.Deg2Rad,c=Mathf.Cos(radians),s=Mathf.Sin(radians);
            Vector2 min=new Vector2(float.MaxValue,float.MaxValue),max=new Vector2(float.MinValue,float.MinValue);
            foreach(var stroke in strokes)
            {
                if(stroke.Length==0)continue;var line=new List<Vector2>();
                for(int i=0;i<stroke.Length;i++)
                {
                    var p=stroke[i];if(!float.IsFinite(p.x)||!float.IsFinite(p.y))continue;
                    if(stroke.Length>4&&i>0&&i<stroke.Length-1)p=stroke[i-1]*.2f+p*.6f+stroke[i+1]*.2f;
                    p.x+=p.y*shear;p=new Vector2(p.x*c-p.y*s,p.x*s+p.y*c);
                    if(line.Count>0&&(line[line.Count-1]-p).sqrMagnitude<.000001f)continue;
                    line.Add(p);min=Vector2.Min(min,p);max=Vector2.Max(max,p);
                }
                if(line.Count>0)clean.Add(line.ToArray());
            }
            float size=Mathf.Max(max.x-min.x,max.y-min.y);if(size<.035f||float.IsInfinity(size))return Array.Empty<Vector2>();
            Vector2 center=(min+max)*.5f,scale=max-min;
            aspect=Mathf.Clamp(scale.x/Mathf.Max(.001f,scale.y),.08f,12);
            // A line remains a line even with small finger wobble; a narrow hooked 1
            // still keeps its hook instead of abruptly becoming a different shape.
            float xx=0,yy=0,xy=0;int samples=0;Vector2 mean=Vector2.zero;
            foreach(var stroke in clean)foreach(var p in stroke){mean+=p;samples++;}mean/=Mathf.Max(1,samples);
            foreach(var stroke in clean)foreach(var p in stroke){var d=p-mean;xx+=d.x*d.x;yy+=d.y*d.y;xy+=d.x*d.y;}
            float spread=Mathf.Sqrt((xx-yy)*(xx-yy)+4*xy*xy),major=xx+yy+spread,minor=xx+yy-spread;
            bool lineLike=minor<major*.012f;
            if(lineLike||Mathf.Min(scale.x,scale.y)<.001f)scale=Vector2.one*size;
            var pathLengths=new List<float>();
            foreach(var stroke in clean)
            {
                for(int i=0;i<stroke.Length;i++)stroke[i]=new Vector2((stroke[i].x-center.x)/scale.x,(stroke[i].y-center.y)/scale.y);
                float path=0;
                for(int i=1;i<stroke.Length;i++)path+=Vector2.Distance(stroke[i-1],stroke[i]);
                pathLengths.Add(path);length+=path;
            }
            if(length<.001f||length>8)return Array.Empty<Vector2>();
            var points=new List<Vector2>(96);
            for(int line=0;line<clean.Count;line++)
            {
                var stroke=clean[line];float path=pathLengths[line];
                if(path<.001f){points.Add(stroke[0]);continue;}
                int count=Mathf.Max(2,Mathf.RoundToInt(64*path/length));float distance=0;int segment=1;
                for(int k=0;k<count;k++)
                {
                    float target=k*path/(count-1);
                    while(segment<stroke.Length-1&&distance+Vector2.Distance(stroke[segment-1],stroke[segment])<target)
                    {distance+=Vector2.Distance(stroke[segment-1],stroke[segment]);segment++;}
                    float part=Vector2.Distance(stroke[segment-1],stroke[segment]);
                    points.Add(Vector2.Lerp(stroke[segment-1],stroke[segment],part>.000001f?(target-distance)/part:0));
                }
            }
            return points.ToArray();
        }
    }
}
