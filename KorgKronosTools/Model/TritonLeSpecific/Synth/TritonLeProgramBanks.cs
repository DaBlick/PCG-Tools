// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonLeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonLeProgramBanks : TritonProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonLeProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }
        

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new TritonLeProgramBank(
                this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Hi, "Id A"));                 //  0

            Add(new TritonLeProgramBank(
                this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Hi, "Id B"));                 //  1

            Add(new TritonLeProgramBank(
                this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Hi, "Id C"));                 //  2

            Add(new TritonLeProgramBank(
                this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Hi, "Id D"));                 //  3

            Add(new TritonLeGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Hi, "GM2 Main programs"));   // [6,16?]
        }
        
            // Index:              0       1       2       3     
            // Name:               A       B       C       D     
            // return this[new[] { 0x0000, 0x0001, 0x0002, 0x0003
            // Index:  6        16???
            // Name:   G       g(d)       
            //        0xF0000, 0xF000A,
            // Index: 17         23
    }
}
