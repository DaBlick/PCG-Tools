using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools;
using PcgTools.Model.Common.File;


// (c) 2011 Michel Keijzers
using PcgTools.Model.Common.Synth.MemoryAndFactory;
using PcgTools.Model.Common.Synth.PatchCombis;
using PcgTools.Model.Common.Synth.PatchPrograms;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class CommandLineArgumentTest
    {
        const string PcgFileName = @"C:\PCG Tools Test Files\TestFiles\Workstations\Kronos\DEFAULT.pcg";

        static void Run(CommandLineArguments args)
        {
            var korgFileReader = new KorgFileReader();
            var memory = korgFileReader.Read(args.PcgFileName);
            
            if (memory is PcgMemory)
            {
                var pcgMemory = memory as PcgMemory;
                args.ListGenerator.PcgMemory = pcgMemory;

                foreach (IProgramBank item in pcgMemory.ProgramBanks.BankCollection)
                {
                    args.ListGenerator.SelectedProgramBanks.Add(item);
                }
                
                foreach (ICombiBank item in pcgMemory.CombiBanks.BankCollection)
                {
                    args.ListGenerator.SelectedCombiBanks.Add(item);
                }

                args.ListGenerator.Run();
            }
        }


        [TestMethod]
        public void TestHelp()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            cla.Run(new[ ] { "-h"});
            Assert.IsFalse(File.Exists("output.txt"));
        }


        [TestMethod]
        public void TestWrongArguments()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            cla.Run(new[] { "-illegal" });
            Assert.IsFalse(File.Exists("output.txt"));
        }


        [TestMethod]
        public void TestDefault()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            const string outputFileName = "output.txt";
            cla.Run(new[] { PcgFileName, "Patch", "Default", outputFileName });
            Run(cla);

            // Length.
            var lines = File.ReadAllLines(outputFileName);
            Assert.AreEqual(2178, lines.Length);
        }


        [TestMethod]
        public void TestOutputXml()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            const string outputFileName = "output.xml";
            cla.Run(new[] { "-o", "xml", PcgFileName, "Patch", "Short", outputFileName });
            Run(cla);

            // Length.
            var lines = File.ReadAllLines(outputFileName);
            Assert.AreEqual(26140, lines.Length);
        }


        [TestMethod]
        public void TestFilterText()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            const string outputFileName = "output.xml";
            cla.Run(new[] { "-f", "on", "-ft", "Piano", "-o", "xml", PcgFileName, "Patch", "Default", outputFileName });
            Run(cla);

            // Length.
            var lines = File.ReadAllLines(outputFileName);
            Assert.AreEqual(1312, lines.Length);
        }


        [TestMethod]
        public void TestSelectedPrograms()
        {
            File.Delete("output.txt");
            var cla = new CommandLineArguments();
            const string outputFileName = "output.txt";
            cla.Run(new[] { "-fcb", "None", "-fpb", "I-A,I-C", PcgFileName, "Patch", "Default", outputFileName });
            Run(cla);

            // Length.
            var lines = File.ReadAllLines(outputFileName);
            Assert.AreEqual(288, lines.Length);
        }
    }
}
