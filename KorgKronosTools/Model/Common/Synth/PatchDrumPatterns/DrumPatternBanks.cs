// (c) 2011 Michel Keijzers

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchDrumPatterns
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DrumPatternBanks : Banks<IDrumPatternBank>, IDrumPatternBanks
    {
        protected DrumPatternBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        protected abstract void CreateBanks();
        

        public override void Fill()
        {
            CreateBanks();
            FillDrumPatterns();
        }


        void FillDrumPatterns()
        {
            foreach (var bank in BankCollection)
            {
                for (var index = 0; index < bank.NrOfPatches; index++)
                {
                    bank.CreatePatch(index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name => "n.a.";


        /// <summary>
        /// 
        /// </summary>
        public int Drk2PcgOffset { get; set; }


        /// <summary>
        /// Returns the indexToSearch, starting with indexToSearch 0 as first bank, first indexToSearch, 
        /// and continuing over banks.
        /// </summary>
        /// <param name="indexToSearch"></param>
        /// <returns></returns>
        public IDrumPattern GetByIndex(int indexToSearch)
        {
            if (BankCollection == null)
            {
                return null;
            }

            foreach (var bank in BankCollection)
            {
                if (!bank.IsLoaded && !bank.IsFromMasterFile)
                {
                    return null;
                }

                if (indexToSearch < bank.CountPatches)
                {
                    return (IDrumPattern)bank.Patches[indexToSearch];
                }

                indexToSearch -= bank.CountPatches;
            }

            return null;
        }


        /// <summary>
        /// Returns the index from the drum Pattern. -1 if not found.
        /// </summary>
        /// <param name="drumPattern"></param>
        /// <returns></returns>
        public int FindIndexOf(IDrumPattern drumPattern)
        {
            var foundIndex = 0;

            if (BankCollection == null)
            {
                return -1;
            }

            foreach (var bank in BankCollection)
            {
                if (!bank.IsLoaded && !bank.IsFromMasterFile)
                {
                    return -1;
                }

                foreach (var drumPatternInBank in bank.Patches)
                {
                    if (drumPatternInBank == drumPattern)
                    {
                        return foundIndex;
                    }
                    foundIndex++;
                }
            }

            return -1;
        }

        public IDrumPatternBank GetDrumPatternBankWithPcgId(int pcgId)
        {
            throw new System.NotImplementedException();
        }
    }
}
