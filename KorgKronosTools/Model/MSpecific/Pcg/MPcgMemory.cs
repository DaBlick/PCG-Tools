// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved


using PcgTools.Model.Common.Synth.MemoryAndFactory;

namespace PcgTools.Model.MSpecific.Pcg
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MPcgMemory : PcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        protected MPcgMemory(string fileName, Models.EModelType modelType)
            : base(fileName, modelType)
        {
            SetLists = null;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool HasSubCategories => true;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfCategories => 18;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfSubCategories => 8;
    }
}
