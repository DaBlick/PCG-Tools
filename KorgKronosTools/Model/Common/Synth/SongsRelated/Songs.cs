// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public class Songs : ISongs
    {
                /// <summary>
        /// 
        /// </summary>
        public ObservableCollectionEx<ISong> SongCollection { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public Songs()
        {
            SongCollection = new ObservableCollectionEx<ISong>();
        }
    }
}
