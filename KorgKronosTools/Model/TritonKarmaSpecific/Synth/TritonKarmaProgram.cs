// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using System.Collections.Generic;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonKarmaSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaProgram : TritonProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public TritonKarmaProgram(IProgramBank programBank, int index)
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
                    parameter = EnumParameter.Instance.Set(Root, Root.Content, ByteOffset + 468, 2, 0,
                        new List<string> { "Single", "Double", "Drums" }, this);
                    break;

                case ParameterNames.ProgramParameterName.Category:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 470, 3, 0, false, this);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }
    }
}
