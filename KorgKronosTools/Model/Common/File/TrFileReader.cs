
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TrFileReader : PatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        private PcgMemory.ContentType ContentType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        protected TrFileReader(IPcgMemory currentPcgMemory, byte[] content, 
            PcgMemory.ContentType contentType)
            : base(currentPcgMemory, content)
        {
            ContentType = contentType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programPatchSize"></param>
        /// <param name="combiPatchSize"></param>
        /// <param name="globalSize"></param>
        /// <param name="startProgramBank"></param>
        /// <param name="startCombiBank"></param>
        protected void ReadAllData(int programPatchSize, int combiPatchSize, int globalSize, 
            int startProgramBank = 0, int startCombiBank = 0)
        {
            Index = 0;

            // Read global data.
            CurrentPcgMemory.Global.ByteOffset = Index;

            if (ContentType == PcgMemory.ContentType.All)
            {
                // Skip global.
                Index += globalSize; 
            }

            ReadCombis(combiPatchSize, startCombiBank);

            ReadPrograms(programPatchSize, startProgramBank);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiPatchSize"></param>
        /// <param name="startCombiBank"></param>
        private void ReadCombis(int combiPatchSize, int startCombiBank)
        {
            if ((ContentType == PcgMemory.ContentType.All) ||
                (ContentType == PcgMemory.ContentType.AllCombis))
            {
                // Read combi data.
                var combiBank = (CombiBank) CurrentPcgMemory.CombiBanks[0];

                for (var bankIndex = startCombiBank; bankIndex < CurrentPcgMemory.CombiBanks.BankCollection.Count; bankIndex++)
                {
                    ReadCombiBank(combiPatchSize, bankIndex, combiBank);

                    // When virtual banks are used, here needs to be checked to stop reading combi banks.
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiPatchSize"></param>
        /// <param name="bankIndex"></param>
        /// <param name="combiBank"></param>
        private void ReadCombiBank(int combiPatchSize, int bankIndex, CombiBank combiBank)
        {
            var bank = (CombiBank) (CurrentPcgMemory.CombiBanks[bankIndex]);
            if (bank.Type != BankType.EType.Virtual)
            {
                bank.ByteOffset = Index;
                bank.PatchSize = combiPatchSize;
                bank.IsLoaded = true;
                bank.IsWritable = true;

                ReadCombi(combiBank, bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="bank"></param>
        private void ReadCombi(CombiBank combiBank, CombiBank bank)
        {
            foreach (IPatch combi in bank.Patches)
            {
                combi.ByteOffset = Index;
                combi.ByteLength = combiBank.PatchSize;
                combi.IsLoaded = true;

                // Skip to next.
                Index += bank.PatchSize;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programPatchSize"></param>
        /// <param name="startProgramBank"></param>
        private void ReadPrograms(int programPatchSize, int startProgramBank)
        {
            if ((ContentType == PcgMemory.ContentType.All) ||
                (ContentType == PcgMemory.ContentType.AllPrograms))
            {
                // Read program data.
                for (var bankIndex = startProgramBank;
                    bankIndex < CurrentPcgMemory.ProgramBanks.BankCollection.Count;
                    bankIndex++)
                {
                    ReadProgramBank(programPatchSize, bankIndex);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programPatchSize"></param>
        /// <param name="bankIndex"></param>
        private void ReadProgramBank(int programPatchSize, int bankIndex)
        {
            var bank = (ProgramBank) (CurrentPcgMemory.ProgramBanks[bankIndex]);
            if ((bank.Type != BankType.EType.Virtual) && (bank.Type != BankType.EType.Gm))
            {
                bank.ByteOffset = Index;
                bank.PatchSize = programPatchSize;
                bank.IsLoaded = true;
                bank.IsWritable = true;

                ReadProgram(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void ReadProgram(IProgramBank bank)
        {
            foreach (var program in bank.Patches)
            {
                program.ByteOffset = Index;
                program.ByteLength = bank.ByteLength;
                program.IsLoaded = true;

                // Skip to next.
                Index += bank.ByteLength;
            }
        }
    }
}