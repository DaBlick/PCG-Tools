// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.ClipBoard;
using PcgTools.Model.Common;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.KronosOasysSpecific.Pcg;
using PcgTools.Model.KronosSpecific.Synth;
using System;
using System.Linq;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.KronosSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosPcgMemory : KronosOasysPcgMemory
    {
        public KronosPcgMemory(string fileName)
            : base(fileName, Models.EModelType.Kronos)
        {
            CombiBanks = new KronosCombiBanks(this);
            ProgramBanks = new KronosProgramBanks(this);
            SetLists = new KronosSetLists(this);
            WaveSequenceBanks = new KronosWaveSequenceBanks(this);
            DrumKitBanks = new KronosDrumKitBanks(this);
            DrumPatternBanks = new KronosDrumPatternBanks(this);
            Global = new KronosGlobal(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="checksumType"></param>
        protected override void FixChecksumValues(ChecksumType checksumType)
        {
            // Loop through all chunks and fix the checksum in the INI2 and INI3 (in OS update 1.5/1.6) chunk.
            // LV: Add "WSQ1" and "DKT1" if WaveSequenceBanks and DrumKits become editable?
            var checksumChunks = new List<string> {"PBK1", "MBK1", "CBK1", "SBK1", "GLB1", "WBK1", "DBK1"};
            var pbkIndex = 0;
            var mbkIndex = 0;
            var cbkIndex = 0;
            var wsqIndex = 0;
            var dbkIndex = 0;

            var startIndex = checksumType == ChecksumType.Kronos1516 ? 3 : 0; // For OS1.5/1.6, skip DIV1 and INI2/3.
            for (var index = startIndex; index < Chunks.Collection.Count; index++)
            {
                var chunk = Chunks.Collection[index];
                if (checksumChunks.Contains(chunk.Name))
                {
                    var checksum = 0;
                    for (var dataIndex = chunk.Offset + 12; dataIndex < chunk.Offset + chunk.Size + 12; dataIndex++)
                    {
                        // Since checksum is a byte it will be automatically moduloed by 256.
                        checksum = (checksum + Content[dataIndex]) % 256; 
                    }

                    if (checksumType == ChecksumType.Kronos1516)
                    {
                        // Save in INI2.
                        int offsetInIni2; // offsetInIni2 = FindIni2Or3Offset(chunk.Name, 0); 
                                          // IMPR: wrong checksum calculated
                        if (FindIni2Offset(chunk, out offsetInIni2,
                            ref pbkIndex, ref mbkIndex, ref cbkIndex, ref wsqIndex, ref dbkIndex)) continue; 

                        Debug.Assert(offsetInIni2 >= 4); // Don't overwrite KORG header
                        Content[offsetInIni2 + 54] = (byte) checksum;
                        
                        //Console.WriteLine(string.Format(
                        //    "Chunk {0} offset {1:x} size {2:x} has checksum ({3:x}..{4:x}): {5:x}, written to {6:x} and {7:x}",
                        //    chunk.Name, chunk.Offset, chunk.Size,
                        //    chunk.Offset + 12, chunk.Offset + chunk.Size + 12, checksum, offsetInIni2 + 54,
                        //    chunk.Offset + 11));
                    }

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
        /// <param name="wbkIndex"></param>
        /// <param name="dbkIndex"></param>
        /// <returns></returns>
        private bool FindIni2Offset(IChunk chunk, out int offsetInIni2,
            ref int pbkIndex, ref int mbkIndex, ref int cbkIndex, ref int wbkIndex, ref int dbkIndex)
        {
            switch (chunk.Name)
            {
                case "PBK1":
                    offsetInIni2 = FindIni2Or3Offset(chunk.Name, pbkIndex);
                    pbkIndex++;
                    break;

                case "MBK1":
                    offsetInIni2 = FindIni2Or3Offset(chunk.Name, mbkIndex);
                    mbkIndex++;
                    break;

                case "CBK1":
                    offsetInIni2 = FindIni2Or3Offset(chunk.Name, cbkIndex);
                    cbkIndex++;
                    break;

                case "SBK1":
                    offsetInIni2 = FindIni2Or3Offset("SLS1", 0); // Only one
                    break;
                
                case "WBK1":
                    offsetInIni2 = FindIni2Or3Offset(chunk.Name, wbkIndex);
                    wbkIndex++;
                    break;

                case "DBK1":
                    offsetInIni2 = FindIni2Or3Offset(chunk.Name, dbkIndex);
                    dbkIndex++;
                    break;

                case "GLB1":
                    //IMPR not implemented
                    offsetInIni2 = 0; 
                    return true;

                default:
                    throw new ApplicationException("Switch error");
            }
            return false;
        }


        int FindIni2Or3Offset(string chunkNameInIni2, int index)
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

                if (Util.GetChars(Content, offsetInIni, 4) == "INI3")
                {
                    offsetInIni += 16;
                }
            }
            return offsetInIni;
        }


        public override bool AreFavoritesSupported => true;


        /// <summary>
        /// Does not only swap the PRG1 contents but also PRG2.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
        public override void SwapPatch(IPatch patch, IPatch otherPatch)
        {
            if (patch != otherPatch)
            {
                // Swap PRG 1/2 content.
                base.SwapPatch(patch, otherPatch);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="patch"></param>
        public override void CopyPatch(IClipBoardPatch patchToPaste, IPatch patch)
        {
            // Copy PRG1 content.
            base.CopyPatch(patchToPaste, patch);

            if (Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
            {
                if (patch is KronosProgram)
                {
                    // Copy PRG2 content.
                    var programToPaste = (ClipBoardProgram) patchToPaste;
                    for (var parameter = 0; parameter < programToPaste.KronosOs1516Content.Length; parameter++)
                    {
                        var patchParameterOffset = ((KronosProgramBank) (patch.Parent)).GetParameterOffsetInPbk2(
                            patch.Index, parameter);
                        Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                        Root.Content[patchParameterOffset] = programToPaste.KronosOs1516Content[parameter];
                    }
                }
                else if (patch is KronosCombi)
                {
                    // Copy CBK2 content.
                    var combiToPaste = (ClipBoardCombi) patchToPaste;

                    for (var parameter = 0; parameter < KronosCombiBanks.ParametersInCbk2Chunk; parameter++)
                    {
                        for (var timbre = 0; timbre < KronosTimbres.TimbresPerCombiConstant; timbre++)
                        {
                            var patchParameterOffset = ((KronosCombiBank) (patch.Parent)).GetParameterOffsetInCbk2(
                                patch.Index, timbre, parameter);
                            Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                            Root.Content[patchParameterOffset] = combiToPaste.KronosOs1516Content[
                                parameter + timbre * KronosCombiBanks.ParametersInCbk2Chunk];
                        }
                    }
                    
                }
                else
                {
                    var slot = patch as KronosSetListSlot;
                    if (slot != null)
                    {
                        Util.SetInt(this, Content, slot.Stl2BankOffset, 1,
                                    ((ClipBoardSetListSlot) patchToPaste).KronosOs1516Bank);
                        Util.SetInt(this, Content, slot.Stl2PatchOffset, 1,
                                    ((ClipBoardSetListSlot) patchToPaste).KronosOs1516Patch);
                    }
                }

                patch.RaisePropertyChanged(string.Empty, false);
            }
        }


        /// <summary>
        /// IMPR: Combine with method above.
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="patch"></param>
        public override void CopyPatch(IPatch patchToPaste, IPatch patch)
        {
            // Copy PRG1 content.
            base.CopyPatch(patchToPaste, patch);

            if (Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
            {
                if (patch is KronosProgram)
                {
                    // Copy PRG2 content.
                    var programToPaste = (ClipBoardProgram)patchToPaste;
                    for (var parameter = 0; parameter < programToPaste.KronosOs1516Content.Length; parameter++)
                    {
                        var patchParameterOffset = 
                            ((KronosProgramBank)(patch.Parent)).GetParameterOffsetInPbk2(patch.Index, parameter);
                        Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                        Root.Content[patchParameterOffset] = programToPaste.KronosOs1516Content[parameter];
                    }
                }
                else if (patch is KronosCombi)
                {
                    // Copy CBK2 content.
                    var combiToPaste = (ClipBoardCombi)patchToPaste;

                    for (var parameter = 0; parameter < KronosCombiBanks.ParametersInCbk2Chunk; parameter++)
                    {
                        for (var timbre = 0; timbre < KronosTimbres.TimbresPerCombiConstant; timbre++)
                        {
                            var patchParameterOffset = ((KronosCombiBank)(patch.Parent)).GetParameterOffsetInCbk2(
                                patch.Index, timbre, parameter);
                            Debug.Assert(patchParameterOffset >= 4); // Don't overwrite KORG header
                            Root.Content[patchParameterOffset] = combiToPaste.KronosOs1516Content[
                                parameter + timbre * KronosCombiBanks.ParametersInCbk2Chunk];
                        }
                    }

                }
                else
                {
                    var slot = patch as KronosSetListSlot;
                    if (slot != null)
                    {
                        Util.SetInt(this, Content, slot.Stl2BankOffset, 1,
                                    ((ClipBoardSetListSlot)patchToPaste).KronosOs1516Bank);
                        Util.SetInt(this, Content, slot.Stl2PatchOffset, 1,
                                    ((ClipBoardSetListSlot)patchToPaste).KronosOs1516Patch);
                    }
                }

                patch.RaisePropertyChanged(string.Empty, false);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programRawBankIndex"></param>
        /// <param name="programRawIndex"></param>
        /// <returns></returns>
        public override IProgram GetPatchByRawIndices(
            int programRawBankIndex,
            int programRawIndex)
        {
            // check if program bank is loaded, if so, return, else, null.
            var programBank = ProgramBanks.BankCollection.FirstOrDefault(
                bank => (bank.PcgId == programRawBankIndex) && bank.IsLoaded);
            return programBank == null ? null : ((IProgramBank) programBank)[programRawIndex] as IProgram;

        }
    }
}
