using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using PcgTools.PcgToolsResources;

namespace PcgTools.Help
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        public AboutWindow(string version)
        {
            InitializeComponent();
            labelVersion.Content = version;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"{Strings.LinkWarning}.\n{Strings.Message}: {exception.Message}", 
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image2MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowDonateButton();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image2KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Return))
            {
                ShowDonateButton();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowDonateButton()
        {
            try
            {
                Process.Start(new ProcessStartInfo(
                 "//www.paypal.com/cgi-bin/webscr?cmd=_donations&business=XC8G28GYMV4VY&lc=US&currency_code=" +
                 "EUR&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted"));
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"{Strings.DonateWarning}.\n{Strings.Message}: {exception.Message}",
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
