// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosCombiBanks : KronosOasysCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KronosCombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public const int ParametersInCbk2Chunk = 2; // Bank, Program


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            foreach (var id in new[] {"I-A", "I-B", "I-C", "I-D", "I-E", "I-F", "I-G"})
            {
                Add(new KronosCombiBank(this, BankType.EType.Int, id, -1));
            }

            foreach (var id in new[] {"U-A", "U-B", "U-C", "U-D", "U-E", "U-F", "U-G"})
            {
                Add(new KronosCombiBank(this, BankType.EType.User, id, -1));
            }

            CreateVirtualBanks();
        }


        /// <summary>
#pragma warning disable 1570
        /// Create 32 virtual banks (8 groups of 8 banks).
        /// To create virtual banks, name them like V<number><A..H>
        /// 
        /// The PCG file that is created, adds the banks before all internal banks 
        /// (so the bank IDS are 0x30, 0x30 + 1, ... 0x30 + 63, 0, 1, 2, ..
        /// Do not forget to change the IDs (0x30 ... 0x30 + 63, and change the chunk size of CMB1. 
        /// 
        /// Some interesting offsets/values:
        /// 0x8EA2A8 CMB1
        /// 0x8EA2AC  Chunk size: D58F50 (change this).
        /// ...
        /// New banks start at CBK1, bank ID at CBK1 + 16 (change this).
        /// 
        /// The size of one bank is 
        /// </summary>
#pragma warning restore 1570
        protected override void CreateVirtualBanks()
        {
            var bankNames = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            for (var bankGroupIndex = 0; bankGroupIndex < 8; bankGroupIndex++)
            {
                foreach (var bankName in bankNames)
                {
                    Add(
                        new KronosCombiBank(
                            this, BankType.EType.Virtual,
                            $"V{bankGroupIndex}-{bankName}", -1));
                }
            }
        }


        /// <summary>
        /// Swaps for two patches (combis) their CBK2 contents.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
        public void SwapCbk2Content(IPatch patch, IPatch otherPatch)
        {
            for (var parameter = 0; parameter < ParametersInCbk2Chunk; parameter++)
            {
                for (var timbre = 0; timbre < KronosTimbres.TimbresPerCombiConstant; timbre++)
                {
                    // Swap bytes.
                    var patchParameterOffset = ((KronosCombiBank)(patch.Parent)).GetParameterOffsetInCbk2(patch.Index, timbre, parameter);
                    var otherPatchParameterOffset = ((KronosCombiBank)(otherPatch.Parent)).GetParameterOffsetInCbk2(otherPatch.Index, timbre, parameter);

                    var temp = Root.Content[patchParameterOffset];
                    Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                    Root.Content[patchParameterOffset] = Root.Content[otherPatchParameterOffset];
                    Debug.Assert(otherPatchParameterOffset >= 4); // Don't overwrite KORG header
                    Root.Content[otherPatchParameterOffset] = temp;
                }
            }
        }
    }
}
