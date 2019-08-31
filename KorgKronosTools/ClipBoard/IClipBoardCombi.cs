// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClipBoardCombi : IClipBoardPatch
    {
        /// <summary>
        /// References to timbres/programs.
        /// </summary>
        IClipBoardPatches References { get; }
    }
}

