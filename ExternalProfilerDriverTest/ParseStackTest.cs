using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    [TestClass]
    public class ParseStackTest
    {
        [TestMethod]
        [DeploymentItem("zlib_example.csv")]
        public void TestDeploymentItem()
        {
            string filename = "zlib_example.csv";
            int expected_sample_count = 5;
            Assert.IsTrue(File.Exists(filename));

            var samples = VTuneToDWJSON.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);
            // Trace.WriteLine($"#### testing...{samples[0]}");
            Assert.IsInstanceOfType(samples[0], typeof(SampleWithTrace));
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
