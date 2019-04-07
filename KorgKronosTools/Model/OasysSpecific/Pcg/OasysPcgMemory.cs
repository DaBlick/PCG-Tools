// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.KronosOasysSpecific.Pcg;
using PcgTools.Model.OasysSpecific.Synth;

namespace PcgTools.Model.OasysSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysPcgMemory : KronosOasysPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public OasysPcgMemory(string fileName)
            : base(fileName, Models.EModelType.Oasys)
        {
            CombiBanks = new OasysCombiBanks(this);
            ProgramBanks = new OasysProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = new OasysWaveSequenceBanks(this);
            DrumKitBanks = new OasysDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new OasysGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionOasys);
        }
    }
}
