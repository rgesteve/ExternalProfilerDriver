using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
            const int expected_sample_count = 5;
            Assert.IsTrue(File.Exists(filename));

            var samples = VTuneToDWJSON.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);

            Assert.IsInstanceOfType(samples[0], typeof(SampleWithTrace));

            var dict = VTuneToDWJSON.ModuleFuncDictFromSamples(samples);
            Assert.IsTrue(dict.ContainsKey("libz.so.1"));
            
            const int expected_funcs_in_libz = 15;
            Assert.AreEqual(dict["libz.so.1"].Count, expected_funcs_in_libz);

#if true
            foreach (var k in dict.Keys) {
                Trace.WriteLine($"#### from the mod/fun dictionary...{k}: {dict[k].Count}");
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
