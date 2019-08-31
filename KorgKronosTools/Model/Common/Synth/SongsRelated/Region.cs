// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;
using Common.Utils;

namespace PcgTools.Model.Common.Synth.SongsRelated
{
    /// <summary>
    /// 
    /// </summary>
    public class Region : ObservableObject, IRegion
    {
        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public int Index { [Annotations.UsedImplicitly] get; private set; }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string Name { [Annotations.UsedImplicitly] get; private set; }


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        // ReSharper disable once MemberCanBePrivate.Global
        public string SampleFileName { [Annotations.UsedImplicitly] get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="sampleFileName"></param>
        public Region(int index, string name, string sampleFileName)
        {
            Index = index;
            Name = name;
            SampleFileName = sampleFileName;
        }


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

    }
}
