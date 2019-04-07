// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KronosSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosSongFileReader: SongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public KronosSongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


        /// <summary>
        /// Byte offset where timbres start.
        /// </summary>
        protected override int TimbresByteOffset => 4802;


        /// <summary>
        /// Number of bytes in a song track (equal to length of a combi timbre).
        /// </summary>
        public override int SongTrackByteLength => 188;


        /// <summary>
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ITimbre CreateTimbre(ITimbres timbres, int index)
        {
            return new KronosTimbre(timbres, index);
        }

    }
}
