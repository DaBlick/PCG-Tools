// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Windows;

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for CommandLineInterfaceWindow.xaml
    /// </summary>
    public partial class CommandLineInterfaceWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        readonly string _diagnosticsText;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagnosticsText"></param>
        public CommandLineInterfaceWindow(string diagnosticsText)
        {
            _diagnosticsText = diagnosticsText;
            InitializeComponent();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBlock.Text = _diagnosticsText;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
