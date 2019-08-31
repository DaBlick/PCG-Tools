// (c) 2011-2019 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsTrinityV3 : AntiCrashTests
    {
        [TestMethod]
        public void Test_TrinityV3_EBCTRYNX_PCG()
        {
            TestAll(@"Workstations\TrinityV3\EBCTRYNX.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_GENERAL_PCG()
        {
            TestAll(@"Workstations\TrinityV3\GENERAL.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_INITMOSS_PCG()
        {
            TestAll(@"Workstations\TrinityV3\INITMOSS.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_MOSS_2_PCG()
        {
            TestAll(@"Workstations\TrinityV3\MOSS_2.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_P_LOAD1_PCG()
        {
            TestAll(@"Workstations\TrinityV3\P_LOAD1.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_SOLO_TRI_PCG()
        {
            TestAll(@"Workstations\TrinityV3\SOLO_TRI.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_V3_PLOAD_PCG()
        {
            TestAll(@"Workstations\TrinityV3\V3_PLOAD.PCG");
        }

        [TestMethod]
        public void Test_TrinityV3_V3_PROD_PCG()
        {
            TestAll(@"Workstations\TrinityV3\V3_PROD.PCG");
        }

    }
}
