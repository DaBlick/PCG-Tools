// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using System;
using System.Collections.Generic;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysProgram : KronosOasysProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public OasysProgram(IProgramBank programBank, int index)
            : base(programBank, index)
        {
        }

        
        /// <summary>
        /// Sets parameters after initialization.
        /// IMPR: Change GetParam.
        /// </summary>
        public override void SetParameters()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.ProgramParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.ProgramParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(Root, Root.Content, ByteOffset + 2546, 1, 0,
                        new List<string> { "Single", "Double", "Drums", "- (EXI)", "- (Unused)", "Double Drums" }, this);
                    break;

                case ParameterNames.ProgramParameterName.Category:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 2556, 4, 0, false, this);
                    break;

                case ParameterNames.ProgramParameterName.SubCategory:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 2556, 7, 5, false, this);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }


        /// <summary>
        /// Zone 0 = High MS, Zone 1 = Mid High MS.
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        protected override int GetZoneMsByteOffset(int osc, int zone)
        {
            return ByteOffset + 2732 + osc * (3032 - 2732) + zone * (2738 - 2732);
        }


        /// <summary>
        /// Number of zones.
        /// </summary>
        protected override int NumberOfZones => 4;
    }
}
