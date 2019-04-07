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
    public class KronosCompletePcgProgramUsageListTest
    {
        const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\DEFAULT.pcg";


        PcgMemory _pcgMemory;


        ListGeneratorProgramUsageList _generator;


        string[] _lines;


        [TestInitialize]
        public void SetDefaults()
        {
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgFileName);

            _generator = new ListGeneratorProgramUsageList
                         {
                             PcgMemory = _pcgMemory,
                             IgnoreInitPrograms = true,
                             IgnoreFirstProgram = false,
                             IgnoreMutedOffTimbres = true,
                             IgnoreMutedOffFirstProgramTimbre = true,
                             IgnoreInitCombis = true,
                             SetListsEnabled = true,
                             SetListsRangeFrom = 0,
                             SetListsRangeTo = 0,
                             IgnoreInitSetListSlots = true,
                             SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                             SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                             DrumKitsEnabled = true,
                             IgnoreInitDrumKits = true,
                             DrumPatternsEnabled = true,
                             IgnoreInitDrumPatterns = true,
                             WaveSequencesEnabled = true,
                             IgnoreInitWaveSequences = true,
                             ListOutputFormat = ListGenerator.OutputFormat.Text,
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

            // All programs (at least one I-A and U-A existing).
            AssertExists("I-A000  : ");
            AssertExists("U-A000  : ");

            // Skip init programs (no U-G).
            AssertNotExists(": U-G");

            // Don't Ignore first program.
            AssertExists("I-A000  : ");

            // Ignore muted off programs.
            AssertNotExists("I-A000  : I-A000   I-A000   I-A001   I-A002   I-A004   I-A005   I-A006   I-A007   I-A008   I-A009   I-A010   I-A011 ");

            // All combi banks (at least one I-A and I-D existing)
            AssertExists(": I-A");
            AssertExists(": I-D");

            // Skip init combis (no I-E and U-A).
            AssertNotExists(": I-E");
            AssertNotExists(": U-A");

            // Set lists enabled (0xx/yyy).
            AssertExists(": 0");

            // OutputFormat is text.
            AssertAll(": ");

            Assert.AreEqual(889, _lines.Length);
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
            AssertNotExists("I-C000 : ");
            AssertNotExists("U-A000 : ");

            Assert.AreEqual(161, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreInitProgramsOff()
        {
            // Cannot be tested since there are no init programs; the programs in U-G are not in the PCG.
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
            AssertExists("I-A000  : I-A000   I-A001   I-A002   I-A004   I-A005   I-A006   I-A007   I-A008   I-A009   I-A010   I-A011 ");

            Assert.AreEqual(889, _lines.Length);
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
            AssertNotExists(": I-C");
            AssertNotExists(": U-A");

            Assert.AreEqual(711, _lines.Length);
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
            Assert.IsTrue(_lines[0].Contains("U-G010   U-G011"));

            Assert.AreEqual(889, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreFirstProgram()
        {
            // Set non defaults and run.
            _generator.IgnoreFirstProgram = true;
            Run();

            // No I-A000.
            AssertNotExists("I-A000 : ");

            Assert.AreEqual(888, _lines.Length);
        }

        
        [TestMethod]
        public void TestSetListsDisabled()
        {
            // Set non defaults and run.
            _generator.SetListsEnabled = false;
            Run();

            // No /.
            AssertNotExists("/");
            Assert.AreEqual(881, _lines.Length);
        }


        [TestMethod]
        public void TestSetListsRange()
        {

            // Set non defaults and run.
            _generator.SetListsRangeFrom = 1;
            _generator.SetListsRangeTo = 127;
            Run();

            // No /.
            AssertNotExists("/");
            Assert.AreEqual(881, _lines.Length);
        }


        [TestMethod]
        public void TestOutputAsciiTable()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.AsciiTable;
            Run();

            // No ': '.
            AssertNotExists(": ");
            Assert.AreEqual(893, _lines.Length);
        }


        [TestMethod]
        public void TestOutputCsv()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Csv;
            Run();

            // No ': '.
            AssertNotExists(": ");
            Assert.AreEqual(889, _lines.Length);
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
            Assert.AreEqual(18929, _lines.Length);
        }
    }
}