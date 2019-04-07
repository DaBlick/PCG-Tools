// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M1ProgramBanks : MntxProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M1ProgramBanks(IPcgMemory pcgMemory)
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
            Add(
                new M1ProgramBank(
                    this, BankType.EType.Int, $"{"I"}", 0,
                    ProgramBank.SynthesisType.Ai, "-"));

            // Add Card banks.
            Add(
                new M1ProgramBank(
                    this, BankType.EType.Int, $"{"C"}", 1,
                    ProgramBank.SynthesisType.Ai, "-"));
        }
    }
}
