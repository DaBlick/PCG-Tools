// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using System;
using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MSpecific.Pcg;
using PcgTools.Model.MicroStationSpecific.Synth;


namespace PcgTools.Model.MicroStationSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroStationPcgMemory : MPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public MicroStationPcgMemory(string fileName)
            : base(fileName, Models.EModelType.MicroStation)
        {
            CombiBanks = new MicroStationCombiBanks(this);
            ProgramBanks = new MicroStationProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new MicroStationDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new MicroStationGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionMicroStation);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="checksumType"></param>
        protected override void FixChecksumValues(ChecksumType checksumType)
        {
            // Loop through all chunks and fix the checksum.
            var checksumChunks = new List<string> { "PBK1", "MBK1", "CBK1", "SBK1", "GLB1" };
            var pbkIndex = 0;
            var mbkIndex = 0;
            var cbkIndex = 0;

            foreach (var chunk in Chunks.Collection)
            {
                if (checksumChunks.Contains(chunk.Name))
                {
                    var checksum = 0;
                    for (var dataIndex = chunk.Offset + 12; dataIndex < chunk.Offset + chunk.Size + 12; dataIndex++)
                    {
                        checksum = (checksum + Content[dataIndex]) % 256; // Since checksum is a byte it will be automatically moduloed by 256
                    }

                    // Save in INI2.
                    int offsetInIni2;
                    if (SaveIni2Offset(chunk, out offsetInIni2, ref pbkIndex, ref mbkIndex, ref cbkIndex))
                    {
                        continue; // offsetInIni2 = FindIni2Offset(chunk.Name, 0); IMPR: wrong checksum calculated
                    }

                    Debug.Assert(offsetInIni2 >= 4); // Don't overwrite KORG header
                    Content[offsetInIni2 + 22] = (byte)checksum;

                    //Console.WriteLine(string.Format(
                    //    "Chunk {0} offset {1:x} size {2:x} has checksum ({3:x}..{4:x}): {5:x}, written to {6:x} and {7:x}",
                    //    chunk.Name, chunk.Offset, chunk.Size,
                    //    chunk.Offset + 12, chunk.Offset + chunk.Size + 12, checksum, offsetInIni2 + 54, chunk.Offset + 11));

                    Debug.Assert(chunk.Offset >= 4); // Don't overwrite KORG header
                    Content[chunk.Offset + 11] = (byte)checksum; // 11 is checksum byte offset
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="offsetInIni2"></param>
        /// <param name="pbkIndex"></param>
        /// <param name="mbkIndex"></param>
        /// <param name="cbkIndex"></param>
        /// <returns></returns>
        private bool SaveIni2Offset(IChunk chunk, out int offsetInIni2, ref int pbkIndex, ref int mbkIndex, ref int cbkIndex)
        {
            switch (chunk.Name)
            {
                case "PBK1":
                    offsetInIni2 = FindIni2Offset(chunk.Name, pbkIndex);
                    pbkIndex++;
                    break;

                case "MBK1":
                    offsetInIni2 = FindIni2Offset(chunk.Name, mbkIndex);
                    mbkIndex++;
                    break;

                case "CBK1":
                    offsetInIni2 = FindIni2Offset(chunk.Name, cbkIndex);
                    cbkIndex++;
                    break;

                case "GLB1":
                    offsetInIni2 = 0; // Not implemented
                    return true;

                default:
                    throw new ApplicationException("Switch error");
            }
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkNameInIni2"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        int FindIni2Offset(string chunkNameInIni2, int index)
        {
            Debug.Assert(Chunks.Collection[1].Name == "INI2");
            var ini2Start = Chunks.Collection[1].Offset; // Index 1 = INI2

            var offsetInIni = ini2Start + 16;
            var occurence = 0;
            while (true)
            {
                if (Util.GetChars(Content, offsetInIni, 4) == chunkNameInIni2)
                {
                    if (occurence == index)
                    {
                        break;
                    }

                    occurence++;
                }

                offsetInIni += 64; // Size of a chunk in INI2.
            }
            return offsetInIni;
        }

    }
}
