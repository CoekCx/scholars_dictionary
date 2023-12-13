using System.Collections.Generic;

namespace Scholars_Dictionary.Models
{
    public class WordDefinition
    {
        public string Word { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public List<string> RelatedWords { get; set; }
    }
}
