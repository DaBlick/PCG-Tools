// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MicroKorgXlSpecific.Synth;


namespace PcgTools.Model.MicroKorgXlSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlMkxlPProgMemory : MkxlAllMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public MicroKorgXlMkxlPProgMemory(string fileName)
            : base(fileName, Models.EModelType.MicroKorgXlPlus)
        {
            CombiBanks = null;
            ProgramBanks = new MicroKorgXlPlusProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = null;
            DrumPatternBanks = null;
            Global = new MicroKorgXlGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionMicroKorgXlPlus);
        }
    }
}
