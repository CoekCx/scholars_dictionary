using Newtonsoft.Json;
using Scholars_Dictionary.Enums;
using Scholars_Dictionary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using Scholars_Dictionary.Constants;
using System.Linq;

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
            LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Successfully added the word \"{word}\" in {language.GetStringValue()}");
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
            LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Successfully removed the word \"{word}\"");
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
                    LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Successfully loaded word collection with {wordCount} words");

                }
                else
                {
                    WordCollection.Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("[WORD-COLLECTION-SERVICE]: An error occurred while loading the word collection", ex);
            }
        }

        /// <summary>
        /// Performs a check for any incomplete data, patches everything it can and removes anything it can't
        /// </summary>
        public async static void SafeCheckCollection()
        {
            var englishKeys = WordCollection.Collection[SupportedLanguages.ENGLISH].Keys;

            // Path up missing translations
            foreach (var word in englishKeys)
            {
                if (!WordCollection.Collection[SupportedLanguages.RUSSIAN].ContainsKey(word))
                {
                    var wordDefinition = DefinitionService.GetDefinition(word);
                    var translatedWordDefinition = await AzureTranslateAPI.TranslateWordDefinition(wordDefinition, SupportedLanguages.ENGLISH, SupportedLanguages.RUSSIAN);
                    AddWord(SupportedLanguages.RUSSIAN, word, translatedWordDefinition);
                    LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Patched word definition for {word} in {SupportedLanguages.RUSSIAN.GetTypeCode()}");
                }
                if (!WordCollection.Collection[SupportedLanguages.SPANISH].ContainsKey(word))
                {
                    var wordDefinition = DefinitionService.GetDefinition(word);
                    var translatedWordDefinition = await AzureTranslateAPI.TranslateWordDefinition(wordDefinition, SupportedLanguages.ENGLISH, SupportedLanguages.SPANISH);
                    AddWord(SupportedLanguages.SPANISH, word, translatedWordDefinition);
                    LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Patched word definition for {word} in {SupportedLanguages.SPANISH.GetTypeCode()}");
                }
            }

            var russianKeys = WordCollection.Collection[SupportedLanguages.RUSSIAN].Keys;
            var spanishKeys = WordCollection.Collection[SupportedLanguages.SPANISH].Keys;

            foreach (var word in russianKeys.Except(englishKeys).ToList())
            {
                WordCollection.Collection[SupportedLanguages.RUSSIAN].Remove(word);
                LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Removed word definition for {word} in {SupportedLanguages.RUSSIAN.GetTypeCode()}");
            }
            foreach(var word in spanishKeys.Except(englishKeys).ToList())
            {
                WordCollection.Collection[SupportedLanguages.SPANISH].Remove(word);
                LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Removed word definition for {word} in {SupportedLanguages.SPANISH.GetTypeCode()}");
            }

            SaveCollection();
            LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Successfully performed safe check on data");
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
                LoggingService.Info($"[WORD-COLLECTION-SERVICE]: Saved word collection");
            }
            catch (Exception ex)
            {
                LoggingService.Error("[WORD-COLLECTION-SERVICE]: An error occurred while saving the word collection", ex);
            }
        }

        /// <summary>
        /// Resets the word collection by clearing all entries.
        /// </summary>
        public static void ResetCollection()
        {
            LoggingService.Info("[WORD-COLLECTION-SERVICE]: Resetting the word collection");
            WordCollection.Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
            SaveCollection();
        }
    }
}
