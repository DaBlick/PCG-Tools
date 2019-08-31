// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonKarmaSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaProgramBanks : TritonProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonKarmaProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }
        

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new TritonKarmaProgramBank(
                this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Hi, "Id A"));                 //  0

            Add(new TritonKarmaProgramBank
                (this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Hi, "Id B"));                 //  1

            Add(new TritonKarmaProgramBank(
                this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Hi, "Id C"));                 //  2

            Add(new TritonKarmaProgramBank(
                this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Hi, "Id D"));                 //  3

            Add(new TritonKarmaProgramBank(
                this, BankType.EType.Int, "E", 4, ProgramBank.SynthesisType.Hi, "Id E"));                 //  4

            Add(new TritonKarmaProgramBank(
                this, BankType.EType.Int, "F", 5, ProgramBank.SynthesisType.MossZ1, "Id F"));             //  5

            Add(new TritonKarmaGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Hi, "GM2 Main programs"));  // [6-16]
        }                                                                                                                                                         
            // Index:              0       1       2       3       4        5
            // Name:               A       B       C       D       E        F      
            // return this[new[] { 0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x08000, 
            // Index:  6         7        8        9       10       11       12       13       14       15       16
            // Name:   G       g(1)     g(2)     g(3)     g(4)     g(5)     g(6)     g(7)     g(8)     g(9)     g(d)       
            //        0xF0000, 0xF0001, 0xF0002, 0xF0003, 0xF0004, 0xF0005, 0xF0006, 0xF0007, 0xF0008, 0xF0009, 0xF000A,
            // Index: 17       18       19       20       21       22       23
            // Name:   H        I        J        K        L        M        N
            //        0x20000, 0x20001, 0x20002, 0x20003, 0x20004, 0x20005, 0x20006}[]
    }
}
