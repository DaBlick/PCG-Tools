using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace ExternalUtilities
{
    /// <summary>
    /// Interaction logic for LanguageCrossReferenceWindow.xaml
    /// </summary>
    public partial class LanguageCrossReferenceWindow : Window
    {
        private const string ResourcePath = 
            @"c:\users\michel\OneDrive\pcgtools2013\korgkronostools\pcgtoolsresources\";

        // Key: text fragment to translate (keyword)
        // Value: English translation.
        private SortedDictionary<string, string> _referenceTranslations;

        private StringBuilder _warnings;

        public LanguageCrossReferenceWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _referenceTranslations = new SortedDictionary<string, string>();
            _warnings = new StringBuilder();

            CheckLanguages();
            //CreateReferenceLanguageList();
            //CheckOtherLanguages();

            var text = ShowAllTexts();
            TranslationsTextBox.Text = text.ToString();
            WarningsTextBox.Text = _warnings.ToString();

            File.WriteAllText("warnings.txt", _warnings.ToString());
            Process.Start(new ProcessStartInfo("warnings.txt"));
        }

        /// <summary>
        /// Not used, but maybe for later; shows all strings and values.
        /// </summary>
        /// <returns></returns>
        private StringBuilder ShowAllTexts()
        {
            var text = new StringBuilder();
            foreach (var key in _referenceTranslations.Keys)
            {
                text.Append(key + ": ");
                var languages = new StringBuilder();
                foreach (var langValue in _referenceTranslations[key])
                {
                    languages.Append(langValue);
                }
                text.AppendLine(languages.ToString());
            }
            return text;
        }

        private void CheckLanguages()
        {
            var cultures = new[] { "", "cs", "de", "el", "es", "fr", "nl", "pl",
                "pt-BR", "pt-BR", "ru", "tr" }; // Removed: "sr-Latn-RS",

            _warnings.Append($"{"Phrase/Word/Item",-50} ");
            foreach (string culture in cultures)
            {
                _warnings.Append($"{(culture == "" ? "English" : culture),-6} ");
            }
            _warnings.AppendLine("\n");
            
            var dict = new Dictionary<string, List<bool>>(); // Word -> present[culture]

             // Create word list
            for (var cultureIndex = 0; cultureIndex < cultures.Length; cultureIndex++)
            {
                var culture = cultures[cultureIndex];
                var fileName = ResourcePath + "Strings" + (culture == "" ? "" : ".") + culture + ".resx";
                var xElement = XDocument.Load(fileName).Root;
                if (xElement != null)
                    foreach (var key in from elem in xElement.Elements("data")
                        let xAttribute = elem.Attribute("name")
                        where xAttribute != null
                        let key = xAttribute.Value
                        let element = elem.Element("value")
                        where element != null
                        let value = element.Value.Replace("\n", "<NL>")
                        where !key.StartsWith("___") && !key.StartsWith("String") select key)
                    {
                        if (dict.ContainsKey(key))
                        {
                            dict[key][cultureIndex] = true;
                        }
                        else
                        {
                            dict[key] = new List<bool>();
                            for (var listIndex = 0; listIndex < cultures.Length; listIndex++)
                            {
                                dict[key].Add(false);
                            }
                            dict[key][cultureIndex] = true;
                        }
                    }
            }

            // show non complete cultures
            var dictKeys = dict.Keys.ToList();
            dictKeys.Sort();

            foreach (var key in dictKeys)
            {
                if (dict[key].Contains(false))
                {
                    var item = dict[key];
                    _warnings.Append($"{key,-50}: ");
                    for (var listIndex = 0; listIndex < cultures.Length; listIndex++)
                    {
                        _warnings.Append(dict[key][listIndex] ? $"{"",-6} " : $"{cultures[listIndex],-6} ");
                    }

                    _warnings.AppendLine();
                }
            }
        }


        void CreateReferenceLanguageList()
        {
            const string fileName = ResourcePath + "Strings.resx";
            var elements = XDocument.Load(fileName).Root.Elements("data").ToList();
            elements.Sort();
            foreach (var elem in elements)
            {
                var key = elem.Attribute("name").Value;
                var value = elem.Element("value").Value.Replace("\n", "<NL>");

                if (!_referenceTranslations.ContainsKey(key))
                {
                    _referenceTranslations.Add(key, value);
                }
                else
                {
                    _warnings.AppendLine(
                        $"In reference language, fragment {key} is defined again with value {value}.");
                }
            }
        }

        void CheckOtherLanguages()
        {

            var cultures = new[] {"cs", "de", "el", "es", "fr", "nl", "pl", "pt-BR", "pt-BR", "ru", "sr-Latn-RS", "tr"};
            foreach (var culture in cultures)
            {
                var dict = new Dictionary<string, string>();

                var fileName = ResourcePath + "Strings." + culture + ".resx";
                foreach (var elem in XDocument.Load(fileName).Root.Elements("data"))
                {
                    var key = elem.Attribute("name").Value;
                    var value = elem.Element("value").Value.Replace("\n", "<NL>");

                    if (!dict.ContainsKey(key))
                    {
                        dict.Add(key, value);
                    }
                    else
                    {
                        _warnings.AppendLine(
                            $"In culture {culture}, fragment {key} is defined again with value {value}.");
                    }

                    // Check if the word exists in reference language.
                    if (!_referenceTranslations.ContainsKey(key))
                    {
                        _warnings.AppendLine(
                            $"Reference language does not contain from culture {culture}, fragment {key} with value {value}");
                    }
                }    

                // Check if all English words are translated in culture language.
                foreach (var key in _referenceTranslations.Keys)
                {
                    if (!dict.Keys.Contains(key))
                    {
                        _warnings.AppendLine(
                            $"Culture {culture} does not contain from reference language, fragment {key} with value {_referenceTranslations[key]}");
                    }
                }
            }
        }
    }
}
