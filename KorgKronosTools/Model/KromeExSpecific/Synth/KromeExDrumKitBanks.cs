

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.KromeExSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KromeExDrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KromeExDrumKitBanks(PcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            // 00(INT)..47(INT)
            //48(USER)..79(USER)
            //80(GM)..88(USER)
            Add(new KromeExDrumKitBank(this, BankType.EType.Int , "INT", -1));
            Add(new KromeExDrumKitBank(this, BankType.EType.User, "USER", -1));
            Add(new KromeExDrumKitBank(this, BankType.EType.Gm  , "GM", -1));
        }
    }
}
