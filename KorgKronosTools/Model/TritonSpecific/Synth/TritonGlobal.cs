// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;

using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.TritonSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonGlobal : Global
    {
        /// <summary>
        /// 
        /// </summary>
        protected override int PcgOffsetCategories => 324;


        /// <summary>
        /// 
        /// </summary>
        protected override int CategoryNameLength => 16;


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfCategories => 16;


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfSubCategories { get { throw new NotSupportedException("No sub categories supported"); } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected TritonGlobal(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }
    }
}
