// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonLeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TritonLeTimbres : TritonTimbres
    {
        /// <summary>
        /// Triton non LE is 224, a Triton LE combi is 96 shorter than a Triton non LE combi.
        /// </summary>
        static int TimbresOffsetConstant => 224 - 96;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public TritonLeTimbres(ICombi combi)
            : base(combi, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new TritonLeTimbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new TritonLeTimbre(this, index);
        }

    }
}
