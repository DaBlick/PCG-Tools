// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.ZeroSeries.Synth;

namespace PcgTools.Model.Zero3Rw.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Zero3RwTimbres : ZeroSeriesTimbres
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        public Zero3RwTimbres(ICombi combi)
            : base(combi)
        {
            TimbresCollection.Clear(); //IMPR: is clear really necessary here?
            for (var n = 0; n < TimbresPerCombi; n++)
            {
                TimbresCollection.Add(new Zero3RwTimbre(this, n));
            }
        }

    }
}
