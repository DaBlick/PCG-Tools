// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using System;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysProgram : Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        protected KronosOasysProgram(IProgramBank programBank, int index)
            : base(programBank, index) 
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
        public override bool IsEmptyOrInit => ((Name == string.Empty) || (Name.Contains("Init") && Name.Contains("Prog")));


        /// <summary>
        /// Do not take drum track program references and volume into account.
        /// Also the max. number of used bytes is 3705 (while higher indexes have differences between OS2.x and OS3.0.
        /// This is especially needed for Kronos OS3 which has a remapped program organization in the VNL.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override bool UseIndexForDifferencing(int index)
        {
            // 2688 = Volume, 2689/2689 = Program reference.
            return (index < 2688) || ((index > 2690) && (index < 3706));
        }


        /// <summary>
        /// Returns used drum kits. 
        /// If OSC Mode is Single/Drums        => use MS Bank/Number as Drum Kit (if MS Type == Drums), for OSC 1      , zone 1-8 (if used)
        /// If OSC Mode is Double/Double Drums => use MS Bank/Number as Drum Kit (if MS Type == Drums), for OSC 1 and 2, zone 1-8 (if used)
        /// </summary>
        public override List<IDrumKit> UsedDrumKits
        {
            get
            {
                var param = GetParam(ParameterNames.ProgramParameterName.OscMode).Value;

                // Only the first MS is used.
                var usedDrumKits = new List<IDrumKit>();
                if ((param == "Drums") || (param == "Double Drums"))
                {
                    usedDrumKits.Add(GetUsedDrumKit(0, 0));
                }

                if (param == "Double Drums")
                {
                    usedDrumKits.Add(GetUsedDrumKit(1, 0));
                }

                return usedDrumKits;
            }
        }


        /// <summary>
        /// Returns the drum kit used by osc (zero based) and MS zone (zero based).
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        protected IDrumKit GetUsedDrumKit(int osc, int zone)
        {
            var parameter = new IntParameter();
            parameter.SetMultiBytes(Root, Root.Content, GetZoneMsByteOffset(osc, zone) + 2, 2, // + 2: Number (bank unused, always 0?)
                false, false, null);
            int index = parameter.Value;

            IDrumKit drumKit = null;
            if (PcgRoot.DrumKitBanks != null)
            {
                drumKit = PcgRoot.DrumKitBanks.GetByIndex(index);
            }

            return drumKit;
        }


        /// <summary>
        /// Sets used drum kit.
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <param name="drumKit"></param>
        /// <returns></returns>
        private void SetUsedDrumKit(int osc, int zone, IDrumKit drumKit)
        {
            var parameter = new IntParameter();
            parameter.SetMultiBytes(Root, Root.Content, GetZoneMsByteOffset(osc, zone) + 2, 2, // + 2: Number (bank unused, alwyas 0?)
                false, false, null);

            var index = PcgRoot.DrumKitBanks.FindIndexOf(drumKit);
            if (index >= 0)
            {
                parameter.Value = index;
            }
        }


        /// <summary>
        /// Replace drumkits.
        /// </summary>
        /// <param name="changes"></param>
        public override void ReplaceDrumKit(Dictionary<IDrumKit, IDrumKit> changes)
        {
            var param = GetParam(ParameterNames.ProgramParameterName.OscMode).Value;

            // Only the first MS is used.
            if ((param == "Drums") || (param == "Double Drums"))
            {
                var usedDrumKit = GetUsedDrumKit(0, 0);

                foreach (var drumKit in changes.Keys.Where(drumKit => drumKit.Id == usedDrumKit.Id))
                {
                    SetUsedDrumKit(0, 0, changes[drumKit]);
                }
            }

            if (param == "Double Drums")
            {
                var usedDrumKit = GetUsedDrumKit(1, 0);
                foreach (var drumKit in changes.Keys.Where(drumKit => drumKit.Id == usedDrumKit.Id))
                {
                    SetUsedDrumKit(1, 0, changes[drumKit]);
                }
            }

            RaisePropertyChanged("ToolTip");
        }


        /// <summary>
        /// 
        /// </summary>
        protected enum EMode
        {
            Off,
            Sample,
            WaveSequence
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        protected abstract int GetZoneMsByteOffset(int osc, int zone);
    }
}
