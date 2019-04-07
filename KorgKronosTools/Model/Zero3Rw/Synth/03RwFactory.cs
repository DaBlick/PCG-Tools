// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.Zero3Rw.Pcg;
using PcgTools.Model.Zero3Rw.Song;
using PcgTools.Model.ZeroSeries.Synth;

namespace PcgTools.Model.Zero3Rw.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Zero3RwFactory : ZeroSeriesFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public Zero3RwFactory(Memory.FileType fileType, PcgMemory.ContentType contentType,
            int sysExStartOffset, int sysExEndOffset)
            : base(fileType, contentType, sysExStartOffset, sysExEndOffset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override IPcgMemory CreatePcgMemory(string fileName)
        {
            IPcgMemory pcgMemory;

            switch (FileType)
            {
                case Memory.FileType.Syx: // Fall through
                case Memory.FileType.Mid: // Fall through
                case Memory.FileType.Raw:
                    pcgMemory = new Zero3RwSysExMemory(fileName, ContentType, SysExStartOffset, SysExEndOffset, FileType == 
                        Memory.FileType.Raw);
                    break;
                    
                default:
                    throw new NotSupportedException("Illegal file type");
            }

            Debug.Assert(pcgMemory != null);
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
            return new Zero3RwFileReader(pcgMemory, content, ContentType, SysExStartOffset, SysExEndOffset);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override ISongMemory CreateSongMemory(string fileName)
        {
            ISongMemory songMemory = new Zero3RwSongMemory(fileName);
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
            return new Zero3RwSongFileReader(memory, content);
        }
    }
}
