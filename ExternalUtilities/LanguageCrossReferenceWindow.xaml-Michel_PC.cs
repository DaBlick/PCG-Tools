using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
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

            CreateReferenceLanguageList();
            CheckOtherLanguages();

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

        void CreateReferenceLanguageList()
        {
            const string fileName = ResourcePath + "Strings.resx";
            foreach (var elem in XDocument.Load(fileName).Root.Elements("data"))
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
            var itemWarnings = new StringDictionary();
            var cultureNumber = 0;
            foreach (var culture in cultures)
            {
                var warningNumber = 0;
                var dict = new Dictionary<string, string>();

                var fileName = ResourcePath + "Strings." + culture + ".resx";
                foreach (var elem in XDocument.Load(fileName).Root.Elements("data"))
                {
                    var key = elem.Attribute("name").Value;
                    if (!key.StartsWith("___"))
                    {
                        var value = elem.Element("value").Value.Replace("\n", "<NL>");

                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, value);
                        }
                        else
                        {
                            _warnings.AppendLine(
                                $"{cultureNumber}-{warningNumber}: In culture {culture}, fragment {key} is defined again with value {value}.");
                            warningNumber++;
                        }

                        // Check if the word exists in reference language.
                        if (!_referenceTranslations.ContainsKey(key))
                        {
                            _warnings.AppendLine(
                                $"{cultureNumber}-{warningNumber}: Reference language does not contain from culture {culture}, fragment {key} with value {value}");
                            warningNumber++;
                        }
                    }
                }

                // Check if all English words are translated in culture language.
                //var referenceNumber = 0;
                foreach (var key in _referenceTranslations.Keys)
                {
                    if (!key.StartsWith("___"))
                    {

                        if (dict.Keys.Contains(key))
                        {
                            //itemWarnings[key] += $"{"", -10}: ";
                        }
                        else
                        { 
                            //_warnings.AppendLine(
                            //    $"{cultureNumber}-{referenceNumber}: Culture {culture} does not contain from reference language, fragment {key} with value {_referenceTranslations[key]}");
                            //referenceNumber++;

                            if (itemWarnings.ContainsKey(key) && itemWarnings[key].Contains(":"))
                            {
                                itemWarnings[key] += $"{culture, -10} ";
                            }
                            else
                            {
                                itemWarnings[key] += '\n' + $"{key, -40}: {culture, -10} ";
                            }
                        }
                    }
                }

                cultureNumber++;
            }

            _warnings.AppendLine("");
            foreach (string key in itemWarnings.Keys)
            {
                _warnings.AppendLine(itemWarnings[key]);
            }
        }
    }
}
