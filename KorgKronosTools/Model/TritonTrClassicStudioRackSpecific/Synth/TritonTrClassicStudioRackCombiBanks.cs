// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.TritonSpecific.Synth;

namespace PcgTools.Model.TritonTrClassicStudioRackSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TritonTrClassicStudioRackCombiBanks : TritonCombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TritonTrClassicStudioRackCombiBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            // 5 and 6 are not used, studio actually is called INT-A..INT-E
            //                          0    1    2    3    4   5    6     
            foreach (var id in new[] { "A", "B", "C", "D", "E", "F", "G"})
            {
                Add(new TritonTrClassicStudioRackCombiBank(this, BankType.EType.Int, id, -1));
            }

            // 13 Only for Studio/Rack, 14 Only for Rack
            //                          7        8        9        10       11       12      13        14
            foreach (var id in new[] { "EXB-A", "EXB-B", "EXB-C", "EXB-D", "EXB-E", "EXB-F", "EXB-G",  "EXB-H" })
            {
                Add(new TritonTrClassicStudioRackCombiBank(this, BankType.EType.User, id, -1));
            }
        }
    }
}
