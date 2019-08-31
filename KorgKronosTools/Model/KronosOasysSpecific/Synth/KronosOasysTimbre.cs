// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysTimbre : Timbre
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <param name="timbresSizeConstant"></param>
        protected KronosOasysTimbre(ITimbres timbres, int index, int timbresSizeConstant)
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
                        new List<string> {"Off", "Int", "Both", "Ext", "Ex2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Mute:
                    parameter = BoolParameter.Instance.Set(Root, Root.Content, TimbresOffset + 34, 7,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Priority:
                    parameter = BoolParameter.Instance.Set(Root, Root.Content, TimbresOffset + 35, 4,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 37, 7, 0,
                        false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 38, 7, 0,
                        false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 40, 7, 0,
                        false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, TimbresOffset + 41, 7, 0,
                        false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 35, 1, 0,
                        new List<string> {"Prg", "Poly", "Mono", "Legato"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscSelect:
                    parameter = EnumParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 35, 3, 2,
                        new List<string> {"Both", "Osc1", "Osc2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Portamento:
                    parameter = IntParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 36, 7, 0, true, Parent as IPatch);
                    break;

                default:
                    return base.GetParam(name);
            }
            return parameter;
        }
    }
}
