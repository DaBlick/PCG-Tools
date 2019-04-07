using System;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchWaveSequences;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysWaveSequenceBank : WaveSequenceBank
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
                        return 150;
                    
                    case BankType.EType.User:
                        return 32;

                    case BankType.EType.Gm: // fall through
                    case BankType.EType.UserExtended: // fall through
                    case BankType.EType.Virtual: // fall through
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveSeqBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        protected KronosOasysWaveSequenceBank(IWaveSequenceBanks waveSeqBanks, BankType.EType type, string id, int pcgId) : base(waveSeqBanks, type, id, pcgId)
        {
        }
    }
}
