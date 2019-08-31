// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.MSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MSongMemory : SongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        protected MSongMemory(string fileName)
            : base(fileName)
        {
        }
    }
}
