// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;
using PcgTools.PcgToolsResources;

namespace PcgTools.Model.TSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class TSeriesTimbre : MntxTimbre
    {
        /// <summary>
        /// 
        /// </summary>
        private static int TimbresSizeConstant => 11;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public TSeriesTimbre(ITimbres timbres, int index)
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
                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }


        /// <summary>
        /// According information of KorgForums/T3owner, the official Korg documentation of the T3 is incorrect for this.
        /// It seems the algorithm is very simple:
        /// - 00..63 : A00..B99
        /// - 64..127: B00..B99
        /// There is a specific bit if a timbre is on or off.
        /// However, when the timbre is off, sometimes the program is A00, sometimes B00.
        /// </summary>
        protected override int UsedProgramBankId => (Combi.PcgRoot.Content[TimbresOffset] <= 0x63) ? 0 : 1;


        /// <summary>
        /// According information of KorgForums/T3owner, the official Korg documentation of the T3 is incorrect for this.
        /// It seems the algorithm is very simple:
        /// - 00..63 : A00..B99
        /// - 64..127: B00..B99
        /// There is a specific bit if a timbre is on or off.
        /// </summary>
        protected override int UsedProgramId => Combi.PcgRoot.Content[TimbresOffset] % 100;


        /// <summary>
        /// If program not used, show Timbre OFF.
        /// </summary>
        public override string ColumnProgramId => GetParam(ParameterNames.TimbreParameterName.Status).Value == "Off" ? Strings.TimbreOff : UsedProgram.Id;


        /// <summary>
        /// If program not used, show Timbre OFF.
        /// </summary>
        public override string ColumnProgramName => GetParam(ParameterNames.TimbreParameterName.Status).Value == "Off" ? Strings.TimbreOff : UsedProgram.Name;
    }
}
