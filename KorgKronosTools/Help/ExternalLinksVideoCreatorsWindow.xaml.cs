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
    public partial class ExternalLinksVideoCreatorsWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksVideoCreatorsWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {
                new ExternalItem
                {
                    Name = "Marcio Badaró",
                    Description = "Video Creator",
                    Url = "https://www.youtube.com/channel/UCP1mBiVbdEZ4yoLcpu7HgEg",
                    BitmapPath = "MarcioBadaro.png"
                },
                new ExternalItem
                {
                    Name ="Rubens S. Felicio",
                    Description = "Video Creator",
                    Url = "https://www.youtube.com/channel/UCescfsHypLlr36lNyoyFDSw",
                    BitmapPath = "RubensFelicioYouTube.png"
                },
                new ExternalItem
                {
                    Name = "Adel Tannouri",
                    Description = "Video Creator",
                    Url = "https://www.youtube.com/user/thebestman0001",
                    BitmapPath = "AdelTannouriYouTube.png"
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
            var item = userControlExternalLink?.Tag as ExternalItem;
            if (item?.Url != null)
            {
                ShowUrl(item.Url);
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


