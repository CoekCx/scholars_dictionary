using Newtonsoft.Json;
using Scholars_Dictionary.Enums;
using Scholars_Dictionary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Scholars_Dictionary.Constants;

namespace Scholars_Dictionary.Services
{
    public static class WordCollectionService
    {
        private static string wordCollectionFileName = "wordCollection.json";
        private static string wordCollectionFilePath = DataConstants.GetDataFilePath(wordCollectionFileName);

        /// <summary>
        /// Adds a word with its definition to the word collection for a specified language.
        /// </summary>
        public static void AddWord(SupportedLanguages language, string word, WordDefinition definition)
        {
            if (!WordCollection.Collection.ContainsKey(language))
            {
                WordCollection.Collection[language] = new Dictionary<string, WordDefinition>();
            }

            WordCollection.Collection[language][word] = definition;
            LoggingService.Info($"Successfully added the word \"{word}\" in {language.GetStringValue()}");
            SaveCollection();
        }

        /// <summary>
        /// Removes a word from the word collection for all supported languages.
        /// </summary>
        public static void RemoveWord(string word)
        {
            WordCollection.Collection[SupportedLanguages.ENGLISH].Remove(word);
            WordCollection.Collection[SupportedLanguages.RUSSIAN].Remove(word);
            WordCollection.Collection[SupportedLanguages.SPANISH].Remove(word);
            LoggingService.Info($"Successfully removed the word \"{word}\"");
            SaveCollection();
        }

        /// <summary>
        /// Loads the word collection from a JSON file, initializing an empty collection if the file doesn't exist.
        /// </summary>
        public static void LoadCollection()
        {
            try
            {
                if (File.Exists(wordCollectionFilePath))
                {
                    using (StreamReader r = new StreamReader(wordCollectionFilePath))
                    {
                        string json = r.ReadToEnd();
                        WordCollection.Collection = JsonConvert.DeserializeObject<Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>>(json)
                                     ?? new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>()
                                     {
                                         [SupportedLanguages.ENGLISH] = new Dictionary<string, WordDefinition>(),
                                         [SupportedLanguages.RUSSIAN] = new Dictionary<string, WordDefinition>(),
                                         [SupportedLanguages.SPANISH] = new Dictionary<string, WordDefinition>()
                                     };
                    }
                    var wordCount = WordCollection.Collection[SupportedLanguages.ENGLISH].Count;
                    LoggingService.Info($"Successfully loaded word collection with {wordCount} words");
                }
                else
                {
                    WordCollection.Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("An error occurred while loading the word collection", ex);
            }
        }

        /// <summary>
        /// Saves the current word collection to a JSON file.
        /// </summary>
        public static void SaveCollection()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(wordCollectionFilePath))
                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, WordCollection.Collection);
                }
                LoggingService.Info($"Saved word collection");
            }
            catch (Exception ex)
            {
                LoggingService.Error("An error occurred while saving the word collection", ex);
            }
        }

        /// <summary>
        /// Resets the word collection by clearing all entries.
        /// </summary>
        public static void ResetCollection()
        {
            LoggingService.Info("Resetting the word collection");
            WordCollection.Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
            SaveCollection();
        }
    }
}
