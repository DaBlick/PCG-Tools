// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.ComponentModel;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBank : INamable, ISelectable, ILoadable, IWritable, ILocatable, ICountable, IUpdatable, IIndexable,
        INavigable, INotifyPropertyChanged, IObservableObject
    {
        /// <summary>
        /// 
        /// </summary>
        ObservablePatchCollection Patches { get; }


        /// <summary>
        /// 
        /// </summary>
        BankType.EType Type { get; }


        /// <summary>
        /// 
        /// </summary>
        string Id { get; }


        /// <summary>
        /// 
        /// </summary>
        int PcgId { get; }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IPatch this[int index] { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsFilled { get; }


        /// <summary>
        /// 
        /// </summary>
        void SetParameters();


        /// <summary>
        /// Number of patches the Patches collection should have after creating patches.
        /// </summary>
        int NrOfPatches { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void CreatePatch(int index);


        /// <summary>
        /// 
        /// </summary>
        bool FilterForUi { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IsFromMasterFile { get; }


        /// <summary>
        /// E.g. GM banks start with 1.
        /// </summary>
        int IndexOffset { get; }


        /// <summary>
        /// Clears the bank (not the patches inside).
        /// </summary>
        void Clear();
    }
}
