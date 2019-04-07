// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.ZeroSeries.Synth;

namespace PcgTools.Model.Zero3Rw.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Zero3RwTimbre : ZeroSeriesTimbre
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        public Zero3RwTimbre(Timbres timbres, int index)
            : base(timbres, index)
        {
        }


        /// <summary>
        /// The program No is based on:
        /// 00~63->Bank A, C, D, using bank A only.
        /// 64~E4->Bank GM
        /// </summary>
        protected override int UsedProgramBankId
        {
            get
            {
                var value = Combi.PcgRoot.Content[TimbresOffset];
                return (value < 100) ? 0 : 3; // 2 = Bank A, 3 = GM Bank
            }
        }


        /// <summary>
        /// The program No is based on:
        /// 00~63->Bank A, C, D
        /// 64~E4->Bank GM
        /// </summary>
        protected override int UsedProgramId
        {
            get 
            {
                var id = Combi.PcgRoot.Content[TimbresOffset];
                return (id < 100) ? id : id - 100; // -100 if GM
            }
        }
    }
}
