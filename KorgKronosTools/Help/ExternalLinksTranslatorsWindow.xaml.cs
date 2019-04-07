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
    public partial class ExternalLinksTranslatorsWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksTranslatorsWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {
                new ExternalItem
                {
                    Name = "Syntey", Description = "Czech Translator",
                    Url = "https://soundcloud.com/syntey", BitmapPath = "syntey.png"
                },
                new ExternalItem
                {
                    Name = "Yuma", Description = "Dutch and German Translator", 
                    Url = "http://soundcloud.com/rmyuma", BitmapPath = "Yuma.jpg", 
                },
                new ExternalItem
                {
                    Name = "Mathieu Maes", Description = "Dutch Translator",
                    Url = "http://partycoverband.wix.com/cupsandplates", BitmapPath = "Cupsandplates.png"
                },
                new ExternalItem
                {
                    Name = "Francois", Description = "French Translator",
                    Url = "http://www.kronoscopie.fr", BitmapPath = "Kronoscopie.jpg"
                },
                new ExternalItem
                {
                    Name= "Frank Callies (Spare)", Description = "German Translator",
                },
                new ExternalItem
                {
                    Name= "Jens", Description = "German Translator",
                },
                new ExternalItem
                {
                    Name= "Timo Lill", Description = "German Translator",
                },
                new ExternalItem
                {
                    Name= "Jim Dijkstra", Description = "Greek Translator"
                },
                new ExternalItem
                {
                    Name= "Giorgos", Description = "Greek Translator",
                    Url="http://www.reverbnation.com/giorgostrichopoulos", BitmapPath = "giorgostrichopoulos.png"
                },
                new ExternalItem
                {
                    Name= "Enrico Puglisi", Description = "Italian Translator",
                    Url="https://www.facebook.com/kronospatchlab", BitmapPath = "KronosPatchLab.png"
                },
                new ExternalItem
                {
                    Name= "Gianluca Calesso", Description = "Italian Translator"
                },
                new ExternalItem
                {
                    Name ="DamianoMusic", Description = "Polish Translator"
                },
                new ExternalItem
                {
                    Name ="Adrian Craig", Description = "Polish Translator"
                },
                 new ExternalItem
                {
                    Name ="Marcin Aleksander", Description = "Polish Translator"
                },
                new ExternalItem
                {
                    Name ="Rubens S. Felicio", Description = "Portuguese (Brazilian) Translator",
                    Url=   "http://facebook.com/rsfmusictech", BitmapPath = "rsfmusictech.jpg",
                },
                new ExternalItem
                {
                    Name="Thiago Costa", Description="Portuguese (Brazilian) Translator"
                },
                new ExternalItem
                {
                    Name ="Luis Costa", Description="Portuguese (Portugal) Translator",
                    Url = "http://palcoprincipal.sapo.pt/user/luiscosta", BitmapPath = "LuisCosta.png", 
                },
                new ExternalItem
                {
                    Name ="Luis Costa", Description="Portuguese (Portugal) Translator",
                    Url = "http://palcoprincipal.sapo.pt/user/luiscosta", BitmapPath = "LuisCosta.png", 
                },
                new ExternalItem
                {
                    Name="Saša Rajak", Description="Serbian Translator"
                },
                new ExternalItem
                {
                    Name="Bernardo W.", Description="Spanish Translator"
                },
                new ExternalItem
                {
                    Name="Mario Pablo", Description="Spanish Translator"
                },
                new ExternalItem
                {
                    Name="Umut Erhan", Description="Turkish Translator",
                    Url ="http://www.youtube.com/user/slimhan", BitmapPath = "UmutErhan.jpg"
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
                ButtonLink30
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


