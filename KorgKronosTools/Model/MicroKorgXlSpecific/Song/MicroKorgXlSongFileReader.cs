// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.MicroKorgXlSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlSongFileReader: SongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public MicroKorgXlSongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


                /// <summary>
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ITimbre CreateTimbre(ITimbres timbres, int index)
        {
            throw new ApplicationException("Songs not supported");
        }
    }
}
