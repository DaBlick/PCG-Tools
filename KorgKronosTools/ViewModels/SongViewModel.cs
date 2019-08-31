// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.OpenedFiles;
using PcgTools.Songs;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.ViewModels 
{
    /// <summary>
    /// 
    /// </summary>
    public class SongViewModel : ObservableObject, ISongViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SongViewModel(OpenedPcgWindows openedPcgWindows)
        {
            OpenedPcgWindows = openedPcgWindows;
            OpenedPcgWindows.Items.CollectionChanged += OpenedPcgWindowsChanged;
        }


        /// <summary>
        /// Not used; does not work either.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenedPcgWindowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If no file selected and model is correct, use it.
            if (string.IsNullOrEmpty(SelectedPcgFileName))
            {
                if (e.NewItems != null)
                {
                    foreach (OpenedPcgWindow item in e.NewItems.Cast<OpenedPcgWindow>().Where(item =>
                        (SelectedMemory.Model.ModelType == Models.EModelType.Kronos) &&
                        ModelCompatibility.AreModelsCompatible(SelectedMemory.Model, item.PcgMemory.Model)))
                    {
                        SelectedPcgFileName = item.PcgMemory.FileName;
                        break;
                    }
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    // If file is selected which is closed, deselect it.
                    foreach (
                        var item in
                            e.OldItems.Cast<OpenedPcgWindow>()
                                .Where(item => SelectedPcgFileName == item.PcgMemory.FileName))
                    {
                        SelectedPcgFileName = null;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private OpenedPcgWindows _openedPcgWindows;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public OpenedPcgWindows OpenedPcgWindows
        {
            get { return _openedPcgWindows; }

            set
            {
                if (_openedPcgWindows != value)
                {
                    _openedPcgWindows = value;
                    RaisePropertyChanged("OpenedPcgWindows");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private string _windowTitle;


        /// <summary>
        /// 
        /// </summary>
        public string WindowTitle
        {
            get { return _windowTitle; }
            private set
            {
                if (value != WindowTitle)
                {
                    _windowTitle = value;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public void UpdateWindowTitle()
        {
            WindowTitle =
                $"{SelectedMemory.FileName}{(SelectedMemory.IsDirty ? " *" : string.Empty)} ({SelectedMemory.Model.ModelAndVersionAsString})";
        }


        /// <summary>
        /// 
        /// </summary>
        private RelayCommand _saveCommand;


        /// <summary>
        /// 
        /// </summary>
        private ISong _song;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public ISong Song
        {
            get { return _song; }

            // ReSharper disable once UnusedMember.Global
            set
            {
                if (_song != value)
                {
                    _song = value;
                    OnPropertyChanged("Song");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(
                    param => ExecuteCommandSaveSong(), param => CanExecuteSaveCommand()));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteSaveCommand()
        {
            return _song.Memory.IsDirty;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ExecuteCommandSaveSong()
        {
            Song.Memory.SaveFile(false, true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Revert()
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public IMemory SelectedMemory { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitMode"></param>
        /// <returns></returns>
        public bool Close(bool exitMode)
        {
            SelectedMemory = null;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        public ISongMemory SelectedSongMemory => (ISongMemory) SelectedMemory;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void OnPropertyChanged(object o, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Dirty":
                    UpdateWindowTitle();
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private string _selectedPcgFileName;


        /// <summary>
        /// 
        /// </summary>
        public string SelectedPcgFileName
        {
            get
            {
                return _selectedPcgFileName;
            }

            set
            {
                if (_selectedPcgFileName != value)
                {
                    _selectedPcgFileName = value;
                    OnPropertyChanged("SelectedPcgFileName");
                }
            }
        }
    }
}
