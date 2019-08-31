// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        IMemory _selectedMemory;


        /// <summary>
        /// 
        /// </summary>
        public virtual IMemory SelectedMemory
        {
            get { return _selectedMemory; }
            set { if (_selectedMemory != value) { _selectedMemory = value; OnPropertyChanged("SelectedMemory"); } }
        }


        /// <summary>
        /// 
        /// </summary>
        public Action CloseWindow { protected get; set; }


        /// <summary>
        /// Returns true if close can continue.
        /// </summary>
        /// <returns></returns>
        public abstract bool Close(bool exitMode);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Revert()
        {
            return true;
        }
    }
}
