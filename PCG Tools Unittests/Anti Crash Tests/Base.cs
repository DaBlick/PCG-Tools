using System.Diagnostics;
using System.IO;
using PcgTools.ListGenerator;
using PcgTools.Model.Common.File;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;
using PcgTools.Model.Common.Synth.PatchSetLists;

namespace PCG_Tools_Unittests
{
    /// <summary>
    /// Tests Triton Extreme and all other Triton series.
    /// </summary>
    public abstract class AntiCrashTests
    {
        private const string DefaultDirectory = @"C:\PCG Tools Test Files\TestFiles\";


        private PcgMemory _pcgMemory;


        private ListGenerator _generator;


        private string[] _lines;


        protected void TestAll(string path)
        {
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory) korgFileReader.Read(DefaultDirectory + path);

            Debug.Assert(_pcgMemory != null);

            // Test set list generator.
            TestDefaultPatchList();
            TestProgramUsageList();
            TestDefaultCombiContentList();
            TestDefaultFileContentList();

            // Swap program (if there is more than one program).
            var programBanks = _pcgMemory.ProgramBanks;
            if (programBanks != null)
            {
                var programBank = (ProgramBank) programBanks[0];

                if (programBank.IsFilled && programBank.IsWritable && (programBank.CountPatches > 1))
                {
                    _pcgMemory.SwapPatch(programBank[0], programBank[1]);
                }
            }

            // Swap combi (if there is more than one combi).
            var combiBanks = _pcgMemory.CombiBanks;
            if (combiBanks != null)
            {
                var combiBank = (CombiBank) combiBanks[0];
                if (combiBank.IsFilled && combiBank.IsWritable && (combiBank.CountPatches > 1))
                {
                    _pcgMemory.SwapPatch(combiBank[0], combiBank[1]);
                }
            }

            // Swap set list slot.
            var setLists = _pcgMemory.SetLists;
            if (setLists != null)
            {
                var setList0 = (SetList) setLists[0];
                if (setList0.IsFilled && setList0.IsWritable)
                {
                    _pcgMemory.SwapPatch((SetListSlot) (setList0[0]), (SetListSlot) (setList0[1]));
                }
            }

            // Test save.
            _pcgMemory.FileName = "C:\\test.pcg";
            _pcgMemory.SaveFile(false, false);

        }


        private void TestDefaultPatchList()
        {
            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();
        }


        private void TestProgramUsageList()
        {
            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();
        }


        private void TestDefaultCombiContentList()
        {
            _generator = new ListGeneratorCombiContentList();
            SetDefaults();
            Run();
        }


        private void TestDefaultFileContentList()
        {
            _generator = new ListGeneratorFileContentList();
            SetDefaults();
            Run();
        }


        private void Run()
        {
            _generator.Run(false);
            _lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
        }


        private void SetDefaults()
        {
            _generator.PcgMemory = _pcgMemory;
            _generator.FilterOnText = false;
            _generator.FilterText = string.Empty;
            _generator.FilterCaseSensitive = false;
            _generator.FilterSetListSlotDescription = true;
            _generator.SelectedProgramBanks = new ObservableBankCollection<IProgramBank>();

            if (_pcgMemory.ProgramBanks != null)
            {
                foreach (var item in _pcgMemory.ProgramBanks.BankCollection)
                {
                    _generator.SelectedProgramBanks.Add((IProgramBank)item);
                }
            }

            _generator.IgnoreInitPrograms = true;

            _generator.SelectedCombiBanks = new ObservableBankCollection<ICombiBank>();
            if (_pcgMemory.CombiBanks != null)
            {
                foreach (var item in _pcgMemory.CombiBanks.BankCollection)
                {
                    _generator.SelectedCombiBanks.Add((ICombiBank)item);
                }
            }

            _generator.IgnoreInitCombis = true;
            _generator.IgnoreFirstProgram = false;
            _generator.IgnoreMutedOffTimbres = true;
            _generator.IgnoreMutedOffFirstProgramTimbre = true;
            _generator.SetListsEnabled = false;
            _generator.SetListsRangeFrom = 0;
            _generator.SetListsRangeTo = 0;
            _generator.DrumKitsEnabled = true;
            _generator.DrumPatternsEnabled = true;
            _generator.WaveSequencesEnabled = true;
            _generator.IgnoreInitDrumKits = true;
            _generator.IgnoreInitDrumPatterns = true;
            _generator.IgnoreInitWaveSequences = true;
            _generator.SortMethod = ListGenerator.Sort.Alphabetical;
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Text;
            _generator.OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt";
            _lines = null;
        }
    }
}
