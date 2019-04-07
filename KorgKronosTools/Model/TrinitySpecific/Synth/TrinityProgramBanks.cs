// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Linq;
using PcgTools.Model.Common.Synth;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.TrinitySpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityProgramBanks : ProgramBanks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TrinityProgramBanks(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void CreateBanks()
        {
            Add(new TrinityProgramBank(this, BankType.EType.Int, "A", 0, ProgramBank.SynthesisType.Access, "A")); //  0

            Add(new TrinityProgramBank(this, BankType.EType.Int, "B", 1, ProgramBank.SynthesisType.Access, "B")); //  1

            Add(new TrinityProgramBank(this, BankType.EType.Int, "C", 2, ProgramBank.SynthesisType.Access, "C")); //  2

            Add(new TrinityProgramBank(this, BankType.EType.Int, "D", 3, ProgramBank.SynthesisType.Access, "D")); //  3

            Add(new TrinityProgramBank(
                this, BankType.EType.Int, "S", 4, ProgramBank.SynthesisType.MossZ1, "S")); //  4 ; Prophecy board

            Add(new TrinityProgramBank(this, BankType.EType.Int, "M", 5, ProgramBank.SynthesisType.MossZ1, "M")); //  5 ; 
        }

            // Index:              0       1       2       3       4        4
            // Name:               A       B       C       D       S        M


        /// <summary>
        /// In the Trinity, when program S or M is referenced, use that bank.
        /// </summary>
        /// <param name="pcgId"></param>
        /// <returns></returns>
        public override IBank GetBankWithPcgId(int pcgId)
        {
            IBank bank = null;

            if ((pcgId == 4) || (pcgId == 5))
            {
                var programBank = BankCollection[4];
                if (programBank.IsLoaded)
                {
                    // S bank exists; return this bank;
                    bank = programBank;
                }

                programBank = BankCollection[5];
                if (programBank.IsLoaded)
                {
                    // S bank exists; return this bank;
                    bank = programBank;
                }
            }

            if (bank == null)
            {
                foreach (var currentBank in BankCollection.Where(bankLambda => bankLambda.PcgId == pcgId))
                {
                    bank = currentBank;
                    break;
                }
            }

            if (bank == null)
            {
                throw new NotSupportedException("No GM Bank present in Trinity");    
            }
            
            return bank as IProgramBank;
        }
    }
}
