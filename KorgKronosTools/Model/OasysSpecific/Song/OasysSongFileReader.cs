// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KronosOasysSpecific.Song;
using PcgTools.Model.OasysSpecific.Synth;

namespace PcgTools.Model.OasysSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysSongFileReader: KronosOasysSongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public OasysSongFileReader(ISongMemory songMemory, byte[] content) 
            : base(songMemory, content)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ITimbre CreateTimbre(ITimbres timbres, int index)
        {
            return new OasysTimbre(timbres, index);
        }
        

        /// <summary>
        /// Byte offset where timbres start.
        /// </summary>
        protected override int TimbresByteOffset => 4790 + 12;


        /// <summary>
        /// Number of bytes in a song track (equal to length of a combi timbre).
        /// </summary>
        public override int SongTrackByteLength => 186;
    }
}
