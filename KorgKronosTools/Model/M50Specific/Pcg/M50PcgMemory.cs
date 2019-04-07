// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.M50Specific.Synth;
using PcgTools.Model.MSpecific.Pcg;

namespace PcgTools.Model.M50Specific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class M50PcgMemory : MPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public M50PcgMemory(string fileName)
            : base(fileName, Models.EModelType.M50)
        {
            CombiBanks = new M50CombiBanks(this);
            ProgramBanks = new M50ProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new M50DrumKitBanks(this);
            DrumPatternBanks = new M50DrumPatternBanks(this);
            Global = new M50Global(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionM50);
        }
    }
}
