// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class Chunks : IChunks
    {
        /// <summary>
        /// 
        /// </summary>
        public List<IChunk> Collection { get; private set; }



        /// <summary>
        /// 
        /// </summary>
        public Chunks()
        {
            Collection = new List<IChunk>();
        }
    }
}
