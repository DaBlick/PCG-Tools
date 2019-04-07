// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonTrClassicStudioRackSpecific.Synth;
using PcgTools.Model.TritonSpecific.Pcg;

namespace PcgTools.Model.TritonTrClassicStudioRackSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonTrClassicStudioRackPcgMemory : TritonPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonTrClassicStudioRackPcgMemory(string fileName)
            : base(fileName, Models.EModelType.TritonTrClassicStudioRack)
        {
            CombiBanks = new TritonTrClassicStudioRackCombiBanks(this);
            ProgramBanks = new TritonTrClassicStudioRackProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new TritonTrClassicStudioRackDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new TritonTrClassicStudioRackGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonTrClassicStudioRack);
        }

    }
}
