// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.Model.Common.Synth.PatchInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILocatable
    {
        /// <summary>
        /// 
        /// </summary>
        int ByteOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int ByteLength { get; set; }
    }
}
