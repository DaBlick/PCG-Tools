// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchDrumPatterns
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDrumPatternBanks : IBanks
    {
        /// <summary>
        /// 
        /// </summary>
        int Drk2PcgOffset { get; set; }


        /// <summary>
        /// Returns the indexToSearch, starting with indexToSearch 0 as first bank, first indexToSearch, 
        /// and continuing over banks.
        /// </summary>
        /// <param name="indexToSearch"></param>
        /// <returns></returns>
        IDrumPattern GetByIndex(int indexToSearch);


        /// <summary>
        /// Returns the drum Pattern index.
        /// </summary>
        /// <param name="drumPattern"></param>
        /// <returns></returns>
        int FindIndexOf(IDrumPattern drumPattern);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgId"></param>
        /// <returns></returns>
        IDrumPatternBank GetDrumPatternBankWithPcgId(int pcgId);
    }
}
