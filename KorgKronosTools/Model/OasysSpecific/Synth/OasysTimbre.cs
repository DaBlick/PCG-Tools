// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysTimbre : KronosOasysTimbre
    {
        /// <summary>
        /// 
        /// </summary>
        static int TimbresSizeConstant => 186;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public OasysTimbre(ITimbres timbres, int index)
            : base(timbres, index, TimbresSizeConstant)
        {
        }


        /// <summary>
        /// 
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
    }
}
