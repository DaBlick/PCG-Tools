// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;
using Common.Utils;
using PcgTools.Model.Common.File;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.OpenedFiles;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public class Song : ObservableObject, ISong
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        [UsedImplicitly]
        private int Index { [Annotations.UsedImplicitly] get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ISongMemory Memory { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public ISongTimbres Timbres { get; set; }


        /// <summary>
        /// 
        /// </summary>
        string _name;


        /// <summary>
        /// Used for UI control binding for selections.
        /// </summary>
        bool _isSelected;


        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public string Name
        {
            [Annotations.UsedImplicitly] get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public int MaxNameLength
        {
            get
            {
                throw new System.NotImplementedException(); 
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public bool IsNameLike(string strName)
        {
            throw new System.NotImplementedException();
        }


        public void SetNameSuffix(string suffix)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="memory"></param>
        /// <param name="name"></param>
        public Song(SongFileReader reader, int index, ISongMemory memory, string name)
        {
            Index = index;
            Memory = memory;
            Name = name;

            Timbres = new SongTimbres(this);

            for (var timbreIndex = 0; timbreIndex < reader.NumberOfSongTracks; timbreIndex++)
            {
                Timbres.TimbresCollection.Add(reader.CreateTimbre(Timbres, timbreIndex));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IMemory Root => Memory;


        /// <summary>
        /// 
        /// </summary>
        public INavigable Parent => Memory;


        /// <summary>
        /// Unused for songs.
        /// </summary>
        public int ByteOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int ByteLength { get; set; }


        /// <summary>
        /// PCG file the song is connected with; should be of same model.
        /// </summary>
        public OpenedPcgWindow ConnectedPcgWindow { get;  set; }
    }
}
