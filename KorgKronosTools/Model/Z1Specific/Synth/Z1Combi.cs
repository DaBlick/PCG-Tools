// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Z1Combi : MntxCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public Z1Combi(IBank combiBank, int index)
            : base(combiBank, index)
        {
            Id = $"{combiBank.Id}{(index).ToString("00")}";
            Timbres = new Z1Timbres(this);
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

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }
    }
}
