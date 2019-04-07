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
    public partial class ExternalLinksKorgRelatedWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ExternalLinksKorgRelatedWindow()
        {
            InitializeComponent();

            var externalItems = new List<ExternalItem>
            {
                // PCG Tools

                new ExternalItem
                {
                    Name = "PCG Tools website",
                    Description = "PCG Tools main web site",
                    Url = "http://pcgtools.mkspace.nl",
                    BitmapPath = "pcgtoolssmaller.jpg"
                },
                new ExternalItem
                {
                    Name = "PCG Tools at Facebook",
                    Description = "PCG Tools Facebook group",
                    Url = "https://www.facebook.com/PcgTools",
                    BitmapPath = "facebook.png"
                },
                new ExternalItem
                {
                    Name = "PCG Tools at Twitter",
                    Description = "PCG Tools twitter account",
                    Url = "https://twitter.com/pcgtools",
                    BitmapPath = "twitter.png"
                },
                new ExternalItem
                {
                    Name = "PCG Tools at Google Plus",
                    Description = "PCG Tools Google Plus account",
                    Url = "https://plus.google.com/#117506377627258933594/posts",
                    BitmapPath = "googleplus.png"
                },
                new ExternalItem
                {
                    Name = "PCG Tools at Yahoo",
                    Description = "PCG Tools Yahoo group",
                    Url = "http://groups.yahoo.com/neo/groups/pcgtools/info",
                    BitmapPath = "yahoo.png"
                },

                // Korg
                new ExternalItem
                {
                    Name = "www.korg.com",
                    Description = "Official Korg website",
                    Url = "http://www.korg.com",
                    BitmapPath = "korg.jpg"
                },

                // Fora

                new ExternalItem
                {
                    Name = "KorgForums",
                    Description = "Biggest Korg forum",
                    Url = "http://www.korgforums.com",
                    BitmapPath = "korgforums.jpg"
                },
                new ExternalItem
                {
                    Name = "Korg Fans",
                    Description = "News, tips, downloads",
                    Url = "http://korgfans.wordpress.com",
                    BitmapPath = "korgfans.png"
                },
                new ExternalItem
                {
                    Name = "Karma-lab",
                    Description = "KARMA by Stephen Kay",
                    Url = "http://karma-lab.com",
                    BitmapPath = "karmalabs.png"
                },
                new ExternalItem
                {
                    Name = "Korg Patches",
                    Description = "Website for Korg patches",
                    Url = "http://www.korgpatches.com",
                    BitmapPath = "korgpatches.jpg"
                },
                new ExternalItem
                {
                    Name = "Kronoshaven",
                    Description = "Forum for Kronos",
                    Url = "http://www.kronoshaven.com",
                    BitmapPath = "kronoshaven.jpg"
                },
                new ExternalItem
                {
                    Name = "Kromeheaven",
                    Description = "Forum for Krome",
                    Url = "http://www.kromeheaven.com",
                    BitmapPath = "kromeheaven.png"
                },
                new ExternalItem
                {
                    Name = "Kronoscopie",
                    Description = "Forum for Kronos (French)",
                    Url = "http://www.kronoscopie.fr",
                    BitmapPath = "Kronoscopie.jpg"
                },
                new ExternalItem
                {
                    Name = "Forum Cifraclub",
                    Description = "Forum for Korg (Brazilian)",
                    Url = "http://forum.cifraclub.com.br/forum/8/",
                    BitmapPath = "forum_cifraclub_com_br.png"
                },
                new ExternalItem
                {
                    Name = "AudioKeys Forum",
                    Description = "Forum for Korg (French)",
                    Url = "http://www.audiokeys.net",
                    BitmapPath = "AudioKeys.png"
                },
                new ExternalItem
                {
                    Name = "Cliff Canyon 01/W FAQ",
                    Description = "Forum/Info for Korg 01/W",
                    Url = "http://indra.com/~cliffcan/01faq.htm#editors",
                    BitmapPath = "cliffcanyon.png"
                },
                new ExternalItem
                {
                    Name = "Qui Robinez",
                    Description = "Producer of Korg Sounds/Tutorials",
                    Url = "http://www.quirobinez.nl",
                    BitmapPath = "quirobinez.png"
                },
                new ExternalItem
                {
                    Name = "Audora",
                    Description = "Producer of Korg Sounds/Tutorials",
                    Url = "https://audora.ca/",
                    BitmapPath = "audora.png"
                },
                // Utilities

                new ExternalItem
                {
                    Name = "TidyKronos",
                    Description = "Kronos Application by Joe Keller",
                    Url = "http://www.keller12.de/tidykronos/",
                    BitmapPath = "tidykronos.png"
                },

                new ExternalItem
                {
                    Name = "PCGrid",
                    Description = "Kronos Application by KorganizR",
                    Url = "http://www.karma-lab.com/forum/showthread.php?t=19681",
                    BitmapPath = "pcgrid.png"
                },

                new ExternalItem
                {
                    Name = "ONKSOR",
                    Description = "Kronos Application by Olaf Arweiler",
                    Url = "http://www.arweiler.onlinehome.de/onksor.html",
                    BitmapPath = "onksor.jpg"
                },


                new ExternalItem
                {
                    Name = "AL-1 Editor",
                    Description = "Kronos Application by Chris",
                    Url = "http://www.chrutil.com/kronos",
                    BitmapPath = "al1editor.png"
                },

                new ExternalItem()
                {
                    Name = "MK Editor", Description = "microKORG Editor by JohnS",
                    Url = "http://www.artlum.com/microkorg", BitmapPath = "Artlum.png"
                },

                new ExternalItem()
                {
                    Name = "Radias Librarian", Description = "Radias Librarian by LiPI",
                    Url = "http://lipi.atw.hu/", BitmapPath = "RadiasLibrarian.png"
                },

                // Affiliates

                new ExternalItem
                {
                    Name = "Robert Rosen",
                    Description = "Technician/Keyboardist",
                    Url = "http://rosensound.com/",
                    BitmapPath = "robertrosen.jpg"
                },

                // Sound designers

                new ExternalItem
                {
                    Name = "KaPro (Kurt Ader Productions)",
                    Description = "Producer of Korg sounds",
                    Url = "https://www.facebook.com/pages/KApro-Kurt-Ader-Productions/323845221074090",
                    BitmapPath = "kapro.png"
                },
                new ExternalItem
                {
                    Name = "Sounds of Planet",
                    Description = "Producer of Korg sounds",
                    Url = "http://soundsofplanet.manifo.com/",
                    BitmapPath = "soundsofplanet.png"
                },
                new ExternalItem
                {
                    Name = "Kid Nepro",
                    Description = "Producer of Korg sounds",
                    Url = "http://www.kidnepro.com/KN/Korg/Korg.html",
                    BitmapPath = "kidheadspinsm.gif"
                },
                new ExternalItem
                {
                    Name = "Synthy Sounds",
                    Description = "Producer of Korg sounds",
                    Url = "http://www.synthysounds.co.uk/",
                    BitmapPath = "synthysounds.png"
                },
                
                // Shops

                new ExternalItem
                {
                    Name = "Lyana",
                    Description = "Dutch music shop",
                    Url = "http://www.lyana.nl",
                    BitmapPath = "lyana.png"
                },
                new ExternalItem
                {
                    Name = "Oostendorp Muziek",
                    Description = "Dutch music shop",
                    Url = "http://www.oostendorp-muziek.nl",
                    BitmapPath = "Oostendorpmuziek.jpg"
                },

                new ExternalItem
                {
                    Name="Dan Stesco",
                    Description ="Sound creator",
                    Url="http://www.danstesco.ro",
                    BitmapPath ="DanStesco.png"
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


