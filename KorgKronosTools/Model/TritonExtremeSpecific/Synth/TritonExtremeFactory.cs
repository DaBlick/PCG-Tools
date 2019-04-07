// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.TritonExtremeSpecific.Pcg;
using PcgTools.Model.TritonExtremeSpecific.Song;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonExtremeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonExtremeFactory : TritonFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            IPcgMemory pcgMemory = new TritonExtremePcgMemory(fileName);
            pcgMemory.Fill();
            return pcgMemory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override IPatchesFileReader CreateFileReader(IPcgMemory pcgMemory, byte[] content)
        {
            return new TritonExtremePcgFileReader(pcgMemory, content);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override ISongMemory CreateSongMemory(string fileName)
        {
            SongMemory songMemory = new TritonExtremeSongMemory(fileName);
            return songMemory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public override ISongFileReader CreateSongFileReader(ISongMemory memory, byte[] content)
        {
            return new TritonExtremeSongFileReader(memory, content);
        }
    }
}
