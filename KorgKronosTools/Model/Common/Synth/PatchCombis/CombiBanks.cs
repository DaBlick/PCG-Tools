// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.PatchCombis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CombiBanks : Banks<ICombiBank>, ICombiBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected CombiBanks(IPcgMemory pcgMemory) : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void CreateBanks();
        

        /// <summary>
        /// 
        /// </summary>
        public override void Fill()
        {
            CreateBanks();
            FillCombis();
        }

        
        /// <summary>
        /// 
        /// </summary>
        void FillCombis()
        {
            foreach (var bank in BankCollection)
            {
                for (var index = 0; index < bank.NrOfPatches; index++)
                {
                    bank.CreatePatch(index);
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changes"></param>
        /// <param name="pcgMemory">PCG Memory of changes</param>
        public void ChangeTimbreReferences(Dictionary<IProgram, IProgram> changes, IPcgMemory pcgMemory)
        {
            foreach (var timbre in from bank in BankCollection
                                   where bank.IsFilled
                                   from combi in bank.Patches
                                   from timbre in ((Combi)combi).Timbres.TimbresCollection
                                   where  (!timbre.UsedProgram.IsFromMasterFile || timbre.UsedProgram.PcgRoot == pcgMemory) &&
                                   changes.ContainsKey(timbre.UsedProgram)
                                   select timbre)
            {
                timbre.UsedProgram = changes[timbre.UsedProgram];
            }
        }


        // Virtual banks.

        /// <summary>
        /// 
        /// </summary>
        public const int FirstVirtualBankId = 0x30;

        
        /// <summary>
        /// 
        /// </summary>
        public const int NumberOfVirtualBanks = 100;


        /// <summary>
        /// Default do not create virtual banks.
        /// </summary>
        protected virtual void CreateVirtualBanks()
        {
        }
    }
}
