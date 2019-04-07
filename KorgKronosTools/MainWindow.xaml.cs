// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
#if !DEBUG
using System.Threading;
#endif
using System.Windows;
using System.Windows.Controls;
using Common.Utils;
using Microsoft.Win32;
using PcgTools.Help;
using PcgTools.MasterFiles;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.OpenedFiles;
using PcgTools.ViewModels;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using WPF.MDI;

// Do not remove; used for Release build

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainViewModel ViewModel { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        private enum EWindowState
        {
            Minimized = 0, // Used in settings
            Normal,
            Maximized
        };


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
#if !DEBUG
            try
            {
#endif
            var splashWindow = new SplashWindow {WindowStartupLocation = WindowStartupLocation.CenterScreen};
            splashWindow.Show();
#if !DEBUG
                Thread.Sleep(5000);
#endif

            // Set culture info ... check list at: http://techmantium.com/culture-codes/
            try
            {
                var culture = new CultureInfo(Settings.Default.UI_Language);
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;

            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                // Continue without selecting a language.
            }

            InitializeComponent();
            LoadWindowProperties();

            // Set UI language.
            foreach (var item in menuItemUiLanguages.Items.Cast<MenuItem>().Where(
                item => item.Tag.ToString().Equals(CultureInfo.CurrentUICulture.Name)))
            {
                item.IsChecked = true;
            }

            // Set view model.
            ViewModel = new MainViewModel();
            ViewModel.PropertyChanged += OnViewModelChanged;
            Container.Children.CollectionChanged += OnMdiContainerChanged;
            

            ViewModel.UpdateSelectedMemory = () =>
            {
                ViewModel.SelectedMemory = (FocusedWindow == null) ? null : FocusedWindow.Memory;
            };
            

            ViewModel.OpenFileDialog = (title, filter, filterIndex, multiSelect) =>
            {
                var dlg = new OpenFileDialog
                {
                    Title = title,
                    Filter = filter,
                    FilterIndex = filterIndex,
                    Multiselect = multiSelect
                };

                dynamic result = new ExpandoObject();
                var showDialog = dlg.ShowDialog();
                if (showDialog != null && showDialog.Value)
                {
                    result.Success = true;
                    result.Files = dlg.FileNames;
                    result.FilterIndex = dlg.FilterIndex;
                }
                else
                {
                    result.Success = false;
                    result.Files = null;
                }
                return result;
            };
            

            ViewModel.SaveFileDialog = (title, filter, fileName) =>
            {
                var dlg = new SaveFileDialog
                {
                    Title = title,
                    Filter = filter,
                    FileName = fileName,
                    FilterIndex = ViewModel.GetFilterIndexOfFile(System.IO.Path.GetExtension(fileName), filter)
                };

                dynamic result = new ExpandoObject();
                var showDialog = dlg.ShowDialog();
                if (showDialog != null && showDialog.Value)
                {
                    result.Success = true;
                    result.Files = dlg.FileNames;
                }
                else
                {
                    result.Success = false;
                    result.Files = null;
                }
                return result;
            };


            ViewModel.SetCursor = WindowUtils.SetCursor;


            ViewModel.ShowDialog = windowType =>
            {
                Window window;
                switch (windowType)
                {
                    case MainViewModel.WindowType.Settings:
                        window = new SettingsWindow {Owner = this};
                        break;

                    case MainViewModel.WindowType.About:
                        window = new AboutWindow(MainViewModel.Version) {Owner = this};
                        break;

                    case MainViewModel.WindowType.ExternalLinksKorgRelated:
                        window = new ExternalLinksKorgRelatedWindow {Owner = this};
                        break;

                    case MainViewModel.WindowType.ExternalLinksContributors:
                        window = new ExternalLinksContributorsWindow { Owner = this };
                        break;

                    case MainViewModel.WindowType.ExternalLinksVideoCreators:
                        window = new ExternalLinksVideoCreatorsWindow { Owner = this };
                        break;

                    case MainViewModel.WindowType.ExternalLinksDonators:
                        window = new ExternalLinksDonatorsWindow {Owner = this};
                        break;

                    case MainViewModel.WindowType.ExternalLinksTranslators:
                        window = new ExternalLinksTranslatorsWindow {Owner = this};
                        break;

                    case MainViewModel.WindowType.ExternalLinksThirdParties:
                        window = new ExternalLinksThirdPartiesWindow {Owner = this};
                        break;

                    case MainViewModel.WindowType.ExternalLinksOasysVoucherCodeSponsorsWindow:
                        window = new ExternalLinksOasysVoucherCodeSponsorsWindow{ Owner = this };
                        break;

                    case MainViewModel.WindowType.ExternalLinksPersonal:
                        window = new ExternalLinksPersonalWindow { Owner = this };
                        break;

                    default:
                        throw new ApplicationException("Illegal window type");
                }

                window.ShowDialog();
            };


            ViewModel.ShowMessageBox = (text, title, messageBoxButton, messageBoxImage, messageBoxResult) =>
                                       WindowUtils.ShowMessageBox(
                                           this, text, title, messageBoxButton, messageBoxImage, messageBoxResult);


            ViewModel.StartProcess = process => Process.Start(process);


            ViewModel.GotoNextWindow = () =>
            {
                var nrChildren = Container.Children.Count;

                int index;
                for (index = 0; index < nrChildren; index++)
                {
                    if (Equals(FocusedWindow, Container.Children[index].Content))
                    {
                        break;
                    }
                }

                if ((index < nrChildren) && (nrChildren > 0))
                {
                    Container.Children[(index + 1) % nrChildren].Focus();
                }
            };


            ViewModel.GotoPreviousWindow = () =>
            {
                var nrChildren = Container.Children.Count;

                int index;
                for (index = 0; index < nrChildren; index++)
                {
                    if (Equals(FocusedWindow, Container.Children[index].Content))
                    {
                        break;
                    }
                }

                if ((index < nrChildren) && (nrChildren > 0))
                {
                    Container.Children[(index - 1 + nrChildren ) % nrChildren].Focus();
                }
            };


            ViewModel.CreateMdiChildWindow = (fileName, childWindowType, memory, width, height) =>
            {
                UIElement uiElement;
                switch (childWindowType)
                {
                    case MainViewModel.ChildWindowType.Pcg:
                        uiElement = new PcgWindow(this, fileName, (PcgMemory) memory);
                        ViewModel.OpenedPcgWindows.Items.Add(new OpenedPcgWindow { PcgMemory = (PcgMemory)memory });
                        break;

                    case MainViewModel.ChildWindowType.Song:
                        uiElement = new SongWindow(this, fileName, (SongMemory) memory, ViewModel.OpenedPcgWindows);
                        break;

                    case MainViewModel.ChildWindowType.MasterFiles:
                        uiElement = new MasterFilesWindow(this);
                        break;

                    default:
                        throw new ApplicationException("Illegal window type");
                }

                var mdiChild = new MdiChild
                {
                    Title = fileName,
                    Content = uiElement,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Width = width,
                    Height = height,
                    Margin = new Thickness(0, 0, 0, 0),
                };

                ((IChildWindow) (mdiChild.Content)).MdiChild = mdiChild;

                mdiChild.GotFocus += MdiGotFocus;
                mdiChild.Closing += MdiClosing;
                Container.Children.Add(mdiChild);
                ViewModel.RaisePropertyChanged("ChildWindows");

                return mdiChild;
            };

            ViewModel.CloseView = Close;

            DataContext = ViewModel;
            ViewModel.UpdateAppTitle();
            splashWindow.CloseWindow();

#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(this, String.Format("{0}: \n\n{1}: {2}\n\n{3}: {4}\n\n{5}: {6}",
                    Strings.ErrorOccurred, Strings.Message, ex.Message, 
                    Strings.InnerExceptionMessage, ex.InnerException == null ? String.Empty : ex.InnerException.Message, Strings.StackTrace, ex.StackTrace), 
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
#if !DEBUG
            try
            {
#endif

            if (App.Arguments == null)
            {
                return;
            }

            ViewModel.HandleAppArguments();

            MasterFiles.MasterFiles.Instances.UpdateStates();

#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(this, String.Format("{0}: \n\n{1}: {2}\n\n{3}: {4}\n\n{5}: {6}",
                    Strings.ErrorOccurred, Strings.Message, ex.Message, 
                    Strings.InnerExceptionMessage, ex.InnerException == null ? 
                        String.Empty :
                        ex.InnerException.Message, Strings.StackTrace, ex.StackTrace), 
                    Strings.PcgTools,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif

            // Start timer.
            //  DispatcherTimer setup
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnTimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 15);
            dispatcherTimer.Start();
        }


        /// <summary>
        /// Backup files if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            ViewModel.OnTimerTick();
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void OnMdiContainerChanged(object o, NotifyCollectionChangedEventArgs args)
        {
            ViewModel.ChildWindows.Clear();
            foreach (var child in Container.Children)
            {
                ViewModel.ChildWindows.Add((IChildWindow) (child.Content));
            }
        }
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void MdiGotFocus(object obj, RoutedEventArgs args)
        {
            if (FocusedWindow == null)
            {
                ViewModel.CurrentChildViewModel = null;
            }
            else
            {
                var window = FocusedWindow as PcgWindow;
                if (window != null)
                {
                    ViewModel.SelectedMemory = window.PcgMemory;
                    ViewModel.CurrentChildViewModel = window.ViewModel;
                }
                else
                {
                    var songWindow = FocusedWindow as SongWindow;
                    if (songWindow != null)
                    {
                        ViewModel.SelectedMemory = songWindow.SongMemory;
                    }
                    else
                    {
                        var combiWindow = FocusedWindow as CombiWindow;
                        if (combiWindow != null)
                        {
                            ViewModel.SelectedMemory = combiWindow.CombiViewModel.Combi.PcgRoot;
                            ViewModel.CurrentChildViewModel = combiWindow.ViewModel;
                        }
                        else
                        {
                            var songTimbresWindow = FocusedWindow as SongTimbresWindow;
                            if (songTimbresWindow != null)
                            {
                                ViewModel.SelectedMemory = songTimbresWindow.SngTimbresViewModel.Song.Memory;
                                ViewModel.CurrentChildViewModel = songTimbresWindow.ViewModel;
                            }
                            else
                            {
                                var filesWindow = FocusedWindow as MasterFilesWindow;
                                if (filesWindow != null)
                                {
                                    ViewModel.SelectedMemory = null;
                                    ViewModel.CurrentChildViewModel = filesWindow.ViewModel;
                                }
                                else
                                {
                                    throw new ApplicationException("Unknown focused window");
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private IChildWindow FocusedWindow
        {
            get
            {
                if (Container?.Children == null)
                {
                    return null;
                }

                return (from child in Container.Children where 
                            child.Focused select (IChildWindow) child.Content).FirstOrDefault();
            }
        }
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void MdiClosing(object obj, RoutedEventArgs args)
        {
            var content = ((IChildWindow) (((MdiChild) obj).Content));
            args.Handled = !content.ViewModel.Close(false);

            if (content is PcgWindow)
            {
                ViewModel.OpenedPcgWindows.RemoveWindowWithPcgMemory(content.Memory);
            }

            // Set child windows of children.
            ViewModel.ChildWindows.Clear();
            foreach (var child in Container.Children)
            {
                ViewModel.ChildWindows.Add((IChildWindow)(child.Content));
            }
            
            // Set current child view model.
            if (Container.Children.Count > 0)
            {
                var lastContent = Container.Children[Container.Children.Count - 1].Content;
                if (lastContent is PcgWindow)
                {
                    ViewModel.CurrentChildViewModel = (lastContent as PcgWindow).ViewModel;
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            var closeApp = true;

            for (var index = Container.Children.Count - 1; index >= 0; index--)
            {
                if (CloseChild(index))
                {
                    continue;
                }

                closeApp = false;
                break;
            }

            if (closeApp)
            {
                SaveWindowProperties();

                Settings.Default.Save();
            }
            else
            {
                e.Cancel = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CloseChild(int index)
        {
            var child = Container.Children[index];
            if (child.Content is PcgWindow)
            {
                if (((PcgWindow) (child.Content)).ViewModel.Close(true))
                {
                    return true;
                }
            }
            else if (child.Content is CombiWindow)
            {
                if (((CombiWindow) (child.Content)).ViewModel.Close(true))
                {
                    return true;
                }
            }
            else if (child.Content is SongWindow)
            {
                if (((SongWindow) (child.Content)).ViewModel.Close(true))
                {
                    return true;
                }
            }
            else if (child.Content is SongTimbresWindow)
            {
                if (((SongTimbresWindow)(child.Content)).ViewModel.Close(true))
                {
                    return true;
                }
            }
            else if (child.Content is MasterFilesWindow)
            {
                if (((MasterFilesWindow) (child.Content)).ViewModel.Close(true))
                {
                    return true;
                }
            }
            else
            {
                throw new ApplicationException("Illegal child window type");
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void LoadWindowProperties()
        {
            Width = Settings.Default.UI_MainWindowWidth;
            Height = Settings.Default.UI_MainWindowHeight;
            Top = Settings.Default.UI_MainWindowTop;
            Left = Settings.Default.UI_MainWindowLeft;

            switch ((EWindowState) Settings.Default.UI_MainWindowState)
            {
                case EWindowState.Minimized:
                    // Do not start in minimized state.
                    WindowState = WindowState.Normal;
                    break;

                case EWindowState.Normal:
                    WindowState = WindowState.Normal;
                    break;

                case EWindowState.Maximized:
                    WindowState = WindowState.Maximized;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveWindowProperties()
        {
            Settings.Default.UI_MainWindowWidth = Width;
            Settings.Default.UI_MainWindowHeight = Height;
            Settings.Default.UI_MainWindowTop = Top;
            Settings.Default.UI_MainWindowLeft = Left;

            switch (WindowState)
            {
                case WindowState.Minimized:
                    Settings.Default.UI_MainWindowState = (int) EWindowState.Minimized;
                    break;

                case WindowState.Normal:
                    Settings.Default.UI_MainWindowState = (int) EWindowState.Normal;
                    break;

                case WindowState.Maximized:
                    Settings.Default.UI_MainWindowState = (int) EWindowState.Maximized;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void OnViewModelChanged(object o, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
            case "SelectedTheme":
                var enumConverter = new Dictionary<MainViewModel.Theme, MdiContainer.ThemeType>
                {
                    {MainViewModel.Theme.Generic, MdiContainer.ThemeType.Generic},
                    {MainViewModel.Theme.Luna, MdiContainer.ThemeType.Luna},
                    {MainViewModel.Theme.Aero, MdiContainer.ThemeType.Aero}
                };
                Container.Theme = enumConverter[ViewModel.SelectedTheme];
                break;

            // default:
                //throw new ApplicationException("Illegal property name");
                // break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectUiLanguage(object sender, RoutedEventArgs e)
        {
            // Uncheck unselected.
            foreach (MenuItem languageItem in menuItemUiLanguages.Items)
            {
                languageItem.IsChecked = false;
            }

            // Check and set selected menu item/language.
            var mi = sender as MenuItem;
            if (mi != null)
            {
                mi.IsChecked = true;

                if (Settings.Default.UI_Language != mi.Tag.ToString())
                {
                    Settings.Default.UI_Language = mi.Tag.ToString();
                    Settings.Default.Save();

                    ViewModel.ShowMessageBox(Strings.RestartToChangeLanguage, Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok,
                                             WindowUtils.EMessageBoxImage.Information, WindowUtils.EMessageBoxResult.Ok);
                }
            }
        }
    }
}
