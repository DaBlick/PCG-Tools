// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchSorting
{

    /// <summary>
    /// Class for comparing if a patch is empty/init (always put at end).
    /// </summary>
    sealed class EmptyOrInitComparer : Comparer<IPatch>
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly EmptyOrInitComparer _instance = new EmptyOrInitComparer();


        /// <summary>
        /// 
        /// </summary>
        public static EmptyOrInitComparer Instance => _instance;


        /// <summary>
        /// 
        /// </summary>
        private EmptyOrInitComparer()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public override int Compare(IPatch p1, IPatch p2)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);

            if (p1.IsEmptyOrInit)
            {
                return p2.IsEmptyOrInit ? 0 : 1;
            }

            return p2.IsEmptyOrInit ? -1 : 0;
        }
    }

    
}
