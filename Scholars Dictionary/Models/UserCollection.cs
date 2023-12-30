using Scholars_Dictionary.Enums;
using System.Collections.Generic;

namespace Scholars_Dictionary.Models
{
    public class UserCollection
    {
        public string Name { get; set; }
        public Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>> Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>()
        {
            {SupportedLanguages.ENGLISH, new Dictionary<string, WordDefinition>()},
            {SupportedLanguages.RUSSIAN, new Dictionary<string, WordDefinition>()},
            {SupportedLanguages.SPANISH, new Dictionary<string, WordDefinition>()}
        };

        public void AddWord(string word)
        {
            if (Collection[SupportedLanguages.ENGLISH].ContainsKey(word) || !WordCollection.Collection[SupportedLanguages.ENGLISH].ContainsKey(word))
            {
                return;
            }

            Collection[SupportedLanguages.ENGLISH].Add(word, WordCollection.Collection[SupportedLanguages.ENGLISH][word]);
            Collection[SupportedLanguages.RUSSIAN].Add(word, WordCollection.Collection[SupportedLanguages.RUSSIAN][word]);
            Collection[SupportedLanguages.SPANISH].Add(word, WordCollection.Collection[SupportedLanguages.SPANISH][word]);
        }

        public void RemoveWord(string word)
        {
            if (!Collection[SupportedLanguages.ENGLISH].ContainsKey(word))
            {
                return;
            }

            Collection[SupportedLanguages.ENGLISH].Remove(word);
            Collection[SupportedLanguages.RUSSIAN].Remove(word);
            Collection[SupportedLanguages.SPANISH].Remove(word);
        }
    }
}
