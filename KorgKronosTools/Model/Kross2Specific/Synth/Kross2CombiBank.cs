// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved


using System;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.Kross2Specific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class Kross2CombiBank : MCombiBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public Kross2CombiBank(ICombiBanks combiBanks, BankType.EType type, string id, int pcgId)
            : base(combiBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new Kross2Combi(this, index));
        }

    }
}