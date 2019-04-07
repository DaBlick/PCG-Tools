using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Properties;
using PcgTools.ViewModels;
using WPF.MDI;

// (c) 2011 Michel Keijzers

namespace PcgTools.MasterFiles
{
    /// <summary>
    /// Interaction logic for MasterFilesWindow.xaml
    /// </summary>
    public partial class MasterFilesWindow : IChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public IViewModel ViewModel { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        readonly MainWindow _mainWindow;


        /// <summary>
        /// 
        /// </summary>
        public MdiChild MdiChild { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IMemory Memory => null;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public MasterFilesWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();

            ViewModel = new MasterFilesViewModel(_mainWindow.ViewModel) { CloseWindow = CloseWindow};

            DataContext = ViewModel;
        }

        /// <summary>
        /// For satisfying XAML.
        /// </summary>
        public MasterFilesWindow()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewMasterFiles.ItemsSource = MasterFiles.Instances;
            FillListView();
        }


        /// <summary>
        /// 
        /// </summary>
        private void FillListView()
        {
            UpdateListView(listViewMasterFiles);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        private void UpdateListView(ListView listView)
        {
            // Scroll into view; all three commands are needed to make it work.
            if (Equals(listView, listViewMasterFiles))
            {
                if (MasterFiles.Instances.Count(item => item.IsSelected) > 0)
                {
                    listView.ScrollIntoView(listView.Items.Cast<ISelectable>().First(item => item.IsSelected));
                }
            }
           
            listView.Items.Refresh();
            try
            {
                listView.UpdateLayout();
            }
            catch (InvalidOperationException)
            {
                // Do nothing; for list view content generation can be in progress.
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void CloseWindow()
        {
            _mainWindow.ViewModel.MasterFilesWindow = null;

            Settings.Default.UI_MasterFilesWindowWidth = (int)MdiChild.Width;
            Settings.Default.UI_MasterFilesWindowHeight = (int)MdiChild.Height;
            Settings.Default.Save();

            MdiChild.Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void ActOnSettingsChanged(string property)
        {
            // No action needed
        }
    }
}