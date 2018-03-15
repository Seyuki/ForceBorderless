using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Static library class for localization
    /// </summary>
    public static class LocalizationLib
    {
        /// <summary>  
        /// Get current culture info from OS language 
        /// </summary>  
        /// <param name="element"></param>  
        /// <returns>Culture info string</returns>  
        private static string GetCurrentCultureName()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        /// <summary>  
        /// Sets or replaces the ResourceDictionary by dynamically loading a Localization ResourceDictionary from the file path passed in.  
        /// </summary>  
        /// <param name="element"></param>  
        public static void SetLanguageResourceDictionary(FrameworkElement element)
        {
            string DefaultLoc = "en-US";
            string CurrentLoc = GetCurrentCultureName();
            string[] ElementNames = element.GetType().ToString().Split('.');
            string ElementName = string.Empty;
            if (ElementNames.Length >= 2) ElementName = ElementNames[ElementNames.Length - 1];

            // Read in ResourceDictionary File  
            ResourceDictionary languageDictionary = new ResourceDictionary();

            try
            {
                languageDictionary.Source = new Uri("/Localization/" + ElementName + "." + CurrentLoc + ".xaml", UriKind.RelativeOrAbsolute);
            }
            catch
            {
                languageDictionary.Source = new Uri("/Localization/" + ElementName + "." + DefaultLoc + ".xaml", UriKind.RelativeOrAbsolute);
            }

            // Remove any previous Localization dictionaries loaded  
            int langDictId = -1;
            for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
            {
                var md = element.Resources.MergedDictionaries[i];

                if (md.Contains("ResourceDictionaryName"))
                {
                    if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                    {
                        langDictId = i;
                        break;
                    }
                }
            }
            if (langDictId == -1)
            {
                // Add in newly loaded Resource Dictionary  
                element.Resources.MergedDictionaries.Add(languageDictionary);
            }
            else
            {
                // Replace the current langage dictionary with the new one  
                element.Resources.MergedDictionaries[langDictId] = languageDictionary;
            }
        }
    }
}
