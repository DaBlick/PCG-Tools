// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlProgramBanks : ProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public MicroKorgXlProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new MicroKorgXlProgramBank(this, BankType.EType.Int, "A", -1, ProgramBank.SynthesisType.Mmt, "-"));  //  0
            Add(new MicroKorgXlProgramBank(this, BankType.EType.Int, "B", -1, ProgramBank.SynthesisType.Mmt, "-"));  //  1
        }
    }
}
