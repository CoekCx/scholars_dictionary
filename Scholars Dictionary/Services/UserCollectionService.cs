using Newtonsoft.Json;
using Scholars_Dictionary.Constants;
using Scholars_Dictionary.Enums;
using Scholars_Dictionary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scholars_Dictionary.Services
{
    public static class UserCollectionService
    {
        private static string userCollectionsFileName = "userCollections.json";
        private static string userCollectionsFilePath = DataConstants.GetDataFilePath(userCollectionsFileName);

        public static List<UserCollection> Collections { get; set; } = new List<UserCollection>();

        public static UserCollection GetCollectionByName(string name)
        {
            return Collections.FirstOrDefault(x => x.Name.Equals(name)) ?? new UserCollection();
        }


        /// <summary>
        /// Adds new user collection, but checks for unique name.
        /// </summary>
        public static bool AddCollection(UserCollection newUserCollection)
        {
            if  (Collections.Where(x => x.Name == newUserCollection.Name).Any())
            {
                return false;
            }

            Collections.Add(newUserCollection);
            LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Successfully added the word \"{newUserCollection.Name}\" with {newUserCollection.Collection.Count} words");
            SaveCollection();
            return true;
        }

        /// <summary>
        /// Removes a user collection.
        /// </summary>
        public static void RemoveCollection(UserCollection userCollection = null, string collectionName = "")
        {
            if  (userCollection != null && Collections.Where(x => x.Name.Equals(userCollection.Name)).Any())
            {
                var collectionToRemove = Collections.FirstOrDefault(x => x.Name.Equals(userCollection.Name));
                Collections.Remove(collectionToRemove);
                LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Successfully removed the user collection \"{userCollection.Name}\"");
                SaveCollection();
                return;
            }

            if (!String.IsNullOrEmpty(collectionName) && Collections.Where(x => x.Name.Equals(collectionName)).Any())
            {
                var collectionToRemove = Collections.FirstOrDefault(x => x.Name.Equals(collectionName));
                Collections.Remove(collectionToRemove);
                LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Successfully removed the user collection \"{collectionName}\"");
                SaveCollection();
            }
        }

        /// <summary>
        /// Loads the user collections from a JSON file, initializing an empty collection list if the file doesn't exist.
        /// </summary>
        public static void LoadCollections()
        {
            try
            {
                if (File.Exists(userCollectionsFilePath))
                {
                    using (StreamReader r = new StreamReader(userCollectionsFilePath))
                    {
                        string json = r.ReadToEnd();
                        Collections = JsonConvert.DeserializeObject<List<UserCollection>>(json);
                        if (Collections == null)
                        {
                            LoggingService.Warning("[USER-COLLECTIONS-SERVICE]: Deserialization failed. Initializing with an empty collection.");
                            Collections = new List<UserCollection>();
                        }
                    }
                    var collectionsCount = Collections.Count;
                    LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Successfully loaded user collections with {collectionsCount} user collections");

                }
                else
                {
                    Collections = new List<UserCollection>();
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("[USER-COLLECTIONS-SERVICE]: An error occurred while loading the user collections", ex);
            }
        }

        /// <summary>
        /// Saves the current user collections to a JSON file.
        /// </summary>
        public static void SaveCollection()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(userCollectionsFilePath))
                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, Collections);
                }
                LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Saved user collections");
            }
            catch (Exception ex)
            {
                LoggingService.Error("[USER-COLLECTIONS-SERVICE]: An error occurred while saving the user collections", ex);
            }
        }

        /// <summary>
        /// Resets the user collections by clearing all entries.
        /// </summary>
        public static void ResetCollection()
        {
            LoggingService.Info("[USER-COLLECTIONS-SERVICE]: Resetting the user collections");
            Collections = new List<UserCollection>();
            SaveCollection();
        }


        /// <summary>
        /// Fills UserCollection with selected words and only those words
        /// </summary>
        public static void FillWords(UserCollection userCollection, List<string> words)
        {
            try
            {
                userCollection.Collection.Clear();
                userCollection.Collection[SupportedLanguages.ENGLISH] = new Dictionary<string, WordDefinition>();
                userCollection.Collection[SupportedLanguages.RUSSIAN] = new Dictionary<string, WordDefinition>();
                userCollection.Collection[SupportedLanguages.SPANISH] = new Dictionary<string, WordDefinition>();

                foreach (var word in words)
                {
                    userCollection.Collection[SupportedLanguages.ENGLISH][word] = WordCollection.Collection[SupportedLanguages.ENGLISH][word];
                    userCollection.Collection[SupportedLanguages.RUSSIAN][word] = WordCollection.Collection[SupportedLanguages.RUSSIAN][word];
                    userCollection.Collection[SupportedLanguages.SPANISH][word] = WordCollection.Collection[SupportedLanguages.SPANISH][word];
                }
                LoggingService.Info($"[USER-COLLECTIONS-SERVICE]: Filling the collection \"{userCollection.Name}\" with words successfully");
            }
            catch (Exception ex)
            {
                LoggingService.Error($"[USER-COLLECTIONS-SERVICE]: Filling the collection \"{userCollection.Name}\" with words failed", ex);
            }
        }
    }
}
