// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesTimbre : MntxTimbre
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
        public ZeroSeriesTimbre(ITimbres timbres, int index)
            : base(timbres, index, TimbresSizeConstant)
        {
        }


        /// <summary>
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
        /// The program No is based on:
        /// If Combination type is     MULTI: 00=Timbre off, 01H-64H=I00..I99, 65H..C8H=C00..C99
        /// If combination type is not MULTI:                00..63H=I00..I99, 64H..C8H=C00..C99
        /// C(ard) is pretended to be bank 1. Virtual banks should start at 2.
        /// Note: there is no byte for a bank ID (it is part of the program No, so virtual banks are not supportable.
        /// </summary>
        protected override int UsedProgramBankId
        {
            get
            {
                // Programs can only be from bank C..D (combis C/D point to programs C/D only).
                var value = Combi.PcgRoot.Content[TimbresOffset]; // Program No.
                return (value <= 100) ? 2 : 3; // 0-100: Bank C, 64..C7: Bank D
            }
        }


        /// <summary>
        /// The program No is 00..99: 
        /// </summary>
        protected override int UsedProgramId => Combi.PcgRoot.Content[TimbresOffset] % 100;
    }
}
