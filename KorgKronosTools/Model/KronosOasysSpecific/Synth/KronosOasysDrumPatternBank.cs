using System;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysDrumPatternBank : DrumPatternBank
    {
        /// <summary>
        /// 
        /// </summary>
        public override int NrOfPatches
        {
            get
            {
                switch (Type)
                {
                    case BankType.EType.Int:
                        return 1000; // Maximum amount

                    case BankType.EType.User:
                        return 1000; // Maximum amount

                    default:
                        throw new NotSupportedException();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPatternBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        protected KronosOasysDrumPatternBank(IDrumPatternBanks drumPatternBanks, BankType.EType type, string id, 
            int pcgId)
            : base(drumPatternBanks, type, id, pcgId)
        {
        }
    }
}
