// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Common.Extensions;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Combi : Patch<Combi>, ICombi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="combiBank"></param>
        /// <param name="index"></param>
        protected Combi(IBank combiBank, int index)
        {
            Bank = combiBank;
            Index = index;
            Id = $"{combiBank.Id}{index.ToString("000")}";
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

            foreach (var timbre in Timbres.TimbresCollection)
            {
                timbre.SetNotifications();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Timbres Timbres { get; protected set; }


        /// <summary>
        /// Switches two MIDI channels (of all timbres if they are enabled).
        /// </summary>
        /// <param name="mainMidiChannel"></param>
        /// <param name="secondaryMidiChannel"></param>
        public void SwitchMidiChannels(int mainMidiChannel, int secondaryMidiChannel)
        {
            foreach (var timbre in Timbres.TimbresCollection.Where(
                timbre => new List<string> {"Int", "On", "Both"}.Contains(timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value)))
            {
                if (timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value == mainMidiChannel - 1)
                {
                    timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = secondaryMidiChannel - 1;
                }
                else if (timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value == secondaryMidiChannel - 1)
                {
                    timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = mainMidiChannel - 1;
                }
            }
        }

        public bool UsesMidiChannel(int secondaryMidiChannel)
        {
            return Timbres.TimbresCollection.Any(timbre =>
                timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value + 1 == secondaryMidiChannel &&
                new List<string> {"On", "Int"}.Contains(timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value));
        }


        /// <summary>
        /// Initialize as MIDI MPE:
        /// MIDI Channel equal to timbre nummer.
        /// Copy all parameters including used program from Timbre 0.
        /// </summary>
        public void InitAsMpe()
        {
            var timbre0 = Timbres.TimbresCollection[0];
            var midiChannel = 0;
            timbre0.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = midiChannel++; // MIDI channel 1 (value 0)

            foreach (var timbre in Timbres.TimbresCollection.Where(timbre => timbre != timbre0))
            {
                timbre.GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = midiChannel++; // MIDI channel same as timbre number

                timbre.UsedProgram = timbre0.UsedProgram;


                var parameterNames = new ParameterNames.TimbreParameterName[]
                {
                    ParameterNames.TimbreParameterName.Status, 
                    ParameterNames.TimbreParameterName.Mute,
                    ParameterNames.TimbreParameterName.Volume,
                    ParameterNames.TimbreParameterName.BottomKey,
                    ParameterNames.TimbreParameterName.TopKey,
                    ParameterNames.TimbreParameterName.BottomVelocity,
                    ParameterNames.TimbreParameterName.TopVelocity,
                    ParameterNames.TimbreParameterName.OscMode,
                    ParameterNames.TimbreParameterName.OscSelect
                    // "Transpose", "Detune", "Portamento", "Bend Range" are not set since parameters with negative values cannot be set.
                };

                var timbreToChange = timbre;
                foreach (
                    var parameterName in
                        parameterNames.Where(parameterName => timbreToChange.GetParam(parameterName) != null))
                {
                    try
                    {
                        dynamic parameterValue = timbre0.GetParam(parameterName).Value;
                        timbre.GetParam(parameterName).Value = parameterValue;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IParameter GetParam(ParameterNames.CombiParameterName name)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleteInPcg
        {
            get
            {
                return Timbres.TimbresCollection.All(timbre => (timbre.UsedProgramBank != null) &&
                                                               !timbre.UsedProgram.IsFromMasterFile &&
                                                               ((timbre.UsedProgramBank.Type == BankType.EType.Gm) ||
                                                                timbre.UsedProgramBank.IsLoaded));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable UnusedMember.Global
        public string Favorite => (((IBank) (Parent)).IsLoaded &&
                                   Root.AreFavoritesSupported &&
                                   GetParam(ParameterNames.CombiParameterName.Favorite).Value)
            ? "X"
            : string.Empty;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string PatchTypeAsString => Strings.Combi;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable UnusedMember.Global
        public string NumberOfReferencesAsString
            // ReSharper restore UnusedMember.Global
            => Settings.Default.UI_ShowNumberOfReferencesColumn
                ? NumberOfReferences.ToString(CultureInfo.InvariantCulture)
                : string.Empty;


        /// <summary>
        /// Count number of reference (program used by combis or set list slots.
        /// </summary>
        public int NumberOfReferences
        {
            get
            {
                var numberOfReferences = 0;

                if (PcgRoot.SetLists != null)
                {
                    // ReSharper disable once UnusedVariable
                    foreach (var patch in from bank in PcgRoot.SetLists.BankCollection
                        where bank.IsLoaded
                        from ISetListSlot patch in bank.Patches
                        where !patch.IsEmptyOrInit
                        where patch.UsedPatch == this
                        select patch)
                    {
                        numberOfReferences++;
                    }
                }

                return numberOfReferences;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string CategoryAsName
        {
            get
            {
                if (!PcgRoot.HasCombiCategories)
                {
                    return string.Empty;
                }

                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.
                return (global == null)
                    ? GetParam(ParameterNames.CombiParameterName.Category).Value.ToString()
                    : global.GetCategoryName(this);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string SubCategoryAsName
        {
            get
            {
                // If the model does not support sub categories, return an empty string.
                if (!PcgRoot.HasSubCategories)
                {
                    return string.Empty;
                }

                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.
                return (global == null)
                    ? GetParam(ParameterNames.CombiParameterName.SubCategory).Value.ToString()
                    : global.GetSubCategoryName(this);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            Name = string.Empty;

            if (PcgRoot.HasCombiCategories)
            {
                GetParam(ParameterNames.CombiParameterName.Category).Value = 0;
            }

            if (PcgRoot.HasSubCategories)
            {
                GetParam(ParameterNames.CombiParameterName.SubCategory).Value = 0;
            }

            foreach (var timbre in Timbres.TimbresCollection)
            {
                timbre.Clear();
            }

            RaisePropertyChanged(string.Empty, false);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMasterPcgFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FileState":
                case "FileStateAsString":
                    //Column3 = CategoryAsName;
                    //Column4 = SubCategoryAsName;
                    break;

                //default:
                // break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPatchPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadingFinished": // Fall through
                case "Global": // Fall through
                case "CategoryChanged":
                    Update("Category");
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
            switch (name)
            {
                case "ContentChanged":
                    UpdateCategory();
                    break;

                case "Category":
                    UpdateCategory();
                    break;

                //default:
                //break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateCategory()
        {
            //Column3 = CategoryAsName;
            //Column4 = SubCategoryAsName;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPatch"></param>
        public override void ChangeReferences(IPatch newPatch)
        {
            IPcgMemory pcgMemory = ((IPcgMemory) Parent.Parent.Parent);
            var setLists = pcgMemory.SetLists == null ? null : pcgMemory.SetLists.BankCollection;
            if (setLists == null)
            {
                return;
            }

            foreach (var setListSlot in
                from setList in setLists
                where setList.IsLoaded
                from ISetListSlot setListSlot in setList.Patches
                where (setListSlot.SelectedPatchType == SetListSlot.PatchType.Combi) &&
                      (setListSlot.UsedPatch == this)
                select setListSlot)
            {
                setListSlot.UsedPatch = newPatch;
            }
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
                else
                {
                    builder.Append(UsedDrumTrackPattern == null
                        ? string.Empty
                        : ("Used Drum Track Pattern: " + UsedDrumTrackPattern.Id + "\n"));

                }

                return builder.ToString().RemoveLastNewLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IDrumPattern UsedDrumTrackPattern
        {
            get
            {
                var paramBank = GetParam(ParameterNames.CombiParameterName.DrumTrackCommonPatternBank);
                if (paramBank != null)
                {
                    IDrumPatternBank bank =
                        (IDrumPatternBank) (PcgRoot.DrumPatternBanks.GetBankWithPcgId((int) paramBank.Value));

                    var paramNumber = GetParam(ParameterNames.CombiParameterName.DrumTrackCommonPatternNumber);
                    if (paramNumber != null)
                    {
                        return bank.Patches[paramNumber.Value];
                    }
                }

                return null;
            }

            set
            {
                var paramBank = GetParam(ParameterNames.CombiParameterName.DrumTrackCommonPatternBank);
                if (paramBank != null)
                {
                    paramBank.Value = ((IDrumPatternBank) value.Parent).PcgId;

                    var paramNumber = GetParam(ParameterNames.CombiParameterName.DrumTrackCommonPatternNumber);
                    if (paramNumber != null)
                    {
                        paramNumber.Value = value.Index;
                    }
                }
            }
        }



        /// <summary>
        /// Minimum volume of all (used) timbres
        /// </summary>
        /// <returns></returns>
        public int GetMinimumVolume()
        {
            int minVolume = 127;

            foreach (var timbre in this.Timbres.TimbresCollection)
            {
                if ((timbre.GetParam(ParameterNames.TimbreParameterName.Mute) == null) ||
                     (!timbre.GetParam(ParameterNames.TimbreParameterName.Mute).Value) &&
                     new List<string> { "Int", "On", "Both" }.Contains(timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value))
                {
                    minVolume = Math.Min(minVolume, timbre.GetParam(ParameterNames.TimbreParameterName.Volume).Value);
                }
            }

            return minVolume;
        }


        /// <summary>
        /// Maximum volume of all (used) timbres
        /// </summary>
        /// <returns></returns>
        public int GetMaximumVolume()
        {
            int maxVolume = 0;

            foreach (var timbre in this.Timbres.TimbresCollection)
            {
                if ((timbre.GetParam(ParameterNames.TimbreParameterName.Mute) == null) ||
                     (!timbre.GetParam(ParameterNames.TimbreParameterName.Mute).Value) &&
                     new List<string> { "Int", "On", "Both" }.Contains(timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value))
                {
                    maxVolume = Math.Max(maxVolume, timbre.GetParam(ParameterNames.TimbreParameterName.Volume).Value);
                }
            }

            return maxVolume;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="minimumVolume"></param>
        /// <param name="maximumVolume"></param>
        public void ChangeVolume(ChangeVolumeParameters parameters, int minimumVolume, int maximumVolume)
        {
            foreach (var timbre in this.Timbres.TimbresCollection)
            {
                var currentVolumeParameter = timbre.GetParam(ParameterNames.TimbreParameterName.Volume);
                
                switch (parameters.ChangeType)
                {
                    case ChangeVolumeParameters.EChangeType.Fixed:
                        currentVolumeParameter.Value = parameters.Value;
                        break;

                    case ChangeVolumeParameters.EChangeType.Relative:
                        currentVolumeParameter.Value = MathUtils.ClipValue(currentVolumeParameter.Value + parameters.Value, 0, 127);
                        break;

                    case ChangeVolumeParameters.EChangeType.Percentage:
                        currentVolumeParameter.Value = (int)(currentVolumeParameter.Value * (float)parameters.Value / 100.0 + 0.5);
                        break;

                    case ChangeVolumeParameters.EChangeType.Mapped:
                        currentVolumeParameter.Value = MathUtils.MapValue(currentVolumeParameter.Value, 0, 127, parameters.Value, parameters.ToValue);
                        break;

                    case ChangeVolumeParameters.EChangeType.SmartMapped:
                        currentVolumeParameter.Value = MathUtils.MapValue(currentVolumeParameter.Value, minimumVolume, maximumVolume, parameters.Value, parameters.ToValue);
                        break;

                    default:
                        throw new ApplicationException("Illegal ChangeVolumeParameter");
                }

                Update("ContentChanged");
            }
        }
    }
}