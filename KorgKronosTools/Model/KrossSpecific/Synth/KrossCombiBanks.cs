// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossCombiBanks : MCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KrossCombiBanks(PcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0    1   
            foreach (var id in new[] {"A", "B"})
            {
                Add(new KrossCombiBank(this, BankType.EType.Int, id, -1));
            }

            Add(new KrossCombiBank(this, BankType.EType.User, "U", -1));
        }
    }
}
