// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.MicroKorgXlSpecific.Synth
{
    /// <summary>
    /// 
    /// </summary>
    public class MicroKorgXlGlobal : Global
    {
        /// <summary>
        /// 
        /// </summary>
        protected override int CategoryNameLength => 16;


        /// <summary>
        /// Hardcoded.
        /// </summary>
        protected override int PcgOffsetCategories => -1;


        /// <summary>
        /// Called genres.
        /// </summary>
        protected override int NrOfCategories => 8;


        /// <summary>
        /// Called Categories.
        /// </summary>
        protected override int NrOfSubCategories => 8;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public MicroKorgXlGlobal(IPcgMemory pcgMemory)
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
            var category = -1;
            if (patch is IProgram)
            {
                category = ((IProgram)patch).GetParam(ParameterNames.ProgramParameterName.Category).Value;
            }
            else if (patch is ICombi)
            {
                category = ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.Category).Value;
            }

            var genres = new List<string>
            {
                "Vintage",
                "Rock/Pop",
                "R&B/Hip Hop",
                "Jazz/Fusion",
                "Techno/Trance",
                "House/Disco",
                "D'N'B/Break",
                "Favorite"
            };
            var name = genres[category];

            return name;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public override string GetSubCategoryName(IPatch patch)
        {
            var subCategory = -1;
            if (patch is IProgram)
            {
                subCategory = ((IProgram)patch).GetParam(ParameterNames.ProgramParameterName.Category).Value;
            }
            else if (patch is ICombi)
            {
                subCategory = ((ICombi)patch).GetParam(ParameterNames.CombiParameterName.Category).Value;
            }

            var categories = new List<string>
            {
                "Poly Synth",
                "Bass",
                "Lead",
                "Arp/Motion",
                "Pad/Strings",
                "Keyboard/Bell",
                "S.E./Hit",
                "Vocoder"
            };
            var name = categories[subCategory];
            return name;
        }

    }
}
