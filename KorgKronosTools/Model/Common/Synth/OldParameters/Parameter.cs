// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Parameter : IParameter
    {
        /// <summary>
        /// 
        /// </summary>
        protected IMemory PcgMemory { get; private set; }

        
        /// <summary>
        /// 
        /// </summary>
        protected byte[] PcgData { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        protected int PcgOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IPatch Patch { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="patch"></param>
        protected void Set(IMemory memory, byte[] data, int offset, IPatch patch)
        {
            PcgMemory = memory;
            PcgData = data;
            PcgOffset = offset;
            Patch = patch;
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract dynamic Value { get; set; }
    }
}