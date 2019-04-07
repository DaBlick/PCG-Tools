// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MntxSeriesSpecific.Pcg;
using PcgTools.Model.TSeries.Synth;

namespace PcgTools.Model.TSeries.Pcg
{
    /// <summary>
    /// 
    /// </summary>
// ReSharper disable once InconsistentNaming
    public class TSeriesSysExMemory : MntxSysExMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public TSeriesSysExMemory(string fileName,
            ContentType contentType, int sysExStartOffset, int sysExEndOffset)
            : base(fileName, Models.EModelType.TSeries, contentType, sysExStartOffset, sysExEndOffset, false)
        {
            CombiBanks = new TSeriesCombiBanks(this);
            ProgramBanks = new TSeriesProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = null;
            Global = new TSeriesGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionTSeries);
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
    }
}
