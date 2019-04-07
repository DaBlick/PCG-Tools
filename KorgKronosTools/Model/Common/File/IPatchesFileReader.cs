
using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="modelType"></param>
        void ReadContent(Memory.FileType fileType, Models.EModelType modelType);
    }
}
