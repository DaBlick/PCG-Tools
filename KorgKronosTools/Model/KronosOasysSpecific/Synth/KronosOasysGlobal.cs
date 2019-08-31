// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosOasysGlobal : Global
    {
        /// <summary>
        /// 
        /// </summary>
        protected override int CategoryNameLength => 24;


        /// <summary>
        /// 
        /// </summary>
        protected override int PcgOffsetCategories => 12912;

// 12918 for kronos ? 9558; } } // In full PCG: global at 3613a0, categories at 363902


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfCategories => 18;


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfSubCategories => 8;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected KronosOasysGlobal(IPcgMemory pcgMemory): base(pcgMemory)
        {
        }
    }
}
