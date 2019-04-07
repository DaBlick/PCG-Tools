// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonKarmaSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaCombiBank : TritonCombiBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public TritonKarmaCombiBank(ICombiBanks combiBanks, BankType.EType type, string id, int pcgId)
            : base(combiBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new TritonKarmaCombi(this, index));
        }
    }
}
