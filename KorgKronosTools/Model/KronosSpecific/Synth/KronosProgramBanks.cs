// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosProgramBanks : KronosOasysProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KronosProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public const int ParametersInPbk2Chunk = 66;


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            // Unknown synthesis type because it is dynamic.
            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-A", 0, ProgramBank.SynthesisType.Unknown, "SGX-1, EP-1 and best of all other EXi"));           //  0

            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-B", 1, ProgramBank.SynthesisType.Unknown, "HD-1"));                                            //  1
            
            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-C", 2, ProgramBank.SynthesisType.Unknown, "HD-1"));                                            //  2
            
            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-D", 3, ProgramBank.SynthesisType.Unknown, "HD-1"));                                            //  3
            
            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-E", 4, ProgramBank.SynthesisType.Unknown, "HD-1"));                                            //  4
            
            Add(new KronosProgramBank(
                this, BankType.EType.Int, "I-F", 5, ProgramBank.SynthesisType.Unknown, "HD-1"));                                            //  5


            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-A", 17, ProgramBank.SynthesisType.Unknown,
                "HD1 including Ambient Drums and Sound Effects")); //  6
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-B", 18, ProgramBank.SynthesisType.Unknown, "AL-1"));                                          //  7
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-C", 19, ProgramBank.SynthesisType.Unknown, "AL-1 and CX-3"));                                 //  8
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-D", 20, ProgramBank.SynthesisType.Unknown, "STR-1"));                                         //  9
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-E", 21, ProgramBank.SynthesisType.Unknown, "MS-20EX & PolysixEX"));                           // 10
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-F", 22, ProgramBank.SynthesisType.Unknown, "MOD-7"));                                         // 11
            
            Add(new KronosProgramBank(
                this, BankType.EType.User, "U-G", 23, ProgramBank.SynthesisType.Unknown, "Initialized HD-1 Programs"));                     // 12 ; if changed -> change KronosTimbre


            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-AA", 24, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 13
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-BB", 25, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 14
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-CC", 26, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 15
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-DD", 27, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 16
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-EE", 28, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 17 
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-FF", 29, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 18
            
            Add(new KronosProgramBank(
                this, BankType.EType.UserExtended, "U-GG", 30, ProgramBank.SynthesisType.Unknown, string.Empty));                           // 19

            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(1)", 7,  "GM2 Main programs"));             // [7]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(2)", 8,  "GM2 Main programs"));             // [8]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(3)", 9,  "GM2 Main programs"));             // [9]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(4)", 10, "GM2 Main programs"));            // [10]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(5)", 11, "GM2 Main programs"));            // [11]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(6)", 12, "GM2 Main programs"));            // [12]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(7)", 13, "GM2 Main programs"));            // [13]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(8)", 14, "GM2 Main programs"));            // [14]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(9)", 15, "GM2 Main programs"));            // [15]
            //Add(new KronosGmProgramBank(this, ProgramBank.ListSubType.Gm, "g(d)", 16, "GM2 Main programs"));            // [16]

            CreateVirtualBanks();

            // Add GM bank.
            Add(new KronosGmProgramBank(
                this, BankType.EType.Gm, "GM", 6, ProgramBank.SynthesisType.Hd1, "GM2 Main programs"));          // [84]
        }


#pragma warning disable 1570
        /// <summary>
        /// Create 32 virtual banks (8 groups of 8 banks).
        /// To create virtual banks, name them like V<number><A..H>.
        /// 
        /// The PCG file that is created, adds the banks after the first two (so the bank IDS are 0, 1, 0x30, 0x30 + 1, ... 0x30 + 63, 2, 3, ...
        /// Do not forget to change the IDs (0x30 ... 0x30 + 63, and change the chunk size of PRG1. 
        /// 
        /// Some interesting offsets/values:
        /// 0x8EA2A8 PRG1
        /// 0x8EA2AC 0xC1C1E0 Chunk size (change this: 0x9B018 per program bank (same for HD1/EXi).
        /// ...
        /// New banks start at A202F8 (?)
        /// </summary>
#pragma warning restore 1570
        protected override void CreateVirtualBanks()
        {
            var bankNames = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            for (var bankGroupIndex = 0; bankGroupIndex < 8; bankGroupIndex++)
            {
                for (var bankIndex = 0; bankIndex < bankNames.Count; bankIndex++)
                {
                    Add(
                        new KronosProgramBank(
                            this, BankType.EType.Virtual,
                            $"V{bankGroupIndex}-{bankNames[bankIndex]}",        // [20..83]
                            FirstVirtualBankId + bankGroupIndex * bankNames.Count + bankIndex, 
                            ProgramBank.SynthesisType.Unknown, string.Empty));
                }
            }
        }


        /// <summary>
        /// Swaps for two patches (programs) their PBK2 contents.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
        public void SwapPbk2Content(IPatch patch, IPatch otherPatch)
        {
            for (var parameter = 0; parameter < ParametersInPbk2Chunk; parameter++)
            {
                // Swap bytes.
                var patchParameterOffset =
                    ((KronosProgramBank) (patch.Parent)).GetParameterOffsetInPbk2(patch.Index, parameter);
                
                var otherPatchParameterOffset =
                    ((KronosProgramBank)(otherPatch.Parent)).GetParameterOffsetInPbk2(otherPatch.Index, parameter);

                var temp = Root.Content[patchParameterOffset];
                Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                Root.Content[patchParameterOffset] = Root.Content[otherPatchParameterOffset];
                Debug.Assert(otherPatchParameterOffset >= 4); // Don't overwrite KORG header
                Root.Content[otherPatchParameterOffset] = temp;
            }
        }


        /// <summary>
        /// Copy from source to destination patch its PBK2 content.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
// ReSharper disable once UnusedMember.Global
        public void CopyPbk2Content(IPatch patch, IPatch otherPatch)
        {
            for (var parameter = 0; parameter < ParametersInPbk2Chunk; parameter++)
            {
                // Swap bytes.
                var patchParameterOffset =
                    ((KronosProgramBank)(patch.Parent)).GetParameterOffsetInPbk2(patch.Index, parameter);
                var otherPatchParameterOffset =
                    ((KronosProgramBank)(otherPatch.Parent)).GetParameterOffsetInPbk2(otherPatch.Index, parameter);

                Debug.Assert(otherPatchParameterOffset >= 4); // Don't overwrite KORG header
                Root.Content[otherPatchParameterOffset] = Root.Content[patchParameterOffset];
            }
        }
    }
}
