// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Common.Utils;
using PcgTools.Edit;
using PcgTools.ListGenerator;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using PcgTools.Tools;
using PcgTools.ViewModels;
using WPF.MDI;

namespace PcgTools
{
    /// <summary>
    /// Interaction logic for PcgWindow.xaml
    /// </summary>
    public partial class PcgWindow : IChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public IViewModel ViewModel { get; private set; }
        

        /// <summary>
        /// 
        /// </summary>
        IPcgViewModel PcgViewModel => (IPcgViewModel) ViewModel;


        /// <summary>
        /// 
        /// </summary>
        readonly IPcgMemory _pcgMemory; // Only for moving variable from PcgWindow constructor to Window_Loaded.
        
        
        /// <summary>
        /// 
        /// </summary>
        public IPcgMemory PcgMemory => PcgViewModel.SelectedPcgMemory;


        /// <summary>
        /// 
        /// </summary>
        public IMemory Memory => PcgMemory;


        /// <summary>
        /// 
        /// </summary>
        public MdiChild MdiChild { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        readonly MainWindow _mainWindow;


        /// <summary>
        /// 
        /// </summary>
        ICollectionView _listViewBanksView;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="pcgFileName"></param>
        /// <param name="pcgMemory"></param>
        public PcgWindow(MainWindow mainWindow, string pcgFileName, IPcgMemory pcgMemory)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            ViewModel = new PcgViewModel(mainWindow.ViewModel.PcgClipBoard)
            {
                ShowDialog = (type, items) =>
                {
                    Window window = null;

                    switch (type)
                    {
                            // Programs
                        case ViewModels.PcgViewModel.DialogType.EditSingleProgram:
                            window = new WindowEditSingleProgram(items.First() as IProgram) {Owner = _mainWindow};
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultiplePrograms:
                            //window = new WindowEditMultiplePrograms(items as List<IProgram>) { Owner = _mainWindow };
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditSingleProgramBank:
                            //window = new WindowEditSingleProgramBank(items.First() as IProgramBank) { Owner = _mainWindow };
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultipleProgramBanks:
                            //window = new WindowEditMultipleProgramBanks(items as List<IProgramBank>) { Owner = _mainWindow };
                            break;

                            // Combis
                        case ViewModels.PcgViewModel.DialogType.EditSingleCombi:
                            window = new WindowEditSingleCombi(items.First() as ICombi) {Owner = _mainWindow};
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultipleCombis:
                            //window = new WindowEditMultipleCombis(items as List<ICombi>) { Owner = _mainWindow };
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditSingleCombiBank:
                            //window = new WindowEditSingleCombiBank(items.First() as ICombiBank) { Owner = _mainWindow };
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultipleCombiBanks:
                            //window = new WindowEditMultipleCombiBanks(items as List<ICombiBank>) { Owner = _mainWindow };
                            break;


                            // Set list slots                                    
                        case ViewModels.PcgViewModel.DialogType.EditSingleSetListSlot:
                            window = new WindowEditSingleSetListSlot(items.First() as ISetListSlot) {Owner = _mainWindow};
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultipleSetListSlots:
                            //window = new WindowEditMultipleSetListSlot(items as List<ISetListSlot>)
                            //{
                            //    Owner = _mainWindow
                            //};
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditSingleSetList:
                            window = new WindowEditSingleSetList(items.First() as ISetList) {Owner = _mainWindow};
                            break;

                        case ViewModels.PcgViewModel.DialogType.EditMultipleSetLists:
                            //window = new WindowEditMultipleSetLists(items as List<ISetLists>) { Owner = _mainWindow };
                            break;

                        default:
                            throw new ApplicationException("Illegal window type");
                    }

                    window?.ShowDialog();
                },


                ShowPasteWindow = () =>
                {
                    var window = new SettingsWindow {Owner = _mainWindow};
                    window.ShowDialog();
                },


                ShowMessageBox = (text, title, messageBoxButton, messageBoxImage, messageBoxResult) =>
                    WindowUtils.ShowMessageBox(_mainWindow, text, title, messageBoxButton,
                        messageBoxImage, messageBoxResult),


                SetCursor = WindowUtils.SetCursor,


                ShowListGenerator = () =>
                {
                    var window = new ListGeneratorWindow((PcgMemory) ViewModel.SelectedMemory)
                    {
                        Owner = _mainWindow
                    };
                    try
                    {
                        window.ShowDialog();
                    }
                    catch (InvalidOperationException)
                    {
                        // Do nothing
                    }
                },


                ShowProgramReferencesChanger = () =>
                {
                    var window = new ProgramReferenceChangerWindow((PcgMemory) ViewModel.SelectedMemory)
                    {
                        Owner = _mainWindow
                    };

                    try
                    {
                        window.ShowDialog();
                    }
                    catch (InvalidOperationException)
                    {
                        // Do nothing
                    }
                },

                ShowTimbresWindow = (combi, width, height) =>
                {
                    // Check if already exists. If so, show the already opened window.
                    foreach (var child in from child in _mainWindow.Container.Children
                        where (child.Content is CombiWindow)
                        let combiWindowIteration = child.Content as CombiWindow
                        where combiWindowIteration.CombiViewModel.Combi == combi
                        select child)
                    {
                        child.Focus();
                        return;
                    }

                    // Create combi window if not already present.
                    var mdiChild = new MdiChild
                    {
                        Title = GenerateCombiWindowTitle(combi),
                        Content = new CombiWindow(PcgViewModel, combi),
                        MinimizeBox = false,
                        MaximizeBox = false,
                        Width = width,
                        Height = height,
                        Margin = new Thickness(0, 0, 0, 0)
                    };

                    ((CombiWindow) (mdiChild.Content)).MdiChild = mdiChild;
                    _mainWindow.Container.Children.Add(mdiChild);
                    mdiChild.GotFocus += _mainWindow.MdiGotFocus;
                    mdiChild.Closing += _mainWindow.MdiClosing;
                },


                EditParameterWindow = patches =>
                {
                    var window = new WindowEditParameter(patches);
                    window.ShowDialog();
                },

                UpdateTimbresWindows = () =>
                {
                    // Update every timbre window.
                    foreach (var child in from child in _mainWindow.Container.Children
                        where (child.Content is CombiWindow)
                        select child)
                    {
                        var viewModel = ((CombiWindow) (child.Content)).CombiViewModel;
                        viewModel.UpdateUiContent();
                        child.Title = GenerateCombiWindowTitle(viewModel.Combi);
                    }

                    /*
                            // If it is a master file being changed, also update all other windows.
                            var masterFile = MasterFiles.Instances.FindMasterPcg(PcgMemory.ModelType);
                            if (masterFile != null)
                            {

                                _mainWindow.ViewModel.UpdatePcgWindowsOfModelType(PcgMemory.ModelType);
                            }
                            */
                },


                MoveSelectedPatchesUp = MoveSelectedPatchesUp,


                MoveSelectedPatchesDown = MoveSelectedPatchesDown,


                GetSelectedPatchListViewIndex = () => listViewPatches.SelectedIndex,


                SetPcgFileAsMasterFile = SetPcgFileAsMasterFile,


                CloseWindow = CloseWindow
            };

            DataContext = ViewModel;
            ViewModel.PropertyChanged += OnViewPropertyChanged;
            listViewBanks.ItemsSource = PcgViewModel.Banks;
            listViewPatches.ItemsSource = PcgViewModel.Patches;

            _pcgMemory = pcgMemory;
            _pcgMemory.FileName = pcgFileName;

            if (_pcgMemory.CombiBanks == null)
            {
                ButtonTimbres.Visibility = Visibility.Collapsed;
            }

            if (_pcgMemory.SetLists == null)
            {
                ButtonAssign.Visibility = Visibility.Collapsed;
            }

            //ToolTipService.ShowOnDisabled = "True"
             //         ToolTipService.IsEnabled = "{Binding Path=ToolTipEnabled, Converter={StaticResource InverseBooleanConverter}}"
              //        ToolTipService.ToolTip = "{Binding Path=ToolTip, Mode=OneTime}"
        }


