// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System.Collections.Generic;
using System.Diagnostics;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.OldParameters;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchDrumKits;
using PcgTools.Model.Common.Synth.PatchDrumPatterns;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;
using PcgTools.Model.Common.Synth.PatchWaveSequences;

namespace PcgTools.Model.Common.Synth.PatchSorting
{
    /// <summary>
    /// Set list slots are sorted by ListSubType/bank/index.
    ///  but have no (Sub)Category) affects this CategoricalComparer class. Add a guard to return 
    /// immediately in those cases?
    /// Drum kits and wave sequences do not have categories, and are sorted by name.
    /// </summary>
    internal sealed class CategoricalComparer : Comparer<IPatch>
    {
        /// <summary>
        /// 
        /// </summary>
        private static CategoricalComparer _instance = new CategoricalComparer();


        /// <summary>
        /// 
        /// </summary>
        public static CategoricalComparer Instance => _instance;


        /// <summary>
        /// 
        /// </summary>
        private static int _p1Category;


        /// <summary>
        /// 
        /// </summary>
        private static int _p2Category;
        

        /// <summary>
        /// 
        /// </summary>
        private static string _p1CategoryName;


        /// <summary>
        /// 
        /// </summary>
        private static string _p2CategoryName;


        /// <summary>
        /// 
        /// </summary>
        private static int _p1SubCategory;


        /// <summary>
        /// 
        /// </summary>
        private static int _p2SubCategory;


        /// <summary>
        /// 
        /// </summary>
        private static string _p1SubCategoryName;


        /// <summary>
        /// 
        /// </summary>
        private static string _p2SubCategoryName;


        /// <summary>
        /// 
        /// </summary>
        private static bool _hasSubCategories;


        /// <summary>
        /// 
        /// </summary>
        private static bool _hasCategoryNames;


        /// <summary>
        /// 
        /// </summary>
        private CategoricalComparer()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        private static IPatch _p1;


        /// <summary>
        /// 
        /// </summary>
        private static IPatch _p2;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public override int Compare(IPatch p1, IPatch p2)
        {
            SetPatches(p1, p2);

            var compare = HandlePatchesWithoutCategories();

            if (compare == 0)
            {
                compare = HandleGmPrograms();
            }

            if (compare == 0)
            {
                compare = HandleOnlyOneHasCategory();
            }

            if (compare == 0)
            {
                compare = HandleBothNoSetListSlot();
            }

            if (compare == 0)
            {
                compare = HandleNoCategoryNoSubCategoryName();
            }

            if (compare == 0)
            {
                compare = HandleCategoriesEqualSubCategories();
            }

            return compare;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private static void SetPatches(IPatch p1, IPatch p2)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);

            _p1 = p1;
            _p2 = p2;

            _hasSubCategories = p1.PcgRoot.HasSubCategories;
            _hasCategoryNames = (p1.PcgRoot.Global != null);
        }


        /// <summary>
        /// Categories are equal, check sub categories if present.
        /// </summary>
        /// <returns></returns>
        private static int HandleCategoriesEqualSubCategories()
        {
            if (!_hasSubCategories)
            {
                return 0; // Patches are equal (category-wise).
            }

            if (_p1SubCategory < _p2SubCategory)
            {
                return -1;
            }

            return _p1SubCategory > _p2SubCategory ? 1 : 0;
        }


        /// <summary>
        /// No category (nor sub category) names.
        /// </summary>
        /// <returns></returns>
        private static int HandleNoCategoryNoSubCategoryName()
        {
            if (_p1Category != _p2Category)
            {
                if (_p1Category < _p2Category)
                {
                    {
                        return -1;
                    }
                }
                if (_p1Category > _p2Category)
                {
                    {
                        return 1;
                    }
                }
            }

           return 0;
        }


        /// <summary>
        /// Both p1 and p2 are no set list slots.
        /// SortMethod on category name if available.
        /// </summary>
        /// <returns></returns>
        private static int HandleBothNoSetListSlot()
        {
            if (_hasCategoryNames)
            {
                if (_p1CategoryName != _p2CategoryName)
                {
                    {
                        return string.CompareOrdinal(_p1CategoryName, _p2CategoryName);
                    }
                }

                if (!_hasSubCategories)
                {
                    {
                        return _p1.Name == _p2.Name ? string.CompareOrdinal(_p1.Id, _p2.Id) : _p1.CompareTo(_p2);
                    }
                }

                if (_p1SubCategoryName != _p2SubCategoryName)
                {
                    {
                        return string.CompareOrdinal(_p1SubCategoryName, _p2SubCategoryName);
                    }
                }

                {
                    return _p1.Name == _p2.Name ? string.CompareOrdinal(_p1.Id, _p2.Id) : _p1.CompareTo(_p2);
                }
            }

            return 0;
        }


