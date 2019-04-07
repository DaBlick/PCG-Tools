// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.MicroStationSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MicroStationTimbres : MTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresOffsetConstant => 664;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public MicroStationTimbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new MicroStationTimbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new MicroStationTimbre(this, index);
        }

    }
}
