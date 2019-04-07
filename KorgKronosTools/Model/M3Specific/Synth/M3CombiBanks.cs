// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M3Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M3CombiBanks : MCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M3CombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            foreach (var id in new[] {"I-A", "I-B", "I-C", "I-D", "I-E", "I-F", "I-G"})
            {
                Add(new M3CombiBank(this, BankType.EType.Int, id, -1));
            }

            foreach (var id in new[] {"U-A", "U-B", "U-C", "U-D", "U-E", "U-F", "U-G"})
            {
                Add(new M3CombiBank(this, BankType.EType.User, id, -1));
            }
        }
    }
}
