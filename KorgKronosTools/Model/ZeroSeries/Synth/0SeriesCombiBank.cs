// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.ZeroSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesCombiBank : MntxCombiBank
    {
        /// <summary>
        /// 
        /// </summary>
        public override int NrOfPatches => 100;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public ZeroSeriesCombiBank(IBanks combiBanks, BankType.EType type, string id, int pcgId)
            : base(combiBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new ZeroSeriesCombi(this, index));
        }
    }
}
