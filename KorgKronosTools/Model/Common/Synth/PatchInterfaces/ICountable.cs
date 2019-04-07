// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.Model.Common.Synth.PatchInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICountable
    {
        /// <summary>
        /// 
        /// </summary>
        int CountPatches { get; }


        /// <summary>
        /// 
        /// </summary>
        int CountFilledPatches { get; }


        /// <summary>
        /// 
        /// </summary>
        int CountFilledAndNonEmptyPatches { get; }


        /// <summary>
        /// 
        /// </summary>
        int CountWritablePatches { get; }
    }
}
