// (c) 2011-2017 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsTritonTr : AntiCrashTests
    {
        [TestMethod]
        public void Test_TritonTr_PRELOAD_PCG()
        {
            TestAll(@"Workstations\TritonTr\PRELOAD.PCG");
        }

    }
}
