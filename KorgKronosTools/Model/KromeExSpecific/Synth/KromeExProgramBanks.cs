// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KromeExSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KromeExProgramBanks : MProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KromeExProgramBanks(PcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new KromeExProgramBank(this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Edsx, "Id A"));    //  0
            Add(new KromeExProgramBank(this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Edsx, "Id B"));    //  1
            Add(new KromeExProgramBank(this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Edsx, "Id C"));    //  2
            Add(new KromeExProgramBank(this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Edsx, "Id D"));    //  3
            Add(new KromeExProgramBank(this, BankType.EType.Int, "E", 4, ProgramBank.SynthesisType.Edsx, "Id E"));    //  4
            Add(new KromeExProgramBank(this, BankType.EType.Int, "F", 5, ProgramBank.SynthesisType.Edsx, "Id F"));    //  5

            Add(new KromeExProgramBank(this, BankType.EType.User, "U-A", 6 , ProgramBank.SynthesisType.Edsx, "Id U-A"));    //  6
            Add(new KromeExProgramBank(this, BankType.EType.User, "U-B", 7 , ProgramBank.SynthesisType.Edsx, "Id U-B"));    //  7
            Add(new KromeExProgramBank(this, BankType.EType.User, "U-C", 8 , ProgramBank.SynthesisType.Edsx, "Id U-C"));    //  8
            Add(new KromeExProgramBank(this, BankType.EType.User, "U-D", 9 , ProgramBank.SynthesisType.Edsx, "Id U-D"));    //  9
            Add(new KromeExProgramBank(this, BankType.EType.User, "U-E", 10, ProgramBank.SynthesisType.Edsx, "Id U-E"));    //  10
            Add(new KromeExProgramBank(this, BankType.EType.User, "U-F", 11, ProgramBank.SynthesisType.Edsx, "Id U-F"));    //  11

            Add(new KromeExGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Edsx, "GM2 Main programs"));   //  6-15
        }                                                                                                                         
    }
}                                                                                                                                                            