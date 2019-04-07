using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Common.Mvvm;
using PcgTools.ClipBoard;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;

namespace PcgTools.ViewModels.Commands.PcgCommands
{
    /// <summary>
    /// 
    /// </summary>
    public class CopyPasteCommands
    {
        /// <summary>
        /// 
        /// </summary>
        IPcgClipBoard PcgClipBoard { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IPcgMemory SelectedPcgMemory { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool ProgramBanksSelected { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool CombiBanksSelected { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool SetListsSelected { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        bool DrumKitBanksSelected { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool DrumPatternBanksSelected { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        bool WaveSequenceBanksSelected { get; set; }


        /// <summary>
        /// All patches.
        /// </summary>
        bool AllPatchesSelected { get; set; }


        /// <summary>
        /// 
        /// </summary>
        PcgViewModel.ScopeSet SelectedScopeSet { get; set; }


        /// <summary>
        /// 
        /// </summary>
        static ObservableCollectionEx<IBank> Banks { get; set; }


        static ObservableCollectionEx<IPatch> Patches { get; set; }

        
        /// <summary>
        /// Cut or Copy for cut/copy/paste.
        /// </summary>
        /// <param name="pcgClipBoard"></param>
        /// <param name="setListsSelected"></param>
        /// <param name="waveSequenceBanksSelected"></param>
        /// <param name="allPatchesSelected"></param>
        /// <param name="banks"></param>
        /// <param name="patches"></param>
        /// <param name="cutPasteAction">Cut paste action selected; clears patches after copying (otherwise it is a 
        /// cut/copy/paste action)</param>
        /// <param name="selectedPcgMemory"></param>
        /// <param name="selectedScopeSet"></param>
        /// <param name="programBanksSelected"></param>
        /// <param name="combiBanksSelected"></param>
        /// <param name="drumKitBanksSelected"></param>
        /// /// <param name="drumPatternBanksSelected"></param>
        public void CopyPasteCopy(IPcgClipBoard pcgClipBoard, IPcgMemory selectedPcgMemory, PcgViewModel.ScopeSet selectedScopeSet,
            bool programBanksSelected, bool combiBanksSelected, bool setListsSelected, bool drumKitBanksSelected, 
            bool drumPatternBanksSelected, bool waveSequenceBanksSelected,
            bool allPatchesSelected,
            ObservableCollectionEx<IBank> banks, ObservableCollectionEx<IPatch> patches, bool cutPasteAction)
        {
            PcgClipBoard = pcgClipBoard;
            SelectedPcgMemory = selectedPcgMemory;
            SelectedScopeSet = selectedScopeSet;
            ProgramBanksSelected = programBanksSelected;
            CombiBanksSelected = combiBanksSelected;
            SetListsSelected = setListsSelected;
            DrumKitBanksSelected = drumKitBanksSelected;
            DrumPatternBanksSelected = drumPatternBanksSelected;
            WaveSequenceBanksSelected = waveSequenceBanksSelected;
            AllPatchesSelected = allPatchesSelected;
            Banks = banks;
            Patches = patches;

            CopyPasteInit(cutPasteAction);

            CopyPasteCopyPatches(cutPasteAction);

            PcgClipBoard.Memorize();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyPatches(bool cutPasteAction)
        {
            if (ProgramBanksSelected)
            {
                CopyPasteCopyPrograms(cutPasteAction);
            }
            else if (CombiBanksSelected)
            {
                CopyPasteCopyCombis(cutPasteAction);
            }
            else if (SetListsSelected)
            {
                CopyPasteCopySetLists(cutPasteAction);
            }
            else if (DrumKitBanksSelected)
            {
                CopyPasteCopyDrumKits(cutPasteAction);
            }
            else if (DrumPatternBanksSelected)
            {
                CopyPasteCopyDrumPatterns(cutPasteAction);
            }
            else if (WaveSequenceBanksSelected)
            {
                CopyPasteCopyWaveSequences(cutPasteAction);
            }
            else if (AllPatchesSelected)
            {
                // Copy programs.
                foreach (var program in Patches.Where(patch => patch.IsSelected && (patch is IProgram)))
                {
                    // Always copy programs.
                    PcgClipBoard.CopyProgramToClipBoard(program as IProgram, cutPasteAction);
                }

                // Copy combis.
                foreach (var combi in Patches.Where(
                    patch => patch.IsSelected && (patch is ICombi)).Where(
                        combi =>
                            cutPasteAction || Settings.Default.CopyPaste_CopyIncompleteCombis || ((ICombi) combi).IsCompleteInPcg))
                {
                    PcgClipBoard.CopyCombiToClipBoard(combi as ICombi, cutPasteAction);
                }

                // Copy set list slots.
                foreach (var setListSlot in Patches.Where(patch => patch.IsSelected && (patch is ISetListSlot)).Where(
                    (setListSlot => cutPasteAction ||
                                    Settings.Default.CopyPaste_CopyIncompleteSetListSlots ||
                                    ((ISetListSlot) setListSlot).IsCompleteInPcg)))
                {
                    PcgClipBoard.CopySetListSlotToClipBoard(setListSlot as ISetListSlot, cutPasteAction);
                }
                // Copy drum kits.
                foreach (var drumKit in Patches.Where(patch => patch.IsSelected && (patch is IDrumKit)))
                {
                    // Always copy drum kits.
                    PcgClipBoard.CopyDrumKitToClipBoard(drumKit as IDrumKit, cutPasteAction);
                }
                
                // Copy drum patterns.
                foreach (var drumPattern in Patches.Where(patch => patch.IsSelected && (patch is IDrumPattern)))
                {
                    // Always copy patterns.
                    PcgClipBoard.CopyDrumPatternToClipBoard(drumPattern as IDrumPattern, cutPasteAction);
                }
                
                // Copy wave sequences.
                foreach (var waveSequence in Patches.Where(patch => patch.IsSelected && (patch is IWaveSequence)))
                {
                    // Always copy wave sequences.
                    PcgClipBoard.CopyWaveSequenceToClipBoard(waveSequence as IWaveSequence, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteInit(bool cutPasteAction)
        {
            PcgClipBoard.Clear();

            if (!cutPasteAction)
            {
                PcgClipBoard.CutPasteSelected = false;
            }

            PcgClipBoard.CopyFileName = SelectedPcgMemory.FileName;
            PcgClipBoard.Model = SelectedPcgMemory.Model;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyPrograms(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var program in from IProgramBank bank in Banks.Where(bank => bank.IsSelected)
                    where bank.Type != BankType.EType.Gm
                    from IProgram program in bank.Patches
                    where bank.IsLoaded
                    // Also copy INIT programs (for copying single INIT programs)
                    select program)
                {
                    // Always copy programs.
                    PcgClipBoard.CopyProgramToClipBoard(program, cutPasteAction);
                }
            }
            else
            {
                foreach (var program in Patches.Where(patch => patch.IsSelected).Cast<IProgram>())
                {
                    // Always copy programs.
                    PcgClipBoard.CopyProgramToClipBoard(program, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyCombis(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var combi in (from ICombiBank bank in Banks.Where(bank => bank.IsSelected)
                    from ICombi combi in bank.Patches
                    where bank.IsLoaded
                    select combi).Where(combi => cutPasteAction ||
                                                 Settings.Default.CopyPaste_CopyIncompleteCombis || combi.IsCompleteInPcg))
                {
                    PcgClipBoard.CopyCombiToClipBoard(combi, cutPasteAction);
                }
            }
            else
            {
                foreach (var combi in Patches.Where(
                    patch => patch.IsSelected).Cast<ICombi>().Where(
                        combi => cutPasteAction || Settings.Default.CopyPaste_CopyIncompleteCombis || combi.IsCompleteInPcg))
                {
                    PcgClipBoard.CopyCombiToClipBoard(combi, cutPasteAction);
                }
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopySetLists(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var setListSlot in (from ISetList setList in Banks.Where(bank => bank.IsSelected)
                    from ISetListSlot setListSlot in setList.Patches
                    where setList.IsLoaded
                    select setListSlot).Where(setListSlot => cutPasteAction ||
                                                             Settings.Default.CopyPaste_CopyIncompleteSetListSlots ||
                                                             setListSlot.IsCompleteInPcg))
                {
                    PcgClipBoard.CopySetListSlotToClipBoard(setListSlot, cutPasteAction);
                }
            }
            else
            {
                foreach (var setListSlot in Patches.Where(patch => patch.IsSelected).Cast<ISetListSlot>().Where(
                    (setListSlot => cutPasteAction ||
                                    Settings.Default.CopyPaste_CopyIncompleteSetListSlots ||
                                    setListSlot.IsCompleteInPcg)))
                {
                    PcgClipBoard.CopySetListSlotToClipBoard(setListSlot, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyDrumKits(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var drumKit in from IDrumKitBank bank in Banks.Where(bank => bank.IsSelected)
                                        where bank.Type != BankType.EType.Gm
                                        from IDrumKit drumKit in bank.Patches
                                        where bank.IsLoaded
                                        // Also copy INIT drum kits (for copying single INIT programs)
                                        select drumKit)
                {
                    // Always copy drum kits.
                    PcgClipBoard.CopyDrumKitToClipBoard(drumKit, cutPasteAction);
                }
            }
            else
            {
                foreach (var drumKit in Patches.Where(patch => patch.IsSelected).Cast<IDrumKit>())
                {
                    // Always copy drum kits.
                    PcgClipBoard.CopyDrumKitToClipBoard(drumKit, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyDrumPatterns(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var drumPattern in from IDrumPatternBank bank in Banks.Where(bank => bank.IsSelected)
                                        where bank.Type != BankType.EType.Gm
                                        from IDrumPattern drumPattern in bank.Patches
                                        where bank.IsLoaded
                                        // Also copy INIT drum patterns (for copying single INIT programs)
                                        select drumPattern)
                {
                    // Always copy drum patterns.
                    PcgClipBoard.CopyDrumPatternToClipBoard(drumPattern, cutPasteAction);
                }
            }
            else
            {
                foreach (var drumPattern in Patches.Where(patch => patch.IsSelected).Cast<IDrumPattern>())
                {
                    // Always copy drum patterns.
                    PcgClipBoard.CopyDrumPatternToClipBoard(drumPattern, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutPasteAction"></param>
        private void CopyPasteCopyWaveSequences(bool cutPasteAction)
        {
            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                foreach (var waveSequence in from IWaveSequenceBank bank in Banks.Where(bank => bank.IsSelected)
                                        where bank.Type != BankType.EType.Gm
                                        from IWaveSequence waveSequence in bank.Patches
                                        where bank.IsLoaded
                                             // Also copy INIT wave sequences (for copying single INIT wave sequences)
                                             select waveSequence)
                {
                    // Always copy wave sequences.
                    PcgClipBoard.CopyWaveSequenceToClipBoard(waveSequence, cutPasteAction);
                }
            }
            else
            {
                foreach (var waveSequence in Patches.Where(patch => patch.IsSelected).Cast<IWaveSequence>())
                {
                    // Always copy wave sequences.
                    PcgClipBoard.CopyWaveSequenceToClipBoard(waveSequence, cutPasteAction);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgClipBoard"></param>
        /// <param name="selectedPcgMemory"></param>
        /// <param name="selectedScopeSet"></param>
        /// <param name="programBanksSelected"></param>
        /// <param name="combiBanksSelected"></param>
        /// <param name="setListsSelected"></param>
        /// <param name="waveSequenceBanksSelected"></param>
        /// <param name="allPatchesSelected"></param>
        /// <param name="banks"></param>
        /// <param name="patches"></param>
        /// <param name="drumKitsSelected"></param>
        /// <param name="drumPatternsSelected"></param>
        /// <returns></returns>
        public string CopyPastePaste(IPcgClipBoard pcgClipBoard, IPcgMemory selectedPcgMemory, PcgViewModel.ScopeSet selectedScopeSet,
            bool programBanksSelected, bool combiBanksSelected, bool setListsSelected, bool drumKitsSelected,
            bool drumPatternsSelected, bool waveSequenceBanksSelected,
            bool allPatchesSelected,
            ObservableCollectionEx<IBank> banks, ObservableCollectionEx<IPatch> patches)
        {
            PcgClipBoard = pcgClipBoard;
            SelectedPcgMemory = selectedPcgMemory;
            SelectedScopeSet = selectedScopeSet;
            ProgramBanksSelected = programBanksSelected;
            CombiBanksSelected = combiBanksSelected;
            SetListsSelected = setListsSelected;
            DrumKitBanksSelected = drumKitsSelected;
            DrumPatternBanksSelected = drumPatternsSelected;
            WaveSequenceBanksSelected = waveSequenceBanksSelected;
            AllPatchesSelected = allPatchesSelected;
            Banks = banks;
            Patches = patches;
            
            PasteDuplicates();

            var infoText = PastePatches();

            if (!PcgClipBoard.CutPasteSelected)
            {
                PcgClipBoard.FixPasteReferencesAfterCopyPaste();
            }

            if (PcgClipBoard.IsPastingFinished)
            {
                PastingFinished();
            }

            return infoText;
        }


        /// <summary>
        /// When copy paste selected, and duplicates have not been pasted, paste them.
        /// </summary>
        private void PasteDuplicates()
        {
            if (!PcgClipBoard.CutPasteSelected && !PcgClipBoard.PasteDuplicatesExecuted)
            {
                PasteDuplicatePatches();
            }
            PcgClipBoard.PasteDuplicatesExecuted = true;
        }


        /// <summary>
        /// Paste duplicates depending on the settings.
        /// If copied to the same PCG, do not paste the type which is copied (e.g. when combis are copied, do not paste them, 
        /// independent of the settings).
        /// </summary>
        void PasteDuplicatePatches()
        {
            FindDuplicatePrograms();
            FindDuplicateCombis();
            FindDuplicateSetListSlots();
            FindDuplicateDrumKits();
            FindDuplicateDrumPatterns();
            FindDuplicateWaveSequences();

            PcgClipBoard.PasteDuplicatesExecuted = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicatePrograms()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicatePrograms) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.Programs)))
            {
                for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
                {
                    FindDuplicatesOfType(PcgClipBoard.Programs[index].CopiedPatches, SelectedPcgMemory.ProgramBanks);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicateCombis()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicateCombis) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.Combis)))
            {
                FindDuplicatesOfType(PcgClipBoard.Combis.CopiedPatches, SelectedPcgMemory.CombiBanks);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicateDrumKits()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicateDrumKits) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.DrumKits)))
            {
                FindDuplicatesOfType(PcgClipBoard.DrumKits.CopiedPatches, SelectedPcgMemory.DrumKitBanks);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicateDrumPatterns()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicateDrumPatterns) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.DrumPatterns)))
            {
                FindDuplicatesOfType(PcgClipBoard.DrumPatterns.CopiedPatches, SelectedPcgMemory.DrumPatternBanks);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicateWaveSequences()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicateWaveSequences) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.WaveSequences)))
            {
                FindDuplicatesOfType(PcgClipBoard.WaveSequences.CopiedPatches, SelectedPcgMemory.WaveSequenceBanks);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDuplicateSetListSlots()
        {
            if ((!Settings.Default.CopyPaste_PasteDuplicateSetListSlots) ||
                ((PcgClipBoard.CopyFileName == SelectedPcgMemory.FileName) &&
                 (PcgClipBoard.SelectedCopyType != ClipBoard.PcgClipBoard.CopyType.SetListSlots)))
            {
                FindDuplicatesOfType(PcgClipBoard.SetListSlots.CopiedPatches, SelectedPcgMemory.SetLists);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void PastingFinished()
        {
            var count = 0;
            for (var index = 0; index < (int)ProgramBank.SynthesisType.Last; index++)
            {
                count += PcgClipBoard.Programs[index].CopiedPatches.Count;
            }

            if (!((PcgClipBoard.SetListSlots.CopiedPatches.Count == 0) &&
                  (PcgClipBoard.Combis.CopiedPatches.Count == 0) &&
                  (PcgClipBoard.DrumKits.CopiedPatches.Count == 0) &&
                  (PcgClipBoard.DrumPatterns.CopiedPatches.Count == 0) &&
                  (PcgClipBoard.WaveSequences.CopiedPatches.Count == 0) &&
                  (count == 1))) //IMPR: Why count == 1 and not 0? (needs the last to be copied ?)
            {
                PcgClipBoard.Clear();
            }
        }


        /// <summary>
        /// Check for duplicates.
        /// </summary>
        /// <param name="clipBoardPatches"></param>
        /// <param name="banks"></param>
        void FindDuplicatesOfType(ObservableCollection<IClipBoardPatch> clipBoardPatches, IBanks banks)
        {
            foreach (var clipBoardPatch in clipBoardPatches)
            {
                foreach (var bank in banks.BankCollection)
                {
                    if (ShouldBankBeSearched(clipBoardPatches, bank)) 
                    {
                        if (CheckForDuplicate(clipBoardPatch, bank))
                        {
                            break; // Break outside program bank iteration and start with next clipboard patch
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Only search filled banks. Program banks should be non GM and from same synthesis type.
        /// </summary>
        /// <param name="clipBoardPatches"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        private bool ShouldBankBeSearched(ObservableCollection<IClipBoardPatch> clipBoardPatches, IBank bank)
        {
            return bank.IsFilled && 
                (bank.Type != BankType.EType.Gm) && // Only can be false for program banks
                   (!(bank is IProgramBank) ||
                    Equals(clipBoardPatches,
                       PcgClipBoard.Programs[(int) (((IProgramBank) bank).BankSynthesisType)].CopiedPatches));
        }


        /// <summary>
        /// 1 First search for bytewise duplicate patch, if found -> don't paste.
        /// 2 Than and if 'Equally Named Patches' is selected, search for an equally named patch and reference to that.
        /// 3 Otherwise, if 'Like-Named Pathces' is selected, remove the ignore characters and find a like-named patch and
        ///   reference to that.
        /// For all searches above, if a patch is found on the same location (a), prefer that, otherwise find the first occurence (b).
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <returns>True if a duplicate is found.</returns>
        private bool CheckForDuplicate(IClipBoardPatch clipBoardPatch, IBank bank)
        {
            Debug.Assert(!(clipBoardPatch is IClipBoardSetListSlot));

            // If the source patch is present in the source PCG file, check if it is present in the target PCG.
            if (clipBoardPatch.Data.Length != 0)
            {
                IPatch sameLocationPatch;
                if (SearchByteWiseEqualPatchSameLocation(clipBoardPatch, bank, out sameLocationPatch))
                {
                    return true;
                }

                if (SearchByteWiseEqualPatchFirstOccurence(clipBoardPatch, bank))
                {
                    return true;
                }

                if (SearchIdenticalNamePatch(clipBoardPatch, bank, sameLocationPatch))
                {
                    return true;
                }

                if (SearchLikeNamedPatch(clipBoardPatch, bank, sameLocationPatch))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 1a. Search for a bytewise equal patch, on same location.
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <param name="sameLocationPatch"></param>
        /// <returns></returns>
        private bool SearchByteWiseEqualPatchSameLocation(IClipBoardPatch clipBoardPatch, IBank bank,
            out IPatch sameLocationPatch)
        {
            sameLocationPatch = GetPatchOnSameLocation(clipBoardPatch, bank);
            if (sameLocationPatch != null)
            {
                if (sameLocationPatch.CalcByteDifferences(clipBoardPatch, true, 1) == 0)
                {
                    clipBoardPatch.PasteDestination = sameLocationPatch;
                    PcgClipBoard.ProtectedPatches.Add(sameLocationPatch);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 1b. Search for a bytewise equal patch, on first occurence.
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        private bool SearchByteWiseEqualPatchFirstOccurence(IClipBoardPatch clipBoardPatch, IBank bank)
        {
            foreach (var patch in bank.Patches.Where(patch => patch.CalcByteDifferences(clipBoardPatch, true, 1) == 0))
            {
                clipBoardPatch.PasteDestination = patch;
                PcgClipBoard.ProtectedPatches.Add(patch);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 2a. Search for an idental named patch if Equally Named Patches is selected, on same location.
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <param name="sameLocationPatch"></param>
        /// <returns></returns>
        private bool SearchIdenticalNamePatch(IClipBoardPatch clipBoardPatch, IBank bank, IPatch sameLocationPatch)
        {
            if (Settings.Default.CopyPaste_PatchDuplicationName == (int) CopyPaste.PatchDuplication.EqualNames)
            {
                if (sameLocationPatch != null)
                {
                    if (sameLocationPatch.Name.Trim() == clipBoardPatch.OriginalLocation.Name.Trim())
                    {
                        clipBoardPatch.PasteDestination = sameLocationPatch;
                        PcgClipBoard.ProtectedPatches.Add(sameLocationPatch);
                        return true;
                    }
                }

                // 2b. Search for an identical name patch, on first occurence.
                foreach (var patch in bank.Patches.Where(
                    patch => patch.Name.Trim() == clipBoardPatch.OriginalLocation.Name.Trim()))
                {
                    clipBoardPatch.PasteDestination = patch;
                    PcgClipBoard.ProtectedPatches.Add(patch);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 3b. Search for a like named patch if Like Named Patches is selected, on same location.
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <param name="sameLocationPatch"></param>
        /// <returns></returns>
        private bool SearchLikeNamedPatch(IClipBoardPatch clipBoardPatch, IBank bank, IPatch sameLocationPatch)
        {
            if (Settings.Default.CopyPaste_PatchDuplicationName == (int) CopyPaste.PatchDuplication.LikeNamedNames)
            {
                if (sameLocationPatch != null)
                {
                    if (sameLocationPatch.IsNameLike(clipBoardPatch.OriginalLocation.Name))
                    {
                        clipBoardPatch.PasteDestination = sameLocationPatch;
                        PcgClipBoard.ProtectedPatches.Add(sameLocationPatch);
                        return true;
                    }
                }

                // 2b. Search for an identical name patch, on first occurence.
                foreach (var patch in bank.Patches.Where(
                    patch => patch.IsNameLike(clipBoardPatch.OriginalLocation.Name)))
                {
                    clipBoardPatch.PasteDestination = patch;
                    PcgClipBoard.ProtectedPatches.Add(patch);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the patch on the same location (if existing).
        /// </summary>
        /// <param name="clipBoardPatch"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        private static IPatch GetPatchOnSameLocation(IClipBoardPatch clipBoardPatch, IBank bank)
        {
            // Check if banks are equal.
            if (((IBank)(clipBoardPatch.OriginalLocation.Parent)).Id != (bank).Id)
            {
                return null;
            }

            // Get same index.
            var index = clipBoardPatch.OriginalLocation.Index;

            return bank.Patches.Count > index ? bank[index] : null;
        }


        /// <summary>
        /// Pre: the PCG window's SelectedPcgMemory is compatible with the clipboard patches and has been pasted before
        ///  (or is first paste).
        /// Returns error text if applicable.
        /// </summary>
        string PastePatches()
        {
            bool[] atLeastOnePatchIsPasted = {false};

            if (SelectedScopeSet == PcgViewModel.ScopeSet.Banks)
            {
                // banks are selected for pasting select the banks where the copied patches can be paste in
                string banksSelected; 
                if (PastePatchesBank(atLeastOnePatchIsPasted,
                    out banksSelected))
                {
                    return banksSelected;
                }
            }
            else
            {
                PastePatchesNotFromBank(atLeastOnePatchIsPasted);
            }

            // If not at least one patch is pasted, show possible reason(s).
            if (!atLeastOnePatchIsPasted[0])
            {
                return Strings.PasteWarning;
            }

            return string.Empty; // No info string
        }


        /// <summary>
        /// Copy clip board patches to selected patches.
        /// </summary>
        /// <param name="atLeastOnePatchIsPasted"></param>
        private void PastePatchesNotFromBank(bool[] atLeastOnePatchIsPasted)
        {
            // ReSharper disable once UnusedVariable
            foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(
                patch => !PasteToSelectedPatch(patch, ref atLeastOnePatchIsPasted[0])))
            {
                // ReSharper disable once RedundantJumpStatement
                break;
            }

            // Continue if extend selection is set and only one patch selected.
            if (Settings.Default.CopyPaste_AutoExtendedSinglePatchSelectionPaste &&
                (Patches.Count(patch => patch.IsSelected) == 1))
            {
                for (var index = Patches.IndexOf(Patches.First(patch => patch.IsSelected)); index < Patches.Count; index++)
                {
                    if (!PasteToSelectedPatch(Patches.ToArray()[index], ref atLeastOnePatchIsPasted[0]))
                    {
                        break;
                    }
                }
            }
            else
            {
                // ReSharper disable once UnusedVariable
                foreach (var patch in Patches.Where(patch => patch.IsSelected).Where(
                    patch => !PasteToSelectedPatch(patch, ref atLeastOnePatchIsPasted[0])))
                {
                    // ReSharper disable once RedundantJumpStatement
                    break;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="atLeastOnePatchIsPasted"></param>
        /// <param name="noBanksSelected">noBanksAreSelectedForPastingSelectTheBankSWhereTheCopiedPatchesCanBePastedIn</param>
        /// <returns></returns>
        private bool PastePatchesBank(IList<bool> atLeastOnePatchIsPasted,
            out string noBanksSelected)
        {
            // Set list(s) selected.
            if (!CheckSelectedBanksForPasting())
            {
                {
                    noBanksSelected =
                        "No banks are selected for pasting. Select the bank(s) where the copied patches can be pasted in.";
                    return true;
                }
            }

            // Copy clip board patches to selected patches.
            foreach (var bank in Banks.Where(bank => bank.IsSelected))
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse, needed for preventing modified closure warning
                var pasted = atLeastOnePatchIsPasted[0];
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
                var cont = bank.Patches.All(patch => PasteToSelectedPatch(patch, ref pasted));
                atLeastOnePatchIsPasted[0] = pasted;
                if (!cont)
                {
                    break;
                }
            }

            noBanksSelected = "";
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="atLeastOnePatchIsPasted">Or-s the value with True if the patch is pasted</param>
        /// <returns></returns>
        bool PasteToSelectedPatch(IPatch patch, ref bool atLeastOnePatchIsPasted)
        {
            if ((patch.IsEmptyOrInit || (PcgClipBoard.PastePcgMemory == SelectedPcgMemory)) &&
                !PcgClipBoard.ProtectedPatches.Contains(patch))
            {
                IClipBoardPatches clipBoardPatches = null;
                var overwriteAllowed = CheckOverwriteallowedForPatches(patch, ref clipBoardPatches);

                IClipBoardPatch clipBoardPatchToPaste = null;
                if (clipBoardPatches != null) // Can happen when not allowed to overwrite
                {
                    clipBoardPatchToPaste = PcgClipBoard.GetFirstPatchToPaste(clipBoardPatches.CopiedPatches);
                }

                if (clipBoardPatchToPaste == null)
                {
                    if (overwriteAllowed)
                    {
                        return false; // Finished pasting (this kind of patches)
                    }
                }
                else
                {
                    atLeastOnePatchIsPasted = true;
                    PcgClipBoard.PastePatch(clipBoardPatchToPaste, patch);
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <returns></returns>
        private bool CheckOverwriteallowedForPatches(IPatch patch, ref IClipBoardPatches clipBoardPatches)
        {
            bool overwriteAllowed;

            var patch1 = patch as IProgram;
            if (patch1 != null)
            {
                overwriteAllowed = CheckOverwriteAllowedForPrograms(out clipBoardPatches, patch1, false);
            }
            else if (patch is Combi)
            {
                overwriteAllowed = CheckOverwriteAllowedForCombis(patch, out clipBoardPatches, false);
            }
            else if (patch is SetListSlot)
            {
                overwriteAllowed = CheckOverwriteAllowedForSetListSlots(patch, out clipBoardPatches, false);
            }
            else if (patch is DrumKit)
            {
                overwriteAllowed = CheckOverwriteAllowedForDrumKits(patch, out clipBoardPatches, false);
            }
            else if (patch is DrumPattern)
            {
                overwriteAllowed = CheckOverwriteAllowedForDrumPatterns(patch, out clipBoardPatches, false);
            }
            else if (patch is WaveSequence)
            {
                overwriteAllowed = CheckOverwriteAllowedForWaveSequences(patch, out clipBoardPatches, false);
            }
            else
            {
                throw new ApplicationException("Illegal type");
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipBoardPatches"></param>
        /// <param name="patch1"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForPrograms(out IClipBoardPatches clipBoardPatches, IProgram patch1,
            bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch1.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledPrograms || patch1.IsEmptyOrInit))
            {
                var program = patch1;
                clipBoardPatches = PcgClipBoard.Programs[(int) (((IProgramBank) (program.Parent)).BankSynthesisType)];
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForCombis(IPatch patch, out IClipBoardPatches clipBoardPatches, bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledCombis || patch.IsEmptyOrInit))
            {
                clipBoardPatches = PcgClipBoard.Combis;
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForSetListSlots(IPatch patch, out IClipBoardPatches clipBoardPatches,
            bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledSetListSlots || patch.IsEmptyOrInit))
            {
                clipBoardPatches = PcgClipBoard.SetListSlots;
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForDrumKits(IPatch patch, out IClipBoardPatches clipBoardPatches, bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledDrumKits || patch.IsEmptyOrInit))
            {
                clipBoardPatches = PcgClipBoard.DrumKits;
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForDrumPatterns(
            IPatch patch, out IClipBoardPatches clipBoardPatches, bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledDrumPatterns || patch.IsEmptyOrInit))
            {
                clipBoardPatches = PcgClipBoard.DrumPatterns;
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="clipBoardPatches"></param>
        /// <param name="overwriteAllowed"></param>
        /// <returns></returns>
        private bool CheckOverwriteAllowedForWaveSequences(IPatch patch, out IClipBoardPatches clipBoardPatches, bool overwriteAllowed)
        {
            clipBoardPatches = null;
            if (((IBank)(patch.Parent)).IsWritable &&
                (Settings.Default.CopyPaste_OverwriteFilledWaveSequences || patch.IsEmptyOrInit))
            {
                clipBoardPatches = PcgClipBoard.WaveSequences;
                overwriteAllowed = true;
            }
            return overwriteAllowed;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static bool CheckSelectedBanksForPasting()
        {
            return (Banks.Count(bank => bank.IsSelected) > 0);
        }
    }
}
