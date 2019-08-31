// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Linq;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.TrinitySpecific.Synth;

namespace PcgTools.Model.TrinitySpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityPcgMemory : PcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        public TrinityPcgMemory(string fileName, Models.EModelType modelType)
            : base(fileName, modelType)
        {
            CombiBanks = new TrinityCombiBanks(this);
            ProgramBanks = new TrinityProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = new TrinityDrumKitBanks(this);
            DrumPatternBanks = null;
            Global = new TrinityGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTrinityV2); // Will be replaced later by V3 if a Moss bank is found.
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool HasSubCategories => false;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfCategories => 16;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfSubCategories => 0;


        /// <summary>
        /// Used for master file check; depending on installation of memory extension deciding if a modeled bank has 64 programs and 
        /// there are 4 instead of 2 program/combi banks.
        /// </summary>
        protected override bool AreAllNeededProgramBanksPresent
        {
            get
            {
                var numberOfSampledProgramBanks = 0;
                var numberOfModeledBanks = ProgramBanks.BankCollection.Aggregate(
                    0, (current, bank1) => IsNeededProgramBankPresent(bank1, current, ref numberOfSampledProgramBanks));

                // Either Prophecy or MOSS bank should be available. If the Prophecy/MOSS bank contains 64 programs,
                // no memory extension is installed and two banks should be present (A and B Access bank, otherwise 
                // there are 128 programs in the Propecy/MOSS bank and C and D Access banks should be present too.
                var programsInModeledBank = ProgramsInModeledProgramBank;

                return ((numberOfModeledBanks == 1) && 
                        (((programsInModeledBank == 64) && (numberOfSampledProgramBanks == 2)) ||
                         ((programsInModeledBank == 128) && (numberOfSampledProgramBanks == 4))));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bank1"></param>
        /// <param name="numberOfModeledBanks"></param>
        /// <param name="numberOfSampledProgramBanks"></param>
        /// <returns></returns>
        private static int IsNeededProgramBankPresent(IBank bank1, int numberOfModeledBanks, ref int numberOfSampledProgramBanks)
        {
            var bank = (IProgramBank) bank1;
            if (bank.IsModeled)
            {
                if (bank.ByteOffset != 0)
                {
                    numberOfModeledBanks++;
                }
            }
            else
            {
                // Check for IsWritable is not needed because a Trinity does not have GM banks.
                if (bank.ByteOffset != 0)
                {
                    numberOfSampledProgramBanks++;
                }
            }
            return numberOfModeledBanks;
        }


        /// <summary>
        /// Used for master file check; depending on installation of memory extension deciding if a modeled bank has 64 programs and 
        /// there are 4 instead of 2 program/combi banks.
        /// </summary>
        protected override bool AreAllNeededCombiBanksPresent
        {
            get
            {
                var numberOfCombiBanks = 0;

                foreach (var bank in CombiBanks.BankCollection)
                {
                    {
                        if (bank.ByteOffset != 0)
                        {
                            numberOfCombiBanks++;
                        }
                    }
                }

                // If the Prophecy/MOSS bank contains 64 programs, no memory extension is installed and two banks should be present 
                // (A and B Combi bank, otherwise there are 128 programs in the Propecy/MOSS bank and C and D Combi banks should be 
                // present too.
                var programsInModeledBanks = ProgramsInModeledProgramBank;

                return  (((programsInModeledBanks == 64) && (numberOfCombiBanks == 2)) ||
                         ((programsInModeledBanks == 128) && (numberOfCombiBanks == 4)));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private int ProgramsInModeledProgramBank
        {
            get
            {
                return ProgramBanks.BankCollection.Where(
                    bank => ((IProgramBank) bank).IsModeled).Where(
                    bank => bank.ByteOffset != 0).Sum(bank => bank.Patches.Count(program => program.ByteOffset != 0));
            }
        }
    }
}
