// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.MntxSeriesSpecific.Pcg;
using PcgTools.Model.XSeries.Synth;

namespace PcgTools.Model.XSeries.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public class XSeriesSysExMemory : MntxSysExMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="sysExStartOffset"></param>
        /// <param name="sysExEndOffset"></param>
        public XSeriesSysExMemory(string fileName,
            ContentType contentType, int sysExStartOffset, int sysExEndOffset)
            : base(fileName, Models.EModelType.XSeries, contentType, sysExStartOffset, sysExEndOffset, false)
        {
            CombiBanks = new XSeriesCombiBanks(this);
            ProgramBanks = new XSeriesProgramBanks(this);
            SetLists = null;
            WaveSequenceBanks = null;
            DrumKitBanks = null;
            DrumPatternBanks = null;
            Global = new XSeriesGlobal(this);
            Model = Models.Find(Models.EOsVersion.EOsVersionXSeries);
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
