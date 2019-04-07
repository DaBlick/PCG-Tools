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
    public class KronosCompletePcgDifferencesListTest
    {
        const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\PRELOAD.pcg";


        PcgMemory _pcgMemory;


        ListGeneratorDifferencesList _generator;


        string[] _lines;


        [TestInitialize]
        public void SetDefaults()
        {
            var korgFileReader = new KorgFileReader();
            _pcgMemory = (PcgMemory)korgFileReader.Read(PcgFileName);
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
            // Create.
            _generator = new ListGeneratorDifferencesList
            {
                PcgMemory = _pcgMemory,
                IgnoreInitCombis = true,
                SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                SetListsEnabled = false,
                DrumKitsEnabled = false,
                DrumPatternsEnabled = false,
                WaveSequencesEnabled = false,
                ListOutputFormat = ListGenerator.OutputFormat.Text,
                OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt"
            };
            _generator.SelectedProgramBanks.Add((IProgramBank)_pcgMemory.ProgramBanks[0]);
            _generator.SelectedProgramBanks.Add((IProgramBank)_pcgMemory.ProgramBanks[6]);
            _generator.SelectedProgramBanks.Add((IProgramBank)_pcgMemory.ProgramBanks[7]);

            foreach (var item in _pcgMemory.CombiBanks.BankCollection)
            {
                _generator.SelectedCombiBanks.Add((ICombiBank) item);
            }

            // Run.
            Run();

            // All programs (at least one I-A and U-A existing).
            AssertExists(" 6 Diffs: U-A001");

            // All combi banks (all contain U-A)
            AssertExists("U-A");

            Assert.AreEqual(27, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultCombiIgnoreNameCsv()
        {
            // Create.
            _generator = new ListGeneratorDifferencesList(450, false)
            {
                PcgMemory = _pcgMemory,
                IgnoreInitCombis = true,
                SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                SetListsEnabled = false,
                ListOutputFormat = ListGenerator.OutputFormat.Csv,
                OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.csv"
            };

            _generator.SelectedCombiBanks.Add((CombiBank) _pcgMemory.CombiBanks[0]); // [0] = I-A
            _generator.SelectedCombiBanks.Add((CombiBank)_pcgMemory.CombiBanks[2]); // [2] = I-C
            //foreach (var item in _pcgMemory.CombiBanks)
           // {
            //    _generator.SelectedCombiBanks.Add(item);
            //}

            // Run.
            Run();

            // All programs (at least one I-A).
            AssertExists("Combi, I-A010, Phantasies, 446, I-C126, Moving Synth-Bells");

            Assert.AreEqual(2, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultSetListOutputAsciiTable()
        {
            // Create.
            _generator = new ListGeneratorDifferencesList(5)
            {
                PcgMemory = _pcgMemory,
                SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                SetListsEnabled = true,
                IgnoreInitSetListSlots = true,
                SetListsRangeFrom = 0,
                SetListsRangeTo = 1,
                DrumKitsEnabled = true,
                IgnoreInitDrumKits = true,
                DrumPatternsEnabled = true,
                IgnoreInitDrumPatterns = true,
                WaveSequencesEnabled = true,
                IgnoreInitWaveSequences = true,
                ListOutputFormat = ListGenerator.OutputFormat.AsciiTable,
                OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt"
            };

            // Only 1 program bank and 2 combi banks to improve speed.
            foreach (var bank in _pcgMemory.ProgramBanks.BankCollection)
            {
                if (bank.Id == "U-A")
                {
                    _generator.SelectedProgramBanks.Add((IProgramBank)bank);
                }
            }

            for (var index = 0; index < 2; index++)
            {
                _generator.SelectedCombiBanks.Add((ICombiBank)_pcgMemory.CombiBanks[index]);
            }

            // Run.
            Run();

            Assert.AreEqual(66, _lines.Length);
        }


        [TestMethod]
        public void TestDefaultSetListOutputAsciiTableCombis()
        {
            // Create.
            _generator = new ListGeneratorDifferencesList(500) // Not low #diffs in combis, so 500
            {
                PcgMemory = _pcgMemory,
                SelectedProgramBanks = new ObservableBankCollection<IProgramBank>(),
                SelectedCombiBanks = new ObservableBankCollection<ICombiBank>(),
                SetListsEnabled = false,
                ListOutputFormat = ListGenerator.OutputFormat.AsciiTable,
                OutputFileName = $"{Path.GetFileNameWithoutExtension(_pcgMemory.FileName)}_output.txt"
            };

            for (var index = 0; index < 3; index++) // Only 3 bank to improve performance.
            {
                _generator.SelectedCombiBanks.Add((CombiBank)_pcgMemory.CombiBanks[index]);
            }

            // Run.
            Run();

            Assert.AreEqual(44, _lines.Length);
        }
    }
}