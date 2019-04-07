// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.OasysSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class OasysCombi : KronosOasysCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public OasysCombi(ICombiBank combiBank, int index)
            : base(combiBank, index)
        {
            Timbres = new OasysTimbres(this);
        }
        

        /// <summary>
        /// Sets parameters after initialization.
        /// </summary>
        public override void SetParameters()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.CombiParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.CombiParameterName.Category:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 4778, 4, 0, false, this);
                    break;

                case ParameterNames.CombiParameterName.SubCategory:
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 4778, 7, 5, false, this);
                    break;

                case ParameterNames.CombiParameterName.Tempo:
                    parameter = WordParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 1292, false, 100, this);
                    break;

                default:
                    parameter = base.GetParam(name);
                    break;
            }
            return parameter;
        }
    }
}
