// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.Common.Synth.PatchInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface INavigable
    {
        /// <summary>
        /// 
        /// </summary>
        IMemory Root { get; }

        
        /// <summary>
        /// 
        /// </summary>
        INavigable Parent { get; }
    }
}
