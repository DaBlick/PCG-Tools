

// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

namespace PcgTools.Model.MicroStationSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroStationDrumKitBanks : DrumKitBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public MicroStationDrumKitBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            // LV: In the microStation docs I only read about a 32 slot INT bank. However I´ve found a
            // PCG which also had a 16 slot USER bank.
            // Although the microStation isn't supported in PcgTools (see KorgFileReader.ReadFile(), 
            // commented out model 0x8D), I did some basic testing on the drumkit part.
            // Needs more testing once microStation is fully supported in PcgTools though, since I did 
            // not get past other exceptions thrown for this model. I commented out the 0x8D again.
            Add(new MicroStationDrumKitBank(this, BankType.EType.Int,  "INT", -1));
            Add(new MicroStationDrumKitBank(this, BankType.EType.User, "USER", -1));
        }
    }
}
