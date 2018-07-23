using System;
using System.IO;
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
        [DeploymentItem("zlib_example.csv")]
        public void TestDeploymentItem()
        {
            string filename = "zlib_example.csv";
            Assert.IsTrue(File.Exists(filename));
#if false
            using (var target = new CSVSource<Sample>(filename))
            {
                Assert.IsNotNull(target.ReadNext());
                Assert.IsNotNull(target.ReadNext());
                Assert.IsNull(target.ReadNext());
                target.Close();
            }
#endif
        }

        [TestMethod]
        [DeploymentItem("mock-funcsline.csv")]
        public void VerifyFuncLines()
        {
            string filename = "mock-funcsline.csv";
            Assert.IsTrue(File.Exists(filename));
        }
    }
}
