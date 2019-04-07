// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosGmProgramBank : ProgramBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        /// <param name="synthesisType"></param>
        /// <param name="description"></param>
        public KronosGmProgramBank(IProgramBanks programBanks, BankType.EType type, string id, int pcgId, 
            SynthesisType synthesisType, string description)
            : base(programBanks, type, id, pcgId, synthesisType, description)
        {
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new KronosGmProgram(this, index, "GM" + (index + 1))); //FUTURE: Real name instead of GMn, use GmPrograms.cs
        }


        /// <summary>
        /// 
        /// </summary>
        public override SynthesisType DefaultModeledSynthesisType
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public override SynthesisType DefaultSampledSynthesisType => SynthesisType.Hd1;


        /// <summary>
        /// E.g. GM banks have index 1.
        /// </summary>
        public override int IndexOffset => 1;
    }
}
