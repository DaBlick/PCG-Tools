namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPcgMemoryInit : IMemoryInit
    {
        /// <summary>
        /// 
        /// </summary>
        IChunks Chunks { get; }


        /// <summary>
        /// 
        /// </summary>
        PcgMemory.ChecksumType PcgChecksumType { get; set; }


        /// <summary>
        /// IMPR: Why new?
        /// </summary>
        new IModel Model { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int Sdb1Index { get; set; }


        /// <summary>
        /// 
        /// </summary>
        void Fill();
    }
}
