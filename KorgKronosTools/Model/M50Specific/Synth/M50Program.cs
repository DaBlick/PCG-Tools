// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using System.Collections.Generic;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M50Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M50Program : MProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public M50Program(IProgramBank programBank, int index)
            : base(programBank, index) 
        {
        }


        /// <summary>
        /// Sets parameters after initialization.
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
                    parameter = EnumParameter.Instance.Set(Root, Root.Content, ByteOffset + 836, 2, 0,
                        new List<string> {"Single", "Double", "Drums"}, this);
                    break;

                case ParameterNames.ProgramParameterName.Category:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 846, 4, 0, false, this);
                    break;

                case ParameterNames.ProgramParameterName.SubCategory:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 846, 7, 5, false, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackCommonPatternNumber:
                    parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 636, false, 1, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackCommonPatternBank:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 638, 1, 0, false, this);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }


        /// <summary>
        /// Do not take drum track program references into account.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override bool UseIndexForDifferencing(int index)
        {
            return ((index != 830) && (index != 831));
        }
    }
}
