using UnityEngine;
using MelonLoader.TinyJSON;
using System.Collections.Generic;

namespace AssetLoader
{
    class LocalizationManager
    {
        internal static List<string> localizationWaitlistBundles = new List<string>(0);
        internal static List<string> localizationWaitlistAssets = new List<string>(0);

        public static string GetText(TextAsset textAsset)
        {
            ByteReader byteReader = new ByteReader(textAsset);
            string contents = "";
            if (byteReader.canRead) contents = byteReader.ReadLine();
            while (byteReader.canRead)
            {
                contents = contents + '\n' + byteReader.ReadLine();
            }
            return contents;
        }

        internal static void LoadPendingAssets()
        {
            Implementation.Log("Loading Waitlisted Localization Assets");
            for (int i = 0; i < LocalizationManager.localizationWaitlistAssets.Count; i++)
            {
                string bundleName = LocalizationManager.localizationWaitlistBundles[i];
                string assetName = LocalizationManager.localizationWaitlistAssets[i];
                LocalizationManager.LoadLocalization(bundleName,assetName);
            }
            LocalizationManager.localizationWaitlistAssets = new List<string>(0);
            LocalizationManager.localizationWaitlistBundles = new List<string>(0);
        }

        public static void LoadLocalization(string bundleName, string assetName)
        {
            AssetBundle assetBundle = ModAssetBundleManager.GetAssetBundle(bundleName);
            Object asset = assetBundle.LoadAsset(assetName);
            if (assetName.ToLower().EndsWith("json"))
            {
                LocalizationManager.LoadJSONLocalization(asset);
            }
            else if (assetName.ToLower().EndsWith("csv"))
            {
                LocalizationManager.LoadCSVLocalization(asset);
            }
            else
            {
                Implementation.LogWarning("Found localization '{0}' that could not be loaded.", assetName);
            }
        }

        public static void LoadLocalization(string localizationID, Dictionary<string, string> translationDictionary, bool useEnglishAsDefault = false)
        {
            string[] knownLanguages = Localization.GetLanguages().ToArray();


            string[] translations = new string[knownLanguages.Length];
            for (int i = 0; i < knownLanguages.Length; i++)
            {
                string language = knownLanguages[i];

                if (translationDictionary.ContainsKey(language))
                {
                    translations[i] = translationDictionary[language];
                }
                else if (useEnglishAsDefault && translationDictionary.ContainsKey("English"))
                {
                    translations[i] = translationDictionary["English"];
                }
            }

            var key = localizationID;
            if (!Localization.s_CurrentLanguageStringTable.DoesKeyExist(key))
            {
                Localization.s_CurrentLanguageStringTable.AddEntryForKey(key);
            }
            for (int j = 0; j < translations.Length; j++)
            {
                Localization.s_CurrentLanguageStringTable.GetEntryFromKey(key).m_Languages[j] = translations[j];
            }
        }

        public static void LoadCSVLocalization(Object asset)
        {
            TextAsset textAsset = asset.Cast<TextAsset>();
            if (textAsset == null)
            {
                Implementation.LogWarning("Asset called '{0}' is not a TextAsset as expected.", asset.name);
                return;
            }

            Implementation.Log("Processing asset '{0}' as csv localization.", asset.name);

            ByteReader byteReader = new ByteReader(textAsset);
            string[] languages = ModAssetBundleManager.Trim(byteReader.ReadCSV().ToArray());
            string[] knownLanguages = Localization.GetLanguages().ToArray();

            while (true)
            {
                BetterList<string> values = byteReader.ReadCSV();
                if (values == null)
                {
                    break;
                }

                string[] translations = new string[knownLanguages.Length];
                for (int i = 0; i < knownLanguages.Length; i++)
                {
                    string language = knownLanguages[i];
                    int index = System.Array.IndexOf(languages, language);
                    if (index == -1)
                    {
                        continue;
                    }

                    translations[i] = values[index].Trim();
                }

                var key = values[0];
                if (!Localization.s_CurrentLanguageStringTable.DoesKeyExist(key))
                {
                    Localization.s_CurrentLanguageStringTable.AddEntryForKey(key);
                }
                for (int j = 0; j < translations.Length; j++)
                {
                    Localization.s_CurrentLanguageStringTable.GetEntryFromKey(key).m_Languages[j] = translations[j];
                }
            }
        }

        public static void LoadJSONLocalization(Object asset)
        {
            TextAsset textAsset = asset.Cast<TextAsset>();
            if (textAsset == null)
            {
                Implementation.LogWarning("Asset called '{0}' is not a TextAsset as expected.", asset.name);
                return;
            }
            Implementation.Log("Processing asset '{0}' as json localization.", asset.name);
            string contents = GetText(textAsset);
            ProxyObject dict = JSON.Load(contents) as ProxyObject;
            foreach (var pair in dict)
            {
                string locID = pair.Key;
                Dictionary<string, string> locDict = pair.Value.Make<Dictionary<string, string>>();
                LoadLocalization(locID, locDict, true);
            }
        }


    }
}
