using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    [TestClass]
    public class BaseSizeTupleTest
    {
        [TestMethod]
        public void TestBaseSizeTupleCtor()
        {
            var baseTest = 10;
            var sizeTest = 20;
            var bs = new BaseSizeTuple(baseTest, sizeTest);

            Assert.AreEqual(baseTest, bs.Base);
        }

        [TestMethod]
        public void TestSequenceBaseSizeGenerate()
        {
            var sbs = new SequenceBaseSize();
            CollectionAssert.AllItemsAreNotNull(sbs.Generate().Take(5).ToList());
        }
        
        [TestMethod]
        public void TestGeneratedSample()
        {
            var sbs = (new SequenceBaseSize()).Generate().Take(10).ToList();
            Assert.IsTrue(sbs[4].Base == 44 && sbs[4].Size == 10);
        }
    }
}
