
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Model.KronosOasysSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysWaveSequence : KronosOasysWaveSequence
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveSeqBank"></param>
        /// <param name="index"></param>
        public OasysWaveSequence(IWaveSequenceBank waveSeqBank, int index)
            : base(waveSeqBank, index)
        {
        }
    }
}
