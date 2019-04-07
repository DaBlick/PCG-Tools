// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossProgramBanks : MProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KrossProgramBanks(PcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new KrossProgramBank(this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Edsx, "Id A"));     //  0
            Add(new KrossProgramBank(this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Edsx, "Id B"));     //  1
            Add(new KrossProgramBank(this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Edsx, "Id C"));     //  2
            Add(new KrossProgramBank(this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Edsx, "Id D"));     //  3
            Add(new KrossProgramBank(this, BankType.EType.User, "U", 4, ProgramBank.SynthesisType.Edsx, "Id U"));     //  4

            Add(new KrossGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Edsx, "GM2 Main programs"));   //  6-15
        }                                                                                                                         
    }
}                                                                                                                                                            