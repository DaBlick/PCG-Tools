// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.TritonSpecific.Song;

namespace PcgTools.Model.TritonExtremeSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonExtremeSongMemory : TritonSongMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public TritonExtremeSongMemory(string fileName)
            : base(fileName)
        {
            Model = Models.Find(Models.EOsVersion.EOsVersionTritonExtreme);
        }

    }
}
