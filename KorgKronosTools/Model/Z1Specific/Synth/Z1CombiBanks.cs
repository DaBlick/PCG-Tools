// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Z1CombiBanks : MntxCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public Z1CombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0    1 
            foreach (var id in new[] { "A", "B" })
            {
                Add(new Z1CombiBank(this, BankType.EType.Int, id, -1));
            }
        }
    }
}
