
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IPcgMemory CreatePcgMemory(string fileName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        ISongMemory CreateSongMemory(string fileName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        IPatchesFileReader CreateFileReader(IPcgMemory pcgMemory, byte[] content);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="songMemory"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        ISongFileReader CreateSongFileReader(ISongMemory songMemory, byte[] content);
        
    }
}
