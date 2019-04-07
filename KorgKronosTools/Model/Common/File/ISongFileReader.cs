namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISongFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        void ReadChunks();


        /// <summary>
        /// Number of song tracks (equal to number of timbres in a combi).
        /// </summary>
        int NumberOfSongTracks { get; }


        /// <summary>
        /// Number of bytes in a song track (equal to length of a combi timbre).
        /// </summary>
        int SongTrackByteLength { get; }
    }
}
