// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Linq;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;

namespace PcgTools.Model.Common.File
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PatchesFileReader :FileReader, IPatchesFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        protected int Index { get; set; }


        /// <summary>
        /// 
        /// </summary>
        protected IPcgMemory CurrentPcgMemory { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        protected PatchesFileReader(IPcgMemory currentPcgMemory, byte[] content)
        {
            CurrentPcgMemory = currentPcgMemory;
            CurrentPcgMemory.Content = content;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="modelType"></param>
        public abstract void ReadContent(Memory.FileType fileType, 
            Models.EModelType modelType);


        /// <summary>
        /// 
        /// </summary>
        protected void SetNotifications()
        {
            foreach (var patch in CurrentPcgMemory.ProgramBanks.BankCollection.SelectMany(bank => bank.Patches))
            {
                patch.SetNotifications();
            }

            foreach (var patch in CurrentPcgMemory.CombiBanks.BankCollection.SelectMany(bank => bank.Patches))
            {
                patch.SetNotifications();
            }

            if (CurrentPcgMemory.SetLists != null)
            {
                foreach (IPatch patch in CurrentPcgMemory.SetLists.BankCollection.SelectMany(bank => bank.Patches))
                {
                    patch.SetNotifications();
                }
            }
        }


        // <summary>
        // Length of one timbre.
        // </summary>
        //protected int TimbreByteSize { get; set; }
    }
}