        /// <summary>
        /// 
        /// </summary>
        public PcgWindow()
        {
        }
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PcgViewModel.SelectedMemory = _pcgMemory;
            PcgViewModel.SelectedScopeSet = ViewModels.PcgViewModel.ScopeSet.Banks;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetProgramBanksGridViews()
        {
            if (PcgViewModel.SelectedPcgMemory == null)
            {
                return;
            }
            
            var columns = ((GridView) listViewBanks.View).Columns;

            columns[0].Width = 50;
            columns[1].Width = 120;
            columns[1].Header = Strings.SynthesisType_pcgw;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.ProgramBanks.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.ProgramBanks.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank) bank).FilterForUi;
            
            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 70.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);

            if (PcgViewModel.SelectedPcgMemory.AreFavoritesSupported)
            {
                SetGridViewPatchesColumn(Strings.Fav, 30.0);
            }

            SetGridViewPatchesColumn(Strings.Category, 100.0);

            if (PcgViewModel.SelectedPcgMemory.HasSubCategories)
            {
                SetGridViewPatchesColumn(Strings.SubCategory, 100.0);
            }

            if (Settings.Default.UI_ShowNumberOfReferencesColumn && 
                ((PcgMemory.CombiBanks != null) || (PcgMemory.SetLists != null)))
            {
                SetGridViewPatchesColumn(Strings.NumberOfReferences, 50);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetCombiBanksGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            var columns = ((GridView)listViewBanks.View).Columns;

            columns[0].Width = 50;
            columns[1].Width = 0;
            columns[1].Header = string.Empty;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.CombiBanks.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.CombiBanks.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank)bank).FilterForUi;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 70.0); // For e.g. Studio Rack EXB-A127
            SetGridViewPatchesColumn(Strings.Name, 175.0);

            if (PcgViewModel.SelectedPcgMemory.AreFavoritesSupported)
            {
                SetGridViewPatchesColumn(Strings.Fav, 30.0);
            }

            SetGridViewPatchesColumn(Strings.Category, 100.0);

            if (PcgViewModel.SelectedPcgMemory.HasSubCategories)
            {
                SetGridViewPatchesColumn(Strings.SubCategory, 100.0);
            }

            if (Settings.Default.UI_ShowNumberOfReferencesColumn &&
                ((PcgMemory.CombiBanks != null) || (PcgMemory.SetLists != null)))
            {
                SetGridViewPatchesColumn(Strings.NumberOfReferences, 50);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetSetListsGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            var columns = ((GridView)listViewBanks.View).Columns;

            columns[0].Width = 50;
            columns[1].Width = 180;
            columns[1].Header = Strings.SetListName;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.SetLists.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.SetLists.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank)bank).FilterForUi;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 60.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);
            SetGridViewPatchesColumn(Strings.Reference, 90.0);
            SetGridViewPatchesColumn(Strings.ProgramCombiName, 210.0);
            SetGridViewPatchesColumn(Strings.ColumnVolume, 50.0);
            SetGridViewPatchesColumn(Strings.Description, 1000.0);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDrumKitBanksGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            var columns = ((GridView)listViewBanks.View).Columns;

            columns[0].Width = 60;
            columns[1].Width = 0;
            columns[1].Header = string.Empty;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.DrumKitBanks.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.DrumKitBanks.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank)bank).FilterForUi;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 70.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetDrumPatternBanksGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            var columns = ((GridView)listViewBanks.View).Columns;

            columns[0].Width = 60;
            columns[1].Width = 0;
            columns[1].Header = string.Empty;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.DrumPatternBanks.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.DrumPatternBanks.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank)bank).FilterForUi;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 70.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetWaveSequenceBanksGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            var columns = ((GridView)listViewBanks.View).Columns;

            columns[0].Width = 60;
            columns[1].Width = 0;
            columns[1].Header = string.Empty;

            listViewBanks.Visibility = Visibility.Visible;
            if (PcgViewModel.SelectedPcgMemory.WaveSequenceBanks.BankCollection.Any(bank => bank.IsSelected))
            {
                listViewBanks.ScrollIntoView(
                    PcgViewModel.SelectedPcgMemory.WaveSequenceBanks.BankCollection.First(bank => bank.IsSelected));
            }
            else if (listViewBanks.SelectedItems.Count > 0)
            {
                listViewBanks.ScrollIntoView(listViewBanks.SelectedItems[0]);
            }

            _listViewBanksView = CollectionViewSource.GetDefaultView(listViewBanks.ItemsSource);
            _listViewBanksView.Filter = bank => ((IBank)bank).FilterForUi;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.ID, 70.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetAllPatchesGridViews()
        {
            if (PcgMemory == null)
            {
                return;
            }

            listViewBanks.Visibility = Visibility.Collapsed;
            _listViewBanksView = null;

            HideAllGridViewPatchesColumns();

            SetGridViewPatchesColumn(Strings.PatchType, 100.0);
            SetGridViewPatchesColumn(Strings.ID, 70.0);
            SetGridViewPatchesColumn(Strings.Name, 175.0);

            if (PcgViewModel.SelectedPcgMemory.AreFavoritesSupported)
            {
                SetGridViewPatchesColumn(Strings.Fav, 30.0);
            }

            SetGridViewPatchesColumn(Strings.Category, 100.0);

            if (PcgViewModel.SelectedPcgMemory.HasSubCategories)
            {
                SetGridViewPatchesColumn(Strings.SubCategory, 100.0);
            }

            if (PcgMemory.SetLists != null)
            {
                SetGridViewPatchesColumn(Strings.Reference, 90.0);
                SetGridViewPatchesColumn(Strings.ProgramCombiName, 210.0);
            }

            if (Settings.Default.UI_ShowNumberOfReferencesColumn &&
                ((PcgMemory.CombiBanks != null) || (PcgMemory.SetLists != null)))
            {
                SetGridViewPatchesColumn(Strings.NumberOfReferences, 50);
            }
        }
        

        /// <summary>
        /// Set column width of a column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="width"></param>
        private void SetGridViewPatchesColumn(string columnName, double width)
        {
            var columns = ((GridView)listViewPatches.View).Columns;
            // There should be always one, but El Kar has a crash about a null pointer exception, so First -> FirstOrDefault.
            var columnWithName = columns.FirstOrDefault(column => column.Header.ToString() == columnName);
            if (columnWithName != null)
            {
                columnWithName.Width = width;
            }
        }


        /// <summary>
        /// Hiding all grid viewi patches columns before setting them.
        /// </summary>
        private void HideAllGridViewPatchesColumns()
        {
            var columns = ((GridView)listViewPatches.View).Columns;
            foreach (var column in columns)
            {
                column.Width = 0.0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void MoveSelectedPatchesUp()
        {
            if (_listViewBanksView.CurrentPosition > 0)
            {
                _listViewBanksView.MoveCurrentToPrevious();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void MoveSelectedPatchesDown()
        {
            if (_listViewBanksView.CurrentPosition < listViewPatches.SelectedItems.Count)
            {
                _listViewBanksView.MoveCurrentToNext();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        private void SetPcgFileAsMasterFile(IModel model, string fileName)
        {
            MasterFiles.MasterFiles.Instances.SetPcgFileAsMasterFile(model, fileName);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <returns></returns>
        private string GenerateCombiWindowTitle(IPatch combi)
        {
            return
                $"{Strings.TimbresOf} {PcgViewModel.SelectedPcgMemory.FileName}, {Strings.Combi.ToLower()} {combi.Id}: {combi.Name}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewBanksSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PcgViewModel.BanksChanged();
            Patches();

            // Due to some bug, remove all selected items from the same bank type which are selected, except when ALL items
            // are to be removed (in the case of going to another bank type or all-patches.
            try
            {
                listViewBanks.UpdateLayout();
            }
            catch (InvalidOperationException)
            {
                // Ignore.
            }
            
            if (e.RemovedItems.Count > 0)
            {
                var selectedBanks = PcgViewModel.Banks.Where(item => item.IsSelected &&
                 item.Parent == ((IBank)(e.RemovedItems[0])).Parent).ToList().Count;

                if (selectedBanks != e.RemovedItems.Count)
                {
                    foreach (var bank in PcgViewModel.Banks.Where(item => item.IsSelected && (e.RemovedItems.Contains(item))))
                    {
                        bank.IsSelected = false;
                    }
                }

                PcgViewModel.NumberOfSelectedPatches = PcgViewModel.Patches.Count(item => item.IsSelected);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewPatchesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncPatches(e);

            if (e.AddedItems.Count > 0)
            {
                listViewPatches.ScrollIntoView(e.AddedItems[0]);
            }
            PcgViewModel.EditSelectedItemCommand.CanExecute(null);

            var selectedPatches = listViewPatches.SelectedItems.Count;
            if ((selectedPatches == 1) && (listViewPatches.SelectedItem is IProgram) ||
                listViewPatches.SelectedItem is ICombi)
            {
                PcgViewModel.LastSelectedProgramOrCombi = (IPatch) listViewPatches.SelectedItem;
            }

            PcgViewModel.NumberOfSelectedPatches = PcgViewModel.Patches.Count(item => item.IsSelected);
        }


        /// <summary>
        /// Since WPF does not take into account binding for nonvisible patches in a listview, it needs to be done manually.
        /// </summary>
        private void SyncPatches(SelectionChangedEventArgs e)
        {
            foreach (ISelectable patch in e.AddedItems)
            {
                patch.IsSelected = true;
            }

            foreach (ISelectable patch in e.RemovedItems)
            {
                patch.IsSelected = false;
            }

            //foreach (var patchInListView in PcgViewModel.Patches)
            //{
            //    patchInListView.IsSelected = listViewPatches.SelectedItems.Contains(patchInListView);
            //}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewBanksGotFocus(object sender, RoutedEventArgs e)
        {
            PcgViewModel.SelectedScopeSet = ViewModels.PcgViewModel.ScopeSet.Banks;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewPatchGotFocus(object sender, RoutedEventArgs e)
        {
            PcgViewModel.SelectedScopeSet = ViewModels.PcgViewModel.ScopeSet.Patches;
        }
        

        /// <summary>
        /// 
        /// </summary>
        void CloseWindow()
        {
            MdiChild.Close();
            foreach (var child in GetChilds())
            {
                _mainWindow.Container.Children.Remove(child);
            }

            Settings.Default.UI_PcgWindowWidth = (int) MdiChild.Width;
            Settings.Default.UI_PcgWindowHeight = (int) MdiChild.Height;
            Settings.Default.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<MdiChild> GetChilds()
        {
            return (from child in _mainWindow.Container.Children
                    let combiWindow = child.Content as CombiWindow
                    where (combiWindow != null) && (combiWindow.CombiViewModel.Combi.Root == PcgViewModel.SelectedPcgMemory)
                    select child).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
            case "ProgramBanksSelected":
                OnViewPropertyChangedProgramBanksSelected();
                break;

            case "CombiBanksSelected":
                OnViewPropertyChangedCombiBanksSelected();
                break;

            case "SetListsSelected":
                OnViewPropertyChangedSetListsSelected();
                break;

            case "DrumKitBanksSelected":
                OnViewPropertyChangedDrumKitBanksSelected();
                break;

            case "DrumPatternBanksSelected":
                OnViewPropertyChangedDrumPatternBanksSelected();
                break;

            case "WaveSequenceBanksSelected":
                OnViewPropertyChangedWaveSequenceBanksSelected();
                break;

            case "AllPatchesSelected":
                OnViewPropertyChangedAllPatchesSelected();
                break;

            case "SelectedScopeSet":
                OnViewPropertyChangedSelectedScopeSet();
                break;
 
            case "WindowTitle":
                // Can be called from background worker.
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MdiChild.Title = PcgViewModel.WindowTitle;
                }));
                break;

            case "Patches":
                Patches();
                break;

            //default:
                // Do nothing, not all properties need to be listened to.
                //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedProgramBanksSelected()
        {
            if (PcgViewModel.ProgramBanksSelected)
            {
                SetProgramBanksGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedCombiBanksSelected()
        {
            if (PcgViewModel.CombiBanksSelected)
            {
                SetCombiBanksGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedSetListsSelected()
        {
            if (PcgViewModel.SetListsSelected)
            {
                SetSetListsGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedDrumKitBanksSelected()
        {
            if (PcgViewModel.DrumKitBanksSelected)
            {
                SetDrumKitBanksGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedDrumPatternBanksSelected()
        {
            if (PcgViewModel.DrumPatternBanksSelected)
            {
                SetDrumPatternBanksGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedWaveSequenceBanksSelected()
        {
            if (PcgViewModel.WaveSequenceBanksSelected)
            {
                SetWaveSequenceBanksGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedAllPatchesSelected()
        {
            if (PcgViewModel.AllPatchesSelected)
            {
                SetAllPatchesGridViews();
                PcgViewModel.BanksChanged();
                Patches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnViewPropertyChangedSelectedScopeSet()
        {
            listViewBanks.BorderThickness =
                new Thickness(PcgViewModel.SelectedScopeSet == ViewModels.PcgViewModel.ScopeSet.Banks ? 3.0 : 1.0);

            listViewPatches.BorderThickness =
                new Thickness(PcgViewModel.SelectedScopeSet == ViewModels.PcgViewModel.ScopeSet.Patches ? 3.0 : 1.0);
        }


        /// <summary>
        /// </summary>
        private void Patches()
        {
            var firstSelected = PcgViewModel.Patches.FirstOrDefault(patch => patch.IsSelected);
            if (firstSelected != null)
            {
                listViewPatches.ScrollIntoView(firstSelected);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPatchesMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PcgViewModel.EditSelectedItem();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBanksDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PcgViewModel.EditSelectedItem();
        }


        /// <summary>
        /// Settings have been changed, so ShowNumberOfReferences might have changed.
        /// So update, the column (to hide/show it).
        /// </summary>
        /// <param name="property"></param>
        public void ActOnSettingsChanged(string property)
        {
            OnViewPropertyChangedProgramBanksSelected();
            OnViewPropertyChangedCombiBanksSelected();
            OnViewPropertyChangedSetListsSelected();
            OnViewPropertyChangedDrumKitBanksSelected();
            OnViewPropertyChangedDrumPatternBanksSelected();
            OnViewPropertyChangedWaveSequenceBanksSelected();
            OnViewPropertyChangedAllPatchesSelected();

            if (listViewPatches.ItemsSource != null)
            {
                foreach (IPatch patch in listViewPatches.ItemsSource)
                {
                    patch.Update("NumberOfReferencesShown");
                }
            }

            if (listViewPatches.ItemsSource != null)
            {
                foreach (ISetListSlot patch in listViewPatches.ItemsSource.OfType<ISetListSlot>())
                {
                    patch.Update("ShowSingleLinedSetListSlotDescriptions");
                }
            }
        }
    }
}
