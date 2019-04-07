using Common.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools.ClipBoard;
using PcgTools.Model.Common.File;

using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Properties;
using PcgTools.ViewModels;
using PcgTools.ViewModels.Commands.PcgCommands;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class SettingsLikeNamedTests
    {
        const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\";


        private PcgMemory _pcgOs2;
        private PcgMemory _pcgOs3;


        private void SetUp()
        {
            var korgFileReader = new KorgFileReader();
            _pcgOs2 = (PcgMemory) korgFileReader.Read(PcgDirectory + @"\Kronos\all.PCG");
            _pcgOs3 = (PcgMemory) korgFileReader.Read(PcgDirectory + @"\Kronos2\PRELOAD_V3_2016-10-01-20-23-33.PCG");

            // Set settings.
            Settings.Default.CopyPaste_AutoExtendedSinglePatchSelectionPaste = false;

            Settings.Default.CopyPaste_CopyIncompleteCombis = false;
            Settings.Default.CopyPaste_CopyIncompleteSetListSlots = false;

            Settings.Default.CopyPaste_CopyPatchesFromMasterFile = false;

            Settings.Default.CopyPaste_OverwriteFilledCombis = false;
            Settings.Default.CopyPaste_OverwriteFilledPrograms = false;
            Settings.Default.CopyPaste_OverwriteFilledSetListSlots = false;

            Settings.Default.CopyPaste_PasteDuplicateCombis = false;
            Settings.Default.CopyPaste_PasteDuplicatePrograms = false;
            Settings.Default.CopyPaste_PasteDuplicateSetListSlots = false;

            Settings.Default.CopyPaste_PatchDuplicationName = (int) CopyPaste.PatchDuplication.DoNotUsePatchNames;
            Settings.Default.CopyPaste_IgnoreCharactersForPatchDuplication = "V2";
        }


        /// <summary>
        /// Copy I-A000 from one file to I-A000 in another file.
        /// There is an equal named patch  on U-A000 but not byte-wise equal.
        /// </summary>
        [TestMethod]
        public void DoNotUsePatchName()
        {
            RunTest(CopyPaste.PatchDuplication.DoNotUsePatchNames, "", 0, 0, true, 1, true);
        }
        

        /// <summary>
        /// Copy/Paste V2.I-A064 to V3.I-C065 while V3.I-A064 is equally named.
        /// </summary>
        [TestMethod]
        public void EqualNames()
        {
            RunTest(CopyPaste.PatchDuplication.EqualNames, "", 64, 64, true, 65, false);
        }


        /// <summary>
        /// Copy/paste I-A049 Mr. Nice :-) to I-C049 Mr. Nice V2, should not be pasted.
        /// </summary>
        [TestMethod]
        public void LikeNamesOneFragment()
        {
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, " V2", 49, 49, false, 50, false);
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, "", 49, 49, false, 50, true);
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, "V3", 49, 49, false, 50, true);
        }


        /// <summary>
        /// Copy/paste I-A049 Mr. Nice :-) to I-C049 Mr. Nice V2, should not be pasted.
        /// </summary>
        [TestMethod]
        public void LikeNamesTwoFragments()
        {
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, "V1,V2 ", 49, 49, false, 50, false);
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, "V2 ,V1", 49, 49, false, 50, false);
            RunTest(CopyPaste.PatchDuplication.LikeNamedNames, "V3 ,V1", 49, 49, false, 50, true);
        }



        private void RunTest(CopyPaste.PatchDuplication patchNameSetting, string ignoreFragments,
            int sourceIndex, int destinationCompareIndex,
            bool patchNamesEqual, int destinationIndex, bool pasted)
        {
            SetUp();
            Settings.Default.CopyPaste_OverwriteFilledPrograms = true;
            Settings.Default.CopyPaste_PatchDuplicationName = (int)patchNameSetting;
            Settings.Default.CopyPaste_IgnoreCharactersForPatchDuplication = ignoreFragments;

            var program2 = ((ProgramBank)_pcgOs2.ProgramBanks["I-A"])[sourceIndex]; // I-A000 Kronos grand.
            var commands2 = new CopyPasteCommands();
            var patches = new ObservableCollectionEx<IPatch> { program2 };

            var clipBoard = new PcgClipBoard();
            program2.IsSelected = true;
            commands2.CopyPasteCopy(clipBoard, _pcgOs2, PcgViewModel.ScopeSet.Patches, true,
                false, false, false, false, false, false,
                null, patches, false);

            var icBank = (ProgramBank)_pcgOs3.ProgramBanks["I-C"];
            if (patchNamesEqual)
            {
                Assert.AreEqual(program2.Name, icBank[destinationCompareIndex].Name);
            }

            var program3 = icBank[destinationIndex];
            var patches3 = new ObservableCollectionEx<IPatch> { program3 };

            program3.Clear();
            program3.IsSelected = true;
            Assert.AreNotEqual(program2.Name, program3.Name);

            commands2.CopyPastePaste(clipBoard, _pcgOs3, PcgViewModel.ScopeSet.Patches, true, 
                false, false, false, false, false, false,
                null, patches3);

            if (pasted)
            {
                Assert.AreEqual(program2.Name, program3.Name);
            }
            else
            {
                Assert.AreNotEqual(program2.Name, program3.Name);
            }
        }

    }
}
