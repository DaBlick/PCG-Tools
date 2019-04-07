// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.Properties;

namespace PcgTools.ClipBoard
{
    /// <summary>
    /// 
    /// </summary>
    public class PcgClipBoard : IPcgClipBoard
    {
        /// <summary>
        /// 
        /// </summary>
        public IModel Model { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string OsVersion { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum CopyType
        {
            Programs,
            Combis,
            SetListSlots,
            DrumKits,
            DrumPatterns,
            WaveSequences
        } 


        /// <summary>
        /// 
        /// </summary>
        public CopyType SelectedCopyType
        {
            get
            {
                return SetListSlots.CopiedPatches.Count > 0
                    ? CopyType.SetListSlots
                    : Combis.CopiedPatches.Count > 0
                        ? CopyType.Combis
                        : Programs.Any(list => list.CopiedPatches.Count > 0) // List per synthesis type
                            ? CopyType.Programs
                            : DrumKits.CopiedPatches.Count > 0
                                ? CopyType.DrumKits
                                : DrumPatterns.CopiedPatches.Count > 0
                                    ? CopyType.DrumPatterns
                                    : CopyType.WaveSequences;
            }
        }

        
        /// <summary>
        /// File name of the PCG where is copied from.
        /// </summary>
        public string CopyFileName { get; set; }

        
        /// <summary>
        /// PCG memory where to copy to. This value is null when not pasted yet.
        /// When true, other editing controls are enabled.
        /// </summary>
        /// 
        public IPcgMemory PastePcgMemory { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<IClipBoardPatches> Programs { get; private set; } // List per Program.SynthesisType.Last


        /// <summary>
        /// 
        /// </summary>
        public IClipBoardPatches Combis { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public IClipBoardPatches SetListSlots { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public IClipBoardPatches DrumKits { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public IClipBoardPatches DrumPatterns { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public IClipBoardPatches WaveSequences { get; private set; }

        
        // The following four items are for the Recall functionality.

        /// <summary>
        /// 
        /// </summary>
        private List<IClipBoardPatches> MemoryPrograms { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IClipBoardPatches MemoryCombis { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IClipBoardPatches MemorySetListSlots { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IClipBoardPatches MemoryDrumKits { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IClipBoardPatches MemoryDrumPatterns { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private IClipBoardPatches MemoryWaveSequences { get; set; }


        /// <summary>
        /// Patches that should not be overwritten during cut/copy/paste mode. This consist of duplicates and already 
        /// pasted patches.
        /// </summary>
        public ObservableCollection<IPatch> ProtectedPatches { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public bool PasteDuplicatesExecuted { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public PcgClipBoard()
        {
            Programs = new List<IClipBoardPatches>();
            MemoryPrograms = new List<IClipBoardPatches>();

            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                Programs.Add(new ClipBoardPatches());
                MemoryPrograms.Add(new ClipBoardPatches());
            }

            Combis = new ClipBoardPatches();
            SetListSlots = new ClipBoardPatches();
            DrumKits = new ClipBoardPatches();
            DrumPatterns = new ClipBoardPatches();
            WaveSequences = new ClipBoardPatches();
            
            ProtectedPatches = new ObservableCollection<IPatch>();
        }


        /// <summary>
        /// Returns true if empty or all items have been copied.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
                {
                    if (Programs[index].CopiedPatches.Count != Programs[index].CopiedPatches.Count(
                        patch => patch.PasteDestination != null))
                    {
                        return false;
                    }
                }

                return ((Combis.CopiedPatches.Count ==
                         Combis.CopiedPatches.Count(patch => patch.PasteDestination != null)) &&
                        (SetListSlots.CopiedPatches.Count ==
                         SetListSlots.CopiedPatches.Count(patch => patch.PasteDestination != null)) &&
                        (SetListSlots.CopiedPatches.Count ==
                         DrumKits.CopiedPatches.Count(patch => patch.PasteDestination != null)) &&
                        (SetListSlots.CopiedPatches.Count ==
                         DrumPatterns.CopiedPatches.Count(patch => patch.PasteDestination != null)) &&
                        (SetListSlots.CopiedPatches.Count ==
                         WaveSequences.CopiedPatches.Count(patch => patch.PasteDestination != null)) &&
                        (Combis.CopiedPatches.Count == 0) &&
                        (SetListSlots.CopiedPatches.Count == 0) &&
                        (DrumKits.CopiedPatches.Count == 0) &&
                        (DrumPatterns.CopiedPatches.Count == 0) &&
                        (WaveSequences.CopiedPatches.Count == 0));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountUncopiedSampledPrograms()
        {
            var uncopied = 0;
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                if (!Program.IsModeled((ProgramBank.SynthesisType) (index)))
                {
                    uncopied += Programs[index].CopiedPatches.Count;
                }
            }

            return uncopied;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CountUncopiedModeledPrograms()
        {
            var uncopied = 0;
            for (var index = 0; index < (int)ProgramBank.SynthesisType.Last; index++)
            {
                if (Program.IsModeled((ProgramBank.SynthesisType)(index)))
                {
                    uncopied += Programs[index].CopiedPatches.Count;
                }
            }

            return uncopied;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool CutPasteSelected { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                Programs[index].CopiedPatches.Clear();
            }

            Combis.CopiedPatches.Clear();
            SetListSlots.CopiedPatches.Clear();
            DrumKits.CopiedPatches.Clear();
            DrumPatterns.CopiedPatches.Clear();
            WaveSequences.CopiedPatches.Clear();

            PasteDuplicatesExecuted = false;
            PastePcgMemory = null;
            ProtectedPatches.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programToFind"></param>
        /// <returns></returns>
        IClipBoardPatch FindProgram(IPatch programToFind)
        {
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                var program = FindProgram(Programs[index].CopiedPatches, programToFind);
                if (program != null)
                {
                    return program;
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programs"></param>
        /// <param name="programToFind"></param>
        /// <returns></returns>
        static IClipBoardPatch FindProgram(IEnumerable<IClipBoardPatch> programs, IPatch programToFind)
        {
            IClipBoardPatch patch = null;
            foreach (var program in programs)
            {
                Debug.Assert(programToFind.ByteLength == program.Data.Length);
                if (Util.ByteCompareEqual(
                    programToFind.PcgRoot.Content, programToFind.ByteOffset, program.Data, programToFind.ByteLength))
                {
                    patch = program;
                    break;
                }
            }

            return patch;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKitToFind"></param>
        /// <returns></returns>
        IClipBoardDrumKit FindDrumKit(IPatch drumKitToFind)
        {
            return FindDrumKit(DrumKits.CopiedPatches, drumKitToFind);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKits"></param>
        /// <param name="drumKitToFind"></param>
        /// <returns></returns>
        static IClipBoardDrumKit FindDrumKit(IEnumerable<IClipBoardPatch> drumKits, IPatch drumKitToFind)
        {
            IClipBoardPatch patch = null;
            foreach (var drumKit in drumKits)
            {
                Debug.Assert(drumKitToFind.ByteLength == drumKit.Data.Length);
                if (Util.ByteCompareEqual(
                    drumKitToFind.PcgRoot.Content, drumKitToFind.ByteOffset, drumKit.Data, drumKitToFind.ByteLength))
                {
                    patch = drumKit;
                    break;
                }
            }

            return patch as IClipBoardDrumKit;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPatternToFind"></param>
        /// <returns></returns>
        IClipBoardDrumPattern FindDrumPattern(IPatch drumPatternToFind)
        {
            return FindDrumPattern(DrumPatterns.CopiedPatches, drumPatternToFind);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPatterns"></param>
        /// <param name="drumPatternToFind"></param>
        /// <returns></returns>
        static IClipBoardDrumPattern FindDrumPattern(IEnumerable<IClipBoardPatch> drumPatterns, 
            IPatch drumPatternToFind)
        {
            IClipBoardPatch patch = null;
            foreach (var drumPattern in drumPatterns)
            {
                Debug.Assert(drumPatternToFind.ByteLength == drumPattern.Data.Length);
                if (Util.ByteCompareEqual(
                    drumPatternToFind.PcgRoot.Content, drumPatternToFind.ByteOffset, drumPattern.Data, 
                    drumPatternToFind.ByteLength))
                {
                    patch = drumPattern;
                    break;
                }
            }

            return patch as IClipBoardDrumPattern;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="program"></param>
        /// <param name="clearAfterCopy"></param>
        /// <returns></returns>
        public IClipBoardProgram CopyProgramToClipBoard(IProgram program, bool clearAfterCopy)
        {
            if (!((IBank) (program.Parent)).IsLoaded)
            {
                return null;
            }

            var clipBoardProgram = new ClipBoardProgram(program);
            Programs[((int)((IProgramBank)(program.Parent)).BankSynthesisType)].CopiedPatches.Add(clipBoardProgram);

            if (clearAfterCopy)
            {
                program.Clear();
            }

            // Copy used drum kits.
            if (!CutPasteSelected)
            {
                foreach (var drumKit in program.UsedDrumKits)
                {
                    CopyDrumKitOfProgramToClipBoard(drumKit, clipBoardProgram);
                }
            }

            /*TODO DRUM TRACK PROGRAM
            // Copy used drum track program.
            if (!CutPasteSelected)
            {
                //TODO CopyDrumProgramOfProgramToClipBoard(clipBoardProgram.dru)
            }
            */

            return clipBoardProgram;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="combi"></param>
        /// <param name="clearAfterCopy"></param>
        /// <returns></returns>
        public IClipBoardCombi CopyCombiToClipBoard(ICombi combi, bool clearAfterCopy)
        {
            // Copy combi.
            IClipBoardCombi clipBoardCombi = null;

            if (combi.PcgRoot.Content != null)
            {
                clipBoardCombi = new ClipBoardCombi(combi);
                Combis.CopiedPatches.Add(clipBoardCombi);
                
                if (clearAfterCopy)
                {
                    combi.Clear();
                }

                // Copy references (timbres).
                if (!CutPasteSelected)
                {
                    foreach (var timbre in combi.Timbres.TimbresCollection)
                    {
                        CopyTimbreOfCombiToClipboard(timbre, clipBoardCombi);
                    }
                }
            }

            return clipBoardCombi;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="timbre"></param>
        /// <param name="clipBoardCombi"></param>
        private void CopyTimbreOfCombiToClipboard(ITimbre timbre, IClipBoardCombi clipBoardCombi)
        {
            IClipBoardProgram clipBoardProgramToAdd = null;
            var usedProgramBank = timbre.UsedProgramBank;
            var usedProgram = (usedProgramBank == null) ? null : timbre.UsedProgram;
            
            if (ShouldTimbreBeCopied(timbre, usedProgramBank, usedProgram))
            {
                clipBoardProgramToAdd = FindProgram(usedProgram) as IClipBoardProgram ??
                                        CopyProgramToClipBoard(usedProgram, false);
            }

            clipBoardCombi.References.CopiedPatches.Add(clipBoardProgramToAdd);
        }


        /// <summary>
        /// Returns true if timbre should be copied.
        /// </summary>
        /// <param name="timbre"></param>
        /// <param name="usedProgramBank"></param>
        /// <param name="usedProgram"></param>
        /// <returns></returns>
        private bool ShouldTimbreBeCopied(ITimbre timbre, IBank usedProgramBank, IProgram usedProgram)
        {
            // Only copy programs which are audible.
            var copy = !timbre.GetParam(ParameterNames.TimbreParameterName.Status).ToString().Equals("Off");

            // Only copy programs which have a valid reference.
            copy &= (usedProgram != null);
            copy &= (usedProgramBank != null);
            
            if (copy)
            {
                // Skip GM programs (those are always present on the synth.
                copy &= (usedProgramBank.Type != BankType.EType.Gm);
            }
            
            if (copy)
            {
                // Copy only filled programs.
                copy &= !usedProgram.IsEmptyOrInit;

                // Copy only programs which are loaded in the file, or otherwise from the master file if configured
                // to be used and also set.
                copy &= (usedProgramBank.IsLoaded ||
                    (Settings.Default.CopyPaste_CopyPatchesFromMasterFile && usedProgram.IsFromMasterFile));
            }
            return copy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setListSlot"></param>
        /// <param name="clearAfterCopy"></param>
        public void CopySetListSlotToClipBoard(ISetListSlot setListSlot, bool clearAfterCopy)
        {
            // Copy set list slot.
            if (setListSlot.PcgRoot.Content != null)
            {
                var clipBoardSetListSlot = new ClipBoardSetListSlot(setListSlot);
                SetListSlots.CopiedPatches.Add(clipBoardSetListSlot);

                if (clearAfterCopy)
                {
                    setListSlot.Clear();
                }

                // Copy program of combi (do not copy song).
                if (!CutPasteSelected)
                {
                    var usedPatch = setListSlot.UsedPatch;
                    if ((usedPatch != null) &&
                        (Settings.Default.CopyPaste_CopyPatchesFromMasterFile || !usedPatch.IsFromMasterFile))
                    {
                        if (setListSlot.UsedPatch is IProgram)
                        {
                            var program = (IProgram) usedPatch;
                            if (((IProgramBank)(program.Parent)).Type != BankType.EType.Gm)
                            {
                                clipBoardSetListSlot.Reference = CopyProgramToClipBoard(program, false);
                            }
                        }
                        else if (setListSlot.UsedPatch is ICombi)
                        {
                            var combi = (ICombi) usedPatch;
                            clipBoardSetListSlot.Reference = CopyCombiToClipBoard(combi, false);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKit"></param>
        /// <param name="clearAfterCopy"></param>
        public IClipBoardDrumKit CopyDrumKitToClipBoard(IDrumKit drumKit, bool clearAfterCopy)
        {
            if ((drumKit == null) || !((IBank)drumKit.Parent).IsLoaded)
            {
                return null;
            }

            var clipBoardDrumKit = new ClipBoardDrumKit(drumKit);
            DrumKits.CopiedPatches.Add(clipBoardDrumKit);

            if (clearAfterCopy)
            {
                drumKit.Clear();
            }

            return clipBoardDrumKit;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumKit"></param>
        /// <param name="clipBoardProgram"></param>
        public void CopyDrumKitOfProgramToClipBoard(IDrumKit drumKit, IClipBoardProgram clipBoardProgram)
        {
            IClipBoardDrumKit clipBoardDrumKitToAdd = FindDrumKit(drumKit) ??
                                                      CopyDrumKitToClipBoard(drumKit, false);

            clipBoardProgram.ReferencedDrumKits.CopiedPatches.Add(clipBoardDrumKitToAdd);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPattern"></param>
        /// <param name="clearAfterCopy"></param>
        public IClipBoardDrumPattern CopyDrumPatternToClipBoard(IDrumPattern drumPattern, bool clearAfterCopy)
        {
            if (!((IBank)drumPattern.Parent).IsLoaded)
            {
                return null;
            }

            var clipBoardDrumPattern = new ClipBoardDrumPattern(drumPattern);
            DrumPatterns.CopiedPatches.Add(clipBoardDrumPattern);

            if (clearAfterCopy)
            {
                drumPattern.Clear();
            }

            return clipBoardDrumPattern;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumPattern"></param>
        /// <param name="clipBoardProgram"></param>
        public void CopyDrumPatternOfProgramToClipBoard(IDrumPattern drumPattern, IClipBoardProgram clipBoardProgram)
        {
            IClipBoardDrumPattern clipBoardDrumPatternToAdd = FindDrumPattern(drumPattern) ??
                                                      CopyDrumPatternToClipBoard(drumPattern, false);

            //clipBoardProgram.ReferencedDrumPatterns.CopiedPatches.Add(clipBoardDrumPatternToAdd);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveSequence"></param>
        /// <param name="clearAfterCopy"></param>
        public void CopyWaveSequenceToClipBoard(IWaveSequence waveSequence, bool clearAfterCopy)
        {
            // Copy wave sequence.
            if (waveSequence.PcgRoot.Content != null)
            {
                var clipBoardWaveSequence = new ClipBoardWaveSequence(waveSequence);
                WaveSequences.CopiedPatches.Add(clipBoardWaveSequence);

                if (clearAfterCopy)
                {
                    waveSequence.Clear();
                }
            }
        }

        
        /// <summary>
        /// Returns the first patch to be pasted (if any).
        /// </summary>
        /// <param name="patches"></param>
        /// <returns></returns>
        public IClipBoardPatch GetFirstPatchToPaste(IEnumerable<IClipBoardPatch> patches)
        {
            return patches.FirstOrDefault(patch => patch.PasteDestination == null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="patch"></param>
        public void PastePatch(IClipBoardPatch patchToPaste, IPatch patch)
        {
            // Copy patch from clipboard to PCG.
            patch.PcgRoot.CopyPatch(patchToPaste, patch);
            patchToPaste.PasteDestination = patch;
            ProtectedPatches.Add(patch);

            if (CutPasteSelected)
            {
                var program = patch as IProgram;
                if (program != null)
                {
                    FixReferencesToProgram(patchToPaste, program);
                }
                else
                {
                    var combi = patch as ICombi;
                    if (combi != null)
                    {
                        FixReferencesToCombi(patchToPaste, combi);
                    }
                    else
                    {
                        var drumKit = patch as IDrumKit;
                        if (drumKit != null)
                        {
                            FixReferencesToDrumKit(patchToPaste, drumKit);
                        }
                        else
                        {
                            var drumPattern = patch as IDrumPattern;
                            if (drumPattern != null)
                            {
                                FixReferencesToDrumPattern(patchToPaste, drumPattern);
                            }   
                        }
                    }
                }
                // If it is a set list slot do nothing. 
            }
        }


        /// <summary>
        /// Changes all references of the original location of patch to program.
        /// Only used for cut/paste.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="program"></param>
        static void FixReferencesToProgram(IClipBoardPatch patch, IProgram program)
        {
            var memory = patch.OriginalLocation.Root as IPcgMemory;
            Debug.Assert(memory != null);

            FixProgramReferencesToCombi(patch, program, memory);
            FixProgramReferencesToSetLists(patch, program, memory);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="program"></param>
        /// <param name="memory"></param>
        private static void FixProgramReferencesToSetLists(IClipBoardPatch patch, IProgram program, IPcgMemory memory)
        {
            // Change set list slots (only which are present in the current file).
            if (memory.SetLists != null)
            {
                foreach (var slot in memory.SetLists.BankCollection.Where(
                    bank => bank.IsWritable && !bank.IsFromMasterFile).
                    SelectMany(list => list.Patches, (list, patch1) => (ISetListSlot) patch1).
                    Where(slot => (slot.SelectedPatchType == SetListSlot.PatchType.Program) &&
                                  (slot.UsedPatch == patch.OriginalLocation)))
                {
                    slot.UsedPatch = program;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="program"></param>
        /// <param name="memory"></param>
        private static void FixProgramReferencesToCombi(IClipBoardPatch patch, IProgram program, IPcgMemory memory)
        {
            // Change combis (only which are present in the current file).
            if (memory.CombiBanks != null)
            {
                foreach (var timbre in from bank in memory.CombiBanks.BankCollection.Where(
                    bank => bank.IsWritable && !bank.IsFromMasterFile)
                    from patch1 in bank.Patches
                    select (ICombi) patch1
                    into combi
                    from timbre in combi.Timbres.TimbresCollection
                    where timbre.UsedProgram == patch.OriginalLocation
                    select timbre)
                {
                    timbre.UsedProgram = program;
                }
            }
        }


        /// <summary>
        /// Changes all references of the original location of patch to combi.
        /// Only used for cut/paste.
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="combi"></param>
        static void FixReferencesToCombi(IClipBoardPatch patch, ICombi combi)
        {
            var memory = patch.OriginalLocation.Root as IPcgMemory;
            Debug.Assert(memory != null);

            // Change set list slots (if present and not from master file).
            if (memory.SetLists != null)
            {
                foreach (var slot in memory.SetLists.BankCollection.Where(
                    bank => bank.IsWritable && !bank.IsFromMasterFile).SelectMany(
                        list => list.Patches, (list, patch1) => (ISetListSlot) patch1).Where(
                            slot => (slot.SelectedPatchType == SetListSlot.PatchType.Combi) &&
                                    (slot.UsedPatch == patch.OriginalLocation)))
                {
                    slot.UsedPatch = combi;
                }
            }
        }


        /// <summary>
        /// Changes all references of the original location of patch to drum kit. 
        /// Only used for cut/paste.
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="drumKit"></param>
        private static void FixReferencesToDrumKit(IClipBoardPatch patchToPaste, IDrumKit drumKit)
        {
            var memory = patchToPaste.OriginalLocation.Root as IPcgMemory;
            Debug.Assert(memory != null);

            // Change programs (if present and not from master file).
            if (memory.ProgramBanks != null)
            {
                foreach (var programBank in memory.ProgramBanks.BankCollection.Where(
                    bank => bank.IsWritable && !bank.IsFromMasterFile))
                {
                    foreach (var program in programBank.Patches)
                    {
                        var changes = new Dictionary<IDrumKit, IDrumKit>
                        {
                            {(IDrumKit) (patchToPaste.OriginalLocation), drumKit}
                        };
                        ((IProgram) program).ReplaceDrumKit(changes);
                    }
                }
            }
        }


        /// <summary>
        /// Changes all references of the original location of patch to drum pattern. 
        /// Only used for cut/paste.
        /// </summary>
        /// <param name="patchToPaste"></param>
        /// <param name="drumPattern"></param>
        private static void FixReferencesToDrumPattern(IClipBoardPatch patchToPaste, IDrumPattern drumPattern)
        {
            var memory = patchToPaste.OriginalLocation.Root as IPcgMemory;
            Debug.Assert(memory != null);

            // Change programs (if present and not from master file).
            if (memory.ProgramBanks != null)
            {
                foreach (var programBank in memory.ProgramBanks.BankCollection.Where(
                    bank => bank.IsWritable && !bank.IsFromMasterFile))
                {
                    foreach (var program in programBank.Patches)
                    {
                        var changes = new Dictionary<IDrumPattern, IDrumPattern>
                        {
                            {(IDrumPattern) (patchToPaste.OriginalLocation), drumPattern}
                        };
                        // TODO ((IProgram)program).ReplaceDrumPattern(changes);
                    }
                }
            }
        }


        /// <summary>
        /// Only used for cut/copy/paste.
        /// - For all programs and combis/set list slots: if program used by combi/set list slot and both pasted, 
        ///   fix reference and set reference to null in combi/set list slot.
        /// - For all combis and set list slots: if combi used in set list slot and both pasted, fix reference and 
        ///   set reference to null in set list slot.
        /// </summary>
        public void FixPasteReferencesAfterCopyPaste()
        {
            // Cut/Copy/paste.
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                FixPasteProgramReferencesInCombis(Programs[index].CopiedPatches);
            }

            foreach (var drumKit in DrumKits.CopiedPatches)
            {
                FixPasteDrumKitReferencesInPrograms(drumKit);
            }

            foreach (var drumPattern in DrumPatterns.CopiedPatches)
            {
                FixPasteDrumPatternReferencesInPrograms(drumPattern);
            }

            FixPastePatchInSetListSlotsReferences();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipBoardPatches"></param>
        private void FixPasteProgramReferencesInCombis(IEnumerable<IClipBoardPatch> clipBoardPatches)
        {
            foreach (var program in clipBoardPatches)
            {
                foreach (var clipBoardPatch in Combis.CopiedPatches)
                {
                    var combi = (IClipBoardCombi) clipBoardPatch;
                    for (var timbreIndex = 0; timbreIndex < combi.References.CopiedPatches.Count; timbreIndex++)
                    {
                        var timbre = combi.References.CopiedPatches[timbreIndex];
                        if (timbre != program)
                        {
                            continue;
                        }

                        // Program used by timbre in combi.
                        if ((program.PasteDestination != null) && (combi.PasteDestination != null))
                        {
                            // When pasting to the same file, then it is not wanted to fix references,
                            // because a duplicate program/combi can exist before the original reference 
                            // so it will be changed unnecessarily.
                            if (program.OriginalLocation.Root != PastePcgMemory)
                            {
                                ((ICombi)(combi.PasteDestination)).Timbres.TimbresCollection[timbreIndex].UsedProgram =
                                    (IProgram) program.PasteDestination;
                            }
                            combi.References.CopiedPatches[timbreIndex] = null; // Prevent fixing it again
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Fix drum kit references in programs (clipBoardPatches).
        /// </summary>
        /// <param name="drumKit"></param>
        private void FixPasteDrumKitReferencesInPrograms(IClipBoardPatch drumKit)
        {
            for (var index = 0; index < (int)ProgramBank.SynthesisType.Last; index++)
            {
                foreach (var clipBoardPatch in Programs[index].CopiedPatches)
                {
                    var program = (IClipBoardProgram) clipBoardPatch;
                    for (var drumKitIndex = 0; drumKitIndex < program.ReferencedDrumKits.CopiedPatches.Count;
                        drumKitIndex++)
                    {
                        var usedDrumKit = program.ReferencedDrumKits.CopiedPatches[drumKitIndex];
                        if (usedDrumKit != drumKit)
                        {
                            continue;
                        }

                        // DrumKit used by usedDrumKit in program.
                        if ((drumKit.PasteDestination != null) && (program.PasteDestination != null))
                        {
                            // When pasting to the same file, then it is not wanted to fix references,
                            // because a duplicate drumkit/program can exist before the original reference 
                            // so it will be changed unnecessarily.
                            if (drumKit.OriginalLocation.Root != PastePcgMemory)
                            {
                                var changes = new Dictionary<IDrumKit, IDrumKit>
                                {
                                    {
                                        ((IProgram) (program.PasteDestination)).UsedDrumKits[drumKitIndex],
                                        (IDrumKit) drumKit.PasteDestination
                                    }
                                };

                                ((IProgram) (program.PasteDestination)).ReplaceDrumKit(changes);
                            }
                            program.ReferencedDrumKits.CopiedPatches[drumKitIndex] = null; // Prevent fixing it again
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Fix drum pattern references in programs (clipBoardPatches).
        /// </summary>
        /// <param name="drumPattern"></param>
        private void FixPasteDrumPatternReferencesInPrograms(IClipBoardPatch drumPattern)
        {
            return;


            //TODO
            /*
            
            for (var index = 0; index < (int)ProgramBank.SynthesisType.Last; index++)
            {
                foreach (var clipBoardPatch in Programs[index].CopiedPatches)
                {
                    var program = (IClipBoardProgram)clipBoardPatch;
                    //TODOfor (var drumPatternIndex = 0; drumPatternIndex < program.ReferencedDrumPatterns.CopiedPatches.Count;
                    //TODO    drumPatternIndex++)
                    {
                        //TODOvar usedDrumPattern = program.ReferencedDrumPatterns.CopiedPatches[drumPatternIndex];
                        //TODOif (usedDrumPattern != drumPattern)
                        {
                            continue;
                        }

                        // DrumPattern used by usedDrumPattern in program.
                        if ((drumPattern.PasteDestination != null) && (program.PasteDestination != null))
                        {
                            // When pasting to the same file, then it is not wanted to fix references,
                            // because a duplicate drumPattern/program can exist before the original reference 
                            // so it will be changed unnecessarily.
                            if (drumPattern.OriginalLocation.Root != PastePcgMemory)
                            {
                                var changes = new Dictionary<IDrumPattern, IDrumPattern>
                                {
                                    {
                                        ((IProgram) (program.PasteDestination)).UsedDrumPatterns[drumPatternIndex],
                                        (IDrumPattern) drumPattern.PasteDestination
                                    }
                                };

                                ((IProgram)(program.PasteDestination)).ReplaceDrumPattern(changes);
                            }
                            //TODOprogram.ReferencedDrumPatterns.CopiedPatches[drumPatternIndex] = null; // Prevent fixing it again
                        }
                    }
                }
            }
             */
        }


        /*
        /// <summary>
        /// Fix drum kit references in programs (clipBoardPatches).
        /// </summary>
        /// <param name="clipBoardPatches"></param>
        private void FixPasteDrumKitReferencesInPrograms(IEnumerable<IClipBoardPatch> clipBoardPatches)
        {
            foreach (var clipBoardPatch in clipBoardPatches)
            {
                var program = (IClipBoardProgram) clipBoardPatch;
                foreach (var clipBoardDrumKit in DrumKits.CopiedPatches)
                {
                    var drumKits = program.ReferencedDrumKits;
                    for (var drumKitIndex = 0; drumKitIndex < drumKits.CopiedPatches.Count(); drumKitIndex++)
                    {
                        var drumKit = program.ReferencedDrumKits.CopiedPatches[drumKitIndex];
                        if (drumKit != clipBoardDrumKit)
                        {
                            continue;
                        }

                        if ((drumKit.PasteDestination != null) && (program.PasteDestination != null))
                        {
                            // When pasting to the same file, then it is not wanted to fix references,
                            // because a duplicate drumkit/program can exist before the original reference 
                            // so it will be changed unnecessarily.
                            if (drumKit.OriginalLocation.Root != PastePcgMemory)
                            {
                                var changes = new Dictionary<IDrumKit, IDrumKit>
                                {
                                   
                                
                                
                                //xxx
                                
                                
                                
                                
                                
                                {(IDrumKit) (drumKit.OriginalLocation), 
                                        (IDrumKit) (drumKit.PasteDestination)}
                                };

                                ((IProgram) (program.PasteDestination)).ReplaceDrumKit(changes);
                            }
                            program.ReferencedDrumKits.CopiedPatches[drumKitIndex] = null; // Prevent fixing it again
                        }
                    }
                }
            }
        }
        */

        /// <summary>
        /// 
        /// </summary>
        void FixPastePatchInSetListSlotsReferences()
        {
            foreach (var clipBoardPatch in SetListSlots.CopiedPatches)
            {
                var clipBoardSetListSlot = (IClipBoardSetListSlot) clipBoardPatch;
                var setListSlot = (ISetListSlot)(clipBoardSetListSlot.PasteDestination);
                if ((setListSlot != null) && (clipBoardSetListSlot.Reference != null)) 
                {
                    // Fix reference to program/combi.
                    var clipBoardReference  = clipBoardSetListSlot.Reference;
                    
                    // When pasting to the same file (second condition below), then it is not wanted to fix references,
                    // because a duplicate program/combi can exist before the original reference so it will be changed 
                    // unnecessarily.
                    if ((clipBoardReference.PasteDestination != null) && 
                        (clipBoardSetListSlot.Reference.OriginalLocation.Root != PastePcgMemory))
                    {
                        if (clipBoardReference.PasteDestination is IProgram)
                        {
                            var program = (IProgram) (clipBoardReference.PasteDestination);
                            setListSlot.UsedPatch = program;
                        }
                        else if (clipBoardReference.PasteDestination is ICombi)
                        {
                            var combi = (ICombi) (clipBoardReference.PasteDestination);
                            setListSlot.UsedPatch = combi;
                        }
                        else
                        {
                            throw new ApplicationException("Illegal clip board reference");
                        }
                        setListSlot.RaisePropertyChanged(string.Empty, false);
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public bool IsPastingFinished
        {
            get
            {
                for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
                {
                    if (Programs[index].CountUncopied > 0)
                    {
                        return false;
                    }
                }

                return ((DrumKits.CountUncopied == 0) &&
                        (DrumPatterns.CountUncopied == 0) &&
                        (WaveSequences.CountUncopied == 0) &&
                        (Combis.CountUncopied == 0) &&
                        (SetListSlots.CountUncopied == 0));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void Memorize()
        {
            MemoryPrograms = new List<IClipBoardPatches>();

            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                MemoryPrograms.Add(new ClipBoardPatches());
                MemoryPrograms[index] = new ClipBoardPatches();
                foreach (var patch in Programs[index].CopiedPatches)
                {
                    MemoryPrograms[index].CopiedPatches.Add(patch);
                }
            }

            MemoryCombis = new ClipBoardPatches();
            foreach (var patch in Combis.CopiedPatches)
            {
                MemoryCombis.CopiedPatches.Add(patch);
            }

            MemorySetListSlots = new ClipBoardPatches();
            foreach (var patch in SetListSlots.CopiedPatches)
            {
                MemorySetListSlots.CopiedPatches.Add(patch);
            }
            
            MemoryDrumKits = new ClipBoardPatches();
            foreach (var patch in DrumKits.CopiedPatches)
            {
                MemoryDrumKits.CopiedPatches.Add(patch);
            }

            MemoryDrumPatterns = new ClipBoardPatches();
            foreach (var patch in DrumPatterns.CopiedPatches)
            {
                MemoryDrumPatterns.CopiedPatches.Add(patch);
            }

            MemoryWaveSequences = new ClipBoardPatches();
            foreach (var patch in WaveSequences.CopiedPatches)
            {
                MemoryWaveSequences.CopiedPatches.Add(patch);
            }


            ProtectedPatches = new ObservableCollection<IPatch>();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Recall()
        {
            RecallPrograms();
            RecallCombis();
            RecallSetListSlots();
            RecallDrumKits();
            RecallDrumPatterns();
            RecallWaveSequences();

            ProtectedPatches = new ObservableCollection<IPatch>();
        }


        /// <summary>
        /// 
        /// </summary>
        private void RecallPrograms()
        {
            for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
            {
                Programs[index] = new ClipBoardPatches();
                if (MemoryPrograms != null)
                {
                    foreach (var patch in MemoryPrograms[index].CopiedPatches)
                    {
                        Programs[index].CopiedPatches.Add(patch);
                        patch.OriginalLocation = patch.PasteDestination;
                        patch.PasteDestination = null;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void RecallCombis()
        {
            Combis = new ClipBoardPatches();
            if (MemoryCombis != null)
            {
                foreach (var patch in MemoryCombis.CopiedPatches)
                {
                    Combis.CopiedPatches.Add(patch);
                    patch.OriginalLocation = patch.PasteDestination;
                    patch.PasteDestination = null;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void RecallSetListSlots()
        {
            SetListSlots = new ClipBoardPatches();
            if (MemorySetListSlots != null)
            {
                foreach (var patch in MemorySetListSlots.CopiedPatches)
                {
                    SetListSlots.CopiedPatches.Add(patch);
                    patch.OriginalLocation = patch.PasteDestination;
                    patch.PasteDestination = null;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void RecallDrumKits()
        {
            DrumKits = new ClipBoardPatches();
            if (MemoryDrumKits != null)
            {
                foreach (var patch in MemoryDrumKits.CopiedPatches)
                {
                    DrumKits.CopiedPatches.Add(patch);
                    patch.OriginalLocation = patch.PasteDestination;
                    patch.PasteDestination = null;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void RecallDrumPatterns()
        {
            DrumPatterns = new ClipBoardPatches();
            if (MemoryDrumPatterns != null)
            {
                foreach (var patch in MemoryDrumPatterns.CopiedPatches)
                {
                    DrumPatterns.CopiedPatches.Add(patch);
                    patch.OriginalLocation = patch.PasteDestination;
                    patch.PasteDestination = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RecallWaveSequences()
        {
            WaveSequences = new ClipBoardPatches();
            if (MemoryWaveSequences != null)
            {
                foreach (var patch in MemoryWaveSequences.CopiedPatches)
                {
                    WaveSequences.CopiedPatches.Add(patch);
                    patch.OriginalLocation = patch.PasteDestination;
                    patch.PasteDestination = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMemoryEmpty
        {
            get
            {
                for (var index = 0; index < (int) ProgramBank.SynthesisType.Last; index++)
                {
                    if ((MemoryPrograms[index] != null) && (MemoryPrograms.Count != 0))
                    {
                        return false;
                    }
                }

                return ((MemoryCombis == null) || (MemoryCombis.CopiedPatches.Count == 0)) &&
                       ((MemorySetListSlots == null) || (MemorySetListSlots.CopiedPatches.Count == 0)) &&
                       ((MemoryDrumKits == null) || (MemoryDrumKits.CopiedPatches.Count == 0)) &&
                       ((MemoryDrumPatterns == null) || (MemoryDrumPatterns.CopiedPatches.Count == 0)) &&
                       ((MemoryWaveSequences == null) || (MemoryWaveSequences.CopiedPatches.Count == 0));
            }
        }
    }
}
