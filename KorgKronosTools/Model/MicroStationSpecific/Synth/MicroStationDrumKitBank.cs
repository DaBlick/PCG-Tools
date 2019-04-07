
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.MicroStationSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroStationDrumKitBank : DrumKitBank
    {
        // LV: In the microStation docs I only read about a 32 slot INT bank. However I´ve found a 
        // PCG which also had a 16 slot USER bank.
        // Although the microStation isn't supported in PcgTools (see KorgFileReader.ReadFile(), 
        // commented out model 0x8D), I did some basic testing on the drumkit part.
        // Needs more testing once microStation is fully supported in PcgTools though, since I did not
        // get past other exceptions thrown for this model. I commented out the 0x8D again in KorgFileReader.
        public override int NrOfPatches => 32;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public MicroStationDrumKitBank(IBanks drumKitBanks, BankType.EType type, string id, int pcgId)
            : base(drumKitBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new MicroStationDrumKit(this, index));
        }
    }
}
