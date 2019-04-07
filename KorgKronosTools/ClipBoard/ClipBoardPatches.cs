// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public class ClipBoardPatches : IClipBoardPatches
    {
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<IClipBoardPatch> CopiedPatches { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public ClipBoardPatches()
        {
            CopiedPatches = new ObservableCollection<IClipBoardPatch>();
        }


        /// <summary>
        /// 
        /// </summary>
         public int CountUncopied
        {
            get
            {
                return CopiedPatches.Count(patch => patch.PasteDestination == null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
         public IEnumerator<IClipBoardPatch> GetEnumerator()
         {
             return CopiedPatches.GetEnumerator();
         }
    }
}
