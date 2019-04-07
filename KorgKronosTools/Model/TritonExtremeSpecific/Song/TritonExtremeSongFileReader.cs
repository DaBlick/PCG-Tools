// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.TritonExtremeSpecific.Synth;
using PcgTools.Model.TritonSpecific.Song;

namespace PcgTools.Model.TritonExtremeSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonExtremeSongFileReader: TritonSongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public TritonExtremeSongFileReader(ISongMemory songMemory, byte[] content) 
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
            return new TritonExtremeTimbre(timbres, index);
        }
    }
}
