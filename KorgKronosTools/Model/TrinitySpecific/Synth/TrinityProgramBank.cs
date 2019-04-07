// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.TrinitySpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityProgramBank : ProgramBank
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
        public TrinityProgramBank(IBanks programBanks, BankType.EType type, string id, int pcgId,
            SynthesisType synthesisType, string description)
            : base(programBanks, type, id, pcgId, synthesisType, description)
        {
        }


        public override void CreatePatch(int index)
        {
            Add(new TrinityProgram(this, index));
        }


        /// <summary>
        /// Not used because depending on bank it can also be Prophecy.
        /// </summary>
        public override SynthesisType DefaultModeledSynthesisType => SynthesisType.MossZ1;


        /// <summary>
        /// 
        /// </summary>
        public override SynthesisType DefaultSampledSynthesisType => SynthesisType.Access;
    }
}
