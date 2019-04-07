// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MSpecific.Pcg;

namespace PcgTools.Model.M3Specific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class M3PcgFileReader : MPcgFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        public M3PcgFileReader(IPcgMemory currentPcgMemory, byte[] content)
            : base(currentPcgMemory, content)
        {
            // PcgFileHeader PcgFileHeader;
            // Pcg1Chunk Pcg1Chunk;
            // Div1Chunk Div1Chunk;
            // Ini2Chunk Ini2Chunk;
            // Ini1Chunk Ini1Chunk;
            // Prg1Chunk Prg1Chunk;
            // Cmb1Chunk Cmb1Chunk;
            // Dkt1Chunk Dkt1Chunk;
            // Arp1Chunk Arp1Chunk;
            // Glb1Chunk Glb1Chunk;

            currentPcgMemory.Model = Models.Find(content[9] == 1 
                ? Models.EOsVersion.EOsVersionM3_20 : Models.EOsVersion.EOsVersionM3_1X);
            if (content[9] == 1)
            {
                currentPcgMemory.PcgChecksumType = PcgMemory.ChecksumType.M3;
            }
        }
    }
}
