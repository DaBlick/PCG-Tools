// (c) 2011 Michel Keijzers

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysDrumPattern : DrumPattern
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPatternBank"></param>
        /// <param name="index"></param>
        protected KronosOasysDrumPattern(IBank drumPatternBank, int index)
            : base(drumPatternBank, index) 
        {
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public override string Name
        {
            get
            {
                return GetChars(0, MaxNameLength);
            }
            set
            {
                if (Name != value)
                {
                    SetChars(0, MaxNameLength, value);
                    OnPropertyChanged("Name");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override int MaxNameLength => 24;


        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmptyOrInit => ((Name == string.Empty) ||
                                               Name.StartsWith("DrumPattern      0") ||
                                               (Name.Contains("Init") && Name.Contains("Drum") && Name.Contains("Pattern")));
    }
}
