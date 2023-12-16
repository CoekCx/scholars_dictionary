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
                                     ?? new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
                    }
                    var wordCount = WordCollection.Collection.Count > 0 ? WordCollection.Collection[SupportedLanguages.ENGLISH].Count : 0;
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

        public static void ResetCollection()
        {
            LoggingService.Info("Resetting the word collection");
            WordCollection.Collection = new Dictionary<SupportedLanguages, Dictionary<string, WordDefinition>>();
            SaveCollection();
        }
    }
}
