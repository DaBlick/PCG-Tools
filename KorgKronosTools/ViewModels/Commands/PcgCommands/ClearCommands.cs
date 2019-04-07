using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchInterfaces;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.PcgToolsResources;
using PcgTools.Properties;

namespace PcgTools.ViewModels.Commands.PcgCommands
{
    /// <summary>
    /// 
    /// </summary>
    public class ClearCommands
    {
        /// <summary>
        /// 
        /// </summary>
        public enum ClearPatchesAlgorithm
        {
            /// <summary>
            /// Don't clear both used and unused patches.
            /// </summary>
            None,
            
            /// <summary>
            /// Skip patches which are used, clear unused.
            /// </summary>
            UnusedOnly,

            /// <summary>
            /// If patches are used, ask to continue (and clean all).
            /// </summary>
            Ask,

            /// <summary>
            /// Always clear both unused and used patches.
            /// </summary>
            UnusedAndUsed
        }


        /// <summary>
        /// 
        /// </summary>
        private IPcgViewModel _pcgViewModel;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgViewModel"></param>
        /// <param name="selectedPatches"></param>
        /// <returns>True if at least one patch cleared</returns>
        public bool ClearPatches(IPcgViewModel pcgViewModel, List<IPatch> selectedPatches)
        {
            _pcgViewModel = pcgViewModel;
            var atLeastOnePatchUsedAsReference =
                selectedPatches.OfType<IReferencable>().Any(reference => reference.NumberOfReferences > 0);

            bool clearUnusedPatches;
            bool clearUsedPatches;
            CheckPatchesToClear(atLeastOnePatchUsedAsReference, out clearUnusedPatches, out clearUsedPatches);
            
            if (!clearUnusedPatches && !clearUsedPatches)
            {
                return false;
            }

            var atLeastOnePatchCleared = false;

            foreach (var patch in selectedPatches.Where(
                patch => !(patch is IReferencable) || // Set list slots
                         ((((IReferencable) patch).NumberOfReferences > 0) && clearUsedPatches) || // Used patch
                         ((((IReferencable) patch).NumberOfReferences == 0) && clearUnusedPatches))) // Unused patch
            {
                if (Settings.Default.UI_ClearPatchesFixReferences)
                {
                    IPatch firstDuplicate = null;
                    if ((patch is IProgram) || (patch is ICombi))
                    {
                        firstDuplicate = patch.FirstDuplicate;
                    }

                    if (firstDuplicate != null)
                    {
                        (patch as IReferencable).ChangeReferences(firstDuplicate);
                    }
                }

                patch.Clear();
                atLeastOnePatchCleared = true;
            }

            return atLeastOnePatchCleared;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="atLeastOnePatchUsedAsReference"></param>
        /// <param name="clearUnusedPatches"></param>
        /// <param name="clearUsedPatches"></param>
        /// <returns>True for cancelling</returns>
        private void CheckPatchesToClear(bool atLeastOnePatchUsedAsReference, out bool clearUnusedPatches,
            out bool clearUsedPatches)
        {
            clearUnusedPatches = false;
            clearUsedPatches = false;

            switch ((ClearPatchesAlgorithm)Settings.Default.UI_ClearPatches)
            {
                case ClearPatchesAlgorithm.None:
                    clearUnusedPatches = !atLeastOnePatchUsedAsReference;
                    break;

                case ClearPatchesAlgorithm.UnusedOnly:
                    clearUnusedPatches = true;
                    break;

                case ClearPatchesAlgorithm.Ask:
                    if (CheckPatchesToClearAfterWarning(atLeastOnePatchUsedAsReference, ref clearUnusedPatches,
                        ref clearUsedPatches))
                    {
                        clearUnusedPatches = false;
                        clearUsedPatches = false;
                    }
                    break;

                case ClearPatchesAlgorithm.UnusedAndUsed:
                    clearUnusedPatches = true;
                    clearUsedPatches = true;
                    break;
            }
        }


        /// <summary>
        /// Shows a warning to decide which patches to clear.
        /// </summary>
        /// <param name="atLeastOnePatchUsedAsReference"></param>
        /// <param name="clearUnusedPatches"></param>
        /// <param name="clearUsedPatches"></param>
        /// <returns>True for cancelling</returns>
        private bool CheckPatchesToClearAfterWarning(bool atLeastOnePatchUsedAsReference, ref bool clearUnusedPatches,
            ref bool clearUsedPatches)
        {
            if (!atLeastOnePatchUsedAsReference)
            {
                clearUnusedPatches = true;
                return false;
            }

            var result = _pcgViewModel.ShowMessageBox(
                Strings.ClearWarning, Strings.PcgTools, WindowUtils.EMessageBoxButton.YesNoCancel,
                WindowUtils.EMessageBoxImage.Warning, WindowUtils.EMessageBoxResult.Cancel);

            switch (result)
            {
                case WindowUtils.EMessageBoxResult.Cancel:
                    return true;

                case WindowUtils.EMessageBoxResult.No:
                    clearUnusedPatches = true;
                    break;

                case WindowUtils.EMessageBoxResult.Yes:
                    clearUsedPatches = true;
                    clearUnusedPatches = true;
                    break;

                default:
                    throw new ApplicationException("Illegal case");
            }
            return false;
        }


        /// <summary>
        /// Clear duplicates only.
        /// </summary>
        /// <param name="pcgViewModel"></param>
        /// <param name="selectedPatches"></param>
        /// <returns>True if at least one patch cleared</returns>
        internal bool ClearDuplicatesPatches(PcgViewModel pcgViewModel, List<IPatch> selectedPatches)
        {
            var atLeastOneCleared = false;

            var reversedSelectedPatches = selectedPatches;
            reversedSelectedPatches.Reverse();
            foreach (var patch in reversedSelectedPatches)
            {
                var firstDuplicate = patch.FirstDuplicate;
                if (firstDuplicate != null)
                {
                    ((IReferencable) (patch)).ChangeReferences(firstDuplicate);
                    patch.Clear();
                    atLeastOneCleared = true;
                }
            }

            return atLeastOneCleared;
        }
    }
}
