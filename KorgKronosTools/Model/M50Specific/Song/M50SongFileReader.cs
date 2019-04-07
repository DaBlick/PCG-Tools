// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.M50Specific.Synth;

namespace PcgTools.Model.M50Specific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class M50SongFileReader: SongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public M50SongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


        /// <summary>
        /// Number of bytes in a song track (equal to length of a combi timbre).
        /// </summary>
        public override int SongTrackByteLength => 112;


        /// <summary>
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ITimbre CreateTimbre(ITimbres timbres, int index)
        {
            return new M50Timbre(timbres, index);
        }
    }
}
