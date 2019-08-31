// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using PcgTools.Model.Common.Synth.Global;
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PcgTools.Model.Z1Specific.Synth
{
    /// <summary>
    /// Note/Improvement: User groups are not taken into account.
    /// There are 16 user groups, independent of categories.
    /// </summary>
    public class Z1Global : Global
    {
        /// <summary>
        /// Category names are not in PCG.
        /// </summary>
        protected override int CategoryNameLength => 14;


        /// <summary>
        /// Hardcoded.
        /// </summary>
        protected override int PcgOffsetCategories => -1;


        /// <summary>
        /// Categories are fixed.
        /// </summary>
        protected override int NrOfCategories => 18;


        /// <summary>
        /// </summary>
        protected override int NrOfSubCategories
        {
            get { throw new NotSupportedException("No sub categories available"); }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgMemory"></param>
        public Z1Global(IPcgMemory pcgMemory)
            : base(pcgMemory)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override List<string> GetCategoryNames(ECategoryType type)
        {
            List<string> names = null;

            if (type == ECategoryType.Program)
            {
                names = new List<string>
                {
                    "Synth-Hard",
                    "Synth-Soft",
                    "Synth-Lead",
                    "Synth-Motion",
                    "Synth-Bass",
                    "E.Piano",
                    "Organ",
                    "Keyboard",
                    "Bell",
                    "Strings",
                    "Bad/Choir",
                    "Brass",
                    "Reed/Wind",
                    "Guitar/Plucked",
                    "Bass",
                    "Percussive",
                    "Arpeggio",
                    "SFX/Other"
                };
            }
            // Else keep empty
            
            return names;
        }


        /// <summary>
        /// Only programs have categories, not multis.
        /// IMPR: Check Z1Programm for duplicate code.
        /// </summary>
        /// <param name="patch"></param>
        /// <returns></returns>
        public override string GetCategoryName(IPatch patch)
        {
            var categoryName = string.Empty;

            if (patch is IProgram)
            {
                var names = new[]
                {
                    "Synth-Hard", "Synth-Soft", "Synth-Lead", "Synth-Motion", "Synth-Bass", "E.Piano", "Organ",
                    "Keyboard",
                    "Bell", "Strings", "Bad/Choir", "Brass", "Reed/Wind", "Guitar/Plucked", "Bass", "Percussive",
                    "Arpeggio", "SFX/Other"
                };

                categoryName = names[((IProgram) patch).GetParam(ParameterNames.ProgramParameterName.Category).Value];
            }
            //else
            //{
            //    throw new NotSupported("Only programs have category names");
            //}

            return categoryName;
        }
    }
}
