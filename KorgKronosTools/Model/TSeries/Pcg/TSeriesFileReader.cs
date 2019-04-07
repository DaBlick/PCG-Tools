// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.TSeries.Pcg
{
    /// <summary>
    /// 
    /// </summary>
// ReSharper disable once InconsistentNaming
    public class TSeriesFileReader : SysExFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public TSeriesFileReader(
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
            var memory = SkipSysexModeChange(filetype);

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
                        case PcgMemory.ContentType.AllCombis:
                        case PcgMemory.ContentType.AllPrograms:
                            ReadAllData();
                            break;

                        case PcgMemory.ContentType.Global:
                        case PcgMemory.ContentType.AllSequence:
                            // Do not read anything.
                            break;
                        
                        default:
                            throw new NotSupportedException("Unsupported SysEx function");
                    }
                    break;

                default:
                    throw new NotSupportedException("Unsupported file type");
            }
        }


        /// <summary>
        /// Skip Mode Change (not for Sysex Manager file and OrigKorg file).
        /// </summary>
        /// <param name="filetype"></param>
        /// <returns></returns>
        private SysExMemory SkipSysexModeChange(Memory.FileType filetype)
        {
            var memory = (SysExMemory) CurrentPcgMemory;
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

                    // default: // Do nothing.
            }
            return memory;
        }


        /// <summary>
        /// Skip mode changes.
        /// Also adapts the SysExFunction.
        /// </summary>
        int SkipModeChanges()
        {
            var offset = 0;
            var memory = (SysExMemory)CurrentPcgMemory;

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
        /// 
        /// </summary>
        /// <param name="offset"></param>
        private void ReadSingleProgram(int offset)
        {
            var bank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[0]);
            bank.ByteOffset = 0;
            bank.BankSynthesisType = ProgramBank.SynthesisType.AnalogModeling;
            bank.ByteLength = 160;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var program = (Program) bank[0];
            program.ByteOffset = offset;
            program.ByteLength = bank.ByteLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        private void ReadSingleCombi(int offset)
        {
            var bank = (ICombiBank)(CurrentPcgMemory.CombiBanks[0]);
            bank.ByteOffset = 0;
            bank.ByteLength = 240;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            var combi = (ICombi)bank[0];
            combi.ByteOffset = offset;
            combi.ByteLength = bank.ByteLength;
            combi.IsLoaded = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void ReadAllData()
        {
            ReadAllData(160, 240, 1701);
        }
    }
}