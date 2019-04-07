// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClipBoardSetListSlot : IClipBoardPatch
    {
        /// <summary>
        ///  Reference to program or combi (or null if song).
        /// </summary>
        IClipBoardPatch Reference { get; set; }
    }
}
