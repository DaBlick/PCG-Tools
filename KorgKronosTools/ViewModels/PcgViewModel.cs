// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Common.Mvvm;
using Common.Utils;
using PcgTools.ClipBoard;
using PcgTools.Gui;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchSorting;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class PcgViewModel : ViewModel, IPcgViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public PcgClipBoard PcgClipBoard { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        private readonly CopyPasteCommands _copyPasteCommands;


        /// <summary>
        /// 
        /// </summary>
        private readonly ClearCommands _clearCommands;


        /// <summary>
        /// 
        /// </summary>
        private readonly DoubleToSingleKeyboardCommands _doubleToSingleKeyboardCommands;


        /// <summary>
        /// 
        /// </summary>
        public enum ScopeSet
        {
            Banks,
            Patches
        }


        /// <summary>
        /// 
        /// </summary>
        enum SelectedBanksType
        {
            None, // Only allowed during init
            ProgramBanks,
            CombiBanks,
            SetLists,
            DrumKitBanks,
            DrumPatternBanks,
            WaveSequenceBanks,
            AllPatches // No banks selected
        }


        /// <summary>
        /// 
        /// </summary>
        public enum DialogType
        {
            EditSingleProgram,
            EditMultiplePrograms,
            EditSingleProgramBank,
            EditMultipleProgramBanks,

            EditSingleCombi,
            EditMultipleCombis,
            EditSingleCombiBank,
            EditMultipleCombiBanks,

            EditSingleSetListSlot,
            EditMultipleSetListSlots,
            EditSingleSetList,
            EditMultipleSetLists,

            EditSingleDrumKit,
            EditMultipleDrumKits,
            EditSingleDrumKitsBank,
            EditMultipleDrumKitBanks,

            EditSingleDrumPattern,
            EditMultipleDrumPatterns,
            EditSingleDrumPatternBank,
            EditMultipleDrumPatternBanks,

            EditSingleWaveSequence,
            EditMultipleWaveSequences,
            EditSingleWaveSequenceBank,
            EditMultipleWaveSequenceBanks,

        }


        /// <summary>
        /// 
        /// </summary>
        public Action<DialogType, IEnumerable<INavigable>> ShowDialog { private get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public Func<string, string, WindowUtils.EMessageBoxButton, WindowUtils.EMessageBoxImage, WindowUtils.EMessageBoxResult, 
         WindowUtils.EMessageBoxResult> ShowMessageBox { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action<WindowUtils.ECursor> SetCursor { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Action ShowPasteWindow { get; set;}
        

        /// <summary>
        /// 
        /// </summary>
        public Action ShowListGenerator { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action ShowProgramReferencesChanger { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action UpdateTimbresWindows { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action<ICombi, int, int> ShowTimbresWindow { private get; set;}

        
        /// <summary>
        /// 
        /// </summary>
        public Action<ObservableCollectionEx<IPatch>> EditParameterWindow { private get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        public Action MoveSelectedPatchesUp { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action MoveSelectedPatchesDown { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action<IModel, string> SetPcgFileAsMasterFile { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Func<int> GetSelectedPatchListViewIndex { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        string _windowTitle;


        /// <summary>
        /// 
        /// </summary>
        public string WindowTitle
        {
            get { return _windowTitle; }
            set { if (_windowTitle != value) { _windowTitle = value; OnPropertyChanged("WindowTitle"); } }
        }

      
        /// <summary>
        /// 
        /// </summary>
        public void UpdateWindowTitle()
        {
            WindowTitle =
                $"{SelectedPcgMemory.FileName}{(SelectedPcgMemory.IsDirty ? " *" : string.Empty)} ({SelectedPcgMemory.Model.ModelAsString}{(SelectedPcgMemory.Model.OsVersionString == string.Empty ? string.Empty : " ")}{SelectedPcgMemory.Model.OsVersionString})";
        }


        /// <summary>
        /// 
        /// </summary>
        SelectedBanksType _selectedBanksType = SelectedBanksType.None;

        
        /// <summary>
        /// FUTURE: Identical to CombiBanksSelected and SetListsSelected.
        /// </summary>
        public bool ProgramBanksSelected
        {
            get
            {
                return _selectedBanksType == SelectedBanksType.ProgramBanks;
            }
            
            // ReSharper disable once MemberCanBePrivate.Global
            [UsedImplicitly] set
            {
                if (value && (_selectedBanksType != SelectedBanksType.ProgramBanks))
                {
                    _selectedBanksType = SelectedBanksType.ProgramBanks;

                    Banks.Clear(); // = new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.ProgramBanks.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                    //BanksChanged();
                    
                    OnPropertyChanged("ProgramBanksSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool CombiBanksSelected
        {
            get
            {
                return _selectedBanksType == SelectedBanksType.CombiBanks;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            [UsedImplicitly] set
            {
                if (value && (_selectedBanksType != SelectedBanksType.CombiBanks))
                {
                    _selectedBanksType = SelectedBanksType.CombiBanks;

                    Banks.Clear(); // new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.CombiBanks.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                   // BanksChanged();

                    OnPropertyChanged("CombiBanksSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool SetListsSelected
        {
            get { return _selectedBanksType == SelectedBanksType.SetLists; }
            // ReSharper disable once MemberCanBePrivate.Global
            [UsedImplicitly] set
            {
                if (value && (_selectedBanksType != SelectedBanksType.SetLists))
                {
                    _selectedBanksType = SelectedBanksType.SetLists;

                    Banks.Clear(); // // new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.SetLists.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                    //BanksChanged();

                    OnPropertyChanged("SetListsSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool DrumKitBanksSelected
        {
            get { return _selectedBanksType == SelectedBanksType.DrumKitBanks; }
            [UsedImplicitly]
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (value && (_selectedBanksType != SelectedBanksType.DrumKitBanks))
                {
                    _selectedBanksType = SelectedBanksType.DrumKitBanks;

                    Banks.Clear(); // new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.DrumKitBanks.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                    //BanksChanged();

                    OnPropertyChanged("DrumKitBanksSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool DrumPatternBanksSelected
        {
            get { return _selectedBanksType == SelectedBanksType.DrumPatternBanks; }
            [UsedImplicitly]
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (value && (_selectedBanksType != SelectedBanksType.DrumPatternBanks))
                {
                    _selectedBanksType = SelectedBanksType.DrumPatternBanks;

                    Banks.Clear(); // new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.DrumPatternBanks.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                    //BanksChanged();

                    OnPropertyChanged("DrumPatternBanksSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool WaveSequenceBanksSelected
        {
            get { return _selectedBanksType == SelectedBanksType.WaveSequenceBanks; }
            [UsedImplicitly]
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (value && (_selectedBanksType != SelectedBanksType.WaveSequenceBanks))
                {
                    _selectedBanksType = SelectedBanksType.WaveSequenceBanks;

                    Banks.Clear(); // new ObservableCollectionEx<IBank>();
                    foreach (var bank in SelectedPcgMemory.WaveSequenceBanks.BankCollection)
                    {
                        Banks.Add(bank);
                    }

                    //BanksChanged();

                    OnPropertyChanged("WaveSequenceBanksSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool AllPatchesSelected
        {
            get { return _selectedBanksType == SelectedBanksType.AllPatches; }

            // ReSharper disable once MemberCanBePrivate.Global
            [UsedImplicitly]
            set
            {
                if (value && (_selectedBanksType != SelectedBanksType.AllPatches))
                {
                    _selectedBanksType = SelectedBanksType.AllPatches;

                    // Add all banks.
                    Banks.Clear(); // new ObservableCollectionEx<IBank>();
                    AddBanks(SelectedPcgMemory.ProgramBanks);
                    AddBanks(SelectedPcgMemory.CombiBanks);
                    AddBanks(SelectedPcgMemory.SetLists);
                    AddBanks(SelectedPcgMemory.DrumKitBanks);
                    AddBanks(SelectedPcgMemory.DrumPatternBanks);
                    AddBanks(SelectedPcgMemory.WaveSequenceBanks);

                    OnPropertyChanged("AllPatchesSelected");
                }
            }
        }


        /// <summary>
        /// Select all banks.
        /// </summary>
        /// <param name="banks"></param>
        private void AddBanks(IBanks banks)
        {
            if (banks != null)
            {
                foreach (var bank in banks.BankCollection)
                {
                    Banks.Add(bank);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IPcgMemory SelectedPcgMemory => (PcgMemory) SelectedMemory;


        /// <summary>
        /// 
        /// </summary>
        public override IMemory SelectedMemory
        {
            get { return base.SelectedMemory; }
            set
            {
                if (value != base.SelectedMemory)
                {
                    base.SelectedMemory = value;
                    OnPropertyChanged("SelectedPcgMemory");
                    
                    if (value == null)
                    {
                        return;
                    }

                    SelectedMemory.PropertyChanged += OnMemoryPropertyChanged;

                    SetEnableType();

                    SetBanksSelected();

                    UpdateWindowTitle();
                }
            }
        }


        /// <summary>
        /// Enable/set types (programs, combis, set list slots, drum kits, wave sequences).
        /// </summary>
        private void SetEnableType()
        {
            Debug.Assert(SelectedMemory != null);
            SetEnableTypePrograms();
            SetEnableTypeCombis();
            SetEnableTypeSetListSlots();
            SetEnableTypeDrumKits();
            SetEnableTypeDrumPatterns();
            SetEnableTypeWaveSequences();
            SetEnableTypeAllPatches();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypePrograms()
        {
            ProgramsEnabled = SelectedPcgMemory.ProgramBanks.BankCollection.Any(bank => bank.FilterForUi);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypeCombis()
        {
            CombiBanksSupported = (SelectedPcgMemory.CombiBanks != null);
            if (CombiBanksSupported)
            {
                Debug.Assert(SelectedPcgMemory.CombiBanks != null);
                CombisEnabled = SelectedPcgMemory.CombiBanks.BankCollection.Any(bank => bank.FilterForUi);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypeSetListSlots()
        {
            SetListSlotsSupported = (SelectedPcgMemory.SetLists != null);
            if (SetListSlotsSupported)
            {
                Debug.Assert(SelectedPcgMemory.SetLists != null);
                SetListSlotsEnabled = (SetListSlotsSupported &&
                                       SelectedPcgMemory.SetLists.BankCollection.Any(bank => bank.FilterForUi));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypeDrumKits()
        {
            DrumKitsSupported = (SelectedPcgMemory.DrumKitBanks != null);
            if (DrumKitsSupported)
            {
                Debug.Assert(SelectedPcgMemory.DrumKitBanks != null);
                DrumKitsEnabled = SelectedPcgMemory.DrumKitBanks.BankCollection.Any(bank => bank.FilterForUi);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypeDrumPatterns()
        {
            DrumPatternsSupported = (SelectedPcgMemory.DrumPatternBanks != null);
            if (DrumPatternsSupported)
            {
                Debug.Assert(SelectedPcgMemory.DrumPatternBanks != null);
                DrumPatternsEnabled = SelectedPcgMemory.DrumPatternBanks.BankCollection.Any(bank => bank.FilterForUi);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEnableTypeWaveSequences()
        {
            WaveSequencesSupported = (SelectedPcgMemory.WaveSequenceBanks != null);
            if (WaveSequencesSupported)
            {
                Debug.Assert(SelectedPcgMemory.WaveSequenceBanks != null);
                WaveSequencesEnabled = SelectedPcgMemory.WaveSequenceBanks.BankCollection.Any(bank => bank.FilterForUi);
            }
        }


        private void SetEnableTypeAllPatches()
        {
            AllPatchesEnabled =
                ProgramsEnabled &&
                CombisEnabled &&
                SetListSlotsEnabled &&
                DrumKitsEnabled &&
                DrumPatternsEnabled &&
                WaveSequencesEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetBanksSelected()
        {
            if (ProgramsEnabled)
            {
                ProgramBanksSelected = true;
            }
            else if (CombisEnabled)
            {
                CombiBanksSelected = true;
            }
            else if (SetListSlotsEnabled)
            {
                SetListsSelected = true;
            }
            else if (DrumKitsEnabled)
            {
                DrumKitBanksSelected = true;
            }
            else if (DrumPatternsEnabled)
            {
                DrumPatternBanksSelected = true;
            }
            else if (WaveSequencesEnabled)
            {
                WaveSequenceBanksSelected = true;
            }
            else
            {
                ShowMessageBox(
                    $"{Strings.NoContentWarning}: {SelectedMemory.FileName}",
                    Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok,
                    WindowUtils.EMessageBoxImage.Warning, WindowUtils.EMessageBoxResult.Ok);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ScopeSet _selectedScopeSet;


        /// <summary>
        /// 
        /// </summary>
        public ScopeSet SelectedScopeSet
        {
            get { return _selectedScopeSet; }

            set
            {
                _selectedScopeSet = value;
                OnPropertyChanged("SelectedScopeSet"); // Needs to be called initially
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ObservableCollectionEx<IBank> _banks;


        /// <summary>
        /// All banks within the selected type.
        /// </summary>
        public ObservableCollectionEx<IBank> Banks
        {
            get
            {
                return _banks;
            }

            private set
            {
                if (_banks != value)
                {
                    _banks = value; 
                    OnPropertyChanged("Banks");
                }
            }
        }


        /// <summary>
        /// All patches within the selected bank.
        /// </summary>
        ObservableCollectionEx<IPatch> _patches;


        public ObservableCollectionEx<IPatch> Patches
        {
            // ReSharper disable once MemberCanBePrivate.Global
            [UsedImplicitly] get
            {
                return _patches;
            } 

            set
            {
                if (value != _patches)
                {
                    _patches = value;
                    OnPropertyChanged("Patches");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _programsEnabled;
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public bool ProgramsEnabled
        {
            get
            {
                return _programsEnabled;
            }

            set
            {
                if (value != _programsEnabled)
                {
                    _programsEnabled = value; 
                    OnPropertyChanged("ProgramsEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _combisEnabled;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public bool CombisEnabled
        {
            get
            {
                return _combisEnabled;
            }

            set
            {
                if (value != _combisEnabled)
                {
                    _combisEnabled = value; 
                    OnPropertyChanged("CombisEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _setListSlotsEnabled;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public bool SetListSlotsEnabled
        {
            get
            {
                return _setListSlotsEnabled;
            }

            set
            {
                if
                    (value != _setListSlotsEnabled)
                {
                    _setListSlotsEnabled = value; OnPropertyChanged("SetListSlotsEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _drumKitsEnabled;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool DrumKitsEnabled
        {
            get
            {
                return _drumKitsEnabled;
            }

            set
            {
                if
                    (value != _drumKitsEnabled)
                {
                    _drumKitsEnabled = value;
                    OnPropertyChanged("DrumKitsEnabled");
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        bool _drumPatternsEnabled;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool DrumPatternsEnabled
        {
            get
            {
                return _drumPatternsEnabled;
            }

            set
            {
                if
                    (value != _drumPatternsEnabled)
                {
                    _drumPatternsEnabled = value;
                    OnPropertyChanged("DrumPatternsEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _waveSequencesEnabled;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool WaveSequencesEnabled
        {
            get
            {
                return _waveSequencesEnabled;
            }

            set
            {
                if (value != _waveSequencesEnabled)
                {
                    _waveSequencesEnabled = value;
                    OnPropertyChanged("WaveSequencesEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _allPatchesEnabled;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool AllPatchesEnabled
        {
            get
            {
                return _allPatchesEnabled;
            }

            set
            {
                if (value != _allPatchesEnabled)
                {
                    _allPatchesEnabled = value;
                    OnPropertyChanged("AllPatchesEnabled");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool IsPcgEmpty => (!_programsEnabled && !_combisEnabled && !_setListSlotsEnabled);


        /// <summary>
        /// 
        /// </summary>
        bool _combiBanksSupported;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool CombiBanksSupported
        {
            get
            {
                return _combiBanksSupported;
            }

            set
            {
                if (value != _combiBanksSupported)
                {
                    _combiBanksSupported = value;
                    OnPropertyChanged("CombiBanksSupported");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _setListSlotsSupported;

        
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public bool SetListSlotsSupported
        {
            get
            {
                return _setListSlotsSupported;
            }

            set
            {
                if (value != _setListSlotsSupported)
                {
                    _setListSlotsSupported = value;
                    OnPropertyChanged("SetListSlotsSupported");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _drumKitsSupported;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool DrumKitsSupported
        {
            get
            {
                return _drumKitsSupported; 
                
            }

            set
            {
                if (value != _drumKitsSupported)
                {
                    _drumKitsSupported = value;
                    OnPropertyChanged("DrumKitsSupported");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _drumPatternsSupported;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool DrumPatternsSupported
        {
            get
            {
                return _drumPatternsSupported;

            }

            set
            {
                if (value != _drumPatternsSupported)
                {
                    _drumPatternsSupported = value;
                    OnPropertyChanged("DrumPatternsSupported");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool _waveSequencesSupported;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public bool WaveSequencesSupported
        {
            get
            {
                return _waveSequencesSupported;
            }

            set
            {
                if (value != _waveSequencesSupported)
                {
                    _waveSequencesSupported = value;
                    OnPropertyChanged("WaveSequencesSupported");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgClipBoard"></param>
        public PcgViewModel(PcgClipBoard pcgClipBoard)
        {
            PcgClipBoard = pcgClipBoard;
            Banks = new ObservableCollectionEx<IBank>();
            Patches = new ObservableCollectionEx<IPatch>();
            _copyPasteCommands = new CopyPasteCommands();
            _clearCommands = new ClearCommands();
            _doubleToSingleKeyboardCommands = new DoubleToSingleKeyboardCommands();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
        void FixReferences(IPatch patch, INavigable otherPatch)
        {
            if (SelectedPcgMemory.CombiBanks != null)
            {
                var key = patch as IProgram;
                if (key != null)
                {
                    FixProgramReferences(key, otherPatch);
                }
                else
                {
                    var combi = patch as ICombi;
                    if (combi != null)
                    {
                        FixCombiReferences(combi, otherPatch);
                    }
                    else
                    {
                        var drumKit = patch as IDrumKit;
                        if (drumKit != null)
                        {
                            FixDrumKitReferences(drumKit, otherPatch);
                        }
                        else
                        {
                            var drumPattern = patch as IDrumPattern;
                            if (drumPattern != null)
                            {
                                FixDrumPatternReferences(drumPattern, otherPatch);
                            }
                            else
                            {
                                var waveSequence = patch as IWaveSequence;
                                if (waveSequence != null)
                                {
                                    FixWaveSequenceReferences(waveSequence, otherPatch);
                                }
                            }  
                        }
                    }
                }
            }

            // LV: Support WaveSequences too? 
        }


        /// <summary>
        /// Fix program references in combi timbres. For set lists, fix also set list slots.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="otherPatch"></param>
        private void FixProgramReferences(IProgram key, INavigable otherPatch)
        {
            var changes = new Dictionary<IProgram, IProgram>
            {
                {key, (IProgram) otherPatch},
                {(IProgram) otherPatch, key}
            };
            SelectedPcgMemory.CombiBanks.ChangeTimbreReferences(changes, SelectedPcgMemory);

            SelectedPcgMemory.SetLists?.ChangeProgramReferences(changes);
        }
        

        /// <summary>
        /// For set lists, fix also set list slots.
        /// </summary>
        /// <param name="otherPatch"></param>
        /// <param name="combi"></param>
        private void FixCombiReferences(ICombi combi, INavigable otherPatch)
        {
            var changes = new Dictionary<ICombi, ICombi>
            {
                {combi, (ICombi) otherPatch},
                {(ICombi) otherPatch, combi}
            };

            SelectedPcgMemory.SetLists?.ChangeCombiReferences(changes);
        }


        /// <summary>
        /// Fix drum kit references (from programs).
        /// </summary>
        /// <param name="drumPattern"></param>
        /// <param name="otherPatch"></param>
        private void FixDrumPatternReferences(IDrumPattern drumPattern, INavigable otherPatch)
        {
            //TODO
            return;
            /*
            var changes = new Dictionary<IDrumKit, IDrumKit>
            {
                {drumKit, (IDrumKit) otherPatch},
                {(IDrumKit) otherPatch, drumKit}
            };

            SelectedPcgMemory.ProgramBanks?.ChangeDrumKitReferences(changes);
            */
        }


        /// <summary>
        /// Fix drum kit references (from programs).
        /// </summary>
        /// <param name="drumKit"></param>
        /// <param name="otherPatch"></param>
        private void FixDrumKitReferences(IDrumKit drumKit, INavigable otherPatch)
        {
            var changes = new Dictionary<IDrumKit, IDrumKit>
            {
                {drumKit, (IDrumKit) otherPatch},
                {(IDrumKit) otherPatch, drumKit}
            };

            SelectedPcgMemory.ProgramBanks?.ChangeDrumKitReferences(changes);
        }

        /// <summary>
        /// Fix drum kit references (from wave sequence).
        /// </summary>
        /// <param name="waveSequence"></param>
        /// <param name="otherPatch"></param>
        private void FixWaveSequenceReferences(IWaveSequence waveSequence, INavigable otherPatch)
        {
            var changes = new Dictionary<IWaveSequence, IWaveSequence>
            {
                {waveSequence, (IWaveSequence) otherPatch},
                {(IWaveSequence) otherPatch, waveSequence}
            };

            SelectedPcgMemory.ProgramBanks?.ChangeWaveSequenceReferences(changes);
        }


        /// <summary>
        /// 
        /// </summary>
        void CopyPasteCut()
        {
            PcgClipBoard.CutPasteSelected = true;
            _copyPasteCommands.CopyPasteCopy(PcgClipBoard, SelectedPcgMemory, SelectedScopeSet, 
                ProgramBanksSelected, CombiBanksSelected, SetListsSelected, DrumKitBanksSelected,
                DrumPatternBanksSelected, WaveSequenceBanksSelected, AllPatchesSelected, 
                Banks, Patches, true);
            OnPropertyChanged("PcgClipBoard");

            UpdateTimbresWindows();
        }

        
        /// <summary>
        /// 
        /// </summary>
        string PastePreconditionsAndWarnings
        {
            get
            {
                var errorText = string.Empty;
                // If the PCG is of the same model.
                if (!ModelCompatibility.AreModelsCompatible(SelectedPcgMemory.Model, PcgClipBoard.Model))
                {
                    errorText = Strings.IncompatibleModelError;
                }

                // If the PCG are of the same OS version.
                else if (!ModelCompatibility.AreOsVersionsCompatible(SelectedPcgMemory.Model.OsVersion, PcgClipBoard.Model.OsVersion))
                {
                    errorText = Strings.IncompatibleOsVersion;
                }

                return errorText;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ExportToCubase()
        {
            var builder = new StringBuilder();

            builder.AppendLine("[cubase parse file]");
            builder.AppendLine("[parser version 0001]\n");
            builder.AppendLine("[creators first name]pcgFileReader Tools");
            builder.AppendLine("[creators last name]Michel Keijzers");
            builder.AppendLine("[device manufacturer]Korg");
            builder.AppendLine("[device name] " + SelectedPcgMemory.Model.ModelType.ToString().ToUpper() + "(KORG)");
            builder.AppendLine("[script name] " + SelectedPcgMemory.FileName);
            builder.AppendLine("[script version]version 1.00\n");
            builder.AppendLine("[define patchnames]\n");
            builder.AppendLine("[mode]" + SelectedPcgMemory.Model.ModelAsString);
            var patches = (from bank in SelectedPcgMemory.ProgramBanks.BankCollection
                            from program in bank.Patches
                           where ((IBank)(program.Parent)).IsLoaded && !program.IsEmptyOrInit
                            select program).ToList();

            PatchSorter.SortBy(patches, PatchSorter.SortOrder.ESortOrderCategoryName);

            var programs = patches.Select(patch => patch as Program).ToList();

            var currentCategory = 0;
            var currentSubCategory = 0;
            programs.Aggregate(false, (current, program) => ExportProgramToCubase(
                program, current, builder, ref currentCategory, ref currentSubCategory));

            builder.AppendLine("[end]");

            var folder = Settings.Default.Slg_DefaultOutputFolderForSequencerFiles;
            if (folder == string.Empty)
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            var fileName = $@"{folder}\Cubase.txt";

            try
            {
                File.WriteAllText(fileName, builder.ToString());
                Process.Start(fileName);
            }
            catch (UnauthorizedAccessException exception)
            {
                ShowMessageBox($"{Strings.CubaseIoError}: {exception.Message}", Strings.PcgTools,
                    WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error, WindowUtils.EMessageBoxResult.Ok);
            }
            catch (IOException exception)
            {
                ShowMessageBox($"{Strings.CubaseIoError}: {exception.Message}", Strings.PcgTools,
                    WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error, WindowUtils.EMessageBoxResult.Ok);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="gmReached"></param>
        /// <param name="builder"></param>
        /// <param name="currentCategory"></param>
        /// <param name="currentSubCategory"></param>
        /// <returns></returns>
        private bool ExportProgramToCubase(IProgram program, bool gmReached, StringBuilder builder, ref int currentCategory,
            ref int currentSubCategory)
        {
            var programIsGm = ((IProgramBank) program.Parent).Type == BankType.EType.Gm;

            gmReached = AddHeaderToCubase(program, gmReached, builder, 
                ref currentCategory, ref currentSubCategory, programIsGm);

            AddProgramInfoToCubase(program, builder, programIsGm);
            return gmReached;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="builder"></param>
        /// <param name="programIsGm"></param>
        private void AddProgramInfoToCubase(IProgram program, StringBuilder builder, bool programIsGm)
        {
            var hasSubCategories = SelectedPcgMemory.HasSubCategories;

            string patchId;
            if (programIsGm)
            {
                patchId = $"[p{2},{program.Index},{121},{((IProgramBank) program.Parent).PcgId - 6}]";
                //FUTURE: 6 is number of internal program banks for Kronos; might be different for other models.
                builder.AppendLine(patchId + " " + program.Id); // Name cannot be added since these are not in the PCG
            }
            else
            {
                switch (((IProgramBank)program.Parent).Type)
                {
                    case BankType.EType.UserExtended: // Fall through
                    case BankType.EType.User:
                        patchId =
                            $"[p{(hasSubCategories ? 3 : 2)},{program.Index},{0},{((IProgramBank) program.Parent).PcgId - 9}]";
                        builder.AppendLine(patchId + " " + program.Id + " " + program.Name);
                        break;

                    case BankType.EType.Int:
                        patchId =
                            $"[p{(hasSubCategories ? 3 : 2)},{program.Index},{0},{((IProgramBank) program.Parent).PcgId}]";
                        builder.AppendLine(patchId + " " + program.Id + " " + program.Name);
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="gmReached"></param>
        /// <param name="builder"></param>
        /// <param name="currentCategory"></param>
        /// <param name="currentSubCategory"></param>
        /// <param name="programIsGm"></param>
        /// <returns></returns>
        private bool AddHeaderToCubase(IProgram program, bool gmReached, StringBuilder builder, ref int currentCategory,
            ref int currentSubCategory, bool programIsGm)
        {
            if (programIsGm)
            {
                if (!gmReached)
                {
                    builder.AppendLine("[g1] GM Bank");
                    gmReached = true;
                }
            }
            else
            {
                AddNonGmProgramHeaderToCubase(program, builder, ref currentCategory, 
                    ref currentSubCategory);
            }
            return gmReached;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="builder"></param>
        /// <param name="currentCategory"></param>
        /// <param name="currentSubCategory"></param>
        private void AddNonGmProgramHeaderToCubase(IProgram program, StringBuilder builder, ref int currentCategory,
            ref int currentSubCategory)
        {
            var hasSubCategories = SelectedPcgMemory.HasSubCategories;
            var hasCategoryNames = (SelectedPcgMemory.Global != null);

            var category = program.GetParam(ParameterNames.ProgramParameterName.Category).Value;
            var subCategory = hasSubCategories ? (int) (program.GetParam(ParameterNames.ProgramParameterName.SubCategory).Value) : -1;

            // Check if a new header should be produced.
            if (currentCategory != category)
            {
                var categoryName = hasCategoryNames ? program.CategoryAsName : (string) ("Category" + category.ToString());
                builder.AppendLine("[g1] " + categoryName);
                currentCategory = category;
                currentSubCategory = -1; // Make sure sub category is also added here after.
            }

            // Also add sub category when category changed (if supported).
            if (hasSubCategories && (currentSubCategory != subCategory))
            {
                var subCategoryName = hasCategoryNames ? program.SubCategoryAsName : ("Sub Category" + subCategory);
                builder.AppendLine("[g2] " + subCategoryName);
                currentSubCategory = subCategory;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveAs"></param>
        /// <param name="saveToFile"></param>
        public void SaveFile(bool saveAs, bool saveToFile)
        {
            SelectedPcgMemory.SaveFile(saveAs, saveToFile);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Revert()
        {
            var fileName = SelectedMemory.FileName;
            var revert = true;

            if (!ExitCopyPasteMode())
            {
                return false;
            }

            if (SelectedMemory.IsDirty)
            {
                revert = (ShowMessageBox(string.Format(Strings.RevertWarning, fileName,
                    (PcgClipBoard.PastePcgMemory == SelectedPcgMemory) ?
                        $"{Strings.CutCopyPasteOperationUnfinishedWarning}. "
                        : string.Empty),
                    Strings.PcgTools,
                    WindowUtils.EMessageBoxButton.YesNo, WindowUtils.EMessageBoxImage.Warning, WindowUtils.EMessageBoxResult.No) ==
                    WindowUtils.EMessageBoxResult.Yes);
            }

            if (revert)
            {
                CloseWindow();
                SelectedMemory = null;
            }
            
            return revert;
        }


        /// <summary>
        /// </summary>
        /// <param name="exitMode"></param>
        /// <returns>True if continue closing.</returns>
        public override bool Close(bool exitMode)
        {
            var fileName = SelectedMemory.FileName;
            var continueClosing = true;

            if (!ExitCopyPasteMode())
            {
                return false;
            }
            
            if (SelectedMemory.IsDirty)
            {
                var result = ShowMessageBox(string.Format(Strings.SaveFile, fileName), Strings.PcgTools,
                    exitMode ? WindowUtils.EMessageBoxButton.YesNoCancel : WindowUtils.EMessageBoxButton.YesNo, 
                    WindowUtils.EMessageBoxImage.Warning, WindowUtils.EMessageBoxResult.No);
                    
                switch (result)
                {
                case WindowUtils.EMessageBoxResult.Yes:
                    SaveFile(false, true);
                    CloseWindow();
                    SelectedMemory = null;
                    break;

                case WindowUtils.EMessageBoxResult.No:
                    CloseWindow();
                    SelectedMemory = null;
                    break;

                case WindowUtils.EMessageBoxResult.Cancel:
                    continueClosing = false;
                    break;
                }
            }
            else
            {
                CloseWindow();
                SelectedMemory = null;
            }
            return continueClosing;
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _selectAllCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand SelectAllCommand
        {
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(param => SelectAll(),
                    param => CanExecuteSelectAllCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _editSelectedItemCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public ICommand EditSelectedItemCommand
        {
            get
            {
                return _editSelectedItemCommand ?? (_editSelectedItemCommand = new RelayCommand(param => EditSelectedItem(),
                    param => CanExecuteEditSelectedItemCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _exportToCubaseCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ExportToCubaseCommand
        {
            get
            {
                return _exportToCubaseCommand ?? (_exportToCubaseCommand = new RelayCommand(param => ExportToCubase(),
                    param => SelectedMemory is PcgMemory));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _runListGeneratorCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand RunListGeneratorCommand
        {
            get { return _runListGeneratorCommand ?? (_runListGeneratorCommand = new RelayCommand(param => RunListGenerator(), 
                param => SelectedMemory is PcgMemory)); }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _runProgramReferencesChangerCommand;
        

        /// <summary>
        /// Executable when the file has either filled/nonempty combis or set list slots.
        /// </summary>
        bool CanExecuteProgramReferenceChangerCommand => ((SelectedMemory is IPcgMemory) &&
                                                          (((SelectedPcgMemory.CombiBanks != null) && 
                                                            (SelectedPcgMemory.CombiBanks.CountFilledAndNonEmptyPatches > 0)) ||
                                                           (((SelectedPcgMemory.SetLists != null) && 
                                                             (SelectedPcgMemory.SetLists.CountFilledAndNonEmptyPatches > 0)))));


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand RunProgramReferencesChangerCommand
        {
            get
            {
                return _runProgramReferencesChangerCommand ??
                    (_runProgramReferencesChangerCommand = new RelayCommand(param => RunProgramReferencesChanger(),
                    param => CanExecuteProgramReferenceChangerCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteSelectAllCommand => (SelectedMemory is IPcgMemory);


        /// <summary>
        /// 
        /// </summary>
        void SelectAll()
        {
            if ((SelectedScopeSet == ScopeSet.Banks))
            {
                SelectAllBanks();
            }
            else
            {
                SelectAllPatches();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SelectAllPatches()
        {
            IBank bank = null;
            if (ProgramBanksSelected)
            {
                bank = SelectedPcgMemory.ProgramBanks.BankCollection.First(bankToCheck => bankToCheck.IsSelected);
            }
            else if (CombiBanksSelected)
            {
                bank = SelectedPcgMemory.CombiBanks.BankCollection.First(bankToCheck => bankToCheck.IsSelected);
            }
            else if (SetListsSelected)
            {
                bank = SelectedPcgMemory.SetLists.BankCollection.First(bankToCheck => bankToCheck.IsSelected);
            }
            // ELSE: add for drum kits and wave sequences.

            if (bank != null)
            {
                foreach (var patch in bank.Patches)
                {
                    patch.IsSelected = true;
                }
            }

            NumberOfSelectedPatches = Patches.Count(item => item.IsSelected);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SelectAllBanks()
        {
            if (ProgramBanksSelected)
            {
                SelectBanks(SelectedPcgMemory.ProgramBanks);
            }
            else if (CombiBanksSelected)
            {
                SelectBanks(SelectedPcgMemory.CombiBanks);
            }
            else if (SetListsSelected)
            {
                SelectBanks(SelectedPcgMemory.SetLists);
            }
            // ELSE: add for drum kits and wave sequences
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        private void SelectBanks(IBanks banks)
        {
            foreach (var bank in banks.BankCollection)
            {
                bank.IsSelected = true;
            }

            NumberOfSelectedPatches = Patches.Count(item => item.IsSelected);
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteEditSelectedItemCommand
        {
            get
            {
                // Can execute if 1 set list selected or 1 patch.
                return (((SelectedScopeSet == ScopeSet.Banks) && SetListsSelected && (Banks.Count(item => item.IsSelected) == 1)) ||
                  ((Patches != null) && 
                  (SelectedScopeSet == ScopeSet.Patches) && 
                  (Patches.Count(item => item.IsSelected) == 1) &&
                  ((!(Patches.First(item => item.IsSelected) is IDrumKit))) &&
                  ((!(Patches.First(item => item.IsSelected) is IDrumPattern))) && 
                  ((!(Patches.First(item => item.IsSelected) is IWaveSequence)))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void EditSelectedItem()
        {
            // Because of a double click, CanExecute... is skipped.
            if (!CanExecuteEditSelectedItemCommand)
            {
                return;
            }

            // Execute edit.
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                EditSelectedBanks();
            }
            else
            {
                EditSelectedPatches();
            }
            
            UpdateTimbresWindows();
        }


        /// <summary>
        /// 
        /// </summary>
        private void EditSelectedBanks()
        {
            DialogType dialogType;
            INavigable firstDialogItem = Banks.First();
            var banksSelected = Banks.Count(bank => bank.IsSelected);

            if (firstDialogItem is ProgramBank)
            {
                dialogType = (banksSelected == 1)
                    ? DialogType.EditSingleProgramBank
                    : DialogType.EditMultipleProgramBanks;
            }
            else if (firstDialogItem is CombiBank)
            {
                dialogType = (banksSelected == 1)
                    ? DialogType.EditSingleCombiBank
                    : DialogType.EditMultipleCombiBanks;
            }
            else if (firstDialogItem is SetList)
            {
                dialogType = (banksSelected == 1)
                    ? DialogType.EditSingleSetList
                    : DialogType.EditMultipleSetLists;
            }
            else
            {
                throw new ApplicationException("Illegal dialog type");
            }

            ShowDialog(dialogType, Banks.Where(bank => bank.IsSelected));
        }


        /// <summary>
        /// 
        /// </summary>
        private void EditSelectedPatches()
        {
            DialogType dialogType;
            INavigable firstDialogItem = Patches.First(patch => patch.IsSelected);
            
            var patchesSelected = Patches.Count(patch => patch.IsSelected);

            if (firstDialogItem is Program)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleProgram
                    : DialogType.EditMultiplePrograms;
            }
            else if (firstDialogItem is Combi)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleCombi
                    : DialogType.EditMultipleCombis;
            }
            else if (firstDialogItem is SetListSlot)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleSetListSlot
                    : DialogType.EditMultipleSetListSlots;
            }
            else if (firstDialogItem is DrumKit)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleDrumKit
                    : DialogType.EditMultipleDrumKits;
            }
            else if (firstDialogItem is DrumPattern)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleDrumPattern
                    : DialogType.EditMultipleDrumPatterns;
            }
            else if (firstDialogItem is WaveSequence)
            {
                dialogType = (patchesSelected == 1)
                    ? DialogType.EditSingleWaveSequence
                    : DialogType.EditMultipleWaveSequences;
            }
            else
            {
                throw new ApplicationException("Illegal dialog type");
            }

            ShowDialog(dialogType, Patches.Where(patch => patch.IsSelected));
        }


        /// <summary>
        /// 
        /// </summary>
        void RunListGenerator()
        {
            ShowListGenerator();
        }

        
        /// <summary>
        /// 
        /// </summary>
        void RunProgramReferencesChanger()
        {
            ShowProgramReferencesChanger();
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _cutCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand CutCommand
        {
            get
            {
                return _cutCommand ?? (_cutCommand = new RelayCommand(param => Cut(),
                    param => CanExecuteCutCommand));
            }
        }


        /// <summary>
        /// Impr: Same as CanExecuteCopyCommand.
        /// </summary>
        bool CanExecuteCutCommand => ((SelectedPcgMemory != null) &&
                                      (PcgClipBoard.IsEmpty || !PcgClipBoard.CutPasteSelected) &&
                                      (PcgClipBoard.IsEmpty || !PcgClipBoard.PasteDuplicatesExecuted) &&
                                      AreItemsSelected);


        /// <summary>
        /// 
        /// </summary>
        void Cut()
        {
            try
            {
                SetCursor(WindowUtils.ECursor.Wait);
                CopyPasteCut();
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        ICommand _copyCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand CopyCommand
        {
            get
            {
                return _copyCommand ?? (_copyCommand = new RelayCommand(param => Copy(),
                    param => CanExecuteCopyCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteCopyCommand => (SelectedPcgMemory != null) &&
                                      (PcgClipBoard.IsEmpty || !PcgClipBoard.CutPasteSelected) &&
                                      (PcgClipBoard.IsEmpty || !PcgClipBoard.PasteDuplicatesExecuted) &&
                                      AreItemsSelected;


        /// <summary>
        /// 
        /// </summary>
        private bool AreItemsSelected
        {
            get
            {
                return ((((SelectedScopeSet == ScopeSet.Banks) && (Banks.Count(item => item.IsSelected) > 0))) ||
                        ((SelectedScopeSet == ScopeSet.Patches) && (Patches.Count(item => item.IsSelected) > 0)));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void Copy()
        {
            try
            {
                SetCursor(WindowUtils.ECursor.Wait);
                _copyPasteCommands.CopyPasteCopy(PcgClipBoard, SelectedPcgMemory, SelectedScopeSet,
                    ProgramBanksSelected, CombiBanksSelected, SetListsSelected, DrumKitBanksSelected, 
                    DrumPatternBanksSelected, WaveSequenceBanksSelected, AllPatchesSelected,
                    Banks, Patches, false);
                OnPropertyChanged("PcgClipBoard");
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _pasteCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand PasteCommand
        {
            get
            {
                return _pasteCommand ?? (_pasteCommand = new RelayCommand(param => Paste(),
                    param => CanExecutePasteCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecutePasteCommand
        {
            get
            {
                var canExecute = SelectedPcgMemory != null;
                
                canExecute &= !PcgClipBoard.IsEmpty;
                
                canExecute &= !AllPatchesSelected;

                if (ProgramBanksSelected)
                {
                    // Check if in the clipboard there is at least one program with the synthesis type of one of the 
                    // selected banks (no need to check for selected paches, because paste can paste to unselected
                    // patches too.
                    var synthesisTypeIndices = new HashSet<int>();
                    foreach (var bank in Banks.Where(bank => bank.IsSelected))
                    {
                        synthesisTypeIndices.Add((int) ((IProgramBank) bank).BankSynthesisType);
                    }

                    var pastablePrograms = false;

                    for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
                    {
                        if (synthesisTypeIndices.Contains(index) && PcgClipBoard.Programs[index].CountUncopied > 0)
                        {
                            pastablePrograms = true;
                            break;
                        }
                    }

                    canExecute &= pastablePrograms;
                }
                else if (CombiBanksSelected)
                {
                    canExecute &= PcgClipBoard.Combis.CountUncopied > 0;
                }
                else if (SetListsSelected)
                {
                    canExecute &= PcgClipBoard.SetListSlots.CountUncopied > 0;
                }
                else if (DrumKitBanksSelected)
                {
                    canExecute &= PcgClipBoard.DrumKits.CountUncopied > 0;
                }
                else if (DrumPatternBanksSelected)
                {
                    canExecute &= PcgClipBoard.DrumPatterns.CountUncopied > 0;
                }
                else if (WaveSequenceBanksSelected)
                {
                    canExecute &= PcgClipBoard.WaveSequences.CountUncopied > 0;
                }
                else
                {
                    canExecute = false;
                }


                return canExecute;

                // MK (!PcgClipBoard.CutPasteSelected || (PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName)) &&  */
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void Paste()
        {
            try
            {
                SetCursor(WindowUtils.ECursor.Wait);

                if (!CheckPastePreconditionsAndWarnings())
                {
                    SetCursor(WindowUtils.ECursor.Arrow);
                    return;
                }

                PcgClipBoard.PastePcgMemory = SelectedPcgMemory;
                var infoText = _copyPasteCommands.CopyPastePaste(PcgClipBoard, SelectedPcgMemory, SelectedScopeSet,
                    ProgramBanksSelected, CombiBanksSelected, SetListsSelected, DrumKitBanksSelected,
                    DrumPatternBanksSelected, WaveSequenceBanksSelected,
                    AllPatchesSelected, Banks, Patches);

                OnPropertyChanged("PcgClipBoard");
                UpdateTimbresWindows();

                if (infoText != string.Empty)
                {
                    ShowMessageBox(infoText, Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, 
                        WindowUtils.EMessageBoxImage.Information, WindowUtils.EMessageBoxResult.Ok);
                }
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _assignCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand AssignCommand
        {
            get
            {
                return _assignCommand ?? (_assignCommand = new RelayCommand(param => Assign(),
                    param => CanExecuteAssignCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteAssignCommand
        {
            get
            {
                return ((SelectedPcgMemory != null) && 
                (SelectedScopeSet == ScopeSet.Patches) && 
                (Patches != null) &&
                (Patches.Count(item => item.IsSelected) == 1) &&
                (Patches.First(item => item.IsSelected) is ISetListSlot) &&
                (LastSelectedProgramOrCombi != null)); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Assign()
        {
            var setListSlot = Patches.First(patch => patch.IsSelected) as ISetListSlot;
            if (setListSlot != null)
            {
                setListSlot.UsedPatch = LastSelectedProgramOrCombi;
                LastSelectedProgramOrCombi = null;
            }
        }


        /// <summary>
        /// Tool tip for assign to set list slot.
        /// </summary>
        [Annotations.UsedImplicitly]
        public string AssignToolTipText
        {
            get
            {
                var text = string.Empty;
                if (LastSelectedProgramOrCombi != null)
                {
                    text =
                        $"{(LastSelectedProgramOrCombi is IProgram ? Strings.Program : Strings.Combi)} {LastSelectedProgramOrCombi.Id}: {LastSelectedProgramOrCombi.Name}";
                }
                return text;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _autoFillInSetListSlotNamesCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand AutoFillInSetListSlotNamesCommand
        {
            get
            {
                return _autoFillInSetListSlotNamesCommand ?? 
                    (_autoFillInSetListSlotNamesCommand = new RelayCommand(param => AutoFillInSetListSlotNames(),
                    param => CanExecuteAutoFillInSetListSlotNamesCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private bool CanExecuteAutoFillInSetListSlotNamesCommand
        {
            get
            {
                // All items should be either come from set lists or from (only) selected set list slots in all patches.
                return ((SelectedPcgMemory != null) &&
                        (SelectedScopeSet == ScopeSet.Patches) &&
                        (Patches != null) &&
                        (Patches.Count(item => item.IsSelected) > 0) &&
                        (Patches.All(item => !item.IsSelected || (item is ISetListSlot))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void AutoFillInSetListSlotNames()
        {
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in from bank in SelectedPcgMemory.ProgramBanks.BankCollection
                    where bank.IsSelected
                    from ISetListSlot patch in bank.Patches
                    where patch.UsedPatch != null
                    select patch)
                {
                    patch.Name = patch.UsedPatch.Name;
                }
            }
            else
            {
                // Iterate through patches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(
                    patch => ((ISetListSlot) patch).UsedPatch != null))
                {
                    patch.Name = ((ISetListSlot) patch).UsedPatch.Name;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool CheckPastePreconditionsAndWarnings()
        {
            var errorText = PastePreconditionsAndWarnings;
            if (errorText != string.Empty)
            {
                ShowMessageBox(errorText, Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error, 
                    WindowUtils.EMessageBoxResult.Ok);
                return false;
            }

            // If the pasting has been started elsewhere you have to clear the clipboard or continue in the other window.
            if ((PcgClipBoard.PasteDuplicatesExecuted) && (PcgClipBoard.PastePcgMemory != SelectedPcgMemory))
            {
                errorText = string.Format(Strings.PastingAlreadyExecutedWarning, PcgClipBoard.PastePcgMemory.FileName);
                ShowMessageBox(errorText, Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error,
                    WindowUtils.EMessageBoxResult.Ok);
                return false;
            }

            return true; // Continue copy & paste
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _exitCopyPasteModeCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ExitCopyPasteModeCommand
        {
            get
            {
                return _exitCopyPasteModeCommand ?? (_exitCopyPasteModeCommand = new RelayCommand(param => ExitCopyPasteMode(),
                    param => CanExecuteExitCopyPasteModeCommand));
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteExitCopyPasteModeCommand
        {
            get
            {
                var result = (PcgClipBoard.CutPasteSelected || PcgClipBoard.PasteDuplicatesExecuted) && !PcgClipBoard.IsEmpty;
                return result;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ExitCopyPasteMode()
        {
            if (!PcgClipBoard.IsPastingFinished)
            {
                if (ShowMessageBox($"{Strings.PastingUnfinishedWarning}\n{Strings.ContinueWarning}",
                    Strings.PcgTools,
                    WindowUtils.EMessageBoxButton.YesNo, WindowUtils.EMessageBoxImage.Exclamation, 
                    WindowUtils.EMessageBoxResult.No) == WindowUtils.EMessageBoxResult.No)
                {
                    return false;
                }
            }

            PcgClipBoard.Clear();
            OnPropertyChanged("PcgClipBoard");

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _recallCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand RecallCommand
        {
            get
            {
                return _recallCommand ?? (_recallCommand = new RelayCommand(param => Recall(),
                    param => CanExecuteRecallCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteRecallCommand => (SelectedPcgMemory != null) &&
                                        PcgClipBoard.IsEmpty && !PcgClipBoard.IsMemoryEmpty;


        /// <summary>
        /// 
        /// </summary>
        void Recall()
        {
            PcgClipBoard.Recall();
            OnPropertyChanged("PcgClipBoard");
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _moveUpCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand MoveUpCommand
        {
            get
            {
                return _moveUpCommand ?? (_moveUpCommand = new RelayCommand(param => MoveUp(),
                    param => CanExecuteMoveUpCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteMoveUpCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                    (SelectedScopeSet != ScopeSet.Banks) &&
                    !AllPatchesSelected &&
                    (Patches != null) && 
                    (Patches.Count(item => item.IsSelected) > 0) &&
                    !Patches.ToArray()[0].IsSelected && (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty);
            }
        }


        /// <summary>
        /// Returns true if there are no programs selected at the beginning of a bank where the previous bank has a different 
        /// synthesis engine type. For other types always return true.
        /// It can be assumed the last patch has not been selected.
        /// </summary>
        /// <returns></returns>
        bool ArePatchesAfterEngineChangeSelected
        {
            get
            {
                for (var index = 0; index < Patches.Count; index++) // Note: first is not selected
                {
                    var patch = Patches[index];
                    if (patch.IsSelected)
                    {
                        var previousPatch = Patches[index - 1];
                        if ((patch is IProgram) && (previousPatch is IProgram))
                        {
                            var patchBank = (patch as IProgram).Parent as IProgramBank;
                            var previousPatchBank = (previousPatch as IProgram).Parent as IProgramBank;

                            Debug.Assert(patchBank != null);
                            Debug.Assert(previousPatchBank != null);

                            if (patchBank.BankSynthesisType != previousPatchBank.BankSynthesisType)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

    
        /// <summary>
        /// 
        /// </summary>
        void MoveUp()
        {
            GetSelectedPatchListViewIndex(); //IMPR: Needed?

            for (var index = 0; index < Patches.Count; index++)
            {
                var patch = Patches[index];

                if (patch.IsSelected)
                {
                    var otherPatch = Patches[index - 1];
                    patch.PcgRoot.SwapPatch(patch, otherPatch);
                    patch.IsSelected = false;
                    otherPatch.IsSelected = true;
                    FixReferences(patch, otherPatch);
                }
            }
            MoveSelectedPatchesUp(); // IsSelected does not seem to work correctly

            UpdateTimbresWindows();
         }


        /// <summary>
        /// 
        /// </summary>
        ICommand _moveDownCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand MoveDownCommand
        {
            get
            {
                return _moveDownCommand ?? (_moveDownCommand = new RelayCommand(param => MoveDown(),
                    param => CanExecuteMoveDownCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteMoveDownCommand
        {
            get
            {
                var listViewBanksActiveForCommands = (SelectedScopeSet == ScopeSet.Banks);
                return (SelectedPcgMemory != null) &&
                    (SelectedScopeSet == ScopeSet.Patches) &&
                    !listViewBanksActiveForCommands &&
                    !AllPatchesSelected &&
                    (Patches != null) && 
                    (Patches.Count(item => item.IsSelected) > 0) &&
                    !Patches.ToArray()[Patches.Count - 1].IsSelected && 
                    ArePatchesBeforeEngineChangeSelected &&
                    (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty);
            }
        }

        
        /// <summary>
        /// Returns true if there are no programs selected at the end of a bank where the next bank has a different 
        /// synthesis engine type. For other types always return true.
        /// It can be assumed the last patch has not been selected.
        /// </summary>
        /// <returns></returns>
        bool ArePatchesBeforeEngineChangeSelected
        {
            get
            {
                for (var index = 0; index < Patches.Count; index++)
                {
                    var patch = Patches[index];
                    if (patch.IsSelected)
                    {
                        var nextPatch = Patches[index + 1];
                        if ((patch is IProgram) && (nextPatch is IProgram))
                        {
                            var patchBank = (patch as IProgram).Parent as IProgramBank;
                            var nextPatchBank = (nextPatch as IProgram).Parent as IProgramBank;

                            Debug.Assert(patchBank != null);
                            Debug.Assert(nextPatchBank != null);

                            if (patchBank.BankSynthesisType != nextPatchBank.BankSynthesisType)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void MoveDown()
        {
            GetSelectedPatchListViewIndex(); //IMPR: Needed?

            for (var index = Patches.Count - 1; index >= 0; index--)
            {
                var patch = Patches[index];

                if (patch.IsSelected)
                {
                    var otherPatch = Patches[index + 1];
                    patch.PcgRoot.SwapPatch(patch, otherPatch);
                    otherPatch.IsSelected = true;
                    patch.IsSelected = false;
                    FixReferences(patch, otherPatch);
                }
            }

            MoveSelectedPatchesDown(); // IsSelected does not seem to work correctly

            UpdateTimbresWindows();
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _changeVolumeCommand;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly]
        public ICommand ChangeVolumeCommand
        {
            get
            {
                return _changeVolumeCommand ?? (_changeVolumeCommand = new RelayCommand(param => ChangeVolume(),
                    param => CanExecuteChangeVolumeCommand));
            }
        }



        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteChangeVolumeCommand
        {
            get
            {
                // Memory selected
                return (SelectedPcgMemory != null) &&
                       // Patches set
                       (Patches != null) &&
                       // Combi banks or set list slots selected
                       (CombiBanksSelected || (SetListsSelected) &&
                       // Banks selected and at least one nonempty combi 
                       (((SelectedScopeSet == ScopeSet.Banks) &&
                         (Banks.Count(item => item.IsSelected) > 0) &&
                         (Banks.Sum(item => item.CountFilledAndNonEmptyPatches)) > 0)
                        ||
                        // Patches selected and at least one nonempty combi or set list slot
                        ((SelectedScopeSet == ScopeSet.Patches) &&
                         (Patches.Count(item => item.IsSelected && !item.IsEmptyOrInit) > 0))) &&
                       // Not busy with paste action
                       (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty) &&
                       // Only combis selected
                       Patches.All(item => !item.IsSelected || (item is ICombi) || (item is ISetListSlot)));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ChangeVolume()
        {
            var parameters = new ChangeVolumeParameters();
            var window = new ChangeVolumeWindow(parameters);
            window.ShowDialog();
            if ((!window.DialogResult.HasValue) || !window.DialogResult.Value)
            {
                return;
            }

            // In case of mapping, find min/max.
            int minValue = Int32.MaxValue;
            int maxValue = Int32.MinValue;

            if (parameters.ChangeType == ChangeVolumeParameters.EChangeType.SmartMapped)
            {
                if (SelectedScopeSet == ScopeSet.Banks)
                {
                    // Iterate through banks.
                    foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                        bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                    {
                        if (patch is ICombi)
                        {
                            minValue = Math.Min(minValue, ((ICombi)patch).GetMinimumVolume());
                            maxValue = Math.Max(minValue, ((ICombi)patch).GetMaximumVolume());
                        }
                        else if (patch is ISetListSlot)
                        {
                            minValue = Math.Min(minValue, ((ISetListSlot)patch).Volume);
                            maxValue = Math.Max(maxValue, ((ISetListSlot)patch).Volume);
                        }
                    }
                }
                else
                {
                    // Iterate through patches.
                    foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                    {
                        if (patch is ICombi)
                        {
                            minValue = Math.Min(minValue, ((ICombi)patch).GetMinimumVolume());
                            maxValue = Math.Max(minValue, ((ICombi)patch).GetMaximumVolume());
                        }
                        else if (patch is ISetListSlot)
                        {
                            minValue = Math.Min(minValue, ((ISetListSlot)patch).Volume);
                            maxValue = Math.Max(maxValue, ((ISetListSlot)patch).Volume);
                        }
                    }
                }
            }

            // Change volume.
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                    bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                {
                    if (patch is ICombi)
                    {
                        ((ICombi)patch).ChangeVolume(parameters, minValue, maxValue);
                    }
                    else if (patch is ISetListSlot)
                    {
                        ((ISetListSlot)patch).ChangeVolume(parameters, minValue, maxValue);
                    }
                }
            }
            else
            {
                // Iterate through patches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                {
                    if (patch is ICombi)
                    {
                        ((ICombi)patch).ChangeVolume(parameters, minValue, maxValue);
                    }
                    else if (patch is ISetListSlot)
                    {
                        ((ISetListSlot)patch).ChangeVolume(parameters, minValue, maxValue);
                    }
                }
            }

            UpdateTimbresWindows();
        }

               
        /// <summary>
        /// 
        /// </summary>
        ICommand _initAsMpeCombiCommand;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly]
        public ICommand InitAsMpeCombiCommand
        {
            get
            {
                return _initAsMpeCombiCommand ?? (_initAsMpeCombiCommand = new RelayCommand(param => InitAsMpeCombi(),
                    param => CanExecuteInitAsMpeCombiCommand));
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteInitAsMpeCombiCommand
        {
            get
            {
                // Memory selected
                return (SelectedPcgMemory != null) &&
                       // Patches set
                       (Patches != null) &&
                       // Combi banks selected
                       CombiBanksSelected &&
                       // Banks selected and at least one nonempty combi 
                       (((SelectedScopeSet == ScopeSet.Banks) &&
                         (Banks.Count(item => item.IsSelected) > 0) &&
                         (Banks.Sum(item => item.CountFilledAndNonEmptyPatches)) > 0)
                        ||
                        // Patches selected and at least one nonempty combi
                        ((SelectedScopeSet == ScopeSet.Patches) &&
                         (Patches.Count(item => item.IsSelected && !item.IsEmptyOrInit) > 0))) &&
                       // Not busy with paste action
                       (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty) &&
                       // Only combis selected
                       Patches.All(item => !item.IsSelected || (item is ICombi));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void InitAsMpeCombi()
        {
             if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                    bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                {
                    ((ICombi) patch).InitAsMpe();
                }
            }
            else
            {
                // Iterate through patches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                {
                    ((ICombi) patch).InitAsMpe();
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        ICommand _sortCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand SortCommand
        {
            get
            {
                return _sortCommand ?? (_sortCommand = new RelayCommand(param => Sort(),
                    param => CanExecuteSortCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteSortCommand => (SelectedPcgMemory != null) &&
                                      (AreMultipleItemsSelected &&
                                       (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty));


        /// <summary>
        /// 
        /// </summary>
        private bool AreMultipleItemsSelected
        {
            get
            {
                return (((SelectedScopeSet == ScopeSet.Banks) && (Banks.Count(bank => bank.IsSelected) > 0) &&
                         (Banks.Where(bank => bank.IsSelected).Sum(bank => bank.CountPatches) > 1)) ||
                        ((SelectedScopeSet == ScopeSet.Patches) && (Patches.Count(patch => patch.IsSelected) > 1)));
            }
        }


        /// <summary>
        /// SortMethod
        /// </summary>
        private void Sort()
        {
            var window = new SelectSortWindow(SelectedPcgMemory);
            window.ShowDialog();
            if ((!window.DialogResult.HasValue) || !window.DialogResult.Value)
            {
                return;
            }

            try
            {
                SetCursor(WindowUtils.ECursor.Wait);

                var listsToSort = BuildProcessList();

                // SortMethod each list.
                foreach (var list in listsToSort)
                {
                    var destination = list.ToList();

                    PatchSorter.SortBy(destination, window.SortOrder);
                    
                    Debug.Assert(list.Count == destination.Count);

                    MovePatchesForSorting(list, destination);
                }

                UpdateTimbresWindows();
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="destination"></param>
        private void MovePatchesForSorting(List<IPatch> list, List<IPatch> destination)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var destinationIndex = list.FindIndex(0, patch => patch == destination[index]);

                var destination1Index = destination.FindIndex(0, patch => patch == list[index]);
                var destination2Index = destination.FindIndex(0, patch => patch == list[destinationIndex]);

                if (destination1Index != destination2Index)
                {
                    SelectedPcgMemory.SwapPatch(list[index], list[destinationIndex]);

                    var temp = destination[destination1Index];
                    destination[destination1Index] = destination[destination2Index];
                    destination[destination2Index] = temp;

                    if (index != destinationIndex)
                    {
                        FixReferences(list[index], list[destinationIndex]); //MK
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<List<IPatch>> BuildProcessList()
        {
            // Create a list of list with patches, for programs: one per synthesis type, for combis/set list slot: only one.
            var listToProcess = new List<List<IPatch>>();

            if (ProgramBanksSelected)
            {
                BuildProgramBanksProcessList(listToProcess);
            }
            else // Combis or set list slots selected.
            {
                listToProcess.Add(new List<IPatch>());

                if (SelectedScopeSet == ScopeSet.Banks)
                {
                    // Iterate through banks.
                    if (CombiBanksSelected)
                    {
                        foreach (var patch in from IBank bank in SelectedPcgMemory.CombiBanks.BankCollection
                                              where Banks.Where(
                                                  elem => elem.IsSelected).Contains(bank)
                                              from patch in bank.Patches
                                              select patch)
                        {
                            listToProcess[0].Add(patch);
                        } 
                    }
                    else
                    {
                        // Set lists selected.
                        foreach (var patch in from IBank bank in SelectedPcgMemory.SetLists.BankCollection
                                              where Banks.Where(
                                                  elem => elem.IsSelected).Contains(bank)
                                              from patch in bank.Patches
                                              select patch)
                        {
                            listToProcess[0].Add(patch);
                        }
                    }
                }
                else
                {
                    // Iterate through patches.
                    if (Patches.Count > 0)
                    {
                        var selectedPatches = Patches.Where(patch => patch.IsSelected);
                        listToProcess[0].AddRange(selectedPatches);
                    }
                }
            }

            return listToProcess;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="listToProcess"></param>
        private void BuildProgramBanksProcessList(IList<List<IPatch>> listToProcess)
        {
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                listToProcess.Add(new List<IPatch>());
            }

            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var bank in SelectedPcgMemory.ProgramBanks.BankCollection)
                {
                    if (Banks.Where(elem => elem.IsSelected).Contains(bank))
                    {
                        var listToUse = listToProcess[(int) ((IProgramBank) bank).BankSynthesisType];
                        listToUse.AddRange(bank.Patches);
                    }
                }
            }
            else
            {
                // Iterate through patches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected))
                {
                    var listToUse = listToProcess[(int)((IProgramBank)patch.Parent).BankSynthesisType];
                    listToUse.Add(patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _setFavoriteCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly] 
        // ReSharper disable once UnusedMember.Global
        public ICommand SetFavoriteCommand
        {
            get
            {
                return _setFavoriteCommand ?? (_setFavoriteCommand = new RelayCommand(param => SetFavorite(true),
                    param => CanExecuteSetFavoriteCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteSetFavoriteCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                       (AreItemsSelected &&
                        (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty)) &&
                       SelectedPcgMemory.AreFavoritesSupported &&
                       !SetListsSelected && !DrumKitBanksSelected && !DrumPatternBanksSelected &&
                       !WaveSequenceBanksSelected &&
                       (Patches != null) &&
                       (Patches.All(item => !item.IsSelected || (item is IProgram) || (item is ICombi)));
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="favorite"></param>
        void SetFavorite(bool favorite)
        {
            Debug.Assert(SelectedPcgMemory.AreFavoritesSupported);

            if (SelectedScopeSet == ScopeSet.Banks)
            {
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(bank => bank.Patches))
                {
                    if (patch is IProgram)
                    {
                        ((IProgram) patch).GetParam(ParameterNames.ProgramParameterName.Favorite).Value = favorite;
                    }
                    else if (patch is ICombi)
                    {
                        ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.Favorite).Value = favorite;
                    }

                    patch.RaisePropertyChanged(string.Empty, false);
                }
            }
            else
            {
                foreach (var patch in Patches.Where(patch => patch.IsSelected))
                {
                    if (patch is IProgram)
                    {
                        ((IProgram)patch).GetParam(ParameterNames.ProgramParameterName.Favorite).Value = favorite;
                    }
                    else if (patch is ICombi)
                    {
                        ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.Favorite).Value = favorite;
                    }

                    patch.RaisePropertyChanged(string.Empty, false);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _unsetFavoriteCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand UnsetFavoriteCommand
        {
            get
            {
                return _unsetFavoriteCommand ?? (_unsetFavoriteCommand = new RelayCommand(param => SetFavorite(false),
                    param => CanExecuteUnsetFavoriteCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteUnsetFavoriteCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                       (AreItemsSelected &&
                        (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty)) &&
                       SelectedPcgMemory.AreFavoritesSupported && !SetListsSelected &&
                       !DrumKitBanksSelected && !DrumPatternBanksSelected && !WaveSequenceBanksSelected &&
                       (Patches != null) &&
                       Patches.All(item => !item.IsSelected || (item is IProgram) || (item is ICombi));
            }
        }

        
        ICommand _assignClearProgramCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand AssignClearProgramCommand
        {
            get
            {
                return _assignClearProgramCommand ?? (_assignClearProgramCommand = new RelayCommand(param => AssignClearProgram(),
                    param => CanExecuteAssignClearProgram));

            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteAssignClearProgram
        {
            get
            {
                return
                    // Valid memory.
                    ((SelectedPcgMemory != null) &&
                     // Patches selected (right part).
                     (SelectedScopeSet == ScopeSet.Patches) &&
                     // Exactly one program selected.
                     (Patches != null) &&
                     (Patches.Count(item => item.IsSelected) == 1) &&
                     (Patches.First(item => item.IsSelected) is IProgram) &&
                     // Combi banks present.
                     (SelectedPcgMemory.CombiBanks != null));
            }
        }


        private void AssignClearProgram()
        {
            var program = Patches.First(patch => patch.IsSelected) as IProgram;
            ((IPcgMemory) SelectedMemory).AssignedClearProgram = program;

            if (program != null)
            {
                ShowMessageBox(
                    string.Format(Strings.ClearProgramIsAssigned, program.Id, program.Name), Strings.PcgTools,
                    WindowUtils.EMessageBoxButton.Ok,
                    WindowUtils.EMessageBoxImage.Information, WindowUtils.EMessageBoxResult.None);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _clearCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(param => Clear(),
                    param => CanExecuteClearCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteClearCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                       (Patches != null) &&
                       ((((SelectedScopeSet == ScopeSet.Banks) && (Banks.Count(item => item.IsSelected) > 0)) ||
                         ((SelectedScopeSet == ScopeSet.Patches) && 
                         (Patches.Count(item => item.IsSelected) > 0))) &&
                        (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Clear()
        {
            SetCursor(WindowUtils.ECursor.Wait);

            var selectedPatches = ((SelectedScopeSet == ScopeSet.Banks)
                ? Banks.Where(bank => bank.IsSelected).SelectMany(bank => bank.Patches)
                : Patches.Where(patch => patch.IsSelected)).ToList();

            if (_clearCommands.ClearPatches(this, selectedPatches))
            {
                UpdateTimbresWindows();
            }

            if (SelectedScopeSet == ScopeSet.Banks)
            {
                foreach (var bank in Banks.Where(bank => bank.IsSelected))
                {
                    bank.Clear();
                }
            }
            SetCursor(WindowUtils.ECursor.Arrow);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _clearDuplicatesCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ClearDuplicatesCommand
        {
            get
            {
                return _clearDuplicatesCommand ?? (_clearDuplicatesCommand = new RelayCommand(param => ClearDuplicates(),
                    param => CanExecuteClearDuplicatesCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteClearDuplicatesCommand
        {
            get
            {
                //var x =             ((Patches != null) &&    Patches.All(
                //    item => !item.IsSelected || (item is IProgram) || (item is ICombi) || (item is ISetListSlot)));

                return (SelectedPcgMemory != null) &&
                       (Patches != null) &&
                       ((((SelectedScopeSet == ScopeSet.Banks) && (Banks.Count(item => item.IsSelected) > 0)) ||
                         ((SelectedScopeSet == ScopeSet.Patches) &&
                          (Patches.Count(item => item.IsSelected) > 0))) &&
                        (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty)) &&
                       Patches.All(
                           item => !item.IsSelected || (item is IProgram) || (item is ICombi) || (item is ISetListSlot));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ClearDuplicates()
        {
            SetCursor(WindowUtils.ECursor.Wait);

            var selectedPatches = ((SelectedScopeSet == ScopeSet.Banks)
                ? Banks.Where(bank => bank.IsSelected).SelectMany(bank => bank.Patches)
                : Patches.Where(patch => patch.IsSelected)).ToList();

            if (_clearCommands.ClearDuplicatesPatches(this, selectedPatches))
            {
                UpdateTimbresWindows();
            }
            
            SetCursor(WindowUtils.ECursor.Arrow);
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _compactCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand CompactCommand
        {
            get
            {
                return _compactCommand ?? (_compactCommand = new RelayCommand(param => Compact(),
                    param => CanExecuteCompactCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private bool CanExecuteCompactCommand => (SelectedPcgMemory != null) &&
                                         AreMultipleItemsSelected &&
                                         (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty) &&
                                         !AllPatchesSelected;


        /// <summary>
        /// Compacts the selected banks or patches by storing the empty patches at the end. Per bank type, create a list
        ///  (e.g. Kronos sampled/modeled program banks). Then compact each list.
        /// </summary>
        private void Compact()
        {
            SetCursor(WindowUtils.ECursor.Wait);

            var listsToCompact = BuildProcessList();
            
            // Compact each list.
            foreach (var list in listsToCompact)
            {
                CompactList(list);
            }

            UpdateTimbresWindows();

            SetCursor(WindowUtils.ECursor.Arrow);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        private void CompactList(IList<IPatch> list)
        {
            var destination = new List<IPatch>();
            var emptyPatches = 0;

            // Put in destination all non empty patches, null for empty patches.
            for (var index = 0; index < list.Count; index++)
            {
                var patch = list[index];
                if (patch.IsEmptyOrInit)
                {
                    emptyPatches++;
                    destination.Add(null);
                }
                else
                {
                    destination.Add(list[index - emptyPatches]);
                }
            }

            // For every null patch, use last locations.
            for (var index = 0; index < destination.Count; index++)
            {
                if (destination[index] == null)
                {
                    destination[index] = list[list.Count - emptyPatches];
                    emptyPatches--;
                }
            }

            Debug.Assert(emptyPatches == 0);
            Debug.Assert(list.Count == destination.Count);

            // Move all patches.
            for (var index = 0; index < list.Count; index++)
            {
                var listClosure = list;
                var indexClosure = index;
                var destinationIndex = destination.FindIndex(index, patch => patch == listClosure[indexClosure]);
                SelectedPcgMemory.SwapPatch(list[index], list[destinationIndex]);
                destination[destinationIndex] = destination[index];

                if (index != destinationIndex)
                {
                    FixReferences(list[index], list[destinationIndex]); //MK
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showTimbresCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowTimbresCommand
        {
            get
            {
                return _showTimbresCommand ?? (_showTimbresCommand = new RelayCommand(param => ShowTimbres(),
                    param => CanExecuteShowTimbresCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteShowTimbresCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                    (Patches != null) && 
                       (SelectedScopeSet == ScopeSet.Patches) &&
                       (Patches.Count(item => item.IsSelected) == 1) &&
                       (Patches.First(item => item.IsSelected) is ICombi) &&
                       (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowTimbres()
        {
            foreach (var selectedCombi in Patches.Where(item => item.IsSelected).Cast<ICombi>())
            {
                ShowTimbresWindow(selectedCombi,
                    Settings.Default.UI_CombiWindowWidth == 0 ? 700 : Settings.Default.UI_CombiWindowWidth,
                    Settings.Default.UI_CombiWindowHeight == 0 ? 500 : Settings.Default.UI_CombiWindowHeight);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _exportToHexCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ExportToHexCommand
        {
            get
            {
                return _exportToHexCommand ?? (_exportToHexCommand = new RelayCommand(param => ExportToHex(),
                    param => CanExecuteExportToHexCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteExportToHexCommand
        {
            get
            {
                return (SelectedPcgMemory != null) &&
                    (Patches != null) && 
                    (SelectedScopeSet == ScopeSet.Patches) &&
                    (Patches.Count(item => item.IsSelected) == 1) &&
                    (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _capitalizeNameCommand;



        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand CapitalizeNameCommand
        {
            get
            {
                return _capitalizeNameCommand ?? (_capitalizeNameCommand = new RelayCommand(param => CapitalizeName(),
                    param => CanExecuteCapitalizeNameCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteCapitalizeNameCommand => CanExecuteCaseCommand;


        /// <summary>
        /// 
        /// </summary>
        void CapitalizeName()
        {
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                    bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = patch.Name.ToUpperInvariant();
                }
            }
            else
            {
                // Iterate through patches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = patch.Name.ToUpperInvariant();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _titleCaseNameCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand TitleCaseNameCommand
        {
            get
            {
                return _titleCaseNameCommand ?? (_titleCaseNameCommand = new RelayCommand(param => TitleCaseName(),
                    param => CanExecuteTitleCaseNameCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteTitleCaseNameCommand => CanExecuteCaseCommand;


        /// <summary>
        /// Keep first character capitalized, rest title cased.
        /// </summary>
        void TitleCaseName()
        {
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                    bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(patch.Name.ToLowerInvariant());
                }
            }
            else
            {
                // Iterate through paches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(patch.Name.ToLowerInvariant());
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _decapitalizeNameCommand;

        
        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand DecapitalizeNameCommand
        {
            get
            {
                return _decapitalizeNameCommand ?? (_decapitalizeNameCommand = new RelayCommand(param => DecapitalizeName(),
                    param => CanExecuteDecapitalizeNameCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteDecapitalizeNameCommand => CanExecuteCaseCommand;


        /// <summary>
        /// Can execute an upper, lower or title case command.
        /// </summary>
        private bool CanExecuteCaseCommand
        {
            get
            {
                // Valid memory
                return ((SelectedPcgMemory != null) &&
                        (Patches != null) && 
                        // If bank: > 0 selected, if patches: > 0 selected and not all-patches selected or program, combi or
                        // set list slot.
                        ((SelectedScopeSet == ScopeSet.Banks) && (Banks.Count(item => item.IsSelected) > 0)) ||
                        ((SelectedScopeSet == ScopeSet.Patches) && (Patches.Count(item => item.IsSelected) > 0)) &&
                        // Not in copy/paste action.
                        (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty));
            }
        }


        /// <summary>
        /// Keep first character capitalized, rest decapitalized.
        /// </summary>
        void DecapitalizeName()
        {
            if (SelectedScopeSet == ScopeSet.Banks)
            {
                // Iterate through banks.
                foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(
                    bank => bank.Patches).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = patch.Name.ToLowerInvariant();
                }
            }
            else
            {
                // Iterate through paches.
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(patch => !patch.IsEmptyOrInit))
                {
                    patch.Name = patch.Name.ToLowerInvariant();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _setPcgFileAsMasterFileCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand SetPcgFileAsMasterFileCommand
        {
            get
            {
                return _setPcgFileAsMasterFileCommand ?? (_setPcgFileAsMasterFileCommand = 
                    new RelayCommand(param => SetFileAsMasterFile(),
                    param => CanExecuteSetPcgFileAsMasterFileCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteSetPcgFileAsMasterFileCommand => (SelectedPcgMemory != null) && 
                                                        SelectedPcgMemory.AreCategoriesEditable && 
                                                        SelectedPcgMemory.AreAllNeededProgramsCombisAndGlobalPresent;


        /// <summary>
        /// 
        /// </summary>
        void SetFileAsMasterFile()
        {
            SetPcgFileAsMasterFile(SelectedPcgMemory.Model, SelectedPcgMemory.FileName);

            ShowMessageBox(
                            string.IsNullOrEmpty(SelectedMemory.Model.OsVersionString) 
                            ? string.Format(Strings.SetPcgFileAsMasterFileShort, SelectedPcgMemory.FileName, SelectedMemory.Model.ModelAsString) 
                            : string.Format(Strings.SetPcgFileAsMasterFileLong, SelectedPcgMemory.FileName, SelectedMemory.Model.ModelAsString,
                             SelectedMemory.Model.OsVersionString),
                            Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok,
                            WindowUtils.EMessageBoxImage.Warning, WindowUtils.EMessageBoxResult.Ok);
            UpdateTimbresWindows();
        }


        /// <summary>
        /// 
        /// </summary>
        private void ExportToHex()
        {
            const int columnsPerLine = 8;
            var builder = new StringBuilder();

            foreach (var patch in Patches.Where(patch => patch.IsSelected))
            {
                builder.AppendLine($"{patch.Id}: {patch.Name}");
                var index = 0;
                while (index < patch.ByteLength)
                {
                    var charsInLine = new char[columnsPerLine];
                    var offset = patch.ByteOffset + index;
                    builder.Append($"{index:x8} ({index:d8}) {offset:x8}: ");

                    for (var column = 0; column < columnsPerLine; column++)
                    {
                        if ((column > 0) && (column % 4 == 0))
                        {
                            builder.Append(' ');
                        }

                        if (index + column < patch.ByteLength)
                        {
                            builder.Append($" {patch.Root.Content[offset + column]:x2}");
                            var charAtColumn = (char)patch.Root.Content[offset + column];
                            charsInLine[column] = char.IsControl(charAtColumn) ? ' ' : charAtColumn;
                        }
                    }
                    builder.Append(": ");
                    for (var column  = 0; column < columnsPerLine; column++)
                    {
                        if ((column > 0) && (column % 4 == 0))
                        {
                            builder.Append(' ');
                        }

                        builder.Append(charsInLine[column]);
                    }
                    builder.AppendLine();
                    index += columnsPerLine;
                }
                builder.AppendLine();
            }
            
            var dlg = new HexExportDlg(builder.ToString());
            dlg.ShowDialog();
        }


        /// <summary>
        /// 
        /// </summary>
        private IPatch _lastSelectedProgramOrCombi;


        /// <summary>
        /// 
        /// </summary>
        public IPatch LastSelectedProgramOrCombi
        {
            get { return _lastSelectedProgramOrCombi; }

            set
            {
                if (value != _lastSelectedProgramOrCombi)
                {
                    _lastSelectedProgramOrCombi = value;
                    OnPropertyChanged("LastSelectedProgramOrCombi");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void OnMemoryPropertyChanged(object o, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "IsDirty":
                    UpdateWindowTitle();
                    break;

                case "FileName":
                    UpdateWindowTitle();
                    break;
            }
        }


        /// <summary>
        /// Number of patches shown in the list view of patches.
        /// </summary>
        private int _numberOfPatches;


        /// <summary>
        /// Displayed in PCG winwdow.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly]
        public int NumberOfPatches
        {
            get { return _numberOfPatches; }
            
            set
            {
                if (value != _numberOfPatches)
                {
                    _numberOfPatches = value;
                    OnPropertyChanged("NumberOfPatches");
                }
            }
        }


        /// <summary>
        /// Number of selected patches shown in the list view of patches.
        /// </summary>
        private int _numberOfSelectedPatches;


        /// <summary>
        /// Displayed in PCG winwdow.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly]
        public int NumberOfSelectedPatches
        {
            get { return _numberOfSelectedPatches; }

            set
            {
                if (value != _numberOfSelectedPatches)
                {
                    _numberOfSelectedPatches = value;
                    OnPropertyChanged("NumberOfSelectedPatches");
                }
            }
        }


        /// <summary>
        /// Sets patches/banks.
        /// </summary>
        public void BanksChanged()
        {
            if (Banks != null)
            {
                Patches.Clear(); //> new ObservableCollectionEx<IPatch>();

                if (_selectedBanksType == SelectedBanksType.AllPatches)
                {
                    // Add all non empty patches.
                    foreach (var patch in from bank in Banks from patch in bank.Patches
                                          where patch.IsLoaded && !patch.IsEmptyOrInit  // Virtual patches have noninit name
                                          select patch)
                    {
                        Patches.Add(patch);
                    }
                }
                else
                {
                    // If no bank selected, select first.
                    /* var firstSelectedBank = Banks.FirstOrDefault(bank => bank.IsSelected);
                    if ((firstSelectedBank == null) && (Banks.Count > 0))
                    {
                        Banks[0].IsSelected = true;
                    }
                    */
                    // Add selected banks.
                    foreach (var patch in Banks.Where(bank => bank.IsSelected).SelectMany(bank => bank.Patches))
                    {
                        Patches.Add(patch);
                    }
                }

                NumberOfPatches = Patches.Count;
                NumberOfSelectedPatches = Patches.Count(item => item.IsSelected);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _doubleToSingleKeyboardCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand DoubleToSingleKeyboardCommand
        {
            get
            {
                return _doubleToSingleKeyboardCommand ?? (_doubleToSingleKeyboardCommand = 
                    new RelayCommand(param => DoubleToSingleKeyboard(), param => CanExecuteDoubleToSingleKeyboardCommand));
            }
        }


        /// <summary>
        /// PCG with set lists, combis and programs.
        /// </summary>
        public bool CanExecuteDoubleToSingleKeyboardCommand => (SelectedPcgMemory != null) &&
                                                               (SelectedPcgMemory.SetLists != null) &&
                                                               (SelectedPcgMemory.SetLists.CountFilledAndNonEmptyPatches > 0) &&
                                                               (SelectedPcgMemory.CombiBanks.CountFilledAndNonEmptyPatches > 0);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void DoubleToSingleKeyboard()
        {
            _doubleToSingleKeyboardCommands.Execute(this);
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _editParameterCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand EditParameterCommand
        {
            get
            {
                return _editParameterCommand ?? (_editParameterCommand = new RelayCommand(param => EditParameter(),
                    param => CanExecuteEditParameterCommand));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void EditParameter()
        {
            var selectedPatches = ((SelectedScopeSet == ScopeSet.Banks)
                ? Banks.Where(bank => bank.IsSelected).SelectMany(bank => bank.Patches)
                : Patches.Where(patch => patch.IsSelected)).ToList();

            var selectedPatchesToEdit = new ObservableCollectionEx<IPatch>();
            foreach (var patch in selectedPatches)
            {
                selectedPatchesToEdit.Add(patch);
            }

            EditParameterWindow(selectedPatchesToEdit);
        }


        /// <summary>
        /// 
        /// </summary>
        bool CanExecuteEditParameterCommand
        {
            get
            {
                var selectedPatches = Patches.Where(item => item.IsSelected);
                var enumerable = selectedPatches as IPatch[] ?? selectedPatches.ToArray();
                var selectedPrograms = enumerable.Count(item => item is IProgram);
                var selectedCombis = enumerable.Count(item => item is ICombi);
                var selectedSetListSlots = enumerable.Count(item => item is ISetListSlot);
                var selectedDrumKits = enumerable.Count(item => item is IDrumKit);
                var selectedDrumPatterns = enumerable.Count(item => item is IDrumPattern);
                var selectedWaveSequences = enumerable.Count(item => item is IWaveSequence);

                // Only if either programs, combis or set list slots are selected.
                return (SelectedPcgMemory != null) &&
                     (Patches != null) && 
                    ((((selectedPrograms > 0) && (selectedCombis == 0) && (selectedSetListSlots == 0)) || // Only programs,
                     ((selectedPrograms == 0) && (selectedCombis > 0) && (selectedSetListSlots == 0)) || // combis OR
                     ((selectedPrograms == 0) && (selectedCombis == 0) && (selectedSetListSlots > 0))) && // set list slots
                    (selectedDrumKits == 0) && (selectedDrumPatterns == 0) && (selectedWaveSequences == 0) && // No drum kits / patterns / wave sequences
                    (!PcgClipBoard.PasteDuplicatesExecuted || PcgClipBoard.IsEmpty)); // No paste in progress
            }
        }

    }
}
