// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchPrograms
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProgramBank : IBank
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsModeled { get; }


        /// <summary>
        /// 
        /// </summary>
        ProgramBank.SynthesisType BankSynthesisType { get; set;  }


        /// <summary>
        /// 
        /// </summary>
        ProgramBank.SynthesisType DefaultSampledSynthesisType { get; }
    }
}
