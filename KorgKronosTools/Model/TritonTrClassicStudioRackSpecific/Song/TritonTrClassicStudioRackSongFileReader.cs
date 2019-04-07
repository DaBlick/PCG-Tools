// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.TritonSpecific.Song;
using PcgTools.Model.TritonTrClassicStudioRackSpecific.Synth;

namespace PcgTools.Model.TritonTrClassicStudioRackSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonTrClassicStudioRackSongFileReader: TritonSongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public TritonTrClassicStudioRackSongFileReader(ISongMemory songMemory, byte[] content) 
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
            return new TritonTrClassicStudioRackTimbre(timbres, index);
        }
    }
}
