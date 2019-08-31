// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.ZeroSeries.Pcg;

namespace PcgTools.Model.Zero3Rw.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class Zero3RwFileReader : ZeroSeriesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public Zero3RwFileReader(
            IPcgMemory currentPcgMemory, byte[] content, PcgMemory.ContentType contentType,
            int sysExStartOffset, int sysExEndOffset)
            : base(currentPcgMemory, content, contentType, sysExStartOffset, sysExEndOffset)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        protected override void ReadSingleProgram(int offset)
        {
            var bank = (IProgramBank) (CurrentPcgMemory.ProgramBanks[0]);
            bank.ByteOffset = 0;
            bank.BankSynthesisType = ProgramBank.SynthesisType.Ai2;
            bank.ByteLength = 172; // 172 bytes despite of 164 according to manual
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
        protected override void ReadAllData()
        {
            // 172 bytes despite of 164 according to manual
            ReadAllData(172, 128, 21 + 840, 0, 0); // 21+840 = global/drum data length
        }
    }
}