

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

namespace PcgTools.Model.M3Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M3DrumPatternBanks : DrumPatternBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M3DrumPatternBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new M3DrumPatternBank(this, BankType.EType.Int, "P", 0)); // Preset

            Add(new M3DrumPatternBank(this, BankType.EType.User, "U", 1));
        }
    }
}
