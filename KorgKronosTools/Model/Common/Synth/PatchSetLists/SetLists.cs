// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.PatchSetLists
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SetLists : Banks<SetList>, ISetLists
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected SetLists(IPcgMemory pcgMemory) : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void CreateSetLists();


        /// <summary>
        /// 
        /// </summary>
        public override void Fill()
        {
            CreateSetLists();
            FillSetLists();
        }


        /// <summary>
        /// 
        /// </summary>
        void FillSetLists()
        {
            foreach (var setList in BankCollection)
            {
                for (var index = 0; index < setList.NrOfPatches; index++)
                {
                    setList.CreatePatch(index);
                }
            }
        }


        /// <summary>
        /// Changes program references; only used from set lists not from a master file.
        /// </summary>
        /// <param name="changes"></param>
        public void ChangeProgramReferences(Dictionary<IProgram, IProgram> changes)
        {
            foreach (var setListSlot in BankCollection.Where(
                bank => bank.IsFilled).SelectMany(bank => bank.Patches).Where(
                    setListSlot => (setListSlot.IsLoaded) &&
                                   (((ISetListSlot) setListSlot).SelectedPatchType == SetListSlot.PatchType.Program) &&
                                   changes.ContainsKey((IProgram) ((ISetListSlot) (setListSlot)).UsedPatch)))
            {
                ((ISetListSlot) setListSlot).UsedPatch =
                    changes[(IProgram) (((ISetListSlot) setListSlot).UsedPatch)];
            }
        }


        /// <summary>
        /// /// Changes combi references; only used from set lists not from a master file.
        /// </summary>
        /// <param name="changes"></param>
        public void ChangeCombiReferences(Dictionary<ICombi, ICombi> changes)
        {
            foreach (
                var setListSlot in
                    BankCollection.Where(bank => bank.IsFilled)
                        .SelectMany(bank => bank.Patches)
                        .Where(setListSlot => (setListSlot.IsLoaded) &&
                                              (((ISetListSlot) setListSlot).SelectedPatchType ==
                                               SetListSlot.PatchType.Combi) &&
                                              changes.ContainsKey((ICombi) ((ISetListSlot) (setListSlot)).UsedPatch)))
            {
                ((ISetListSlot) setListSlot).UsedPatch =
                    changes[(ICombi) (((ISetListSlot) setListSlot).UsedPatch)];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get { throw new NotSupportedException(); } }


        /// <summary>
        /// 
        /// </summary>
        public int Stl2PcgOffset { get; set; }
    }
}
