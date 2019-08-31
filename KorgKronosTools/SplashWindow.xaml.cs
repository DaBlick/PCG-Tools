// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Windows.Media.Imaging;
using PcgTools.Gui;
using PcgTools.ViewModels;

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        const int ImagePixelArea = 10000;


        /// <summary>
        /// 
        /// </summary>
        public SplashWindow()
        {
            InitializeComponent();
            DrawLogoAndName();
        }


        /// <summary>
        /// 
        /// </summary>
        void DrawLogoAndName()
        {
            var logo = new Logos().GetRandomLogo();
            if (logo.ImageName != string.Empty)
            {
                image.Source = new BitmapImage(
                    new Uri("/PcgTools;component/Help/External Links/" + logo.ImageName, UriKind.Relative));

                /* Adapt size to a maximum of ImagePixelArea:
                 * 1. nw * nh = 10.000
                 * 2. nw / nh = ow / oh               where nw = new width, nh = new height, ow = org width, oh = org height
                 * 3. nw / nh = ow / oh => nw = ow / oh * nh
                 * Combine 3 with 1: ow / oh * nh * nh = 10000 => nh * nh = 10000 / (ow / oh) => 
                 *                   nh = sqrt(10000 / (ow / oh)) 
                 *           with 1: nw = 10000 / nh 
                 */
                image.Height = Math.Sqrt(ImagePixelArea / (image.Source.Width / image.Source.Height));
                image.Width = ImagePixelArea / image.Height;
            }

            labelVersion.Content = MainViewModel.Version;
            labelSponsorName.Content = logo.Name;
        }


        /// <summary>
        /// 
        /// </summary>
        public void CloseWindow()
        {
           Close();
        }
    }
}
