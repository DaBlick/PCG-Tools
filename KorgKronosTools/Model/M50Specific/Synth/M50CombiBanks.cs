// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.M50Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M50CombiBanks : MCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M50CombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0    1    2    3   
            foreach (var id in new[] { "A", "B", "C", "D" })
            {
                Add(new M50CombiBank(this, BankType.EType.Int, id, -1));
            }
        }
    }
}
