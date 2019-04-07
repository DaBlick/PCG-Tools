// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlPlusProgramBanks : ProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public MicroKorgXlPlusProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new MicroKorgXlPlusProgramBank(this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Mmt, "-"));                 //  0
        }
    }
}
