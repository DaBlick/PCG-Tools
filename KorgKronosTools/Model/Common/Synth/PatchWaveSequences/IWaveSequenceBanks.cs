// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchWaveSequences
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWaveSequenceBanks : IBanks
    {
        /// <summary>
        /// 
        /// </summary>
        int Wsq2PcgOffset { get; set; }
    }
}
