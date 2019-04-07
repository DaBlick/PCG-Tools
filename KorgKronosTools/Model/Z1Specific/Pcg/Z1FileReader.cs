// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Z1Specific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class Z1FileReader : SysExFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public Z1FileReader(
            IPcgMemory currentPcgMemory, byte[] content, PcgMemory.ContentType contentType,
            int sysExStartOffset, int sysExEndOffset)
            : base(currentPcgMemory, content, contentType, sysExStartOffset, sysExEndOffset)
        {
            //TimbreByteSize = 16;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filetype"></param>
        /// <param name="modelType"></param>
        public override void ReadContent(Memory.FileType filetype, Models.EModelType modelType)
        {
            var memory = SkipModeChange(filetype);

            Index = SysExStartOffset;

            // Continue parsing.
            switch (filetype)
            {
                case Memory.FileType.Syx: // Fall through
                case Memory.FileType.Mid: 
                    memory.Convert7To8Bits();
                    
                    switch (ContentType)
                    {
                        case PcgMemory.ContentType.CurrentProgram:
                            ReadSingleProgram(SysExStartOffset);
                            memory.SynchronizeProgramName();
                            break;

                        case PcgMemory.ContentType.CurrentCombi:
                            ReadSingleCombi(SysExStartOffset);
                            memory.SynchronizeCombiName();
                            break;

                        case PcgMemory.ContentType.All:
                            ReadAllData();
                            break;

                        case PcgMemory.ContentType.AllCombis:
                            // Not used.
                            break;

                        case PcgMemory.ContentType.AllPrograms:
                            ReadPrograms(memory);

                            break;

                        case PcgMemory.ContentType.Global:
                        case PcgMemory.ContentType.AllSequence:
                            // Do not read anything.
                            break;
                        
                        // default: Do nothing, should result in: No Patch data in file
                    }
                    break;

                default:
                    throw new NotSupportedException("Unsupported file type");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        private void ReadPrograms(IMemoryInit memory)
        {
            var unit = Util.GetBits(memory.Content, SysExStartOffset - 3, 5, 4); // 00:Prog/01:Bank/10:All (start with 0), Bank:
            var bankStart = Util.GetBits(memory.Content, SysExStartOffset - 3, 0, 0); // 0:A/1:B
            var programNo = memory.Content[SysExStartOffset - 2]; // Ignored when Bank or All dump

            switch (unit)
            {
                case 0: // Prog
                    ReadSingleProgram(SysExStartOffset, bankStart, programNo);
                    break;

                case 1: // Bank
                    ReadProgramData(bankStart, 1); // 1: Read single bank
                    break;

                case 2: // All Banks
                    ReadAllData();
                    break;

                default:
                    throw new ApplicationException("Illegal unit");
            }
        }


        /// <summary>
        /// Skip Mode Change (not for Sysex Manager file and OrigKorg file).
        /// </summary>
        /// <param name="filetype"></param>
        /// <returns></returns>
        private ISysExMemory SkipModeChange(Memory.FileType filetype)
        {
            var memory = (ISysExMemory) CurrentPcgMemory;
            switch (filetype)
            {
                case Memory.FileType.Syx:
                    if ((Util.GetChars(memory.Content, 0, 14) != "Sysex Manager-") &&
                        (Util.GetChars(memory.Content, 2, 8) != "OrigKorg"))
                    {
                        var offset = SkipModeChanges();
                        SysExStartOffset += offset;
                        ContentType = (PcgMemory.ContentType) memory.Content[offset + 4];
                    }
                    break;

                case Memory.FileType.Mid:
                    break;

                // default: Do nothing.
            }
            return memory;
        }


        /// <summary>
        /// Skip mode changes.
        /// Also adapts the contentType.
        /// </summary>
        int SkipModeChanges()
        {
            var offset = 0;
            var memory = (ISysExMemory)CurrentPcgMemory;

            while ((memory.Content[offset] == 0xF0) && // MIDI SysEx
                   (memory.Content[offset + 1] == 0x42) && // Korg
                   (memory.Content[offset + 4] == (int)PcgMemory.ContentType.ModeChange))
            {
                offset += 8;
            }
            memory.SysExStartOffset += offset;
            return offset;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="bankStart"></param>
        /// <param name="programNo"></param>
        private void ReadSingleProgram(int offset, int bankStart = 0, int programNo = 0)
        {
            var bank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[bankStart]);
            bank.ByteOffset = 0;
            bank.BankSynthesisType = ProgramBank.SynthesisType.AnalogModeling;
            bank.ByteLength = 75;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var program = (IProgram) bank[programNo];
            program.ByteOffset = offset;
            program.ByteLength = bank.ByteLength;
            program.IsLoaded = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        private void ReadSingleCombi(int offset)
        {
            var bank = CurrentPcgMemory.CombiBanks[0];
            bank.ByteOffset = 0;
            bank.ByteLength = 126;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var combi = bank[0];
            combi.ByteOffset = offset;
            combi.ByteLength = bank.ByteLength;
            combi.IsLoaded = true;
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void ReadAllData()
        {
            // Read global data.
            CurrentPcgMemory.Global.ByteOffset = Index;

            if ((ContentType == PcgMemory.ContentType.All) ||
                (ContentType == PcgMemory.ContentType.AllPrograms))
            {
                ReadProgramData(0, 2); // Read both banks
            }

            if ((ContentType == PcgMemory.ContentType.All) ||
                (ContentType == PcgMemory.ContentType.AllCombis))
            {
                ReadCombiData(0, 2);
            }
            
            if (ContentType == PcgMemory.ContentType.All)
            {
                // Skip global.
                Index += 416 + 416; // Global + MIDI (part of global).
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadCombiData(int bankStart, int numberOfBanks)
        {
            for (var bankIndex = bankStart; bankIndex < numberOfBanks; bankIndex++)
            {
                var bank = CurrentPcgMemory.CombiBanks[bankIndex];
                bank.ByteOffset = Index;
                bank.ByteLength = 208;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                for (var index = 0; index < bank.Patches.Count; index++)
                {
                    // Place in PcgMemory.
                    var combi = (ICombi) bank[index];
                    combi.ByteOffset = Index;
                    combi.ByteLength = bank.ByteLength;
                    combi.IsLoaded = true;

                    combi.Timbres.ByteOffset = combi.ByteOffset + TimbresByteOffset;

                    foreach (var timbre in combi.Timbres.TimbresCollection)
                    {
                        timbre.ByteOffset = combi.Timbres.ByteOffset + timbre.Index * timbre.ByteLength;
                    }

                    // Skip to next.
                    Index += bank.ByteLength;
                }
            }

            // When virtual banks are used, here needs to be checked to stop reading combi banks.
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadProgramData(int bankStart, int numberOfBanks)
        {
            for (var bankIndex = bankStart; bankIndex < numberOfBanks; bankIndex++)
            {
                var bank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[bankIndex]);
                bank.ByteOffset = Index;

                bank.BankSynthesisType = ProgramBank.SynthesisType.MossZ1;
                bank.ByteLength = 576;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                for (var index = 0; index < bank.Patches.Count; index++)
                {
                    // Place in PcgMemory.
                    var program = (IProgram) bank[index];
                    program.ByteOffset = Index;
                    program.ByteLength = bank.ByteLength;
                    program.IsLoaded = true;

                    // Skip to next.
                    Index += bank.ByteLength;
                }
            }
        }



        /// <summary>
        /// Byte offset where timbres start.
        /// </summary>
        protected override int TimbresByteOffset => 16;
    }
}