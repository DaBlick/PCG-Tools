// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Linq;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchWaveSequences;

namespace PcgTools.Model.Common.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ProgramBanks : Banks<IProgramBank>, IProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected ProgramBanks(IPcgMemory pcgMemory) : base(pcgMemory)
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
            FillPrograms();
        }


        /// <summary>
        /// 
        /// </summary>
        void FillPrograms()
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
        /// <param name="pcgId"></param>
        /// <returns></returns>
        public override IBank GetBankWithPcgId(int pcgId)
        {
            var bank = base.GetBankWithPcgId(pcgId);

            // GM variation bank selected, select GM bank (last).
            if (bank == null)
            {
                bank = BankCollection.Last();
            }

            return bank;
        }


        /// <summary>
        /// /// Changes drum kit references; only used from programs not from a master file.
        /// </summary>
        /// <param name="changes"></param>

        public void ChangeDrumKitReferences(Dictionary<IDrumKit, IDrumKit> changes)
        {
            foreach (var program in BankCollection.Where(bank => bank.IsFilled)
                        .SelectMany(bank => bank.Patches)
                        .Where(program => program.IsLoaded))
            {
                ((IProgram) program).ReplaceDrumKit(changes);
            }
        }


        /// <summary>
        /// /// Changes wave sequence references; only used from programs not from a master file.
        /// </summary>
        /// <param name="changes"></param>

        public void ChangeWaveSequenceReferences(Dictionary<IWaveSequence, IWaveSequence> changes)
        {
            foreach (var program in BankCollection.Where(bank => bank.IsFilled && !((IProgramBank)bank).IsModeled)
                        .SelectMany(bank => bank.Patches)
                        .Where(program => program.IsLoaded))
            {
                ((IProgram)program).ReplaceWaveSequence(changes);
            }
        }


        // ISetNavigation
        /// <summary>
        /// CountPatches filled banks (except GM banks).
        /// </summary>
        public override int CountFilledBanks
        {
            get { return BankCollection.Count(bank => bank.IsFilled && (bank.Type != BankType.EType.Gm)); }
        }


        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string Name => "n.a.";


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
        /// 
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        protected virtual void CreateVirtualBanks()
        {
            // Default do not create virtual banks.
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new IBank this[int index] => base[index];
    }
}
