// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.MSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MTimbres : Timbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresPerCombiConstant => 16;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <param name="timbresOffsetConstant"></param>
        protected MTimbres(ICombi combi, int timbresOffsetConstant)
            : base(combi, TimbresPerCombiConstant, timbresOffsetConstant)
        {
        }
    }
}
