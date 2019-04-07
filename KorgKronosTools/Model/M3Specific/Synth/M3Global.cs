// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M3Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M3Global : MGlobal
    {
        /// <summary>
        /// 
        /// </summary>
        protected override int PcgOffsetCategories => 15730;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M3Global(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }
    }
}
