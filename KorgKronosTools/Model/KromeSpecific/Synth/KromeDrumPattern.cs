using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.MSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.KromeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KromeDrumPattern : MDrumPattern
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPatternBank"></param>
        /// <param name="index"></param>
        public KromeDrumPattern(DrumPatternBank drumPatternBank, int index)
            : base(drumPatternBank, index)
        {
        }
    }
}
