// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysProgramBank : ProgramBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        /// <param name="synthesisType"></param>
        /// <param name="description"></param>
        protected KronosOasysProgramBank(IProgramBanks programBanks, BankType.EType type, string id, int pcgId,
             SynthesisType synthesisType, string description)
            : base(programBanks, type, id, pcgId, synthesisType, description)
        {
        }
    }
}
