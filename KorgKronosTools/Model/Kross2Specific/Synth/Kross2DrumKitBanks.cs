

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.Kross2Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Kross2DrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public Kross2DrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new Kross2DrumKitBank(this, BankType.EType.Int, "INT",  -1));

            // 00(INT)..31(INT)
            //32(USER)..47(USER)
            foreach (var id in new[] { "INT", "USER" })
            {
                Add(new Kross2DrumKitBank(this, BankType.EType.User, id, -1));
            }
        }
    }
}
