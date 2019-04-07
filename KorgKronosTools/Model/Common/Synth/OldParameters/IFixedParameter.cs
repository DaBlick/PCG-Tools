// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;


namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFixedParameter : IParameter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="patch"></param>
        void Set(IMemory memory, byte[] data, FixedParameter.EType type, IPatch patch);
    }
}