// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosProgramBank : KronosOasysProgramBank
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBanks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        /// <param name="synthesisType"></param>
        /// <param name="description"></param>
        public KronosProgramBank(IProgramBanks programBanks, BankType.EType type, string id, int pcgId,
            SynthesisType synthesisType, string description)
            : base(programBanks, type, id, pcgId, synthesisType, description)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void CreatePatch(int index)
        {
            Add(new KronosProgram(this, index));
        }


        /// <summary>
        /// Kronos only; in this chunk there are 66 parameters of 128 programs in order: (prg0, par0), (prg1, par0) to (prg126, par 65), (prg127, par65)
        /// </summary>
        public int Pbk2PcgOffset { get; set; }


        /// <summary>
        /// Returns the offset of a specific program index/parameter index.
        /// Order is:
        /// for each program:
        ///    for parameter 0-31:
        ///       1 byte
        /// for each program:
        ///    for parameter 32-63:
        ///       1 byte
        /// parameter 64:
        ///    for each program
        ///       1 byte
        /// parameter 65:
        ///    for each program
        ///       1 byte
        /// </summary>
        /// <param name="programIndex"></param>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        public int GetParameterOffsetInPbk2(int programIndex, int parameterIndex)
        {
            Debug.Assert((parameterIndex >= 0) && (parameterIndex < KronosProgramBanks.ParametersInPbk2Chunk));
            Debug.Assert((programIndex >= 0) && (programIndex <= CountPatches));

            var offset = Pbk2PcgOffset;
            if (parameterIndex < 32)
            {
                 offset += 32 * programIndex + parameterIndex;
            }
            else if (parameterIndex < 64)
            {
                offset += 32 * (CountPatches + programIndex) + parameterIndex - 32;
            }
            else if (parameterIndex == 64)
            {
                offset += 32 * (2 * CountPatches) + programIndex;
            }
            else if (parameterIndex == 65)
            {
                offset += 32 * (2 * CountPatches) + CountPatches + programIndex; // two times 32 parameters + par 64 space + par 65 offset
            }
            return offset;
        }


        /// <summary>
        /// 
        /// </summary>
        public override SynthesisType DefaultModeledSynthesisType => SynthesisType.Exi;


        /// <summary>
        /// 
        /// </summary>
        public override SynthesisType DefaultSampledSynthesisType => SynthesisType.Hd1;
    }
}
