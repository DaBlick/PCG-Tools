// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public class FixedParameter : Parameter, IFixedParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public enum EType
        {
            Genre, // Only for MicroKorg XL
            Category, // Only for MicroKorg XL, MS2000
            Mode
        }


        /// <summary>
        /// 
        /// </summary>
        private EType Type { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private static FixedParameter _instance;


        /// <summary>
        /// 
        /// </summary>
        public static FixedParameter Instance => _instance ?? (_instance = new FixedParameter());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="patch"></param>
        public void Set(IMemory memory, byte[] data, EType type, IPatch patch)
        {
            base.Set(memory, data, 0, patch);
            Type = type;
        }


        /// <summary>
        /// 
        /// </summary>
        public override dynamic Value
        {
            get { return ((IProgram)Patch).GetFixedParameterValue(Type); }

            set { ((IProgram)Patch).SetFixedParameterValue(Type, value); }
        }
    }
}