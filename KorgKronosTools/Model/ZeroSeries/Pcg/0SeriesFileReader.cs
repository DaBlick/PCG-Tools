// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;
using PcgTools.Model.Common;
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.ZeroSeries.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesFileReader : SysExFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public ZeroSeriesFileReader(
            IPcgMemory currentPcgMemory, byte[] content, PcgMemory.ContentType contentType,
            int sysExStartOffset, int sysExEndOffset)
            : base(currentPcgMemory, content, contentType, sysExStartOffset, sysExEndOffset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filetype"></param>
        /// <param name="modelType"></param>
        public override void ReadContent(Memory.FileType filetype, Models.EModelType modelType)
        {
            var memory = SkipModeChange(filetype);

            // Continue parsing.
            switch (filetype)
            {
                case Memory.FileType.Syx: // Fall through
                case Memory.FileType.Mid:
                    ReadSyxMidContent(memory);
                    break;

                case Memory.FileType.Raw:
                    ReadRawDiskImage();
                    break;

                default:
                    throw new NotSupportedException("Unsupported file type");
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        private void ReadSyxMidContent(ISysExMemory memory)
        {
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

                case PcgMemory.ContentType.All: // Fall through
                case PcgMemory.ContentType.AllCombis: // Fall through
                case PcgMemory.ContentType.AllPrograms:
                    ReadAllData();
                    break;

                case PcgMemory.ContentType.Global: // Fall through
                case PcgMemory.ContentType.AllSequence: // Fall through
                case PcgMemory.ContentType.Drums:
                    // Do not read anything.
                    break;

                default:
                    throw new NotSupportedException("Unsupported SysEx function");
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
                   (memory.Content[offset + 4] == (int) PcgMemory.ContentType.ModeChange))
            {
                offset += 8;
            }
            memory.SysExStartOffset += offset;
            return offset;
        }

        
        /// <summary>
        /// Improve: use parent method, set patch size in base class.
        /// </summary>
        /// <param name="offset"></param>
        protected virtual void ReadSingleProgram(int offset)
        {
            var bank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[0]);
            bank.ByteOffset = 0;
            bank.BankSynthesisType = ProgramBank.SynthesisType.Ai2;
            bank.ByteLength = 172;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var program = bank[0];
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
            bank.ByteLength = 128;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var combi = bank[0];
            combi.ByteOffset = offset;
            combi.ByteLength = bank.ByteLength;
            combi.IsLoaded = true;
        }


        /// <summary>
        /// Use base class, pass values in constructor.
        /// </summary>
        protected virtual void ReadAllData()
        {
            ReadAllData(172, 128, 1701, 2, 2); // 2, 2: Skip A/B banks for programs/combis
        }


        /// <summary>
        /// Read raw disk image format.
        /// </summary>
        private void ReadRawDiskImage()
        {
            CurrentPcgMemory.Global = null;
            Index = 0x4800;

            var combiBankIndex = 4; // Skip internal banks
            var programBankIndex = 4; // Skip internal banks

            // Read all 4 disk images.
            for (var diskImage = 0; diskImage < 4; diskImage++)
            {
                // Skip global etc.
                Index += 0xA84;

                combiBankIndex = ReadCombiData(combiBankIndex);

                programBankIndex = ReadProgramData(programBankIndex);

                // Go to next disk image.
                switch (diskImage)
                {
                    case 0:
                        Index = 0xD800;
                        break;

                    case 1:
                        Index = 0x38400;
                        break;

                    case 2:
                        Index = 0x41400;
                        break;

                    case 3:
                        // Last; do nothing.
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBankIndex"></param>
        /// <returns></returns>
        private int ReadCombiData(int combiBankIndex)
        {
            var combiBank = (ICombiBank) CurrentPcgMemory.CombiBanks[combiBankIndex];

            combiBank.ByteOffset = Index;
            combiBank.ByteLength = 128;
            combiBank.IsWritable = true;
            combiBank.IsLoaded = true;

            foreach (var combi in combiBank.Patches)
            {
                combi.ByteOffset = Index;
                combi.ByteLength = combiBank.ByteLength;
                combi.IsLoaded = true;

                // Skip to next.
                Index += combiBank.ByteLength;
            }

            combiBankIndex += 1;
            return combiBankIndex;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBankIndex"></param>
        /// <returns></returns>
        private int ReadProgramData(int programBankIndex)
        {
            var programBank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[programBankIndex]);
            programBank.ByteOffset = Index;

            programBank.BankSynthesisType = ProgramBank.SynthesisType.Ai;
            programBank.ByteLength = 172;
            programBank.ByteLength = programBank.ByteLength;
            programBank.IsWritable = true;
            programBank.IsLoaded = true;

            foreach (var program in programBank.Patches)
            {
                program.ByteOffset = Index;
                program.ByteLength = programBank.ByteLength;
                program.IsLoaded = true;

                // Skip to next.
                Index += programBank.ByteLength;
            }

            programBankIndex += 1;
            return programBankIndex;
        }
    }
}