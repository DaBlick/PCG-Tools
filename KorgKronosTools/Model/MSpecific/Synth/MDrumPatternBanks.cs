
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.MSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MDrumPatternBanks : DrumPatternBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public MDrumPatternBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new MDrumPatternBank(this, BankType.EType.Int, "P", 0)); // Preset

            Add(new MDrumPatternBank(this, BankType.EType.User, "U", 1));
        }
    }
}
