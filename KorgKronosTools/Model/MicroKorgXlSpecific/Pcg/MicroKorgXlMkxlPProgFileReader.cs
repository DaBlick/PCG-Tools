// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlMkxlPProgFileReader : PatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        public MicroKorgXlMkxlPProgFileReader(IPcgMemory currentPcgMemory, byte[] content)
            : base(currentPcgMemory, content)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filetype"></param>
        /// <param name="modelType"></param>
        public override void ReadContent(Memory.FileType filetype, Models.EModelType modelType)
        {
            var bank = (ProgramBank) CurrentPcgMemory.ProgramBanks[0];
            bank.ByteOffset = Index;
            bank.BankSynthesisType = ProgramBank.SynthesisType.Mmt;
            bank.PatchSize = 496;
            bank.IsWritable = true;
            bank.IsLoaded = true;

            // Place in PcgMemory.
            var program = (Program) bank[0];
            program.ByteOffset = 32; // Fixed
            program.ByteLength = bank.PatchSize;
            program.IsLoaded = true;
        }
    }
}
