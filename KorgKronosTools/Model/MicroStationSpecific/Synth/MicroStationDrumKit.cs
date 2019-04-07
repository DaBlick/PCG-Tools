using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.MicroStationSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroStationDrumKit : DrumKit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBank"></param>
        /// <param name="index"></param>
        public MicroStationDrumKit(IBank drumKitBank, int index)
            : base(drumKitBank, index)
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
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("Init") && Name.Contains("Drum") && Name.Contains("Kit")) ||
                                               (Name.Contains("Drumkit    U")));


        /// <summary>
        /// Sets parameters after initialization.
        /// </summary>
        public override void SetParameters()
        {
        }
    }
}
