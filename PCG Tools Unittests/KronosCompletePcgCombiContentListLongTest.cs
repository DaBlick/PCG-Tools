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
    public class KronosCompletePcgCombiContentListLongTest
    {
        const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\DEFAULT.pcg";

        
        PcgMemory _pcgMemory;


        ListGeneratorCombiContentList _generator;


        string[] _lines;

        
        [TestInitialize]
        public void SetDefaults()
        {
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgFileName);

            _generator = new ListGeneratorCombiContentList
                         {
                             PcgMemory = _pcgMemory,
                             ListSubType = ListGenerator.SubType.Long,
                             IgnoreFirstProgram = false,
                             IgnoreMutedOffTimbres = true,
                             IgnoreMutedOffFirstProgramTimbre = true,
                             IgnoreInitCombis = true,
                             SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                             SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                             ListOutputFormat = ListGenerator.OutputFormat.AsciiTable,
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
            _lines = File.ReadAllLines(_generator.OutputFileName);
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

            // All programs (at least one I-A and U-A existing).

            // Don't Ignore first program.
            AssertExists("|I-A000    |");

            // Ignore muted off programs.

            // All combi banks (at least one I-A and I-D existing)
            AssertExists("Combi I-A000");
            AssertExists("Combi I-D095");

            // Skip init combis (no I-E and U-A).
            AssertNotExists("Combi I-D097");

            // Set lists disabled.
            AssertNotExists(": 0");

            Assert.AreEqual(9223, _lines.Length);
        }


        [TestMethod]
        public void TestSelectedProgramBanks()
        {
            // Set non defaults and run.
            var selection = new ObservableBankCollection<ICombiBank>
            {
                (ICombiBank)_pcgMemory.CombiBanks[0],
                (ICombiBank)_pcgMemory.CombiBanks[1]
            };

            _generator.SelectedCombiBanks = selection;
            Run();

            // No I-C and U-A program banks.
            AssertNotExists("Combi I-C");
            AssertNotExists("Combi U-A");

            Assert.AreEqual(4816, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreInitProgramsOff()
        {
            // Cannot be tested.
        }


        [TestMethod]
        public void TestIgnoreMutedProgramsOff()
        {
            // Cannot be tested since there are no init programs; the programs in U-G are not in the PCG.
            // Tested with TestIgnoreInitCombisOff
        }


        [TestMethod]
        public void TestDontIgnoreMutedPrograms()
        {
            // Set non defaults and run.
            _generator.IgnoreMutedOffTimbres = false;
            _generator.IgnoreMutedOffFirstProgramTimbre = false;
            Run();

            // U-G program banks would exist but are not shown since they are muted.

            Assert.AreEqual(12480, _lines.Length);
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
            AssertNotExists("Combi I-C000");
            AssertNotExists("Combi U-A000");

            Assert.AreEqual(4816, _lines.Length);
        }
        

        [TestMethod]
        public void TestIgnoreInitCombisOff()
        {
            // Set non defaults and run.
            _generator.IgnoreInitCombis = false;
            _generator.IgnoreMutedOffTimbres = false;
            _generator.IgnoreMutedOffFirstProgramTimbre = false;
            Run();

            // Combi U-G010 and 11 exist on line 0.
            AssertExists("Combi U-G010");

            Assert.AreEqual(46592, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreFirstProgram()
        {
            // Set non defaults and run.
            _generator.IgnoreFirstProgram = true;
            Run();

            // No I-A000.
            AssertNotExists("I-A000: I-A000");

            Assert.AreEqual(9071, _lines.Length);
        }


        [TestMethod]
        public void TestFilterText()
        {
            // Filter text.
            _generator.FilterOnText = true;
            _generator.FilterText = "Synth";
            _generator.FilterCaseSensitive = true;
            Run();

            Assert.AreEqual(74, _lines.Length);
        }
    }
}