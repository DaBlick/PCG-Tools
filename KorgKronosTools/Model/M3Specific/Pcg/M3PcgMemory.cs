// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.M3Specific.Synth;
using PcgTools.Model.MSpecific.Pcg;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M3Specific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class M3PcgMemory : MPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public M3PcgMemory(string fileName)
            : base(fileName, Models.EModelType.M3)
        {
            CombiBanks = new M3CombiBanks(this);
            ProgramBanks = new M3ProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new M3DrumKitBanks(this);
            DrumPatternBanks = new MDrumPatternBanks(this);
            Global = new M3Global(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checksumType"></param>
        protected override void FixChecksumValues(ChecksumType checksumType)
        {
            // Loop through all chunks and fix the checksum.
            var checksumChunks = new List<string> {"PBK1", "MBK1", "CBK1", "SBK1", "GLB1"};
            var mbkIndex = 0;
            var cbkIndex = 0;

            Chunks.Collection.Where(chunk => checksumChunks.Contains(chunk.Name)).Aggregate(
                0, (current, chunk) => FixChecksumValue(chunk, current, ref mbkIndex, ref cbkIndex));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="pbkIndex"></param>
        /// <param name="mbkIndex"></param>
        /// <param name="cbkIndex"></param>
        /// <returns></returns>
        private int FixChecksumValue(IChunk chunk, int pbkIndex, ref int mbkIndex, ref int cbkIndex)
        {
            var checksum = 0;
            for (var dataIndex = chunk.Offset + 12; dataIndex < chunk.Offset + chunk.Size + 12; dataIndex++)
            {
                checksum = (checksum + Content[dataIndex])%256;
                    // Since checksum is a byte it will be automatically moduloed by 256
            }

            // Save in INI2.
            int offsetInIni2;
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
                    return pbkIndex;

                default:
                    throw new ApplicationException("Switch error");
            }

            Debug.Assert(offsetInIni2 >= 4); // Don't overwrite KORG header
            Content[offsetInIni2 + 54] = (byte) checksum;

            //Console.WriteLine(string.Format(
            //    "Chunk {0} offset {1:x} size {2:x} has checksum ({3:x}..{4:x}): {5:x}, written to {6:x} and {7:x}",
            //    chunk.Name, chunk.Offset, chunk.Size,
            //    chunk.Offset + 12, chunk.Offset + chunk.Size + 12, checksum, offsetInIni2 + 54, chunk.Offset + 11));

            Debug.Assert(chunk.Offset >= 4); // Don't overwrite KORG header
            Content[chunk.Offset + 11] = (byte) checksum; // 11 is checksum byte offset
            return pbkIndex;
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
