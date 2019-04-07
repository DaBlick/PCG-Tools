// (c) 2011 Michel Keijzers

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.PatchDrumKits
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DrumKitBanks : Banks<IDrumKitBank>, IDrumKitBanks
    {
        protected DrumKitBanks(IPcgMemory pcgMemory) : base(pcgMemory)
        {
        }


        protected abstract void CreateBanks();
        

        public override void Fill()
        {
            CreateBanks();
            FillDrumKits();
        }

        
        void FillDrumKits()
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
        public IDrumKit GetByIndex(int indexToSearch)
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
                    return  (IDrumKit) bank.Patches[indexToSearch];
                }

                indexToSearch -= bank.CountPatches;
            }

            return null;
        }


        /// <summary>
        /// Returns the index from the drum kit. -1 if not found.
        /// </summary>
        /// <param name="drumKit"></param>
        /// <returns></returns>
        public int FindIndexOf(IDrumKit drumKit)
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

                foreach (var drumKitInBank in bank.Patches)
                {
                    if (drumKitInBank == drumKit)
                    {
                        return foundIndex;
                    }
                    foundIndex++;
                }
            }

            return -1;
        }
    }
}
