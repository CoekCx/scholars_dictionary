using Scholars_Dictionary.Enums;
using System.Collections.Generic;

namespace Scholars_Dictionary.Models
{
    public static class WordCollection
    {
        public static Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>> Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();

        public static bool WordExists(WordDefinition wordDefinition = null, string word = "")
        {
            if (wordDefinition != null)
            {
                return Collection[SupportedLanguages.ENGLISH].ContainsKey(wordDefinition.Word);
            }

            return Collection[SupportedLanguages.ENGLISH].ContainsKey(word);
        }

        public static bool WordExistsInLanguage(SupportedLanguages language, WordDefinition wordDefinition = null, string word = "")
        {
            if (wordDefinition != null)
            {
                return Collection[language].ContainsKey(wordDefinition.Word);
            }

            return Collection[language].ContainsKey(word);
        }
    }
}
