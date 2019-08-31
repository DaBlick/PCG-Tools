// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosCombiBank : KronosOasysCombiBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        public KronosCombiBank(ICombiBanks combiBanks,  BankType.EType type, string id, int pcgId)
            : base(combiBanks, type, id, pcgId)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new KronosCombi(this, index));
        }


        /// <summary>
        /// 
        /// </summary>
        public int Cbk2PcgOffset { get; set; }


        /// <summary>
        /// Returns the offset of a specific combi index/parameter index.
        /// Param 0: Bank Combi 0, tim 1, Combi 0, tim 2, .... Combi 127, tim 16.
        /// Param 1: Prg:  Combi 0, tim 1, Combi 0, tim 2, .... Combi 127, tim 16.
        /// </summary>
        /// <param name="combiIndex"></param>
        /// <param name="timbreIndex"></param>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        public int GetParameterOffsetInCbk2(int combiIndex, int timbreIndex, int parameterIndex)
        {
            Debug.Assert((parameterIndex >= 0) && (parameterIndex < KronosCombiBanks.ParametersInCbk2Chunk * KronosTimbres.TimbresPerCombiConstant));
            Debug.Assert((combiIndex >= 0) && (combiIndex <= CountPatches));

            return Cbk2PcgOffset + parameterIndex * CountPatches * KronosTimbres.TimbresPerCombiConstant +  // = 0 if parameterIndex = 0, else start of parameter 1
                    combiIndex * KronosTimbres.TimbresPerCombiConstant + timbreIndex;
        }
    }
}
