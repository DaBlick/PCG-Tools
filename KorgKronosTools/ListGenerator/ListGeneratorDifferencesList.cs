// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class ListGeneratorDifferencesList : ListGenerator
    {
        /// <summary>
        /// Dictionary from each patch in each patch bank to a dictionary with #diffs as key and a list of programs 
        /// with that many diffs.
        /// </summary>
        Dictionary<IProgram, Dictionary<int, LinkedList<IProgram>>> _diffPrograms;


        /// <summary>
        /// 
        /// </summary>
        Dictionary<ICombi, Dictionary<int, LinkedList<ICombi>>> _diffCombis;


        /// <summary>
        /// 
        /// </summary>
        Dictionary<ISetListSlot, Dictionary<int, LinkedList<ISetListSlot>>> _diffSetListSlots;


        /// <summary>
        /// 
        /// </summary>
        Dictionary<IDrumKit, Dictionary<int, LinkedList<IDrumKit>>> _diffDrumKits;

        
        /// <summary>
        /// 
        /// </summary>
        Dictionary<IDrumPattern, Dictionary<int, LinkedList<IDrumPattern>>> _diffDrumPatterns;


        /// <summary>
        /// 
        /// </summary>
        Dictionary<IWaveSequence, Dictionary<int, LinkedList<IWaveSequence>>> _diffWaveSequences; 


        /// <summary>
        /// 
        /// </summary>
        public int MaxDiffs { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IgnorePatchNames { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreSetListSlotDescriptions { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool SearchBothDirections { private get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDiffs"></param>
        /// <param name="ignorePatchNames"></param>
        /// <param name="ignoreSetListSlotDescriptions"></param>
        /// <param name="searchBothDirections"></param>
        public ListGeneratorDifferencesList(
            int maxDiffs = 10, bool ignorePatchNames = true, bool ignoreSetListSlotDescriptions = true, 
            bool searchBothDirections = false)
        {
            Debug.Assert(maxDiffs >= 0);
            MaxDiffs = maxDiffs;
            IgnorePatchNames = ignorePatchNames;
            IgnoreSetListSlotDescriptions = ignoreSetListSlotDescriptions;
            SearchBothDirections = searchBothDirections;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected override string RunAfterFilteringBanks(bool useFileWriter = true)
        {
            using (var writer = File.CreateText(OutputFileName))
            {
                // Init diff list.
                FindPrograms();
                CreateProgramsList();

                FindCombis();
                CreateCombisList();

                FindSetListSlots();
                CreateSetListSlotsList();

                FindDrumKits();
                CreateDrumKitsList();

                FindDrumPatterns();
                CreateDrumPatternsList();

                FindWaveSequences();
                CreateWaveSequencesList();

                // Write to file.

                WriteToFile(writer);
                writer.Close();

                if (ListOutputFormat == OutputFormat.Xml)
                {
                    WriteXslFile();
                }
            }
          
            return OutputFileName;
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindPrograms()
        {
            _diffPrograms = new Dictionary<IProgram, Dictionary<int, LinkedList<IProgram>>>();

            foreach (var bank in SelectedProgramBanks)
            {
                foreach (var patch in bank.Patches)
                {
                    FindProgram((IProgram) patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        private void FindProgram(IProgram patch)
        {
            if ((PcgMemory.AreFavoritesSupported && (ListFilterOnFavorites != FilterOnFavorites.All)) &&
                (((ListFilterOnFavorites != FilterOnFavorites.Yes) || 
                !patch.GetParam(ParameterNames.ProgramParameterName.Favorite).Value) &&
                 ((ListFilterOnFavorites != FilterOnFavorites.No) || 
                 patch.GetParam(ParameterNames.ProgramParameterName.Favorite).Value)))
            {
                return;
            }

            var dict = new Dictionary<int, LinkedList<IProgram>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<IProgram>();
            }
            lock (_diffPrograms)
            {
                _diffPrograms.Add(patch, dict);
            }
        }


        /// <summary>
        /// IMPR: Check if both directions should be supported (like combi).
        /// </summary>
        void CreateProgramsList()
        {
            // Compare with all other selected programs.
            foreach (var bank in SelectedProgramBanks.Where(bank => bank.Type != BankType.EType.Gm))
            {
                CreateProgramList(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void CreateProgramList(IProgramBank bank)
        {
            for (var index = 0; index < bank.Patches.Count; index++)
            {
                var patch = (IProgram) bank[index];

                if (!bank.IsLoaded ||
                    !patch.UseInList(IgnoreInitPrograms, FilterOnText, FilterText, FilterCaseSensitive, 
                    ListFilterOnFavorites, false))
                {
                    continue;
                }

                var bank1 = bank;
                foreach (var bank2 in SelectedProgramBanks.Where(
                    bank2 => (bank1.BankSynthesisType == bank2.BankSynthesisType) &&
                             (bank2.Type != BankType.EType.Gm)))
                {
                    AnalyzePrograms(bank, bank2, index, patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bank2"></param>
        /// <param name="programIndex"></param>
        /// <param name="patch"></param>
        private void AnalyzePrograms(IProgramBank bank, IProgramBank bank2, int programIndex, IProgram patch)
        {
            if (SearchBothDirections || (string.Compare(bank.Id, bank2.Id, StringComparison.Ordinal) <= 0))
            {
                var startIndex = SearchBothDirections ? 0 : (bank == bank2 ? programIndex + 1 : 0);
                for (var patch2Index = startIndex; patch2Index < bank2.Patches.Count; patch2Index++)
                {
                    AnalyzeProgram(bank2, patch, patch2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank2"></param>
        /// <param name="patch"></param>
        /// <param name="patch2Index"></param>
        private void AnalyzeProgram(IBank bank2, IProgram patch, int patch2Index)
        {
            var patch2 = (IProgram) bank2[patch2Index];
            if ((patch == patch2) || !bank2.IsLoaded ||
                !patch2.UseInList(IgnoreInitPrograms, FilterOnText, FilterText, FilterCaseSensitive,
                    ListFilterOnFavorites, false))
            {
                return;
            }

            var diffs = CalculateDiffs(patch, patch2);
            if (diffs <= MaxDiffs)
            {
                _diffPrograms[patch][diffs].AddLast(patch2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindCombis()
        {
            _diffCombis = new Dictionary<ICombi, Dictionary<int, LinkedList<ICombi>>>();

            foreach (var bank in SelectedCombiBanks)
            {
                foreach (var patch in bank.Patches)
                {
                    FindCombi((ICombi)patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        private void FindCombi(ICombi patch)
        {
            if ((PcgMemory.AreFavoritesSupported && (ListFilterOnFavorites != FilterOnFavorites.All)) &&
                (((ListFilterOnFavorites != FilterOnFavorites.Yes) ||
                !patch.GetParam(ParameterNames.CombiParameterName.Favorite).Value) &&
                 ((ListFilterOnFavorites != FilterOnFavorites.No) ||
                 patch.GetParam(ParameterNames.CombiParameterName.Favorite).Value)))
            {
                return;
            }

            var dict = new Dictionary<int, LinkedList<ICombi>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<ICombi>();
            }
            lock (_diffCombis)
            {
                _diffCombis.Add(patch, dict);
            }
        }


        /// <summary>
        /// IMPR: Check if both directions should be supported (like combi).
        /// </summary>
        void CreateCombisList()
        {
            // Compare with all other selected Combis.
            foreach (var bank in SelectedCombiBanks.Where(bank => bank.Type != BankType.EType.Gm))
            {
                CreateCombiList(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void CreateCombiList(ICombiBank bank)
        {
            for (var index = 0; index < bank.Patches.Count; index++)
            {
                var patch = (ICombi)bank[index];

                if (!bank.IsLoaded ||
                    !patch.UseInList(IgnoreInitCombis, FilterOnText, FilterText, FilterCaseSensitive,
                     ListFilterOnFavorites, false))
                {
                    continue;
                }

                foreach (var bank2 in SelectedCombiBanks)
                {
                    AnalyzeCombis(bank, bank2, index, patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bank2"></param>
        /// <param name="CombiIndex"></param>
        /// <param name="patch"></param>
        private void AnalyzeCombis(ICombiBank bank, ICombiBank bank2, int CombiIndex, ICombi patch)
        {
            if (SearchBothDirections || (string.Compare(bank.Id, bank2.Id, StringComparison.Ordinal) <= 0))
            {
                var startIndex = SearchBothDirections ? 0 : (bank == bank2 ? CombiIndex + 1 : 0);
                for (var patch2Index = startIndex; patch2Index < bank2.Patches.Count; patch2Index++)
                {
                    AnalyzeCombi(bank2, patch, patch2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank2"></param>
        /// <param name="patch"></param>
        /// <param name="patch2Index"></param>
        private void AnalyzeCombi(IBank bank2, ICombi patch, int patch2Index)
        {
            var patch2 = (ICombi)bank2[patch2Index];
            if ((patch == patch2) || !bank2.IsLoaded ||
                !patch2.UseInList(IgnoreInitCombis, FilterOnText, FilterText, FilterCaseSensitive,
                    ListFilterOnFavorites, false))
            {
                return;
            }

            var diffs = CalculateDiffs(patch, patch2);
            if (diffs <= MaxDiffs)
            {
                _diffCombis[patch][diffs].AddLast(patch2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindSetListSlots()
        {
            _diffSetListSlots = new Dictionary<ISetListSlot, Dictionary<int, LinkedList<ISetListSlot>>>();

            if (SetListsEnabled)
            {
                for (var setListIndex = SetListsRangeFrom; setListIndex <= SetListsRangeTo; setListIndex++)
                {
                    var setList = (ISetList) PcgMemory.SetLists[setListIndex];
                    for (var setListSlotIndex = 0; setListSlotIndex < setList.NrOfPatches; setListSlotIndex++)
                    {
                        FindSetListSlot((ISetListSlot) setList[setListSlotIndex]);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setListSlot"></param>
        private void FindSetListSlot(ISetListSlot setListSlot)
        {
            var dict = new Dictionary<int, LinkedList<ISetListSlot>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<ISetListSlot>();
            }
            lock (_diffSetListSlots)
            {
                _diffSetListSlots.Add(setListSlot, dict);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void CreateSetListSlotsList()
        {
            if (SetListsEnabled)
            {
                // Compare with all other selected set list slots.
                for (var setListIndex = SetListsRangeFrom; setListIndex < SetListsRangeTo + 1; setListIndex++)
                {
                    var setList = (ISetList) PcgMemory.SetLists[setListIndex];
                    CreateSetListSlotList(setList);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setList"></param>
        private void CreateSetListSlotList(ISetList setList)
        {
            for (var setListSlotIndex = 0; setListSlotIndex < setList.Patches.Count; setListSlotIndex++)
            {
                var setListSlot = (ISetListSlot) setList[setListSlotIndex];

                if (!setList.IsLoaded ||
                    !setListSlot.UseInList(IgnoreInitSetListSlots, FilterOnText, FilterText, FilterCaseSensitive,
                        FilterOnFavorites.All, FilterSetListSlotDescription))
                {
                    continue;
                }

                var setList1 = setList;
                for (var setListIndex2 = 0; setListIndex2 < SetListsRangeTo + 1; setListIndex2++)
                {
                    var setList2 = PcgMemory.SetLists[setListIndex2] as ISetList;
                    if (SearchBothDirections || (setList1.Index <= setList2.Index))
                    {
                        AnalyzeCheckListSlots(setList, setList2, setListSlotIndex, setListSlot);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setList"></param>
        /// <param name="setList2"></param>
        /// <param name="setListSlotIndex"></param>
        /// <param name="setListSlot"></param>
        private void AnalyzeCheckListSlots(ISetList setList, ISetList setList2, int setListSlotIndex, 
            ISetListSlot setListSlot)
        {
            if (SearchBothDirections || (setList.Index <= setList2.IndexOffset))
            {
                var startIndex = SearchBothDirections ? 0 : (setList == setList2 ? setListSlotIndex + 1 : 0);
                for (var setListSlot2Index = startIndex; setListSlot2Index < setList2.Patches.Count; 
                    setListSlot2Index++)
                {
                    AnalyzeSetListSlot(setList2, setListSlot, setListSlot2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setList2"></param>
        /// <param name="setListSlot"></param>
        /// <param name="setListSlot2Index"></param>
        private void AnalyzeSetListSlot(IBank setList2, ISetListSlot setListSlot, int setListSlot2Index)
        {
            var setListSlot2 = (ISetListSlot)setList2[setListSlot2Index];
            if ((setListSlot == setListSlot2) || !setList2.IsLoaded ||
                !setListSlot2.UseInList(IgnoreInitPrograms, FilterOnText, FilterText, FilterCaseSensitive,
                    FilterOnFavorites.All, FilterSetListSlotDescription))
            {
                return;
            }

            var diffs = CalculateDiffs(setListSlot, setListSlot2);
            if (diffs <= MaxDiffs)
            {
                _diffSetListSlots[setListSlot][diffs].AddLast(setListSlot2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void FindDrumKits()
        {
            _diffDrumKits = new Dictionary<IDrumKit, Dictionary<int, LinkedList<IDrumKit>>>();

            foreach (var bank in SelectedDrumKitBanks)
            {
                foreach (var patch in bank.Patches)
                {
                    FindDrumKit((IDrumKit)patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        private void FindDrumKit(IDrumKit patch)
        {
            // Assumed filtering on favorites is not needed.

            var dict = new Dictionary<int, LinkedList<IDrumKit>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<IDrumKit>();
            }
            lock (_diffDrumKits)
            {
                _diffDrumKits.Add(patch, dict);
            }
        }


        /// <summary>
        /// IMPR: Check if both directions should be supported (like combi).
        /// </summary>
        void CreateDrumKitsList()
        {
            // Compare with all other selected DrumKits.
            foreach (var bank in SelectedDrumKitBanks.Where(bank => bank.Type != BankType.EType.Gm))
            {
                CreateDrumKitList(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void CreateDrumKitList(IDrumKitBank bank)
        {
            for (var index = 0; index < bank.Patches.Count; index++)
            {
                var patch = (IDrumKit)bank[index];

                if (!bank.IsLoaded ||
                    !patch.UseInList(IgnoreInitDrumKits, FilterOnText, FilterText, FilterCaseSensitive, 
                    ListFilterOnFavorites, false))
                {
                    continue;
                }

                foreach (var bank2 in SelectedDrumKitBanks)
                {
                    AnalyzeDrumKits(bank, bank2, index, patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bank2"></param>
        /// <param name="DrumKitIndex"></param>
        /// <param name="patch"></param>
        private void AnalyzeDrumKits(IDrumKitBank bank, IDrumKitBank bank2, int DrumKitIndex, IDrumKit patch)
        {
            if (SearchBothDirections || (string.Compare(bank.Id, bank2.Id, StringComparison.Ordinal) <= 0))
            {
                var startIndex = SearchBothDirections ? 0 : (bank == bank2 ? DrumKitIndex + 1 : 0);
                for (var patch2Index = startIndex; patch2Index < bank2.Patches.Count; patch2Index++)
                {
                    AnalyzeDrumKit(bank2, patch, patch2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank2"></param>
        /// <param name="patch"></param>
        /// <param name="patch2Index"></param>
        private void AnalyzeDrumKit(IBank bank2, IDrumKit patch, int patch2Index)
        {
            var patch2 = (IDrumKit)bank2[patch2Index];
            if ((patch == patch2) || !bank2.IsLoaded ||
                !patch2.UseInList(IgnoreInitDrumKits, FilterOnText, FilterText, FilterCaseSensitive,
                    ListFilterOnFavorites, false))
            {
                return;
            }

            var diffs = CalculateDiffs(patch, patch2);
            if (diffs <= MaxDiffs)
            {
                _diffDrumKits[patch][diffs].AddLast(patch2);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private void FindDrumPatterns()
        {
            _diffDrumPatterns = new Dictionary<IDrumPattern, Dictionary<int, LinkedList<IDrumPattern>>>();

            foreach (var bank in SelectedDrumPatternBanks)
            {
                foreach (var patch in bank.Patches)
                {
                    FindDrumPattern((IDrumPattern)patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        private void FindDrumPattern(IDrumPattern patch)
        {
            // Assumed filtering on favorites is not needed.

            var dict = new Dictionary<int, LinkedList<IDrumPattern>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<IDrumPattern>();
            }
            lock (_diffDrumPatterns)
            {
                _diffDrumPatterns.Add(patch, dict);
            }
        }


        /// <summary>
        /// IMPR: Check if both directions should be supported (like combi).
        /// </summary>
        void CreateDrumPatternsList()
        {
            // Compare with all other selected DrumPatterns.
            foreach (var bank in SelectedDrumPatternBanks.Where(bank => bank.Type != BankType.EType.Gm))
            {
                CreateDrumPatternList(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void CreateDrumPatternList(IDrumPatternBank bank)
        {
            for (var index = 0; index < bank.Patches.Count; index++)
            {
                var patch = (IDrumPattern)bank[index];

                if (!bank.IsLoaded ||
                    !patch.UseInList(IgnoreInitDrumPatterns, FilterOnText, FilterText, FilterCaseSensitive, 
                    ListFilterOnFavorites, false))
                {
                    continue;
                }

                foreach (var bank2 in SelectedDrumPatternBanks)
                {
                    AnalyzeDrumPatterns(bank, bank2, index, patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bank2"></param>
        /// <param name="DrumPatternIndex"></param>
        /// <param name="patch"></param>
        private void AnalyzeDrumPatterns(IDrumPatternBank bank, IDrumPatternBank bank2, int DrumPatternIndex, 
            IDrumPattern patch)
        {
            if (SearchBothDirections || (string.Compare(bank.Id, bank2.Id, StringComparison.Ordinal) <= 0))
            {
                var startIndex = SearchBothDirections ? 0 : (bank == bank2 ? DrumPatternIndex + 1 : 0);
                for (var patch2Index = startIndex; patch2Index < bank2.Patches.Count; patch2Index++)
                {
                    AnalyzeDrumPattern(bank2, patch, patch2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank2"></param>
        /// <param name="patch"></param>
        /// <param name="patch2Index"></param>
        private void AnalyzeDrumPattern(IBank bank2, IDrumPattern patch, int patch2Index)
        {
            var patch2 = (IDrumPattern)bank2[patch2Index];
            if ((patch == patch2) || !bank2.IsLoaded ||
                !patch2.UseInList(IgnoreInitDrumPatterns, FilterOnText, FilterText, FilterCaseSensitive,
                    ListFilterOnFavorites, false))
            {
                return;
            }

            var diffs = CalculateDiffs(patch, patch2);
            if (diffs <= MaxDiffs)
            {
                _diffDrumPatterns[patch][diffs].AddLast(patch2);
            }
        }
        

        /// <summary>
        /// 
        /// </summary>
        private void FindWaveSequences()
        {
            _diffWaveSequences = new Dictionary<IWaveSequence, Dictionary<int, LinkedList<IWaveSequence>>>();

            foreach (var bank in SelectedWaveSequenceBanks)
            {
                foreach (var patch in bank.Patches)
                {
                    FindWaveSequence((IWaveSequence)patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        private void FindWaveSequence(IWaveSequence patch)
        {
            // Assumed filtering on favorites is not needed.

            var dict = new Dictionary<int, LinkedList<IWaveSequence>>();
            for (var diffs = 0; diffs <= MaxDiffs; diffs++) // Including end range
            {
                dict[diffs] = new LinkedList<IWaveSequence>();
            }
            lock (_diffWaveSequences)
            {
                _diffWaveSequences.Add(patch, dict);
            }
        }


        /// <summary>
        /// IMPR: Check if both directions should be supported (like combi).
        /// </summary>
        void CreateWaveSequencesList()
        {
            // Compare with all other selected WaveSequences.
            foreach (var bank in SelectedWaveSequenceBanks.Where(bank => bank.Type != BankType.EType.Gm))
            {
                CreateWaveSequenceList(bank);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        private void CreateWaveSequenceList(IWaveSequenceBank bank)
        {
            for (var index = 0; index < bank.Patches.Count; index++)
            {
                var patch = (IWaveSequence)bank[index];

                if (!bank.IsLoaded ||
                    !patch.UseInList(IgnoreInitWaveSequences, FilterOnText, FilterText, FilterCaseSensitive, 
                     ListFilterOnFavorites, false))
                {
                    continue;
                }

                foreach (var bank2 in SelectedWaveSequenceBanks)
                {
                    AnalyzeWaveSequences(bank, bank2, index, patch);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank"></param>
        /// <param name="bank2"></param>
        /// <param name="WaveSequenceIndex"></param>
        /// <param name="patch"></param>
        private void AnalyzeWaveSequences(IWaveSequenceBank bank, IWaveSequenceBank bank2, int WaveSequenceIndex, 
            IWaveSequence patch)
        {
            if (SearchBothDirections || (string.Compare(bank.Id, bank2.Id, StringComparison.Ordinal) <= 0))
            {
                var startIndex = SearchBothDirections ? 0 : (bank == bank2 ? WaveSequenceIndex + 1 : 0);
                for (var patch2Index = startIndex; patch2Index < bank2.Patches.Count; patch2Index++)
                {
                    AnalyzeWaveSequence(bank2, patch, patch2Index);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank2"></param>
        /// <param name="patch"></param>
        /// <param name="patch2Index"></param>
        private void AnalyzeWaveSequence(IBank bank2, IWaveSequence patch, int patch2Index)
        {
            var patch2 = (IWaveSequence)bank2[patch2Index];
            if ((patch == patch2) || !bank2.IsLoaded ||
                !patch2.UseInList(IgnoreInitWaveSequences, FilterOnText, FilterText, FilterCaseSensitive,
                    ListFilterOnFavorites, false))
            {
                return;
            }

            var diffs = CalculateDiffs(patch, patch2);
            if (diffs <= MaxDiffs)
            {
                _diffWaveSequences[patch][diffs].AddLast(patch2);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch1"></param>
        /// <param name="patch2"></param>
        /// <returns></returns>
        int CalculateDiffs(IPatch patch1, IPatch patch2)
        {
            Debug.Assert(patch1.GetType() == patch2.GetType());
            Debug.Assert(patch1.ByteLength == patch2.ByteLength);
            
            if (patch1 is IProgram)
            {
                Debug.Assert(
                    ((IProgramBank)((patch1 as IProgram).Parent)).BankSynthesisType ==
                    ((IProgramBank)(patch2.Parent)).BankSynthesisType);
            }

            // Add all byte differences (including PBK2, SBK2 and SLS2 differences).
            var diffs = patch1.CalcByteDifferences(patch2, !IgnorePatchNames, MaxDiffs); 

            Debug.Assert(diffs >= 0);
            return diffs;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteToFile(TextWriter writer)
        {
            switch (ListOutputFormat)
            {
            case OutputFormat.AsciiTable:
                OutputToAsciiTable(writer);
                break;

            case OutputFormat.Text:
                OutputToText(writer);
                break;

            case OutputFormat.Csv:
                OutputToCsv(writer);
                break;

            case OutputFormat.Xml:
                OutputToXml(writer);
                break;

            default:
                throw new NotSupportedException("Unsupported output");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputToAsciiTable(TextWriter writer)
        {
            OutputProgramsToAsciiTable(writer);

            if (PcgMemory.CombiBanks != null)
            {
                OutputCombisToAsciiTable(writer);
            }

            if ((PcgMemory.SetLists != null) && SetListsEnabled)
            {
                OutputSetListSlotsToAsciiTable(writer);
            }

            if (PcgMemory.DrumKitBanks != null)
            {
                OutputDrumKitsToAsciiTable(writer);
            }

            if (PcgMemory.DrumPatternBanks != null)
            {
                OutputDrumPatternsToAsciiTable(writer);
            }

            if (PcgMemory.WaveSequenceBanks != null)
            {
                OutputWaveSequencesToAsciiTable(writer);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputProgramsToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall

            var max = _diffPrograms.Where(
                diffListPair => diffListPair.Value.Any(
                    differencesPair =>
                    differencesPair.Value.Count > 0)).
                                    Aggregate(
                                        0, (current1, diffListPair) => diffListPair.Value.Where(
                                            differencesPair =>
                                            differencesPair.Value.Count > 0).
                                                                                    Aggregate(
                                                                                        current1,
                                                                                        (current, differencesPair) =>
                                                                                        Math.Max(
                                                                                            current,
                                                                                            differencesPair.Value.Count)));


            const string titleName = "Programs Differences List";
            const int patchIdLength = 9;
            var lineLength = Math.Max("|Diffs||".Length + max*patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Program|".Length + patchIdLength + ":".Length +
                24 + "|".Length); // 24 = max patch name length
            writer.WriteLine($"+{new string('-', lineLength - "++".Length)}+");
            writer.WriteLine(
                $"|{titleName}{new string(' ', lineLength - titleName.Length - "||".Length)}|");
            writer.WriteLine($"+-----+-+{new string('-', lineLength - "+-----+-++".Length)}+");

            // ReSharper disable FormatStringProblem
            foreach (var diffListPair in _diffPrograms.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;
                // ReSharper disable FormatStringProblem
                writer.WriteLine(string.Format(
                        "|Program|{0," + patchTextLength + "}{1}|", patchText,
                        new string(' ', lineLength - "||Program||".Length - patchTextLength + 1)));
                // +1 because of no space at end
                writer.WriteLine($"+-----+-+{new string('-', lineLength - "+-----+-++".Length)}+");
                writer.WriteLine($"|Diffs|Program IDs{new string(' ', lineLength - "|Diffs|Program IDs|".Length)}|");
                writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");

                foreach (
                    var differencesPair in diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                {
                    writer.Write($"|{differencesPair.Key,5}|");

                    foreach (var patch in differencesPair.Value)
                    {
                        writer.Write(
                            "{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ", patch.Id);
                    }
                    writer.WriteLine(
                        $"{new string(' ', lineLength - differencesPair.Value.Count*patchIdLength - "|    4||".Length)}|");
                }
                writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");
            }

            writer.WriteLine(string.Empty);

            // ReSharper restore FormatStringProblem
            // ReSharper restore RedundantStringFormatCall
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputCombisToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall
            var max = _diffCombis.Where(diffListPair => diffListPair.Value.Any(differencesPair =>
                differencesPair.Value.Count > 0)).Aggregate(0, (current1, diffListPair) => diffListPair.Value.Where(
                    differencesPair =>
                    differencesPair.Value.Count > 0).Aggregate(current1, (current, differencesPair) =>
                        Math.Max(current, differencesPair.Value.Count)));


            const string titleName = "Combis Differences List";
            const int patchIdLength = 9;
            var lineLength = Math.Max("|Diffs||".Length + max * patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Combi|".Length + patchIdLength + ":".Length + 24 + "|".Length); 
                        // 24 = max patch name length
            writer.WriteLine($"+{new string('-', lineLength - "++".Length)}+");
            writer.WriteLine($"|{titleName}{new string(' ', lineLength - titleName.Length - "||".Length)}|");
            writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");

            foreach (var diffListPair in _diffCombis.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;

                // ReSharper disable once FormatStringProblem
                writer.WriteLine(string.Format(
                    "|Combi|{0," + patchTextLength + "}{1}|", patchText,
                    new string(' ', lineLength - "||Combi|".Length - patchTextLength)));
                writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");
                writer.WriteLine($"|Diffs|Combi IDs{new string(' ', lineLength - "|Diffs|Combi IDs|".Length)}|");
                writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");

                foreach (var differencesPair in diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                {
                    writer.Write($"|{differencesPair.Key,5}|");

                    foreach (var patch in differencesPair.Value)
                    {
                        // ReSharper disable once FormatStringProblem
                        writer.Write("{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ", patch.Id);
                    }
                    writer.WriteLine(
                        $"{new string(' ', lineLength - differencesPair.Value.Count*patchIdLength - "|    4||".Length)}|");
                }
                writer.WriteLine($"+-----+-------+{new string('-', lineLength - "+-----+-------++".Length)}+");
            }
            writer.WriteLine(string.Empty);
            // ReSharper restore RedundantStringFormatCall
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputSetListSlotsToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall

            var max = _diffSetListSlots.Where(diffListPair => diffListPair.Value.Any(differencesPair =>
                differencesPair.Value.Count > 0)).Aggregate(0, (current1, diffListPair) => diffListPair.Value.Where(
                    differencesPair =>
                    differencesPair.Value.Count > 0).Aggregate(current1, (current, differencesPair) =>
                        Math.Max(current, differencesPair.Value.Count)));


            const string titleName = "Set List Slots Differences List";
            const int patchIdLength = 8;
            var lineLength = Math.Max("|Diffs||".Length + max * patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Set List Slot|".Length + 
                patchIdLength + ":".Length + 24 + "|".Length); // 24 = max patch name length
            writer.WriteLine($"+{new string('-', lineLength - "++".Length)}+");
            writer.WriteLine($"|{titleName}{new string(' ', lineLength - titleName.Length - "||".Length)}|");
            writer.WriteLine($"+-----+-------+{new string('-', lineLength - "+-----+-------++".Length)}+");

            foreach (var diffListPair in _diffSetListSlots.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;

                // ReSharper disable once FormatStringProblem
                writer.WriteLine("|Set List Slot|{0," + patchTextLength + "}{1}|", patchText,
                    new string(' ', lineLength - "||Set List Slot|".Length - 
                        patchTextLength + 1));  // +1 because of no space at end
                writer.WriteLine("+-----+-------+{0}+", new string('-', lineLength - "+-----+-------++".Length));
                writer.WriteLine("|Diffs|Set List Slot IDs{0}|", new string(' ', lineLength -
                                                                                 "|Diffs|Set List Slot IDs|".Length));
                writer.WriteLine("+-----+{0}+", new string('-', lineLength - "+-----++".Length));

                foreach (var differencesPair in diffListPair.Value.Where(differencesPair => 
                    differencesPair.Value.Count > 0))
                {
                    writer.Write("|{0,5}|", differencesPair.Key);

                    foreach (var patch in differencesPair.Value)
                    {
                        // ReSharper once FormatStringProblem
                        writer.Write("{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ", 
                            patch.Id);
                    }
                    writer.WriteLine("{0}|", new string(' ', lineLength - differencesPair.Value.Count * patchIdLength - "|    4||".Length));
                }
                writer.WriteLine("+-----+-+{0}+", new string('-', lineLength - "+-----+-++".Length));
            }
            writer.WriteLine(string.Empty);
        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputDrumKitsToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall

            var max = _diffDrumKits.Where(diffListPair => diffListPair.Value.Any(differencesPair =>
                differencesPair.Value.Count > 0)).Aggregate(0, (current1, diffListPair) => diffListPair.Value.Where(
                    differencesPair =>
                    differencesPair.Value.Count > 0).Aggregate(current1, (current, differencesPair) =>
                        Math.Max(current, differencesPair.Value.Count)));


            const string titleName = "DrumKits Differences List";
            const int patchIdLength = 9; // is variable, ut max is e.g. U-INT035 = 8, plus space
            var lineLength = Math.Max("|Diffs||".Length + max * patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Drum Kit|".Length + patchIdLength + ":".Length +
                24 + "|".Length); // 24 = max patch name length
            writer.WriteLine("+{0}+", new string('-', lineLength - "++".Length));
            writer.WriteLine("|{0}{1}|", titleName, new string(' ', lineLength - titleName.Length - "||".Length));
            writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));

            foreach (var diffListPair in _diffDrumKits.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;

                // ReSharper disable FormatStringProblem
                writer.WriteLine(
                     "|Drum Kit|{0," + patchTextLength + "}{1}|", patchText,
                    new string(' ', lineLength - "||Drum Kit|".Length - patchTextLength + 1));  // +1 because of no space at end
                writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));
                writer.WriteLine("|Diffs|DrumKit IDs{0}|", new string(' ', lineLength - 
                                                                           "|Diffs|DrumKit IDs|".Length));
                writer.WriteLine("+-----+{0}+", new string('-', lineLength - "+-----++".Length));

                foreach (var differencesPair in diffListPair.Value.Where(differencesPair => differencesPair.Value.Count >
                    0))
                {
                    writer.Write("|{0,5}|", differencesPair.Key);

                    foreach (var patch in differencesPair.Value)
                    {
                        // ReSharper disable once FormatStringProblem
                        { writer.Write("{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ", 
                            patch.Id);}
                    }
                    writer.WriteLine("{0}|", new string(' ', lineLength - differencesPair.Value.Count * 
                        patchIdLength - "|    4||".Length));
                }
                writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));
            }
            writer.WriteLine(string.Empty);

            // ReSharper restore RedundantStringFormatCall
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputDrumPatternsToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall

            var max = _diffDrumPatterns.Where(diffListPair => diffListPair.Value.Any(differencesPair =>
                differencesPair.Value.Count > 0)).Aggregate(0, (current1, diffListPair) => diffListPair.Value.Where(
                    differencesPair =>
                    differencesPair.Value.Count > 0).Aggregate(current1, (current, differencesPair) =>
                        Math.Max(current, differencesPair.Value.Count)));


            const string titleName = "DrumPatterns Differences List";
            const int patchIdLength = 9; // is variable, ut max is e.g. U-INT035 = 8, plus space
            var lineLength = Math.Max("|Diffs||".Length + max * patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Drum Pattern|".Length + patchIdLength + ":".Length +
                24 + "|".Length); // 24 = max patch name length
            writer.WriteLine("+{0}+", new string('-', lineLength - "++".Length));
            writer.WriteLine("|{0}{1}|", titleName, new string(' ', lineLength - titleName.Length - "||".Length));
            writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));

            foreach (var diffListPair in _diffDrumPatterns.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;

                // ReSharper disable FormatStringProblem
                writer.WriteLine(
                     "|Drum Pattern|{0," + patchTextLength + "}{1}|", patchText,
                    new string(' ', lineLength - "||Drum Pattern|".Length - patchTextLength + 1));  // +1 because of no space at end
                writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));
                writer.WriteLine("|Diffs|DrumPattern IDs{0}|", new string(' ', lineLength -
                                                                           "|Diffs|DrumPattern IDs|".Length));
                writer.WriteLine("+-----+{0}+", new string('-', lineLength - "+-----++".Length));

                foreach (var differencesPair in diffListPair.Value.Where(differencesPair => 
                    differencesPair.Value.Count > 0))
                {
                    writer.Write("|{0,5}|", differencesPair.Key);

                    foreach (var patch in differencesPair.Value)
                    {
                        // ReSharper disable once FormatStringProblem
                        { writer.Write("{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ",
                            patch.Id); }
                    }
                    writer.WriteLine("{0}|", new string(' ', lineLength - differencesPair.Value.Count * 
                        patchIdLength - "|    4||".Length));
                }
                writer.WriteLine("+-----+--+{0}+", new string('-', lineLength - "+-----+--++".Length));
            }
            writer.WriteLine(string.Empty);

            // ReSharper restore RedundantStringFormatCall
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void OutputWaveSequencesToAsciiTable(TextWriter writer)
        {
            // ReSharper disable RedundantStringFormatCall

            var max = _diffWaveSequences.Where(diffListPair => diffListPair.Value.Any(differencesPair =>
                differencesPair.Value.Count > 0)).Aggregate(0, (current1, diffListPair) => 
                    diffListPair.Value.Where(differencesPair =>
                    differencesPair.Value.Count > 0).Aggregate(current1, (current, differencesPair) =>
                        Math.Max(current, differencesPair.Value.Count)));


            const string titleName = "WaveSequences Differences List";
            const int patchIdLength = 7;
            var lineLength = Math.Max("|Diffs||".Length + max * patchIdLength, "||".Length + titleName.Length);
            lineLength = Math.Max(lineLength, "|Wave Sequence|".Length + patchIdLength + ":".Length + 
                24 + "|".Length); // 24 = max patch name length
            writer.WriteLine($"+{new string('-', lineLength - "++".Length)}+");
            writer.WriteLine($"|{titleName}{new string(' ', lineLength - titleName.Length - "||".Length)}|");
            writer.WriteLine($"+-----+-------+{new string('-', lineLength - "+-----+-------++".Length)}+");

            foreach (var diffListPair in _diffWaveSequences.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchText = diffListPair.Key.Id;
                if (ListSubType == SubType.IncludingPatchName)
                {
                    patchText += ":" + diffListPair.Key.Name;
                }
                var patchTextLength = patchText.Length;

                // ReSharper disable FormatStringProblem
                writer.WriteLine(
                    "|Wave Sequence|{0," + patchTextLength + "}{1}|", patchText,
                    new string(' ', lineLength - "||Wave Sequence|".Length - patchTextLength + 1));  // +1 because of no space at end
                writer.WriteLine($"+-----+-------+{new string('-', lineLength - "+-----+-------++".Length)}+");
                writer.WriteLine(
                    $"|Diffs|Wave Sequence IDs{new string(' ', lineLength - "|Diffs|Wave Sequence IDs|".Length)}|");
                writer.WriteLine($"+-----+{new string('-', lineLength - "+-----++".Length)}+");

                foreach (var differencesPair in diffListPair.Value.Where(differencesPair => 
                    differencesPair.Value.Count > 0))
                {
                    writer.Write($"|{differencesPair.Key,5}|");

                    foreach (var patch in differencesPair.Value)
                    {
                        // ReSharper disable once FormatStringProblem
                        writer.Write("{0," + (patchIdLength - 1).ToString(CultureInfo.InvariantCulture) + "} ",
                            patch.Id);
                    }
                    writer.WriteLine(
                        $"{new string(' ', lineLength - differencesPair.Value.Count*patchIdLength - "|    4||".Length)}|");
                }
                writer.WriteLine($"+-----+-------+{new string('-', lineLength - "+-----+-------++".Length)}+");
            }
            writer.WriteLine(string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputToText(TextWriter writer)
        {
            OutputProgramBanksToText(writer);
            OutputCombiBanksToText(writer);
            OutputSetListsToText(writer);
            OutputDrumKitBanksToText(writer);
            OutputDrumPatternBanksToText(writer);
            OutputWaveSequenceBanksToText(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputProgramBanksToText(TextWriter writer)
        {
            foreach (var diffListPair in _diffPrograms.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                var patchName = (ListSubType == SubType.ExcludingPatchName 
                    ? string.Empty : (":" + diffListPair.Key.Name));
                writer.WriteLine($"Program {diffListPair.Key.Id}{patchName}");

                foreach (
                    var differencesPair in diffListPair.Value.Where(differencesPair =>
                        differencesPair.Value.Count > 0))
                {
                    writer.Write($" {differencesPair.Key} Diffs: ");

                    foreach (var patch in differencesPair.Value)
                    {
                        writer.Write("{0} ", patch.Id);
                    }
                    writer.WriteLine();
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputCombiBanksToText(TextWriter writer)
        {
            if (PcgMemory.CombiBanks != null)
            {
                foreach (var diffListPair in _diffCombis.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    var patchName = (ListSubType == SubType.ExcludingPatchName 
                        ? string.Empty : (":" + diffListPair.Key.Name));
                    writer.WriteLine($"Combi {diffListPair.Key.Id}{patchName}");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.Write($" {differencesPair.Key} Diffs: ");

                        foreach (var patch in differencesPair.Value)
                        {
                            writer.Write("{0} ", patch.Id);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputSetListsToText(TextWriter writer)
        {
            if ((PcgMemory.SetLists != null) && SetListsEnabled)
            {
                foreach (var diffListPair in  _diffSetListSlots.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    var patchName = (ListSubType == SubType.ExcludingPatchName
                        ? string.Empty : (":" + diffListPair.Key.Name));
                    writer.WriteLine($"Set List Slot {diffListPair.Key.Id}{patchName}");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.Write($" {differencesPair.Key} Diffs: ");

                        foreach (var patch in differencesPair.Value)
                        {
                            writer.Write("{0} ", patch.Id);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumKitBanksToText(TextWriter writer)
        {
            if (PcgMemory.DrumKitBanks != null)
            {
                foreach (var diffListPair in _diffDrumKits.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    var patchName = (ListSubType == SubType.ExcludingPatchName
                        ? string.Empty : (":" + diffListPair.Key.Name));
                    writer.WriteLine($"DrumKit {diffListPair.Key.Id}{patchName}");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.Write($" {differencesPair.Key} Diffs: ");

                        foreach (var patch in differencesPair.Value)
                        {
                            writer.Write("{0} ", patch.Id);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumPatternBanksToText(TextWriter writer)
        {
            if (PcgMemory.DrumPatternBanks != null)
            {
                foreach (var diffListPair in _diffDrumPatterns.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    var patchName = (ListSubType == SubType.ExcludingPatchName 
                        ? string.Empty : (":" + diffListPair.Key.Name));
                    writer.WriteLine($"DrumPattern {diffListPair.Key.Id}{patchName}");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.Write($" {differencesPair.Key} Diffs: ");

                        foreach (var patch in differencesPair.Value)
                        {
                            writer.Write("{0} ", patch.Id);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputWaveSequenceBanksToText(TextWriter writer)
        {
            if (PcgMemory.WaveSequenceBanks != null)
            {
                foreach (var diffListPair in _diffWaveSequences.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    var patchName = (ListSubType == SubType.ExcludingPatchName 
                        ? string.Empty : (":" + diffListPair.Key.Name));
                    writer.WriteLine($"Wave Sequence {diffListPair.Key.Id}{patchName}");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.Write($" {differencesPair.Key} Diffs: ");

                        foreach (var patch in differencesPair.Value)
                        {
                            writer.Write("{0} ", patch.Id);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputToCsv(TextWriter writer)
        {
            OutputProgramBanksToCsv(writer);
            OutputCombiBanksToCsv(writer);
            OutputSetListsToCsv(writer);
            OutputDrumKitBanksToCsv(writer);
            OutputDrumPatternBanksToCsv(writer);
            OutputWaveSequenceBanksToCsv(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputProgramBanksToCsv(TextWriter writer)
        {
            foreach (var diffListPair in _diffPrograms.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                foreach (
                    var differencesPair in diffListPair.Value.Where(differencesPair => 
                        differencesPair.Value.Count > 0))
                {
                    foreach (var patch in differencesPair.Value)
                    {
                        if (ListSubType == SubType.ExcludingPatchName)
                        {
                            writer.WriteLine(
                                $"Program, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                        }
                        else
                        {
                            writer.WriteLine(
                                $"Program, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputCombiBanksToCsv(TextWriter writer)
        {
            if (PcgMemory.CombiBanks != null)
            {
                foreach (var diffListPair in _diffCombis.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        foreach (var patch in differencesPair.Value)
                        {
                            if (ListSubType == SubType.ExcludingPatchName)
                            {
                                writer.WriteLine(
                                    $"Combi, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                            }
                            else
                            {
                                writer.WriteLine(
                                    $"Combi, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputSetListsToCsv(TextWriter writer)
        {
            if ((PcgMemory.SetLists) != null && SetListsEnabled)
            {
                foreach (var diffListPair in _diffSetListSlots.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        foreach (var patch in differencesPair.Value)
                        {
                            if (ListSubType == SubType.ExcludingPatchName)
                            {
                                writer.WriteLine(
                                    $"Set List Slot, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                            }
                            else
                            {
                                writer.WriteLine(
                                    $"Set List Slot, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumKitBanksToCsv(TextWriter writer)
        {
            if (PcgMemory.DrumKitBanks != null)
            {
                foreach (var diffListPair in _diffDrumKits.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        foreach (var patch in differencesPair.Value)
                        {
                            if (ListSubType == SubType.ExcludingPatchName)
                            {
                                writer.WriteLine(
                                    $"Drum Kit, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                            }
                            else
                            {
                                writer.WriteLine(
                                    $"Drum Kit, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumPatternBanksToCsv(TextWriter writer)
        {
            if (PcgMemory.DrumPatternBanks != null)
            {
                foreach (var diffListPair in _diffDrumPatterns.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        foreach (var patch in differencesPair.Value)
                        {
                            if (ListSubType == SubType.ExcludingPatchName)
                            {
                                writer.WriteLine(
                                    $"Drum Pattern, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                            }
                            else
                            {
                                writer.WriteLine(
                                    $"Drum Pattern, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputWaveSequenceBanksToCsv(TextWriter writer)
        {
            if (PcgMemory.WaveSequenceBanks != null)
            {
                foreach (var diffListPair in _diffWaveSequences.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        foreach (var patch in differencesPair.Value)
                        {
                            if (ListSubType == SubType.ExcludingPatchName)
                            {
                                writer.WriteLine(
                                    $"Wave Sequence, {diffListPair.Key.Id}, {differencesPair.Key}, {patch.Id}");
                            }
                            else
                            {
                                writer.WriteLine(
                                    $"Wave Sequence, {diffListPair.Key.Id}, {diffListPair.Key.Name}, {differencesPair.Key}, {patch.Id}, {patch.Name}");
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputToXml(TextWriter writer)
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine(
                $"<?xml-stylesheet type=\"text/xsl\" href=\"{Path.ChangeExtension(OutputFileName, "xml")}\"?>");
            writer.WriteLine("<differences_list xml:lang=\"en\">");

            OutputProgramBanksToXml(writer);
            OutputCombiBanksToXml(writer);
            OutputSetListsToXml(writer);
            OutputDrumKitBanksToXml(writer);
            OutputDrumPatternBanksToXml(writer);
            OutputWaveSequenceBanksToXml(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputProgramBanksToXml(TextWriter writer)
        {
            foreach (var diffListPair in _diffPrograms.Where(
                diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
            {
                writer.WriteLine("  <patch>");
                writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                foreach (
                    var differencesPair in diffListPair.Value.Where(differencesPair => 
                        differencesPair.Value.Count > 0))
                {
                    writer.WriteLine("    <diffs>");
                    writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                    writer.WriteLine("      <list>");
                    foreach (var patch in differencesPair.Value)
                    {
                        writer.WriteLine($"        <patch>{patch.Id}</patch>");
                    }
                    writer.WriteLine("      </list>");
                    writer.WriteLine("    </diffs>");
                }
                writer.WriteLine("  </patch>");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputCombiBanksToXml(TextWriter writer)
        {
            if (PcgMemory.CombiBanks != null)
            {
                foreach (var diffListPair in _diffCombis.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    writer.WriteLine("  <combi>");
                    writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.WriteLine("    <diffs>");
                        writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                        writer.WriteLine("      <list>");
                        foreach (var patch in differencesPair.Value)
                        {
                            writer.WriteLine($"        <combi>{patch.Id}</combi>");
                        }
                        writer.WriteLine("      </list>");
                        writer.WriteLine("    </diffs>");
                    }
                    writer.WriteLine("  </combi>");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputSetListsToXml(TextWriter writer)
        {
            if ((PcgMemory.SetLists != null) && SetListsEnabled)
            {
                foreach (var diffListPair in _diffSetListSlots.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    writer.WriteLine("  <setlistslot>");
                    writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.WriteLine("    <diffs>");
                        writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                        writer.WriteLine("      <list>");
                        foreach (var patch in differencesPair.Value)
                        {
                            writer.WriteLine($"        <setlistslot>{patch.Id}</setlistslot>");
                        }
                        writer.WriteLine("      </list>");
                        writer.WriteLine("    </diffs>");
                    }
                    writer.WriteLine("  </setlistslot>");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumKitBanksToXml(TextWriter writer)
        {
            if (PcgMemory.DrumKitBanks != null)
            {
                foreach (var diffListPair in _diffDrumKits.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    writer.WriteLine("  <drumkit>");
                    writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.WriteLine("    <diffs>");
                        writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                        writer.WriteLine("      <list>");
                        foreach (var patch in differencesPair.Value)
                        {
                            writer.WriteLine($"        <drumkit>{patch.Id}</drumkit>");
                        }
                        writer.WriteLine("      </list>");
                        writer.WriteLine("    </diffs>");
                    }
                    writer.WriteLine("  </drumkit>");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputDrumPatternBanksToXml(TextWriter writer)
        {
            if (PcgMemory.DrumPatternBanks != null)
            {
                foreach (var diffListPair in _diffDrumPatterns.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    writer.WriteLine("  <drumpattern>");
                    writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.WriteLine("    <diffs>");
                        writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                        writer.WriteLine("      <list>");
                        foreach (var patch in differencesPair.Value)
                        {
                            writer.WriteLine($"        <drumpattern>{patch.Id}</drumpattern>");
                        }
                        writer.WriteLine("      </list>");
                        writer.WriteLine("    </diffs>");
                    }
                    writer.WriteLine("  </drumpattern>");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void OutputWaveSequenceBanksToXml(TextWriter writer)
        {
            if (PcgMemory.WaveSequenceBanks != null)
            {
                foreach (var diffListPair in _diffWaveSequences.Where(
                    diffListPair => diffListPair.Value.Any(differencesPair => differencesPair.Value.Count > 0)))
                {
                    writer.WriteLine("  <wavesequence>");
                    writer.WriteLine($"    <id>{diffListPair.Key.Id}</id>");

                    foreach (
                        var differencesPair in
                            diffListPair.Value.Where(differencesPair => differencesPair.Value.Count > 0))
                    {
                        writer.WriteLine("    <diffs>");
                        writer.WriteLine($"      <amount>{differencesPair.Key}</amount>");
                        writer.WriteLine("      <list>");
                        foreach (var patch in differencesPair.Value)
                        {
                            writer.WriteLine($"        <wavesequence>{patch.Id}</wavesequence>");
                        }
                        writer.WriteLine("      </list>");
                        writer.WriteLine("    </diffs>");
                    }
                    writer.WriteLine("  </wavesequence>");
                }
                writer.WriteLine("</differences_list>");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        void WriteXslFile()
        {
            var builder = new StringBuilder();
            //IMPR; not a simple table

            File.WriteAllText(Path.ChangeExtension(OutputFileName, "xsl"), builder.ToString());
        }
    }
}
