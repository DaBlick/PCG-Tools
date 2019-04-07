// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchWaveSequences;
using PcgTools.PcgToolsResources;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ListGenerator : IListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        public IPcgMemory PcgMemory { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum Sort
        {
            TypeBankIndex,
            Categorical,
            Alphabetical
        }


        /// <summary>
        /// 
        /// </summary>
        public Sort SortMethod { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool OptionalColumnCrcIncludingName { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool OptionalColumnCrcExcludingName { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool OptionalColumnSetListSlotReferenceId { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool OptionalColumnSetListSlotReferenceName { protected get; set; }


        /// <summary>
        /// Only used for combi content list.
        /// </summary>
        public enum SubType
        {
            Compact, // Combi content list
            Short, // Combi content list
            Long, // Combi Content list
            IncludingPatchName, // Differences list
            ExcludingPatchName // Differences list
        }


        /// <summary>
        /// 
        /// </summary>
        public SubType ListSubType { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum OutputFormat
        {
            AsciiTable,
            Text,
            Csv,
            Xml
        }


        /// <summary>
        /// 
        /// </summary>
        public OutputFormat ListOutputFormat { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterOnText { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterCaseSensitive { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string FilterText { protected get; set; }


// ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// 
        /// </summary>
        public bool FilterProgramNames { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterCombiNames { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterSetListSlotNames { protected get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


        /// <summary>
        /// 
        /// </summary>
        public bool FilterSetListSlotDescription { protected get; set; }


// ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// 
        /// </summary>
        public bool FilterWaveSequenceNames { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterDrumKitNames { protected get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterDrumPatternNames { protected get; set; }

// ReSharper restore UnusedAutoPropertyAccessor.Global


        /// <summary>
        /// </summary>
        public IObservableBankCollection<IProgramBank> SelectedProgramBanks { get; set; } 


        /// <summary>
        /// 
        /// </summary>
        public List<string> FilterProgramBankNames { private get; set; } // Only used for CLI


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitPrograms { get; set; }


        /// <summary>
        /// </summary>
        public IObservableBankCollection<ICombiBank> SelectedCombiBanks { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<string> FilterCombiBankNames { private get; set; } // Only used for CLI


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitCombis { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreFirstProgram { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreMutedOffTimbres { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreMutedOffFirstProgramTimbre { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        public bool SetListsEnabled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitSetListSlots { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public int SetListsRangeFrom { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int SetListsRangeTo { get; set; }


// ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// 
        /// </summary>
        public bool DrumKitsEnabled { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitDrumKits { get; set; }


        /// <summary>
        /// </summary>
        public IObservableBankCollection<IDrumKitBank> SelectedDrumKitBanks { get; set; }


        // ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// 
        /// </summary>
        public bool DrumPatternsEnabled { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitDrumPatterns { get; set; }


        /// <summary>
        /// </summary>
        public IObservableBankCollection<IDrumPatternBank> SelectedDrumPatternBanks { get; set; }


// ReSharper disable UnusedAutoPropertyAccessor.Global
        /// <summary>
        /// 
        /// </summary>
        public bool WaveSequencesEnabled { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreInitWaveSequences { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IObservableBankCollection<IWaveSequenceBank> SelectedWaveSequenceBanks { get; set; }


        // public List<string> FilterWaveSequenceBankNames { get; set; } // Only used for CLI when banks are supported


        /// <summary>
        /// 
        /// </summary>
        public string OutputFileName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public enum FilterOnFavorites
        {
            All,
            No,
            Yes
        }


        /// <summary>
        /// 
        /// </summary>
        public FilterOnFavorites ListFilterOnFavorites { protected get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        protected ListGenerator()
        {
            SelectedProgramBanks = new ObservableBankCollection<IProgramBank>();
            SelectedCombiBanks = new ObservableBankCollection<ICombiBank>();
            SelectedDrumKitBanks = new ObservableBankCollection<IDrumKitBank>();
            SelectedDrumPatternBanks = new ObservableBankCollection<IDrumPatternBank>();
            SelectedWaveSequenceBanks = new ObservableBankCollection<IWaveSequenceBank>(); 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        protected abstract string RunAfterFilteringBanks(bool useFileWriter = true);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="useFileWriter"></param>
        /// <returns></returns>
        public string Run(bool useFileWriter = true)
        {
#if !DEBUG
            string errorText;

            try
            {
#endif
                FilterBanks();
                return RunAfterFilteringBanks(useFileWriter);
#if !DEBUG
            }
            catch (ArgumentException exception)
            {
                errorText = exception.Message;
            }
            catch (IOException exception)
            {
                errorText = exception.Message;
            }
            catch (NotSupportedException exception)
            {
                errorText = exception.Message;
            }
            catch (Win32Exception exception)
            {
                errorText = exception.Message;
            }
            catch (UnauthorizedAccessException exception)
            {
                errorText = exception.Message;
            }

            //if (!String.IsNullOrEmpty(errorText))
            //{
            //    throw new ApplicationException(errorText);   
            //}

            // Inv: Exception has occurred.
            WindowUtils.ShowMessageBox(null, String.Format("{0}: {1}", Strings.GenerateListError, errorText), 
                Strings.PcgTools,
                WindowUtils.EMessageBoxButton.Ok, WindowUtils.EMessageBoxImage.Error,
                WindowUtils.EMessageBoxResult.Ok);
            return null;
#endif
        }


        /// <summary>
        /// Only used when command line arguments are used.
        /// </summary>
        private void FilterBanks()
        {
            if (FilterProgramBankNames != null)
            {
                for (var index = SelectedProgramBanks.Count - 1; index >= 0; index--)
                {
                    var bank = SelectedProgramBanks[index];

                    if (!FilterProgramBankNames.Contains(bank.Id))
                    {
                        SelectedProgramBanks.Remove(bank);
                    }
                }
            }

            if (FilterCombiBankNames != null)
            {
                for (var index = SelectedCombiBanks.Count - 1; index >= 0; index--)
                {
                    var bank = SelectedCombiBanks[index];
                    if (!FilterCombiBankNames.Contains(bank.Id))
                    {
                        SelectedCombiBanks.Remove(bank);
                    }
                }
            }
        }
    }
}
