// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICombi : IPatch, ICategoriesNamable, IArtistable, ICompleteInPcgable, IReferencable, IDrumTrackReference
    {
        /// <summary>
        /// 
        /// </summary>
        Timbres Timbres { get; }


        /// <summary>
        /// Switches two MIDI channels (of all timbres if they are enabled).
        /// </summary>
        /// <param name="mainMidiChannel"></param>
        /// <param name="secondaryMidiChannel"></param>
        void SwitchMidiChannels(int mainMidiChannel, int secondaryMidiChannel);


        /// <summary>
        /// Returns true if at least one timbre uses the specified MIDI channel and is used.
        /// </summary>
        /// <param name="secondaryMidiChannel"></param>
        /// <returns></returns>
        bool UsesMidiChannel(int secondaryMidiChannel);


        /// <summary>
        /// Init as MIDI MPE command.
        /// </summary>
        void InitAsMpe();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IParameter GetParam(ParameterNames.CombiParameterName name);


        /// <summary>
        /// Minimum volume of all (used) timbres
        /// </summary>
        /// <returns></returns>
        int GetMinimumVolume();


        /// <summary>
        /// Maximum volume of all (used) timbres
        /// </summary>
        /// <returns></returns>
        int GetMaximumVolume();


       /// <summary>
       /// 
       /// </summary>
       /// <param name="parameters"></param>
       /// <param name="minimumVolume"></param>
       /// <param name="maximumVolume"></param>
        void ChangeVolume(ChangeVolumeParameters parameters, int minimumVolume, int maximumVolume);
    }
}
