// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.TritonSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TritonDrumKitBank : DrumKitBank
    {
        /// <summary>
        /// 
        /// </summary>
        public override int NrOfPatches => 16;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        protected TritonDrumKitBank(IBanks drumKitBanks, BankType.EType type, string id, int pcgId)
            : base(drumKitBanks, type, id, pcgId)
            {
        }
    }
}
