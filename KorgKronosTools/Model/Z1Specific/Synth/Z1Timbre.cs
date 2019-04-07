// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Z1Timbre : MntxTimbre
    {
        /// <summary>
        /// 
        /// </summary>
        private static int TimbresSizeConstant => 16;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public Z1Timbre(ITimbres timbres, int index)
            : base(timbres, index, TimbresSizeConstant)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.TimbreParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.TimbreParameterName.Status: // Voice Reserve Total, 0 = OFF, 1~18 = reserve voice value (5 bytes), treat all as Int
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 1, 4, 0, new List<string>
                    {
                        "Off", "Int", "Int", "Int", "Int", "Int", "Int", "Int", "Int", // 1~8
                        "Int", "Int", "Int", "Int", "Int", "Int", "Int", "Int", "Int", "Int" // 9~1~2
                    }, Parent as IPatch);
                    break;
                    
                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 8, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 9, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 10, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 11, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Volume: // Output level
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 5, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.MidiChannel:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 12, 4, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Transpose:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 2, 7, 0, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BendRange: // Detune
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 3, 7, 0, true, Parent as IPatch);
                    break;

                default:
                    return base.GetParam(name);
            }
            return parameter;
        }


        /// <summary>
        /// </summary>
        protected override int UsedProgramBankId => Util.GetBits(Combi.PcgRoot.Content, TimbresOffset, 7, 7);


        /// <summary>
        /// </summary>
        protected override int UsedProgramId => Util.GetBits(Combi.PcgRoot.Content, TimbresOffset, 6, 0);
    }
}
