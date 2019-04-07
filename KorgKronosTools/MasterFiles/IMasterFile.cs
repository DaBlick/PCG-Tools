using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.MasterFiles
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMasterFile : ISelectable
    {
        /// <summary>
        /// 
        /// </summary>
        string FileName { get; }


        /// <summary>
        /// 
        /// </summary>
        MasterFile.EFileState FileState { get; }


        /// <summary>
        /// Model
        /// </summary>
        IModel Model { get; }


        /// <summary>
        /// Sets a master file with file name to the model specified. Use an empty string to remove.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileName"></param>
        void SetModel(IModel model, string fileName);
    }
}
