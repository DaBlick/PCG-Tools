// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common.Synth.PatchSorting;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public abstract class TimbreSorting
    {
        /// <summary>
        /// 
        /// </summary>
        private TimbreSorting()
        {
            // Not implemented.
        }


        /// <summary>
        /// 
        /// </summary>
        public enum ESortKey
        {
            ESortKeyStatus,
            ESortKeyMute,
            ESortKeyMidiChannel,
            ESortKeyKeyVelocity,
            ESortKeyKeyKeyZone,
            Last
        };


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="sortKeys"></param>
        public static void SortBy(List<Timbre> timbres, IEnumerable<ESortKey> sortKeys)
        {
            var comparers = new CompositeComparer<Timbre>();

            foreach (var key in sortKeys)
            {
                comparers.Comparers.Add(new TimbreComparer(key));
            }

            Debug.Assert(comparers.Comparers.Count == (int) ESortKey.Last);

            timbres.Sort(comparers);
        }
    }
}
