// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchSorting
{
    /// <summary>
    /// Class for comparing names in an ordinal manner.
    /// </summary>
    sealed class NameComparer : Comparer<IPatch>
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static NameComparer _instance = new NameComparer();


        /// <summary>
        /// 
        /// </summary>
        public static NameComparer Instance => _instance;


        /// <summary>
        /// 
        /// </summary>
        private NameComparer()
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

            return string.Compare(p1.Name, p2.Name, StringComparison.Ordinal);
        }
    }
}
