// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesProgramBanks : MntxProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public ZeroSeriesProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            // Add internal banks.
            var pcgId = 0;
            foreach (var id in new[] {"A", "B", "C", "D"})
            {
                Add(
                    new ZeroSeriesProgramBank(
                        this, BankType.EType.Int, id, pcgId, ProgramBank.SynthesisType.Ai2, string.Empty));
                pcgId++;
            }

            // Add virtual banks for raw disk image file.
            for (var id = 0; id <= 4; id++)
            {
                Add(new ZeroSeriesProgramBank(
                        this, BankType.EType.Virtual, $"V{id + 1}", pcgId, 
                        ProgramBank.SynthesisType.Ai2, string.Empty));
                pcgId++;
            }
        }
    }
}
