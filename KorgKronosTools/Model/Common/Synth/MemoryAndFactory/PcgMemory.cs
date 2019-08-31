// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using PcgTools.ClipBoard;
using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Model.KronosSpecific.Synth;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PcgMemory : Memory, IPcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        protected Models.EModelType ModelType { get; private set; }


        /// <summary>
        /// The values come from Sysex functions.
        /// </summary>
        public enum ContentType
        {
            All = 0x50,

            AllPrograms = 0x4C,
            ProgramBank = 0x80, // No Sysex function
            CurrentProgram = 0x40,

            AllCombis = 0x4D,
            CombiBank = 0x90, // No Sysex function
            CurrentCombi = 0x49,

            MultiSound = 0x44,

            AllDrumSound = 0x47,
            Drums = 0x52,
            // ReSharper disable once UnusedMember.Global
            DrumKitAndMultiSoundParameterChange = 0x53,

            ArpeggioPattern = 0x69, // Z1, ...
            CurrentArpeggioPattern = 0x6B, // Z1, ...

            AllSequence = 0x48,
            CurrentSequence = 0xa0, // No Sysex function

            Global = 0x51,

            ModeChange = 0x4E, // Skip
            // ReSharper disable once UnusedMember.Global
            ParameterChange = 0x41,

            // Unused:
            // All requests
            // WriteCompleted       // 0x21
            // WriteError           // 0x22
            // DataLoadCompleted    // 0x23
            // DataLoadError        // 0x24
            // DataFormatError      // 0x26
        }


        /// <summary>
        /// 
        /// </summary>
        public ContentType ContentTypeType { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        protected PcgMemory(string fileName, Models.EModelType modelType)
        {
            OriginalFileName = fileName;
            FileName = fileName;
            AssignedClearProgram = null;
            ModelType = modelType;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Fill()
        {
            if (CombiBanks != null)
            {
                CombiBanks.Fill();
            }

            ProgramBanks.Fill();
            var firstProgramBank = (IProgramBank) ProgramBanks[0];

            AssignedClearProgram = (IProgram) (firstProgramBank[0]);

            if (SetLists != null)
            {
                SetLists.Fill();
            }

            if (WaveSequenceBanks != null)
            {
                WaveSequenceBanks.Fill();
            }

            if (DrumKitBanks != null)
            {
                DrumKitBanks.Fill();
            }

            if (DrumPatternBanks != null)
            {
                DrumPatternBanks.Fill();
            }

            PcgChecksumType = ChecksumType.None;
            Chunks = new Chunks();
        }


        /// <summary>
        /// 
        /// </summary>
        public IChunks Chunks { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public enum ChecksumType
        {
            None,
            Kronos1516,
            Kronos2XOr3X,
            Krome,
            KromeEx,
            Kross,
            Kross2,
            M3,
            MicroStation
        }


        /// <summary>
        /// 
        /// </summary>
        public ChecksumType PcgChecksumType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IProgram _assignedClearProgram;


        public IProgram AssignedClearProgram
        {
            get { return _assignedClearProgram; }
            set
            {
                _assignedClearProgram = value;
                OnPropertyChanged("AssignedClearProgram");
            }
        }


        /// <summary>
        /// </summary>
        /// /// <param name="saveAs">Must be true when the Save To function is used</param>
        /// <param name="saveToFile">Must always be true, except for unit tests</param>
        public override void SaveFile(bool saveAs, bool saveToFile)
        {
            UpdateSdb1Chunk();

            if (PcgChecksumType != ChecksumType.None)
            {
                FixChecksumValues(PcgChecksumType);
            }

            // Save file.
            try
            {
                if (saveToFile)
                {
                    System.IO.File.WriteAllBytes(FileName, Content);
                }
                IsDirty = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Strings.PcgTools, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // If the file name has changed and the settings are set to automatically rename a file, rename to file.
            if (IsDeleteOriginalFileNeeded(saveAs, saveToFile))
            {
                DeleteOriginalFile();
            }

            if (saveAs)
            {
                OriginalFileName = FileName;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveAs"></param>
        /// <param name="saveToFile"></param>
        /// <returns></returns>
        private bool IsDeleteOriginalFileNeeded(bool saveAs, bool saveToFile)
        {
            return saveToFile &&
                   !saveAs && (OriginalFileName != FileName) && Settings.Default.Edit_RenameFileWhenPatchNameChanges;
        }


        /// <summary>
        /// Delete original file.
        /// </summary>
        private void DeleteOriginalFile()
        {
            try
            {
                System.IO.File.Delete(OriginalFileName);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Strings.PcgTools, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IProgramBanks ProgramBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountSampledPrograms
        {
            get
            {
                return ProgramBanks == null
                    ? 0
                    : ProgramBanks.BankCollection.Where(
                      bank => !((IProgramBank) bank).IsModeled && bank.IsWritable).Sum(bank => bank.NrOfPatches);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountModeledPrograms
        {
            get
            {
                return ProgramBanks == null
                    ? 0
                    : ProgramBanks.BankCollection.Where(
                      bank => ((IProgramBank) bank).IsModeled && bank.IsWritable).Sum(bank => bank.NrOfPatches);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public ICombiBanks CombiBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountCombis
        {
            get { return CombiBanks == null
                    ? 0
                    : CombiBanks.BankCollection.Where(bank => bank.IsWritable).Sum(bank => bank.NrOfPatches); }
        }


        /// <summary>
        /// 
        /// </summary>
        public ISetLists SetLists { get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountSetListSlots
        {
            get { return SetLists == null
                    ? 0
                    : SetLists.BankCollection.Where(bank => bank.IsWritable).Sum(bank => bank.NrOfPatches); }
        }


        /// <summary>
        /// 
        /// </summary>
        public IWaveSequenceBanks WaveSequenceBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountWaveSequences
        {
            get
            {
                return WaveSequenceBanks == null
                    ? 0
                    : WaveSequenceBanks.BankCollection.Where(bank => bank.IsWritable).Sum(bank => bank.NrOfPatches);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IDrumKitBanks DrumKitBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public int CountDrumKits
        {
            get { return DrumKitBanks == null
                    ? 0
                    : DrumKitBanks.BankCollection.Where(bank => bank.IsWritable).Sum(bank => bank.NrOfPatches); }
        }


        /// <summary>
        /// 
        /// </summary>
        public IDrumPatternBanks DrumPatternBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int CountDrumPatterns
        {
            get { return DrumPatternBanks == null 
                    ? 0
                    : DrumPatternBanks.BankCollection.Where(bank => bank.IsWritable).Sum(bank => bank.NrOfPatches); }
        }


        /// <summary>
        /// 
        /// </summary>
        private IGlobal _global;


        /// <summary>
        /// 
        /// </summary>
        public IGlobal Global
        {
            get { return _global; }
            set
            {
                if (_global != value)
                {
                    _global = value;
                    OnPropertyChanged("Global");
                }
            }
        }


        // INavigable

        /// <summary>
        /// 
        /// </summary>
        public PcgMemory PcgRoot => this;


        /// <summary>
        /// Fix checksum values for models using checksums. Default is to do nothing.
        /// </summary>
        protected virtual void FixChecksumValues(ChecksumType checksumType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual bool HasProgramCategories => true;


        /// <summary>
        /// 
        /// </summary>
        public virtual bool HasCombiCategories => true;


        /// <summary>
        /// 
        /// </summary>
        public abstract bool HasSubCategories { get; }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public abstract int NumberOfCategories { get; }


        // ReSharper disable once UnusedMember.Global
        public abstract int NumberOfSubCategories { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="otherPatch"></param>
        public virtual void SwapPatch(IPatch patch, IPatch otherPatch)
        {
            if (patch == otherPatch)
            {
                return;
            }

            // Swap PRG1 content.
            Util.SwapBytes(this, Content, patch.ByteOffset, Content, otherPatch.ByteOffset, patch.ByteLength);

            // Swap PRG2 content (only used for Kronos, OS1.5/1.6).
            if (patch.Root.Model.OsVersion == Models.EOsVersion.EOsVersionKronos15_16)
            {
                if (patch is KronosProgram)
                {
                    ((KronosProgramBanks) (PcgRoot.ProgramBanks)).SwapPbk2Content(patch, otherPatch);
                }
                else if (patch is KronosCombi)
                {
                    ((KronosCombiBanks) (PcgRoot.CombiBanks)).SwapCbk2Content(patch, otherPatch);
                }
                else
                {
                    var slot = patch as KronosSetListSlot;
                    if (slot != null)
                    {
                        slot.SwapOs1516Data((KronosSetListSlot) otherPatch);
                    }
                }
            }

            patch.RaisePropertyChanged(string.Empty, false);
            if (patch is Program)
            {
                (patch as Program).Update("All");
            }

            otherPatch.RaisePropertyChanged(string.Empty, false);
            if (otherPatch is Program)
            {
                (otherPatch as Program).Update("All");
            }
        }


        /// <summary>
        /// Copy a patch to the clipboard.
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="patch"></param>
        public virtual void CopyPatch(IClipBoardPatch patchToPaste, IPatch patch)
        {
            Util.CopyBytes(this, patchToPaste.Data, patch.Root.Content, patch.ByteOffset, patch.ByteLength);
            patch.RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// Copy a patch to the clipboard.
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="patch"></param>
        public virtual void CopyPatch(IPatch patchToPaste, IPatch patch)
        {
            Debug.Assert(patchToPaste.GetType() == patch.GetType());
            Debug.Assert(patchToPaste.ByteLength == patch.ByteLength);
            Util.CopyBytes(this, patchToPaste.ByteOffset, patch.Root.Content, patch.ByteOffset, patch.ByteLength);
            patch.RaisePropertyChanged(string.Empty, false);
        }


        /// <summary>
        /// Returns true if the following items are present:
        /// - All program banks (except GM which is not writable)
        /// - All combi banks
        /// - Global section.
        /// Set lists are not necessary (no references needed to master file.
        /// </summary>
        public bool AreAllNeededProgramsCombisAndGlobalPresent => ((Global != null) &&
                                                                   AreAllNeededProgramBanksPresent &&
                                                                   AreAllNeededCombiBanksPresent);


        /// <summary>
        /// 
        /// </summary>
        protected virtual bool AreAllNeededProgramBanksPresent
        {
            get
            {
                return ((ProgramBanks == null) ||
                        (ProgramBanks.BankCollection.All(bank => !bank.IsWritable || (bank.ByteOffset != 0))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual bool AreAllNeededCombiBanksPresent
        {
            get
            {
                return ((CombiBanks == null) ||
                        (CombiBanks.BankCollection.All(bank => !bank.IsWritable || (bank.ByteOffset != 0))));
            }
        }


        /// <summary>
        /// Stores set list slot names and set list names for used in the Disk mode browser on the synth itself.
        /// </summary>
        public int Sdb1Index { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private void UpdateSdb1Chunk()
        {
            if ((SetLists != null) && (Sdb1Index != 0)) // If there is no SLS1 chuck there are no set lists present
            {
                foreach (var setList in SetLists.BankCollection)
                {
                    // Update set list name.
                    var sdb1Base = Sdb1Index + 20 + Convert.ToInt16(setList.Id)*0xE1C;
                    Util.SetChars(PcgRoot, Root.Content, sdb1Base, setList.MaxNameLength, setList.Name);

                    // Update set list slot names.
                    foreach (var slot in setList.Patches)
                    {
                        Util.SetChars(
                            PcgRoot, Root.Content, sdb1Base + 28 + 28*slot.Index, slot.MaxNameLength, slot.Name);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryName => Strings.Category;


        /// <summary>
        /// 
        /// </summary>
        public virtual string SubCategoryName => Strings.SubCategory;


        /// <summary>
        /// Returns true if the memory CAN only contain one patch. 
        /// Used for generating banks before the content of the file is actually read.
        /// </summary>
        public bool CanContainOnlyOnePatch => ((ContentTypeType == ContentType.CurrentProgram) ||
                                               (ContentTypeType == ContentType.CurrentCombi) ||
                                               (ContentTypeType == ContentType.CurrentSequence) ||
                                               (ContentTypeType == ContentType.CurrentArpeggioPattern));


        /// <summary>
        /// Returns true if the number of programs/combis in all banks is 1 (also count empty patches).
        /// </summary>
        public bool HasOnlyOnePatch
        {
            get
            {
                var programs = ProgramBanks == null ? 0 : ProgramBanks.CountWritablePatches;
                var combis = CombiBanks == null ? 0 : CombiBanks.CountWritablePatches;

                return (programs + combis == 1);
            }
        }


        /// <summary>
        /// Returns the name of the only patch (only if there is a single patch).
        /// </summary>
        public string NameOfOnlyPatch
        {
            get
            {
                //Debug.Assert(HasOnlyOnePatch);

                var foundPatch = FindOnlyPatch(ProgramBanks) ?? FindOnlyPatch(CombiBanks);
                Debug.Assert(foundPatch != null, "No patch found");
                return foundPatch.Name;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <returns></returns>
        private IPatch FindOnlyPatch(IBanks banks)
        {
            if (banks != null)
            {
                return banks.BankCollection.SelectMany(
                    bank => bank.Patches.Where(patch => ((IBank) (patch.Parent)).IsLoaded)).FirstOrDefault();
            }

            return null;
        }


        /// <summary>
        /// Some single program files do not have their program name filled in at the start of the program
        /// contents, but only as file name. This method copies the file name to the correct location.
        /// </summary>
        public void SynchronizeProgramName()
        {
            var firstProgram = (Program) ((ProgramBank) ProgramBanks[0])[0];
            if (firstProgram.Name == string.Empty)
            {
                var fileName = Path.GetFileNameWithoutExtension(FileName);
                if (fileName != null)
                {
                    firstProgram.Name = fileName.Substring(0, Math.Min(fileName.Length, firstProgram.MaxNameLength));
                }
            }
        }


        /// <summary>
        /// Some single combi files do not have their combi name filled in at the start of the combi
        /// contents, but only as file name. This method copies the file name to the correct location.
        /// </summary>
        public void SynchronizeCombiName()
        {
            var firstCombi = (Combi) ((CombiBank) CombiBanks[0])[0];
            if (firstCombi.Name == string.Empty)
            {
                var fileName = Path.GetFileNameWithoutExtension(FileName);
                if (fileName != null)
                {
                    firstCombi.Name = fileName.Substring(0, Math.Min(fileName.Length, firstCombi.MaxNameLength));
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void SetParameters()
        {
            SetParameters(ProgramBanks);
            SetParameters(CombiBanks);
            SetParameters(SetLists);
            SetParameters(DrumKitBanks);
            SetParameters(DrumPatternBanks);
            SetParameters(WaveSequenceBanks);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        private static void SetParameters(IBanks banks)
        {
            if (banks != null)
            {
                foreach (var bank in banks.BankCollection)
                {
                    bank.SetParameters();
                }
            }
        }


        /*
        /// <summary>
        /// Updates number of references of programs/combis.
        /// </summary>
        public void UpdateNumberOfReferences()
        {
            if (PcgRoot.CombiBanks != null)
            {
                foreach (var timbre in from bank in PcgRoot.CombiBanks.BankCollection
                                       where bank.IsLoaded
                    from patch in bank.Patches.Cast<ICombi>()
                    where !patch.IsEmptyOrInit
                    from timbre in patch.Timbres.TimbresCollection
                    select timbre)
                {
                    timbre.UsedProgram.NumberOfReferences++;
                }
            }

            if (PcgRoot.SetLists != null)
            {
                foreach (var patch in from bank in PcgRoot.SetLists.BankCollection
                                      where bank.IsLoaded
                    from ISetListSlot patch in bank.Patches
                    where patch.UsedPatch is IReferencable
                    select patch)
                {
                    (patch.UsedPatch as IReferencable).NumberOfReferences++;
                }
            }
        }
         */


        /// <summary>
        /// Select first indices of each bank type
        /// </summary>
        public void SelectFirstBanks()
        {
            if ((CombiBanks != null) && (CombiBanks.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(CombiBanks.BankCollection);
            }

            if ((ProgramBanks != null) && (ProgramBanks.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(ProgramBanks.BankCollection);
            }

            if ((SetLists != null) && (SetLists.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(SetLists.BankCollection);
            }

            if ((DrumKitBanks != null) && (DrumKitBanks.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(DrumKitBanks.BankCollection);
            }

            if ((DrumPatternBanks != null) && (DrumPatternBanks.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(DrumPatternBanks.BankCollection);
            }

            if ((WaveSequenceBanks != null) && (WaveSequenceBanks.BankCollection.Count > 0))
            {
                SelectFirstIfLoaded(WaveSequenceBanks.BankCollection);
            }
        }


        /// <summary>
        /// If there is a bank loaded, select first.
        /// </summary>
        /// <param name="banks"></param>
        private void SelectFirstIfLoaded(IEnumerable<IBank> banks)
        {
            var firstLoaded = banks.FirstOrDefault(bank => bank.IsLoaded);
            if (firstLoaded != null)
            {
                firstLoaded.IsSelected = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programRawBankIndex"></param>
        /// <param name="programRawIndex"></param>
        /// <returns></returns>
        public virtual IProgram GetPatchByRawIndices(
            int programRawBankIndex,
            int programRawIndex)
        {
            throw new ApplicationException("Not supported");
        }
    }
}
