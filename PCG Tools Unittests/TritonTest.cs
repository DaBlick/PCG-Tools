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
    public class TritonTest
    {
        const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\TritonExtreme";
        
        
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
                _generator.SelectedCombiBanks.Add((ICombiBank)item);
            }
            
            _generator.IgnoreInitCombis = true;
            _generator.IgnoreFirstProgram = false;
            _generator.IgnoreMutedOffTimbres = true;
            _generator.IgnoreMutedOffFirstProgramTimbre = true;
            _generator.SetListsEnabled = true;
            _generator.SetListsRangeFrom = 0;
            _generator.SetListsRangeTo = 0;
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
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\NEWFILE.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(3103, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultCombiContentList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\NEWFILE.PCG");

            _generator = new ListGeneratorCombiContentList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1485, _lines.Length);
        }


        [TestMethod]
        public void TestAllProgramBanksPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ALLPRGBA.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1618, _lines.Length);
        }


        [TestMethod]
        public void Test1ProgramBankPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\1PRGBANK.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(256, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestAllCombiBanksPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ALLCOMBA.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1613, _lines.Length);
        }
        

        [TestMethod]
        public void Test1CombiBankPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\1COMBANK.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(256, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestDrumKitArpeggiosPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\DRKITARP.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(128, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestGlobalPatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\GLOBAL.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(128, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestDefaultProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\NEWFILE.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1254, _lines.Length);
        }


        [TestMethod]
        public void TestAllProgramBanksProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ALLPRGBA.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(0, _lines.Length);
        }


        [TestMethod]
        public void Test1ProgramBankProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\1PRGBANK.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(0, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestAllCombiBanksProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\ALLCOMBA.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(1254, _lines.Length);
        }


        [TestMethod]
        public void Test1CombiBankProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\1COMBANK.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(400, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestDrumKitArpeggiosProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\DRKITARP.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(0, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestGlobalProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\GLOBAL.PCG");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(0, _lines.Length);   // Including GM
        }


        [TestMethod]
        public void TestChip2K()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\CHIP2.PCG");

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
        public void TestKnCp80PatchList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\CLS_EXB.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(1024, _lines.Length);
        }


        [TestMethod]
        public void TestKnCp80ProgramUsageList()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\CLS_EXB.pcg");

            _generator = new ListGeneratorProgramUsageList();
            SetDefaults();
            Run();

            // Length.
            Assert.AreEqual(496, _lines.Length);
        }


        [TestMethod]
        public void TestExb08()
        {
            // Run.
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgDirectory + @"\..\TritonLe\EXB08.PCG");

            _generator = new ListGeneratorPatchList();
            SetDefaults();

            if (_pcgMemory != null)
            {
                _generator.SelectedProgramBanks.RemoveAt(_pcgMemory.ProgramBanks.BankCollection.Count - 1); // Last bank is GM bank
            }

            Run();

            // Length.
            Assert.AreEqual(256, _lines.Length);
        }


    }
}
