// (c) 2011 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsMicroStation : AntiCrashTests
    {
        [TestMethod]
        public void Test_MicroStation_0000()
        {
            TestAll(@"Workstations\MicroStation\0000.pcg");
        }

        [TestMethod]
        public void Test_MicroStation_Dakota()
        {
            TestAll(@"Workstations\MicroStation\Dakota.pcg");
        }

        [TestMethod]
        public void Test_MicroStation_Dakota2()
        {
            TestAll(@"Workstations\MicroStation\Dakota2.pcg");
        }

        [TestMethod]
        public void Test_MicroStation_JUANB_E()
        {
            TestAll(@"Workstations\MicroStation\JUANB_E.pcg");
        }

        [TestMethod]
        public void Test_MicroStation_PRELOAD()
        {
            TestAll(@"Workstations\MicroStation\PRELOAD.pcg");
        }

        [TestMethod]
        public void Test_MicroStation_Tv04()
        {
            TestAll(@"Workstations\MicroStation\Tv04.pcg");
        }
    }
}