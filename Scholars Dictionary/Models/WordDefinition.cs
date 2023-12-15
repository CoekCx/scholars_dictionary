using System.Collections.Generic;
using Scholars_Dictionary.Enums;

namespace Scholars_Dictionary.Models
{
    public class WordDefinition
    {
        public string Word { get; set; }
        public List<string> Definitions { get; set; } = new List<string>();
        public List<WordType> Types { get; set; } = new List<WordType>();
        public List<string> RelatedWords { get; set; } = new List<string>();

        public void CleanUpName()
        {
            if (Word.Contains("_"))
            {
                Word = Word.Replace('_', ' ');
            }
        }

        public bool IsDefined()
        {
            return Definitions.Count > 0;
        }
    }
}
