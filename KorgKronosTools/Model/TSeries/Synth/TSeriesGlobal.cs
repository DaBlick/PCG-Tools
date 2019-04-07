// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.MntxSeriesSpecific.Synth;

namespace PcgTools.Model.TSeries.Synth
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TSeriesGlobal : MntxGlobal
    {
        /// <summary>
        /// Category names are not in PCG.
        /// </summary>
        protected override int CategoryNameLength
        {
            get { throw new NotSupportedException(); }
        }

        
        /// <summary>
        /// Hardcoded.
        /// </summary>
        protected override int PcgOffsetCategories
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        /// Categories are taken from Mode.
        /// </summary>
        protected override int NrOfCategories
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfSubCategories
        {
            get { throw new NotSupportedException(); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public TSeriesGlobal(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public override string GetCategoryName(IPatch patch)
        {
            throw new NotSupportedException();
        }
    }
}
