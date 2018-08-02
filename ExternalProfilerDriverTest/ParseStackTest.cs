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
            int expected_sample_count = 5;
            Assert.IsTrue(File.Exists(filename));

            var samples = VTuneToDWJSON.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);

            Assert.IsInstanceOfType(samples[0], typeof(SampleWithTrace));

            var dict = VTuneToDWJSON.ModuleFuncDictFromSamples(samples);
            Assert.IsTrue(dict.ContainsKey("libz.so.1"));

            Assert.ThrowsException<ArgumentException>(() => VTuneToDWJSON.AddLineNumbers(ref dict, "/etc/test"));
            int initial_count = dict.Count;
            VTuneToDWJSON.AddLineNumbers(ref dict, "/home/rgesteve/projects/zlib-1.2.11/build");
            Assert.AreEqual(initial_count, dict.Count);

            foreach (var r in VTuneToDWJSON.ModFunToTrace(dict)) {
                Trace.WriteLine($"**** Got module {r.name}, assigned id: [{r.id}]");
            }
            
            var mfiles = VTuneToDWJSON.SourceFilesByModule(dict);
            foreach (var r in mfiles) {
                System.Diagnostics.Trace.WriteLine($"**** (from test) found ({r.Value.Count}) files in module {r.Key}.");
            }

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
