using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    [TestClass]
    public class BaseSizeTupleTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var baseTest = 10;
            var sizeTest = 20;
            var bs = new BaseSizeTuple(baseTest, sizeTest);

            Assert.AreEqual(baseTest, bs.Base);
        }
    }
}
