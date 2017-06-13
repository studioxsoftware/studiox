using System.IO;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Localization.Sources;

namespace StudioX.Localization.Dictionaries.Json
{
    /// <summary>
    ///     Provides localization dictionaries from json files in a directory.
    /// </summary>
    public class JsonFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly string directoryPath;

        /// <summary>
        ///     Creates a new <see cref="JsonFileLocalizationDictionaryProvider" />.
        /// </summary>
        /// <param name="directoryPath">Path of the dictionary that contains all related XML files</param>
        public JsonFileLocalizationDictionaryProvider(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }
        
        public override void Initialize(string sourceName)
        {
            var fileNames = Directory.GetFiles(directoryPath, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var fileName in fileNames)
            {
                var dictionary = CreateJsonLocalizationDictionary(fileName);
                if (Dictionaries.ContainsKey(dictionary.CultureInfo.Name))
                {
                    throw new StudioXInitializationException(sourceName + " source contains more than one dictionary for the culture: " + dictionary.CultureInfo.Name);
                }

                Dictionaries[dictionary.CultureInfo.Name] = dictionary;

                if (fileName.EndsWith(sourceName + ".json"))
                {
                    if (DefaultDictionary != null)
                    {
                        throw new StudioXInitializationException("Only one default localization dictionary can be for source: " + sourceName);
                    }

                    DefaultDictionary = dictionary;
                }
            }
        }

        protected virtual JsonLocalizationDictionary CreateJsonLocalizationDictionary(string fileName)
        {
            return JsonLocalizationDictionary.BuildFromFile(fileName);
        }
    }
}