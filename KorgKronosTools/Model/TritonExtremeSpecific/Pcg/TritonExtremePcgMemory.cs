// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonExtremeSpecific.Synth;
using PcgTools.Model.TritonSpecific.Pcg;

namespace PcgTools.Model.TritonExtremeSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonExtremePcgMemory : TritonPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonExtremePcgMemory(string fileName)
            : base(fileName, Models.EModelType.TritonExtreme)
        {
            CombiBanks = new TritonExtremeCombiBanks(this);
            ProgramBanks = new TritonExtremeProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new TritonExtremeDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new TritonExtremeGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonExtreme);
        }

    }
}
