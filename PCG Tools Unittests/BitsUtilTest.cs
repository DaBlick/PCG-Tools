using Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// (c) 2011 Michel Keijzers

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class BitsUtilTest
    {
        [TestMethod]
        public void TestSetBits1()
        {
            var array = new byte[5];
            array[3] = 0x99;
            BitsUtil.SetBits(array, 3, 4, 2, 0x02);
            Assert.AreEqual(array[3], 0x89);

            Assert.AreEqual(0x02, BitsUtil.GetBits(array, 3, 4, 2));
        }

        [TestMethod]
        public void TestSetBits2()
        {
            var array = new byte[5];
            array[3] = 0x00;
            BitsUtil.SetBits(array, 3, 7, 0, 0x0E3);
            Assert.AreEqual(array[3], 0xE3);

            Assert.AreEqual(0xE3, BitsUtil.GetBits(array, 3, 7, 0));
        }

        [TestMethod]
        public void TestSetBit()
        {
            var array = new byte[5];
            array[3] = 0x77;
            BitsUtil.SetBit(array, 3, 4, 0);
            Assert.AreEqual(BitsUtil.GetBit(array, 3, 4), false);

            BitsUtil.SetBit(array, 3, 4);
            Assert.AreEqual(BitsUtil.GetBit(array, 3, 4), true);

            BitsUtil.SetBit(array, 3, 4);
            Assert.AreEqual(BitsUtil.GetBit(array, 3, 4), true);
        }
    }
}