// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.MntxSeriesSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MntxTimbre : Timbre
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <param name="timbresSizeConstant"></param>
        protected MntxTimbre(ITimbres timbres, int index, int timbresSizeConstant)
            : base(timbres, index, timbresSizeConstant)
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
                case ParameterNames.TimbreParameterName.Status:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 10, 4, 4, new List<string> { "On", "Off" }, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 5, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 6, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 7, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 8, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Volume: // But called Output Level
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 1, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.MidiChannel:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 10, 3, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Transpose: // Called Key Transpose
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 2, 7, 0, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Detune:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 3, 7, 0, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BendRange: // Called Detune
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 3, 7, 0, true, Parent as IPatch);
                    break;

            // No OSC Mode
            // No OSC Select
            // No portamento

            default:
                parameter = base.GetParam(name);
                break;
            }
           return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        public override string ColumnOscMode => string.Empty;


        /// <summary>
        /// 
        /// </summary>
        public override string ColumnOscSelect => string.Empty;


        /// <summary>
        /// 
        /// </summary>
        public override string ColumnPortamento => string.Empty;


        /// <summary>
        /// 
        /// </summary>
        public override string ColumnBendRange => string.Empty;
    }
}
