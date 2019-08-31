// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.TritonSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TritonSongFileReader: SongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        protected TritonSongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


        /// <summary>
        /// Tritons do not use chunks.
        /// </summary>
        public override void ReadChunks()
        {
            var end = Util.GetInt(SongMemory.Content, 0x414, 4);

            const int songNameLength = 16;
            var songIndex = 0;
            for (var index = 0x428; index <= end; index += songNameLength)
            {
                SongMemory.Songs.SongCollection.Add(
                    new Common.Synth.SongsRelated.Song(
                        this, songIndex, SongMemory, Util.GetChars(SongMemory.Content, index, songNameLength)));
                songIndex++;
            }
        }


        /// <summary> 
        /// Number of song tracks (equal to combi timbres)
        /// </summary>
        public override int NumberOfSongTracks => 8;
    }
}
