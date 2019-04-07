// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.MSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MTimbre : Timbre
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <param name="timbresSizeConstant"></param>
        protected MTimbre(ITimbres timbres, int index, int timbresSizeConstant)
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
                        Root, Root.Content, TimbresOffset + 2, 7, 5,
                        new List<string> {"Off", "Int", "Ext", "Ex2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Mute:
                    parameter = BoolParameter.Instance.Set(Root, Root.Content, TimbresOffset + 27, 7,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 30, 7, 0, false,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 31, 7, 0, false,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 33, 7, 0, false,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 34, 7, 0, false,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 28, 1, 0,
                        new List<string> {"Prg", "Poly", "Mono", "Legato"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscSelect:
                    parameter = EnumParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 28, 3, 2,
                        new List<string> {"Both", "Osc1", "Osc2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Portamento:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 29, 7, 0, true,
                        Parent as IPatch);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }
    }
}
