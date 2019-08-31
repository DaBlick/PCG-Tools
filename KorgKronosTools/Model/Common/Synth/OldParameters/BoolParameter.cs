// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.Synth.OldParameters
{
    /// <summary>
    /// 
    /// </summary>
    public class BoolParameter: Parameter
    {
        /// <summary>
        /// 
        /// </summary>
        int _bit;


        /// <summary>
        /// 
        /// </summary>
        static BoolParameter _instance;


        /// <summary>
        /// 
        /// </summary>
        public static BoolParameter Instance => _instance ?? (_instance = new BoolParameter());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="pcgData"></param>
        /// <param name="pcgOffset"></param>
        /// <param name="bit"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        public BoolParameter Set(IMemory memory, byte[] pcgData, int pcgOffset, int bit, IPatch patch)
        {
            Set(memory, pcgData, pcgOffset, patch);
            _bit = bit;
            
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override dynamic Value
        {
            get
            {
                return BitsUtil.GetBit(PcgData, PcgOffset, _bit);
            }

            set
            {
                Debug.Assert(PcgData != null);
                PcgMemory.IsDirty |= BitsUtil.SetBit(PcgData, PcgOffset, _bit, value);
                if (Patch != null)
                {
                    Patch.RaisePropertyChanged(string.Empty, false);
                }
            }
        }
    }
}