// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.KromeSpecific.Synth;
using PcgTools.Model.MSpecific.Pcg;

namespace PcgTools.Model.KromeSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class KromePcgMemory : MPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public KromePcgMemory(string fileName)
            : base(fileName, Models.EModelType.Krome)
        {
            CombiBanks = new KromeCombiBanks(this);
            ProgramBanks = new KromeProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new KromeDrumKitBanks(this);
            DrumPatternBanks = new KromeDrumPatternBanks(this);
            Global = new KromeGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionKrome);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checksumType"></param>
        protected override void FixChecksumValues(ChecksumType checksumType)
        {
            // Loop through all chunks and fix the checksum.
            var checksumChunks = new List<string> {"PBK1", "MBK1", "CBK1", "SBK1", "GLB1", "DKT1"};
            var pbkIndex = 0;
            var mbkIndex = 0;
            var cbkIndex = 0;
            var dktIndex = 0;

            foreach (var chunk in Chunks.Collection)
            {
                if (checksumChunks.Contains(chunk.Name))
                {
                    var checksum = 0;
                    for (var dataIndex = chunk.Offset + 12; dataIndex < chunk.Offset + chunk.Size + 12; dataIndex++)
                    {
                        // Since checksum is a byte it will be automatically moduloed by 256.
                        checksum = (checksum + Content[dataIndex])%256;
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

                    case "DKT1":
                        offsetInIni2 = FindIni2Offset(chunk.Name, dktIndex);
                        dktIndex++;
                        break;

                    case "GLB1":
                        continue; // offsetInIni2 = FindIni2Offset(chunk.Name, 0); IMPR: wrong checksum calculated

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
                }
            }
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
