// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Z1ProgramBanks : MntxProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public Z1ProgramBanks(IPcgMemory pcgMemory)
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
            var bankId = 0;
            foreach (var id in new[] {"A", "B"})
            {
                Add(
                    new Z1ProgramBank(
                        this, BankType.EType.Int, $"{id}", bankId, 
                        ProgramBank.SynthesisType.MossZ1, "-"));
                bankId++;
            }

            // Add Card banks.
            foreach (var id in new[] {"CARD-A", "CARD-B"})
            {
                Add(
                    new Z1ProgramBank(
                        this, BankType.EType.Int, $"{id}", bankId, 
                        ProgramBank.SynthesisType.MossZ1, "-"));
                bankId++;
            }
        }
    }
}
