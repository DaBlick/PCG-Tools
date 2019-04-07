using System;
using System.Linq;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchSetLists;

namespace PcgTools.ViewModels.Commands.PcgCommands
{
    /// <summary>
    /// In case it needs to be implemented for combis:
    /// 
    /// foreach combi in Bank
    ///    copy combi to target bank
    ///    if combi ContainsTimbre with MC1 or MC2
    ///       change name with suffix ‘/MC source ’
    ///    copy combi to target bank
    ///    change name with suffix ‘/MC target ’
    ///    switch MC1 and MC2
    /// </summary>
    public class DoubleToSingleKeyboardCommands
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pcgViewModel"></param>
        public void Execute(IPcgViewModel pcgViewModel)
        {
            var window = new DoubleToSingleKeyboardWindow(pcgViewModel.SelectedPcgMemory, this);
            window.ShowDialog();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="setListSource"></param>
        /// <param name="setListTarget"></param>
        /// <param name="combiBankTarget"></param>
        /// <param name="mainMidiChannel"></param>
        /// <param name="secondaryMidiChannel"></param>
        internal void Process(ISetList setListSource, ISetList setListTarget,
            Model.Common.Synth.PatchCombis.ICombiBank combiBankTarget, 
            int mainMidiChannel, int secondaryMidiChannel)
        {
            var currentTargetSetListSlotIndex = 0;
            var currentTargetCombiIndex = 0;

            var finishedPrematurely = false;
            var errorText = string.Empty;

            if (!setListSource.IsLoaded)
            {
                finishedPrematurely = true;
                errorText = PcgToolsResources.Strings.KeyboardSetupErrorSetListsNotPresent; //TODO page 178 manual
            }
            else if (setListTarget.CountFilledPatches > 0)
            {
                finishedPrematurely = true;
                errorText = PcgToolsResources.Strings.KeyboardSetupErrorTargetSetListNotEmpty;
            }
            else if (!combiBankTarget.IsWritable)
            {
                finishedPrematurely = true;
                errorText = PcgToolsResources.Strings.KeyboardSetupErrorTargetCombiBankNotPresent;
            }
            else
            { 
                foreach (var patch in setListSource.Patches.Where(patch => !((ISetListSlot)patch).IsEmptyOrInit))
                {
                    var sourceSetListSlot = (ISetListSlot)patch;
                    if (currentTargetSetListSlotIndex >= setListTarget.Patches.Count)
                    {
                        finishedPrematurely = true;
                        errorText = PcgToolsResources.Strings.KeyboardSetupErrorNotEnoughSetListSlotsInTargetSetList;
                        break;
                    }

                    // copy set list slot to target set list.
                    // change suffix sourcemidi 
                    sourceSetListSlot.PcgRoot.CopyPatch(sourceSetListSlot, setListTarget.Patches[currentTargetSetListSlotIndex]);
                    var targetSetListSlot = setListTarget.Patches[currentTargetSetListSlotIndex];
                    targetSetListSlot.SetNameSuffix($"/MC{mainMidiChannel}");
                    currentTargetSetListSlotIndex++;

                    if (sourceSetListSlot.SelectedPatchType == SetListSlot.PatchType.Combi)
                    {
                        if (CopySecondaryPatches(setListTarget, combiBankTarget, mainMidiChannel, secondaryMidiChannel,
                            sourceSetListSlot,
                            ref currentTargetCombiIndex, ref finishedPrematurely, ref currentTargetSetListSlotIndex))
                        {
                            break;
                        }
                    }
                }
            }

            if (finishedPrematurely)
            {
                // Show message box.
                Console.WriteLine(errorText);
            }
        }


        /// <summary>
        /// Copies combi and set list slot.
        /// </summary>
        /// <param name="setListTarget"></param>
        /// <param name="combiBankTarget"></param>
        /// <param name="mainMidiChannel"></param>
        /// <param name="secondaryMidiChannel"></param>
        /// <param name="sourceSetListSlot"></param>
        /// <param name="currentTargetCombiIndex"></param>
        /// <param name="finishedPrematurely"></param>
        /// <param name="currentTargetSetListSlotIndex"></param>
        /// <returns></returns>
        private static bool CopySecondaryPatches(ISetList setListTarget, ICombiBank combiBankTarget, int mainMidiChannel,
            int secondaryMidiChannel, ISetListSlot sourceSetListSlot, ref int currentTargetCombiIndex,
            ref bool finishedPrematurely, ref int currentTargetSetListSlotIndex)
        {
            var sourceCombi = (ICombi) sourceSetListSlot.UsedPatch;
            if (sourceCombi.UsesMidiChannel(secondaryMidiChannel))
            {
                // Create second combi with changed name and MIDI channels switched.
                if (currentTargetCombiIndex >= combiBankTarget.CountPatches)
                {
                    finishedPrematurely = true;
                    return true;
                }
                sourceCombi.PcgRoot.CopyPatch(sourceCombi, combiBankTarget.Patches[currentTargetCombiIndex]);
                var combi = (ICombi) combiBankTarget.Patches[currentTargetCombiIndex];
                combi.SetNameSuffix($"/MC{secondaryMidiChannel}");
                combi.SwitchMidiChannels(mainMidiChannel, secondaryMidiChannel);
                currentTargetCombiIndex++;

                // Copy set list slot with changed name and referencing to combi.
                if (currentTargetSetListSlotIndex >= setListTarget.Patches.Count)
                {
                    finishedPrematurely = true;
                    return true;
                }

                sourceSetListSlot.PcgRoot.CopyPatch(sourceSetListSlot, setListTarget.Patches[currentTargetSetListSlotIndex]);
                var setListSlotSecondary = (ISetListSlot) setListTarget.Patches[currentTargetSetListSlotIndex];
                setListSlotSecondary.SetNameSuffix($"/MC{secondaryMidiChannel}");
                setListSlotSecondary.UsedPatch = combi;
                currentTargetSetListSlotIndex++;
            }
            return false;
        }
    }
}
