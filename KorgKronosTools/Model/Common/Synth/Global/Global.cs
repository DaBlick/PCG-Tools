// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Common.Synth.Global
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Global : IGlobal
    {
        /// <summary>
        /// 
        /// </summary>
        protected IPcgMemory PcgMemory { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public enum ECategoryType
        {
            Program,
            // ReSharper disable once UnusedMember.Global
            Combi
        };


        /// <summary>
        /// 
        /// </summary>
        public int ByteOffset { get; set; }


        /// <summary>
        /// Only used when supporting copying globals.
        /// </summary>
        public int ByteLength
        {
            get { throw new NotImplementedException();  }
            set { throw new NotImplementedException();  }
        }

        /// <summary>
        /// Category and sub category length.
        /// </summary>
        protected abstract int CategoryNameLength { get; }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int PcgOffsetCategories { get; }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int NrOfCategories { get; }


        /// <summary>
        /// 
        /// </summary>
        protected abstract int NrOfSubCategories { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        protected Global(IPcgMemory pcgMemory)
        {
            PcgMemory = pcgMemory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public virtual string GetCategoryName(IPatch patch)
        {
            var type = patch is IProgram ? ECategoryType.Program : ECategoryType.Combi;
            var category = patch is IProgram 
                ? ((IProgram) patch).GetParam(ParameterNames.ProgramParameterName.Category).Value 
                : ((ICombi) patch).GetParam(ParameterNames.CombiParameterName.Category ).Value;

            return Util.GetChars(PcgMemory.Content, CalcCategoryNameOffset(type, category), CategoryNameLength);
        }


// ReSharper disable once UnusedMember.Global
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        public virtual void SetCategoryName(ECategoryType type, int index, string name)
        {
            Util.SetChars(PcgMemory, PcgMemory.Content, CalcCategoryNameOffset(type, index), CategoryNameLength, name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public virtual string GetSubCategoryName(IPatch patch)
        {
            if (!PcgMemory.HasSubCategories)
            {
                throw new ApplicationException("ModelAndVersionAsString has no sub categories.");
            }

            ECategoryType type;
            int category;
            int subCategory;

            if (patch is IProgram)
            {
                type = ECategoryType.Program;
                category = ((IProgram) patch).GetParam(ParameterNames.ProgramParameterName.Category).Value;
                subCategory = ((IProgram)patch).GetParam(ParameterNames.ProgramParameterName.SubCategory).Value;
            }
            else
            {
                type = ECategoryType.Combi;
                category = ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.Category).Value;
                subCategory = ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.SubCategory).Value;
            }
            return Util.GetChars(
                PcgMemory.Content, CalcSubCategoryNameOffset(type, category, subCategory), CategoryNameLength);
        }


// ReSharper once UnusedMember.Global
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="subIndex"></param>
        public virtual void SetSubCategoryName(ECategoryType type, int index, string name, int subIndex)
        {
            Util.SetChars(
                PcgMemory, PcgMemory.Content, CalcSubCategoryNameOffset(type, index, subIndex), CategoryNameLength, name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual List<string> GetCategoryNames(ECategoryType type)
        {
            var categories = new List<string>();

            for (var category = 0; category < NrOfCategories; category++)
            {
                categories.Add(
                    Util.GetChars(PcgMemory.Content, CalcCategoryNameOffset(type, category), CategoryNameLength));
            }

            return categories;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public LinkedList<string> GetSubCategoryNames(ECategoryType type, int category)
        {
            var categories = new LinkedList<string>();
            for (var subCategory = 0; subCategory < NrOfSubCategories; subCategory++)
            {
                categories.AddLast(
                    Util.GetChars(PcgMemory.Content, CalcSubCategoryNameOffset(type, category, subCategory), CategoryNameLength));
            }

            return categories;
        }


        
        /// <summary>
        /// Returns offset from global.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual int CalcCategoryNameOffset(ECategoryType type, int index)
        {
            var offset = ByteOffset + PcgOffsetCategories;
            
            offset += (type == ECategoryType.Program) ? 0 : SizeOfProgramsCategoriesAndSubCategories;
            offset += index * CategoryNameLength;
            return offset;
        }


        /// <summary>
        /// Returns offset from global.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        protected virtual int CalcSubCategoryNameOffset(ECategoryType type, int index, int subIndex)
        {
            var offset = ByteOffset + PcgOffsetCategories;
            var typeSize = NrOfCategories * (CategoryNameLength + SubCategoriesSize);

            offset += (type == ECategoryType.Program) ? 0 : typeSize;
            offset += NrOfCategories * CategoryNameLength; // Categories size
            offset += index * SubCategoriesSize;
            offset += subIndex * CategoryNameLength;
            return offset;
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual int SizeOfProgramsCategoriesAndSubCategories => NrOfCategories * (CategoryNameLength + SubCategoriesSize);


        /// <summary>
        /// 
        /// </summary>
        protected int SubCategoriesSize => PcgMemory.HasSubCategories ? NrOfSubCategories * CategoryNameLength : 0;
    }
}
