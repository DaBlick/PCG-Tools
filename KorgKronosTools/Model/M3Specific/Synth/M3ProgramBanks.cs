// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M3Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M3ProgramBanks : MProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M3ProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-A", 0, ProgramBank.SynthesisType.Eds, "SGX-1, EP-1 and best of all other EXi"));             //  0

            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-B", 1, ProgramBank.SynthesisType.Eds, "HD-1"));                                              //  1

            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-C", 2, ProgramBank.SynthesisType.Eds, "HD-1"));                                              //  2

            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-D", 3, ProgramBank.SynthesisType.Eds, "HD-1"));                                              //  3

            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-E", 4, ProgramBank.SynthesisType.Eds, "HD-1"));                                              //  4

            Add(new M3ProgramBank(
                this, BankType.EType.Int, "I-F", 5, ProgramBank.SynthesisType.Radias, "HD-1"));                                              //  5

            // User banks can contain either EDS or Radias.
            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-A", 17, ProgramBank.SynthesisType.Unknown, 
                "HD1 including Ambient Drums and Sound Effects")); //  6

            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-B", 18, ProgramBank.SynthesisType.Unknown, "AL-1"));                                          //  7
            
            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-C", 19, ProgramBank.SynthesisType.Unknown, "AL-1 and CX-3"));                                 //  8

            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-D", 20, ProgramBank.SynthesisType.Unknown, "STR-1"));                                         //  9

            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-E", 21, ProgramBank.SynthesisType.Unknown, "MS-20EX & PolysixEX"));                           // 10

            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-F", 22, ProgramBank.SynthesisType.Unknown, "MOD-7"));                                         // 11

            Add(new M3ProgramBank(
                this, BankType.EType.User, "U-G", 23, ProgramBank.SynthesisType.Unknown, "Initialized HD-1 Programs"));                     // 12

            Add(new M3GmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Eds, "GM2 Main programs"));                                // [6-16]
        }
    }
}
