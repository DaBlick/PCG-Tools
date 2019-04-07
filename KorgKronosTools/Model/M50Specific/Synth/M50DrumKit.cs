
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.MSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.M50Specific.Synth
{
    public class M50DrumKit : MDrumKit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBank"></param>
        /// <param name="index"></param>
        public M50DrumKit(IDrumKitBank drumKitBank, int index)
            : base(drumKitBank, index)
        {
        }


        ///
        /// <summary>
        /// Sets parameters after initialization.
        /// </summary>
        public override void SetParameters()
        {
        }
    }
}