        /// <summary>
        /// If one of the patches does not have a category it is 'last'. 
        /// </summary>
        /// <returns></returns>
        private static int HandleOnlyOneHasCategory()        
        {

            var p1HasCategory = PatchHasCategory(_p1);
            var p2HasCategory = PatchHasCategory(_p2);

            if (p1HasCategory && !p2HasCategory)
            {
                return -1;
            }

            if (!p1HasCategory && p2HasCategory)
            {
                return 1;
            }

            if (!p1HasCategory)
            {
                return 0;
            }

            ParsePatch1(_p1);
            ParsePatch2(_p2);
            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool PatchHasCategory(IPatch patch)
        {
            return ((patch is Program) && (patch.PcgRoot.HasProgramCategories)) ||
                   ((patch is Combi) && (patch.PcgRoot.HasCombiCategories));
        }


        /// <summary>
        /// Check for GM programs (always come last but before set list slots).
        /// </summary>
        /// <returns></returns>
        private static int HandleGmPrograms()
        {
            var p1IsGm = (_p1 is Program) && (((IBank) (_p1).Parent).Type == BankType.EType.Gm);
            var p2IsGm = (_p2 is Program) && (((IBank) (_p2).Parent).Type == BankType.EType.Gm);

            if (p1IsGm)
            {
                return p2IsGm ? string.CompareOrdinal(_p1.Id, _p2.Id) : 1;
            }

            if (p2IsGm)
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// Handle drum kits, wave sequences and set list slot (do not have categories).
        /// </summary>
        /// <returns></returns>
        private static int HandlePatchesWithoutCategories()
        {
            if ((_p1 is DrumKit) || (_p1 is DrumPattern) || (_p1 is WaveSequence) || (_p1 is SetListSlot))
            {
                if ((_p2 is DrumKit) || (_p2 is DrumPattern) || (_p2 is WaveSequence) || (_p2 is SetListSlot))
                {
                    return 0;
                }

                return 1;
            }

            if ((_p2 is DrumKit) || (_p2 is DrumPattern) || (_p2 is WaveSequence) || (_p2 is SetListSlot))
            {
                return -1;
            }

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        private static void ParsePatch1(IPatch p1)
        {
            if (p1 is IProgram)
            {
                _p1Category = ((IProgram) p1).GetParam(ParameterNames.ProgramParameterName.Category).Value;
            } else if (p1 is ICombi)
            {
                _p1Category = ((ICombi)p1).GetParam(ParameterNames.CombiParameterName.Category).Value;
            }
            
            if (_hasCategoryNames)
            {
                var program = p1 as Program;
                _p1CategoryName = program != null ? program.CategoryAsName : ((Combi) p1).CategoryAsName;
            }

            if (_hasSubCategories)
            {
                if (p1 is IProgram)
                {
                    _p1SubCategory = ((IProgram)p1).GetParam(ParameterNames.ProgramParameterName.SubCategory).Value;
                }
                else if (p1 is ICombi)
                {
                    _p1SubCategory = ((ICombi)p1).GetParam(ParameterNames.CombiParameterName.SubCategory).Value;
                }

                if (_hasCategoryNames)
                {
                    var program = p1 as Program;
                    _p1SubCategoryName = program != null ? program.SubCategoryAsName : ((Combi) p1).SubCategoryAsName;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static void ParsePatch2(IPatch p2)
        {
            if (p2 is IProgram)
            {
                _p2Category = ((IProgram)p2).GetParam(ParameterNames.ProgramParameterName.Category).Value;
            }
            else if (p2 is ICombi)
            {
                _p2Category = ((ICombi)p2).GetParam(ParameterNames.CombiParameterName.Category).Value;
            }

            if (_hasCategoryNames)
            {
                var program = p2 as Program;
                _p2CategoryName = program != null ? program.CategoryAsName : ((Combi) p2).CategoryAsName;
            }

            if (_hasSubCategories)
            {
                if (p2 is IProgram)
                {
                    _p2SubCategory = ((IProgram)p2).GetParam(ParameterNames.ProgramParameterName.SubCategory).Value;
                }
                else if (p2 is ICombi)
                {
                    _p2SubCategory = ((ICombi)p2).GetParam(ParameterNames.CombiParameterName.SubCategory).Value;
                }

                if (_hasCategoryNames)
                {
                    var program = p2 as Program;
                    _p2SubCategoryName = program != null ? program.SubCategoryAsName : ((Combi) p2).SubCategoryAsName;
                }
            }
        }
    }
}
