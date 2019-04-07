// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.KronosOasysSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysSongMemory : SongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        protected KronosOasysSongMemory(string fileName)
            : base(fileName)
        {
        }
    }
}
