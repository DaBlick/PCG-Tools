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
    public partial class ExternalLinksThirdPartiesWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksThirdPartiesWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {

                new ExternalItem
                {
                    Name = "Microsoft Visual Studio",
                    Description = "Developer software",
                    Url = "http://www.visualstudio.com/en-us/visual-studio-homepage-vs.aspx",
                    BitmapPath = "VisualStudio.png"
                },
                new ExternalItem
                {
                    Name = "Stack Overflow",
                    Description = "Programmer forum",
                    Url = "http://www.stackoverflow.com",
                    BitmapPath = "stackoverflow.png"
                },
                new ExternalItem
                {
                    Name = "Flexera Installshield LE",
                    Description = "Installer software",
                    Url =
                        "http://www.flexerasoftware.com/products/software-installation/installshield-software-installer/",
                    BitmapPath = "Flexera.png"
                },
                new ExternalItem
                {
                    Name = "Assembla",
                    Description = "Versioning website",
                    Url = "https://www.assembla.com/home",
                    BitmapPath = "Assembla.png"
                },
                new ExternalItem
                {
                    Name = "Git",
                    Description = "Free versioning software",
                    Url = "http://git-scm.com/",
                    BitmapPath = "git.png"
                },
                new ExternalItem
                {
                    Name = "Git Extensions",
                    Description = "Free extension for Git",
                    Url = "http://sourceforge.net/projects/gitextensions",
                    BitmapPath = "git.png"
                },
                new ExternalItem
                {
                    Name = "WPF.MDI (CodePlex)",
                    Description = "Free Multiple Document Interface library",
                    Url = "http://wpfmdi.codeplex.com",
                    BitmapPath = ""
                },
                new ExternalItem
                {
                    Name = "Core FTP LE",
                    Description = "Free software for FTP",
                    Url = "http://coreftp.com",
                    BitmapPath = "coreftple.png"
                },
                new ExternalItem
                {
                    Name = "MIDI-OX",
                    Description = "Free MIDI application",
                    Url = "http://www.midiox.com/",
                    BitmapPath = "midiox.png"
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


