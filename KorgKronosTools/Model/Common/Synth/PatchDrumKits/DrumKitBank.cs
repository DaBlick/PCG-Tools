using System;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.PcgToolsResources;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.Common.Synth.PatchDrumKits
{
    /// <summary>
    /// </summary>
    public abstract class DrumKitBank : Bank<DrumKit>, IDrumKitBank
    {
        /// <summary>
        /// </summary>
        /// <param name="drumKitBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        protected DrumKitBank(IBanks drumKitBanks, BankType.EType type, string id, int pcgId)
            : base(drumKitBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// </summary>
        public override string Name
        {
            get { return "n.a."; }
            set { throw new ApplicationException("Not yet implemented"); }
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Strings.Bank_2str} {Id}";
        }
    }
}