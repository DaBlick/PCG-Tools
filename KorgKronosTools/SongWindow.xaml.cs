using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PcgTools.Annotations;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KronosSpecific.Song;
using PcgTools.OpenedFiles;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using PcgTools.Songs;
using PcgTools.ViewModels;
using PcgTools.ViewModels.Commands.PcgCommands;
using WPF.MDI;

// (c) 2011 Michel Keijzers

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for SongWindow.xaml
    /// </summary>
    public partial class SongWindow : IChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public IViewModel ViewModel { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void ActOnSettingsChanged(string property)
        {
            // No action needed.
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public ISongViewModel SongViewModel => (ISongViewModel) ViewModel;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable NotAccessedField.Local
        private readonly MainWindow _mainWindow;


        /// <summary>
        /// 
        /// </summary>
        public MdiChild MdiChild { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ISongMemory SongMemory { get; }


        /// <summary>
        /// 
        /// </summary>
        public IMemory Memory => SongMemory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="songFileName"></param>
        /// <param name="songMemory"></param>
        /// <param name="openedPcgWindows"></param>
        public SongWindow(MainWindow mainWindow, string songFileName, ISongMemory songMemory, OpenedPcgWindows openedPcgWindows)
        {
            InitializeComponent();

            SongMemory = songMemory;
            _mainWindow = mainWindow;
            ViewModel = new SongViewModel(openedPcgWindows);
            SongViewModel.Song = listViewSongs.SelectedIndex >= 0
                ? songMemory.Songs.SongCollection[listViewSongs.SelectedIndex]
                : null;

            DataContext = ViewModel;

            /*
            ViewModel.PropertyChanged += OnViewModelChanged;

            ComboBoxConnectedPcgFile.ItemsSource = SongViewModel.OpenedPcgWindows.Items;

            var view = (CollectionView) CollectionViewSource.GetDefaultView(ComboBoxConnectedPcgFile.ItemsSource);
            view.Filter += ViewFilter;
            */

            songMemory.FileName = songFileName;
        }


        /// <summary>
        /// Returns only if from some model (both OS) and Kronos only.
        /// Note: Not used.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool ViewFilter(object sender)
        {
            var window = (OpenedPcgWindow) sender;
            return (SongMemory.Model.ModelType == Models.EModelType.Kronos) &&
                ModelCompatibility.AreModelsCompatible(window.PcgMemory.Model, SongMemory.Model);
        }


        /// <summary>
        /// For satisfying XAML.
        /// Note: Not used.
        /// </summary>
        public SongWindow()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewSongs.ItemsSource = SongMemory?.Songs.SongCollection;
            listViewSamples.ItemsSource = SongMemory?.Regions.RegionsCollection;
            FillListView();

            ButtonMidiTracks.IsEnabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void FillListView()
        {
            UpdateListView(listViewSongs);
            UpdateListView(listViewSamples);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        private void UpdateListView([NotNull] ListBox listView)
        {
            if (listView == null) throw new ArgumentNullException("listView");

            if (SongMemory == null)
            {
                return;
            }

            // Scroll into view; all three commands are needed to make it work.
            if (Equals(listView, listViewSongs))
            {
                if (SongMemory.Songs.SongCollection.Count(item => item.IsSelected) > 0)
                {
                    listView.ScrollIntoView(listView.Items.Cast<ISelectable>().First(item => item.IsSelected));
                }
            }
            else if (Equals(listView, listViewSamples))
            {
                if (SongMemory.Regions.RegionsCollection.Count(item => item.IsSelected) > 0)
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
        /// Note: Not used.
        /// </summary>
        // ReSharper disable MemberCanBePrivate.Global
        public void CloseWindow()
        {
            MdiChild.Close();

            Settings.Default.UI_SongWindowWidth = (int) MdiChild.Width;
            Settings.Default.UI_SongWindowHeight = (int) MdiChild.Height;
            Settings.Default.Save();
        }


        /// <summary>
        /// Note: Not used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case "WindowTitle":
                    MdiChild.Title = SongViewModel.WindowTitle;
                    break;

                //default:
                // Do nothing, not all properties need to be listened to.
                //break;
            }
        }


        /// <summary>
        /// Show (MIDI) tracks/timbres.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            {
                // Check if already exists. If so, show the already opened window.
                foreach (var child in from child in _mainWindow.Container.Children
                    where (child.Content is SongTimbresWindow)
                    let sngTimbresIteration = child.Content as SongTimbresWindow
                    where sngTimbresIteration.SngTimbresViewModel.Song == SongViewModel.Song
                    select child)
                {
                    child.Focus();
                    return;
                }
                 

                // Create combi window if not already present.
                var mdiChild = new MdiChild
                {
                    Title = GenerateSngTimbresWindowTitle(),
                    Content = new SongTimbresWindow(SongViewModel),
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Width = Settings.Default.UI_SongWindowWidth == 0 ? 700 : Settings.Default.UI_SongTimbresWindowWidth,
                    Height = Settings.Default.UI_SongWindowHeight == 0 ? 500 : Settings.Default.UI_SongTimbresWindowHeight,
                    Margin = new Thickness(0, 0, 0, 0)
                };

                ((SongTimbresWindow) (mdiChild.Content)).MdiChild = mdiChild;
                _mainWindow.Container.Children.Add(mdiChild);
                mdiChild.GotFocus += _mainWindow.MdiGotFocus;
                mdiChild.Closing += _mainWindow.MdiClosing;
            }
        }


        private void ExportToFile_Click(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();
            builder.AppendLine(" #  Song Name");
            builder.AppendLine("--- ------------------------------------------------------------");

            var index = 0;
            foreach (var song in SongMemory.Songs.SongCollection)
            {
                index++;
                builder.AppendLine($"{index,3} {song.Name}");
            }
            
            string fileName = $"{SongMemory.FileName}_output.txt";

            try
            {
                System.IO.File.WriteAllText(fileName, builder.ToString());
                Mouse.OverrideCursor = Cursors.Wait;
                    Process.Start(fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                var stringCaption = $"{Strings.ErrorOccurred}: \n\n" + 
                    $"{Strings.Message}: {ex.Message}\n\n" +
                    $"{Strings.InnerExceptionMessage}: {ex.InnerException?.Message ?? string.Empty}\n\n" + 
                    $"{Strings.StackTrace}: {ex.StackTrace}";

                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(Strings.PcgTools, stringCaption, MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK, MessageBoxOptions.None);
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

        }


        private void SamplesExportToFile_Click(object sender, RoutedEventArgs e)
        {
            
            var builder = new StringBuilder();
            builder.AppendLine(" #  Sample Name                                                  Sample File Name");
            builder.AppendLine("--- ------------------------------------------------------------ ------------------------------------------------------------");

            foreach (var region in SongMemory.Regions.RegionsCollection)
            {
                var sample = (Region) region;
                builder.AppendLine($"{sample.Index, 3} {sample.Name,-60} {sample.SampleFileName,-60}");
            }

            string fileName = $"{SongMemory.FileName}_output.txt";

            try
            {
                System.IO.File.WriteAllText(fileName, builder.ToString());
                Mouse.OverrideCursor = Cursors.Wait;
                Process.Start(fileName);

            }
            catch (UnauthorizedAccessException ex)
            {
                var stringCaption = $"{Strings.ErrorOccurred}: \n\n" +
                    $"{Strings.Message}: {ex.Message}\n\n" +
                    $"{Strings.InnerExceptionMessage}: {ex.InnerException?.Message ?? string.Empty}\n\n" +
                    $"{Strings.StackTrace}: {ex.StackTrace}";

                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(Strings.PcgTools, stringCaption, MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK, MessageBoxOptions.None);
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GenerateSngTimbresWindowTitle()
        {
            return MdiChild.Title + " - " + SongViewModel.Song.Name; //return MdiChild.WindowTitle "SONG"; // Use SelectedSong
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SongViewModel.Song = SongMemory.Songs.SongCollection[listViewSongs.SelectedIndex];
            ButtonMidiTracks.IsEnabled = ((SongViewModel.Song != null) && (SongMemory is KronosSongMemory));
        }


        /// <summary>
        /// Note: Not used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxPcgFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // _songMemory.ConnectedPcgMemory = ((OpenedPcgWindow) (ComboBoxConnectedPcgFile.SelectedValue)).PcgMemory;
        }
    }
}