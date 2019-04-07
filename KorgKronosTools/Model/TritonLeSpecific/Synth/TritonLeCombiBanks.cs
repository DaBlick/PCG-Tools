// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonLeSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonLeCombiBanks : TritonCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonLeCombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0    1    2 
            foreach (var id in new[] { "A", "B", "C" })
            {
                Add(new TritonLeCombiBank(this, BankType.EType.Int, id, -1));
            }
        }
    }
}
