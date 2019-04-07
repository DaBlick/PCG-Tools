// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using PcgTools.ClipBoard;
using PcgTools.Edit;

using PcgTools.Model.Common.Synth.PatchSorting;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using Common.Extensions;
using PcgTools.ViewModels.Commands.PcgCommands;


namespace PcgTools
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow // : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _isSplitCharacterSelected;


        /// <summary>
        /// 
        /// </summary>
        private const string DefaultSplitCharacter = "-";


        // private readonly List<CheckBox> _midiChannelCheckBoxes = new List<CheckBox>();
        // private readonly List<CheckBox> _statusCheckBoxes = new List<CheckBox>();
        // private readonly Dictionary<char, string> _sortTimbresKeys = new Dictionary<char, string>();


        /// <summary>
        /// 
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();

            TabControl.SelectedIndex = Settings.Default.SettingsTabIndex;

            /* TEMPORARY REMOVED: CLEAN TIMBRES
            _midiChannelCheckBoxes.Add(checkBoxMidiChannelGm);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel1);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel2);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel3);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel4);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel5);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel6);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel7);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel8);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel9);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel10);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel11);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel12);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel13);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel14);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel15);
            _midiChannelCheckBoxes.Add(checkBoxMidiChannel16);

            _statusCheckBoxes.Add(checkBoxStatusOff);
            _statusCheckBoxes.Add(checkBoxStatusInt);
            _statusCheckBoxes.Add(checkBoxStatusExt);
            _statusCheckBoxes.Add(checkBoxStatusEx2);

            _sortTimbresKeys.Add('S', Strings.TimbreSortStatus_setw);
            _sortTimbresKeys.Add('M', Strings.TimbreSortMute_setw);
            _sortTimbresKeys.Add('C', Strings.TimbreSortMidiChannel_setw);
            _sortTimbresKeys.Add('Z', Strings.TimbreSortKeyZone_setw);
            _sortTimbresKeys.Add('V', Strings.TimbreSortKeyVelocity_setw);

            TEMPORARY REMOVED: CLEAN TIMBRES */

            UpdateTimbreFilterControls();

            //listBoxTimbreSortKeys.SelectedIndex = 0;
            //UpdateTimbreSortControls();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // PCG Window settings.
            CheckBoxShowNumberOfReferencesColumn.IsChecked = Settings.Default.UI_ShowNumberOfReferencesColumn;

            CheckBoxShowSingleLinedSetListSlotDescriptions.IsChecked =
                Settings.Default.SingleLinedSetListSlotDescriptions;

            CheckBoxShowSingleLinedSetListSlotDescriptions.IsChecked =
                Settings.Default.SingleLinedSetListSlotDescriptions;

            WindowLoadedClearPatches();

            CheckBoxFixReferencesToClearedUsedPatches.IsChecked = Settings.Default.UI_ClearPatchesFixReferences;
            
            // Load file settings.
            WindowLoadedAutoBackupFiles();
            
            WindowLoadedMasterFile();

            if ((Settings.Default.Slg_DefaultOutputFolder == string.Empty) || 
                    !Directory.Exists(Settings.Default.Slg_DefaultOutputFolder))
            {
                Settings.Default.Slg_DefaultOutputFolder =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            textBoxDefaultOutputDirectory.Text = Settings.Default.Slg_DefaultOutputFolder;

            if ((Settings.Default.Slg_DefaultOutputFolderForSequencerFiles == string.Empty) ||
                !Directory.Exists(Settings.Default.Slg_DefaultOutputFolderForSequencerFiles))
            {
                Settings.Default.Slg_DefaultOutputFolderForSequencerFiles =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            textBoxDefaultOutputDirectoryForSequencerFiles.Text =
                Settings.Default.Slg_DefaultOutputFolderForSequencerFiles;

            textBoxDefaultManualPath.Text = Settings.Default.DefaultManualFolder;

            // Load edit settings.
            checkBoxRenameFileWhenPatchNameChanges.IsChecked = Settings.Default.Edit_RenameFileWhenPatchNameChanges;

            WindowLoadedCopyPasteSettings();

            textBoxIgnoreCharacters.Text = Settings.Default.CopyPaste_IgnoreCharactersForPatchDuplication;

            checkBoxOverwriteFilledPrograms.IsChecked = Settings.Default.CopyPaste_OverwriteFilledPrograms;
            checkBoxOverwriteFilledCombis.IsChecked = Settings.Default.CopyPaste_OverwriteFilledCombis;
            checkBoxOverwriteFilledSetListSlots.IsChecked = Settings.Default.CopyPaste_OverwriteFilledSetListSlots;
            checkBoxOverwriteFilledDrumKits.IsChecked = Settings.Default.CopyPaste_OverwriteFilledDrumKits;
            checkBoxOverwriteFilledDrumPatterns.IsChecked = Settings.Default.CopyPaste_OverwriteFilledDrumPatterns;
            checkBoxOverwriteFilledWaveSequences.IsChecked = Settings.Default.CopyPaste_OverwriteFilledWaveSequences;

            // Clean Timbres tab.
            /* TEMPORARY REMOVED: CLEAN TIMBRES
           checkBoxCleanTimbresWithMidiChannel.IsChecked = Settings.Default.CleanTimbres_WithMidiChannel;

           for (var index = 0; index < _midiChannelCheckBoxes.Patches.Count; index++)
           {
               _midiChannelCheckBoxes[index].IsChecked = (Settings.Default.CleanTimbres_MidiChannels &
                                                          (int) Math.Pow(2, index)) != 0;
           }

           checkBoxCleanTimbre10Only.IsChecked = Settings.Default.CleanTimbres_OnlyCleanTimbre10;

           checkBoxCleanTimbresWithMuteOn.IsChecked = Settings.Default.CleanTimbres_WithMuteOn;
           checkBoxCleanTimbresWithStatus.IsChecked = Settings.Default.CleanTimbres_WithStatus;

           for (var index = 0; index < _statusCheckBoxes.Patches.Count; index++)
           {
               _statusCheckBoxes[index].IsChecked = (Settings.Default.CleanTimbres_Statusses & (int) Math.Pow(2, index)) !=
                                                    0;
           }

           UpdateTimbreFilterControls();

           TEMPORARY REMOVED: CLEAN TIMBRES */

            // SortMethod tab.
           textBoxSplitCharacter.Text = Settings.Default.Sort_SplitCharacter;
           radioButtonSortArtistTitle.IsChecked = Settings.Default.Sort_ArtistTitleSortOrder;

           WindowLoadedSortOrder();

           UpdateSortOrder();
           UpdateSortType();

           // SortMethod Timbres tab.
           /* TEMPORARY REMOVED: CLEAN TIMBRES
           listBoxTimbreSortKeys.Items.Clear();
           foreach (var sortKey in Settings.Default.SortTimbres_SortKeys) // Chars
           {
               listBoxTimbreSortKeys.Items.Add(_sortTimbresKeys[sortKey]);
           }

           if (Settings.Default.SortTimbres_FixedGaps)
           {
               radioButtonFixedGaps.IsChecked = true;
           }
           else
           {
               radioButtonDynamicGaps.IsChecked = true;
           }

           checkBoxCreateGapsBetweenMidiChannels.IsChecked = Settings.Default.SortTimbres_GapsBetweenMidiChannels == 0;

           switch (Settings.Default.SortTimbres_GapsBetweenMidiChannels)
           {
               case 0:
                   radioButtonMidiChannels0Gaps.IsChecked = true;
                   break;

               case 1:
                   radioButtonMidiChannels1Gap.IsChecked = true;
                   break;

               case 2:

                   radioButtonMidiChannels2Gaps.IsChecked = true;
                   break;

               default:
                   throw new ApplicationException();
           }

           checkBoxCreateGapsBetweenKeyZones.IsChecked = Settings.Default.SortTimbres_GapsBetweenKeyZones == 0;

           switch (Settings.Default.SortTimbres_GapsBetweenKeyZones)
           {
               case 0:
                   radioButtonKeyZones0Gaps.IsChecked = true;
                   break;

               case 1:
                   radioButtonKeyZones1Gap.IsChecked = true;
                   break;

               case 2:

                   radioButtonKeyZones2Gaps.IsChecked = true;
                   break;

               default:
                   throw new ApplicationException();
           }

           checkBoxUse8TimbresMaxIfPossible.IsChecked = Settings.Default.SortTimbres_Use8TimbresMax;
           checkBoxKeepTimbre10Fixed.IsChecked = Settings.Default.SortTimbres_KeepTimbre10Fixed;

           // Category (Trinity) settings.
           if ((Settings.Default.TrinityCategorySetA))
           {
               rbCategoryA.IsChecked = true;
           }
           else
           {
               rbCategoryB.IsChecked = true;
           }
           */
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedCopyPasteSettings()
        {
            checkBoxCopyIncompleteCombis.IsChecked = Settings.Default.CopyPaste_CopyIncompleteCombis;
            checkBoxCopyIncompleteSetListSlots.IsChecked = Settings.Default.CopyPaste_CopyIncompleteSetListSlots;
            checkBoxCopyPatchesFromMasterFile.IsChecked = Settings.Default.CopyPaste_CopyPatchesFromMasterFile;

            checkBoxPasteDuplicatePrograms.IsChecked = Settings.Default.CopyPaste_PasteDuplicatePrograms;
            checkBoxPasteDuplicateCombis.IsChecked = Settings.Default.CopyPaste_PasteDuplicateCombis;
            checkBoxPasteDuplicateSetListSlots.IsChecked = Settings.Default.CopyPaste_PasteDuplicateSetListSlots;
            checkBoxPasteDuplicateDrumKits.IsChecked = Settings.Default.CopyPaste_PasteDuplicateDrumKits;
            checkBoxPasteDuplicateDrumPatterns.IsChecked = Settings.Default.CopyPaste_PasteDuplicateDrumPatterns;
            checkBoxPasteDuplicateWaveSequences.IsChecked = Settings.Default.CopyPaste_PasteDuplicateWaveSequences;

            checkBoxAutoExtendedSinglePatchSelectionPaste.IsChecked =
                Settings.Default.CopyPaste_AutoExtendedSinglePatchSelectionPaste;

            WindowLoadedCopyPastePatchDuplication();
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedSortOrder()
        {
            switch ((PatchSorter.SortOrder) (Settings.Default.Sort_Order))
            {
                case PatchSorter.SortOrder.ESortOrderNameCategory:
                    radioButtonNameCategory.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderTitleArtistCategory:
                    radioButtonTitleArtistCategory.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderArtistTitleCategory:
                    radioButtonArtistTitleCategory.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryName:
                    radioButtonCategoryName.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryTitleArtist:
                    radioButtonCategoryTitleArtist.IsChecked = true;
                    break;

                case PatchSorter.SortOrder.ESortOrderCategoryArtistTitle:
                    radioButtonCategoryArtistTitle.IsChecked = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedCopyPastePatchDuplication()
        {
            switch ((CopyPaste.PatchDuplication) (Settings.Default.CopyPaste_PatchDuplicationName))
            {
                case CopyPaste.PatchDuplication.DoNotUsePatchNames:
                    radioButtonDoNotUsePatchNames.IsChecked = true;
                    break;

                case CopyPaste.PatchDuplication.EqualNames:
                    radioButtonEquallyNamedPatches.IsChecked = true;
                    break;

                case CopyPaste.PatchDuplication.LikeNamedNames:
                    radioButtonLikeNamedPatches.IsChecked = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedClearPatches()
        {
            switch ((ClearCommands.ClearPatchesAlgorithm)(Settings.Default.UI_ClearPatches))
            {
                case ClearCommands.ClearPatchesAlgorithm.None:
                    RadioButtonClearPatchesNone.IsChecked = true;
                    break;

                case ClearCommands.ClearPatchesAlgorithm.UnusedOnly:
                    RadioButtonClearPatchesUnusedOnly.IsChecked = true;
                    break;

                case ClearCommands.ClearPatchesAlgorithm.Ask:
                    RadioButtonClearPatchesAsk.IsChecked = true;
                    break;

                case ClearCommands.ClearPatchesAlgorithm.UnusedAndUsed:
                    RadioButtonClearPatchesUnusedAndUsed.IsChecked = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedAutoBackupFiles()
        {
            CheckBoxAutoBackupFilesEnabled.IsChecked = Settings.Default.Settings_AutoBackupFilesEnabled;
            NumericUpDownAutoBackupFilesIntervalTime.Value = Settings.Default.Settings_AutoBackupFilesIntervalTime;
            NumericUpDownAutoBackupMaxStorage.Value = Settings.Default.Settings_AutoBackupFilesMaxStorage;
        }


        /// <summary>
        /// 
        /// </summary>
        private void WindowLoadedMasterFile()
        {
            switch ((MasterFiles.MasterFiles.AutoLoadMasterFiles) (Settings.Default.MasterFiles_AutoLoad))
            {
                case MasterFiles.MasterFiles.AutoLoadMasterFiles.Always:
                    rbFilesAutoLoadMasterFileAlways.IsChecked = true;
                    break;

                case MasterFiles.MasterFiles.AutoLoadMasterFiles.Ask:
                    rbFilesAutoLoadMasterFileAsk.IsChecked = true;
                    break;

                case MasterFiles.MasterFiles.AutoLoadMasterFiles.Never:
                    rbFilesAutoLoadMasterFileNever.IsChecked = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDefaultOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = Strings.SelectFolderForGeneratedFiles_settw,
                SelectedPath = Settings.Default.Slg_DefaultOutputFolder
            };

            if ((dialog.SelectedPath == string.Empty) || !Directory.Exists(dialog.SelectedPath))
            {
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDefaultOutputDirectory.Text = dialog.SelectedPath;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDefaultOutputDirectoryForSequencerFiles_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = Strings.SelectFolderForGeneratedFiles_settw,
                SelectedPath = Settings.Default.Slg_DefaultOutputFolderForSequencerFiles
            };

            if ((dialog.SelectedPath == string.Empty) || !Directory.Exists(dialog.SelectedPath))
            {
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDefaultOutputDirectoryForSequencerFiles.Text = dialog.SelectedPath;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDefaultManualPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = Strings.SelectFolderOfManual_settw,
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = Settings.Default.DefaultManualFolder
            };

            if (dialog.FileName == string.Empty)
            {
                dialog.FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDefaultManualPath.Text = dialog.FileName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRestoreClick(object sender, RoutedEventArgs e)
        {
            checkBoxCopyIncompleteCombis.IsChecked = false;
            checkBoxCopyIncompleteSetListSlots.IsChecked = false;
            checkBoxCopyPatchesFromMasterFile.IsChecked = false;

            checkBoxPasteDuplicatePrograms.IsChecked = false;
            checkBoxPasteDuplicateCombis.IsChecked = false;
            checkBoxPasteDuplicateSetListSlots.IsChecked = true;

            checkBoxAutoExtendedSinglePatchSelectionPaste.IsChecked = true;

            checkBoxOverwriteFilledPrograms.IsChecked = false;
            checkBoxOverwriteFilledCombis.IsChecked = false;
            checkBoxOverwriteFilledSetListSlots.IsChecked = false;
            checkBoxOverwriteFilledDrumKits.IsChecked = false;
            checkBoxOverwriteFilledDrumPatterns.IsChecked = false;
            checkBoxOverwriteFilledWaveSequences.IsChecked = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSplitCharacter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EditUtils.CheckText(textBoxSplitCharacter.Text, 1, EditUtils.ECheckType.SplitCharacter) != string.Empty)
            {
                textBoxSplitCharacter.Text = DefaultSplitCharacter;
            }

            UpdateSortOrder();
            UpdateSortType();
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateSortOrder()
        {

            if (radioButtonSortArtistTitle != null)
            {
                _isSplitCharacterSelected = textBoxSplitCharacter.Text != string.Empty;
                radioButtonSortTitleArtist.IsEnabled = _isSplitCharacterSelected;
                radioButtonSortArtistTitle.IsEnabled = _isSplitCharacterSelected;

                if (radioButtonSortArtistTitle.IsEnabled)
                {
                    radioButtonSortTitleArtist.Content =
                        $"{Strings.Title_settw} {textBoxSplitCharacter.Text} {Strings.Artist_settw}";
                    radioButtonSortArtistTitle.Content =
                        $"{Strings.Artist_settw} {textBoxSplitCharacter.Text} {Strings.Title_settw}";
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateSortType()
        {
            if (radioButtonArtistTitleCategory != null)
            {
                UpdateSortTypeEnableRadioButtons();
                UpdateSortTypeCheckRadioButtons();
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void UpdateSortTypeEnableRadioButtons()
        {
            radioButtonArtistTitleCategory.IsEnabled = _isSplitCharacterSelected;
            radioButtonCategoryArtistTitle.IsEnabled = _isSplitCharacterSelected;
            radioButtonCategoryTitleArtist.IsEnabled = _isSplitCharacterSelected;
            radioButtonTitleArtistCategory.IsEnabled = _isSplitCharacterSelected;
        }


        /// <summary>
        /// In case a radio button is selected that is disabled, select a valid radio button.
        /// </summary>
        private void UpdateSortTypeCheckRadioButtons()
        {
            if ((radioButtonTitleArtistCategory.IsReallyChecked() ||
                 radioButtonArtistTitleCategory.IsReallyChecked()) &&
                !_isSplitCharacterSelected)
            {
                radioButtonNameCategory.IsChecked = true;
            }

            if ((radioButtonCategoryArtistTitle.IsReallyChecked() ||
                 radioButtonCategoryTitleArtist.IsReallyChecked()) &&
                !_isSplitCharacterSelected)
            {
                radioButtonCategoryName.IsChecked = true;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            SavePcgWindowSettings();
            SaveFileSettings();
            SaveEditSettings();
            SaveCopyPasteSettings();
            // Save timbre clear settings.
            /* TEMPORARY REMOVED: CLEAN TIMBRES
            Settings.Default.CleanTimbres_WithMidiChannel = checkBoxCleanTimbresWithMidiChannel.IsReallyChecked();

            Settings.Default.CleanTimbres_MidiChannels = 0;
            for (var index = 0; index < _midiChannelCheckBoxes.Patches.Count; index++)
            {
                if (_midiChannelCheckBoxes[index].IsReallyChecked())
                {
                    Settings.Default.CleanTimbres_MidiChannels += (int) Math.Pow(2, index);
                }
            }

            Settings.Default.CleanTimbres_OnlyCleanTimbre10 = checkBoxCleanTimbre10Only.IsReallyChecked();

            Settings.Default.CleanTimbres_WithMuteOn = checkBoxCleanTimbresWithMuteOn.IsReallyChecked();
            Settings.Default.CleanTimbres_WithStatus = checkBoxCleanTimbresWithStatus.IsReallyChecked();

            Settings.Default.CleanTimbres_Statusses = 0;
            for (var index = 0; index < _statusCheckBoxes.Patches.Count; index++)
            {
                if (_statusCheckBoxes[index].IsReallyChecked())
                {
                    Settings.Default.CleanTimbres_Statusses += (int) Math.Pow(2, index);
                }
            }
             TEMPORARY REMOVED: CLEAN TIMBRES*/
            SaveSortOrderSettings();
            // Save SortMethod Timbres settings.
            SaveCategorySettings();

            // Save and close.
            Settings.Default.SettingsTabIndex = TabControl.SelectedIndex;

            Settings.Default.Save();
            Close();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SavePcgWindowSettings()
        {
            Settings.Default.UI_ShowNumberOfReferencesColumn = CheckBoxShowNumberOfReferencesColumn.IsReallyChecked();

            Settings.Default.SingleLinedSetListSlotDescriptions =
                CheckBoxShowSingleLinedSetListSlotDescriptions.IsReallyChecked();
            

            Settings.Default.SingleLinedSetListSlotDescriptions =
                CheckBoxShowSingleLinedSetListSlotDescriptions.IsReallyChecked();

            Settings.Default.UI_ClearPatches =
                RadioButtonClearPatchesNone.IsReallyChecked()
                    ? (int) ClearCommands.ClearPatchesAlgorithm.None
                    : RadioButtonClearPatchesUnusedOnly.IsReallyChecked()
                        ? (int) ClearCommands.ClearPatchesAlgorithm.UnusedOnly
                        : RadioButtonClearPatchesAsk.IsReallyChecked()
                            ? (int) ClearCommands.ClearPatchesAlgorithm.Ask
                            : (int) ClearCommands.ClearPatchesAlgorithm.UnusedAndUsed;

            Settings.Default.UI_ClearPatchesFixReferences = CheckBoxFixReferencesToClearedUsedPatches.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveFileSettings()
        {
            // Auto backup files.
            Settings.Default.Settings_AutoBackupFilesEnabled = CheckBoxAutoBackupFilesEnabled.IsReallyChecked();
            Settings.Default.Settings_AutoBackupFilesIntervalTime = NumericUpDownAutoBackupFilesIntervalTime.Value ?? 5;
            Settings.Default.Settings_AutoBackupFilesMaxStorage = NumericUpDownAutoBackupMaxStorage.Value ?? 500;

            // Master files.
            Settings.Default.MasterFiles_AutoLoad =
                rbFilesAutoLoadMasterFileAlways.IsReallyChecked()
                    ? (int) MasterFiles.MasterFiles.AutoLoadMasterFiles.Always
                    : rbFilesAutoLoadMasterFileAsk.IsReallyChecked()
                        ? (int) MasterFiles.MasterFiles.AutoLoadMasterFiles.Ask
                        : (int) MasterFiles.MasterFiles.AutoLoadMasterFiles.Never;

            // Output folders.
            Settings.Default.Slg_DefaultOutputFolder = textBoxDefaultOutputDirectory.Text;
            Settings.Default.Slg_DefaultOutputFolderForSequencerFiles =
                textBoxDefaultOutputDirectoryForSequencerFiles.Text;

            // Manual folder.
            Settings.Default.DefaultManualFolder = textBoxDefaultManualPath.Text;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveEditSettings()
        {
            Settings.Default.Edit_RenameFileWhenPatchNameChanges =
                checkBoxRenameFileWhenPatchNameChanges.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveCopyPasteSettings()
        {
            Settings.Default.CopyPaste_CopyIncompleteCombis = checkBoxCopyIncompleteCombis.IsReallyChecked();
            Settings.Default.CopyPaste_CopyIncompleteSetListSlots = checkBoxCopyIncompleteSetListSlots.IsReallyChecked();
            Settings.Default.CopyPaste_CopyPatchesFromMasterFile = checkBoxCopyPatchesFromMasterFile.IsReallyChecked();

            Settings.Default.CopyPaste_PasteDuplicatePrograms = checkBoxPasteDuplicatePrograms.IsReallyChecked();
            Settings.Default.CopyPaste_PasteDuplicateCombis = checkBoxPasteDuplicateCombis.IsReallyChecked();
            Settings.Default.CopyPaste_PasteDuplicateSetListSlots = checkBoxPasteDuplicateSetListSlots.IsReallyChecked();
            Settings.Default.CopyPaste_PasteDuplicateDrumKits = checkBoxPasteDuplicateDrumKits.IsReallyChecked();
            Settings.Default.CopyPaste_PasteDuplicateDrumPatterns = checkBoxPasteDuplicateDrumPatterns.IsReallyChecked();
            Settings.Default.CopyPaste_PasteDuplicateWaveSequences = checkBoxPasteDuplicateWaveSequences.IsReallyChecked();

            Settings.Default.CopyPaste_AutoExtendedSinglePatchSelectionPaste =
                checkBoxAutoExtendedSinglePatchSelectionPaste.IsReallyChecked();

            SaveCopyPastePatchDuplicationSettings();

            Debug.Assert(Settings.Default.CopyPaste_PatchDuplicationName != -1);
            Settings.Default.CopyPaste_IgnoreCharactersForPatchDuplication = textBoxIgnoreCharacters.Text;

            SaveCopyPasteOverwriteSettings();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveCopyPastePatchDuplicationSettings()
        {
            Settings.Default.CopyPaste_PatchDuplicationName =
                radioButtonDoNotUsePatchNames.IsReallyChecked()
                    ? (int) CopyPaste.PatchDuplication.DoNotUsePatchNames
                    : radioButtonEquallyNamedPatches.IsReallyChecked()
                        ? (int) CopyPaste.PatchDuplication.EqualNames
                        : radioButtonLikeNamedPatches.IsReallyChecked()
                            ? (int) CopyPaste.PatchDuplication.LikeNamedNames
                            : -1;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveCopyPasteOverwriteSettings()
        {
            Settings.Default.CopyPaste_OverwriteFilledPrograms = checkBoxOverwriteFilledPrograms.IsReallyChecked();
            Settings.Default.CopyPaste_OverwriteFilledCombis = checkBoxOverwriteFilledCombis.IsReallyChecked();
            Settings.Default.CopyPaste_OverwriteFilledSetListSlots =
                checkBoxOverwriteFilledSetListSlots.IsReallyChecked();
            Settings.Default.CopyPaste_OverwriteFilledDrumKits = checkBoxOverwriteFilledDrumKits.IsReallyChecked();
            Settings.Default.CopyPaste_OverwriteFilledDrumPatterns = checkBoxOverwriteFilledDrumPatterns.IsReallyChecked();
            Settings.Default.CopyPaste_OverwriteFilledWaveSequences =
                checkBoxOverwriteFilledWaveSequences.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveSortOrderSettings()
        {
            Settings.Default.Sort_SplitCharacter = textBoxSplitCharacter.Text;
            Settings.Default.Sort_ArtistTitleSortOrder = radioButtonSortArtistTitle.IsReallyChecked();

            Settings.Default.Sort_Order =
                radioButtonArtistTitleCategory.IsReallyChecked()
                    ? (int) PatchSorter.SortOrder.ESortOrderArtistTitleCategory
                    : radioButtonCategoryArtistTitle.IsReallyChecked()
                        ? (int) PatchSorter.SortOrder.ESortOrderCategoryArtistTitle
                        : radioButtonCategoryName.IsReallyChecked()
                            ? (int) PatchSorter.SortOrder.ESortOrderCategoryName
                            : radioButtonCategoryTitleArtist.IsReallyChecked()
                                ? (int) PatchSorter.SortOrder.ESortOrderCategoryTitleArtist
                                : radioButtonNameCategory.IsReallyChecked()
                                    ? (int) PatchSorter.SortOrder.ESortOrderNameCategory
                                    : -1;

            Debug.Assert(Settings.Default.Sort_Order != -1);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveCategorySettings()
        {
            Settings.Default.TrinityCategorySetA = rbCategoryA.IsReallyChecked();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_CleanTimbresWithMidiChannelChecked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreFilterControls();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxMidiChannel10_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreFilterControls();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxCleanTimbresWithStatus_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreFilterControls();
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateTimbreFilterControls()
        {
            // Update MIDI channel check boxes.
            /* TEMPORARY REMOVED: CLEAN TIMBRES
            var isChecked = checkBoxCleanTimbresWithMidiChannel.IsReallyChecked();
            foreach (var checkBox in _midiChannelCheckBoxes)
            {
                checkBox.IsEnabled = isChecked;
            }

            // Update MIDI channel 10 special check box.
            checkBoxCleanTimbre10Only.IsEnabled = checkBoxMidiChannel10.IsReallyChecked();

            // Update checkboxes for status.
            isChecked = checkBoxCleanTimbresWithStatus.IsReallyChecked();
            foreach (var checkBox in _statusCheckBoxes)
            {
                checkBox.IsEnabled = isChecked;
            }
              TEMPORARY REMOVED: CLEAN TIMBRES */
        }

        private void RadioButtonProgramDuplication_checked(object sender, RoutedEventArgs e)
        {
            var likeNamed = radioButtonLikeNamedPatches.IsReallyChecked();
            labelIgnoreCharacters.IsEnabled = likeNamed;
            textBoxIgnoreCharacters.IsEnabled = likeNamed;
        }

        /*
        private void TimbreSortKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTimbreSortControls();
        }

        
        private void buttonMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = listBoxTimbreSortKeys.SelectedIndex;
            var temp = listBoxTimbreSortKeys.Items[selectedIndex];
            listBoxTimbreSortKeys.Items[selectedIndex] = listBoxTimbreSortKeys.Items[selectedIndex - 1];
            listBoxTimbreSortKeys.Items[selectedIndex - 1] = temp;
            listBoxTimbreSortKeys.SelectedIndex = selectedIndex - 1;

            UpdateTimbreSortControls();
        }


        private void buttonMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = listBoxTimbreSortKeys.SelectedIndex;
            var temp = listBoxTimbreSortKeys.Items[selectedIndex];
            listBoxTimbreSortKeys.Items[selectedIndex] = listBoxTimbreSortKeys.Items[selectedIndex + 1];
            listBoxTimbreSortKeys.Items[selectedIndex + 1] = temp;
            listBoxTimbreSortKeys.SelectedIndex = selectedIndex + 1;

            UpdateTimbreSortControls();
        }


        private void radioButtonFixedGaps_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreSortControls();
        }


        private void checkBoxCreateGapsBetweenMidiChannels_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreSortControls();
        }


        private void checkBoxCreateGapsBetweenKeyZones_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTimbreSortControls();
        }


        private void UpdateTimbreSortControls()
        {
            buttonMoveUp.IsEnabled = listBoxTimbreSortKeys.SelectedIndex > 0;
            buttonMoveDown.IsEnabled = listBoxTimbreSortKeys.SelectedIndex < listBoxTimbreSortKeys.Items.Patches.Count - 1;

            var isChecked = radioButtonFixedGaps.IsReallyChecked();

            checkBoxCreateGapsBetweenMidiChannels.IsEnabled = isChecked;
            radioButtonMidiChannels1Gap.IsEnabled = checkBoxCreateGapsBetweenMidiChannels.IsReallyChecked();
            radioButtonMidiChannels2Gaps.IsEnabled = checkBoxCreateGapsBetweenMidiChannels.IsReallyChecked();

            checkBoxCreateGapsBetweenKeyZones.IsEnabled = isChecked;
            radioButtonKeyZones1Gap.IsEnabled = checkBoxCreateGapsBetweenKeyZones.IsReallyChecked();
            radioButtonKeyZones2Gaps.IsEnabled = checkBoxCreateGapsBetweenKeyZones.IsReallyChecked();
        }

        */
    }
}
