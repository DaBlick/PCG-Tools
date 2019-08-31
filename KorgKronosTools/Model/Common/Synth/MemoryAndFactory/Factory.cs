// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Factory : IFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract IPcgMemory CreatePcgMemory(string fileName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract ISongMemory CreateSongMemory(string fileName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract IPatchesFileReader CreateFileReader(IPcgMemory pcgMemory, byte[] content);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract ISongFileReader CreateSongFileReader(ISongMemory songMemory, byte[] content);
    }
}
