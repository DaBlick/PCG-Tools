// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonKarmaSpecific.Synth;
using PcgTools.Model.TritonSpecific.Pcg;

namespace PcgTools.Model.TritonKarmaSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaPcgMemory : TritonPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonKarmaPcgMemory(string fileName)
            : base(fileName, Models.EModelType.TritonKarma)
        {
            CombiBanks = new TritonKarmaCombiBanks(this);
            ProgramBanks = new TritonKarmaProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new TritonKarmaDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new TritonKarmaGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonKarma);
        }

    }
}
