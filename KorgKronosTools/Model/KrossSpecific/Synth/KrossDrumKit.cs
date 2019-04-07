
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.MSpecific.Synth;

// (c) 2011 Michel Keijzers

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossDrumKit : MDrumKit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitBank"></param>
        /// <param name="index"></param>
        public KrossDrumKit(IDrumKitBank drumKitBank, int index)
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
