// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M3rSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class M3RTimbre : MntxTimbre
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
        public M3RTimbre(ITimbres timbres, int index)
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
        /// If Combination type is MULTI (always for M3r): 00=Timbre off, 01H-64H=I00..I99, 65H..C8H=C00..C99
        /// Note: there is no byte for a bank ID (it is part of the program No, so virtual banks are not supportable.
        /// </summary>
        protected override int UsedProgramBankId => (Combi.PcgRoot.Content[TimbresOffset] <= 0x64) ? 0 : 1;


        /// <summary>
        /// The program No is based on:
        /// Minimum 0, because it should be callable even when the program is not used.
        /// </summary>
        protected override int UsedProgramId => Math.Max(0, Combi.PcgRoot.Content[TimbresOffset] - 1);
    }
}
