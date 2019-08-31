// (c) 2011-2019 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsTSeries : AntiCrashTests
    {
        [TestMethod]
        public void Test_TSeries_1723_syx()
        {
            TestAll(@"Workstations\TSeries\1723.syx");
        }

        [TestMethod]
        public void Test_TSeries_1729_syx()
        {
            TestAll(@"Workstations\TSeries\1729.syx");
        }

        [TestMethod]
        public void Test_TSeries_t2t3pl_mid()
        {
            TestAll(@"Workstations\TSeries\t2t3pl.mid");
        }

        [TestMethod]
        public void Test_TSeries_t2t3pl_syx()
        {
            TestAll(@"Workstations\TSeries\t2t3pl.syx");
        }

    }
}
