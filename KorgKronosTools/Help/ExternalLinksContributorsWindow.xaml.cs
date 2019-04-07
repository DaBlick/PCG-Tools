using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using PcgTools.PcgToolsResources;

// (c) 2011 Michel Keijzers

namespace PcgTools.Help
{
    /// <summary>
    ///
    /// </summary>
    public partial class ExternalLinksContributorsWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksContributorsWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {
                // Developers

                new ExternalItem
                {
                    Name = "KorganizR", Description = "Feature Developer",
                },
                new ExternalItem
                {
                    Name = "mTrilby", Description = "Feature Developer",
                },
                new ExternalItem
                {
                    Name = "Vanni Torelli", Description = "Feature Developer",
                },
                new ExternalItem
                {
                    Name = "Mike Hildner", Description = "Bug Fixer",
                },
  
                // Korg
                             
                /*new ExternalItem
                {
                    Name = "Richard Formidoni", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Steve Pavao", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Dan Phillips", Description = "Information Supplier",
                },
                */

                // Important people

                new ExternalItem
                {
                    Name = "Sharp (Irish Acts Studio)", Description = "Forum Moderator",
                    Url = "http://www.irishacts.com/", BitmapPath = "IrishActsStudio.png"
                },
                new ExternalItem
                {
                    Name = "François Rossi", Description = "Language Support Expert",
                    Url = "http://www.kronoscopie.fr", BitmapPath = "Kronoscopie.jpg"
                },

                // Idea Notificators
                
                new ExternalItem
                {
                    Name = "Tim Godfrey", Description = "Idea Notificator",
                },
                //new ExternalItem
                //{
                    // Does not want to be mentioned.
                    // Name = "Sander Veeken", Description = "Idea Notificator",
                //},
                new ExternalItem
                {
                    Name = "mTrilby", Description = "Idea Notificator",
                },
                new ExternalItem
                {
                    Name = "Sunriser", Description = "Idea Supplier / Tester",
                },

                // Information Suppliers

                new ExternalItem
                {
                    Name = "Cynkh", Description = "Information Supplier / Tester",
                },
                new ExternalItem
                {
                    Name = "McHale", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Matt Gerasimof", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Gary Slattery", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "CJ Johansson", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Paul Hirschvogel", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Patrick Keijzer", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Joost Wilgehof", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Ed Fenner", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Xavier Miller", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "PpublicDuendo", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Youri", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Mark Steven McLeod", Description = "Information Supplier",
                },
                new ExternalItem
                {
                    Name = "Mark White", Description = "Information Supplier",
                },
                new ExternalItem()
                {
                    Name="keyboarder", Description = "Information Supplier",
                    Url = "http://forum.rmmedia.ru", BitmapPath = "rmmedia.png"
                },
                new ExternalItem()
                {
                    Name="Igor Elshaidt", Description = "Information Supplier"
                },
                new ExternalItem()
                {
                    Name="JohnS", Description = "Information Supplier",
                    Url = "http://www.artlum.com/microkorg", BitmapPath = "Artlum.png"
                },

                // Testers
                
                new ExternalItem
                {
                    Name = "Cello", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "Jim Knopf", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "John Laumen", Description = "Tester",
                },
                 new ExternalItem
                {
                    Name = "Matej Golian", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "AdDeRoo", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "Klaus Jäde", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "Paul Hirschvogel", Description = "Tester",
                },
                new ExternalItem
                {
                    Name = "Jörn Westhoff", Description = "Bug Reporter",
                },
                new ExternalItem
                {
                    Name="Karim El-Far", Description="Bug Reporter",
                    Url="http://www.kelfar.net", BitmapPath= "kelfar.png"
                },
                new ExternalItem
                {
                    Name="Michael Maschek", Description="Bug Reporter",
                    Url="https://www.facebook.com/celticvoyagerband", BitmapPath= "celticvoyager.png"
                },
                new ExternalItem
                {
                    Name="Dan Stesco", Description="Information Supplier",
                    Url="http://www.danstesco.ro", BitmapPath="DanStesco.png"
                }
            };

            var linkButtons = new List<UserControlExternalLink>
            {
                ButtonLink1,
                ButtonLink2,
                ButtonLink3,
                ButtonLink4,
                ButtonLink5,
                ButtonLink6,
                ButtonLink7,
                ButtonLink8,
                ButtonLink9,
                ButtonLink10,
                ButtonLink11,
                ButtonLink12,
                ButtonLink13,
                ButtonLink14,
                ButtonLink15,
                ButtonLink16,
                ButtonLink17,
                ButtonLink18,
                ButtonLink19,
                ButtonLink20,
                ButtonLink21,
                ButtonLink22,
                ButtonLink23,
                ButtonLink24,
                ButtonLink25,
                ButtonLink26,
                ButtonLink27,
                ButtonLink28,
                ButtonLink29,
                ButtonLink30,
                ButtonLink31,
                ButtonLink32,
                ButtonLink33,
                ButtonLink34,
                ButtonLink35,
                ButtonLink36,
                ButtonLink37,
                ButtonLink38,
                ButtonLink39,
                ButtonLink40,
                ButtonLink41,
                ButtonLink42,
                ButtonLink43,
                ButtonLink44,
                ButtonLink45,
                ButtonLink46,
                ButtonLink47,
                ButtonLink48,
                ButtonLink49,
                ButtonLink50,
                ButtonLink51,
                ButtonLink52,
                ButtonLink53,
                ButtonLink54,
                ButtonLink55,
                ButtonLink56,
                ButtonLink57,
                ButtonLink58,
                ButtonLink59,
                ButtonLink60
            };

            for (var index = 0; index < externalItems.Count; index++)
            {
                var userControl = linkButtons[index];
                userControl.PreviewMouseLeftButtonUp += ButtonLinkOnPreviewMouseLeftButtonUp;
                userControl.Tag = externalItems[index];
                userControl.DataContext = externalItems[index];
            }

            for (var index = externalItems.Count; index < linkButtons.Count; index++)
            {
                linkButtons[index].Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        private void ButtonLinkOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var userControlExternalLink = sender as UserControlExternalLink;
            if (userControlExternalLink != null)
            {
                var item = userControlExternalLink.Tag as ExternalItem;
                if ((item != null) && (item.Url != null))
                {
                    ShowUrl(item.Url);
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        private void ShowUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url));
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"{Strings.LinkWarning}.\n{Strings.Message}:{exception.Message}", 
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


