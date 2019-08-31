// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PcgTools.ClipBoard;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Model.KronosOasysSpecific.Synth;

namespace PcgTools.Model.KronosSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KronosProgram : KronosOasysProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        public KronosProgram(IProgramBank programBank, int index)
            : base(programBank, index)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            if (PcgRoot.AreFavoritesSupported)
            {
                GetParam(ParameterNames.ProgramParameterName.Favorite).Value = false;
            }

            RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// Returns used drum kits. 
        /// If OSC Mode is Single/Drums        => use MS Bank/Number as Drum Kit (if MS Type == Drums), for OSC 1      , zone 1-8 (if used)
        /// If OSC Mode is Double/Double Drums => use MS Bank/Number as Drum Kit (if MS Type == Drums), for OSC 1 and 2, zone 1-8 (if used)

        /// </summary>
        public override IEnumerable<IWaveSequence> UsedWaveSequences
        {
            get
            {
                var param = GetParam(ParameterNames.ProgramParameterName.OscMode).Value;

                var maxOsc = 0;
                switch ((string) param)
                {
                    case "Single":
                        maxOsc = 1;
                        break;

                    case "Double":
                        maxOsc = 2;
                        break;

                    case "- (EXI)":
                        maxOsc = 0; // EXI do not have wave sequences
                        break;

                    default:
                        Debug.Fail("Unknown OSC Mode");
                        break;
                }

                var usedWaveSequences = new List<IWaveSequence>();
                for (var osc = 0; osc < maxOsc; osc++)
                {
                    for (var zone = 0; zone < 8; zone++)
                    {
                        var usedWaveSequence = GetUsedWaveSequence(osc, zone);
                        if (usedWaveSequence != null)
                        {
                            usedWaveSequences.Add(usedWaveSequence);
                        }
                    }
                }

                return usedWaveSequences;
            }
        }


        /// <summary>
        /// Returns the MS type of osc (zero based) and zone (zero based). If OSC Mode is drum kit, MS type is ignored.
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        private string GetMsType(int osc, int zone)
        {
            var parameter = new EnumParameter();
            parameter.Set(Root, Root.Content, ByteOffset + 2774 + osc*(3240 - 2774) + zone*(2796 - 2774), 1, 0,
                new List<string> {"Off", "MS", "Wave Sequence"}, this);
            return parameter.Value;
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
        public override IParameter GetParam(ParameterNames.ProgramParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.ProgramParameterName.OscMode: // In the documentation called fully Oscillator Mode
                    parameter = EnumParameter.Instance.Set(Root, Root.Content, ByteOffset + 2558, 2, 0,
                        new List<string> {"Single", "Double", "Drums", "- (EXI)", "- (Unused)", "Double Drums"}, this);
                    break;

                case ParameterNames.ProgramParameterName.Category:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 2568, 4, 0, false, this);
                    break;

                case ParameterNames.ProgramParameterName.SubCategory:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 2568, 7, 5, false, this);
                    break;

                case ParameterNames.ProgramParameterName.Favorite:
                    parameter = BoolParameter.Instance.Set(Root, Root.Content, ByteOffset + 2558, 5, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackCommonPatternNumber:
                    parameter = WordParameter.Instance.Set(Root, Root.Content, ByteOffset + 1292, true, 1, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackCommonPatternBank:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 1294, 1, 0, false, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackProgramNumber:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 2688, 7, 0, false, this);
                    break;

                case ParameterNames.ProgramParameterName.DrumTrackProgramBank:
                    parameter = IntParameter.Instance.Set(Root, Root.Content, ByteOffset + 2689, 7, 0, false, this);
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
        public static int SizeBetweenPrg2AndPbk2 => 8;


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

            // Take PBK2 differences into account.
            if (((KronosProgramBank) (Parent)).Pbk2PcgOffset != 0)
            {
                for (var parameterIndex = 0;
                    parameterIndex < KronosProgramBanks.ParametersInPbk2Chunk;
                    parameterIndex++)
                {
                    var patchIndex = ((KronosProgramBank) Parent).GetParameterOffsetInPbk2(Index, parameterIndex);
                    var otherPatchIndex = ((KronosProgramBank) otherPatch.Parent).GetParameterOffsetInPbk2(Index,
                        parameterIndex);

                    diffs += (Util.GetInt(PcgRoot.Content, patchIndex, 1) !=
                              Util.GetInt(otherPatch.PcgRoot.Content, otherPatchIndex, 1))
                        ? 1
                        : 0;

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
            var otherProgram = otherPatch as ClipBoardProgram;
            Debug.Assert(otherProgram != null);

            var diffs = base.CalcByteDifferences(otherPatch, includingName, maxDiffs);

            // Take PBK2 differences into account.
            if (((KronosProgramBank) (Parent)).Pbk2PcgOffset != 0)
            {
                for (var parameterIndex = 0;
                    parameterIndex < KronosProgramBanks.ParametersInPbk2Chunk;
                    parameterIndex++)
                {
                    var patchIndex = ((KronosProgramBank) Parent).GetParameterOffsetInPbk2(Index, parameterIndex);
                    diffs += (Util.GetInt(PcgRoot.Content, patchIndex, 1) !=
                              otherProgram.KronosOs1516Content[parameterIndex])
                        ? 1
                        : 0;
                }
            }
            return diffs;
        }


        /// <summary>
        /// Number of zones.
        /// </summary>
        protected override int NumberOfZones => 8;



        /// <summary>
        /// 
        /// </summary>
        /// /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public override IWaveSequence GetUsedWaveSequence(int osc, int zone)
        {
            IWaveSequence waveSequence = null;
            if (GetZoneMsType(osc, zone) == EMode.WaveSequence)
            {
                var parameter = new IntParameter();
                var waveSequenceByteOffset = GetZoneMsByteOffset(osc, zone);
                int bankIndex;
                int patchIndex;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (PcgRoot.Model.OsVersion)
                {
                    case Models.EOsVersion.EOsVersionKronos10_11: // FALL THROUGH
                    case Models.EOsVersion.EOsVersionKronos15_16:
                        parameter.Set(Root, Root.Content, waveSequenceByteOffset + 16, 1, 0, false, this);
                        bankIndex = parameter.Value;
                        if (bankIndex >= 0x40)
                        {
                            bankIndex -= 0x40; // 40..46.. U-A..U-G
                        }
                        parameter.Set(Root, Root.Content, waveSequenceByteOffset + 17, 1, 0, false, this);
                        patchIndex = parameter.Value;
                        waveSequence = (IWaveSequence) PcgRoot.WaveSequenceBanks[bankIndex].Patches[patchIndex];
                        break;

                    case Models.EOsVersion.EOsVersionKronos2x: // FALL THROUGH
                    case Models.EOsVersion.EOsVersionKronos3x:
                        parameter.Set(Root, Root.Content, waveSequenceByteOffset + 16, 2, 0, false, this);
                        int waveSequenceIndex = parameter.Value;

                        GetWaveSequenceIndices(waveSequenceIndex, out bankIndex, out patchIndex);
                        waveSequence = (IWaveSequence) PcgRoot.WaveSequenceBanks[bankIndex].Patches[patchIndex];
                        break;

                    default:
                        throw new ApplicationException("Illegal (non Kronos) model");
                }
            }

            return waveSequence;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        private EMode GetZoneMsType(int osc, int zone)
        {
            var offset = ByteOffset + 2774 + osc*(3240 - 2774) +
                         zone*(2796 - 2774);

            var parameter = new IntParameter();
            parameter.Set(Root, Root.Content, offset, 1, 0, false, null);
            int value = parameter.Value;
            EMode mode;
            switch (value)
            {
                case 0:
                    mode = EMode.Off;
                    break;

                case 1:
                    mode = EMode.Sample;
                    break;

                case 2:
                    mode = EMode.WaveSequence;
                    break;

                default:
                    mode = EMode.Off;
                    // Not supported, selected when copy drum kit from Kronos OS3.0 file.
                    // throw new ApplicationException("Illegal case");
                    break;
            }

            return mode;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// /// <param name="zone"></param>
        /// <returns></returns>
        protected override int GetZoneMsByteOffset(int osc, int zone)
        {
            return ByteOffset + 2774 + osc*(3240 - 2774) + zone*(2796 - 2774);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveSequenceIndex"></param>
        /// <param name="bankIndex"></param>
        /// <param name="patchIndex"></param>
        void GetWaveSequenceIndices(int waveSequenceIndex, out int bankIndex, out int patchIndex)
        {
            bankIndex = 0;
            patchIndex = waveSequenceIndex;

            while (waveSequenceIndex >= PcgRoot.WaveSequenceBanks[bankIndex].Patches.Count)
            {
                patchIndex -= PcgRoot.WaveSequenceBanks[bankIndex].Patches.Count;
                bankIndex++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveSequence"></param>
        /// <returns></returns>
        int GetWaveSequenceIndex(IWaveSequence waveSequence)
        {
            var bank = (IWaveSequenceBank) waveSequence.Parent;

            var index = PcgRoot.WaveSequenceBanks.BankCollection.TakeWhile(
                bankIterator => bank != bankIterator).Sum(bankIterator => bankIterator.Patches.Count);

            index += waveSequence.Index;

            return index;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// /// <param name="zone"></param>
        /// <param name="waveSequence"></param>
        public override void SetWaveSequence(int osc, int zone, IWaveSequence waveSequence)
        {
            IntParameter parameter = new IntParameter();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (PcgRoot.Model.OsVersion)
            {
                case Models.EOsVersion.EOsVersionKronos10_11: // FALL THROUGH
                case Models.EOsVersion.EOsVersionKronos15_16:
                    var bankIndex = ((IBank) waveSequence.Parent).Index;
                    if (bankIndex >= 0x40)
                    {
                        bankIndex -= 0x40; // 40..46.. U-A..U-G
                    }

                    parameter.Set(Root, Root.Content, GetZoneMsByteOffset(osc, zone) + 1, 7, 0, false, this);
                    parameter.Value = bankIndex;

                    parameter.Set(Root, Root.Content, GetZoneMsByteOffset(osc, zone) + 2, 7, 0, false, this);
                    parameter.Value = waveSequence.Index;
                    break;

                case Models.EOsVersion.EOsVersionKronos2x: // FALL THROUGH
                case Models.EOsVersion.EOsVersionKronos3x:
                    parameter.SetMultiBytes(Root, Root.Content, GetZoneMsByteOffset(osc, zone), 2, false, false, null);
                    parameter.Value = GetWaveSequenceIndex(waveSequence);
                    break;

                default:
                    throw new ApplicationException("Illegal (non Kronos) model");
            }
        }
    }
}