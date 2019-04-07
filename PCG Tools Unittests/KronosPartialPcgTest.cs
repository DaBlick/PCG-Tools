using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools.ListGenerator;
using PcgTools.Model.Common.File;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.Meta;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class KronosPartialPcgPatchListTest
    {
        const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\";


        PcgMemory _pcgMemory;


        ListGenerator _generator;


        string[] _lines;


        private void SetDefaults()
        {
            _generator.PcgMemory = _pcgMemory;
            _generator.FilterOnText = false;
            _generator.FilterText = string.Empty;
            _generator.FilterCaseSensitive = false;
            _generator.FilterSetListSlotDescription = true;
            _generator.SelectedProgramBanks = new ObservableBankCollection<IProgramBank>();

            foreach (IProgramBank item in _pcgMemory.ProgramBanks.BankCollection)
            {
                _generator.SelectedProgramBanks.Add(item);
            }

            _generator.IgnoreInitPrograms = true;

            _generator.SelectedCombiBanks = new ObservableBankCollection<ICombiBank>();

            foreach (ICombiBank item in _pcgMemory.CombiBanks.BankCollection)
            {
                _generator.SelectedCombiBanks.Add(item);
            }
            
            _generator.IgnoreInitCombis = true;
            _generator.IgnoreFirstProgram = false;
            _generator.IgnoreMutedOffTimbres = true;
            _generator.IgnoreMutedOffFirstProgramTimbre = true;
            _generator.SetListsEnabled = true;
            _generator.SetListsRangeFrom = 0;
            _generator.SetListsRangeTo = 0;
            _generator.IgnoreInitSetListSlots = true;
            _generator.DrumKitsEnabled = true;
            _generator.IgnoreInitDrumKits = false;
            _generator.DrumPatternsEnabled = true;
            _generator.IgnoreInitDrumPatterns = false;
            _generator.WaveSequencesEnabled = true;
            _generator.IgnoreInitWaveSequences = false;

            _generator.SortMethod = ListGenerator.Sort.Alphabetical;
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Text;
            _generator.OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt";
            _lines = null;
        }


        private void Run()
        {
            _generator.Run();
            _lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
        }


        private void AssertExists(string text)
        {
            Assert.IsTrue(_lines.Count(line => line.Contains(text)) > 0);
        }


        private void AssertAll(string text)
        {
            Assert.AreEqual(_lines.Length, _lines.Count(line => line.Contains(text)));
        }


        private void AssertNotExists(string text)
        {
            Assert.AreEqual(0, _lines.Count(line => line.Contains(text)));
        }


        [TestMethod]
        public void TestPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\pipes.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(4, _lines.Length);
        }


        [TestMethod]
        public void TestProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\pipes.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(4, _lines.Length);
        }


        [TestMethod]
        public void TestPhaseoPadPcg()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\PhasoPad.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(1, _lines.Length);
        }


        [TestMethod]
        public void TestDfRhodesJimKnopfPcg()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\DF Rhodes.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(4, _lines.Length);
        }


        [TestMethod]
        public void TestAronPatchBankPcg()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\aron patch bank.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(64, _lines.Length);
        }


        [TestMethod]
        public void TestBeschKeysPcg()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\BuschKeys.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(14, _lines.Length);
        }


        [TestMethod]
        public void TestResidentEvilPcg()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ResidentEvil.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(2020, _lines.Length);
        }
    }
}
