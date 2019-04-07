using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Extensions;

// (c) 2011 Michel Keijzers

namespace PCG_Tools_Unittests
{
    [TestClass]
    public class StringUtilTest
    {
        [TestMethod]
        public void TestSetBits1()
        {
            Assert.AreEqual(0, "A".CountCharsAroundIndex(0, 'A'));
        }

        [TestMethod]
        public void TestSetBits2()
        {
            Assert.AreEqual(1, "AA".CountCharsAroundIndex(0, 'A'));
        }

        [TestMethod]
        public void TestSetBits3()
        {
            Assert.AreEqual(1, "AA".CountCharsAroundIndex(1, 'A'));
        }

        [TestMethod]
        public void TestSetBits4()
        {
            Assert.AreEqual(0, "AB".CountCharsAroundIndex(0, 'A'));
        }

        [TestMethod]
        public void TestSetBits5()
        {
            Assert.AreEqual(1, "AB".CountCharsAroundIndex(1, 'A'));
        }

        [TestMethod]
        public void TestSetBits6()
        {
            Assert.AreEqual(2, "ABA".CountCharsAroundIndex(1, 'A'));
        }

        [TestMethod]
        public void TestSetBits7()
        {
            Assert.AreEqual(0, "ABA".CountCharsAroundIndex(0, 'A'));
        }

        [TestMethod]
        public void TestSetBits8()
        {
            Assert.AreEqual(0, "ABA".CountCharsAroundIndex(2, 'A'));
        }


        [TestMethod]
        public void Expand0()
        {
            Assert.AreEqual("Test Test", "TestTest".Expand());
        }

        [TestMethod]
        public void Expand1()
        {
            Assert.AreEqual("T Test Test", "TTestTest".Expand());
        }

        [TestMethod]
        public void Expand2()
        {
            Assert.AreEqual("Test Test", " TestTest".Expand());
        }

        [TestMethod]
        public void Expand3()
        {
            Assert.AreEqual("test Tes T", "testTesT".Expand());
        }



    }
}