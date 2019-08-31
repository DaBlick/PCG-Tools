// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonKarmaSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonKarmaCombiBanks : TritonCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonKarmaCombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0    1    2    3    4    5    6  
            foreach (var id in new[] { "A", "B", "C", "D", "E", "F", "G" })
            {
                Add(new TritonKarmaCombiBank(this, BankType.EType.Int, id, -1));
            }

            //                          7    8    9   10   11   12   13
            foreach (var id in new[] { "H", "I", "J", "K", "L", "M", "N" })
            {
                Add(new TritonKarmaCombiBank(this, BankType.EType.User, id, -1));
            }
        }
    }
}
