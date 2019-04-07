// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISongs 
    {
        /// <summary>
        /// 
        /// </summary>
        ObservableCollectionEx<ISong> SongCollection { get; }
    }
}
