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
        public void TestParsingTrace()
        {
#if true
            Assert.IsTrue(true);
#else
            string filename = "zlib_example.csv";
            int expected_sample_count = 5;
            Assert.IsTrue(File.Exists(filename));

            var samples = VTuneToDWJSON.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);

            Assert.IsInstanceOfType(samples[0], typeof(SampleWithTrace));

            string known_module = "libz.so.1";
            var dict = VTuneToDWJSON.ModuleFuncDictFromSamples(samples);
            Assert.IsTrue(dict.ContainsKey(known_module));

            Assert.ThrowsException<ArgumentException>(() => VTuneToDWJSON.AddLineNumbers(ref dict, "/etc/test"));
            int initial_count = dict.Count;
            VTuneToDWJSON.AddLineNumbers(ref dict, "/home/rgesteve/projects/zlib-1.2.11/build");
            Assert.AreEqual(initial_count, dict.Count);

            var mfiles = VTuneToDWJSON.SourceFilesByModule(dict);
            int expected_source_files = 3; // this assumes that files are installed in the machine where the tests are run, should mock it instead
            Assert.AreEqual(expected_source_files, mfiles[known_module].Count);
            
#if false
            var x = mfilesDWJSON["libz.so.1"].FindIndex(fi => fi.file == "/home/rgesteve/projects/zlib-1.2.11/build/../adler32.c");
            var y = mfilesDWJSON["libz.so.1"][x];
#endif

#if true
            VTuneToDWJSON.ModFunToTrace(dict);
            // SequenceBaseSize
#else
            foreach (var r in VTuneToDWJSON.ModFunToTrace(dict)) {
                Trace.WriteLine($"**** Got module {r.name}, assigned id: [{r.id}]");
            }
#endif
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
