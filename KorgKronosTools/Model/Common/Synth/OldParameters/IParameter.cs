// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// 
        /// </summary>
        dynamic Value { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IPatch Patch { get; }
    }
}
