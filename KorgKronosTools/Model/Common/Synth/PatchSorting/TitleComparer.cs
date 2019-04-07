// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.PatchSorting
{
    /// <summary>
    /// Class for comparing titles (i.e. the part before or after the split character.)
    /// </summary>
    internal sealed class TitleComparer : Comparer<IPatch>
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static TitleComparer _instance = new TitleComparer();


        /// <summary>
        /// 
        /// </summary>
        public static TitleComparer Instance => _instance;


        /// <summary>
        /// 
        /// </summary>
        private TitleComparer()
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
            var patch1 = p1 as IArtistable;
            var patch2 = p2 as IArtistable;

            if ((patch1 == null) || (patch2 == null))
            {
                return 0;
            }

            return string.Compare(patch1.Title, patch2.Title, StringComparison.Ordinal);
        }
    }

}