// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M3Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class M3Timbres : MTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresOffsetConstant => 5700;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public M3Timbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new M3Timbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new M3Timbre(this, index);
        }

    }
}
