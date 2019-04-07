// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Common.Extensions;
using Common.Utils;
using System.Collections.Generic;
using System.Text;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;

namespace PcgTools.Model.Common.Synth.PatchPrograms
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Program : Patch<Program>, IProgram // , INavigable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="programBank"></param>
        /// <param name="index"></param>
        protected Program(IBank programBank, int index)
        {
            Bank = programBank;
            Index = index;
            Id = $"{programBank.Id}{index.ToString("000")}";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsModeled(ProgramBank.SynthesisType type)
        {
            return (type >= ProgramBank.FirstModeledSynthesisType); // From Prophecy board
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
        public override void Clear()
        {
            Name = string.Empty;
            var category = GetParam(ParameterNames.ProgramParameterName.Category);
            if (category != null)
            {
                category.Value = 0;
                if (PcgRoot.HasSubCategories)
                {
                    var subCategory = GetParam(ParameterNames.ProgramParameterName.SubCategory);
                    if (subCategory != null)
                    {
                        subCategory.Value = 0;
                    }
                }
            }
            Update("Category");

            RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Global
        public string Favorite => Root.AreFavoritesSupported && GetParam(ParameterNames.ProgramParameterName.Favorite).Value ? "X" : string.Empty;


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string PatchTypeAsString => Strings.Program;


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

                if (PcgRoot.CombiBanks != null)
                {
// ReSharper disable once UnusedVariable
                    foreach (var timbre in from bank in PcgRoot.CombiBanks.BankCollection
                        where bank.IsLoaded
                        from patch in bank.Patches.Cast<ICombi>()
                        where !patch.IsEmptyOrInit
                        from timbre in patch.Timbres.TimbresCollection
                        where timbre.UsedProgram == this
                        select timbre)
                    {
                        numberOfReferences++;
                    }

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
                if (!PcgRoot.HasProgramCategories)
                {
                    return string.Empty;
                }

                var categoryAsName = string.Empty;
                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.

                if (global == null)
                {
                    var param = GetParam(ParameterNames.ProgramParameterName.Category);
                    if (param != null)
                    {
                        categoryAsName = GetParam((ParameterNames.ProgramParameterName.Category)).Value.ToString();
                    }
                }
                else
                {
                    categoryAsName = global.GetCategoryName(this);
                }


                return categoryAsName;
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

                // If the program is a GM patch, it is unknown what the category is since GM program parameters are unknown.
                if (((ProgramBank) Bank).Type == BankType.EType.Gm)
                {
                    return "???";
                }

                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.
                var subCategoryAsName = string.Empty;

                if (global == null)
                {
                    var param = GetParam((ParameterNames.ProgramParameterName.SubCategory));
                    if (param != null)
                    {
                        subCategoryAsName = GetParam((ParameterNames.ProgramParameterName.SubCategory)).Value.ToString();
                    }
                }
                else
                {
                    subCategoryAsName = global.GetSubCategoryName(this);
                }

                return subCategoryAsName;
            }
        }


        /// <summary>
        /// Return empty list by default. Override when drum kits are supported.
        /// </summary>
        public virtual List<IDrumKit> UsedDrumKits => new List<IDrumKit>();


        /// <summary>
        ///
        /// </summary>
        /// <param name="changes"></param>
        public virtual void ReplaceDrumKit(Dictionary<IDrumKit, IDrumKit> changes)
        {
            return;
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<IWaveSequence> UsedWaveSequences => new List<IWaveSequence>();


        /// <summary>
        ///
        /// </summary>
        /// <param name="changes"></param>
        public virtual void ReplaceWaveSequence(Dictionary<IWaveSequence, IWaveSequence> changes)
        {
            for (var osc = 0; osc < 2; osc++)
            {
                for (var zone = 0; zone < NumberOfZones; zone++)
                {
                    var osc1 = osc;
                    var zone1 = zone;
                    foreach (var change in changes.Where(change => GetUsedWaveSequence(osc1, zone1) == change.Key))
                    {
                        SetWaveSequence(osc, zone, change.Value);
                        break; // If one change made, skip other changes (otherwise it reverts back)
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual int NumberOfZones => 0;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// /// <param name="zone"></param>
        /// <returns></returns>
        public virtual IWaveSequence GetUsedWaveSequence(int osc, int zone)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="osc"></param>
        /// /// <param name="zone"></param>
        /// <param name="waveSequence"></param>
        public virtual void SetWaveSequence(int osc, int zone, IWaveSequence waveSequence)
        {
            // Default without implementation
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IParameter GetParam(ParameterNames.ProgramParameterName name)
        {
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMasterPcgFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FileState":
                case "FileStateAsString":
                    break;
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

                    //default: No action needed
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

                case "NumberOfReferencesShown":
                    RaisePropertyChanged("", false);
                    break;

                //default: no action needed.
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateCategory()
        {
            // No action needed.
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool IsDrumProgram => (CategoryAsName.ToUpper().Contains("DRUM"));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual int GetFixedParameterValue(FixedParameter.EType type)
        {
            throw new NotSupportedException("Not supported fixed parameter value");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public virtual void SetFixedParameterValue(FixedParameter.EType type, int value)
        {
            throw new NotSupportedException("Not supported fixed parameter value");
        }


        /// <summary>
        /// Change all references to the current patch, towards the specified patch.
        /// </summary>
        /// <param name="newPatch"></param>
        public override void ChangeReferences(IPatch newPatch)
        {
            if (newPatch.PcgRoot.CombiBanks != null)
            {
                ChangeReferencesFromCombis(newPatch as IProgram);
            }

            if (newPatch.PcgRoot.SetLists != null)
            {
                ChangeReferencesFromSetLists(newPatch as IProgram);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPatch"></param>
        private void ChangeReferencesFromCombis(IProgram newPatch)
        {
            var combiBanks = (((IPcgMemory) Parent.Parent.Parent).CombiBanks.BankCollection);
            if (combiBanks == null)
            {
                return;
            }

            foreach (var timbre in from combiBank in combiBanks
                where combiBank.IsLoaded
                from patch in combiBank.Patches
                select (ICombi) patch
                into combi
                from timbre in combi.Timbres.TimbresCollection
                where timbre.UsedProgram == this
                where ((timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value == "Off") ||
                       (timbre.GetParam(ParameterNames.TimbreParameterName.Status).Value == "Int"))    
                select timbre)
            {
                timbre.UsedProgram = newPatch;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPatch"></param>
        private void ChangeReferencesFromSetLists(IPatch newPatch)
        {
            var setLists = (((IPcgMemory) Parent.Parent.Parent).SetLists.BankCollection);
            if (setLists == null)
            {
                return;
            }

            foreach (var setListSlot in
                from setList in setLists
                where setList.IsLoaded
                from ISetListSlot setListSlot in setList.Patches
                where (setListSlot.SelectedPatchType == SetListSlot.PatchType.Program) &&
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
                    var usedDrumKits = UsedDrumKits.Where(drumKit => drumKit != null);
                    if (usedDrumKits.Any())
                    {
                        builder.AppendLine("Used Drumkits:");
                    }

                    foreach (var drumKit in usedDrumKits)
                    {
                        builder.AppendLine(drumKit.Id + ": " + drumKit.Name);
                    }

                    builder.Append(UsedDrumTrackProgram == null
                        ? string.Empty
                        : "Used Drum Track Program: " + UsedDrumTrackProgram.Id + "\n");

                    builder.Append(UsedDrumTrackPattern == null
                        ? string.Empty
                        : "Used Drum Track Pattern: " + UsedDrumTrackPattern.Id + "\n");

                    var usedWaveSequences = UsedWaveSequences.Where(WaveSequence => WaveSequence != null);
                    if (usedWaveSequences.Any())
                    {
                        builder.AppendLine("Used Wave Sequences:");
                    }

                    foreach (var waveSequence in usedWaveSequences)
                    {
                        builder.AppendLine(waveSequence.Id + ": " + waveSequence.Name);
                    }
                }

                return builder.ToString().RemoveLastNewLine();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IProgram UsedDrumTrackProgram
        {
            get
            {
                var paramBank = GetParam(ParameterNames.ProgramParameterName.DrumTrackProgramBank);
                if (paramBank != null)
                {
                    var bank = (IProgramBank) PcgRoot.ProgramBanks.GetBankWithPcgId((int) (paramBank.Value));

                    var paramNumber = GetParam(ParameterNames.ProgramParameterName.DrumTrackProgramNumber);
                    if (paramNumber != null)
                    {
                        return bank.Patches[paramNumber.Value];
                    }
                }

                return null;
            }

            set
            {
                var paramBank = GetParam(ParameterNames.ProgramParameterName.DrumTrackProgramBank);
                if (paramBank != null)
                {
                    paramBank.Value = ((IProgramBank) value.Parent).PcgId;

                    var paramNumber = GetParam(ParameterNames.ProgramParameterName.DrumTrackProgramNumber);
                    if (paramNumber != null)
                    {
                        paramNumber.Value = value.Index;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IDrumPattern UsedDrumTrackPattern
        {
            get 
            {
                var paramBank = GetParam(ParameterNames.ProgramParameterName.DrumTrackCommonPatternBank);
                if (paramBank != null)
                {
                    var bank = (IDrumPatternBank) PcgRoot.DrumPatternBanks.GetBankWithPcgId((int) (paramBank.Value));

                    var paramNumber = GetParam(ParameterNames.ProgramParameterName.DrumTrackCommonPatternNumber);
                    if (paramNumber != null)
                    {
                        return bank.Patches[paramNumber.Value];
                    }
                }

                return null;
            }

            set
            {
                var paramBank = GetParam(ParameterNames.ProgramParameterName.DrumTrackCommonPatternBank);
                if (paramBank != null)
                {
                    paramBank.Value = ((IDrumPatternBank) value.Parent).PcgId;

                    var paramNumber = GetParam(ParameterNames.ProgramParameterName.DrumTrackCommonPatternNumber);
                    if (paramNumber != null)
                    {
                        paramNumber.Value = value.Index;
                    }
                }
            }
        }
    }
}
