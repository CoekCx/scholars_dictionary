using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scholars_Dictionary.Constants;
using Scholars_Dictionary.Enums;

namespace Scholars_Dictionary.Services
{
    /// <summary>
    /// The RandomWordPicker class is a utility class designed to facilitate the random selection of words based on different word types (Noun, Verb, Adverb, Adjective).
    /// It employs a method called PickRandomWord, which allows users to specify a desired word type or, if left unspecified, randomly selects one.
    /// The class reads word data from external files, skips irrelevant lines, and picks a random word from the remaining lines.
    /// </summary>
    public static class RandomWordPicker
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// If wordType is not specified, it randomly selects one of the available types(Noun, Verb, Adverb, Adjective).
        /// Reads word data from external files based on the specified or randomly chosen type.
        /// Picks a random line.
        /// Extracts the word (index 4) from the line, handling cases where the element contains an underscore.
        /// If the selected word contains an underscore, the method recursively calls itself to pick another random word.
        /// </summary>
        /// <param name="wordType">(optional). Specifies the type of word to be picked.</param>
        /// <returns>The randomly selected word.</returns>
        public static string PickRandomWord(WordTypes? wordType = null)
        {
            if (wordType == null)
            {
                // Assign a random value to wordType
                var values = Enum.GetValues(typeof(WordTypes));
                var random = new Random();
                wordType = (WordTypes)values.GetValue(random.Next(values.Length));
            }

            List<string> lines = new List<string>();
            switch(wordType)
            {
                case WordTypes.NOUN:
                    var x = DataConstants.GetDataFilePath("data.noun");
                    lines = ReadLinesFromFile(DataConstants.GetDataFilePath("data.noun"));
                    break;
                case WordTypes.VERB:
                    lines = ReadLinesFromFile(DataConstants.GetDataFilePath("data.verb"));
                    break;
                case WordTypes.ADVERB:
                    lines = ReadLinesFromFile(DataConstants.GetDataFilePath("data.adv"));
                    break;
                case WordTypes.ADJECTIVE:
                    lines = ReadLinesFromFile(DataConstants.GetDataFilePath("data.adj"));
                    break;
            }

            // Skip the first 29 lines
            List<string> remainingLines = lines.Skip(29).ToList();

            // Pick a random line
            string randomLine = remainingLines[random.Next(remainingLines.Count)];

            // Split the line by space and get the 5th element (index 4)
            string[] lineParts = randomLine.Split(' ');
            if (lineParts.Length < 5)
            {
                return string.Empty;
            }

            string retVal = lineParts[4];
            if(!retVal.Contains("_"))
            {
                return retVal;
            }

            return PickRandomWord(wordType);
        }

        /// <summary>
        /// Reads the lines from the specified file and returns them as a list. Handles exceptions and prints an error message if encountered during file reading.
        /// </summary>
        /// <param name="filePath">The path to the file to be read.</param>
        /// <returns>A list containing each line of the file.</returns>
        private static List<string> ReadLinesFromFile(string filePath)
        {
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return lines;
        }
    }
}
