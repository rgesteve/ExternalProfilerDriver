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
    }
}
