// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Common.Extensions;
using Common.Mvvm;
using Common.Utils;
using PcgTools.Common;
using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.SongsRelated;
using PcgTools.PcgToolsResources;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Timbre : ObservableObject, ITimbre
    {
        /// <summary>
        /// 
        /// </summary>
        private int _index;


        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return _index;  }
            set
            {
                _index = value;
                RefillColumns();
            }
        }


        /// <summary>
        /// Used for UI control binding for selections.
        /// </summary>
        bool _isSelected;


        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private readonly ITimbres _timbres;


        /// <summary>
        /// 
        /// </summary>
        public int TimbresSize { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbres"></param>
        /// <param name="index"></param>
        /// <param name="timbresSize"></param>
        protected Timbre(ITimbres timbres, int index, int timbresSize)
        {
            Debug.Assert(timbresSize > 0);

            _timbres = timbres;
            Index = index;
            TimbresSize = timbresSize;
            ByteOffset = timbres.ByteOffset + index * timbresSize;
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetNotifications()
        {
            var masterFile = MasterFiles.MasterFiles.Instances.FindMasterFile(Root.Model);
            if ((masterFile != null) && !PcgRoot.FileName.IsEqualFileAs(masterFile.FileName))
            {
                masterFile.PropertyChanged += OnMasterPcgFilePropertyChanged;
            }

            PcgRoot.PropertyChanged += OnPcgRootPropertyChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        protected ICombi Combi => (ICombi) (Parent.Parent);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IParameter GetParam(ParameterNames.TimbreParameterName name)
        {
            IParameter parameter;

            switch (name)
            {
                case ParameterNames.TimbreParameterName.Volume:
                    parameter = IntParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 5, 7, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.MidiChannel:
                    parameter = IntParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 2, 4, 0, false, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Transpose:
                    parameter = IntParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 7, 7, 0, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.Detune:
                    parameter = IntParameter.Instance.SetMultiBytes(
                        Root, Root.Content, TimbresOffset + 8, 2, true, true, Parent as IPatch);
                    break;

                case ParameterNames.TimbreParameterName.BendRange:
                    parameter = IntParameter.Instance.Set(
                        Root, Root.Content, TimbresOffset + 6, 7, 0, true, Parent as IPatch);
                    break;

                default:
                    parameter = null;
                    break;
            }

            return parameter;
        }


        // ReSharper disable once UnusedMember.Global
        public int GetInt(int offset, int length)
        {
            return (PcgRoot.Content == null) ? 0 : Util.GetInt(PcgRoot.Content, ByteOffset + offset, length);
        }


        // ReSharper disable once UnusedMember.Global
        public void SetInt(int offset, int length, int value)
        {
            Util.SetInt(PcgRoot, PcgRoot.Content, ByteOffset + offset, length, value);
        }


        /// <summary>
        /// 
        /// </summary>
        public bool HasMidiChannelGch
        {
            get
            {
                var param = GetParam(ParameterNames.TimbreParameterName.MidiChannel);
                return ((param != null) && (param.Value == ParameterValues.MidiChannelGch)); 
            }
        }


        public virtual IMemory Root => (IMemory) 
            (_timbres is SongTimbres ? _timbres.Parent.Parent : _timbres.Parent.Parent.Parent.Parent);


        /// <summary>
        /// 
        /// </summary>
        protected PcgMemory PcgRoot => (PcgMemory) Root;


        // INavigable

        /// <summary>
        /// 
        /// </summary>
        public virtual INavigable Parent => _timbres;


        /// <summary>
        /// 
        /// </summary>
        public int ByteOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected virtual int UsedProgramBankId => Combi.PcgRoot.Content[TimbresOffset + 1];


        /// <summary>
        /// 
        /// </summary>
        public virtual IProgramBank UsedProgramBank
        {
            get
            {
                // If Combi.Id seems to be null, check in the call stack if bank.IsWritable() should be added.
                var pcgData = Combi.PcgRoot.Content;
                return pcgData == null ? null : (IProgramBank) (PcgRoot.ProgramBanks.GetBankWithPcgId(UsedProgramBankId));
            }
            protected set
            {
                Combi.PcgRoot.Content[TimbresOffset + 1] = (byte)value.PcgId;
                RaisePropertyChanged("UsedProgramBank", false);
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual int UsedProgramId => Combi.PcgRoot.Content[TimbresOffset];


        /// <summary>
        /// 
        /// </summary>
        public virtual IProgram UsedProgram
        {
            get
            {
                if (Parent is ISongTimbres)
                {
                    return null; //TODO Use connected PCG file
                }

                if (Combi.PcgRoot.Content == null)
                {
                    return null;
                }

                var programId = UsedProgramId;
                if (programId >= UsedProgramBank.Patches.Count)
                {
                    // This can happen if a non complete bank is loaded and the remainder is removed
                    // after loading.
                    return null;
                }

                var program = (Program) UsedProgramBank[programId];
                if (!UsedProgramBank.IsWritable && ((ProgramBank) (program.Bank)).Type != BankType.EType.Gm)
                {
                    // Try to find it in the master file.
                    var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(Root.Model);
                    if ((masterPcgMemory != null) && (masterPcgMemory.FileName != Root.FileName))
                    {
                        var programBank = masterPcgMemory.ProgramBanks.BankCollection.FirstOrDefault(
                            item => (item.PcgId == UsedProgramBank.PcgId) && item.IsFilled);
                        return programBank == null ? null : programBank[programId] as Program;
                    }
                }
                return program;
            }
            set
            {
                Combi.PcgRoot.Content[TimbresOffset] = (byte)value.Index;
                UsedProgramBank = (ProgramBank) value.Parent;
                RaisePropertyChanged("UsedProgram", false);
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int TimbresOffset
        {
            get
            {
                var parent = Parent;
                if (parent is SongTimbres)
                {
                    return ByteOffset;
                }

                return Combi.ByteOffset + ((Timbres) Parent).TimbresOffset + Index*TimbresSize;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherTimbre"></param>
        public virtual void Swap(ITimbre otherTimbre)
        {
            if (otherTimbre.TimbresOffset != TimbresOffset)
            {
                Util.SwapBytes(PcgRoot, Root.Content, TimbresOffset, Root.Content, otherTimbre.TimbresOffset,
                               TimbresSize);
                RaisePropertyChanged(string.Empty, false);
                otherTimbre.RaisePropertyChanged(string.Empty, false);

                UsedProgram.RaisePropertyChanged(string.Empty, false);
                otherTimbre.UsedProgram.RaisePropertyChanged(string.Empty, false);
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromTimbre"></param>
        public virtual void CopyFrom(ITimbre fromTimbre)
        {
            if (fromTimbre.TimbresOffset != TimbresOffset)
            {
                Util.CopyBytes(PcgRoot, fromTimbre.TimbresOffset, TimbresOffset, TimbresSize);
                RaisePropertyChanged(string.Empty, false);

                UsedProgram.RaisePropertyChanged(string.Empty, false);
                RefillColumns();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            var memory = (PcgMemory) Root;
            if (memory.AssignedClearProgram == null)
            {
                UsedProgram = (Program) (((ProgramBank) (PcgRoot.ProgramBanks[0]))[0]);
            }
            else
            {
                UsedProgram = memory.AssignedClearProgram;
            }

            GetParam(ParameterNames.TimbreParameterName.Status).Value = "Off";
            if (GetParam(ParameterNames.TimbreParameterName.Mute) != null)
            {
                GetParam(ParameterNames.TimbreParameterName.Mute).Value = true;
            }
            GetParam(ParameterNames.TimbreParameterName.Volume).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value = 15;

            GetParam(ParameterNames.TimbreParameterName.BottomKey).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.TopKey).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value = 0;
            var parameter = GetParam(ParameterNames.TimbreParameterName.OscMode);
            if (parameter != null)
            {
                parameter.Value = "Mono";
            }

            parameter = GetParam(ParameterNames.TimbreParameterName.OscSelect);
            if (parameter != null)
            {
                parameter.Value = "Osc2";
            }

            GetParam(ParameterNames.TimbreParameterName.Transpose).Value = 0;
            GetParam(ParameterNames.TimbreParameterName.Detune).Value = 0;

            parameter = GetParam(ParameterNames.TimbreParameterName.Portamento);
            if (parameter != null)
            {
                parameter.Value = 0;
            }

            parameter = GetParam(ParameterNames.TimbreParameterName.BendRange);
            if (parameter != null)
            {
                parameter.Value = 0;
            }

            RefillColumns();
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        private bool IsGmTimbre => ((ProgramBank) (UsedProgram.Parent)).Type == BankType.EType.Gm;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnIndex => (Index + 1).ToString(CultureInfo.InvariantCulture);


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnProgramId
        {
            get
            {
                if (Parent is ISongTimbres) // 'this' is normal timbre
                {
                    var rawBankIndex = ProgramRawBankIndex;
                    var rawProgramIndex = ProgramRawIndex;
                    return Root.ProgramIdByIndex(rawBankIndex, rawProgramIndex);
                }
                else
                {
                    var usedProgram = UsedProgram;
                    return usedProgram == null ? Strings.Unknown : usedProgram.Id;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual int ProgramRawIndex
        {
            get
            {
                throw new ApplicationException("Not supported");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual int ProgramRawBankIndex
        {
            get
            {
                throw new ApplicationException("Not supported");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnProgramName
        {
            get
            {
                var usedProgram = UsedProgram;
                return usedProgram == null ? Strings.Unknown : UsedProgram.Name;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnCategory
        {
            get
            {
                var usedProgram = UsedProgram;
                return usedProgram == null ? Strings.Unknown : UsedProgram.CategoryAsName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnSubCategory
        {
            get
            {
                var usedProgram = UsedProgram;
                return usedProgram == null ? Strings.Unknown : UsedProgram.SubCategoryAsName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnVolume => (string) GetParam(ParameterNames.TimbreParameterName.Volume).Value.ToString();


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnStatus => GetParam(ParameterNames.TimbreParameterName.Status).Value;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnMute => GetParam(ParameterNames.TimbreParameterName.Mute) == null 
            ? "-" 
            : ((bool) (GetParam(ParameterNames.TimbreParameterName.Mute).Value)).ToYesNo();


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnPriority => ((GetParam(ParameterNames.TimbreParameterName.Priority) == null) 
            ? "-" : 
            ((bool) GetParam(ParameterNames.TimbreParameterName.Priority).Value).ToYesNo());


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnMidiChannel => (GetParam(ParameterNames.TimbreParameterName.MidiChannel) == null)
            ? "-"
            : ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.MidiChannel, 
                (int)GetParam(ParameterNames.TimbreParameterName.MidiChannel).Value);


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnKeyZone => (string.Format(
            "{0}~{1}",
            ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.BottomKey,
                GetParam(ParameterNames.TimbreParameterName.BottomKey).Value),
            ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.TopKey, 
                GetParam(ParameterNames.TimbreParameterName.TopKey).Value)));


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnVelocityZone => (string.Format("{0}~{1}", (string) GetParam(ParameterNames.TimbreParameterName.BottomVelocity).Value.ToString(),
            GetParam(ParameterNames.TimbreParameterName.TopVelocity).Value.ToString()));


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnOscMode => (string) GetParam(ParameterNames.TimbreParameterName.OscMode).Value;

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnOscSelect => (string) GetParam(ParameterNames.TimbreParameterName.OscSelect).Value;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnTranspose => (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Transpose,
            GetParam(ParameterNames.TimbreParameterName.Transpose).Value);


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public string ColumnDetune => (string)
            ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Detune, GetParam(ParameterNames.TimbreParameterName.Detune).Value);


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnPortamento
        {
            get
            {
                var parameter = GetParam(ParameterNames.TimbreParameterName.Portamento);
                return parameter == null ? "-" :
                           (string) ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.Portamento, parameter.Value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        public virtual string ColumnBendRange => (string)
            ParameterValues.GetStringValue(ParameterNames.TimbreParameterName.BendRange, 
                GetParam(ParameterNames.TimbreParameterName.BendRange).Value);


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        private string CategoryAsName
        {
            get
            {
                // If the program is a GM patch, it is unknown what the category is since GM program parameters are unknown.
                if ((UsedProgramBank == null) || (UsedProgramBank.Type == BankType.EType.Gm))
                {
                    //ColumnCategory = "(unknown)";
                    return UsedProgram.SubCategoryAsName;
                }

                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.
                var category = (global == null)
                                   ? UsedProgram.GetParam(ParameterNames.ProgramParameterName.Category).Value.ToString()
                                   : global.GetCategoryName(UsedProgram);

                return category;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        private string SubCategoryAsName
        {
            get
            {
                // If the model does not support sub categories, return an empty string.
                if (!PcgRoot.HasSubCategories)
                {
                    return UsedProgram.SubCategoryAsName;
                }

                // If the program is a GM patch, it is unknown what the category is since GM program parameters are unknown.
                if ((UsedProgramBank == null) || (UsedProgramBank.Type == BankType.EType.Gm))
                {
                    return "(unknown)";
                }

                // Use the global setting, if not available, check the Master PCG file, else use just the number.
                var global = FindGlobal();

                // Return either the value if no global/Master file pressent otherwise the name.
                return (global == null)
                           ? UsedProgram.GetParam(ParameterNames.ProgramParameterName.SubCategory).Value.ToString()
                           : global.GetSubCategoryName(UsedProgram);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once UnusedMember.Local
        private string ProgramName
        {
            get
            {
                if (UsedProgram == null)
                {
                    return "(unknown)";
                }
                if (UsedProgram.ByteOffset == 0)
                {
                    // Find master PCG memory (except when file is master file itself).
                    var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(Root.Model);
                    if ((masterPcgMemory != null) && (masterPcgMemory.FileName != Root.FileName))
                    {
                        // Iterate through bank IDs.
                        var masterBank =
                            masterPcgMemory.ProgramBanks.BankCollection.FirstOrDefault(bank => bank.Id == UsedProgramBank.Id);
                        if (masterBank != null)
                        {
                            var masterProgram = masterBank[UsedProgram.Index];
                            return masterProgram.Name;
                        }

                        // Program not present in master file either.
                        return "(unknown)";
                    }

                    // NO master file present.
                    return "(unknown)";
                }

                // Program available in PCG file.
                return UsedProgram.Name;
            }
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
                    //ColumnProgramName = ProgramName;
                    /*
                    if (ColumnProgramName == "(unknown)")
                    {
                        //ColumnCategory = "(unknown)";
                        //ColumnSubCategory = "(unknown)";
                    }
                    else
                    {
                        ColumnCategory = CategoryAsName;
                        ColumnSubCategory = SubCategoryAsName;
                    }
                     */
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
        private void OnPcgRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ReadingFinished": // Fall through
                case "Global": // Fall through
                case "":
                    if (((ICombi) (Parent.Parent)).IsLoaded && (UsedProgramBank != null))
                    {
                        RefillColumns();
                    }
                    break;

                //default:
                   // break;
            }
        }


        /// <summary>
        /// Returns the global of this PCG or if no global present, the Master PCG file's global.
        /// This code is a copy from Patch class.
        /// </summary>
        /// <returns></returns>
        private IGlobal FindGlobal()
        {
            var global = PcgRoot.Global;
            if (global == null)
            {
                // Find master PCG memory (except when file is master file itself).
                var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(Root.Model);
                if ((masterPcgMemory != null) && (masterPcgMemory.FileName != Root.FileName))
                {
                    global = masterPcgMemory.Global;
                }
            }
            return global;
        }


        /// <summary>
        /// 
        /// </summary>
        protected void RefillColumns()
        {
            // ColumnProgramName = UsedProgram.Name;
            //ColumnCategory = UsedProgram.CategoryAsName;
            //ColumnSubCategory = UsedProgram.SubCategoryAsName;

            RaisePropertyChanged(string.Empty, false);
            //RaisePropertyChanged("ColumnProgramName");
            if (UsedProgram != null) // Null while initializing
            {
                UsedProgram.RaisePropertyChanged(string.Empty, false);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        // ReSharper disable once UnusedMember.Global
        public void Update(string name)
        {
            switch (name)
            {
                case "All":
                    RefillColumns();
                    break;

                //default: No action needed.
            }
        }


        /// <summary>
        /// Returns true if the timbre has the drum category (actual string depends per workstation model,
        /// but since user categories are possible, 'drum' case insensitive should be part of the category name.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public bool HasDrumCategory
        {
            get
            {
                var usedProgram = UsedProgram;
                if (usedProgram != null)
                {
                    var category = usedProgram.GetParam(ParameterNames.ProgramParameterName.Category);
                    return ((category != null) && category.Value.ToString().ToUpper().Contains("DRUM"));
                }

                return false; // No program
            }
        }


        /// <summary>
        /// Not used (yet).
        /// </summary>
        public void SetParameters()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual int GetFixedParameterValue(FixedParameter.EType type)
        {
            throw new ApplicationException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public virtual void SetFixedParameterValue(FixedParameter.EType type, int value)
        {
            throw new ApplicationException();
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsLoaded
        {
            get { return ((ICombi) (Parent.Parent)).IsLoaded; }
            set { ((ICombi) (Parent.Parent)).IsLoaded = value; }
        }


        /// <summary>
        /// Not used yet; needed when copying timbres will be supported.
        /// </summary>
        public int ByteLength
        {
            get { return TimbresSize; }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
