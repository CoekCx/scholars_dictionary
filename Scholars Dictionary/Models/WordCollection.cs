using Scholars_Dictionary.Enums;
using System.Collections.Generic;

namespace Scholars_Dictionary.Models
{
    public static class WordCollection
    {
        public static Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>> Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
    }
}
