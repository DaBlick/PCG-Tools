

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

namespace PcgTools.Model.KromeExSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KromeExDrumPatternBanks : DrumPatternBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KromeExDrumPatternBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new KromeExDrumPatternBank(this, BankType.EType.Int, "P", 0)); // Preset

            Add(new KromeExDrumPatternBank(this, BankType.EType.User, "U", 1)); // User
        }
    }
}
