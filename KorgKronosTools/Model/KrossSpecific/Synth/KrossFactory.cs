// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KrossSpecific.Pcg;
using PcgTools.Model.KrossSpecific.Song;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossFactory : MFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly PcgMemory.ContentType _contentType;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        public KrossFactory(PcgMemory.ContentType contentType)
        {
            _contentType = contentType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            PcgMemory pcgMemory = new KrossPcgMemory(fileName);
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
            PatchesFileReader reader;
            if (Util.GetChars(content, 0, 2) == "tr")
            {
                reader = new KrossTrFileReader(pcgMemory, content, _contentType);
            }
            else
            {
                reader = new KrossPcgFileReader(pcgMemory, content);
            }
            return reader;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override ISongMemory CreateSongMemory(string fileName)
        {
            SongMemory songMemory = new KrossSongMemory(fileName);
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
            return new KrossSongFileReader(memory, content);
        }
    }
}
