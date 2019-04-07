// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.ObjectModel;

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClipBoardPatches
    {
        /// <summary>
        /// 
        /// </summary>
        int CountUncopied { get; }


        /// <summary>
        /// 
        /// </summary>
        ObservableCollection<IClipBoardPatch> CopiedPatches { get; }
    }
}
