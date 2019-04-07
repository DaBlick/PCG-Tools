// (c) Copyright 2011-2016 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.Model.KromeSpecific.Synth;
using PcgTools.Model.M3rSpecific.Synth;
using PcgTools.Model.MntxSeriesSpecific.Song;

namespace PcgTools.Model.M3rSpecific.Song
{
    /// <summary>
    /// 
    /// </summary>
    public class M3RSongFileReader : MntxSongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        public M3RSongFileReader(ISongMemory songMemory, byte[] content) 
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
            return new M3RTimbre(timbres, index);
        }
    }
}
