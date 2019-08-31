// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;
using PcgTools.Model.Common;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.TrinitySpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class TrinityPcgFileReader : PcgFileReader
    {
        /// <summary>
        /// 
        /// </summary>
        private const int SampledProgramSize = 433;


        /// <summary>
        /// 
        /// </summary>
        private const int ModeledProgramSize = 521;


        /// <summary>
        /// 
        /// </summary>
        private const int CombiSize = 388;


        /// <summary>
        /// 
        /// </summary>
        private const int DrumKitSize = 1426;


        /// <summary>
        /// 
        /// </summary>
        private const int GlobalSize = 1175; // 0x499 - 2


        /// <summary>
        /// 
        /// </summary>
        private int _index;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPcgMemory"></param>
        /// <param name="content"></param>
        public TrinityPcgFileReader(IPcgMemory currentPcgMemory, byte[] content)
            : base(currentPcgMemory, content)
        {
            // PcgFileHeader PcgFileHeader;
            // Pcg1Chunk Pcg1Chunk;
            // Div1Chunk Div1Chunk;
            // Ini2Chunk Ini2Chunk;
            // Ini1Chunk Ini1Chunk;
            // Prg1Chunk Prg1Chunk;
            // Cmb1Chunk Cmb1Chunk;
            // Dkt1Chunk Dkt1Chunk;
            // Arp1Chunk Arp1Chunk;
            // Glb1Chunk Glb1Chunk;
        }


        /// <summary>
        /// Trinity does not use chunks.
        /// </summary>
        public override void ReadContent(Memory.FileType filetype, Models.EModelType modelType)
        {
            _index = 0x20; // Start of PCG usage.

            int programBanks;
            int programsInProgramBank;
            ReadBanksUsage(out programBanks, out programsInProgramBank, SampledProgramSize);

            int sProgramBanks;
            int sProgramsInProgramBank;
            ReadBanksUsage(out sProgramBanks, out sProgramsInProgramBank, ModeledProgramSize);

            int combiBanks;
            int combisInCombiBank;
            ReadBanksUsage(out combiBanks, out combisInCombiBank, CombiSize);
            Debug.Assert(combisInCombiBank % 128 == 0);

            int drumKits;
            int drumKitsInDrumKitBank;
            ReadBanksUsage(out drumKits, out drumKitsInDrumKitBank, DrumKitSize, false);
            
            int global;
            int globalBanks;
            ReadBanksUsage(out global, out globalBanks, GlobalSize, false);

            int mProgramBanks;
            int mProgramsInProgramBank;
            ReadBanksUsage(out mProgramBanks, out mProgramsInProgramBank, ModeledProgramSize);

            if (mProgramBanks > 0)
            {
                // Only V3 have M (Moss) bank(s).
                CurrentPcgMemory.Model = Models.Find(Models.EOsVersion.EOsVersionTrinityV3); 
            }
            
            ReadProgramBanks(programBanks, programsInProgramBank);
            ReadSProgramBanks(sProgramBanks, sProgramsInProgramBank);
            ReadCombiBanks(combiBanks);
            ReadDrumKitBanks(drumKits);
            ReadGlobal(globalBanks == 1);
            ReadMProgramBanks(mProgramBanks, mProgramsInProgramBank);

            SetNotifications();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="patchesPerBank"></param>
        /// <param name="patchSize"></param>
        /// <param name="check"></param>
        private void ReadBanksUsage(out int banks, out int patchesPerBank, int patchSize, 
// ReSharper disable once UnusedParameter.Local; check is used but only for a debug check.
             bool check = true)
        {
            Debug.Assert(patchSize > 0);

            _index += 2;
            banks = Util.GetInt(CurrentPcgMemory.Content, _index, 2);
            _index += 2;

            // When the Trinity has double memory, twice the number of unknownExtraBytes should be added.
            var size = Util.GetInt(CurrentPcgMemory.Content, _index, 4);
            if (banks == 0)
            {
                patchesPerBank = 0;
            }
            else
            {
                // Division might be a float just above the integer value but patchesPerBank is an integer
                // so (automatic) casting makes it correct.
                // THis is a very dirty way since sometimes I am a few bytes off what I would expect.
                var patchesPerBankFloat = banks == 0 ? 0.0 : (double) size / patchSize / banks;
                Debug.Assert(patchesPerBankFloat  % 1.0 <= 0.01) ;
                patchesPerBank = banks == 0 ? 0 : size / patchSize / banks;
            }
           
            if (check)
            {
                Debug.Assert(patchesPerBank % 64 == 0);
            }
            _index += 4;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="patches"></param>
        private void ReadProgramBanks(int banks, int patches)
        {
            for (var index = 0; index < banks; index++)
            {
                var bankIndex = Util.GetInt(CurrentPcgMemory.Content, _index, 2);
                _index += 2;

                var bank = (IProgramBank)CurrentPcgMemory.ProgramBanks[bankIndex];
                //programBank.PcgId = bankId;
                bank.ByteOffset = _index;
                bank.ByteLength = SampledProgramSize;
                bank.BankSynthesisType = bank.DefaultSampledSynthesisType;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                // Read patches.
                for (var patchIndex = 0; patchIndex < patches; patchIndex++)
                {
                    // Place in PcgMemory.
                    var program = (IProgram)bank[patchIndex];
                    program.ByteOffset = _index;
                    program.ByteLength = bank.ByteLength;
                    program.IsLoaded = true;

                    _index += bank.ByteLength;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="patches"></param>
        private void ReadSProgramBanks(int banks, int patches)
        {
            Debug.Assert(banks <= 1);
            for (var bankIndex = 0; bankIndex < banks; bankIndex++)
            {
                Debug.Assert(Util.GetInt(CurrentPcgMemory.Content, _index, 2) == 4);
                _index += 2;

                var bank = (IProgramBank) CurrentPcgMemory.ProgramBanks[4]; // Fixed
                //bank.PcgId = bankId;
                bank.ByteOffset = _index;
                bank.ByteLength = ModeledProgramSize;
                bank.BankSynthesisType = ProgramBank.SynthesisType.MossZ1; // From Prophecy;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                // Read patches.
                for (var index = 0; index < patches; index++)
                {
                    // Place in PcgMemory.
                    var program = (Program) bank[index];
                    program.ByteOffset = _index;
                    program.ByteLength = bank.ByteLength;
                    program.IsLoaded = true;

                    _index += bank.ByteLength;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        private void ReadCombiBanks(int banks)
        {
            for (var index = 0; index < banks; index++)
            {
                // Normally the bank index should be 2 bytes, however, PRELOAD1.PCG's second bank at 0x2F3B4 has
                // value 0x0500.
                var unusedMsbBankIndex = Util.GetInt(CurrentPcgMemory.Content, _index, 1);
                Debug.Assert((unusedMsbBankIndex == 0x00) || (unusedMsbBankIndex == 0x05));
                _index += 1;

                var bankIndex = Util.GetInt(CurrentPcgMemory.Content, _index, 1);
                _index += 1;

                var bank = (CombiBank)CurrentPcgMemory.CombiBanks[bankIndex];
                // combiBank.PcgId = 
                bank.ByteOffset = _index;
                bank.PatchSize = CombiSize;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                for (var patchIndex = 0; patchIndex < 128; patchIndex++)
                {
                    // Place in PcgMemory.
                    var combi = (Combi)bank[patchIndex];
                    combi.ByteOffset = _index;
                    combi.ByteLength = bank.PatchSize;
                    combi.IsLoaded = true;

                    _index += bank.PatchSize;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="drumkits"></param>
        private void ReadDrumKitBanks(int drumkits)
        {
            //IMPR  Debug.Assert(drumkits <= TrinityDrumKitBank.DrumKitsPerBankIntConstant);

            if (drumkits > 0)
            {
                var startIndex = _index;
                _index += 2;

                var bank = (IDrumKitBank) CurrentPcgMemory.DrumKitBanks[0];
                bank.ByteOffset = startIndex;
                bank.ByteLength = DrumKitSize;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                for (var index = 0; index < drumkits; index++)
                {
                    // Place in PcgMemory.
                    var drumKit = (DrumKit) bank[index];
                    drumKit.ByteOffset = _index;
                    drumKit.ByteLength = bank.ByteLength;
                    drumKit.IsLoaded = true;

                    _index += bank.ByteLength;
                }
            }
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="present"></param>
        private void ReadGlobal(bool present)
        {
            if (present)
            {
                _index += 2;
                CurrentPcgMemory.Global.ByteOffset = _index;
                _index += GlobalSize;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="banks"></param>
        /// <param name="patches"></param>
        private void ReadMProgramBanks(int banks, int patches)
        {
            Debug.Assert(banks <= 1);
            for (var index = 0; index < banks; index++)
            {
                var bankIndex = Util.GetInt(CurrentPcgMemory.Content, _index, 2);
                Debug.Assert(bankIndex == 4);
                
                _index += 2;

                var bank = (IProgramBank)CurrentPcgMemory.ProgramBanks[5]; // Store in M bank.
                //programBank.PcgId = bankId;
                bank.ByteOffset = _index;
                bank.ByteLength = ModeledProgramSize;
                bank.BankSynthesisType = ProgramBank.SynthesisType.MossZ1;
                bank.IsWritable = true;
                bank.IsLoaded = true;

                // Read patches.
                for (var patchIndex = 0; patchIndex < patches; patchIndex++)
                {
                    // Place in PcgMemory.
                    var program = (Program)bank[index];
                    program.ByteOffset = _index;
                    program.ByteLength = bank.ByteLength;
                    program.IsLoaded = true;

                    _index += bank.ByteLength;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int Div1Offset
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int BetweenChunkGapSize
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int GapSizeAfterMbk1ChunkName
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int Pbk1NumberOfProgramsOffset
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int SizeBetweenCmb1AndCbk1
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int Cbk1NumberOfCombisOffset
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int Dbk1NumberOfDrumKitsOffset
        {
            get { throw new NotSupportedException("Trinity has no chunks"); }
        }

        protected override int Dpi1NumberOfDrumPatternsOffset
        {
            get { throw new NotImplementedException(); }
        }
    }
}
