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
    public class KronosCompletePcgCombiContentListCompactTest
    {
        private const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\DEFAULT.pcg";


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
                             IgnoreFirstProgram = false,
                             IgnoreMutedOffTimbres = true,
                             IgnoreMutedOffFirstProgramTimbre = true,
                             IgnoreInitCombis = true,
                             SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                             SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
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
            AssertExists("I-A021: I-A076");
            AssertExists("I-A027: I-C011");

            // Don't Ignore first program.
            AssertExists("I-A000: ");

            // Ignore muted off programs.
            AssertNotExists("I-A001: I-A000");

            // All combi banks (at least one I-A and I-D existing)
            AssertExists("I-A000: ");
            AssertExists("I-D095");

            // Skip init combis (no I-E and U-A).
            AssertNotExists("I-D097:");

            // Set lists disabled.
            AssertNotExists(": 0");

            // OutputFormat is text.
            AssertAll(": ");

            Assert.AreEqual(480, _lines.Length);
        }


        [TestMethod]
        public void TestSelectedProgramBanks()
        {
            // Set non defaults and run.
            var selection = new ObservableBankCollection<IProgramBank>
            {
                (IProgramBank) (_pcgMemory.ProgramBanks[0]), 
                (IProgramBank) (_pcgMemory.ProgramBanks[1])
            };

            _generator.SelectedProgramBanks = selection;
            Run();

            // No I-C and U-A program banks.
            AssertNotExists(": I-C");
            AssertNotExists(": U-A");

            Assert.AreEqual(480, _lines.Length);
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
            AssertExists("I-A020: I-A000"); // I-A020 does not use I-A000

            Assert.AreEqual(480, _lines.Length);
        }


        [TestMethod]
        public void TestSelectedCombiBanks()
        {
            // Set non defaults and run.
            var selection = new ObservableBankCollection<ICombiBank>
            {
                (ICombiBank) (_pcgMemory.CombiBanks[0]), 
                (ICombiBank) (_pcgMemory.CombiBanks[1])
            };

            _generator.SelectedCombiBanks = selection;
            Run();

            // No I-C and U-A combi banks.
            AssertNotExists("I-C000: ");
            AssertNotExists("U-A000: ");

            Assert.AreEqual(256, _lines.Length);
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
            AssertExists("U-G010: ");

            Assert.AreEqual(1792, _lines.Length);
        }


        [TestMethod]
        public void TestIgnoreFirstProgram()
        {
            // Set non defaults and run.
            _generator.IgnoreFirstProgram = true;
            Run();

            // No I-A000.
            AssertNotExists("I-A000: I-A000");

            Assert.AreEqual(480, _lines.Length);
        }
        

        [TestMethod]
        public void TestOutputAsciiTable()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.AsciiTable;
            _generator.OutputFileName = Path.ChangeExtension(_generator.OutputFileName, "txt");
            Run();

            // No ': '.
            AssertNotExists(": ");
            Assert.AreEqual(485, _lines.Length);
        }


        [TestMethod]
        public void TestOutputCsv()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Csv;
            _generator.OutputFileName = Path.ChangeExtension(_generator.OutputFileName, "csv");
            Run();

            // No ': '.
            AssertNotExists(": ");
            Assert.AreEqual(480, _lines.Length);
        }


        [TestMethod]
        public void TestOutputXml()
        {
            // Set non defaults and run.
            _generator.ListOutputFormat = ListGenerator.OutputFormat.Xml;
            _generator.OutputFileName = Path.ChangeExtension(_generator.OutputFileName, "xml");
            Run();

            // No ': '.
            AssertAll("<");
            AssertAll(">");
            Assert.AreEqual(13198, _lines.Length);

            _generator.OutputFileName = Path.ChangeExtension(_generator.OutputFileName, "xsl");
            Run();

            // No ': '.
            AssertAll("<");
            AssertAll(">");
            Assert.AreEqual(0x1c, _lines.Length);
        }
    }
}