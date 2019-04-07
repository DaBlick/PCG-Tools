// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.Model.Common.Synth.PatchInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// Loaded and available in the file.
        /// </summary>
        bool IsLoaded { get; set; }
    }
}
