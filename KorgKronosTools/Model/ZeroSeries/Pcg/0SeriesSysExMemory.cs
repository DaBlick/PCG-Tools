// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MntxSeriesSpecific.Pcg;
using PcgTools.Model.ZeroSeries.Synth;

namespace PcgTools.Model.ZeroSeries.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class ZeroSeriesSysExMemory : MntxSysExMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        /// <param name="patchNamesCopyNeeded"></param>
        public ZeroSeriesSysExMemory(string fileName,
            ContentType contentType, int sysExStartOffset, int sysExEndOffset, bool patchNamesCopyNeeded)
            : base(fileName, Models.EModelType.ZeroSeries, contentType, sysExStartOffset, sysExEndOffset, patchNamesCopyNeeded)
        {
            CombiBanks = new ZeroSeriesCombiBanks(this);
            ProgramBanks = new ZeroSeriesProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = null;
            DrumPatternBanks = null;
            Global = new ZeroSeriesGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionZeroSeries);
        }
        

        /// <summary>
        /// 
        /// </summary>
        public override bool HasSubCategories => false;


        /// <summary>
        /// Hardcoded (taken from Mode parameter).
        /// </summary>
        public override int NumberOfCategories => 4;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfSubCategories
        {
            get
            {
                throw new NotSupportedException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool AreCategoriesEditable => false;


        /// <summary>
        /// Duplicates the patch names of the (fixed length) RAW file.
        /// 
        /// 
        /// Disk image  Start   Combi       Program Combi Dup   Program Dup 
        ///                     Start       Start   Start       Start       
        ///     0       0x04800 0x05284     0x08484 0x2800      0x3000      
        ///     1       0x0d800 0x0e284     0x11484 0x2be8      0x33e8      
        ///     2       0x38000?0x38e84     0x3c084 0x3800      0x4000      
        ///     3       0x41400 0x41e84     0x45084 0x3be8      0x43e8      
        /// 
        /// Order of summary: DI0/C, DI1/C, 96 bytes, DI0/P, DI1/P, 96 bytes, DI2/C, DI3/C, 96 bytes, DI2/P, DI3/P
        /// where DI = disk image, C = combis, P = programs, 96 bytes = 96 unused bytes.
        /// </summary>
        protected override void CopyPatchNames()
        {
            // Copy disk image 0, combis and programs.
            DuplicatePatchNames(0x5284, 0x2800, false);
            DuplicatePatchNames(0x8484, 0x3000, true);

            // Copy disk image 1, combis and programs.
            DuplicatePatchNames(0xe284, 0x2be8, false);
            DuplicatePatchNames(0x11484, 0x33e8, true);

            // Copy disk image 2, combis and programs.
            DuplicatePatchNames(0x38e84, 0x3800, false);
            DuplicatePatchNames(0x3c084, 0x4000, true);

            // Copy disk image 3, combis and programs.
            DuplicatePatchNames(0x41e84, 0x3be8, false);
            DuplicatePatchNames(0x45084, 0x43e8, true);
        }


        /// <summary>
        /// Duplicate names in the beginning of the file from the original programs/combis.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="copyPrograms"></param>
        private void DuplicatePatchNames(int source, int destination, bool copyPrograms)
        {
            // 100 = patches in a bank
            for (var patchIndex = 0; patchIndex < 100; patchIndex++)
            {
                // 172 and 128 are program/combi patch size.
                Util.CopyBytes(this, source + patchIndex*(copyPrograms ? 172 : 128), destination + patchIndex*10, 10);
            }
        }
    }
}

