// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Common.Extensions;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchSorting;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public class ListGeneratorPatchList : ListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        List<IPatch> _list;


        /// <summary>
        /// 
        /// </summary>
        bool _areFavoritesSupported;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected override string RunAfterFilteringBanks(bool useFileWriter = true)
        {
            _areFavoritesSupported = PcgMemory.AreFavoritesSupported;

            using (var writer = File.CreateText(OutputFileName))
            {
                _list = new List<IPatch>();
                CreateTypeBankIndexSortedProgramsList();
                CreateTypeBankIndexSortedCombisList();
                CreateTypeBankIndexSortedSetListSlotsList();
                CreateTypeBankIndexSortedDrumKitsList();
                CreateTypeBankIndexSortedDrumPatternsList();
                CreateTypeBankIndexSortedWaveSequencesList();

                switch (SortMethod)
                {
                case Sort.TypeBankIndex:
                    // No action needed (list is built up this way.
                    break;

                case Sort.Alphabetical:
                    _list.Sort();
                    break;

                case Sort.Categorical:
                    PatchSorter.SortBy(_list, PatchSorter.SortOrder.ESortOrderCategoryName);
                    break;

                default:
                    throw new NotSupportedException("Unsupported sort");
                }

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
        private void CreateTypeBankIndexSortedProgramsList()
        {
            foreach (var patch in SelectedProgramBanks.SelectMany(
                programBank =>
                (from program in programBank.Patches
                 where programBank.IsLoaded &&
                       program.UseInList(
                           IgnoreInitPrograms, FilterOnText, FilterText, FilterCaseSensitive,
                           ListFilterOnFavorites, false)
                 select program)))
            {
                _list.Add(patch);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateTypeBankIndexSortedCombisList()
        {
            foreach (var patch in SelectedCombiBanks.SelectMany(
                combiBank =>
                (from combi in combiBank.Patches
                 where
                     combiBank.IsLoaded &&
                     combi.UseInList(
                         IgnoreInitCombis, FilterOnText, FilterText, FilterCaseSensitive, ListFilterOnFavorites,
                         false)
                 select combi)))
            {
                _list.Add(patch);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateTypeBankIndexSortedSetListSlotsList()
        {
            for (var setListIndex = 0; setListIndex < 128; setListIndex++)
            {
                if ((PcgMemory.SetLists == null) ||
                    !SetListsEnabled ||
                    (SetListsRangeFrom > setListIndex) ||
                    (SetListsRangeTo < setListIndex))
                {
                    continue;
                }

                foreach (var setListSlot in
                    (from setListSlot in ((SetList) PcgMemory.SetLists[setListIndex]).Patches
                     where
                         ((IBank) setListSlot.Parent).IsLoaded &&
                         setListSlot.UseInList(
                             IgnoreInitSetListSlots, FilterOnText, FilterText,
                             FilterCaseSensitive,
                             FilterOnFavorites.All, FilterSetListSlotDescription)
                     select setListSlot).Where(setListSlot => setListSlot != null))
                {
                    _list.Add(setListSlot);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateTypeBankIndexSortedDrumKitsList()
        {
            foreach (var patch in SelectedDrumKitBanks.SelectMany(
                drumKitBank =>
                (from drumKit in drumKitBank.Patches
                 where drumKitBank.IsLoaded &&
                       drumKit.UseInList(
                           IgnoreInitDrumKits, FilterOnText, FilterText,
                           FilterCaseSensitive, ListFilterOnFavorites, false)
                 select drumKit)))
            {
                _list.Add(patch);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private void CreateTypeBankIndexSortedDrumPatternsList()
        {
            foreach (var patch in SelectedDrumPatternBanks.SelectMany(
                drumPatternBank =>
                (from drumPattern in drumPatternBank.Patches
                 where drumPatternBank.IsLoaded &&
                       drumPattern.UseInList(
                           IgnoreInitDrumPatterns, FilterOnText, FilterText,
                           FilterCaseSensitive, ListFilterOnFavorites, false)
                 select drumPattern)))
            {
                _list.Add(patch);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CreateTypeBankIndexSortedWaveSequencesList()
        {
            foreach (var patch in SelectedWaveSequenceBanks.SelectMany(
                waveSequenceBank =>
                (from waveSequence in waveSequenceBank.Patches
                 where
                     waveSequenceBank.IsLoaded &&
                     waveSequence.UseInList(
                         IgnoreInitWaveSequences, FilterOnText, FilterText, 
                         FilterCaseSensitive, ListFilterOnFavorites, false)
                 select waveSequence)))
            {
                _list.Add(patch);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void WriteToFile(TextWriter writer)
        {
            // Print header.
            var setListSlotInList = _list.OfType<SetListSlot>().Any();
            var setListSlotMaxSize = 0;

            var favoriteLineHeaderText = _areFavoritesSupported ? "---+" : string.Empty;
            var favoriteHeaderText = _areFavoritesSupported ? "Fav|" : string.Empty;

// ReSharper disable LoopCanBeConvertedToQuery, do NOT change into LINQ (otherwise it will not work anymore)
            foreach (var patch in _list)            
// ReSharper restore LoopCanBeConvertedToQuery
            {
                var slot = patch as SetListSlot;
                if (slot != null)
                {
                    setListSlotMaxSize = Math.Max(setListSlotMaxSize, slot.Description.Length);
                }
            }

            var asciiTableHeaderLine = WriteHeader(writer, setListSlotInList, favoriteLineHeaderText, 
                setListSlotMaxSize, favoriteHeaderText);

            WritePatchesToFile(writer, setListSlotInList, setListSlotMaxSize);

            switch (ListOutputFormat)
            {
            case OutputFormat.AsciiTable:
                writer.WriteLine(asciiTableHeaderLine);
                break;

            case OutputFormat.Xml:
                writer.WriteLine("</patch_list>");
                break;

            //default:
                // No action required.
                //break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        private void WritePatchesToFile(TextWriter writer, bool setListSlotInList, int setListSlotMaxSize)
        {
            foreach (var patch in _list)
            {
                var description = string.Empty;
                var setListName = string.Empty;
                var patchType = string.Empty;
                var isFavorite = " ";
                var category = string.Empty;
                var subCategory = string.Empty;
                var checksumIncludingName = OptionalColumnCrcIncludingName
                    ? $"{patch.CalcCrc(true),6} "
                    : string.Empty;
                var checksumExcludingName = OptionalColumnCrcExcludingName
                    ? $"{patch.CalcCrc(false),6} "
                    : string.Empty;
                var setListSlotReferenceId = string.Empty;
                if (OptionalColumnSetListSlotReferenceId)
                {
                    var slot = patch as ISetListSlot;
                    var usedPatch = slot?.UsedPatch;
                    if (usedPatch is IProgram)
                    {
                        setListSlotReferenceId = $"Prg {usedPatch.Id,-8}";
                    }
                    else if (usedPatch is ICombi)
                    {
                        setListSlotReferenceId = $"Cmb {usedPatch.Id,-8}";
                    }
                    else
                    {
                        setListSlotReferenceId = $"{"",-12}";
                    }
                }

                var setListSlotReferenceName = string.Empty;
                if (OptionalColumnSetListSlotReferenceName)
                {
                    var slot = patch as ISetListSlot;
                    var usedPatch = slot?.UsedPatch;
                    if (usedPatch is IProgram || usedPatch is ICombi)
                    {
                        setListSlotReferenceName = $"{usedPatch.Name,-24}";
                    }
                    else
                    {
                        setListSlotReferenceName = $"{"",-24}";
                    }
                }

                var program = patch as IProgram;
                if (program != null)
                {
                    patchType = ParseProgram(patchType, program, ref isFavorite, ref category, ref subCategory);
                }
                else
                {
                    var combi = patch as ICombi;
                    if (combi != null)
                    {
                        isFavorite = ParseCombi(isFavorite, combi, ref patchType, ref category, ref subCategory);
                    }
                    else
                    {
                        var slot = patch as ISetListSlot;
                        if (slot != null)
                        {
                            patchType = ParseSetListSlot(patchType, patch, slot, ref setListName, ref description);
                        }
                        else if (patch is IDrumKit)
                        {
                            patchType = "DrumKit";
                        }
                        else if (patch is IDrumPattern)
                        {
                            patchType = "DrumPattern";
                        }
                        else if (patch is IWaveSequence)
                        {
                            patchType = "WaveSequence";
                        }
                        else
                        {
                            Debug.Fail("Error in switch");
                        }
                    }
                }

                WritePatch(writer, setListSlotInList, setListSlotMaxSize, patch, isFavorite, patchType, 
                    category, subCategory, checksumIncludingName, checksumExcludingName, setListName,
                    setListSlotReferenceId, setListSlotReferenceName, description);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchType"></param>
        /// <param name="program"></param>
        /// <param name="isFavorite"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
// ReSharper disable once RedundantAssignment
        private string ParseProgram(string patchType, IProgram program, ref string isFavorite, ref string category,
            ref string subCategory)
        {
            patchType = "Program";
            if (((IProgramBank) (program.Parent)).Type != BankType.EType.Gm)
            {
                isFavorite = (_areFavoritesSupported && program.GetParam(ParameterNames.ProgramParameterName.Favorite).Value) ? "X" : " ";
                category = program.CategoryAsName;
                subCategory = program.SubCategoryAsName;
            }
            return patchType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="isFavorite"></param>
        /// <param name="combi"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        private string ParseCombi(string isFavorite, ICombi combi, ref string patchType, ref string category,
            ref string subCategory)
        {
            isFavorite = (_areFavoritesSupported && combi.GetParam(ParameterNames.CombiParameterName.Favorite).Value) ? "X" : " ";
            patchType = "Combi  "; // Align combis and programs because of (sub)category
            category = combi.CategoryAsName;
            subCategory = combi.SubCategoryAsName;
            return isFavorite;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchType"></param>
        /// <param name="patch"></param>
        /// <param name="slot"></param>
        /// <param name="setListName"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static string ParseSetListSlot(string patchType, IPatch patch, ISetListSlot slot, ref string setListName,
            ref string description)
        {
            patchType = "SetListSlot";
            // Set list slots do not have to be aligned, because they have no (sub)category
            setListName = ((SetList) patch.Parent).Name;
            description = $"{slot.Description.Replace('\n', '\\')}";
            return patchType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WritePatch(TextWriter writer, bool setListSlotInList, int setListSlotMaxSize, IPatch patch,
            string isFavorite, string patchType, string category, string subCategory, string checksumIncludingName,
            string checksumExcludingName, string setListName, string setListSlotReferenceId,
            string setListSlotReferenceName, string description)
        {
            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    WriteAsciiTablePatch(writer, patch, isFavorite, patchType, category, subCategory, checksumIncludingName,
                        checksumExcludingName, setListSlotInList, setListSlotMaxSize, setListName, setListSlotReferenceId,
                        setListSlotReferenceName, description);
                    break;

                case OutputFormat.Text:
                    WriteTextPatch(writer, patch, isFavorite, patchType, category, subCategory, checksumIncludingName,
                        checksumExcludingName, setListSlotInList, setListSlotMaxSize, setListName, setListSlotReferenceId,
                        setListSlotReferenceName, description);

                    break;

                case OutputFormat.Csv:
                    WriteCsvPatch(writer, isFavorite, patch, patchType, category, subCategory, checksumIncludingName,
                        checksumExcludingName, setListSlotInList, setListName, setListSlotReferenceId,
                        setListSlotReferenceName, description);
                    break;

                case OutputFormat.Xml:
                    WriteXmlPatch(writer, patchType, patch, isFavorite, category, subCategory, setListName, 
                        setListSlotReferenceId, setListSlotReferenceName, description);
                    break;

                default:
                    throw new NotSupportedException("Unsupported output");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="favoriteLineHeaderText"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="favoriteHeaderText"></param>
        /// <returns></returns>
        private string WriteHeader(TextWriter writer, bool setListSlotInList, string favoriteLineHeaderText,
            int setListSlotMaxSize, string favoriteHeaderText)
        {
            var categoryHeaderName =
                PcgMemory.UsesCategoriesAndSubCategories ? Strings.Category : Strings.Genre;
            var subCategoryHeaderName =
                PcgMemory.UsesCategoriesAndSubCategories ? Strings.SubCategory : Strings.Category;

            string asciiTableHeaderLine = string.Empty;

            switch (ListOutputFormat)
            {
                case OutputFormat.AsciiTable:
                    asciiTableHeaderLine = WriteAsciiTableHeader(writer, setListSlotInList, 
                        favoriteLineHeaderText, setListSlotMaxSize, favoriteHeaderText, asciiTableHeaderLine, 
                        categoryHeaderName, subCategoryHeaderName);
                    break;

                case OutputFormat.Xml:
                    WriteXmlHeader(writer);
                    break;

                //default:
                // No action required.
                //break;
            }
            return asciiTableHeaderLine;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="favoriteLineHeaderText"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="favoriteHeaderText"></param>
        /// <param name="asciiTableHeaderLine"></param>
        /// <param name="categoryHeaderName"></param>
        /// <param name="subCategoryHeaderName"></param>
        /// <returns></returns>
        private string WriteAsciiTableHeader(TextWriter writer, bool setListSlotInList, string favoriteLineHeaderText,
            int setListSlotMaxSize, string favoriteHeaderText, string asciiTableHeaderLine, string categoryHeaderName,
            string subCategoryHeaderName)
        {
            if (setListSlotInList)
            {
                asciiTableHeaderLine = WriteAsciiSetListInSlotTableHeader(
                    writer, favoriteLineHeaderText, setListSlotMaxSize, favoriteHeaderText, 
                    categoryHeaderName, subCategoryHeaderName);
            }
            else
            {
                asciiTableHeaderLine = WriteAsciiNoSetListInSlotTableHeader(
                    writer, favoriteLineHeaderText, favoriteHeaderText, categoryHeaderName, subCategoryHeaderName);
            }
            // ReSharper restore RedundantStringFormatCall
            return asciiTableHeaderLine;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="favoriteLineHeaderText"></param>
        /// <param name="favoriteHeaderText"></param>
        /// <param name="categoryHeaderName"></param>
        /// <param name="subCategoryHeaderName"></param>
        /// <returns></returns>
        private string WriteAsciiNoSetListInSlotTableHeader(TextWriter writer, string favoriteLineHeaderText,
            string favoriteHeaderText, string categoryHeaderName, string subCategoryHeaderName)
        {
            var asciiTableHeaderLine =
                $"+------------------------+------------+-----------+{favoriteLineHeaderText}"+
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? "----------------+" : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? "----------------+" : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? "-------+" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? "-------+" : string.Empty)}";

            writer.WriteLine(asciiTableHeaderLine);

            writer.WriteLine((
                $"|Patch Name              |Patch Type  |Patch ID   |{favoriteHeaderText}" +
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? $"{categoryHeaderName,-16}|" : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? $"{subCategoryHeaderName,-16}|" : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? "CRC Inc|" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? "CRC Exc|" : string.Empty)}")
                    .Trim());

            writer.WriteLine(asciiTableHeaderLine);
            return asciiTableHeaderLine;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="favoriteLineHeaderText"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="favoriteHeaderText"></param>
        /// <param name="categoryHeaderName"></param>
        /// <param name="subCategoryHeaderName"></param>
        /// <returns></returns>
        private string WriteAsciiSetListInSlotTableHeader(TextWriter writer, string favoriteLineHeaderText,
            int setListSlotMaxSize, string favoriteHeaderText, string categoryHeaderName, string subCategoryHeaderName)
        {
            string asciiTableHeaderLine;
// ReSharper disable RedundantStringFormatCall
            asciiTableHeaderLine =
                ($"+------------------------+------------+-----------+{favoriteLineHeaderText}" +
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? "----------------+" : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? "----------------+" : string.Empty)}" + 
                $"{(OptionalColumnCrcIncludingName ? "-------+" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? "-------+" : string.Empty)}" +
                $"------------------------+" +
                $"{(OptionalColumnSetListSlotReferenceId ? "------------+" : string.Empty)}" +
                $"{(OptionalColumnSetListSlotReferenceName ? "------------------------+" : string.Empty)}" +
                $"{new string('-', setListSlotMaxSize) + "+"}");

            writer.WriteLine(asciiTableHeaderLine);

            const string setListSlotDescriptionHeader = "Set List Slot Description";

            writer.WriteLine(
                $"|Patch Name              |Patch Type  |Patch ID   |" +
                $"{favoriteHeaderText}{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? $"{categoryHeaderName,-16}|" : string.Empty)}"+
                $"{(PcgMemory.HasSubCategories ? $"{subCategoryHeaderName,-16}|" : string.Empty)}"+
                $"{(OptionalColumnCrcIncludingName ? "CRC Inc|" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? "CRC Exc|" : string.Empty)}" +
                $"{string.Format($"{Strings.SetList,-24}|", Strings.SetList)}" +
                $"{(OptionalColumnSetListSlotReferenceId ? "Ref Patch ID|" : string.Empty)}" +
                $"{(OptionalColumnSetListSlotReferenceName ? "Ref Patch Name          |" : string.Empty)}" +
                $"{setListSlotDescriptionHeader}" +
                $"{new string(' ', setListSlotMaxSize - setListSlotDescriptionHeader.Length) + "|"}");

            writer.WriteLine(asciiTableHeaderLine);
            return asciiTableHeaderLine;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        private void WriteXmlHeader(TextWriter writer)
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            // ReSharper disable RedundantStringFormatCall
            writer.WriteLine(
                $"<?xml-stylesheet type=\"text/xsl\" href=\"{Path.ChangeExtension(OutputFileName, "xsl")}\"?>");
            // ReSharper restore RedundantStringFormatCall
            writer.WriteLine("<patch_list xml:lang=\"en\">");
        }


        /// <summary>
        /// IMPR: Use separate data class.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteAsciiTablePatch(TextWriter writer, IPatch patch, string isFavorite, string patchType, string category,
            string subCategory, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            int setListSlotMaxSize, string setListName, string setListSlotReferenceId, string setListSlotReferenceName, 
            string description)
        {
            if ((patch is Program) || (patch is Combi))
            {
                WriteAsciiTableProgramOrCombiPatch(writer, patch, isFavorite, patchType, category, subCategory,
                    checksumIncludingName, checksumExcludingName, setListSlotInList, setListSlotMaxSize);
            }
            else if (patch is SetListSlot)
            {
                WriteAsciiTableSetListSlotPatch(writer, patch, patchType, checksumIncludingName, checksumExcludingName, 
                    setListSlotInList, setListSlotMaxSize, setListName, setListSlotReferenceId, setListSlotReferenceName, 
                    description);
            }
            else if ((patch is DrumKit) || (patch is DrumPattern) || (patch is WaveSequence))
            {
                WriteAsciiTableDrumKitOrDrumPatternOrWaveSequencePatch(writer, patch, isFavorite, patchType, checksumIncludingName, 
                    checksumExcludingName, setListSlotInList, setListSlotMaxSize);
            }
            else
            {
                throw new ApplicationException("Illegal switch");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        private void WriteAsciiTableProgramOrCombiPatch(TextWriter writer, IPatch patch, string isFavorite, string patchType,
            string category, string subCategory, string checksumIncludingName, string checksumExcludingName,
            bool setListSlotInList, int setListSlotMaxSize)
        {
            var favoriteText = _areFavoritesSupported
                ? $" {isFavorite} |"
                : string.Empty;
            writer.WriteLine((
                $"|{patch.Name,-24}|{patchType,-12}|{patch.Id,-11}|{favoriteText}" +
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? (category.Equals(string.Empty) ? "                |" : $"{category,-16}|") : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? (subCategory.Equals(string.Empty) ? string.Empty : $"{subCategory,-16}|") : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? checksumIncludingName + "|" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? checksumExcludingName + "|" : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', 24) + "|" : string.Empty)}" +
                $"{(setListSlotInList && OptionalColumnSetListSlotReferenceId ? new string(' ', 12) + "|" : string.Empty)}" +
                $"{(setListSlotInList && OptionalColumnSetListSlotReferenceName ? new string(' ', 24) + "|" : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', setListSlotMaxSize) + "|" : string.Empty)}")
                .Trim());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="setListName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteAsciiTableSetListSlotPatch(TextWriter writer, IPatch patch, string patchType,
            string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,  int setListSlotMaxSize,
            string setListName, string setListSlotReferenceId, string setListSlotReferenceName, string description)
        {
            writer.WriteLine(
                "|{0,-24}|{1,-12}|{2,-11}|{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}|", patch.Name, patchType, patch.Id,
                _areFavoritesSupported ? "   |" : string.Empty, // {3}
                (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? "                |" : string.Empty,
                // {4}
                PcgMemory.HasSubCategories ? "                |" : string.Empty, // {5}
                OptionalColumnCrcIncludingName ? checksumIncludingName + "|" : string.Empty, // {6}
                OptionalColumnCrcExcludingName ? checksumExcludingName + "|" : string.Empty, // {7}
                setListSlotInList
                    ? $"{setListName,-24}" + "|"
                    : new string(' ', 24) + "|", // {8}
                OptionalColumnSetListSlotReferenceId && setListSlotInList ? setListSlotReferenceId + "|" : string.Empty, // {9}
                OptionalColumnSetListSlotReferenceName && setListSlotInList ? setListSlotReferenceName + "|" : string.Empty, // {10}
                description, // {11}
                new string(
                    ' ', setListSlotInList
                        ? (setListSlotMaxSize +
                           description.Count(c => c == '\r') - description.Length)
                        : 0)); // {12}

            // {9}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        private void WriteAsciiTableDrumKitOrDrumPatternOrWaveSequencePatch(TextWriter writer, IPatch patch, string isFavorite,
            string patchType, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            int setListSlotMaxSize)
        {
            writer.WriteLine((
                $"|{patch.Name,-24}|{patchType,-12}|{patch.Id,-11}|{(_areFavoritesSupported ? $" {isFavorite} |" : " ")}" +
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? $"{string.Empty,-16}|" : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? $"{string.Empty,-16}|" : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? checksumIncludingName + "|" : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? checksumExcludingName + "|" : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', 24) + "|" : string.Empty)}" +
                $"{(setListSlotInList && OptionalColumnSetListSlotReferenceId ? new string(' ', 12) + "|" : string.Empty)}" +
                $"{(setListSlotInList && OptionalColumnSetListSlotReferenceName ? new string(' ', 24) + "|" : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', setListSlotMaxSize) + "|" : string.Empty)}")
                    .Trim());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteTextPatch(TextWriter writer, IPatch patch, string isFavorite, string patchType, string category,
            string subCategory, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            int setListSlotMaxSize, string setListName, string setListSlotReferenceId, string setListSlotReferenceName, 
            string description)
        {
            if ((patch is Program) || (patch is Combi))
            {
                WriteTextProgramOrCombiPatch(writer, patch, isFavorite, patchType, category, subCategory,
                    checksumIncludingName, checksumExcludingName, setListSlotInList, setListSlotMaxSize);
            }
            else if (patch is SetListSlot)
            {
                WriteTextSetListSlotPatch(writer, patch, patchType, checksumIncludingName, checksumExcludingName,
                    setListSlotInList, setListSlotMaxSize, setListName, setListSlotReferenceId, setListSlotReferenceName, description);
            }
            else if ((patch is DrumKit) || (patch is DrumPattern) || (patch is WaveSequence))
            {
                WriteTextDrumKitOrDrumPatternOrWaveSequencePatch(writer, patch, isFavorite, patchType, checksumIncludingName,
                    checksumExcludingName, setListSlotInList, setListSlotMaxSize);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        private void WriteTextProgramOrCombiPatch(TextWriter writer, IPatch patch, string isFavorite, string patchType,
            string category, string subCategory, string checksumIncludingName, string checksumExcludingName,
            bool setListSlotInList, int setListSlotMaxSize)
        {
            var favoriteText = _areFavoritesSupported ? $"{isFavorite} " : string.Empty;
            writer.WriteLine((
                $"{patch.Name,-24} {patchType,-12} {patch.Id,-11}{favoriteText}" +
                $"{((PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? (category.Equals(string.Empty) ? string.Empty : $"{category,-16} ") : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? (subCategory.Equals(string.Empty) ? string.Empty : $"{subCategory,-16} ") : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? checksumIncludingName + " " : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? checksumExcludingName + " " : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', setListSlotMaxSize) : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', setListSlotMaxSize) : string.Empty)}")
                    .Trim());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteTextSetListSlotPatch(TextWriter writer, IPatch patch, string patchType, string checksumIncludingName,
            string checksumExcludingName, bool setListSlotInList, int setListSlotMaxSize, string setListName,
             string setListSlotReferenceId, string setListSlotReferenceName, string description)
        {
            writer.WriteLine(
                "{0,-24} {1,-12} {2,-11} {3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", patch.Name, patchType, patch.Id,
                _areFavoritesSupported ? "   " : string.Empty, // {3}
                (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? "                " : string.Empty, // {4}
                PcgMemory.HasSubCategories ? "                " : string.Empty, // {5}
                OptionalColumnCrcIncludingName ? checksumIncludingName + " " : string.Empty, // {6}
                OptionalColumnCrcExcludingName ? checksumExcludingName + " " : string.Empty, // {7}
                setListSlotInList ? $"{setListName,-24} " : string.Empty, // {8}
                OptionalColumnSetListSlotReferenceId && setListSlotInList ? setListSlotReferenceId + " " : string.Empty, // {9}
                OptionalColumnSetListSlotReferenceName && setListSlotInList ? setListSlotReferenceName + " " : string.Empty, // {10}
                setListSlotInList ? description : string.Empty, // {11}
                new string(
                    ' ', setListSlotInList
                        ? (setListSlotMaxSize +
                           description.Count(c => c == '\r') - description.Length)
                        : 0)); // {12}
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patchType"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotMaxSize"></param>
        private void WriteTextDrumKitOrDrumPatternOrWaveSequencePatch(TextWriter writer, IPatch patch, string isFavorite, string patchType,
            string checksumIncludingName, string checksumExcludingName, bool setListSlotInList, int setListSlotMaxSize)
        {
            writer.WriteLine((
                $"{patch.Name,-24} {patchType,-12} {patch.Id,-11}{(_areFavoritesSupported ? $"{isFavorite} " : " ")}" +
                $"{(PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories ? $"{string.Empty,-16} " : string.Empty)}" +
                $"{(PcgMemory.HasSubCategories ? $"{string.Empty,-16} " : string.Empty)}" +
                $"{(OptionalColumnCrcIncludingName ? checksumIncludingName + " " : string.Empty)}" +
                $"{(OptionalColumnCrcExcludingName ? checksumExcludingName + " " : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', 24) : string.Empty)}" +
                $"{(setListSlotInList ? new string(' ', setListSlotMaxSize) : string.Empty)}") // {9}
                .Trim());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isFavorite"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteCsvPatch(TextWriter writer, string isFavorite, IPatch patch, string patchType, string category,
            string subCategory, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            string setListName, string setListSlotReferenceId, string setListSlotReferenceName, string description)
        {
            var favoriteCsvText = _areFavoritesSupported ? $"{isFavorite}," : string.Empty;
            if ((patch is Program) || (patch is Combi))
            {
                WriteCsvProgramOrCombi(writer, patch, patchType, category, subCategory, checksumIncludingName, 
                    checksumExcludingName, setListSlotInList, favoriteCsvText);
            }
            else if (patch is SetListSlot)
            {
                WriteCsvSetListSlot(writer, patch, patchType, checksumIncludingName, checksumExcludingName, setListName,
                    setListSlotInList, setListSlotReferenceId, setListSlotReferenceName, description);
            }
            else if (patch is DrumKit || patch is WaveSequence)
            {
                WriteCsvDrumKitOrDrumPatternOrWaveSequence(writer, patch, patchType, category, subCategory, checksumIncludingName, 
                    checksumExcludingName, setListSlotInList, favoriteCsvText);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="favoriteCsvText"></param>
        private void WriteCsvProgramOrCombi(TextWriter writer, IPatch patch, string patchType, string category,
            string subCategory, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            string favoriteCsvText)
        {
            writer.WriteLine(
                "{0},{1},{2},{3}{4}{5}{6}{7}{8}{9}", patch.Name.Replace(',', '_'), patchType, patch.Id,
                _areFavoritesSupported ? favoriteCsvText + "," : string.Empty,
                (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? category + "," : string.Empty,
                PcgMemory.HasSubCategories ? subCategory + "," : string.Empty,
                OptionalColumnCrcIncludingName ? checksumIncludingName + "," : string.Empty,
                OptionalColumnCrcExcludingName ? checksumExcludingName + "," : string.Empty,
                setListSlotInList ? "," : string.Empty,
                setListSlotInList ? "," : string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteCsvSetListSlot(TextWriter writer, IPatch patch, string patchType, string checksumIncludingName,
            string checksumExcludingName, string setListName, bool setListSlotInList, string setListSlotReferenceId,
            string setListSlotReferenceName, string description)
        {
            writer.WriteLine(
                "{0},{1},{2},{3}{4}{5}{6}{7}{8}{9}{10}{11}", patch.Name.Replace(',', '_'), patchType, patch.Id,
                _areFavoritesSupported ? "," : string.Empty,
                (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? "," : string.Empty,
                PcgMemory.HasSubCategories ? "," : string.Empty,
                OptionalColumnCrcIncludingName ? checksumIncludingName + "," : string.Empty,
                OptionalColumnCrcExcludingName ? checksumExcludingName + "," : string.Empty,
                setListName + ",",
                OptionalColumnSetListSlotReferenceId && setListSlotInList ? setListSlotReferenceId + ",": string.Empty,
                OptionalColumnSetListSlotReferenceName && setListSlotInList ? setListSlotReferenceName + "," : string.Empty,
                description.Replace('\r', ';').Replace('\n', ';')); //
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patch"></param>
        /// <param name="patchType"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="checksumIncludingName"></param>
        /// <param name="checksumExcludingName"></param>
        /// <param name="setListSlotInList"></param>
        /// <param name="favoriteCsvText"></param>
        private void WriteCsvDrumKitOrDrumPatternOrWaveSequence(TextWriter writer, IPatch patch, string patchType, string category,
            string subCategory, string checksumIncludingName, string checksumExcludingName, bool setListSlotInList,
            string favoriteCsvText)
        {
            writer.WriteLine(
                "{0},{1},{2},{3}{4}{5}{6}{7}{8}{9}", patch.Name.Replace(',', '_'), patchType, patch.Id,
                _areFavoritesSupported ? favoriteCsvText + "," : string.Empty,
                (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories) ? category + "," : string.Empty,
                PcgMemory.HasSubCategories ? subCategory + "," : string.Empty,
                OptionalColumnCrcIncludingName ? checksumIncludingName + "," : string.Empty,
                OptionalColumnCrcExcludingName ? checksumExcludingName + "," : string.Empty,
                setListSlotInList ? "," : string.Empty,
                setListSlotInList ? "," : string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="patchType"></param>
        /// <param name="patch"></param>
        /// <param name="isFavorite"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        /// <param name="setListName"></param>
        /// <param name="setListSlotReferenceName"></param>
        /// <param name="description"></param>
        /// <param name="setListSlotReferenceId"></param>
        private void WriteXmlPatch(TextWriter writer, string patchType, IPatch patch, string isFavorite, string category,
            string subCategory, string setListName, string setListSlotReferenceId, string setListSlotReferenceName, 
            string description)
        {
            writer.WriteLine("  <patch>");
            writer.WriteLine("    <type>{0}</type>", patchType);
            writer.WriteLine("    <id>{0}</id>", patch.Id);
            writer.WriteLine("    <name>{0}</name>", patch.Name.ConvertToXml().Trim());
            if (PcgMemory.AreFavoritesSupported)
            {
                writer.WriteLine("    <favorite>{0}</favorite>", 
                    isFavorite);
            }

            if (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories)
            {
                writer.WriteLine("    <category>{0}</category>", 
                    category.ConvertToXml().Trim());
            }

            if (PcgMemory.HasSubCategories)
            {
                writer.WriteLine("    <subcategory>{0}</subcategory>",
                    subCategory.ConvertToXml().Trim());
            }

            if (OptionalColumnCrcIncludingName)
            {
                writer.WriteLine("    <crc_including_name>{0}</crc_including_name>", 
                    patch.CalcCrc(true).ToString().Trim());
            }

            if (OptionalColumnCrcExcludingName)
            {
                writer.WriteLine("    <crc_excluding_name>{0}</crc_excluding_name>", 
                    patch.CalcCrc(false).ToString().Trim());
            }

            if (PcgMemory.SetLists != null)
            {
                writer.WriteLine("    <setlistname>{0}</setlistname>", 
                    setListName.ConvertToXml().Trim());
            }

            if (PcgMemory.SetLists != null)
            {
                writer.WriteLine("    <setlistslotreferenceid>{0}</setlistslotreferenceid>", 
                    setListSlotReferenceId.ConvertToXml().Trim());
            }

            if (PcgMemory.SetLists != null)
            {
                writer.WriteLine("    <setlistslotreferencename>{0}</setlistslotreferencename>", 
                    setListSlotReferenceName.ConvertToXml().Trim());
            }

            if (PcgMemory.SetLists != null)
            {
                writer.WriteLine("    <description>{0}</description>", 
                    description.ConvertToXml().Trim());
            }

            writer.WriteLine("  </patch>");
        }



        /// <summary>
        /// 
        /// </summary>
        void WriteXslFile()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\"?>");
            builder.AppendLine(" <xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">");
            builder.AppendLine(string.Empty);
            builder.AppendLine(" <xsl:template match=\"/\">");
            builder.AppendLine("   <html>");
            builder.AppendLine("   <body>");
            builder.AppendLine("     <h2>Patch List</h2>");
            builder.AppendLine("     <table border=\"1\">");
            builder.AppendLine("       <tr bgcolor=\"#80a0ff\">");

            var setListSlotInList = WriteXslPatchParameters(builder);

            builder.AppendLine("       </tr>");

            WriteXslItemToFile(builder, setListSlotInList);

            builder.AppendLine("     </table>");
            builder.AppendLine("   </body>");
            builder.AppendLine("   </html>");
            builder.AppendLine(" </xsl:template>");
            builder.AppendLine(string.Empty);
            builder.AppendLine(" </xsl:stylesheet>");
            File.WriteAllText(Path.ChangeExtension(OutputFileName, "xsl"), builder.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private bool WriteXslPatchParameters(StringBuilder builder)
        {
            builder.AppendLine("         <th>ListSubType</th>");
            builder.AppendLine("         <th>Id</th>");
            builder.AppendLine("         <th>Name</th>");

            if (_areFavoritesSupported)
            {
                builder.AppendLine("         <th>Fav</th>");
            }

            if (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories)
            {
                var categoryHeaderName =
                    PcgMemory.UsesCategoriesAndSubCategories ? Strings.Category : Strings.Genre;
                builder.AppendLine($"         <th>{categoryHeaderName}</th>");
            }

            if (PcgMemory.HasSubCategories)
            {
                var subCategoryHeaderName =
                    PcgMemory.UsesCategoriesAndSubCategories ? Strings.SubCategory : Strings.Category;
                builder.AppendLine($"         <th>{subCategoryHeaderName}</th>");
            }

            if (OptionalColumnCrcIncludingName)
            {
                builder.AppendLine("         <th>CRC Including Name</th>");
            }

            if (OptionalColumnCrcExcludingName)
            {
                builder.AppendLine("         <th>CRC Excluding Name</th>");
            }

            var setListSlotInList = _list.OfType<SetListSlot>().Any();
            if (setListSlotInList)
            {
                if (OptionalColumnSetListSlotReferenceId)
                {
                    builder.AppendLine("         <th>Set List Slot Reference Id</th>");
                }

                if (OptionalColumnSetListSlotReferenceName)
                { 
                    builder.AppendLine("         <th>Set List Slot Reference Name</th>");
                }

                builder.AppendLine("         <th>Set List Name</th>");
                builder.AppendLine("         <th>Description</th>");
            }
            return setListSlotInList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setListSlotInList"></param>
        private void WriteXslItemToFile(StringBuilder builder, bool setListSlotInList)
        {
            builder.AppendLine("       <xsl:for-each select=\"patch_list/patch\">");
            builder.AppendLine("         <tr>");
            builder.AppendLine("           <td><xsl:value-of select=\"type\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"id\"/></td>");
            builder.AppendLine("           <td><xsl:value-of select=\"name\"/></td>");

            if (_areFavoritesSupported)
            {
                builder.AppendLine("           <td><xsl:value-of select=\"favorite\"/></td>");
            }

            if (PcgMemory.HasProgramCategories || PcgMemory.HasCombiCategories)
            {
                builder.AppendLine("           <td><xsl:value-of select=\"category\"/></td>");
            }

            if (PcgMemory.HasSubCategories)
            {
                builder.AppendLine("           <td><xsl:value-of select=\"subcategory\"/></td>");
            }

            if (OptionalColumnCrcIncludingName)
            {
                builder.AppendLine("         <td><xsl:value-of select=\"crc_including_name\"/></td>");
            }

            if (OptionalColumnCrcExcludingName)
            {
                builder.AppendLine("         <td><xsl:value-of select=\"crc_excluding_name\"/></td>");
            }

            if (setListSlotInList)
            {
                if (OptionalColumnSetListSlotReferenceId)
                {
                    builder.AppendLine("           <td><xsl:value-of select=\"setlistslotreferenceid\"/></td>");
                }

                if (OptionalColumnSetListSlotReferenceName)
                {
                    builder.AppendLine("           <td><xsl:value-of select=\"setlistslotreferencename\"/></td>");
                }

                builder.AppendLine("           <td><xsl:value-of select=\"setlistname\"/></td>");
                builder.AppendLine("           <td><xsl:value-of select=\"description\"/></td>");
            }

            builder.AppendLine("         </tr>");
            builder.AppendLine("       </xsl:for-each>");
        }
    }
}

