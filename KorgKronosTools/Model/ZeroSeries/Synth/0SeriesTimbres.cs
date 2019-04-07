// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesTimbres : MntxTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        private static int TimbresOffsetConstant => 40;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public ZeroSeriesTimbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new ZeroSeriesTimbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new ZeroSeriesTimbre(this, index);
        }

    }
}
