// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Windows;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Properties;
using PcgTools.ViewModels;
using WPF.MDI;

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for SngTimbres window.xaml
    /// </summary>
    public partial class SongTimbresWindow : IChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public IViewModel ViewModel { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public ISngTimbresViewModel SngTimbresViewModel => (ISngTimbresViewModel) ViewModel;


        /// <summary>
        /// 
        /// </summary>
        public MdiChild MdiChild { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="songViewModel"></param>
        public SongTimbresWindow(ISongViewModel songViewModel)
        {
            InitializeComponent();
            ViewModel = new SngTimbresViewModel(songViewModel)
            {
                UpdateUiContent = () =>
                {
                    listViewTimbres.Items.Refresh();
                    listViewTimbres.UpdateLayout();
                },

                CloseWindow = CloseWindow
            };

            DataContext = ViewModel;
        }


        /// <summary>
        /// 
        /// </summary>
        public SongTimbresWindow()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewTimbres.ItemsSource = SngTimbresViewModel.Song.Timbres.TimbresCollection;
            //var view = CollectionViewSource.GetDefaultView(listViewTimbres.ItemsSource);
            //view.Filter = bank => true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewTimbresSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                listViewTimbres.ScrollIntoView(e.AddedItems[0]);
                SngTimbresViewModel.UpdateUiContent();
            }
        }


        // IChildWindow

        /// <summary>
        /// 
        /// </summary>
        void CloseWindow()
        {
            MdiChild.Close();

            Settings.Default.UI_SongTimbresWindowWidth = (int)MdiChild.Width;
            Settings.Default.UI_SongTimbresWindowHeight = (int)MdiChild.Height;
            Settings.Default.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        public IMemory Memory => ViewModel.SelectedMemory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void ActOnSettingsChanged(string property)
        {
            // No action needed.
        }
    }
}