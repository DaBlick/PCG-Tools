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
    public class OasysTest
    {
        const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\Oasys";

        
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
                _generator.SelectedProgramBanks.Add((IProgramBank) item);
            }
            
            _generator.IgnoreInitPrograms = true;

            _generator.SelectedCombiBanks = new ObservableBankCollection<ICombiBank>();

            foreach (var item in _pcgMemory.CombiBanks.BankCollection)
            {
                _generator.SelectedCombiBanks.Add((ICombiBank) item);
            }
            
            _generator.IgnoreInitCombis = true;
            _generator.IgnoreFirstProgram = false;
            _generator.IgnoreMutedOffTimbres = true;
            _generator.IgnoreMutedOffFirstProgramTimbre = true;
            _generator.SetListsEnabled = false;
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
        public void TestDefaultPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory) korgFileReader.Read(PcgDirectory + @"\OasysPRELOAD.PCG");
            
            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1995, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\OasysPRELOAD.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(762, _lines.Length);
        }


        [TestMethod]
        public void Test01WPack()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\01WPACK.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(6, _lines.Length);
        }


        [TestMethod]
        public void TestKnCp80()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\..\Kronos\\KN_CP80.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(10, _lines.Length);
        }


        [TestMethod]
        public void TestKnal1_3()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory +  @"\..\Kronos\\KNAL1_3.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(107, _lines.Length);
        }


        [TestMethod]
        public void TestKNMod7()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\..\Kronos\\KNAL1_3.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(107, _lines.Length);
        }


        [TestMethod]
        public void TestUg0106_1()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\..\Kronos\\UG0106_1.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(128, _lines.Length);
        }


        [TestMethod]
        public void TestAkSons()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\AK_SONS.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(1475, _lines.Length);
        }


        [TestMethod]
        public void TestAkSonsProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\AK_SONS.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(683, _lines.Length);
        }


        [TestMethod]
        public void TestEnigma()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ENIGMA.PCG");

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
        public void TestGroove01()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\GROOVE01.pcg");

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
        public void TestGroove02()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\GROOVE02.pcg");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(1433, _lines.Length);
        }
    }
}
