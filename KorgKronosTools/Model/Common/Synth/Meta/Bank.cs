// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Linq;
using Common.Mvvm;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchInterfaces;

namespace PcgTools.Model.Common.Synth.Meta
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Bank<T> : ObservableObject, IBank where T:IPatch
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IBanks _banks;


        /// <summary>
        /// 
        /// </summary>
        public BankType.EType Type { get; }


        /// <summary>
        /// Default is 128 patches per bank.
        /// </summary>
        public virtual int NrOfPatches {  get { return 128; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="pcgId"></param>
        protected Bank(IBanks banks, BankType.EType type, string id, int pcgId)
        {
            _banks = banks;
            Type = type;

            // A GM bank is always loaded.
            if (type == BankType.EType.Gm)
            {
                IsLoaded = true;
            }

            Id = id;
            PcgId = pcgId;
            Patches = new ObservablePatchCollection();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="otherName"></param>
        /// <returns></returns>
        public bool IsNameLike(string otherName)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="suffix"></param>
        public void SetNameSuffix(string suffix)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public bool FilterForUi => IsWritable;


        /// <summary>
        /// 
        /// </summary>
        public int PcgId { get; }


        /// <summary>
        /// 
        /// </summary>
        public int Index => ((IBanks) Parent).IndexOfBank(this);


        /// <summary>
        /// 
        /// </summary>
        public ObservablePatchCollection Patches { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        protected void Add(T patch)
        {
            Patches.Add(patch);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPatch this[int index] => Patches[index];


        /// <summary>
        /// 
        /// </summary>
        public IMemory Root => _banks.Root;


        /// <summary>
        /// 
        /// </summary>
        protected IPcgMemory PcgRoot => _banks.Root as IPcgMemory;


        /// <summary>
        /// 
        /// </summary>
        public bool IsLoaded { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsWritable { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsFilled { get { return IsLoaded && Patches.Any(program => !program.IsEmptyOrInit); } }


        /// <summary>
        /// 
        /// </summary>
        public string Id { get; private set; }


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
        public INavigable Parent => _banks;


        /// <summary>
        /// 
        /// </summary>
        public virtual int ByteOffset { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int ByteLength { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int CountPatches => Patches.Count;


        /// <summary>
        /// 
        /// </summary>
        public virtual int CountWritablePatches { get { return Patches.Count(patch => ((IBank)(patch.Parent)).IsWritable); } }


        /// <summary>
        /// 
        /// </summary>
        public virtual int CountFilledPatches { get { return Patches.Count(
            patch => ((IBank) (patch.Parent)).IsLoaded && !patch.IsEmptyOrInit); } }

        
        /// <summary>
        /// 
        /// </summary>
        public abstract string Name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int PatchSize { get; set; }


        /// <summary>
        /// Returns true if the patch is present within the master file (in contrary to the file itsself).
        /// </summary>
        public bool IsFromMasterFile
        {
            get
            {
                var masterPcgMemory = MasterFiles.MasterFiles.Instances.FindMasterPcg(Root.Model);
                return (masterPcgMemory == Root);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Update(string name)
        {
            foreach (var item in Patches)
            {
                item.Update(name);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual void SetParameters()
        {
            foreach (var patch in Patches)
            {
                patch.SetParameters();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public abstract void CreatePatch(int index);


        /// <summary>
        /// 
        /// </summary>
        public int CountFilledAndNonEmptyPatches
        {
            get
            {
                return Patches.Count(patch =>
                    ((IBank) (patch.Parent)).IsLoaded && !patch.IsEmptyOrInit && (Type != BankType.EType.Gm));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public virtual int MaxNameLength { get {  throw new NotImplementedException();} }


        /// <summary>
        /// E.g. GM banks have index 0.
        /// </summary>
        public virtual int IndexOffset => 0;


        /// <summary>
        /// 
        /// </summary>
        public virtual void Clear()
        {
            // Nothing to do. Banks do not have any information handled by PCG Tools.
        }
    }
}
