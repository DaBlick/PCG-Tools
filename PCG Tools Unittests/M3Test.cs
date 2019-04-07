using System.IO;
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
    /// <summary>
    /// Tests Triton Extreme and all other Triton series.
    /// </summary>
    [TestClass]
    public class M3Test
    {
        const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\M3";


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

            foreach (var item in _pcgMemory.ProgramBanks.BankCollection)
            {
                _generator.SelectedProgramBanks.Add((IProgramBank)item);
            }

            _generator.IgnoreInitPrograms = true;

            _generator.SelectedCombiBanks = new ObservableBankCollection<ICombiBank>();

            foreach (var item in _pcgMemory.CombiBanks.BankCollection)
            {
                _generator.SelectedCombiBanks.Add((ICombiBank)item);
            }
            
            _generator.IgnoreInitCombis = true;
            _generator.IgnoreFirstProgram = false;
            _generator.IgnoreMutedOffTimbres = true;
            _generator.IgnoreMutedOffFirstProgramTimbre = true;
            _generator.SetListsEnabled = false;
            _generator.SetListsRangeFrom = 0;
            _generator.SetListsRangeTo = 0;
            _generator.DrumKitsEnabled = true;
            _generator.IgnoreInitDrumKits = true;
            _generator.DrumPatternsEnabled = true;
            _generator.IgnoreInitDrumPatterns = true;
            _generator.WaveSequencesEnabled = true;
            _generator.IgnoreInitWaveSequences = true;
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


        [TestMethod]
        public void TestDefaultPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\M3_ORGPCGV2.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(2179, _lines.Length);
        }


        [TestMethod]
        public void TestProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\M3_ORGPCGV2.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(707, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultCombiContentList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\M3_ORGPCGV2.PCG");

            _generator = new ListGeneratorCombiContentList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(567, _lines.Length);
        }


        [TestMethod]
        public void TestCombi1()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\USERC.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(130, _lines.Length);
        }


        [TestMethod]
        public void TestCombi2()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\SOUVENIR.PCG");

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
        public void TestCombi3()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\NEOSOUL.PCG");
            
            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(2, _lines.Length);
        }


        [TestMethod]
        public void TestCombi4()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\GUITARQR.PCG");

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
        public void TestCombi5()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ALEXSCM1.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(3, _lines.Length);
        }
    }
}
