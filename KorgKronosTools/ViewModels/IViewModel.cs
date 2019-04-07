// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.ComponentModel;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        IMemory SelectedMemory { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitMode"></param>
        /// <returns></returns>
        bool Close(bool exitMode);


        /// <summary>
        /// Returns true if revert can continue.
        /// </summary>
        /// <returns></returns>
        bool Revert();
    }
}
