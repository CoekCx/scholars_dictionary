using System.Collections.Generic;
using Scholars_Dictionary.Enums;
using Scholars_Dictionary.Services;

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

        public bool TryDefineSelf()
        {
            if (IsDefined())
            {
                return true;
            }
            if(string.IsNullOrEmpty(Word))
            {
                return false;
            }

            var wordDefinition = DefinitionService.GetDefinition(Word);
            if(!wordDefinition.IsDefined())
            {
                return false;
            }

            Definitions.AddRange(wordDefinition.Definitions);
            Types.AddRange(wordDefinition.Types);
            RelatedWords.AddRange(wordDefinition.RelatedWords);
            return true;
        }
    }
}
