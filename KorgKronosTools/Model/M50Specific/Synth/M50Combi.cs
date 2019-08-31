// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M50Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M50Combi : MCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public M50Combi(ICombiBank combiBank, int index)
            : base(combiBank, index)
        {
            Timbres = new M50Timbres(this);
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
                    parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 824, 4, 0, false, this);
                break;

            case ParameterNames.CombiParameterName.SubCategory:
                parameter = IntParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 824, 7, 5, false, this);
                break;

            case ParameterNames.CombiParameterName.Tempo:
                parameter = WordParameter.Instance.Set(PcgRoot, PcgRoot.Content, ByteOffset + 792, false, 100, this);
                break;

            case ParameterNames.CombiParameterName.DrumTrackCommonPatternNumber:
                parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 636, false, 1, this);
                break;

            case ParameterNames.CombiParameterName.DrumTrackCommonPatternBank:
                parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 638, 1, 0, false, this);
                break;

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }
    }
}
