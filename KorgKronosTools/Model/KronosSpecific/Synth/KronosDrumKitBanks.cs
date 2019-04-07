
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.KronosOasysSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosDrumKitBanks : KronosOasysDrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KronosDrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new KronosDrumKitBank(this, BankType.EType.Int, "INT", -1));

            foreach (var id in new[] { "U-A", "U-B", "U-C", "U-D", "U-E", "U-F", "U-G" })
            {
                Add(new KronosDrumKitBank(this, BankType.EType.User, id, -1));
            }

            foreach (var id in new[] { "U-AA", "U-BB", "U-CC", "U-DD", "U-EE", "U-FF", "U-GG" })
            {
                Add(new KronosDrumKitBank(this, BankType.EType.User, id, -1));
            }

            // LV: GM Drumkit bank. For the patches, probably use a separate subclass KronosGmDrumKit (similar to KronosGmProgram)?
        }
    }
}
