// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonExtremeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TritonExtremeTimbres : TritonTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresOffsetConstant => 224;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public TritonExtremeTimbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new TritonExtremeTimbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new TritonExtremeTimbre(this, index);
        }

    }
}
