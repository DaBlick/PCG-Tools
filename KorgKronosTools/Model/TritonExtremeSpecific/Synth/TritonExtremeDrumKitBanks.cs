

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.TritonExtremeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonExtremeDrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonExtremeDrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }

        protected override void CreateBanks()
        {
            // LV: Following is based on documentation in "Triton Extreme Operation Guide page 121".
            // I did see it work with a PCG for Triton Extreme which had all Drumkit banks.
            //
            // I am not sure if it is correct for other Triton models that share this same class implementation! 
            // E.g. the bank names used.
            // Also have a look at PcgFileReader.DrumKitBankId2DrumKitIndex() especially for which bank ids in 
            // the PCG are >=0x20000.
            // Sorry but I could not do better than this at the moment, the Triton Drumkit banks are very differently organized...

            foreach (var id in new[] { "A/B", "H", "I", "J", "K", "L", "M", "N" })
            
            {
                Add(new TritonExtremeDrumKitBank(this, BankType.EType.Int, id, -1));
            }

            Add(new TritonExtremeDrumKitBank(this, BankType.EType.User, "USER", -1));
        }
    }
}
