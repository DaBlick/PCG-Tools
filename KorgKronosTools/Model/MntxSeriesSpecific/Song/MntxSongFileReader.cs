// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.MntxSeriesSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MntxSongFileReader: SongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        protected MntxSongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


        /// <summary>
        /// M, N, T and X series do not use chunks.
        /// </summary>
        public override void ReadChunks()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Number of song tracks (equal to combi timbres)
        /// </summary>
        public override int NumberOfSongTracks => 8;
    }
}
