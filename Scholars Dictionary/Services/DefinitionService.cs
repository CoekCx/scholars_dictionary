using Scholars_Dictionary.Models;
using System.Collections.Generic;
using System.Text;
using WordNet.Common;
using Scholars_Dictionary.Enums;

namespace Scholars_Dictionary.Services
{
    public static class DefinitionService
    {
        public static WordDefinition GetDefinition(string word)
        {
            Dictionary<string, List<Definition>> definitions = DictionaryHelper.GetDefinition(word);
            WordDefinition wordDefinition = FormatResults(word, definitions);
			wordDefinition.CleanUpName();
			return wordDefinition;
        }

		private static WordDefinition FormatResults(string originalWord, Dictionary<string, List<Definition>> results)
		{
			WordDefinition wordDefinition = new WordDefinition();
			wordDefinition.Word = originalWord;
			StringBuilder sb = new StringBuilder();

			if (results.Count > 0)
			{
				// Prep for sorting, build and rendering
				List<string> words = new List<string>();
				Dictionary<string, List<Definition>> defSets = new Dictionary<string, List<Definition>>();

				// Sort results by part of speech
				foreach (string key in results.Keys)
				{
					foreach (Definition def in results[key])
					{
						string pos = def.DisplayPartOfSpeech;
						if (!defSets.ContainsKey(pos))
							defSets.Add(pos, new List<Definition>());

						defSets[pos].Add(def);
						foreach (string word in def.Words)
						{
							if (!words.Contains(word))
                            {
								words.Add(word);
								wordDefinition.RelatedWords.Add(word);
                            }
						}
					}
				}

				// Build markup for browser control
				foreach (string key in defSets.Keys)
				{
					// StringBuilder defText = new StringBuilder("<ul>");
					StringBuilder defText = new StringBuilder("");
					foreach (Definition def in defSets[key])
					{
						string formattedDefinition = def.DefinitionText;
						if (!string.IsNullOrEmpty(formattedDefinition))
						{
							defText.AppendLine(string.Format(" • {0}", formattedDefinition));
						}
					}
					// defText.AppendLine("</ul>");
					switch(key)
                    {
						case "Noun":
							wordDefinition.Types.Add(WordType.NOUN);
							break;
						case "Verb":
							wordDefinition.Types.Add(WordType.VERB);
							break;
						case "Adj":
							wordDefinition.Types.Add(WordType.ADJECTIVE);
							break;
						case "Adv":
							wordDefinition.Types.Add(WordType.ADVERB);
							break;
                    }
					wordDefinition.Definitions.Add($"{defText}");
					sb.AppendFormat(string.Format("{0} <sup>({1})</sup>", originalWord, key), defText.ToString());
				}

				sb.Append(string.Join(", ", words.ToArray()));
			}
			else
			{
				// Build no results markup
				sb.AppendFormat("<h1>No match was found for \"{0}\".</h1><br />Try your search on <a href=\"http://wordnet.princeton.edu/perl/webwn?s={0}\" target=\"_blank\">WordNet Online</a>", originalWord);
			}

			return wordDefinition;
		}
	}
}