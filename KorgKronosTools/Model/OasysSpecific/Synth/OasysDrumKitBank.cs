
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.KronosOasysSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysDrumKitBank : KronosOasysDrumKitBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public OasysDrumKitBank(IDrumKitBanks drumKitBanks,  BankType.EType type, string id, int pcgId)
            : base(drumKitBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new OasysDrumKit(this, index));
        }
    }
}
