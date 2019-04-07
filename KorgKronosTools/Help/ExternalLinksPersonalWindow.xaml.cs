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
    public partial class ExternalLinksPersonalWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksPersonalWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {
                new ExternalItem
                {
                    Name = "Co-incidental",
                    Description = "Rock-cover band I play in",
                    Url = "http://co-incidental.nl/",
                    BitmapPath = "Co-incidental.png"
                },
                new ExternalItem
                {
                    Name = "Co-incidental",
                    Description = "FB Page of Co-Incidental",
                    Url = "https://www.facebook.com/Co-incidental-204155766341066/?ref=hl",
                    BitmapPath = "Co-incidental.png"
                },  
                new ExternalItem
                {
                    Name = "Nothing Else Matters",
                    Description = "Yearly Charity Concert",
                    Url = "www.nothingelsematters.nl",
                    BitmapPath = "NothingElseMatters.png"
                }, 
                new ExternalItem
                {
                    Name = "Nothing Else Matters",
                    Description = "FB Page of Nothing Else Matters",
                    Url = "https://www.facebook.com/nothingelsematters.nl/?fref=ts",
                    BitmapPath = "NothingElseMatters.png"
                }, 
                new ExternalItem
                {
                    Name = "GitaarDemo",
                    Description = "Guitar Shop of Co-incidental Guitarist",
                    Url = "https://www.facebook.com/GitaarDemo-1428013174150822/",
                    BitmapPath = "GitaarDemo.png"
                },
                new ExternalItem
                {
                    Name = "Altran.nl",
                    Description = "My employer",
                    Url ="http://www.altran.nl/",
                    BitmapPath = "Altran.png"
                },
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
                ButtonLink50
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


