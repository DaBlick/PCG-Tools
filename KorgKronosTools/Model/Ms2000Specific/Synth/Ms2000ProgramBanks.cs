// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common.Synth;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Ms2000Specific.Synth
{
    public class Ms2000ProgramBanks : ProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public Ms2000ProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// The first (default internal) eight program banks are called A..H.
        /// The next (virtual) banks will be called V1A, V1B, ... V1H, V2A, ...
        /// </summary>
        protected override void CreateBanks()
        {
            // Add internal banks.
            foreach (var bankName in new List<string> {"A", "B", "C", "D", "E", "F", "G", "H"})
            {
                Add(
                    new Ms2000ProgramBank(
                        this, BankType.EType.Int, $"{bankName}", 0, 
                        ProgramBank.SynthesisType.AnalogModeling, "-"));
            }

            // Add virtual banks.
            for (var set = 0; set < 16; set++) // 15 virtual banks
            {
                foreach (var bankName in new List<string> {"A", "B", "C", "D", "E", "F", "G", "H"})
                {
                    Add(
                        new Ms2000ProgramBank(
                            this, BankType.EType.Virtual, $"V{set}{bankName}", 0, 
                            ProgramBank.SynthesisType.AnalogModeling, "-"));
                }
            }
        }
    }
}
