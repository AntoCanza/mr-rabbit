using System;
using System.Collections.Generic;

namespace MrRabbit
{
    // A small, explicitly Italian first-primary sequence; no subtraction or curriculum claims.
    public sealed class LearningLesson
    {
        public enum Layout { Word, Letter, Syllable, Picture, Sentence, Count, Sequence, Sum }
        public Layout Kind;
        public string Question,Answer,Word,Voice;
        public string[] Choices,PictureChoices;
        public bool Writing,Alphabet,Vowels;
        public int Left,Right,CorrectIndex;
        public string Signature=>Kind+":"+Question+":"+Word+":"+Answer;
        static readonly string[] Simple={"MELA","PERA","LUNA","CASA","MANO","NASO","RANA","PANE","VISO","DITO","LUPO","MURO","NIDO","SOLE"};
        static readonly string[] Fruit={"MELA","PERA","PANE","UVA","SOLE","TRENO","RANA","LUNA"};
        public static LearningLesson Next(int skill,Random random,HashSet<string> used)
        {
            if(skill<0||skill>=LearningProgress.SkillCount)throw new ArgumentOutOfRangeException(nameof(skill));
            LearningLesson p=null;for(int i=0;i<200;i++){p=Make(skill,random);if(used==null||used.Add(p.Signature))return p;}
            throw new InvalidOperationException("Insufficient distinct questions for this session");
        }
        static LearningLesson Make(int skill,Random r)
        {
            var p=new LearningLesson();
            switch((LearningSkill)skill)
            {
                case LearningSkill.Vowels:
                    p.Kind=Layout.Word;p.Word=SchoolLesson.Vocabulary[r.Next(SchoolLesson.Vocabulary.Count)];int k;
                    do{k=r.Next(p.Word.Length);}while("AEIOU".IndexOf(p.Word[k])<0);
                    p.Answer=p.Word[k].ToString();p.Question=p.Word.Substring(0,k)+"_"+p.Word.Substring(k+1);
                    p.Writing=p.Vowels=true;p.Voice="vowel";break;
                case LearningSkill.Sounds:
                    p.Kind=Layout.Letter;p.Answer="ABCDEFGHILMNOPQRSTUVZ"[r.Next(21)].ToString();p.Question="";p.Voice="letter_"+p.Answer;
                    p.SetChoices(p.Answer,LettersExcept(p.Answer),r);break;
                case LearningSkill.Letters:
                    p.Kind=Layout.Letter;p.Answer="BCDFGHLMNPQRSTVZ"[r.Next(16)].ToString();p.Question=p.Answer;p.Writing=p.Alphabet=true;p.Voice="write_"+p.Answer;break;
                case LearningSkill.Syllables:
                    p.Kind=Layout.Syllable;p.Word=Simple[r.Next(Simple.Length)];p.Answer=p.Word.Substring(0,2);p.Question="__ "+p.Word.Substring(2);p.Voice="syllable";
                    p.SetChoices(p.Answer,new[]{"MA","PE","LU","CA","NA","RA","PA","VI","DI","MU","NI","SO"},r);break;
                case LearningSkill.Words:
                    p.Kind=Layout.Picture;p.Word=Simple[r.Next(Simple.Length)];p.Answer=p.Word;p.Question="";p.Voice="word";p.SetChoices(p.Answer,Simple,r);break;
                case LearningSkill.Sentences:
                    p.Kind=Layout.Sentence;p.Answer=Fruit[r.Next(Fruit.Length)];p.Question="TOCCA "+(p.Answer=="PANE"||p.Answer=="SOLE"||p.Answer=="TRENO"?"IL ":"LA ")+p.Answer;
                    if(p.Answer=="UVA")p.Question="TOCCA L’UVA";p.Voice="sentence";p.SetChoices(p.Answer,Fruit,r);p.PictureChoices=p.Choices;break;
                case LearningSkill.Count10:
                    p.Kind=Layout.Count;p.Left=r.Next(11);p.Answer=p.Left.ToString();p.Question="";p.Writing=true;p.Voice="count";break;
                case LearningSkill.Numbers20:
                    p.Kind=Layout.Sequence;p.Left=r.Next(19);p.Answer=(p.Left+2).ToString();p.Question=p.Left+"    "+(p.Left+1)+"    ?";p.Writing=true;p.Voice="sequence";break;
                default:
                    int max=skill==(int)LearningSkill.Add5?5:skill==(int)LearningSkill.Add10?10:20;
                    p.Kind=Layout.Sum;p.Left=r.Next(max+1);p.Right=r.Next(max-p.Left+1);p.Answer=(p.Left+p.Right).ToString();p.Question=p.Left+" + "+p.Right+" = ?";p.Writing=true;p.Voice="sum";break;
            }
            return p;
        }
        static string[] LettersExcept(string answer)
        {var letters=new string[21];for(int i=0;i<21;i++)letters[i]="ABCDEFGHILMNOPQRSTUVZ"[i].ToString();return letters;}
        void SetChoices(string correct,string[] pool,Random r)
        {
            var choices=new List<string>{correct};while(choices.Count<3){string x=pool[r.Next(pool.Length)];if(!choices.Contains(x))choices.Add(x);}
            for(int i=choices.Count-1;i>0;i--){int j=r.Next(i+1);string x=choices[i];choices[i]=choices[j];choices[j]=x;}
            Choices=choices.ToArray();CorrectIndex=Array.IndexOf(Choices,correct);
        }
    }
}
