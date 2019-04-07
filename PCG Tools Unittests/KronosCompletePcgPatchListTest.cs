using System;
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
    public class KronosCompletePcgPatchListTest
    {
        const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\DEFAULT.pcg";

        PcgMemory _pcgMemory;

        ListGeneratorPatchList _generator;

        string[] _lines;

        [TestInitialize]
        public void SetDefaults()
        {
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory) korgFileReader.Read(PcgFileName);

            _generator = new ListGeneratorPatchList
                         {
                             PcgMemory = _pcgMemory,
                             FilterOnText = false,
                             FilterText = string.Empty,
                             FilterCaseSensitive = false,
                             FilterProgramNames = true,
                             FilterCombiNames = true,
                             FilterSetListSlotNames = true,
                             FilterSetListSlotDescription = true,
                             FilterWaveSequenceNames = true,
                             FilterDrumKitNames = true,
                             FilterDrumPatternNames = true,
                             IgnoreInitPrograms = true,
                             IgnoreInitCombis = true,
                             SetListsEnabled = true,
                             IgnoreInitSetListSlots = true,
                             SetListsRangeFrom = 0,
                             SetListsRangeTo = 0,
                             DrumKitsEnabled = true,
                             IgnoreInitDrumKits = true,
                             DrumPatternsEnabled = true,
                             IgnoreInitDrumPatterns = true,
                             WaveSequencesEnabled = true,
                             IgnoreInitWaveSequences = true,
                             SortMethod = ListGenerator.Sort.Alphabetical,
                             ListOutputFormat = ListGenerator.OutputFormat.Text, 
                             SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                             SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                             OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt"
                         };

            if (_pcgMemory != null)
            {
                foreach (var item in _pcgMemory.ProgramBanks.BankCollection)
                {
                    _generator.SelectedProgramBanks.Add((IProgramBank) item);
                }

                foreach (var item in _pcgMemory.CombiBanks.BankCollection)
                {
                    _generator.SelectedCombiBanks.Add((ICombiBank) item);
                }
            }


            _lines = null;
        }


        private void Run()
        {

            _generator.Run();
            _lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
        }


        private void AssertExists(string text)
        {
            if (text == null) throw new ArgumentNullException("text");

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
        public void TestDefault()
        {
            // Run.
            Run();

            // Filters off.
            AssertExists("Piano");
            AssertExists("Organ");

            // Filter case sensitive off.
            AssertExists("piano");

            // Filter slot list descriptions.
            AssertExists("Rudess");
            
            // All programs (at least one I-A and U-A existing).
            AssertExists(" Program      I-A");
            AssertExists(" Program      U-A");

            // Skip init programs (no U-G).
            AssertNotExists(" Program      U-G010"); // My PCG is not the real default since U-G000/1 are filled

            // All combi banks (at least one I-A and I-D existing)
            AssertExists(" Combi        I-A");
            AssertExists(" Combi        I-D");

            // Skip init combis (no I-E and U-A).
            AssertNotExists(" Combi        I-E");
            AssertNotExists(" Combi        U-A");

            // Set lists enabled (0xx/yyy).
            AssertExists(" SetListSlot ");

            // SortMethod is alphabetical.
            Assert.IsTrue(_lines[0].StartsWith("\"...lead to Rome"));

            // Length.
            Assert.AreEqual(2178, _lines.Length);
        }


        [TestMethod]
        public void TestFilteredText()
        {
            // Set non defaults and run.
            _generator.FilterOnText = true;
            _generator.FilterText = "Synth";
            Run();

            var lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
            Assert.AreEqual(58, lines.Length);
        }


        [TestMethod]
        public void TestFilterCaseSensitive()
        {
            // Set non defaults and run.
            _generator.FilterOnText = true;
            _generator.FilterText = "Synth";
            _generator.FilterCaseSensitive = true;
            Run();

            var lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
            Assert.AreEqual(56, lines.Length);
        }


        [TestMethod]
        public void TestFilterSetListDescriptionOff()
        {
            // Set non defaults and run.
            _generator.FilterOnText = true;
            _generator.FilterText = "Synth";
            _generator.FilterSetListSlotDescription = false;
            Run();

            var lines = File.ReadAllLines($"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt");
            Assert.AreEqual(54, lines.Length);
        }


        [TestMethod]
        public void TestSelectedProgramBanks()
        {
            // Set non defaults and run.
            var selection = new ObservableBankCollection<IProgramBank>
            {
                (IProgramBank)_pcgMemory.ProgramBanks[0], 
                (IProgramBank)_pcgMemory.ProgramBanks[1]
            };
            _generator.SelectedProgramBanks = selection;
            Run();

            // No I-C and U-A program banks.
            AssertNotExists(" Program      I-C000");
            AssertNotExists(" Program      U-A000");

            Assert.AreEqual(768, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreInitProgramsOff()
        {
            // Set non defaults and run.
            _generator.IgnoreInitPrograms = false;
            Run();

            // U-G010 is INIT (Due to non default Kronos PCG file G000/1 is filled)
            AssertExists(" Program      U-G010");
            
            Assert.AreEqual(2304, _lines.Length);
        }


        [TestMethod]
        public void TestSelectedCombiBanks()
        {
            // Set non defaults and run.
            var selection = new ObservableBankCollection<ICombiBank>
            {
                (ICombiBank)_pcgMemory.CombiBanks[0], 
                (ICombiBank)_pcgMemory.CombiBanks[1]
            };
            _generator.SelectedCombiBanks = selection;
            Run();

            // No I-C and U-A combi banks.
            AssertNotExists("Combi        I-C");
            AssertNotExists("Combi        U-A");
            Assert.AreEqual(1954, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreInitCombisOff()
        {
            // Set non defaults and run.
            _generator.IgnoreInitCombis = false;
            _generator.IgnoreMutedOffTimbres = false;
            _generator.IgnoreMutedOffFirstProgramTimbre = false;
            Run();

            // Combi U-G exist.
            AssertExists(" Combi        U-G");

            Assert.AreEqual(3490, _lines.Length);
        }


        [TestMethod]
        public void TestSetListsDisabled()
        {
            // Set non defaults and run.
            _generator.SetListsEnabled = false;
            Run();

            // No /.
            AssertNotExists("/0");
            Assert.AreEqual(2146, _lines.Length);
        }


        [TestMethod]
        public void TestSetListsRange()
        {

            // Set non defaults and run.
            _generator.SetListsRangeFrom = 1;
            _generator.SetListsRangeTo = 127;
            Run();

            // No /.
            AssertNotExists("/0");
            Assert.AreEqual(2146, _lines.Length);
        }


        [TestMethod]
        public void TestSortTypeBankIndex()
        {
            // Set non defaults and run.
            _generator.SortMethod = ListGenerator.Sort.TypeBankIndex;
            Run();

            Assert.IsTrue(_lines[0].Contains("I-A000"));
            Assert.AreEqual(2178, _lines.Length);
        }


        [TestMethod]
        public void TestOutputAsciiTable()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.AsciiTable;
            Run();

            Assert.AreEqual(2182, _lines.Length);
        }


        [TestMethod]
        public void TestOutputCsv()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Csv;
            Run();

            Assert.AreEqual(2178, _lines.Length);
        }


        [TestMethod]
        public void TestOutputXml()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Xml;
            Run();

            // No ': '.
            AssertAll("<");
            AssertAll(">");
            Assert.AreEqual(26140, _lines.Length);
        }
    }
}
