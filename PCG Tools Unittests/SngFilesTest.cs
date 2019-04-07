using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcgTools.Model.Common.File;


// (c) 2011 Michel Keijzers

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class SngFilesTest
    {
        private const string PcgDirectory = @"C:\PCG Tools Test Files\TestFiles\Workstations\";


        private static void Run(string fileName)
        {
            var korgFileReader = new KorgFileReader();
            var memory = korgFileReader.Read(PcgDirectory + fileName);
            Assert.IsNotNull(memory);
        }
/*

        [TestMethod]
        public void TestKrs01()
        {
            Run(@"Kronos\KRS-01.sng");
        }


        [TestMethod]
        public void TestMoss()
        {
            Run(@"Triton Extreme\MOSS.sng");
        }


        [TestMethod]
        public void TestPreldV2()
        {
            Run(@"Karma\PRELD_V2.sng");
        }


        [TestMethod]
        public void TestPreload()
        {
            Run(@"Kronos\PRELOAD.sng");
        }


        [TestMethod]
        public void TestProSharp()
        {
            Run(@"Triton Extreme\PROSHARP.sng");
        }
 * 
 */ 
    }
}
