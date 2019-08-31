// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.MSpecific.Synth;

namespace PcgTools.Model.KrossSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class KrossGlobal : MGlobal
    {
        /// <summary>
        /// 
        /// </summary>
        protected override int PcgOffsetCategories => 334;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public KrossGlobal(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected override int NrOfCategories => 12;


        /// <summary>
        /// The category names are hardcoded in Kross. 
        /// </summary>
        /// <returns></returns>
        public override string GetCategoryName(IPatch patch)
        {
            var category = -1;
            if (patch is IProgram)
            {
                category = ((IProgram) patch).GetParam(ParameterNames.ProgramParameterName.Category).Value;
            } else if (patch is ICombi)
            {
                category = ((ICombi) patch).GetParam(ParameterNames.CombiParameterName.Category).Value;
            }

            var categories = GetCategoryNames(ECategoryType.Program); // Ignored
            return categories[category];
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override List<string> GetCategoryNames(ECategoryType type)
        {
            var categories = new List<string>
            {
                "PIANO",
                "E.PIANO",
                "ORGAN",
                "BELL",
                "STRINGS",
                "BRASS",
                "SYNTH",
                "LEAD",
                "SYNTH PAD",
                "GUITAR",
                "BASS",
                "DRUM/SFX",
                "USER"
            };

            return categories;
        }


        /// <summary>
        /// Returns offset from global.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        protected override int CalcSubCategoryNameOffset(ECategoryType type, int index, int subIndex)
        {
            var offset = ByteOffset + PcgOffsetCategories;

            // Skip categories; the Kross has no category names in its GLOBAL.
           
            offset += index * SubCategoriesSize;
            offset += subIndex * CategoryNameLength;
            return offset;
        }
    }
}
