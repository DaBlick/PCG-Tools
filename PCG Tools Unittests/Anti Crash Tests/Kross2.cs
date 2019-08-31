// (c) 2011-2019 Michel Keijzers

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class AntiCrashTestsKross2 : AntiCrashTests
    {
        [TestMethod]
        public void Test_Kross2_newfile_pcg()
        {
            TestAll(@"Workstations\Kross2\NEWFILE.PCG");
        }
    }
}
