// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonSpecific.Song;

namespace PcgTools.Model.TritonKarmaSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaSongMemory : TritonSongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonKarmaSongMemory(string fileName)
            : base(fileName)
        {
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonKarma);
        }
    }
}
