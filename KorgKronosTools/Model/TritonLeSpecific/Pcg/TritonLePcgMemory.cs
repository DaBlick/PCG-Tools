// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonLeSpecific.Synth;
using PcgTools.Model.TritonSpecific.Pcg;

namespace PcgTools.Model.TritonLeSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonLePcgMemory : TritonPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonLePcgMemory(string fileName)
            : base(fileName, Models.EModelType.TritonLe)
        {
            CombiBanks = new TritonLeCombiBanks(this);
            ProgramBanks = new TritonLeProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new TritonLeDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new TritonLeGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonLe);
        }

    }
}
