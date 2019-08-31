// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Properties;
using PcgTools.PcgToolsResources;
using Common.Extensions;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// Interaction logic for WindowSortedLists.xaml
    /// </summary>
    public partial class ListGeneratorWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        List<CheckBox> _programBankButtons;


        /// <summary>
        /// 
        /// </summary>
        List<CheckBox> _combiBankButtons;


        /// <summary>
        /// 
        /// </summary>
        readonly IPcgMemory _pcgMemory;


        /// <summary>
        /// 
        /// </summary>
        int _setListRangeMin;


        /// <summary>
        /// 
        /// </summary>
        int _setListRangeMax;


        /// <summary>
        /// 
        /// </summary>
        const int InternalProgramBanksCheckBoxesAmount = 8;


        /// <summary>
        /// 
        /// </summary>
        const int UserProgramBanksCheckBoxesAmount = 8;

        
        /// <summary>
        /// 
        /// </summary>
        const int ExtendedUserProgramBanksCheckBoxesAmount = 8;


        /// <summary>
        /// 
        /// </summary>
        const int InternalProgramBanksCheckBoxesStart = 0;


        /// <summary>
        /// 
        /// </summary>
        const int ProgramBankGmCheckBox = InternalProgramBanksCheckBoxesStart + InternalProgramBanksCheckBoxesAmount;


        /// <summary>
        /// 
        /// </summary>
        private const int UserProgramBanksCheckBoxesStart = ProgramBankGmCheckBox + 1;


        /// <summary>
        /// 
        /// </summary>
        private const int ExtendedUserProgramBanksCheckBoxesStart =
            UserProgramBanksCheckBoxesStart + UserProgramBanksCheckBoxesAmount;


        /// <summary>
        /// 
        /// </summary>
        const int ProgramBanksVirtualCheckBox = ExtendedUserProgramBanksCheckBoxesStart + ExtendedUserProgramBanksCheckBoxesAmount;


        /// <summary>
        /// 
        /// </summary>
        const int InternalCombiBanksCheckBoxesStart = 0;


        /// <summary>
        /// 
        /// </summary>
        const int InternalCombiBanksCheckBoxesAmount = 8;


        /// <summary>
        /// 
        /// </summary>
        const int UserCombiBanksCheckBoxesStart = InternalCombiBanksCheckBoxesAmount;


        /// <summary>
        /// 
        /// </summary>
        const int UserCombiBanksCheckBoxesAmount = 8;


        /// <summary>
        /// 
        /// </summary>
        private const int CombiBanksVirtualCheckBox = UserCombiBanksCheckBoxesStart + UserCombiBanksCheckBoxesAmount;


        /// <summary>
        /// 
        /// </summary>
        readonly SaveFileDialog _saveDialog;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public ListGeneratorWindow(PcgMemory pcgMemory)
        {
            //Parent = mainWindow;
            _pcgMemory = pcgMemory;
            InitializeComponent();

            // Main type of list.
            radioButtonPatchList.IsChecked = true;

            InitBanks();

            if ((_pcgMemory.ProgramBanks == null) ||
                ((((_pcgMemory.CombiBanks == null) || (_pcgMemory.CombiBanks.CountWritablePatches == 0)) &&
                  (((_pcgMemory.SetLists == null)) || (_pcgMemory.SetLists.CountWritablePatches == 0)))))
            {
                radioButtonProgramUsageList.Visibility = Visibility.Collapsed;
            }

            // Save dialog.
            var folderName = $"{Settings.Default.Slg_DefaultOutputFolder}";
            if ((folderName == string.Empty) || !Directory.Exists(folderName))
            {
                folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var fileName =
                $@"{folderName}\{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}" +
                $@"_{Strings.OutputTxt}";

            _saveDialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = Strings.TxtExtension,

                FileName = textBoxOutputFile.Text = fileName,
                InitialDirectory = Path.GetDirectoryName(fileName),
                Filter = Strings.TextFilesFilter,
                Title = Strings.SelectWriteFile,
                ValidateNames = true
            };

            // Sorting.
            InitSorting();

            InitOutput(fileName);

            InitDrumKits();

            InitDrumPatterns();

            InitWaveSequences();

            InitFavoriteGroups();

            Width = Settings.Default.UI_ListGeneratorWindowWidth == 0
                ? 1000
                : Settings.Default.UI_ListGeneratorWindowWidth;
            Height = Settings.Default.UI_ListGeneratorWindowHeight == 0
                ? 800
                : Settings.Default.UI_ListGeneratorWindowHeight;
        }


        /// <summary>
        /// Initializes banks.
        /// </summary>
        private void InitBanks()
        {
            if (_pcgMemory.ProgramBanks != null)
            {
                InitPrograms();
            }

            if (_pcgMemory.CombiBanks != null)
            {
                InitCombiBanks();
            }

            if (_pcgMemory.SetLists != null)
            {
                InitSetLists();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitPrograms()
        {
            if (_pcgMemory.ProgramBanks == null)
            {
                checkBoxFilterProgramNames.Visibility = Visibility.Collapsed;
                groupBoxFilterProgramBanks.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetProgramBankButtons();
                checkBoxProgramBanksIgnoreInitPrograms.IsChecked = true;
                checkBoxIgnoreFirstProgram.IsEnabled = false;
            }
        }


        /// <summary>
        /// // Set lists. IMPR: Need to copy because cast from IBank to ISetList does not work.
        /// </summary>
        private void InitSetLists()
        {
            var setLists = new ObservableBankCollection<ISetList>();
            foreach (var bank in _pcgMemory.SetLists.BankCollection)
            {
                setLists.Add(bank as ISetList);
            }

            if (_pcgMemory.SetLists == null)
            {
                checkBoxDiffListIgnoreSetListSlotDescriptions.Visibility = Visibility.Collapsed;
                checkBoxFilterSetListSlotNames.Visibility = Visibility.Collapsed;
                checkBoxFilterSetListSlotDescriptions.Visibility = Visibility.Collapsed;
                groupBoxSetLists.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!(setLists.Any(setList => setList.IsFilled)))
                {
                    InitNoSetLists();
                }
                else
                {
                    checkBoxSetListsEnable.IsChecked = true;

                    _setListRangeMin = _pcgMemory.SetLists.BankCollection.Where(
                        setList => setList.IsFilled).Min(setList => Convert.ToInt16(setList.Id));
                    _setListRangeMax = _pcgMemory.SetLists.BankCollection.Where(
                        setList => setList.IsFilled).Max(setList => Convert.ToInt16(setList.Id));

                    textBoxSetListsFrom.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
                    textBoxSetListsTo.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
                }

                checkBoxSetListsIgnoreInitSetListSlots.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitNoSetLists()
        {
            groupBoxSetLists.Visibility = Visibility.Collapsed;
            checkBoxSetListsEnable.IsEnabled = false;
            labelSetListRange.IsEnabled = false;
            textBoxSetListsFrom.IsEnabled = false;
            labelSetListsRangeTo.IsEnabled = false;
            textBoxSetListsTo.IsEnabled = false;
            InitNoSetListsFilter();
            buttonSetListsSelectAll.IsEnabled = false;
        }

        
        ///
        private void InitNoSetListsFilter()
        {
            checkBoxFilterProgramNames.IsEnabled = false;
            checkBoxFilterCombiNames.IsEnabled = false;
            checkBoxFilterSetListSlotNames.IsEnabled = false;
            checkBoxFilterSetListSlotDescriptions.IsEnabled = false;
            checkBoxFilterWaveSequenceNames.IsEnabled = false;
            checkBoxFilterDrumKitNames.IsEnabled = false;
            checkBoxFilterDrumPatternNames.IsEnabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitCombiBanks()
        {
            if (_pcgMemory.CombiBanks == null)
            {
                radioButtonCombiContentList.Visibility = Visibility.Collapsed;
                checkBoxFilterCombiNames.Visibility = Visibility.Collapsed;
                groupBoxFilterCombiBanks.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetCombiBankButtons();
                checkBoxIgnoreMutedOffTimbres.IsChecked = true;
                checkBoxIgnoreMutedOffFirstProgramTimbre.IsChecked = true;
                checkBoxCombiBanksIgnoreInitCombis.IsChecked = true;
                checkBoxIgnoreMutedOffTimbres.IsEnabled = false;
                checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitSorting()
        {
            radioButtonCategorical.Visibility = (_pcgMemory.HasProgramCategories || _pcgMemory.HasCombiCategories)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitOutput(string fileName)
        {
            radioButtonTypeBankIndex.IsChecked = true;
            radioButtonAsciiTable.IsChecked = true;

            textBoxOutputFile.Text = fileName;
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitDrumKits()
        {
            if (_pcgMemory.DrumKitBanks == null)
            {
                groupBoxFilterDrumKits.Visibility = Visibility.Collapsed;
                checkBoxFilterDrumKitNames.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkBoxDrumKitsIgnoreInitDrumKits.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitDrumPatterns()
        {
            if (_pcgMemory.DrumPatternBanks == null)
            {
                groupBoxFilterDrumPatterns.Visibility = Visibility.Collapsed;
                checkBoxFilterDrumPatternNames.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkBoxDrumPatternsIgnoreInitDrumPatterns.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitWaveSequences()
        {
            if (_pcgMemory.WaveSequenceBanks == null)
            {
                groupBoxFilterWaveSequences.Visibility = Visibility.Collapsed;
                checkBoxFilterWaveSequenceNames.Visibility = Visibility.Collapsed;
            }
            else
            {
                checkBoxWaveSequencesIgnoreInitWaveSequences.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void InitFavoriteGroups()
        {
            if (_pcgMemory.AreFavoritesSupported)
            {
                threeStateCheckBoxFavorites.IsChecked = null;
            }
            else
            {
                groupBoxFilterOnFavorites.Visibility = Visibility.Collapsed;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PerformPatchListOptions();

            if (!_pcgMemory.UsesCategoriesAndSubCategories)
            {
                radioButtonCategorical.Content = Strings.GenreThanPatchName_control;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Settings.Default.UI_ListGeneratorWindowWidth = (int)(Width + 0.5);
            Settings.Default.UI_ListGeneratorWindowHeight = (int)(Height + 0.5);
            Settings.Default.Save();    
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonPatchListChecked(object sender, RoutedEventArgs e)
        {
            PerformPatchListOptions();
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptions()
        {
            UpdateSpecificListOptions();
            SetProgramBankButtons();
            SetCombiBankButtons();
            SetDrumKitBankButtons();
            SetDrumPatternBankButtons();
            SetWaveSequenceBankButtons();

            PerformPatchListOptionsPrograms();
            PerformPatchListOptionsCombis();
            PerformPatchListOptionsGroupBoxes();
            PerformPatchListOptionsDrumKits();
            PerformPatchListOptionsDrumPatterns();
            PerformPatchListOptionsWaveSequences();
            PerformPatchListOptionsOptionalColumns();
            PerformPatchListOptionsSorting();
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsPrograms()
        {
            checkBoxProgramBanksIgnoreInitPrograms.IsEnabled = true;
            checkBoxIgnoreFirstProgram.IsEnabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsCombis()
        {
            checkBoxCombiBanksIgnoreInitCombis.IsEnabled = true;
            checkBoxIgnoreMutedOffTimbres.IsEnabled = false;
            checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsGroupBoxes()
        {
            groupBoxFilterOnText.Visibility = Visibility.Visible;
            groupBoxFilterOnText.IsEnabled =
                groupBoxFilterOnText.IsExpanded = true;

            var checkFilterOnText = checkBoxFilterOnText.IsReallyChecked();
            textBoxTextToFilterOn.IsEnabled =
                checkBoxCaseSensitive.IsEnabled = checkFilterOnText;
            checkBoxSetListsIgnoreInitSetListSlots.IsEnabled = AreSetListsAvailable;

            PerformPatchListOptionsGroupBoxesFilters(checkFilterOnText, AreSetListsAvailable);
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkFilterOnText"></param>
        /// <param name="setListsAvailable"></param>
        private void PerformPatchListOptionsGroupBoxesFilters(bool checkFilterOnText, bool setListsAvailable)
        {
            checkBoxFilterProgramNames.IsEnabled =
                checkBoxFilterCombiNames.IsEnabled = 
                checkBoxFilterWaveSequenceNames.IsEnabled =
                checkBoxFilterDrumKitNames.IsEnabled =
                checkBoxFilterDrumPatternNames.IsEnabled =
                checkFilterOnText;

            checkBoxFilterSetListSlotNames.IsEnabled =
                checkBoxFilterSetListSlotDescriptions.IsEnabled = checkFilterOnText && setListsAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsDrumKits()
        {
            var drumKitsAvailable = ((_pcgMemory.DrumKitBanks != null) &&
                                     _pcgMemory.DrumKitBanks.BankCollection.Any(drumKitBank => drumKitBank.IsFilled));
            checkBoxDrumKitsIgnoreInitDrumKits.IsEnabled = drumKitsAvailable;
        }



        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsDrumPatterns()
        {
            var drumPatternsAvailable = ((_pcgMemory.DrumPatternBanks != null) &&
                                         _pcgMemory.DrumPatternBanks.BankCollection.Any(
                                             drumPatternBank => drumPatternBank.IsFilled));
            checkBoxDrumPatternsIgnoreInitDrumPatterns.IsEnabled = drumPatternsAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsWaveSequences()
        {
            var waveSequencesAvailable = ((_pcgMemory.WaveSequenceBanks != null) &&
                                          _pcgMemory.WaveSequenceBanks.BankCollection.Any(
                                              waveSequenceBank => waveSequenceBank.IsFilled));
            checkBoxWaveSequencesIgnoreInitWaveSequences.IsEnabled = waveSequencesAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsOptionalColumns()
        {
            checkBoxCrcIncludingName.IsChecked = false;
            checkBoxCrcExcludingName.IsChecked = false;
            checkBoxSetListSlotReferenceId.IsChecked = true;
            checkBoxSetListSlotReferenceName.IsChecked = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void PerformPatchListOptionsSorting()
        {
            groupBoxSorting.Visibility = Visibility.Visible;
            groupBoxSorting.IsEnabled = true;
            groupBoxSorting.IsExpanded = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonProgramUsageListChecked(object sender, RoutedEventArgs e)
        {
            UpdateSpecificListOptions();
            SetProgramBankButtons();
            SetCombiBankButtons();

            checkBoxProgramBanksIgnoreInitPrograms.IsEnabled = true;
            checkBoxIgnoreFirstProgram.IsEnabled = true;

            checkBoxCombiBanksIgnoreInitCombis.IsEnabled = true;
            checkBoxIgnoreMutedOffTimbres.IsEnabled = true;
            checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = true;

            UpdateFilterOptions(false);

            groupBoxSorting.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonCombiContentListChecked(object sender, RoutedEventArgs e)
        {
            UpdateSpecificListOptions();
            SetProgramBankButtons();
            SetCombiBankButtons();

            checkBoxProgramBanksIgnoreInitPrograms.IsEnabled = false;
            checkBoxIgnoreFirstProgram.IsEnabled = true;

            checkBoxCombiBanksIgnoreInitCombis.IsEnabled = true;
            checkBoxIgnoreMutedOffTimbres.IsEnabled = true;
            checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = true;
            
            var longList = (SelectedSubType == ListGenerator.SubType.Long);
            UpdateFilterOptions(longList);

            groupBoxSorting.Visibility = Visibility.Collapsed; // Always compact
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonDifferencesListChecked(object sender, RoutedEventArgs e)
        {
            UpdateSpecificListOptions();
            SetProgramBankButtons();
            SetCombiBankButtons();

            checkBoxProgramBanksIgnoreInitPrograms.IsEnabled = true;
            checkBoxIgnoreFirstProgram.IsEnabled = false;

            checkBoxCombiBanksIgnoreInitCombis.IsEnabled = true;
            checkBoxIgnoreMutedOffTimbres.IsEnabled = false;
            checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = false;

            UpdateFilterOptions(false);

            groupBoxSorting.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonFileContentListChecked(object sender, RoutedEventArgs e)
        {
            UpdateSpecificListOptions();
            SetProgramBankButtons();
            SetCombiBankButtons();
            groupBoxFilterWaveSequences.Visibility = Visibility.Collapsed;
            groupBoxFilterDrumKits.Visibility = Visibility.Collapsed;
            groupBoxFilterDrumPatterns.Visibility = Visibility.Collapsed;

            checkBoxProgramBanksIgnoreInitPrograms.IsEnabled = false;
            checkBoxIgnoreFirstProgram.IsEnabled = false;

            checkBoxCombiBanksIgnoreInitCombis.IsEnabled = false;
            checkBoxIgnoreMutedOffTimbres.IsEnabled = false;
            checkBoxIgnoreMutedOffFirstProgramTimbre.IsEnabled = false;
            
            UpdateFilterOptions(false);

            groupBoxSorting.Visibility = Visibility.Collapsed;
            groupBoxSorting.IsEnabled = false;
            groupBoxSorting.IsExpanded = false;
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateSpecificListOptions()
        {
            // Sub type
            var subTypeEnabled = (radioButtonCombiContentList.IsReallyChecked() ||
                                  radioButtonDifferencesList.IsReallyChecked());
            comboBoxListSubType.Items.Clear();
            comboBoxListSubType.IsEnabled = subTypeEnabled;
            labelListSubType.IsEnabled = subTypeEnabled;

            if (radioButtonCombiContentList.IsReallyChecked())
            {
                comboBoxListSubType.Items.Add(new ComboBoxItem
                {
                    Content = Strings.CompactList,
                    Tag = ListGenerator.SubType.Compact
                });

                comboBoxListSubType.Items.Add(new ComboBoxItem
                {
                    Content = Strings.ShortList,
                    Tag = ListGenerator.SubType.Short
                });

                comboBoxListSubType.Items.Add(new ComboBoxItem
                {
                    Content = Strings.LongList,
                    Tag = ListGenerator.SubType.Long
                });
                
                comboBoxListSubType.SelectedIndex = 0; // Compact
            }
            else if (radioButtonDifferencesList.IsReallyChecked())
            {
                comboBoxListSubType.Items.Add(new ComboBoxItem
                {
                    Content = Strings.IncludingPatchName,
                    Tag = ListGenerator.SubType.IncludingPatchName
                });

                comboBoxListSubType.Items.Add(new ComboBoxItem
                {
                    Content = Strings.ExcludingPatchName,
                    Tag = ListGenerator.SubType.ExcludingPatchName
                });

                comboBoxListSubType.SelectedIndex = 0; // Without patch name     
            }
            
            // Update Differences List options.
            UpdateDifferencesListOptions();
            
            // Optional columns.
            groupBoxOptionalColumns.Visibility = radioButtonPatchList.IsReallyChecked()
                                         ? Visibility.Visible : Visibility.Collapsed;

            UpdateSetListSlots();

            UpdateOutputOptions();
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateDifferencesListOptions()
        {
            var show = radioButtonDifferencesList.IsReallyChecked();
            groupBoxDifferencesListOptions.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            groupBoxDifferencesListOptions.IsEnabled = show;
            groupBoxDifferencesListOptions.IsExpanded = radioButtonDifferencesList.IsReallyChecked();
            sliderDiffListMaxNumberOfDifferences.Value = 5;
            checkBoxDiffListIgnorePatchNames.IsChecked = true;
            checkBoxDiffListIgnoreSetListSlotDescriptions.IsChecked = true;
        }


        /// <summary>
        /// Options unavailable for Files Bank List
        /// </summary>
        private void UpdateSetListSlots()
        {
            checkBoxSetListsEnable.IsEnabled = !radioButtonFileContentList.IsReallyChecked();
            textBoxSetListsFrom.IsEnabled = !radioButtonFileContentList.IsReallyChecked();
            textBoxSetListsTo.IsEnabled = !radioButtonFileContentList.IsReallyChecked();
            threeStateCheckBoxFavorites.IsEnabled = !radioButtonFileContentList.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        private void UpdateFilterOptions(bool enable)
        {
            groupBoxFilterOnText.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterOnText.IsEnabled =
                groupBoxFilterOnText.IsExpanded = enable;
            UpdatePatchFilterOptions(enable);
            UpdateSortingFilterOptions(enable);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        private void UpdatePatchFilterOptions(bool enable)
        {
            checkBoxFilterProgramNames.IsEnabled =
                checkBoxFilterCombiNames.IsEnabled =
                    checkBoxFilterSetListSlotNames.IsEnabled =
                        checkBoxFilterSetListSlotDescriptions.IsEnabled =
                            checkBoxFilterWaveSequenceNames.IsEnabled =
                                checkBoxFilterDrumKitNames.IsEnabled = 
                                    checkBoxFilterDrumPatternNames.IsEnabled = enable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        private void UpdateSortingFilterOptions(bool enable)
        {
            groupBoxSorting.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
            groupBoxSorting.IsEnabled = enable;
            groupBoxSorting.IsExpanded = enable;
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateOutputOptions()
        {
            // Ascii is always enabled.
            var filter = !LongCombiContentListSelected;
            radioButtonText.IsEnabled = filter;
            radioButtonCsv.IsEnabled = filter;
            radioButtonXml.IsEnabled = (!radioButtonDifferencesList.IsReallyChecked() && filter);

            SelectHighestEnabledRadioButton();
        }


        /// <summary>
        /// 
        /// </summary>
        private bool LongCombiContentListSelected => (radioButtonCombiContentList.IsReallyChecked() &&
                                                      (comboBoxListSubType.SelectedItem != null) && 
                                                      (SelectedSubType == ListGenerator.SubType.Long));


        /// <summary>
        /// Select only the highest enabled radio button.
        /// </summary>
        private void SelectHighestEnabledRadioButton()
        {
            RadioButton button = null;

            if (XmlButtonCheckedButDisabled)
            {
                button = radioButtonCsv;
            }

            if (CsvRadioButtonCheckedButDisabled(button))
            {
                button = radioButtonText;
            }

            if (TextRadioButtonCheckedButDisabled(button))
            {
                button = radioButtonAsciiTable;
            }

            if (button != null)
            {
                button.IsChecked = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool TextRadioButtonCheckedButDisabled(RadioButton button)
        {
            return ((Equals(button, radioButtonText)) || radioButtonText.IsReallyChecked()) && 
                   (radioButtonText != null) && !radioButtonText.IsEnabled;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private bool CsvRadioButtonCheckedButDisabled(RadioButton button)
        {
            return radioButtonCsv != null && (((Equals(button, radioButtonCsv)) || radioButtonCsv.IsReallyChecked()) && 
                                              (radioButtonCsv != null) && !radioButtonCsv.IsEnabled);
        }


        /// <summary>
        /// 
        /// </summary>
        private bool XmlButtonCheckedButDisabled => radioButtonXml.IsReallyChecked() && !radioButtonXml.IsEnabled;


        /// <summary>
        /// 
        /// </summary>
        private ListGenerator.SubType SelectedSubType => (ListGenerator.SubType) ((ComboBoxItem) comboBoxListSubType.SelectedItem).Tag;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckBoxFilterOnTextChecked(object sender, RoutedEventArgs e)
        {
            var filterEnabled = checkBoxFilterOnText.IsReallyChecked();
            textBoxTextToFilterOn.IsEnabled = filterEnabled;
            
            checkBoxCaseSensitive.IsEnabled = filterEnabled;
            CheckBoxFilterOnTextCheckedPrograms(filterEnabled);
            CheckBoxFilterOnTextCheckedCombis(filterEnabled);

            CheckBoxFilterOnTextCheckedSetLists(filterEnabled);

            CheckBoxFilterOnTextCheckedWaveSequences(filterEnabled);

            CheckBoxFilterOnTextCheckedDrumKits(filterEnabled);

            CheckBoxFilterOnTextCheckedDrumPatterns(filterEnabled);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedPrograms(bool filterEnabled)
        {
            checkBoxFilterProgramNames.IsEnabled = filterEnabled;
            checkBoxFilterProgramNames.IsChecked = filterEnabled;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedCombis(bool filterEnabled)
        {
            checkBoxFilterCombiNames.IsEnabled = filterEnabled;
            checkBoxFilterCombiNames.IsChecked = filterEnabled;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedSetLists(bool filterEnabled)
        {
            var setListsAvailable = AreSetListsAvailable;
            var filter = filterEnabled && setListsAvailable;
            checkBoxFilterSetListSlotNames.IsEnabled = filter;
            checkBoxFilterSetListSlotNames.IsChecked = filter;
            checkBoxFilterSetListSlotDescriptions.IsEnabled = filter;
            checkBoxFilterSetListSlotDescriptions.IsChecked = filter;
        }


        /// <summary>
        /// 
        /// </summary>
        private bool AreSetListsAvailable
        {
            get
            {
                return (_pcgMemory.SetLists != null) &&
                       _pcgMemory.SetLists.BankCollection.Any(setList => setList.IsFilled);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedWaveSequences(bool filterEnabled)
        {
            var waveSequencesAvailable = (_pcgMemory.WaveSequenceBanks != null) &&
                                         _pcgMemory.WaveSequenceBanks.BankCollection.Any(
                                             waveSequenceBank => waveSequenceBank.IsFilled);
            checkBoxFilterWaveSequenceNames.IsEnabled = filterEnabled && waveSequencesAvailable;
            checkBoxFilterWaveSequenceNames.IsChecked = filterEnabled && waveSequencesAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedDrumKits(bool filterEnabled)
        {
            var drumKitsAvailable = (_pcgMemory.DrumKitBanks != null) &&
                                    _pcgMemory.DrumKitBanks.BankCollection.Any(drumKitBanks => drumKitBanks.IsFilled);
            checkBoxFilterDrumKitNames.IsEnabled = filterEnabled && drumKitsAvailable;
            checkBoxFilterDrumKitNames.IsChecked = filterEnabled && drumKitsAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterEnabled"></param>
        private void CheckBoxFilterOnTextCheckedDrumPatterns(bool filterEnabled)
        {
            var drumPatternsAvailable = (_pcgMemory.DrumPatternBanks != null) &&
                                        _pcgMemory.DrumPatternBanks.BankCollection.Any(
                                            drumPatternBanks => drumPatternBanks.IsFilled);
            checkBoxFilterDrumPatternNames.IsEnabled = filterEnabled && drumPatternsAvailable;
            checkBoxFilterDrumPatternNames.IsChecked = filterEnabled && drumPatternsAvailable;
        }


        /// <summary>
        /// 
        /// </summary>
        void SetProgramBankButtons()
        {
            var show = !radioButtonFileContentList.IsReallyChecked();
            groupBoxFilterProgramBanks.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterProgramBanks.IsEnabled = show;
            groupBoxFilterProgramBanks.IsExpanded = show;

            AssignProgramBankButtons();

            var internalBanks = 0;
            var userBanks = 0;
            var extendedUserBanks = 0;

            CollapseProgramBankButtons();

            foreach (var bank in _pcgMemory.ProgramBanks.BankCollection)
            {
                var checkBox = SetProgramBankIsEnabledAndIsChecked(bank, ref internalBanks, ref userBanks, ref extendedUserBanks);

                SetProgramBankContentTagAndVisibility(checkBox, bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CollapseProgramBankButtons()
        {
            foreach (var button in _programBankButtons)
            {
                button.Visibility = Visibility.Collapsed;
                button.IsChecked = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void AssignProgramBankButtons()
        {
            _programBankButtons = new List<CheckBox>
            {
                // Internal, User, User Extended, GM, Virtual banks
                checkBoxProgramBankIa,
                checkBoxProgramBankIb,
                checkBoxProgramBankIc,
                checkBoxProgramBankId,
                checkBoxProgramBankIe,
                checkBoxProgramBankIf,
                checkBoxProgramBankIg,
                checkBoxProgramBankIh,
                checkBoxProgramBankGm,
                checkBoxProgramBankUa,
                checkBoxProgramBankUb,
                checkBoxProgramBankUc,
                checkBoxProgramBankUd,
                checkBoxProgramBankUe,
                checkBoxProgramBankUf,
                checkBoxProgramBankUg,
                checkBoxProgramBankUh,
                checkBoxProgramBankUaa,
                checkBoxProgramBankUbb,
                checkBoxProgramBankUcc,
                checkBoxProgramBankUdd,
                checkBoxProgramBankUee,
                checkBoxProgramBankUff,
                checkBoxProgramBankUgg,
                checkBoxProgramBankUhh,
                checkBoxProgramBanksVirtual
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="internalBanks"></param>
        /// <param name="userBanks"></param>
        /// <param name="extendedUserBanks"></param>
        /// <returns></returns>
        private CheckBox SetProgramBankIsEnabledAndIsChecked(IBank bank, ref int internalBanks, ref int userBanks,
            ref int extendedUserBanks)
        {
            CheckBox checkBox;

            switch (bank.Type)
            {
                case BankType.EType.Int:
                    checkBox = SetIntProgramBankIsEnabledAndIsChecked(bank, ref internalBanks);
                    break;

                case BankType.EType.User:
                    checkBox = SetUserProgramBankIsEnabledAndIsChecked(bank, ref userBanks);
                    break;

                case BankType.EType.UserExtended:
                    checkBox = SetUserExtendedProgramBankIsEnabledAndIsChecked(bank, ref extendedUserBanks);
                    break;

                case BankType.EType.Gm:
                    checkBox = SetGmProgramBankIsEnabledAndIsChecked();
                    break;

                case BankType.EType.Virtual:
                    checkBox = SetVirtualProgramBankIsEnabledAndIsChecked(bank);
                    break;

                default:
                    throw new ApplicationException("Illegal switch");
            }

            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="internalBanks"></param>
        /// <returns></returns>
        private CheckBox SetIntProgramBankIsEnabledAndIsChecked(ILoadable bank, ref int internalBanks)
        {
            var checkBox = _programBankButtons[InternalProgramBanksCheckBoxesStart + internalBanks];
            checkBox.IsEnabled = bank.IsLoaded ||
                                 (radioButtonProgramUsageList.IsReallyChecked() &&
                                  !radioButtonFileContentList.IsReallyChecked()) ||
                                 radioButtonCombiContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            internalBanks++;
            Debug.Assert(internalBanks <= InternalProgramBanksCheckBoxesAmount);
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="userBanks"></param>
        /// <returns></returns>
        private CheckBox SetUserProgramBankIsEnabledAndIsChecked(ILoadable bank, ref int userBanks)
        {
            var checkBox = _programBankButtons[UserProgramBanksCheckBoxesStart + userBanks];
            checkBox.IsEnabled = bank.IsLoaded ||
                                 (radioButtonProgramUsageList.IsReallyChecked() &&
                                  !radioButtonFileContentList.IsReallyChecked()) ||
                                 radioButtonCombiContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            userBanks++;
            Debug.Assert(userBanks <= UserProgramBanksCheckBoxesAmount);
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="extendedUserBanks"></param>
        /// <returns></returns>
        private CheckBox SetUserExtendedProgramBankIsEnabledAndIsChecked(ILoadable bank, ref int extendedUserBanks)
        {
            var checkBox = _programBankButtons[ExtendedUserProgramBanksCheckBoxesStart + extendedUserBanks];
            checkBox.IsEnabled = bank.IsLoaded ||
                                 (radioButtonProgramUsageList.IsReallyChecked()) &&
                                 !radioButtonFileContentList.IsReallyChecked() ||
                                 radioButtonCombiContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            extendedUserBanks++;
            Debug.Assert(extendedUserBanks <= ExtendedUserProgramBanksCheckBoxesAmount);
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private CheckBox SetGmProgramBankIsEnabledAndIsChecked()
        {
            var checkBox = _programBankButtons[ProgramBankGmCheckBox];
            checkBox.IsEnabled = !radioButtonDifferencesList.IsReallyChecked() &&
                                 !radioButtonFileContentList.IsReallyChecked();
            checkBox.IsChecked = radioButtonProgramUsageList.IsReallyChecked() ||
                                 radioButtonCombiContentList.IsReallyChecked();
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        private CheckBox SetVirtualProgramBankIsEnabledAndIsChecked(IBank bank)
        {
            var checkBox = _programBankButtons[ProgramBanksVirtualCheckBox];
            checkBox.IsEnabled = checkBox.IsEnabled || bank.IsLoaded ||
                                 (radioButtonProgramUsageList.IsReallyChecked() &&
                                  !radioButtonFileContentList.IsReallyChecked()) ||
                                 radioButtonCombiContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkBox"></param>
        /// <param name="bank"></param>
        private static void SetProgramBankContentTagAndVisibility(CheckBox checkBox, IBank bank)
        {
            if (checkBox != null)
            {
                // Write the name of the program bank in the check box (unless it is a virtual bank).
                if (bank.Type != BankType.EType.Virtual)
                {
                    checkBox.Content = bank.Id;
                }

                // Set the tag to the bank.
                checkBox.Tag = bank;

                // Make the program bank visible if it is enabled.
                if (checkBox.IsEnabled)
                {
                    checkBox.Visibility = Visibility.Visible;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonProgramBanksSelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _programBankButtons)
            {
                if (checkBox.IsEnabled)
                {
                    checkBox.IsChecked = checkBox.IsEnabled;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonProgramBanksDeselectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _programBankButtons)
            {
                if (checkBox.IsEnabled)
                {
                    checkBox.IsChecked = false;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void SetCombiBankButtons()
        {
            var show = !radioButtonFileContentList.IsReallyChecked();
            groupBoxFilterCombiBanks.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterCombiBanks.IsEnabled = show;
            groupBoxFilterCombiBanks.IsExpanded = show;

            AssignCombiBankButtons();

            if (_pcgMemory.CombiBanks == null)
            {
                DisableCombiBankButtons();
            }
            else
            {
                var internalBanks = 0;
                var userBanks = 0;

                CollapseCombiBankButtons();

                foreach (var bank in _pcgMemory.CombiBanks.BankCollection)
                {
                    var checkBox = SetCombiBankIsEnabledAndIsChecked(bank, ref internalBanks, ref userBanks);

                    SetCombiBankContentTagAndVisibility(checkBox, bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CollapseCombiBankButtons()
        {
            foreach (var button in _combiBankButtons)
            {
                button.Visibility = Visibility.Collapsed;
                button.IsChecked = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void AssignCombiBankButtons()
        {
            _combiBankButtons = new List<CheckBox>
            {
                // Internal, User.
                checkBoxCombiBankIa,
                checkBoxCombiBankIb,
                checkBoxCombiBankIc,
                checkBoxCombiBankId,
                checkBoxCombiBankIe,
                checkBoxCombiBankIf,
                checkBoxCombiBankIg,
                checkBoxCombiBankIh,
                checkBoxCombiBankUa,
                checkBoxCombiBankUb,
                checkBoxCombiBankUc,
                checkBoxCombiBankUd,
                checkBoxCombiBankUe,
                checkBoxCombiBankUf,
                checkBoxCombiBankUg,
                checkBoxCombiBankUh,
                checkBoxCombiBanksVirtual
            };
        }


        /// <summary>
        /// 
        /// </summary>
        private void DisableCombiBankButtons()
        {
            foreach (var button in _combiBankButtons)
            {
                button.Visibility = Visibility.Collapsed;
                button.IsChecked = false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="internalBanks"></param>
        /// <param name="userBanks"></param>
        /// <returns></returns>
        private CheckBox SetCombiBankIsEnabledAndIsChecked(IBank bank, ref int internalBanks, ref int userBanks)
        {
            CheckBox checkBox;

            switch (bank.Type)
            {
                case BankType.EType.Int:
                    checkBox = SetIntCombiBankIsEnabledAndIsChecked(bank, ref internalBanks);
                    break;

                case BankType.EType.User:
                    checkBox = SetUserCombiBankIsEnabledAndIsChecked(bank, ref userBanks);
                    break;

                case BankType.EType.Virtual:
                    checkBox = SetVirtualCombiBankIsEnabledAndIsChecked(bank);
                    break;

                default:
                    throw new ApplicationException("Illegal switch");
            }
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="internalBanks"></param>
        /// <returns></returns>
        private CheckBox SetIntCombiBankIsEnabledAndIsChecked(IBank bank, ref int internalBanks)
        {
            var checkBox = _combiBankButtons[InternalCombiBanksCheckBoxesStart + internalBanks];
            checkBox.IsEnabled = bank.IsLoaded && !radioButtonFileContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            internalBanks++;
            Debug.Assert(internalBanks <= InternalCombiBanksCheckBoxesAmount);
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="userBanks"></param>
        /// <returns></returns>
        private CheckBox SetUserCombiBankIsEnabledAndIsChecked(IBank bank, ref int userBanks)
        {
            var checkBox = _combiBankButtons[UserCombiBanksCheckBoxesStart + userBanks];
            checkBox.IsEnabled = bank.IsLoaded && !radioButtonFileContentList.IsReallyChecked();
            checkBox.IsChecked = checkBox.IsEnabled;
            userBanks++;
            Debug.Assert(userBanks <= UserCombiBanksCheckBoxesAmount);
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        private CheckBox SetVirtualCombiBankIsEnabledAndIsChecked(IBank bank)
        {
            var checkBox = _combiBankButtons[CombiBanksVirtualCheckBox];
            checkBox.IsEnabled = checkBox.IsEnabled ||
                                 (bank.IsLoaded && !radioButtonFileContentList.IsReallyChecked());
            checkBox.IsChecked = checkBox.IsEnabled;
            return checkBox;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkBox"></param>
        /// <param name="bank"></param>
        private static void SetCombiBankContentTagAndVisibility(ContentControl checkBox, IBank bank)
        {
            if (checkBox != null)
            {
                // Write the name of the program bank in the check box (unless it is a virtual bank).
                if (bank.Type != BankType.EType.Virtual)
                {
                    checkBox.Content = bank.Id;
                }

                // Set the tag to the bank.
                checkBox.Tag = bank;

                // Make the program bank visible if it is enabled.
                if (checkBox.IsEnabled)
                {
                    checkBox.Visibility = Visibility.Visible;
                }
            }
        }


        /// <summary>
        /// FUTURE: Enable for other list types.
        /// </summary>
        void SetDrumKitBankButtons()
        {
            var isSupported = radioButtonPatchList.IsReallyChecked() &&
                            (_pcgMemory.DrumKitBanks != null);

            SetDrumKitBankButtonsCheckBoxGroupBox(isSupported);
            SetDrumKitBankButtonsCheckBox(isSupported);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetDrumKitBankButtonsCheckBoxGroupBox(bool isSupported)
        {
            groupBoxFilterDrumKits.Visibility = isSupported ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterDrumKits.IsEnabled = isSupported;
            groupBoxFilterDrumKits.IsExpanded = isSupported;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetDrumKitBankButtonsCheckBox(bool isSupported)
        {
            checkBoxFilterDrumKits.IsEnabled = isSupported;
            checkBoxFilterDrumKits.IsChecked = isSupported;
        }


        /// <summary>
        /// FUTURE: Enable for other list types.
        /// </summary>
        void SetDrumPatternBankButtons()
        {
            var isSupported = radioButtonPatchList.IsReallyChecked() &&
                            (_pcgMemory.DrumPatternBanks != null);

            SetDrumPatternBankButtonsCheckBoxGroupBox(isSupported);
            SetDrumPatternBankButtonsCheckBox(isSupported);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetDrumPatternBankButtonsCheckBoxGroupBox(bool isSupported)
        {
            groupBoxFilterDrumPatterns.Visibility = isSupported ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterDrumPatterns.IsEnabled = isSupported;
            groupBoxFilterDrumPatterns.IsExpanded = isSupported;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetDrumPatternBankButtonsCheckBox(bool isSupported)
        {
            checkBoxFilterDrumPatterns.IsEnabled = isSupported;
            checkBoxFilterDrumPatterns.IsChecked = isSupported;
        }


        /// <summary>
        /// FUTURE: Enable for other list types.
        /// </summary>
        void SetWaveSequenceBankButtons()
        {
            var isSupported = radioButtonPatchList.IsReallyChecked() &&
                              (_pcgMemory.WaveSequenceBanks != null);
            
            SetWaveSequenceBankButtonsGroupBox(isSupported);
            SetWaveSequenceBankButtonsCheckBox(isSupported);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetWaveSequenceBankButtonsGroupBox(bool isSupported)
        {
            groupBoxFilterWaveSequences.Visibility = isSupported ? Visibility.Visible : Visibility.Collapsed;
            groupBoxFilterWaveSequences.IsEnabled = isSupported;
            groupBoxFilterWaveSequences.IsExpanded = isSupported;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSupported"></param>
        private void SetWaveSequenceBankButtonsCheckBox(bool isSupported)
        {
            checkBoxFilterWaveSequences.IsEnabled = isSupported;
            checkBoxFilterWaveSequences.IsChecked = isSupported;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCombiBanksSelectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _combiBankButtons)
            {
                checkBox.IsChecked = checkBox.IsEnabled;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCombiBanksDeselectAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var checkBox in _combiBankButtons)
            {
                checkBox.IsChecked = false;
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxSetListsEnableCheckedChanged(object sender, RoutedEventArgs e)
        {
            var enabled = checkBoxSetListsEnable.IsReallyChecked();
            labelSetListRange.IsEnabled = enabled;
            textBoxSetListsFrom.IsEnabled = enabled;
            labelSetListsRangeTo.IsEnabled = enabled;
            textBoxSetListsTo.IsEnabled = enabled;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSetListsSelectAllClick(object sender, RoutedEventArgs e)
        {
            checkBoxSetListsEnable.IsChecked = true;
            textBoxSetListsFrom.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
            textBoxSetListsTo.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSetListsFromLostFocus(object sender, RoutedEventArgs e)
        {
            int fromValue;
            if (Int32.TryParse(textBoxSetListsFrom.Text, out fromValue))
            {
                int toValue;
                if (Int32.TryParse(textBoxSetListsTo.Text, out toValue))
                {
                    if (fromValue > toValue)
                    {
                        textBoxSetListsTo.Text = textBoxSetListsFrom.Text;
                    }
                }
                else
                {
                    textBoxSetListsTo.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                textBoxSetListsFrom.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
            }

            LimitRanges();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSetListsToLostFocus(object sender, RoutedEventArgs e)
        {
            int toValue;
            if (Int32.TryParse(textBoxSetListsTo.Text, out toValue))
            {
                int fromValue;
                if (Int32.TryParse(textBoxSetListsFrom.Text, out fromValue))
                {
                    if (fromValue > toValue)
                    {
                        textBoxSetListsFrom.Text = textBoxSetListsTo.Text;
                    }
                }
                else
                {
                    textBoxSetListsFrom.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                textBoxSetListsTo.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
            }

            LimitRanges();
        }


        /// <summary>
        /// 
        /// </summary>
        private void LimitRanges()
        {
            var fromValue = Int32.Parse(textBoxSetListsFrom.Text);
            if (fromValue < 0)
            {
                textBoxSetListsFrom.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
            }
            else if (fromValue > 127)
            {
                textBoxSetListsFrom.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
            }
        
            var toValue = Int32.Parse(textBoxSetListsTo.Text);
            if (toValue < 0)
            {
                textBoxSetListsTo.Text = _setListRangeMin.ToString(CultureInfo.InvariantCulture);
            }
            else if (toValue > 127)
            {
                textBoxSetListsTo.Text = _setListRangeMax.ToString(CultureInfo.InvariantCulture);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGenerateClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var askForContinuation = false; // Long time action

                Mouse.OverrideCursor = Cursors.Wait;
                var generator = CreateGenerator(ref askForContinuation);

                // Ask when a long time action is requested, for continuation.
                if (askForContinuation)
                {
                    if (MessageBox.Show(this, Strings.LongListGenerationWarning, Strings.PcgTools,
                        MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }

                SetGeneratorSubTypeProperty(generator);

                generator.PcgMemory = _pcgMemory;

                SetGeneratorFilterProperties(generator);
                SetGeneratorProgramParameters(generator);
                SetGeneratorCombiParameters(generator);
                SetGeneratorSetListProperties(generator);
                SetGeneratorDrumKitProperties(generator);
                SetGeneratorDrumPatternProperties(generator);
                SetGeneratorWaveSequenceProperties(generator);
                SetGeneratorOptionalColumnProperties(generator);
                SetGeneratorSortProperties(generator);
                SetGeneratorOutputFormatProperties(generator);
                SetGeneratorOutputProperties(generator);
                SetGeneratorFavoriteProperties(generator);

                var fileName = generator.Run();
                if (fileName != null)
                {
                    Process.Start(fileName);
                }
            }
            catch (UnauthorizedAccessException ex) 
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show(this,
                    $"{Strings.ErrorOccurred}: \n\n{Strings.Message}: {ex.Message}\n\n{Strings.InnerExceptionMessage}: {(ex.InnerException == null ? string.Empty : ex.InnerException.Message)}\n\n{Strings.StackTrace}: {ex.StackTrace}", 
                    Strings.PcgTools, MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorOptionalColumnProperties(ListGenerator generator)
        {
            generator.OptionalColumnCrcIncludingName = checkBoxCrcIncludingName.IsReallyChecked();
            generator.OptionalColumnCrcExcludingName = checkBoxCrcExcludingName.IsReallyChecked();
            generator.OptionalColumnSetListSlotReferenceId = checkBoxSetListSlotReferenceId.IsReallyChecked();
            generator.OptionalColumnSetListSlotReferenceName = checkBoxSetListSlotReferenceName.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorSubTypeProperty(ListGenerator generator)
        {
            if (comboBoxListSubType.SelectedItem != null)
            {
                switch (SelectedSubType)
                {
                    case ListGenerator.SubType.Compact:
                        generator.ListSubType = ListGenerator.SubType.Compact;
                        break;

                    case ListGenerator.SubType.Short:
                        generator.ListSubType = ListGenerator.SubType.Short;
                        break;

                    case ListGenerator.SubType.Long:
                        generator.ListSubType = ListGenerator.SubType.Long;
                        break;

                    case ListGenerator.SubType.IncludingPatchName:
                        generator.ListSubType = ListGenerator.SubType.IncludingPatchName;
                        break;

                    case ListGenerator.SubType.ExcludingPatchName:
                        generator.ListSubType = ListGenerator.SubType.ExcludingPatchName;
                        break;

                    default:
                        throw new ApplicationException("Illegal sub type");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="askForContinuation"></param>
        /// <returns></returns>
        private ListGenerator CreateGenerator(ref bool askForContinuation)
        {
            ListGenerator generator;
            if (radioButtonPatchList.IsReallyChecked())
            {
                generator = new ListGeneratorPatchList();
            }
            else if (radioButtonProgramUsageList.IsReallyChecked())
            {
                generator = new ListGeneratorProgramUsageList();
            }
            else if (radioButtonCombiContentList.IsReallyChecked())
            {
                generator = new ListGeneratorCombiContentList();
            }
            else if (radioButtonDifferencesList.IsReallyChecked())
            {
                generator = new ListGeneratorDifferencesList((int)(sliderDiffListMaxNumberOfDifferences.Value),
                    checkBoxDiffListIgnorePatchNames.IsReallyChecked(),
                    checkBoxDiffListIgnoreSetListSlotDescriptions.IsReallyChecked(),
                    checkBoxDiffListSearchBothDirections.IsReallyChecked());
                askForContinuation = true;
            }
            else if (radioButtonFileContentList.IsReallyChecked())
            {
                generator = new ListGeneratorFileContentList();
            }
            else
            {
                throw new ApplicationException("Wrong list type");
            }

            return generator;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorFilterProperties(ListGenerator generator)
        {
            generator.FilterOnText = checkBoxFilterOnText.IsReallyChecked();
            generator.FilterCaseSensitive = checkBoxCaseSensitive.IsReallyChecked();
            generator.FilterText = textBoxTextToFilterOn.Text;
            generator.FilterProgramNames = checkBoxFilterProgramNames.IsReallyChecked();
            generator.FilterCombiNames = checkBoxFilterProgramNames.IsReallyChecked();
            generator.FilterSetListSlotNames = checkBoxFilterSetListSlotDescriptions.IsReallyChecked();
            generator.FilterSetListSlotDescription = checkBoxFilterSetListSlotDescriptions.IsReallyChecked();
            generator.FilterWaveSequenceNames = checkBoxFilterWaveSequenceNames.IsReallyChecked();
            generator.FilterDrumKitNames = checkBoxFilterDrumKitNames.IsReallyChecked();
            generator.FilterDrumPatternNames = checkBoxFilterDrumPatternNames.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorProgramParameters(IListGenerator generator)
        {
            foreach (var checkBox in _programBankButtons.Where(checkBox => checkBox.IsReallyChecked()))
            {
                if (checkBox.Equals(_programBankButtons[ProgramBanksVirtualCheckBox]))
                {
                    foreach (var programBank in _pcgMemory.ProgramBanks.BankCollection.Where(
                        bank => bank.Type == BankType.EType.Virtual))
                    {
                        var bank = (IProgramBank) programBank;
                        generator.SelectedProgramBanks.Add(bank);
                    }
                }
                else
                {
                    generator.SelectedProgramBanks.Add(checkBox.Tag as IProgramBank);
                }
            }

            generator.IgnoreInitPrograms = checkBoxProgramBanksIgnoreInitPrograms.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorCombiParameters(IListGenerator generator)
        {
            foreach (var checkBox in _combiBankButtons.Where(checkBox => checkBox.IsReallyChecked()))
            {
                if (checkBox.Equals(_combiBankButtons[CombiBanksVirtualCheckBox]))
                {
                    foreach (var bank in _pcgMemory.CombiBanks.BankCollection.Where(
                        bank => bank.Type == BankType.EType.Virtual).Cast<ICombiBank>())
                    {
                        generator.SelectedCombiBanks.Add(bank);
                    }
                }
                else
                {
                    generator.SelectedCombiBanks.Add(checkBox.Tag as ICombiBank);
                }
            }

            generator.IgnoreInitCombis = checkBoxCombiBanksIgnoreInitCombis.IsReallyChecked();
            generator.IgnoreFirstProgram = checkBoxIgnoreFirstProgram.IsReallyChecked();
            generator.IgnoreMutedOffTimbres = checkBoxIgnoreMutedOffTimbres.IsReallyChecked();
            generator.IgnoreMutedOffFirstProgramTimbre = checkBoxIgnoreMutedOffFirstProgramTimbre.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorSetListProperties(IListGenerator generator)
        {
            generator.SetListsEnabled = checkBoxSetListsEnable.IsReallyChecked();
            generator.IgnoreInitSetListSlots = checkBoxSetListsIgnoreInitSetListSlots.IsReallyChecked();
            if (generator.SetListsEnabled)
            {
                generator.SetListsRangeFrom = (Int32.Parse(textBoxSetListsFrom.Text));
                generator.SetListsRangeTo = (Int32.Parse(textBoxSetListsTo.Text));
            }
        }


        /// <summary>
        /// FUTURE: Change if check boxes are used for drum kit banks.
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorDrumKitProperties(IListGenerator generator)
        {
            generator.DrumKitsEnabled = checkBoxFilterDrumKits.IsReallyChecked();
            generator.IgnoreInitDrumKits = checkBoxDrumKitsIgnoreInitDrumKits.IsReallyChecked();
            if (checkBoxFilterDrumKits.IsReallyChecked())
            {
                foreach (var bank1 in _pcgMemory.DrumKitBanks.BankCollection)
                {
                    var bank = (IDrumKitBank) bank1;
                    generator.SelectedDrumKitBanks.Add(bank);
                }
            }
        }


        /// <summary>
        /// FUTURE: Change if check boxes are used for drum Pattern banks.
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorDrumPatternProperties(IListGenerator generator)
        {
            generator.DrumPatternsEnabled = checkBoxFilterDrumPatterns.IsReallyChecked();
            generator.IgnoreInitDrumPatterns = checkBoxDrumPatternsIgnoreInitDrumPatterns.IsReallyChecked();
            if (checkBoxFilterDrumPatterns.IsReallyChecked())
            {
                foreach (var bank1 in _pcgMemory.DrumPatternBanks.BankCollection)
                {
                    var bank = (IDrumPatternBank)bank1;
                    generator.SelectedDrumPatternBanks.Add(bank);
                }
            }
        }


        /// <summary>
        /// FUTURE: Change if check boxes are used for wave sequence banks.
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorWaveSequenceProperties(IListGenerator generator)
        {
            generator.WaveSequencesEnabled = checkBoxFilterWaveSequences.IsReallyChecked();
            generator.IgnoreInitWaveSequences = checkBoxWaveSequencesIgnoreInitWaveSequences.IsReallyChecked();
            if (checkBoxFilterWaveSequences.IsReallyChecked())
            {
                foreach (var bank1 in _pcgMemory.WaveSequenceBanks.BankCollection)
                {
                    var bank = (IWaveSequenceBank) bank1;
                    generator.SelectedWaveSequenceBanks.Add(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorOutputProperties(ListGenerator generator)
        {
            generator.OutputFileName = textBoxOutputFile.Text;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorSortProperties(ListGenerator generator)
        {
            if (radioButtonAlphabetical.IsReallyChecked())
            {
                generator.SortMethod = ListGenerator.Sort.Alphabetical;
            }
            else if (radioButtonCategorical.IsReallyChecked())
            {
                generator.SortMethod = ListGenerator.Sort.Categorical;
            }
            else if (radioButtonTypeBankIndex.IsReallyChecked())
            {
                generator.SortMethod = ListGenerator.Sort.TypeBankIndex;
            }
            else
            {
                throw new ApplicationException("Unknown sort radio button");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorOutputFormatProperties(ListGenerator generator)
        {
            if (radioButtonAsciiTable.IsReallyChecked())
            {
                generator.ListOutputFormat = ListGenerator.OutputFormat.AsciiTable;
            }
            else if (radioButtonText.IsReallyChecked())
            {
                generator.ListOutputFormat = ListGenerator.OutputFormat.Text;
            }
            else if (radioButtonCsv.IsReallyChecked())
            {
                generator.ListOutputFormat = ListGenerator.OutputFormat.Csv;
            }
            else if (radioButtonXml.IsReallyChecked())
            {
                generator.ListOutputFormat = ListGenerator.OutputFormat.Xml;
            }
            else
            {
                throw new ApplicationException("Unknown output radio button");
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="generator"></param>
        private void SetGeneratorFavoriteProperties(ListGenerator generator)
        {
            if (threeStateCheckBoxFavorites.IsChecked == null)
            {
                generator.ListFilterOnFavorites = ListGenerator.FilterOnFavorites.All;
            }
            else if (threeStateCheckBoxFavorites.IsChecked.Value)
            {
                generator.ListFilterOnFavorites = ListGenerator.FilterOnFavorites.Yes;
            }
            else
            {
                generator.ListFilterOnFavorites = ListGenerator.FilterOnFavorites.No;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOutputFileBrowseClick(object sender, RoutedEventArgs e)
        {
            var showDialog = _saveDialog.ShowDialog();
            if ((showDialog != null) && showDialog.Value)
            {
                textBoxOutputFile.Text = _saveDialog.FileName;
                _saveDialog.InitialDirectory = Path.GetDirectoryName(textBoxOutputFile.Text);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonAsciiTableChecked(object sender, RoutedEventArgs e)
        {
            _saveDialog.DefaultExt = Strings.TxtExtension;
            _saveDialog.InitialDirectory = Path.GetDirectoryName(textBoxOutputFile.Text);
            textBoxOutputFile.Text = Path.ChangeExtension(textBoxOutputFile.Text, Strings.TxtExtension);
            _saveDialog.FileName = textBoxOutputFile.Text;
            _saveDialog.Filter = Strings.AsciiFilter;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonTextChecked(object sender, RoutedEventArgs e)
        {
            _saveDialog.DefaultExt = "txt";
            _saveDialog.InitialDirectory = Path.GetDirectoryName(textBoxOutputFile.Text);
            textBoxOutputFile.Text = Path.ChangeExtension(textBoxOutputFile.Text, "txt");
            _saveDialog.FileName = textBoxOutputFile.Text;
            _saveDialog.Filter = Strings.AsciiFilter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonCsvChecked(object sender, RoutedEventArgs e)
        {
            _saveDialog.DefaultExt = "csv";
            _saveDialog.InitialDirectory = Path.GetDirectoryName(textBoxOutputFile.Text);
            textBoxOutputFile.Text = Path.ChangeExtension(textBoxOutputFile.Text, "csv");
            _saveDialog.FileName = textBoxOutputFile.Text;
            _saveDialog.Filter = Strings.CsvFileFilter;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonXmlChecked(object sender, RoutedEventArgs e)
        {
            _saveDialog.DefaultExt = "xml";
            _saveDialog.InitialDirectory = Path.GetDirectoryName(textBoxOutputFile.Text);
            textBoxOutputFile.Text = Path.ChangeExtension(textBoxOutputFile.Text, "xml");
            _saveDialog.FileName = textBoxOutputFile.Text;
            _saveDialog.Filter = Strings.XmlFileFilter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderDiffListMaxNumberOfDifferencesValueChanged(
            object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBoxDiffListMaxNumberOfDifferences.Text =
                sliderDiffListMaxNumberOfDifferences.Value.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboBoxListSubTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = comboBoxListSubType.SelectedItem;
            if (selectedItem != null)
            {
                var longList = (SelectedSubType == ListGenerator.SubType.Long);
                UpdateFilterOptions(longList);

                UpdateOutputOptions();
            }
        }
    }
}
