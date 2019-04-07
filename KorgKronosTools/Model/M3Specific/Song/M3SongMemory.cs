// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MSpecific.Song;

namespace PcgTools.Model.M3Specific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class M3SongMemory : MSongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public M3SongMemory(string fileName)
            : base(fileName)
        {
            Model = Models.Find(Models.EOsVersion.EOsVersionM3_20); // Songs are always considered M3 2.0 files
        }
    }
}
