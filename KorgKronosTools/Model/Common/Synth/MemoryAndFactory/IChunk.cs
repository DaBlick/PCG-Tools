// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// 
        /// </summary>
        string Name { get; }


        /// <summary>
        /// 
        /// </summary>
        int Offset { get; }


        /// <summary>
        /// 
        /// </summary>
        int Size { get; }
    }
}
