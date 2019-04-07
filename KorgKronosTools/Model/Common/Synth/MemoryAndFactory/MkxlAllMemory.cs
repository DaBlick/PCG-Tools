// (c) Copyright 2011-2017 MiKeSoft, Michel Keijzers, All rights reserved

using PcgTools.PcgToolsResources;

namespace PcgTools.Model.Common.Synth.MemoryAndFactory
{
    /// <summary>
    /// 
    /// </summary>
    public class MkxlAllMemory : PcgMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="modelType"></param>
        protected MkxlAllMemory(string fileName, Models.EModelType modelType)
            : base(fileName, modelType)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool HasSubCategories => true;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfCategories => 8;


        /// <summary>
        /// 
        /// </summary>
        public override int NumberOfSubCategories => 8;


        /// <summary>
        /// On the MicroKorg XL, programs are divided into genres, then into categories.
        /// </summary>
        public override string CategoryName => Strings.Genre;


        /// <summary>
        /// On the MicroKorg XL, programs are divided into genres, then into categories.
        /// </summary>
        public override string SubCategoryName => Strings.Category;


        /// <summary>
        /// MicroKorg XL uses genres and categories instead of categories and sub categories.
        /// </summary>
        public override bool UsesCategoriesAndSubCategories => false;


        /// <summary>
        /// On the MicroKorg XL, categories (actually genres and categories) are not editable.
        /// </summary>
        public override bool AreCategoriesEditable => false;
    }
}
