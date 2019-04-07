

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossDrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KrossDrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new KrossDrumKitBank(this, BankType.EType.Int, "INT",  -1));

            // 00(INT)..31(INT)
            //32(USER)..47(USER)
            foreach (var id in new[] { "INT", "U-B", "U-C", "U-D", "U-E", "U-F", "U-G" })
            {
                Add(new KrossDrumKitBank(this, BankType.EType.User, id, -1));
            }
        }
    }
}
