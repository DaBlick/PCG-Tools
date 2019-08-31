// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.TrinitySpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TrinityTimbre : Timbre
    {
        static int TimbresSizeConstant => 19;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public TrinityTimbre(Timbres timbres, int index)
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
                case ParameterNames.TimbreParameterName.Status: // Called Timbre Mode
                    parameter = EnumParameter.Instance.Set(
                     PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 2, 6, 5, 
                     new List<string> { "Int", "Off", "Both", "Ext" }, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 12, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomKey:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 13, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.TopVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 15, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BottomVelocity:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 16, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscMode:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 11, 6, 6, new List<string> { "Prg", "Force Poly" },
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.OscSelect:
                    parameter = EnumParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 11, 5, 5, new List<string> { "Both", "Hide OSC2" },
                        Parent as IPatch);
                    break;

                // No portamento.

                case ParameterNames.TimbreParameterName.Volume:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 3, 7, 0, false, 
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.MidiChannel:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 2, 4, 0, false,
                        Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Transpose:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 5, 7, 0, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BendRange:
                    parameter = IntParameter.Instance.Set(
                        PcgRoot, Combi.PcgRoot.Content, TimbresOffset + 4, 7, 0, true, Parent as IPatch);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            var memory = (IPcgMemory)Root;
            if (memory.AssignedClearProgram == null)
            {
                UsedProgram = (IProgram)(PcgRoot.ProgramBanks[0][0]);
            }
            else
            {
                UsedProgram = memory.AssignedClearProgram;
            }

            GetParam(ParameterNames.TimbreParameterName.Status).Value = "Off";
            if (GetParam(ParameterNames.TimbreParameterName.Mute) != null)
            {
                GetParam(ParameterNames.TimbreParameterName.Mute).Value = true;
            }
            GetParam(ParameterNames.TimbreParameterName.Volume).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = 15;

            GetParam(ParameterNames.TimbreParameterName.BottomKey).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.TopKey).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.OscMode).Value = "Force Poly";
            GetParam(ParameterNames.TimbreParameterName.OscSelect).Value = "Hide OSC2";

            GetParam(ParameterNames.TimbreParameterName.Transpose).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.BendRange).Value = 0;

            RefillColumns();
        }
    }
}
