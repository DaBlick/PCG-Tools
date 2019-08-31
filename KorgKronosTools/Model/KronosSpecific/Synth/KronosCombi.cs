// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Diagnostics;
using PcgTools.ClipBoard;
using PcgTools.Model.Common;

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosCombi : KronosOasysCombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        public KronosCombi(CombiBank combiBank, int index)
            : base(combiBank, index)
        {
            Timbres = new KronosTimbres(this);
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            if (PcgRoot.AreFavoritesSupported)
            {
                GetParam(ParameterNames.CombiParameterName.Favorite).Value = false;
            }

            RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// Sets parameters after initialization.
        /// </summary>
        public override void SetParameters()
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override IParameter GetParam(ParameterNames.CombiParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
            case ParameterNames.CombiParameterName.Category:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 4790, 4, 0, false, this);
                break;

            case ParameterNames.CombiParameterName.SubCategory:
                parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 4790, 7, 5, false, this);
                break;

            case ParameterNames.CombiParameterName.Favorite:
                parameter = BoolParameter.Instance.Set(Root, Root.Content, ByteOffset + 4791, 0, this);
                break;

            case ParameterNames.CombiParameterName.Tempo:
                parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 1304, false, 100, this);
                break;

            case ParameterNames.CombiParameterName.DrumTrackCommonPatternNumber:
                parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 1292, true, 1, this);
                break;

            case ParameterNames.CombiParameterName.DrumTrackCommonPatternBank:
                parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 1294, 1, 0, false, this);
                break;

            default:
                parameter = base.GetParam(name);
                break;
            }
            return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherPatch"></param>
        /// <param name="includingName"></param>
        /// <param name="maxDiffs"></param>
        /// <returns></returns>
        public override int CalcByteDifferences(IPatch otherPatch, bool includingName, int maxDiffs)
        {
            var diffs = base.CalcByteDifferences(otherPatch, includingName, maxDiffs);

            // Take CBK2 differences into account.
            if (((KronosCombiBank)(Parent)).Cbk2PcgOffset != 0)
            {
                for (var parameterIndex = 0; parameterIndex < KronosCombiBanks.ParametersInCbk2Chunk; parameterIndex++)
                {
                    for (var timbre = 0; timbre < Timbres.TimbresCollection.Count; timbre++)
                    {
                        var patchIndex = ((KronosCombiBank) Parent).GetParameterOffsetInCbk2(Index, timbre, parameterIndex);
                        var otherPatchIndex = ((KronosCombiBank) otherPatch.Parent).GetParameterOffsetInCbk2(Index, timbre, parameterIndex);

                        diffs += (Util.GetInt(PcgRoot.Content, patchIndex, 1) != Util.GetInt(
                            otherPatch.PcgRoot.Content, otherPatchIndex, 1)) ? 1 : 0;
                    }
                }
            }
            return diffs;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherPatch"></param>
        /// <param name="includingName"></param>
        /// <param name="maxDiffs"></param>
        /// <returns></returns>
        public override int CalcByteDifferences(IClipBoardPatch otherPatch, bool includingName, int maxDiffs)
        {
            var otherCombi = otherPatch as ClipBoardCombi;
            Debug.Assert(otherCombi != null);

            var diffs = base.CalcByteDifferences(otherPatch, includingName, maxDiffs);

            // Take CBK2 differences into account.
            if (((KronosCombiBank)(Parent)).Cbk2PcgOffset != 0)
            {
                for (var parameterIndex = 0; parameterIndex < KronosCombiBanks.ParametersInCbk2Chunk; parameterIndex++)
                {
                    for (var timbre = 0; timbre < Timbres.TimbresCollection.Count; timbre++)
                    {
                        var patchIndex = ((KronosCombiBank)Parent).GetParameterOffsetInCbk2(Index, timbre, parameterIndex);
                        diffs += (Util.GetInt(PcgRoot.Content, patchIndex, 1) != otherCombi.KronosOs1516Content[parameterIndex]) ? 1 : 0;
                    }
                }
            }
            return diffs;
        }


        /// <summary>
        /// 
        /// </summary>
        public static int SizeBetweenCmb2AndCbk2 => 8;
    }
}
