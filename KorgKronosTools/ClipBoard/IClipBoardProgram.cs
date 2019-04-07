// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClipBoardProgram : IClipBoardPatch
    {
        /// <summary>
        /// References to used drum kits.
        /// </summary>
        IClipBoardPatches ReferencedDrumKits { get; set; }


        /// <summary>
        /// References to used wave sequences.
        /// </summary>
        IClipBoardPatches ReferencedWaveSequences { get; set; }
    }
}
