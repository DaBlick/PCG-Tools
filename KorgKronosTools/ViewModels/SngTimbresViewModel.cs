// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.ComponentModel;
using System.Linq;
using Common.Utils;
using PcgTools.Model.Common.Synth.SongsRelated;

namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class SngTimbresViewModel : ViewModel, ISngTimbresViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ISong Song { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        private ISongViewModel SongViewModel { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="songViewModel"></param>
        public SngTimbresViewModel(ISongViewModel songViewModel)
        {
            SongViewModel = songViewModel;

            Song = SongViewModel.Song;

            // Select first if none selected.
            if ((Song.Timbres.TimbresCollection.Any()) && 
                (Song.Timbres.TimbresCollection.Count(item => item.IsSelected) == 0))
            {
                Song.Timbres.TimbresCollection[0].IsSelected = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Action UpdateUiContent { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exit"></param>
        /// <returns></returns>
        public override bool Close(bool exit)
        {
            CloseWindow();
            return true;
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [UsedImplicitly]
        void OnSongRootChanged(object sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            {
               // default: Ignore file name (and possibly more.
            }
        }
    }
}



