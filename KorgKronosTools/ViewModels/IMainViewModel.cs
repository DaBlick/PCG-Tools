// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.ObjectModel;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using WPF.MDI;


namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMainViewModel
    {
        /// <summary>
        ///
        /// </summary>
        IViewModel CurrentChildViewModel { set; }


        /// <summary>
        /// 
        /// </summary>
        Func<string, MainViewModel.ChildWindowType, IMemory, int, int, MdiChild> CreateMdiChildWindow { get; set; }


        /// <summary>
        /// 
        /// </summary>
        ObservableCollection<IChildWindow> ChildWindows { get; }


        /// <summary>
        /// 
        /// </summary>
        IMemory SelectedMemory { get; set; }


        /// <summary>
        /// 
        /// </summary>
        Func<string, string, WindowUtils.EMessageBoxButton, WindowUtils.EMessageBoxImage, WindowUtils.EMessageBoxResult,
            WindowUtils.EMessageBoxResult> ShowMessageBox { get; }


        /// <summary>
        /// Checks if there is already a file open with the file name, if so, show dialog. Always open file afterwards.
        /// </summary>
        /// <param name="fileNameToOpen"></param>
        void CheckAndOpenFile(string fileNameToOpen);


        /// <summary>
        ///  Closes all PCG files with specified name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>True if all files with specified name close (can be cancelled due to dirty unsaved file).</returns>
        bool ClosePcgFile(string fileName);


        /// <summary>
        /// Timer.
        /// </summary>
        void OnTimerTick();

        
        /// <summary>
        /// 
        /// </summary>
        ClipBoard.PcgClipBoard PcgClipBoard { get; }
    }
}

