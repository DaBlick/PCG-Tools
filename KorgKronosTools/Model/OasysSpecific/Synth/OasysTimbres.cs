// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class OasysTimbres : Timbres
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresPerCombiConstant => 16;


        /// <summary>
        /// 
        /// </summary>
        static int TimbresOffsetConstant => 4790;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public OasysTimbres(ICombi combi)
            : base(combi, TimbresPerCombiConstant, TimbresOffsetConstant)
        {
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new OasysTimbre(this, n));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override ITimbre CreateNewTimbre(int index)
        {
            return new OasysTimbre(this, index);
        }
    }
}
