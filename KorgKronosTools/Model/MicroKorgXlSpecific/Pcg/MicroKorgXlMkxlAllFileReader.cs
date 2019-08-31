// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlMkxlAllFileReader : PatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        public MicroKorgXlMkxlAllFileReader(IPcgMemory currentPcgMemory, byte[] content)
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
            Index = 0x60;

            var programBank = (ProgramBank) CurrentPcgMemory.ProgramBanks[0];

            const int numberOfProgramsInBank = 64;
            for (var bankIndex = 0; bankIndex < CurrentPcgMemory.ProgramBanks.BankCollection.Count; bankIndex++)
            {
                var bank = (ProgramBank) (CurrentPcgMemory.ProgramBanks[bankIndex]);
                bank.ByteOffset = Index;
                bank.BankSynthesisType = ProgramBank.SynthesisType.Mmt;
                bank.PatchSize = 496;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                for (var index = 0; index < numberOfProgramsInBank; index++)
                {
                    // Place in PcgMemory.
                    var program = (Program) bank[index];
                    program.ByteOffset = Index;
                    program.ByteLength = programBank.PatchSize;
                    program.IsLoaded = true;

                    // Skip to next.
                    Index += 0x210;
                }
            }
        }
    }
}