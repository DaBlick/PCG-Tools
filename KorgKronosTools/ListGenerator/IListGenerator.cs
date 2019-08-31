// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchWaveSequences;

namespace PcgTools.ListGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public interface IListGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitPrograms { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IObservableBankCollection<IProgramBank> SelectedProgramBanks { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitCombis { get; set; }

        IObservableBankCollection<ICombiBank> SelectedCombiBanks { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreFirstProgram { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreMutedOffTimbres { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreMutedOffFirstProgramTimbre { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool SetListsEnabled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitSetListSlots { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int SetListsRangeFrom { get; set; }


        /// <summary>
        /// 
        /// </summary>
        int SetListsRangeTo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool DrumKitsEnabled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool DrumPatternsEnabled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitDrumKits { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitDrumPatterns { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IObservableBankCollection<IDrumKitBank> SelectedDrumKitBanks { get; }


        /// <summary>
        /// 
        /// </summary>
        IObservableBankCollection<IDrumPatternBank> SelectedDrumPatternBanks { get; }


        /// <summary>
        /// 
        /// </summary>
        bool IgnoreInitWaveSequences { get; set; }


        /// <summary>
        /// 
        /// </summary>
        bool WaveSequencesEnabled { get; set; }


        /// <summary>
        /// 
        /// </summary>
        IObservableBankCollection<IWaveSequenceBank> SelectedWaveSequenceBanks { get; }
    }
}
