
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MkxlAllFileReader : MkxlFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        protected MkxlAllFileReader(IPcgMemory currentPcgMemory, byte[] content)
            : base(currentPcgMemory, content)
        {
        }
    }
}