// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.ComponentModel;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBanks : ILocatable, INavigable, ICountable, IFillable, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgId"></param>
        /// <returns></returns>
        IBank GetBankWithPcgId(int pcgId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IBank this[int index] { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        int IndexOfBank(IBank bank);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IBank this[string id] { get; }


        /// <summary>
        /// 
        /// </summary>
        IObservableBankCollection<IBank> BankCollection { get; }


        /// <summary>
        /// 
        /// </summary>
        int CountFilledBanks { get; }
    }
}
