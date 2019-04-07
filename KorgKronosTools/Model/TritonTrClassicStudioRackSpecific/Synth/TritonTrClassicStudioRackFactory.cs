// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.TritonTrClassicStudioRackSpecific.Pcg;
using PcgTools.Model.TritonTrClassicStudioRackSpecific.Song;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonTrClassicStudioRackSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonTrClassicStudioRackFactory : TritonFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            PcgMemory pcgMemory = new TritonTrClassicStudioRackPcgMemory(fileName);
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
            return new TritonTrClassicStudioRackPcgFileReader(pcgMemory, content);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override ISongMemory CreateSongMemory(string fileName)
        {
            SongMemory songMemory = new TritonTrClassicStudioRackSongMemory(fileName);
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
            return new TritonTrClassicStudioRackSongFileReader(memory, content);
        }
    }
}
