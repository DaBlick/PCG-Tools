// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.TritonSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TritonTimbre : Timbre
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresSizeConstant => 28;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        protected TritonTimbre(ITimbres timbres, int index)
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
                case ParameterNames.TimbreParameterName.Status:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 2, 7, 5,
                        new List<string> {"Int", "Off", "Both", "Ext", "Ex2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 21, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 22, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 24, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 25, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 19, 1, 0,
                        new List<string> {"Prg", "Poly", "Mono", "Legato"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscSelect:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 19, 3, 2,
                        new List<string> {"Both", "Osc1", "Osc2"}, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Portamento:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 20, 7, 0, true, Parent as IPatch);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }
    }
}
