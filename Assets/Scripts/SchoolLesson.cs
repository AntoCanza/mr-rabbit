using System;

namespace MrRabbit
{
    // No network, no negative answers, and no dependence on the handwritten answer.
    public sealed class SchoolLesson
    {
        public enum Subject { Vowels, Addition }
        public Subject Mode { get; private set; }
        public string Question { get; private set; }
        public string Answer { get; private set; }
        public string CompleteWord { get; private set; }
        public int Left { get; private set; }
        public int Right { get; private set; }
        private string previous;
        private readonly Random random;
        private static readonly string[] Words={"CASA","SOLE","LUNA","MARE","MELA","PERA","PANE","MANO","NASO","RANA","GATTO","PESCE","FIORE","TRENO","UVA","LUPO","MURO","NIDO","VISO","DITO","APE","ERBA","ORSO","ISOLA"};
        public static System.Collections.Generic.IReadOnlyList<string> Vocabulary=>Words;
        public SchoolLesson(int seed){random=new Random(seed);}
        public void Next(Subject subject)
        {
            if(subject!=Subject.Vowels&&subject!=Subject.Addition)throw new ArgumentOutOfRangeException(nameof(subject));
            Mode=subject;
            for(int attempt=0;attempt<30;attempt++)
            {
                CompleteWord=null;
                if(subject==Subject.Vowels)
                {
                    string word=Words[random.Next(Words.Length)];int index;
                    do{index=random.Next(word.Length);}while("AEIOU".IndexOf(word[index])<0);
                    CompleteWord=word;Answer=word[index].ToString();
                    Question=word.Substring(0,index)+"_"+word.Substring(index+1);
                }
                else
                {
                    Left=random.Next(11);Right=random.Next(11-Left);
                    Answer=(Left+Right).ToString();
                    Question=Left+" + "+Right+" = ?";
                }
                if(Question!=previous)break;
            }
            previous=Question;
        }
        public bool IsCorrect(string answer)=>string.Equals(Answer,answer,StringComparison.OrdinalIgnoreCase);
    }
}
