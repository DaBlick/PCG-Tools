// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public class Regions : IRegions
    {
                /// <summary>
        /// 
        /// </summary>
        public List<IRegion> RegionsCollection { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public Regions()
        {
            RegionsCollection = new List<IRegion>();
        }
    }
}
