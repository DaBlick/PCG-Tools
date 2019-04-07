// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISysExMemory : IPcgMemory
    {
        /// <summary>
        /// Convert everything from SysExStartOffset to _sysExEndOffset from 7 to 8 bits where the
        /// first out of eight target bytes contains the MS bits of the seven source bytes.
        /// /// This method is for READING a sysex file.
        /// </summary>
        void Convert7To8Bits();


        /// <summary>
        /// 
        /// </summary>
        PcgMemory.ContentType ContentTypeType { set; }


        /// <summary>
        /// 
        /// </summary>
        int SysExStartOffset { get; set; }
    }
}
