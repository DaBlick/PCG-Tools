// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonTrClassicStudioRackSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonTrClassicStudioRackProgramBanks : TritonProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonTrClassicStudioRackProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }
        

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Hi, "Id A"));                     //  0
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Hi, "Id B"));                     //  1
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Hi, "Id C"));                     //  2
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Hi, "Id D"));                     //  3
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "E", 4, ProgramBank.SynthesisType.Hi, "Id E"));                     //  4
            
            // Bank F is for Triton Studio only (and actually these banks should be called INT-A .. INT-F.
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.Int, "F", 5, ProgramBank.SynthesisType.MossZ1, "Id F"));                 //  5

            // Following banks are for Triton Studio only.
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-A", 17, ProgramBank.SynthesisType.Hi, "Id EXB_A"));           //  6
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-B", 18, ProgramBank.SynthesisType.Hi, "Id EXB_B"));           //  7
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-C", 19, ProgramBank.SynthesisType.Hi, "Id EXB_C"));           //  8
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-D", 20, ProgramBank.SynthesisType.Hi, "Id EXB_D"));           //  9
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-E", 21, ProgramBank.SynthesisType.Hi, "Id EXB_E"));           // 10
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB_F", 22, ProgramBank.SynthesisType.Hi, "Id EXB_F"));           // 11
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-G", 23, ProgramBank.SynthesisType.Hi, "Id EXB_G"));           // 12
            
            Add(new TritonTrClassicStudioRackProgramBank(
                this, BankType.EType.User, "EXB-H", 24, ProgramBank.SynthesisType.Hi, "Id EXB_H"));           // 13

            Add(new TritonTrClassicStudioRackGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Hi, "GM2 Main programs"));      // [6-16]
        }
        
        // Index:              0       1       2       3       4        5
        // Name:               A       B       C       D       E        F      
        // return this[new[] { 0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x08000, 
        // Index:  6         7        8        9       10       11       12       13       14       15       16
        // Name:   G       g(1)     g(2)     g(3)     g(4)     g(5)     g(6)     g(7)     g(8)     g(9)     g(d)       
        //        0xF0000, 0xF0001, 0xF0002, 0xF0003, 0xF0004, 0xF0005, 0xF0006, 0xF0007, 0xF0008, 0xF0009, 0xF000A,
        // Index: 17       18       19       20       21       22       23
        // Name:  EXB-A    EXB-B    EXB_C    EXB-D    EXB_E    EXB-F    EXB_G
        //        0x20000, 0x20001, 0x20002, 0x20003, 0x20004, 0x20005, 0x20006}[]
    }
}
