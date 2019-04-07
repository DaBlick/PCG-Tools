// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PcgTools.Model.Common.Synth;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Model.KronosSpecific.Pcg;
using PcgTools.Model.KronosSpecific.Synth;

namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PcgFileReader : PatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        protected PcgFileReader(IPcgMemory currentPcgMemory, byte[] content) : base(currentPcgMemory, content)
        {
        }


        /// <summary>
        /// Reads content of a PCG file.
        /// </summary>
        /// <param name="filetype"></param>
        /// <param name="modelType"></param>
        public override void ReadContent(Memory.FileType filetype, Models.EModelType modelType)
        {
            Index = Div1Offset;
            while (Index < CurrentPcgMemory.Content.Length)
            {
                var chunkName = Util.GetChars(CurrentPcgMemory.Content, Index, 4);
                var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
                // ReSharper disable LocalizableElement
                Console.WriteLine("index = " + Index.ToString("X08") + ", Chunk name: " +
                                  chunkName + ", size of chunk: " + chunkSize.ToString("X08"));
                // ReSharper enable LocalizableElement
                CurrentPcgMemory.Chunks.Collection.Add(new Chunk(chunkName, Index, chunkSize));
                ReadChunk(chunkName, chunkSize);
            }

            SetNotifications();
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Div1Offset { get; }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int BetweenChunkGapSize { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        private delegate void Function(int chunkSize);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkName"></param>
        /// <param name="chunkSize"></param>
        private void ReadChunk(string chunkName, int chunkSize)
        {
            var map = new Dictionary<string, Function>
            {
                {"INI2", ReadIni2Chunk},
                {"INI3", ReadIni3Chunk},
                {"CMB1", ReadCmb1Chunk},
                {"CMB2", ReadCmb2Chunk},
                {"PRG1", ReadPrg1Chunk},
                {"PRG2", ReadPrg2Chunk},
                {"SLS1", ReadSls1Chunk},
                {"STL2", ReadStl2Chunk},
                {"WSQ1", ReadWsq1Chunk},
                {"DKT1", ReadDkt1Chunk},
                {"GLB1", ReadGlb1Chunk},
                {"DPI1", ReadDpi1Chunk}
            };

            if (map.ContainsKey(chunkName))
            {
                map[chunkName](chunkSize);
            }
            else
            {
                // Others are ignored for now.
                Index += chunkSize + BetweenChunkGapSize;
            }
        }


        /// <summary>
        /// Kronos has checksum bytes.
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadIni2Chunk(int chunkSize)
        {
            Index += 12;
            Index += chunkSize;
        }


        /// <summary>
        /// Kronos has checksum bytes. Used in OS Update 1.5 and 1.6.
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadIni3Chunk(int chunkSize)
        {
            if (CurrentPcgMemory is KronosPcgMemory)
            {
                CurrentPcgMemory.PcgChecksumType = PcgMemory.ChecksumType.Kronos1516;
                CurrentPcgMemory.Model = Models.Find(Models.EOsVersion.EOsVersionKronos15_16);
            }

            ReadIni2Chunk(chunkSize);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadCmb1Chunk(int chunkSize)
        {
            Index += SizeBetweenCmb1AndCbk1 + 4;
            var start = Index;
            while (Index - start < chunkSize)
            {
                ReadCbk1Chunk();
            }
        }


        /// <summary>
        /// Returns bank index.
        /// </summary>
        /// <returns></returns>
        private void ReadCbk1Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("CBK1", Index, chunkSize));

            var startIndex = Index;
            Index += Cbk1NumberOfCombisOffset;
            var numberOfCombisInBank = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 4;
            var sizeOfACombi = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
// ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a combi: {sizeOfACombi}");
// ReSharper restore RedundantStringFormatCall
            Index += 4;
            var bankId = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            var bankIndex = CombiBankId2CombiIndex(bankId);
            Index += 4;

            var combiBank = (CombiBank) CurrentPcgMemory.CombiBanks[bankIndex];
            combiBank.ByteOffset = startIndex;
            combiBank.PatchSize = sizeOfACombi;
            combiBank.IsWritable = true;
            combiBank.IsLoaded = true;

            for (var index = 0; index < numberOfCombisInBank; index++)
            {
                // Place in PcgMemory.
                var combi = (Combi) combiBank[index];
                combi.ByteOffset = Index;
                combi.ByteLength = sizeOfACombi;
                combi.IsLoaded = true;

                combi.Timbres.ByteOffset = combi.ByteOffset + TimbresByteOffset;

                foreach (var timbre in combi.Timbres.TimbresCollection)
                {
                    timbre.ByteOffset = combi.Timbres.ByteOffset + timbre.Index*timbre.TimbresSize;
                }

                // Skip to next.
                //for (int timbre = 0; timbre < 8; timbre++)
                //    Console.WriteLine(String.Format("Combi id {0}, timbre index  {1} bank {2} program {3}, offset {4:x}",
                //        combi.Id, combi.Timbres[timbre].Index, 
                //        combi.Timbres[timbre].UsedProgramBank.Id, combi.Timbres[timbre].UsedProgram.Id,
                //        combi.ByteOffset));
                Index += sizeOfACombi;
            }
        }


        /// <summary>
        /// Kronos only.
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadCmb2Chunk(int chunkSize)
        {
            //CurrentPcgMemory.Chunks.Add(new Chunk("CMB1", Index, -1));
            Index += KronosCombi.SizeBetweenCmb2AndCbk2 + 4;
            var start = Index;
            var bankIndex = 0;
            while (Index - start < chunkSize)
            {
                // Find combi bank with index of filled banks.
                var writableBanks = CurrentPcgMemory.CombiBanks.BankCollection.Where(bank => bank.IsWritable);
                var combiBank = (KronosCombiBank) writableBanks.ToArray()[bankIndex];

                // Set offset.
                combiBank.Cbk2PcgOffset = Index + 16; // 12 = Chunk size, 8 zeros
                bankIndex++;
                ReadCbk2Chunk();
            }
        }


        /// <summary>
        /// Kronos only.
        /// </summary>
        private void ReadCbk2Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            Index += chunkSize + 12; // 12 = chunk size, 8 zeros
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int SizeBetweenCmb1AndCbk1 { get; }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Cbk1NumberOfCombisOffset { get; }


        /// <summary>
        /// Converts 0 (I-A) to 0 ... 6 (I-G) to 6, 0x20000 (U-A) to 7 ... 0x20006 (U-G) to 13
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int CombiBankId2CombiIndex(int id)
        {
            // Check for virtual bank.
            if ((id >= CombiBanks.FirstVirtualBankId) &&
                (id <= (CombiBanks.FirstVirtualBankId + CombiBanks.NumberOfVirtualBanks)))
            {
                return id - CombiBanks.FirstVirtualBankId + 14; // Kronos specific, 14 internal banks
            }

            // Convert non virtual banks.
            return id < 0x20000 ? id : id - 0x20000 + 7;
        }


        /// <summary>
        /// Kronos only. There is only 1 SBK2 chunk.
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadStl2Chunk(int chunkSize)
        {
            Index += KronosSetListSlot.SizeBetweenStl2AndSbk2 + 4;
            CurrentPcgMemory.SetLists.Stl2PcgOffset = Index + 16; // 12 = chunk size, 8 zeros, (1) SBK2 chunk header.
            Index += chunkSize;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadPrg1Chunk(int chunkSize)
        {
            Index += BetweenChunkGapSize; // Skip PRG1 + size + 4 other bytes
            var start = Index;
            while (Index - start < chunkSize)
            {
                switch (Util.GetChars(CurrentPcgMemory.Content, Index, 4))
                {
                    case "MBK1":
                        ReadMbk1Chunk();
                        break;

                    case "PBK1":
                        ReadPbk1Chunk();
                        break;

                    default:
                        throw new ApplicationException("Illegal case");
                }
            }
        }


        /// <summary>
        /// Kronos only.
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadPrg2Chunk(int chunkSize)
        {
            Index += KronosProgram.SizeBetweenPrg2AndPbk2 + 4;
            var start = Index;
            var bankIndex = 0;
            while (Index - start < chunkSize)
            {
                // Find program bank with index of filled banks.
                var writableBanks = CurrentPcgMemory.ProgramBanks.BankCollection.Where(bank => bank.IsWritable);
                var programBank = writableBanks.ToArray()[bankIndex] as KronosProgramBank;

                // Set offset.
                if (programBank != null) // It is null if it is a GM bank
                {
                    programBank.Pbk2PcgOffset = Index + 16; // 12 = Chunk size, 8 zeros
                }
                bankIndex++;
                ReadPbk2Chunk();
            }
        }


        /// <summary>
        /// Kronos only.
        /// </summary>
        private void ReadPbk2Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            Index += chunkSize + 12; // 12 = chunk size, 8 zeros
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int GapSizeAfterMbk1ChunkName { get; }


        /// <summary>
        /// For special banks (EXI, Moss, ...).
        /// Returns bank index.
        /// </summary>
        private void ReadMbk1Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("MBK1", Index, chunkSize));
            var startIndex = Index;
            Index += GapSizeAfterMbk1ChunkName;

            var numberOfProgramsInBank = Util.GetInt(CurrentPcgMemory.Content, Index + 8, 4);
            var sizeOfAProgram = Util.GetInt(CurrentPcgMemory.Content, Index + 12, 4);
// ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a program: {sizeOfAProgram}");
// ReSharper restore RedundantStringFormatCall
            var bankId = Util.GetInt(CurrentPcgMemory.Content, Index + 16, 4);
            var bankIndex = ProgramBankId2ProgramIndex(bankId);
            Index += 5*4;

            var programBank = (ProgramBank) CurrentPcgMemory.ProgramBanks[bankIndex];
            programBank.ByteOffset = startIndex;
            programBank.PatchSize = sizeOfAProgram;
            programBank.BankSynthesisType = programBank.DefaultModeledSynthesisType;
            programBank.IsWritable = true;
            programBank.IsLoaded = true;

            for (var index = 0; index < numberOfProgramsInBank; index++)
            {
                // Place in PcgMemory.
                var program = (Program) programBank[index];
                program.ByteOffset = Index;
                program.ByteLength = sizeOfAProgram;
                program.IsLoaded = true;

                // Skip to next.
                Index += sizeOfAProgram;
            }
        }


        /// <summary>
        /// For 'normal' EDS, HD1 like programs.
        /// Returns the bank index.
        /// </summary>
        private void ReadPbk1Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("PBK1", Index, chunkSize));

            var startIndex = Index;
            Index += Pbk1NumberOfProgramsOffset;
            var numberOfProgramsInBank = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 4;
            var sizeOfAProgram = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
// ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a program: {sizeOfAProgram}");
// ReSharper restore RedundantStringFormatCall
            Index += 4;
            var bankId = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            var bankIndex = ProgramBankId2ProgramIndex(bankId);
            Index += 4;

            var programBank = (ProgramBank) CurrentPcgMemory.ProgramBanks[bankIndex];
            programBank.ByteOffset = startIndex;
            programBank.PatchSize = sizeOfAProgram;
            programBank.BankSynthesisType = programBank.DefaultSampledSynthesisType;
            programBank.IsWritable = true;
            programBank.IsLoaded = true;

            for (var index = 0; index < numberOfProgramsInBank; index++)
            {
                // Place in PcgMemory.
                var program = (Program) programBank[index];
                program.ByteOffset = Index;
                program.ByteLength = sizeOfAProgram;
                program.IsLoaded = true;

                // Skip to next.
                Index += sizeOfAProgram;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Pbk1NumberOfProgramsOffset { get; }


        /// <summary>
        /// Kronos specific function for MBK1/PBK1 chunks.
        /// 
        /// ModelType ID        Name     Index
        ///           Kronos Triton
        ///           Oasys
        /// MBK 00000  IA       A     0
        /// PBK 00001  IB       B     1
        /// PBK 00002  IC       C     2
        /// PBK 00003  ID       D     3
        /// PBK 00004  IE       E     4
        /// PBK 08000  IF       F     5
        /// PBK 20000  UA       H     6
        /// MBK 20001  UB       I     7
        /// MBK 20002  UC       J     8
        /// MBK 20003  UD       K     9
        /// MBK 20004  UE       L    10
        /// MBK 20005  UF       M    11
        /// MBK 20006  UG       N    12
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int ProgramBankId2ProgramIndex(int id)
        {
            // Check for virtual bank.
            if ((id >= ProgramBanks.FirstVirtualBankId) &&
                (id <= (ProgramBanks.FirstVirtualBankId + ProgramBanks.NumberOfVirtualBanks)))
            {
                return id - ProgramBanks.FirstVirtualBankId + 20; // Kronos specific
            }

            // Non virtual banks.
            return (id == 0x8000 ? 5 : (id < 0x8000 ? id : id - 0x20000 + 6)); // Kronos specific
        }


        // Set lists.

        /// <summary>
        /// 
        /// </summary>
        private void ReadSls1Chunk(int chunkSize)
        {
            Index += 16; // Skip SLD1.
            CurrentPcgMemory.Sdb1Index = Index + 12;
            Index += Util.GetInt(CurrentPcgMemory.Content, Index, 4) + 8; // Skip SDB1 chunk until STL1
            Index += 12; // Skip fist part STL1 chunk

            ReadSetList();
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadSetList()
        {
            // Add SBK1 chunk.
            var name = Util.GetChars(CurrentPcgMemory.Content, Index, 4);
            var chunkStartIndex = Index;
            Debug.Assert(name == "SBK1");
            Index += 4;
            var sbk1ChunkSize = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk(name, chunkStartIndex, sbk1ChunkSize));
            Index += 8;

            // Parse set lists.
            var numberOfSetLists = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 4;
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            var sizeOfASetListSlot = chunkSize/numberOfSetLists;
            Index += 8; // Skip 8 other bytes

            for (var setListIndex = 0; setListIndex < numberOfSetLists; setListIndex++)
            {
                // Place in PcgMemory.
                var setList = (SetList) CurrentPcgMemory.SetLists[setListIndex];
                setList.ByteOffset = Index;
                setList.PatchSize = sizeOfASetListSlot;
                setList.Name = Util.GetChars(CurrentPcgMemory.Content, Index, 24);
                setList.IsWritable = true;
                setList.IsLoaded = true;
                Index += 24;

                for (var slotIndex = 0; slotIndex < 128; slotIndex++)
                {
                    var setList2 = (SetList) CurrentPcgMemory.SetLists[setListIndex];
                    var slot = (SetListSlot) setList2[slotIndex];
                    slot.ByteOffset = Index;
                    slot.ByteLength = sizeOfASetListSlot;
                    slot.IsLoaded = true;

                    // Skip to next.
                    Index += sizeOfASetListSlot;
                }
                Index += 16;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadWsq1Chunk(int chunkSize)
        {
            Index += BetweenChunkGapSize;
            var start = Index;
            while (Index - start < chunkSize)
            {
                ReadWbk1Chunk();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadWbk1Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("WBK1", Index, chunkSize));

            var startIndex = Index;
            Index += 12; // LV? Wbk1NumberOfWaveSequencesOffset; override for various synths?
            var numberOfWaveSeqsInBank = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 4;
            var sizeOfAWaveSeq = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            // ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a waveseq: {sizeOfAWaveSeq}");
            // ReSharper restore RedundantStringFormatCall
            Index += 4;
            var bankId = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            var bankIndex = WaveSequenceBankId2WaveSequenceIndex(bankId);
            Index += 4;

            var waveSeqBank = (WaveSequenceBank) CurrentPcgMemory.WaveSequenceBanks[bankIndex];
            waveSeqBank.ByteOffset = startIndex;
            waveSeqBank.PatchSize = sizeOfAWaveSeq;
            waveSeqBank.IsWritable = true;
            waveSeqBank.IsLoaded = true;

            for (var index = 0; index < numberOfWaveSeqsInBank; index++)
            {
                // Place in PcgMemory.
                var waveSeq = (WaveSequence) waveSeqBank[index];
                waveSeq.ByteOffset = Index;
                waveSeq.ByteLength = sizeOfAWaveSeq;
                waveSeq.IsLoaded = true;

                Index += sizeOfAWaveSeq;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Dbk1NumberOfDrumKitsOffset { get; }


        /// <summary>
        /// Converts 0 (INT) to 0, 0x20000 (USER-A) to 1 ... 0x20006 (USER-GG) to 14
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int WaveSequenceBankId2WaveSequenceIndex(int id)
        {
            return id < 0x20000 ? id : id - 0x20000 + 1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunkSize"></param>
        private void ReadDkt1Chunk(int chunkSize)
        {
            Index += BetweenChunkGapSize;
            var start = Index;
            while (Index - start < chunkSize)
            {
                ReadDbk1Chunk();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadDbk1Chunk()
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("DBK1", Index, chunkSize));

            var startIndex = Index;
            Index += Dbk1NumberOfDrumKitsOffset;
            var numberOfDrumKitsInBank = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 4;
            var sizeOfADrumKit = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
// ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a drumkit: {sizeOfADrumKit}");
// ReSharper restore RedundantStringFormatCall
            Index += 4;
            var bankId = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            var bankIndex = DrumKitBankId2DrumKitIndex(bankId);
            Index += 4;

            var drumKitBank = (DrumKitBank) CurrentPcgMemory.DrumKitBanks[bankIndex];
            drumKitBank.ByteOffset = startIndex;
            drumKitBank.PatchSize = sizeOfADrumKit;
            drumKitBank.IsWritable = true;
            drumKitBank.IsLoaded = true;

            for (var index = 0; index < numberOfDrumKitsInBank; index++)
            {
                // Place in PcgMemory.
                var drumKit = (DrumKit) drumKitBank[index];
                drumKit.ByteOffset = Index;
                drumKit.ByteLength = sizeOfADrumKit;
                drumKit.IsLoaded = true;

                Index += sizeOfADrumKit;
            }
        }


        /// <summary>
        /// LV: This function might require attention for Triton models. I don't have PCG files to verify.
        ///
        /// For all models except Triton:
        ///     Converts 0 (INT) to 0, 0x20000 (USER-A) to 1 ... 0x2000c (USER-GG) to 14
        /// For Triton Extreme model:
        ///     Converts 0 (A/B) to 0, 0x20000 (H) to 1 ... 0x20006 (N) to 7, 0x20007 (USER) to 8
        /// For other Triton models (???):
        ///     Converts 0 (A/B) to 0, 1 (C) to 1, 2 (D) to 2, 3 or 0x20000???? (USER) to 3
        ///     If "USER" has id 0x20000 then this function will not work, it overwrites an Int index...
        /// Sorry but I could not do better than this at the moment, the Triton Drumkit banks are very differently organized...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int DrumKitBankId2DrumKitIndex(int id)
        {
            return id < 0x20000 ? id : id - 0x20000 + 1;
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual void ReadGlb1Chunk(int chunkSizeNotUsed)
        {
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            Index += 12; // Skip 'GLB1", chunk size and 4 other/unknown bytes.
            //CurrentPcgMemory.Chunks.Add(new Chunk("GLB1", Index, chunkSize));
            CurrentPcgMemory.Global.ByteOffset = Index;
            Index += chunkSize;

            /*
            var chunkSize = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            Index += 12; // Skip 'GLB1", chunk size and 4 other/unknown bytes.
            //CurrentPcgMemory.Chunks.Add(new Chunk("GLB1", Index, chunkSize));
            CurrentPcgMemory.Global.ByteOffset = Index;
            Index += chunkSize - 4; // 4 because initial 12 was too much (see above).
             */
        }


        /// <summary>
        /// DPI1 chunk consists of:
        /// - DPN1  size x01D8
        /// - DPD1  size x1CCC contains drum pattern names + other (see documentation).
        /// - DPS1  size x4F4B8 containing
        ///   - multiple DPV1 0x17C00 probably containing the drum kit sequences 
        /// </summary>
        private void ReadDpi1Chunk(int chunkSize)
        {
            CurrentPcgMemory.Chunks.Collection.Add(new Chunk("DPI1", Index, chunkSize));

            // Goto DPN1.
            Index += 12; // Skip DPI1 header.
            Debug.Assert(Util.GetChars(CurrentPcgMemory.Content, Index, 4) == "DPN1");

            Index += Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4) + 12; // Skip DPN1

            Debug.Assert(Util.GetChars(CurrentPcgMemory.Content, Index, 4) == "DPD1");
            Index += 4;
            var dpd1ChunkSize = Util.GetInt(CurrentPcgMemory.Content, Index, 4);
            Index += 8; // Skip DPD1 header

            var sizeOfADrumPattern = Util.GetInt(CurrentPcgMemory.Content, Index + 4, 4);
            var numberOfDrumPatternsInBank = dpd1ChunkSize / sizeOfADrumPattern;
            Debug.Assert(Util.GetInt(CurrentPcgMemory.Content, Index, 4) == numberOfDrumPatternsInBank);
            // ReSharper disable RedundantStringFormatCall
            Console.WriteLine($" Size of a drumpattern: {sizeOfADrumPattern}");
            // ReSharper restore RedundantStringFormatCall
            var bankId = 1; // User bank
            var bankIndex = DrumPatternBankId2DrumPatternIndex(bankId);

            var drumPatternBank = (DrumPatternBank) CurrentPcgMemory.DrumPatternBanks[bankIndex];
            drumPatternBank.ByteOffset = Index + 12; 
            drumPatternBank.PatchSize = sizeOfADrumPattern;
            drumPatternBank.IsWritable = true;
            drumPatternBank.IsLoaded = true;

            Index += 12; // Goto first pattern data
            var index = 0;
            for (index = 0; index < numberOfDrumPatternsInBank; index++)
            {
                // Place in PcgMemory.
                var drumPattern = (DrumPattern) drumPatternBank[index];
                drumPattern.ByteOffset = Index;
                drumPattern.ByteLength = sizeOfADrumPattern;
                drumPattern.IsLoaded = true;

                Index += sizeOfADrumPattern;
            }

            // Clear all remaining (unexisting) patches.
            while (index < drumPatternBank.CountPatches)
            {
                drumPatternBank.Patches.RemoveAt(index);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int Dpi1NumberOfDrumPatternsOffset { get; }


        /// <summary>
        /// LV: This function might require attention for Triton models. I don't have PCG files to verify.
        ///
        /// For all models except Triton:
        ///     Converts 0 (INT) to 0, 0x20000 (USER-A) to 1 ... 0x2000c (USER-GG) to 14
        /// For Triton Extreme model:
        ///     Converts 0 (A/B) to 0, 0x20000 (H) to 1 ... 0x20006 (N) to 7, 0x20007 (USER) to 8
        /// For other Triton models (???):
        ///     Converts 0 (A/B) to 0, 1 (C) to 1, 2 (D) to 2, 3 or 0x20000???? (USER) to 3
        ///     If "USER" has id 0x20000 then this function will not work, it overwrites an Int index...
        /// Sorry but I could not do better than this at the moment, the Triton Drumkit banks are very differently organized...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int DrumPatternBankId2DrumPatternIndex(int id)
        {
            return id < 0x20000 ? id : id - 0x20000 + 1; //TODO
        }
    }
}

