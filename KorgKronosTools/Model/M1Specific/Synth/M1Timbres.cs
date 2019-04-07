// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class M1Timbres : MntxTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresOffsetConstant => 36;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public M1Timbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new M1Timbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new M1Timbre(this, index);
        }

    }
}
