// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.ComponentModel;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMemory : INavigable, IDirtiable, ISupportedFeatures, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        string FileName { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        byte[] Content { get; }


        /// <summary>
        /// 
        /// </summary>
        IModel Model { get; }


        /// <summary>
        /// 
        /// </summary>
        Memory.FileType MemoryFileType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveAs"></param>
        /// <param name="saveToFile"></param>
        void SaveFile(bool saveAs, bool saveToFile);


        /// <summary>
        /// Make a backup of the file.
        /// </summary>
        void BackupFile();


        /// <summary>
        /// Returns the program ID; only supported by Kronos.
        /// </summary>
        /// <param name="rawBankIndex"></param>
        /// <param name="rawProgramIndex"></param>
        /// <returns></returns>
        string ProgramIdByIndex(int rawBankIndex, int rawProgramIndex);
    }
}
