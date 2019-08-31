// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Mvvm;
using Common.Utils;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.PcgToolsResources;


namespace PcgTools.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class EditParameterViewModel : ViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        private IPcgMemory _memory;


        /// <summary>
        /// Selected patches.
        /// </summary>
        [Annotations.UsedImplicitly]
        public ObservableCollectionEx<IPatch> Patches { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedPatches"></param>
        public EditParameterViewModel(ObservableCollectionEx<IPatch>  selectedPatches)
        {
            Patches = selectedPatches;
            _memory = Patches.Count > 0 ? Patches[0].PcgRoot : null;
            ErrorText = Strings.EditParameterChangeEmpty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="exitMode"></param>
        /// <returns></returns>
        public override bool Close(bool exitMode)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        private string _valueRangeBeforeChange;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public string ValueRangeBeforeChange
        {
            get { return _valueRangeBeforeChange; }
            set
            {
                if (_valueRangeBeforeChange != value)
                {
                    _valueRangeBeforeChange = value;
                    OnPropertyChanged("ValueRangeBeforeChange");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private string _valueRangeAfterChange;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public string ValueRangeAfterChange
        {
            get { return _valueRangeAfterChange; }
            set
            {
                if (_valueRangeAfterChange != value)
                {
                    _valueRangeAfterChange = value;
                    OnPropertyChanged("ValueRangeAfterChange");
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        private string _errorText;


        /// <summary>
        /// 
        /// </summary>
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                if (_errorText != value)
                {
                    _errorText = value;
                    OnPropertyChanged("ErrorText");
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private int _numberOfClippedPatches;


        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public int NumberOfClippedPatches
        {
            get { return _numberOfClippedPatches; }
            set
            {
                if (_numberOfClippedPatches != value)
                {
                    _numberOfClippedPatches = value;
                    OnPropertyChanged("NumberOfClippedPatches");
                }
            }
        }
   }
}

