// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.KronosOasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class KronosOasysTimbres : Timbres
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <param name="timbresPerCombiConstant"></param>
        /// <param name="timbresOffsetConstant"></param>
        protected KronosOasysTimbres(Combi combi, int timbresPerCombiConstant, int timbresOffsetConstant)
            : base(combi, timbresPerCombiConstant, timbresOffsetConstant)
        {
        }
    }
}
