// (c) 2011-2019 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsMicroKorgXlPlus : AntiCrashTests
    {
        [TestMethod]
        public void Test_MicroKorgXlPlus_KorgUSA_MicroKORGXL_bank_mkxl_all()
        {
            TestAll(@"Synths Racks and Modules\MicroKorgXlPlus\KorgUSA_MicroKORGXL_bank.mkxl_all");
        }

    }
}
