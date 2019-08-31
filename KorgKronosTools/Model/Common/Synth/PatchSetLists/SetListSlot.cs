// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Common.Extensions;
using Common.Utils;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.NewParameters;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.Model.Common.Synth.PatchSetLists
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SetListSlot : Patch<SetListSlot>, ISetListSlot // , INavigable
    {
        /// <summary>
        /// 
        /// </summary>
        protected int DescriptionPcgOffset { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        protected int VolumePcgOffset { get; private set; }
        

        /// <summary>
        /// Order is not a mistake, order is S, XS, M, L, XL (MIDI Spec OS3.0).
        /// </summary>
        public enum TextSize
        {
            S,
            Xs,
            M,
            L,
            Xl
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setList"></param>
        /// <param name="index"></param>
        /// <param name="volumePcgOffset"></param>
        /// <param name="descriptionPcgOffset"></param>
        protected SetListSlot(IBank setList, int index,
            int volumePcgOffset, int descriptionPcgOffset)
        {
            Bank = setList;
            Index = index;
            Id = $"{setList.Id}/{index}";
            VolumePcgOffset = volumePcgOffset;
            DescriptionPcgOffset = descriptionPcgOffset;
        }


        /// <summary>
        /// Returns the number of differences within the name and description of two patches.
        /// </summary>
        /// <param name="otherPatch"></param>
        /// <returns></returns>
        public override int CalcByteDifferencesInNameAndDescription(IPatch otherPatch)
        {
            var patchSize = ByteLength;
            Debug.Assert(patchSize == ByteLength);
            Debug.Assert(otherPatch != null);
            Debug.Assert(MaxNameLength == otherPatch.MaxNameLength);

            // Add name differences.
            var diffs = 0;
            for (var index = 0; index < MaxDescriptionLength; index++)
            {
                diffs += (PcgRoot.Content[ByteOffset + DescriptionPcgOffset + index] != 
                    otherPatch.PcgRoot.Content[otherPatch.ByteOffset + DescriptionPcgOffset + index]) ? 1 : 0;
            }

            return diffs;
        }


        /// <summary>
        /// 
        /// </summary>
        public override void SetNotifications()
        {
            var masterFile = MasterFiles.MasterFiles.Instances.FindMasterFile(Root.Model);
            if ((masterFile != null) && !PcgRoot.FileName.IsEqualFileAs(masterFile.FileName))
            {
                masterFile.PropertyChanged += OnMasterPcgFilePropertyChanged;
            }
            
            PcgRoot.PropertyChanged += OnPatchPropertyChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] protected abstract IBank UsedProgramBank { get; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] protected abstract IBank UsedCombiBank { get; }


        /// <summary>
        /// 
        /// </summary>
        string FullPatchId
        {
            get
            {
                string fullPatchId;
                switch (SelectedPatchType)
                {
                case PatchType.Program:
                    var usedProgram = UsedPatch;
                    fullPatchId = (usedProgram == null) ? "(Unknown)" : "Prg " + (usedProgram.Id);
                    break;

                case PatchType.Combi:
                    var usedCombi = UsedPatch;
                    fullPatchId = (usedCombi == null) ? "Unknown)" : "Cmb " + (usedCombi.Id);
                    break;

                case PatchType.Song:
                    fullPatchId = "Song";
                    break;

                default:
                    throw new NotSupportedException("Unknown type");
                }
                return fullPatchId;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string PatchTypeAsString => Strings.SetListSlot;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly]
        public string Reference => FullPatchId;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public string ProgramCombiName => ((SelectedPatchType == PatchType.Song) || (PcgRoot.Content == null) || (UsedPatch == null))
            ? "(Unknown)"
            : (((IBank)(UsedPatch.Parent)).IsLoaded ? UsedPatch.Name : "(Unknown)");


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        [UsedImplicitly] public string VolumeAsString
        // ReSharper restore UnusedMember.Global
            => Volume.ToString(CultureInfo.InvariantCulture);


        /// <summary>
        /// 
        /// </summary>
        public abstract string Description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public string DescriptionInList => Settings.Default.SingleLinedSetListSlotDescriptions ? Description.Replace("\r\n", " / ") : Description;


        /// <summary>
        /// 
        /// </summary>
        public abstract int Volume { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public abstract int Transpose { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IParameter GetParam(ParameterNames.SetListSlotParameterName name)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract IIntParameter Color { get; }


        /// <summary>
        /// 
        /// </summary>
        public abstract TextSize SelectedTextSize { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public abstract int MaxDescriptionLength { get; }


        /// <summary>
        /// 
        /// </summary>
        public abstract IPatch UsedPatch { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum PatchType { Program = 1, Combi = 0, Song = 2 } ;


        /// <summary>
        /// 
        /// </summary>
        public abstract PatchType SelectedPatchType { get; set; }


        /// <summary>
        /// A set list slot is considered empty when the name is empty AND it references to program I-A000
        /// (or first program in first bank).
        /// </summary>
        public override bool IsEmptyOrInit
        {
            get
            {
                var isEmpty = ((Name == string.Empty) && (SelectedPatchType == PatchType.Program));
                if (isEmpty)
                {
                    // Check further (Program is from bank 0, index 0.
                    var usedProgram = (Program) UsedPatch;
                    var usedProgramBank = ((ProgramBank) (usedProgram.Bank));
                    isEmpty = ((((ProgramBanks) (usedProgramBank.Parent)).BankCollection.IndexOf(usedProgramBank)) == 0) &&
                              (usedProgram.Index == 0);

                }

                return isEmpty;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            Name = string.Empty;
            Description = string.Empty;
            SelectedPatchType = PatchType.Program;

            RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleteInPcg
        {
            get
            {
                if (UsedPatch is Program)
                {
                    return (((IBank)(UsedPatch.Parent)).IsLoaded || (((ProgramBank)UsedPatch.Parent).Type != BankType.EType.Gm));
                }

                var combi = UsedPatch as Combi;
                if (combi != null)
                {
                    return combi.IsCompleteInPcg;
                }

                return false; // Songs never are in a PCG.
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterOnText"></param>
        /// <param name="filterText"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="filterDescription"></param>
        /// <returns></returns>
        protected override bool FilterOnText(bool filterOnText, string filterText, bool caseSensitive, 
            bool filterDescription = true)
        {
            return !filterOnText || (caseSensitive && Name.Contains(filterText)) ||
                (!caseSensitive && Name.ToUpper().Contains(filterText.ToUpper())) ||
             (filterDescription && ((caseSensitive && Description.Contains(filterText)) ||
             (!caseSensitive && Description.ToUpper().Contains(filterText.ToUpper()))));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMasterPcgFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FileState":
                case "FileStateAsString":
                    if (PcgRoot.Content != null)
                    {
                       UpdateUsedPatch();
                    }
                    break;

                //default:
                    //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPatchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadingFinished":
                    Update("UsedPatch");
                    break;

                //default:
                    //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void Update(string name)
        {
            RaisePropertyChanged(string.Empty, false);
            switch (name)
            {
                case "ContentChanged":
                    // UpdateUsedPatch();
                    break;

                case "UsedPatch":
                    //UpdateUsedPatch();
                    break;

                case "ShowSingleLinedSetListSlotDescriptions":
                    RaisePropertyChanged("DescriptionInList");
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void UpdateUsedPatch()
        {
            RaisePropertyChanged("UsedPatch");
            RaisePropertyChanged("Reference");
            RaisePropertyChanged("ProgramCombiName");
            RaisePropertyChanged("VolumeAsString");
            RaisePropertyChanged("Description");
        }


        /// <summary>
        /// Change all references to the current patch, towards the specified patch.
        /// Since set list slots are not referenced; do nothing.
        /// </summary>
        /// <param name="newPatch"></param>
        public override void ChangeReferences(IPatch newPatch)
        {
            // Do nothing
        }
        

        /// <summary>
        /// 
        /// </summary>
        public override bool ToolTipEnabled => !IsEmptyOrInit;


        /// <summary>
        /// 
        /// </summary>
        public override string ToolTip
        {
            get
            {
                var builder = new StringBuilder();
                if (IsEmptyOrInit)
                {
                    builder.Append(Strings.EmptyOrInitPatchName);
                }

                return builder.ToString().RemoveLastNewLine();
            }
        }

        public int NumberOfReferences
        {
            get
            {
                return 0;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void ChangeVolume(ChangeVolumeParameters parameters, int minimumValue, int maximumValue)
        {
            switch (parameters.ChangeType)
            {
                case ChangeVolumeParameters.EChangeType.Fixed:
                    Volume = parameters.Value;
                    break;

                case ChangeVolumeParameters.EChangeType.Relative:
                    Volume = MathUtils.ClipValue(Volume + parameters.Value, 0, 127);
                    break;

                case ChangeVolumeParameters.EChangeType.Percentage:
                    Volume = (int)(Volume * (float)parameters.Value / 100.0 + 0.5);
                    break;

                case ChangeVolumeParameters.EChangeType.Mapped:
                    Volume = MathUtils.MapValue(Volume, 0, 127, parameters.Value, parameters.ToValue);
                    break;

                case ChangeVolumeParameters.EChangeType.SmartMapped:
                    Volume = MathUtils.MapValue(Volume, minimumValue, maximumValue, parameters.Value, parameters.ToValue);
                    break;

                default:
                    throw new ApplicationException("Illegal ChangeVolumeParameter");
            }

            Update("ContentChanged");
        }
    }
}
