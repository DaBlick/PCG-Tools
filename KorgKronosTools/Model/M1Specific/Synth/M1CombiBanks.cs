// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.M1Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class M1CombiBanks : MntxCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public M1CombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            //                          0 
            foreach (var id in new[] {"I", "C"})
            {
                Add(new M1CombiBank(this, BankType.EType.Int, id, -1));
            }
        }
    }
}
