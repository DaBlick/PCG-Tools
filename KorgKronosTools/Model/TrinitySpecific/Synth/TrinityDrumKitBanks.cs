

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.TrinitySpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityDrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TrinityDrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            foreach (var id in new[] { "A" })
            {
                Add(new TrinityDrumKitBank(this, BankType.EType.Int, id, -1));
            }
        }
    }
}
