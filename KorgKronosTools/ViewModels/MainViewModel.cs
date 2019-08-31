// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Common.Mvvm;
using Common.Utils;
using Common.Extensions;
using PcgTools.ClipBoard;
using PcgTools.MasterFiles;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Properties;
using PcgTools.PcgToolsResources;
using PcgTools.Songs;
using PcgTools.ViewModels.Commands;
using WPF.MDI;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MainViewModel : ViewModel, IMainViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Version = "3.1.0";


        /// <summary>
        /// 
        /// </summary>
        public enum ChildWindowType
        {
            Pcg,
            Song,
            MasterFiles
        } ;


        /// <summary>
        /// 
        /// </summary>
        string _appTitle;
        // ReSharper disable once MemberCanBePrivate.Global
        [UsedImplicitly] public string AppTitle
        {
            // ReSharper disable once UnusedMember.Global
            get { return _appTitle; }
            set {  if (_appTitle != value) { _appTitle = value; OnPropertyChanged("AppTitle"); } }
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateAppTitle()
        {
            AppTitle = $"{Strings.PcgTools} {Version}  ©2011-2019 Michel Keijzers";
        }


        /// <summary>c
        /// 
        /// </summary>
        public override IMemory SelectedMemory
        {
            get { return base.SelectedMemory; }
            set
            {
                if (base.SelectedMemory != value)
                {
                    if (base.SelectedMemory != null)
                    {
                        base.SelectedMemory.PropertyChanged -= OnSelectedMemoryChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += OnSelectedMemoryChanged;
                    }
                    base.SelectedMemory = value;
                    RecalculateStatusBar();
                }
            }
        }


        /// <summary>
        /// Returns file formats for FileOpen and FileSaveAs dialog.
        /// Use of multiple extensions in one filter: Check: Images (*.png,*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*"
        /// </summary>
        public MainViewModel()
        {
            PcgClipBoard = new PcgClipBoard();
            ChildWindows = new ObservableCollection<IChildWindow>();
            SelectedTheme = Theme.Aero;
            MasterFiles.MasterFiles.Instances.Set(this);
            OpenedPcgWindows = new OpenedPcgWindows();

            _fileFormats = string.Format(
                "Korg PCG {0} (*.pcg)|*.pcg|" +
                "Korg Sysex {0} (*.syx)|*.syx|" +
                "Korg Midi {0} (*.mid)|*.mid|" +
                "Korg Song {0} (*.sng)|*.sng|" +
                "Korg 01W series {0} (*.01p,*.01w,*.all,*.mid,*.raw,*.syx)|*.01p;*.01w;*.all;*.mid;*.raw;*.syx|" +
                "Korg 03R/W {0} (*.mid,*.syx)|*.mid;*.syx|" +
                "Korg microKorg {0} (*.syx)|*.syx|" +
                "Korg microKorg XL {0} (*.mkxl_all,*.syx)|*.mkxl_all;*.syx|" +
                "Korg microKorg XL Plus {0} (*.mkxlp_prog)|*.mkxlp_prog|" +
                "Korg microSTATION {0} (*.pcg)|*.pcg|" +
                "Korg Karma {0} (*.pcg)|*.pcg|" +
                "Korg Krome {0} (*.pcg)|*.pcg|" +
                "Korg Krome EX {0} (*.pcg)|*.pcg|" +
                "Korg Kronos series {0} (*.pcg)|*.pcg|" +
                "Korg Kross {0} (*.pcg)|*.pcg|" +
                "Korg Kross {0} (*.pcg,*.KRSall,*.KRSapr,*.KRSbpr,*.KRSpr,*.KRSacm,*.KRSbcm,*.KRScm)|" +
                                "*.pcg;*.KRSall;*.KRSapr;*.KRSbpr;*.KRSpr;*.KRSacm;*.KRSbcm;*.KRScm|" +
                "Korg Kross 2 {0} (*.pcg)|*.pcg|" +
                "Korg M1 series {0} (*.syx,*.mid)|*.syx;*.mid|" +
                "Korg M3 series {0} (*.pcg)|*.pcg|" +
                "Korg M3R {0} (*.syx)|*.syx|" +
                "Korg M50 {0} (*.pcg)|*.pcg|" +
                "Korg MS2000 {0} (*.prg,*.syx,*.lib;*.mid;*.exl)|*.prg;*.syx;*.lib;*.mid;*.exl|" +
                "Korg Oasys {0} (*.pcg)|*.pcg|" +
                "Korg Triton series {0} (*.pcg)|*.pcg|" +
                "Korg Trinity series {0} (*.pcg|*.pcg|" +
                "Korg T1/T2/T3 {0} (*.syx,*.mid)|*.syx;*.mid|" +
                "Korg Z1 {0} (*.syx,*.mid)|*.syx;*.mid|" +
                "{1} {0} (*.*)|*.*",
                Strings.FileOpenDialogFiles,
                Strings.All);

            // Create folder for backup.
            var dir = PcgToolsApplicationDataDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        IViewModel _currentChildViewModel;


        /// <summary>
        /// 
        /// </summary>
        public IViewModel CurrentChildViewModel
        {
            [UsedImplicitly]
            // ReSharper disable once MemberCanBePrivate.Global
            get { return _currentChildViewModel; }
            set
            {
                if (_currentChildViewModel != value)
                {
                    if (_currentChildViewModel != null)
                    {
                        _currentChildViewModel.PropertyChanged -= OnChildViewModelChanged;
                    }
                    _currentChildViewModel = value;
                    OnPropertyChanged("CurrentChildViewModel");

                    if (_currentChildViewModel != null)
                    {
                        _currentChildViewModel.PropertyChanged += OnChildViewModelChanged;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private readonly string _fileFormats;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IPcgViewModel FindPcgViewModelWithName(string fileName)
        {
            var childs = (from child in ChildWindows
                where (child.ViewModel is IPcgViewModel) &&
                      (child.ViewModel.SelectedMemory != null) &&
                      (child.ViewModel.SelectedMemory.FileName.IsEqualFileAs(fileName.ToUpper()))
                select (IPcgViewModel) (child.ViewModel));

            return childs.FirstOrDefault();
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbModel;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarModel
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbModel; }

            private set
            {
                if (value != _statbModel)
                {
                    _statbModel = value;
                    OnPropertyChanged("StatusBarModel");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbFileType;

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarFileType
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbFileType; }
            set
            {
                if (value != _statbFileType)
                {
                    _statbFileType = value;
                    OnPropertyChanged("StatusBarFileType");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbPrograms;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarPrograms
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbPrograms; }
            private set
            {
                if (value != _statbPrograms)
                {
                    _statbPrograms = value;
                    OnPropertyChanged("StatusBarPrograms");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbCombis;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarCombis
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbCombis; }
            private set
            {
                if (value != _statbCombis)
                {
                    _statbCombis = value;
                    OnPropertyChanged("StatusBarCombis");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbDrumKits;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global    
        public string StatusBarDrumKits

        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbDrumKits; }
            private set
            {
                if (value != _statbDrumKits)
                {
                    _statbDrumKits = value;
                    OnPropertyChanged("StatusBarDrumKits");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbDrumPatterns;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global    
        public string StatusBarDrumPatterns
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbDrumPatterns; }
            private set
            {
                if (value != _statbDrumPatterns)
                {
                    _statbDrumPatterns = value;
                    OnPropertyChanged("StatusBarDrumPatterns");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbWaveSequences;
        
        
        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarWaveSequences
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbWaveSequences; }
            private set 
            {
                if (value != _statbWaveSequences)
                {
                    _statbWaveSequences = value;
                    OnPropertyChanged("StatusBarWaveSequences");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbSetLists;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarSetLists
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbSetLists; }
            private set
            {
                if (value != _statbSetLists)
                {
                    _statbSetLists = value;
                    OnPropertyChanged("StatusBarSetLists");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbClipBoard;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarClipBoard
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbClipBoard; }
            private set
            {
                if (value != _statbClipBoard)
                {
                    _statbClipBoard = value;
                    OnPropertyChanged("StatusBarClipBoard");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbSongs;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable MemberCanBePrivate.Global
        public string StatusBarSongs
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbSongs; }
            private set
            {
                if (value != _statbSongs)
                {
                    _statbSongs = value;
                    OnPropertyChanged("StatusBarSongs");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        string _statbSamples;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string StatusBarSamples
        {
            // ReSharper disable once UnusedMember.Global
            get { return _statbSamples; }
            private set
            {
                if (value != _statbSamples)
                {
                    _statbSamples = value;
                    OnPropertyChanged("StatusBarSamples");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        PcgClipBoard _pcgClipBoard;


        /// <summary>
        /// 
        /// </summary>
        public PcgClipBoard PcgClipBoard
        {
            get { return _pcgClipBoard; }
            private set
            {
                if (value != _pcgClipBoard)
                {
                    _pcgClipBoard = value;
                    OnPropertyChanged("PcgClipBoard");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ObservableCollection<IChildWindow> _childWindows;


        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<IChildWindow> ChildWindows
        {
            get { return _childWindows; }
            private set
            {
                if (_childWindows != value)
                {
                    _childWindows = value;
                    OnPropertyChanged("ChildWindows");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _openFileCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(param => OpenFile(), param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _saveFileCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand SaveFileCommand
        {
            get { return _saveFileCommand ?? (_saveFileCommand = new RelayCommand(param => SaveFile(), 
                param => (CurrentChildViewModel?.SelectedMemory is PcgMemory && CurrentChildViewModel.SelectedMemory.IsDirty))); }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _saveAsFileCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand SaveAsFileCommand
        {
            get
            {
                return _saveAsFileCommand ?? (_saveAsFileCommand = new RelayCommand(param => SaveAsFile(),
                    param => (CurrentChildViewModel?.SelectedMemory is IPcgMemory))); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _revertToSavedFileCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand RevertToSavedFileCommand
        {
            get
            {
                return _revertToSavedFileCommand ??
                       (_revertToSavedFileCommand = new RelayCommand(param => RevertToSavedFile(),
                           param =>(CurrentChildViewModel?.SelectedMemory is IPcgMemory)));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _closeFileCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand CloseFileCommand
        {
            get
            {
                return _closeFileCommand ?? (_closeFileCommand = new RelayCommand(param => CloseFile(),
                    param => ((CurrentChildViewModel != null) &&
                              ((CurrentChildViewModel.SelectedMemory is ISongMemory) ||
                               (CurrentChildViewModel.SelectedMemory is IPcgMemory)))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void CloseFile()
        {
            CurrentChildViewModel.Close(false);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _exitCommand;
        
        
        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand = new RelayCommand(param => Close(), param => true));
            }
        }
        

        /// <summary>
        /// Default is 0: PCG.
        /// </summary>
        int _lastUsedFilterType;


        /// <summary>
        /// 
        /// </summary>
        void OpenFile()
        {
            var result = OpenFileDialog(Strings.SelectFileToRead, _fileFormats, _lastUsedFilterType, true);

            if (!result.Success)
            {
                return;
            }

            _lastUsedFilterType = result.FilterIndex;

            try
            {
                SetCursor(WindowUtils.ECursor.Wait);

                foreach (var fileNameToOpen in result.Files)
                {
                    CheckAndOpenFile(fileNameToOpen);
                }
            }
            catch (NotSupportedException exc)
            {
                ShowMessageBox(
                    string.Format(Strings.FileOpenException,
                        exc.Message + "\n\n" + exc.InnerException, exc.StackTrace), Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok,
                        WindowUtils.EMessageBoxImage.Error, WindowUtils.EMessageBoxResult.Ok);
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }

        }


        /// <summary>
        /// Check if there are PCG windows opened with the same file name, and open file.
        /// </summary>
        /// <param name="fileNameToOpen"></param>
        public void CheckAndOpenFile(string fileNameToOpen)
        {
            // Check if there are PCG windows opened with the same file name.
            if (IsFileAlreadyOpened(fileNameToOpen))
            {
                ShowMessageBox(
                    string.Format(Strings.OpenedDuplicateWarning, fileNameToOpen),
                    Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Warning,
                    WindowUtils.EMessageBoxResult.Ok);
            }

            // Open/show file.
            ReadAndShowFile(fileNameToOpen, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool IsFileAlreadyOpened(string fileName)
        {
            return (ChildWindows.Any(child => (child.Memory != null) &&
                (string.Equals(child.Memory.FileName, fileName, StringComparison.CurrentCultureIgnoreCase))));
        }


        /// <summary>
        /// Reads and shows the requested file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="checkAutoLoadMasterFileSetting">True for checking settings for autoloading the master file</param>
        void ReadAndShowFile(string fileName, bool checkAutoLoadMasterFileSetting)
        {
#if !DEBUG
            try
#endif
            {
                PcgFileCommands.LoadFileAndMasterFile(this, fileName, checkAutoLoadMasterFileSetting);
            }
#if !DEBUG
            catch (IOException exception)
            {
                ShowMessageBox(String.Format(Strings.FileOpenException, fileName, exception.Message, exception.StackTrace),
                                Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error, 
                                WindowUtils.EMessageBoxResult.Ok);
            }
            catch (ApplicationException exception)
            {
                ShowMessageBox(String.Format(Strings.FileOpenException, fileName, exception.Message, exception.StackTrace),
                                Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error,
                                WindowUtils.EMessageBoxResult.Ok);
            }
#endif
        }

        
        /// <summary>
        /// 
        /// </summary>
        void SaveFile()
        {
            SaveToFile(false);
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveAsFile()
        {
            var extension = Path.GetExtension(SelectedMemory.FileName);
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.Remove(0, 1); // Remove dot at first position
            }
            var filter = (extension == string.Empty) ?
                $"No Extension {Strings.FileSaveDialogFile.ToLower()} (*)|*"
                :
                             string.Format("{0} {1} (*.{0})|*.{0}", extension, Strings.FileSaveDialogFile.ToLower());

            dynamic result = SaveFileDialog(
                Strings.SelectFileToSaveTo,
                filter, CurrentChildViewModel.SelectedMemory.FileName);

            if (result.Success)
            {
                CurrentChildViewModel.SelectedMemory.FileName = result.Files[0];
                ((IPcgViewModel) CurrentChildViewModel).UpdateWindowTitle();
                SaveToFile(true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveAsFile"></param>
        private void SaveToFile(bool saveAsFile)
        {
            try
            {
                SetCursor(WindowUtils.ECursor.Wait);
                ((IPcgViewModel) CurrentChildViewModel).SaveFile(saveAsFile, true);
            }
            finally
            {
                SetCursor(WindowUtils.ECursor.Arrow);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void RevertToSavedFile()
        {
            var fileName = CurrentChildViewModel.SelectedMemory.FileName;
            if (CurrentChildViewModel.Revert())
            {
                ReadAndShowFile(fileName, false);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        public Action UpdateSelectedMemory { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Func<string, string, int, bool, dynamic> OpenFileDialog { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Func<string, string, string, dynamic> SaveFileDialog { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action<WindowUtils.ECursor> SetCursor { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum WindowType
        {
            Settings,
            About,
            ChangeVolume,
            ExternalLinksKorgRelated,
            ExternalLinksContributors,
            ExternalLinksVideoCreators,
            ExternalLinksDonators,
            ExternalLinksTranslators,
            ExternalLinksThirdParties,
            ExternalLinksOasysVoucherCodeSponsorsWindow,
            ExternalLinksPersonal
        }

        
        /// <summary>
        /// 
        /// </summary>
        public Action<WindowType> ShowDialog { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Func<string, string, WindowUtils.EMessageBoxButton, WindowUtils.EMessageBoxImage, WindowUtils.EMessageBoxResult, 
            WindowUtils.EMessageBoxResult>  ShowMessageBox { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action<string> StartProcess { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action GotoNextWindow { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Action GotoPreviousWindow { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Func<string, ChildWindowType, IMemory, int, int, MdiChild> CreateMdiChildWindow { get; set;}


        /// <summary>
        /// 
        /// </summary>
        public Action CloseView;


        /// <summary>
        /// 
        /// </summary>
        void RecalculateStatusBar()
        {
            // Update status bar.
            if (SelectedMemory == null)
            {
                EmptyStatusBar();
                return;
            }

            StatusBarModel = SelectedMemory.Model.ModelAndVersionAsString;
            StatusBarFileType = Memory.FileTypeAsString(SelectedMemory.MemoryFileType);

            var memory = SelectedMemory as IPcgMemory;
            if (memory != null)
            {
                UpdateStatusBarForPcg(memory);
            }
            else
            {
                UpdateStatusBarForSong();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        private void UpdateStatusBarForPcg(IPcgMemory memory)
        {
            StatusBarSongs = string.Empty;
            StatusBarSamples = string.Empty;
            var pcgMemory = memory;

            RecalculateStatusBarPrograms(pcgMemory);

            RecalculateStatusBarCombis(pcgMemory);

            RecalculateStatusBarSetListSlots(pcgMemory);

            RecalculateStatusBarDrumKits(pcgMemory);

            RecalculateStatusBarDrumPatterns(pcgMemory);

            RecalculateStatusBarWaveSequences(pcgMemory);

            // Build clipboard text.
            var nrPrograms = 0;
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                nrPrograms += PcgClipBoard.Programs[index].CountUncopied;
            }

            var nrCombis = PcgClipBoard.Combis.CountUncopied;

            var nrSetListSlots = PcgClipBoard.SetListSlots.CountUncopied;

            var nrDrumKits = PcgClipBoard.DrumKits.CountUncopied;

            var nrDrumPatterns = PcgClipBoard.DrumPatterns.CountUncopied;

            var nrWaveSequences = PcgClipBoard.WaveSequences.CountUncopied;

            // Perhaps only add to statusbar string counts are > 0 to prevent status bar string to become unnecessarily long.
            if ((PcgClipBoard.IsEmpty) ||
                ((nrPrograms == 0) && (nrCombis == 0) && (nrSetListSlots == 0) && (nrDrumKits == 0) && (nrDrumPatterns == 0) && (nrWaveSequences == 0)))
            {
                StatusBarClipBoard = $"{Strings.Clipboard}: {string.Empty}";
            }
            else
            {
                var builder = UpdateStatusBarForClipBoard(nrCombis, nrSetListSlots, nrDrumKits, nrDrumPatterns, nrWaveSequences);

                StatusBarClipBoard = builder.ToString();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nrCombis"></param>
        /// <param name="nrSetListSlots"></param>
        /// <param name="nrDrumKits"></param>
        /// <param name="nrDrumPatterns"></param>
        /// <param name="nrWaveSequences"></param>
        /// <returns></returns>
        private StringBuilder UpdateStatusBarForClipBoard(int nrCombis, int nrSetListSlots, int nrDrumKits, int nrDrumPatterns, int nrWaveSequences)
        {
            var builder = new StringBuilder($"{Strings.Clipboard}: {PcgClipBoard.Model.ModelAndVersionAsString}: ");

            // Add programs.
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                var uncopied = PcgClipBoard.Programs[index].CountUncopied;
                var programsString = (uncopied == 1) ? Strings.Program : Strings.Programs;
                if (uncopied > 0)
                {
                    builder.Append(
                        $" {uncopied} {ProgramBank.SynthesisTypeAsString((ProgramBank.SynthesisType) index)} {programsString} ");
                }
            }

            if (nrCombis > 0)
            {
                builder.Append($" {nrCombis} {(nrCombis == 1 ? Strings.Combi : Strings.Combis)}");
            }

            if (nrSetListSlots > 0)
            {
                builder.Append($" {nrSetListSlots} {(nrSetListSlots == 1 ? Strings.SetListSlot : Strings.SetListSlots)}");
            }

            if (nrDrumKits > 0)
            {
                builder.Append($" {nrDrumKits} {(nrDrumKits == 1 ? Strings.DrumKit : Strings.DrumKits)}");
            }

            if (nrDrumPatterns > 0)
            {
                builder.Append($" {nrDrumPatterns} {(nrDrumPatterns == 1 ? Strings.DrumPattern : Strings.DrumPatterns)}");
            }

            if (nrWaveSequences > 0)
            {
                builder.Append(
                    $" {nrWaveSequences} {(nrWaveSequences == 1 ? Strings.WaveSequence : Strings.WaveSequences)}");
            }
            return builder;
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateStatusBarForSong()
        {
            var selectedMemory = SelectedMemory as SongMemory;
            if (selectedMemory != null)
            {
                StatusBarPrograms = string.Empty;
                StatusBarCombis = string.Empty;
                StatusBarSetLists = string.Empty;
                StatusBarDrumKits = string.Empty;
                StatusBarDrumPatterns = string.Empty;
                StatusBarWaveSequences = string.Empty;

                var songMemory = selectedMemory;
                StatusBarSongs =
                    $"{songMemory.Songs.SongCollection.Count} {(songMemory.Songs.SongCollection.Count == 1 ? Strings.Song : Strings.Songs)}";
                StatusBarSamples =
                    $"{songMemory.Regions.RegionsCollection.Count} {(songMemory.Regions.RegionsCollection.Count == 1 ? Strings.Sample : Strings.Samples)}";
            }
            else
            {
                throw new ApplicationException("Illegal type");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void EmptyStatusBar()
        {
            StatusBarModel = string.Empty;
            StatusBarFileType = string.Empty;
            StatusBarPrograms = string.Empty;
            StatusBarCombis = string.Empty;
            StatusBarSetLists = string.Empty;
            StatusBarDrumKits = string.Empty;
            StatusBarDrumPatterns = string.Empty;
            StatusBarWaveSequences = string.Empty;
            StatusBarClipBoard = string.Empty;
            StatusBarSongs = string.Empty;
            StatusBarSamples = string.Empty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        private void RecalculateStatusBarWaveSequences(IPcgMemory pcgMemory)
        {
            StatusBarWaveSequences = string.Empty;
            if (pcgMemory.WaveSequenceBanks != null)
            {
                var waveSequences = pcgMemory.WaveSequenceBanks.CountFilledPatches;
                var waveSequenceBanks = pcgMemory.WaveSequenceBanks.CountFilledBanks;

                switch (waveSequences)
                {
                    case 0:
                        break;

                    case 1:
                        switch (waveSequenceBanks)
                        {
                            case 1:
                                StatusBarWaveSequences = Strings.Stb_OneWaveSequenceInOneBank;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneWaveSequenceInMultipleBanks, waveSequenceBanks);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (waveSequenceBanks)
                        {
                            case 1:
                                StatusBarWaveSequences = string.Format(
                                    Strings.Stb_MultipleWaveSequencesInOneBank, waveSequences);
                                break;

                            default: // > 1
                                StatusBarWaveSequences = string.Format(
                                    Strings.Stb_MultipleWaveSequencesInMultipleBanks, waveSequences,
                                    waveSequenceBanks);
                                break;
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        private void RecalculateStatusBarDrumKits(IPcgMemory pcgMemory)
        {
            StatusBarDrumKits = string.Empty;
            if (pcgMemory.DrumKitBanks != null)
            {
                var drumKits = pcgMemory.DrumKitBanks.CountFilledPatches;
                var drumKitBanks = pcgMemory.DrumKitBanks.CountFilledBanks;

                switch (drumKits)
                {
                    case 0:
                        break;

                    case 1:
                        switch (drumKitBanks)
                        {
                            case 1:
                                StatusBarDrumKits = Strings.Stb_OneDrumKitInOneBank;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneDrumKitInMultipleBanks, drumKits);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (drumKitBanks)
                        {
                            case 1:
                                StatusBarDrumKits = string.Format(Strings.Stb_MultipleDrumKitsInOneBank, drumKits);
                                break;

                            default: // > 1
                                StatusBarDrumKits = string.Format(
                                    Strings.Stb_MultipleDrumKitsInMultipleBanks, drumKits, drumKitBanks);
                                break;
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        private void RecalculateStatusBarDrumPatterns(IPcgMemory pcgMemory)
        {
            StatusBarDrumPatterns = string.Empty;
            if (pcgMemory.DrumPatternBanks != null)
            {
                var drumPatterns = pcgMemory.DrumPatternBanks.CountFilledPatches;
                var drumPatternBanks = pcgMemory.DrumPatternBanks.CountFilledBanks;

                switch (drumPatterns)
                {
                    case 0:
                        break;

                    case 1:
                        switch (drumPatternBanks)
                        {
                            case 1:
                                StatusBarDrumPatterns = Strings.Stb_OneDrumPatternInOneBank;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneDrumPatternInMultipleBanks, drumPatterns);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (drumPatternBanks)
                        {
                            case 1:
                                StatusBarDrumPatterns = string.Format(Strings.Stb_MultipleDrumPatternsInOneBank, drumPatterns);
                                break;

                            default: // > 1
                                StatusBarDrumPatterns = string.Format(
                                    Strings.Stb_MultipleDrumPatternsInMultipleBanks, drumPatterns, drumPatternBanks);
                                break;
                        }
                        break;
                }
            }
        }


        private void RecalculateStatusBarSetListSlots(IPcgMemory pcgMemory)
        {
            StatusBarSetLists = string.Empty;

            if (pcgMemory.SetLists != null)
            {
                var setListSlots = pcgMemory.SetLists.CountFilledPatches;
                var setLists = pcgMemory.SetLists.CountFilledBanks;

                switch (setListSlots)
                {
                    case 0:
                        break;

                    case 1:
                        switch (setLists)
                        {
                            case 1:
                                StatusBarSetLists = Strings.Stb_OneSetListSlotInOneSetList;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneSetListSlotInMultipleSetLists, setLists);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (setLists)
                        {
                            case 1:
                                StatusBarSetLists = string.Format(
                                    Strings.Stb_MultipleSetListSlotsInOneSetList, setListSlots);
                                break;

                            default: // > 1
                                StatusBarSetLists = string.Format(
                                    Strings.Stb_MultipleSetListSlotsInMultipleSetLists, setListSlots, setLists);
                                break;
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        private void RecalculateStatusBarCombis(IPcgMemory pcgMemory)
        {
            StatusBarCombis = string.Empty;
            if (pcgMemory.CombiBanks != null)
            {
                var combis = pcgMemory.CombiBanks.CountFilledPatches;
                var combiBanks = pcgMemory.CombiBanks.CountFilledBanks;

                switch (combis)
                {
                    case 0:
                        break;

                    case 1:
                        switch (combiBanks)
                        {
                            case 1:
                                StatusBarCombis = Strings.Stb_OneCombiInOneBank;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneCombiInMultipleBanks, combiBanks);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (combiBanks)
                        {
                            case 1:
                                StatusBarCombis = string.Format(Strings.Stb_MultipleCombisInOneBank, combis);
                                break;

                            default: // > 1
                                StatusBarCombis = string.Format(
                                    Strings.Stb_MultipleCombisInMultipleBanks, combis, combiBanks);
                                break;
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        private void RecalculateStatusBarPrograms(IPcgMemory pcgMemory)
        {
            StatusBarPrograms = string.Empty;
            if (pcgMemory.ProgramBanks != null)
            {
                var programs = pcgMemory.ProgramBanks.CountFilledAndNonEmptyPatches;
                var programBanks = pcgMemory.ProgramBanks.CountFilledBanks;

                switch (programs)
                {
                    case 0:
                        break;

                    case 1:
                        switch (programBanks)
                        {
                            case 1:
                                StatusBarPrograms = Strings.Stb_OneProgramInOneBank;
                                break;

                            default:
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_OneProgramInMultipleBanks, programBanks);
                                break;
                        }
                        break;

                    default: // > 1
                        switch (programBanks)
                        {
                            case 1:
                                StatusBarPrograms = string.Format(Strings.Stb_MultipleProgramsInOneBank, programs);
                                break;

                            default: // > 1
                                StatusBarPrograms = string.Format(
                                    Strings.Stb_MultipleProgramsInMultipleBanks, programs, programBanks);
                                break;
                        }
                        break;
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        ICommand _showMasterFilesCommand;



        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable UnusedMember.Global
        public ICommand ShowMasterFilesCommand
        {
            get { return _showMasterFilesCommand ?? (_showMasterFilesCommand = new RelayCommand(param => ShowMasterFiles(),
                param => (SelectedMemory != null) && ((IPcgMemory) SelectedMemory).AreCategoriesEditable));
            }
        }


        /// <summary>
        /// Null if not shown.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public MasterFilesWindow MasterFilesWindow { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public void ShowMasterFiles()
        {
            if (MasterFilesWindow != null)
            {
                // It would be better to put focus to the window, but I couldn't succeed (with Focus(), and optionally
                // BringIntoView or UpudateLayout.
                MasterFilesWindow.CloseWindow();
                MasterFiles.MasterFiles.Instances.MasterFilesWindow = null;
            }

            if (MasterFilesWindow == null)
            {
                var width = Settings.Default.UI_MasterFilesWindowWidth == 0 ? 400 : Settings.Default.UI_MasterFilesWindowWidth;
                var height = Settings.Default.UI_MasterFilesWindowHeight == 0 ? 300 : Settings.Default.UI_MasterFilesWindowHeight;
                var mdiChild = CreateMdiChildWindow(
                    Strings.PcgMasterFile, ChildWindowType.MasterFiles, null, width, height); // No memory
                SelectedMemory = null;
                MasterFilesWindow = (MasterFilesWindow) (mdiChild.Content);
                CurrentChildViewModel = MasterFilesWindow.ViewModel;
                MasterFiles.MasterFiles.Instances.MasterFilesWindow = MasterFilesWindow;
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _showSingleLinedSetListSlotDescriptionsCommand;


        /// <summary>
        /// 
        /// </summary>

        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowSingleLinedSetListSlotDescriptionsCommand
        {
            get
            {
                return _showSingleLinedSetListSlotDescriptionsCommand ??
                    (_showSingleLinedSetListSlotDescriptionsCommand = new RelayCommand(param => ShowSingleLinedSetListSlotDescriptions(),
                    param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowSingleLinedSetListSlotDescriptions()
        {
            // Settings changed already handled by IsShowSingleLinedSetListSlotDescriptions change.
        }


        /// <summary>
        /// 
        /// </summary>
        private bool _isShowSingleLinedSetListSlotDescriptions;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public bool IsShowSingleLinedSetListSlotDescriptions
        {
            get { return _isShowSingleLinedSetListSlotDescriptions; }
            set
            {
                if (value != _isShowSingleLinedSetListSlotDescriptions)
                {
                    _isShowSingleLinedSetListSlotDescriptions = value;
                    Settings.Default.SingleLinedSetListSlotDescriptions = value;
                    SettingsChanged();
                    RaisePropertyChanged("IsShowSingleLinedSetListSlotDescriptions");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showSettingsCommand;

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowSettingsCommand
        {
            get
            {
                return _showSettingsCommand ?? (_showSettingsCommand = new RelayCommand(param => ShowSettings(),
                param => (PcgClipBoard == null) || !PcgClipBoard.PasteDuplicatesExecuted || !PcgClipBoard.CutPasteSelected));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowSettings()
        {
            ShowDialog(WindowType.Settings);

            SettingsChanged();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SettingsChanged()
        {
            ActOnSettingsChanged(""); // ShowSingleLinedSetListSlotDescriptions

            foreach (var child in ChildWindows)
            {
                child.ActOnSettingsChanged("");
                    // NumberOfReferences, ShowSingleLinedSetListSlotDescriptions but arguments are ignored
            }
        }


        /// <summary>
        /// Settings have been changed. 
        /// </summary>
        /// <param name="property"></param>
        public static void ActOnSettingsChanged(string property)
        {
            // IsShowSingleLinedSetListSlotDescriptions = Settings.Default.SingleLinedSetListSlotDescriptions;
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _gotoNextWindowCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand GotoNextWindowCommand
        {
            get
            {
                return _gotoNextWindowCommand ??
                    (_gotoNextWindowCommand = new RelayCommand(param => GotoNextWindow(), param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _gotoPreviousWindowCommand;

        
        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand GotoPreviousWindowCommand
        {
            get
            {
                return _gotoPreviousWindowCommand ?? 
                    (_gotoPreviousWindowCommand = new RelayCommand(param => GotoPreviousWindow(), param => true));
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _showManualCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowManualCommand
        {
            get
            {
                return _showManualCommand ?? (   _showManualCommand = new RelayCommand(
                                                                          param => ShowManual(),
                                                                          param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowManual()
        {
            try
            {
                StartProcess(Settings.Default.DefaultManualFolder);
            }
            catch (Exception exc)
            {
                ShowMessageBox(
                    string.Format(Strings.CouldNotOpenManualWarning,
                        exc.Message, exc.InnerException), Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, 
                        WindowUtils.EMessageBoxImage.Error, WindowUtils.EMessageBoxResult.Ok);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showHomePageCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowHomePageCommand
        {
            get
            {
                return _showHomePageCommand ?? (_showHomePageCommand = new RelayCommand(
                                                                          param => ShowHomePage(),
                                                                          param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowHomePage()
        {
            try
            {
                StartProcess("http://pcgtools.mkspace.nl");
            }
            catch (Exception exc)
            {
                ShowMessageBox(
                    string.Format(Strings.CouldNotOpenHomePageWarning,
                        exc.Message, exc.InnerException), Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok,
                        WindowUtils.EMessageBoxImage.Error, WindowUtils.EMessageBoxResult.Ok);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showAboutCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowAboutCommand
        {
            get { return _showAboutCommand ?? (_showAboutCommand = new RelayCommand(param => ShowAbout(), param => true)); }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowAbout()
        {
            ShowDialog(WindowType.About);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksOasysVoucherCodeSponsorsCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksOasysVoucherCodeSponsorsCommand
        {
            get
            {
                return _showExternalLinksOasysVoucherCodeSponsorsCommand ??
                    (_showExternalLinksOasysVoucherCodeSponsorsCommand = new RelayCommand(
                param => ShowExternalLinksOasysVoucherCodeSponsors(), param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        private void ShowExternalLinksOasysVoucherCodeSponsors()
        {
            ShowDialog(WindowType.ExternalLinksOasysVoucherCodeSponsorsWindow);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksContributorsCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksContributorsCommand
        {
            get
            {
                return _showExternalLinksContributorsCommand ??
                    (_showExternalLinksContributorsCommand = new RelayCommand(param => ShowExternalLinksContributors(),
                param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksContributors()
        {
            ShowDialog(WindowType.ExternalLinksContributors);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksVideoCreatorsCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksVideoCreatorsCommand
        {
            get
            {
                return _showExternalLinksVideoCreatorsCommand ??
                    (_showExternalLinksVideoCreatorsCommand = new RelayCommand(param => ShowExternalLinksVideoCreators(),
                param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksVideoCreators()
        {
            ShowDialog(WindowType.ExternalLinksVideoCreators);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksKorgRelatedCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksKorgRelatedCommand
        {
            get
            {
                return _showExternalLinksKorgRelatedCommand ?? 
                    (_showExternalLinksKorgRelatedCommand = new RelayCommand(param => ShowExternalLinksKorgRelated(), 
                param => true)); }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksKorgRelated()
        {
            ShowDialog(WindowType.ExternalLinksKorgRelated);
        }

        
        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksDonatorsCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksDonatorsCommand
        {
            get
            {
                return _showExternalLinksDonatorsCommand ?? 
                    (_showExternalLinksDonatorsCommand = new RelayCommand(param => ShowExternalLinksDonators(),
                    param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksDonators()
        {
            ShowDialog(WindowType.ExternalLinksDonators);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksTranslatorsCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksTranslatorsCommand
        {
            get
            {
                return _showExternalLinksTranslatorsCommand ?? 
                    (_showExternalLinksTranslatorsCommand = new RelayCommand(param => ShowExternalLinksTranslators(),
                    param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksTranslators()
        {
            ShowDialog(WindowType.ExternalLinksTranslators);
        }


        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksThirdPartiesCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksThirdPartiesCommand
        {
            get
            {
                return _showExternalLinksThirdPartiesCommand ??
                    (_showExternalLinksThirdPartiesCommand = new RelayCommand(param => ShowExternalLinksThirdParties(),
                    param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksThirdParties()
        {
            ShowDialog(WindowType.ExternalLinksThirdParties);
        }



        /// <summary>
        /// 
        /// </summary>
        ICommand _showExternalLinksPersonalCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowExternalLinksPersonalCommand
        {
            get
            {
                return _showExternalLinksPersonalCommand ??
                    (_showExternalLinksPersonalCommand = new RelayCommand(param => ShowExternalLinksPersonal(),
                    param => true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void ShowExternalLinksPersonal()
        {
            ShowDialog(WindowType.ExternalLinksPersonal);
        }
        

        /// <summary>
        /// 
        /// </summary>
        ICommand _showSpecialEventCommand;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public ICommand ShowSpecialEventCommand

        {
            get { return _showSpecialEventCommand ?? 
                (_showSpecialEventCommand = new RelayCommand(param => ShowExternalEvent(), param => true)); }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ShowExternalEvent()
        {
            ShowMessageBox(Strings.Event, Strings.PcgTools, WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Information,
                WindowUtils.EMessageBoxResult.Ok);
        }


        /// <summary>
        /// 
        /// </summary>
        public enum Theme
        {
            Generic,
            Luna,
            Aero
        }


        /// <summary>
        /// 
        /// </summary>
        Theme _selectedTheme;


        /// <summary>
        /// 
        /// </summary>
        public static string PcgToolsApplicationDataDir => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/PCGTools";


        /// <summary>
        /// 
        /// </summary>
        public Theme SelectedTheme
        {
            get { return _selectedTheme; }

            // ReSharper disable once MemberCanBePrivate.Global
            set { if (_selectedTheme != value) { _selectedTheme = value; OnPropertyChanged("SelectedTheme"); } }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectedMemoryChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
            case "IsDirty":
                // update everything :-)
                RecalculateStatusBar();
                break;

            //default: Do nothing
            }
        }


        /// <summary>
        /// Handle application arguments and click once arguments (used by file association PCG and SNG).
        /// </summary>
        public void HandleAppArguments()
        {
            if (App.Arguments.Length == 0)
            {
                return;
            }

            if (App.Arguments[0] == "debug")
            {
                foreach (var file in App.Arguments.Skip(1)) // Skip 'Debug'
                {
                    ReadAndShowFile(file, true);
                }
            }
            else
            {
                ReadAndShowFile(App.Arguments[0], true);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        void OnChildViewModelChanged(object o, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
            case "SelectedPcgMemory":
                UpdateSelectedMemory();
                break;

            case "MemoryChanged":
                RecalculateStatusBar();
                break;

            case "PcgClipBoard":
                RecalculateStatusBar();
                break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Revert()
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitMode"></param>
        /// <returns></returns>
        public override bool Close(bool exitMode)
        {
            return true;
        }


        ///
        /// <summary>
        /// Closes the view.
        /// </summary>
        void Close()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // Cannot be changed to for loop because CloseView changes the collection.
            for (var index = 0; index < ChildWindows.Count; index++)
            {
                var childWindow = ChildWindows[index];
                if (childWindow.ViewModel.Close(true))
                {
                    CloseView();
                }
            }

            CloseView();
        }


        ///
        /// <summary>
        /// Closes all windows with the specified filename.
        /// Currently only used for master files.
        /// Iterate backwards because the collection is decreased during the loop.
        /// Returns: True if all files with specified name close (can be cancelled due to dirty unsaved file).
        /// </summary>
        public bool ClosePcgFile(string fileName)
        {
            // Cannot be changed to for loop because CloseView changes the collection.
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = ChildWindows.Count - 1; index >= 0; index--)
            {
                var childWindow = ChildWindows[index];
                if ((childWindow.ViewModel.SelectedMemory is IPcgMemory) &&
                    (string.Equals(childWindow.ViewModel.SelectedMemory.FileName, fileName, 
                    StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (!childWindow.ViewModel.Close(true))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Handles timer ticks:
        /// - Backs up files which are dirty.
        /// - Remove excessive backup files.
        /// </summary>
        public void OnTimerTick()
        {
            var backupsCreated = false;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Create new backups.
                if (Settings.Default.Settings_AutoBackupFilesEnabled)
                {
                    foreach (var window in ChildWindows.Where(
                        window => (window is PcgWindow) &&
                                  window.Memory.IsBackupDirty &&
                                  (DateTime.Now - window.Memory.LastSaved >
                                   new TimeSpan(0, Settings.Default.Settings_AutoBackupFilesIntervalTime, 0))))
                    {
                        // Backup file.
                        window.Memory.BackupFile();
                        backupsCreated = true;
                    }
                }
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Remove old/excessive backups (only do this when the backup(s) has/have been created.
                if (backupsCreated)
                {
                    // Count disk space.
                    var files = Directory.GetFiles(PcgToolsApplicationDataDir).ToList();
                    var diskSpace = files.Sum(file => new FileInfo(file).Length);

                    // Sort files.
                    files.Sort(new FileUtils.FileAgeComparer());

                    // Remove oldest file(s) until diskSpace is met.
                    while (diskSpace > Settings.Default.Settings_AutoBackupFilesMaxStorage*1024*1024) // Bytes->MB
                    {
                        var oldestFile = files[0];
                        diskSpace -= new FileInfo(oldestFile).Length;
                        File.Delete(oldestFile);
                        files.RemoveAt(0);
                    }
                }
            }
            finally
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }


        /// <summary>
        /// Opened PCG windows.
        /// </summary>
        public OpenedPcgWindows OpenedPcgWindows { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public int GetFilterIndexOfFile(string extension, string filter)
        {
            var model = SelectedMemory.Model.ModelAsString;
            var filterParts = filter.Split('|');
            var filters = new List<string>();

            for (var index = 0; index < filterParts.Length; index += 2)
            {
                // Combine every two filter parts because a filter is made up by two |'s, e.g.: 
                // "MicroKorg XL {0} (*.mkxl_all,*.syx)|*.mkxl_all;*.syx|" +
                filters.Add(filterParts[index] + filterParts[index + 1]);
            }

            var foundFilter = filters.FirstOrDefault(filterToCheck => filterToCheck.Contains(model + " ")) ??
                              filters.FirstOrDefault(
                                  filterToCheck => filterToCheck.ToUpper().Contains(extension.ToUpper() + ",") ||
                                                   filterToCheck.ToUpper().Contains(extension.ToUpper() + ")"));

            for (var n = 0; n < filters.Count; n++)
            {
                if (filters[n] == foundFilter)
                {
                    return n + 1; // First filter is index 1
                }
            }

            return 0;
        }
    }
}

