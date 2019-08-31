// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KromeExSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KromeExCombi : MCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public KromeExCombi(CombiBank combiBank, int index)
            : base(combiBank, index)
        {
            Timbres = new KromeExTimbres(this);
        }


        ///
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
                parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 824, 4, 0, false, this);
                break;

            case ParameterNames.CombiParameterName.SubCategory:
                parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 824, 7, 5, false, this);
                break;

            case ParameterNames.CombiParameterName.Tempo:
                parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 792, false, 100, this);
                break;

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }
    }
}
