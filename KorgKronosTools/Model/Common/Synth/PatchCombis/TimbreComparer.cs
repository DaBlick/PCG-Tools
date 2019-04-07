// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using PcgTools.Model.Common.Synth.OldParameters;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// Class for comparing on status.
    /// </summary>
    internal sealed class TimbreComparer : Comparer<Timbre>
    {
        /// <summary>
        /// 
        /// </summary>
        private TimbreSorting.ESortKey SortKey { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public TimbreComparer(TimbreSorting.ESortKey key)
        {
            SortKey = key;
        }


        /// <summary>
        /// [IMPR] Split into different comparers and add singleton pattern (like in PatchSorting).
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public override int Compare(Timbre p1, Timbre p2)
        {
            int order;
            switch (SortKey)
            {
                case TimbreSorting.ESortKey.ESortKeyStatus:
                    order = SortStatus(p1, p2);
                    break;

                case TimbreSorting.ESortKey.ESortKeyMute:
                    order = SortMute(p1, p2);
                    break;

                case TimbreSorting.ESortKey.ESortKeyMidiChannel:
                    order = SortMidiChannel(p1, p2);

                    break;

                case TimbreSorting.ESortKey.ESortKeyKeyVelocity:
                    order = SortVelocity(p1, p2);
                    break;

                case TimbreSorting.ESortKey.ESortKeyKeyKeyZone:
                    order = SortKeyZones(p1, p2);
                    break;

                default:
                    throw new ApplicationException("Illegal sort key");
            }

            return order;
        }


        /// <summary>
        ///SortMethod order of status is: Int, Both, Off, Ext, Ex2
        /// Rationale: move Int and Both to the left, move Ext and Ex2 to the right, keep unused timbres in between.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int SortStatus(Timbre p1, Timbre p2)
        {
            int order;
            {
                var values = new List<string> {"Int", "On", "Both", "Off", "Ext", "Ex2"};

                var p1Status = p1.GetParam(ParameterNames.TimbreParameterName.Status).Value;
                var p1Value = values.IndexOf(p1Status);

                var p2Status = p2.GetParam(ParameterNames.TimbreParameterName.Status).Value;
                var p2Value = values.IndexOf(p2Status);

                order = (p1Value < p2Value) ? -1 : p1Value > p2Value ? 1 : 0;
            }
            return order;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int SortMute(Timbre p1, Timbre p2)
        {
            int order;
            {
                var p1Mute = p1.GetParam(ParameterNames.TimbreParameterName.Mute);
                var p1Value = (p1Mute == null) ? false : p1Mute.Value; // If not existing, it is not muted

                var p2Mute = p2.GetParam(ParameterNames.TimbreParameterName.Mute);
                var p2Value = (p2Mute == null) ? false : p2Mute.Value; // If not existing, it is not muted

                order = p1Value ? (p2Value ? 0 : 1) : (p2Value ? -1 : 0); // Unmuted first
            }
            return order;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int SortMidiChannel(Timbre p1, Timbre p2)
        {
            int order;
            if (p1.HasMidiChannelGch)
            {
                if (p2.HasMidiChannelGch)
                {
                    order = 0;
                }
                else
                {
                    order = -1;
                }
            }
            else
            {
                order = p2.HasMidiChannelGch ? 1 :
                    p1.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value.CompareTo(
                    p2.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value);
            }
            return order;
        }


        /// <summary>
        /// Bottom first, top last.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int SortVelocity(Timbre p1, Timbre p2)
        {
            int order = p1.GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value.CompareTo(
                p2.GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value);

            if (order == 0)
            {
                order = p1.GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value.CompareTo(
                    p2.GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value);
            }
            return order;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static int SortKeyZones(Timbre p1, Timbre p2)
        {
            int order = p1.GetParam(ParameterNames.TimbreParameterName.BottomKey).Value.CompareTo(
                p2.GetParam(ParameterNames.TimbreParameterName.BottomKey).Value);

            if (order == 0)
            {
                order = p1.GetParam(ParameterNames.TimbreParameterName.TopKey).Value.CompareTo(
                    p2.GetParam(ParameterNames.TimbreParameterName.TopKey).Value);
            }
            return order;
        }
    }
}
