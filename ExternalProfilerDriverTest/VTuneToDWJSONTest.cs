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
    public class BaseSizeTuple_Test
    {
        [TestMethod]
        public void TestBaseSizeTupleCtor()
        {
            var baseTest = 10;
            var sizeTest = 20;
            var bs = new BaseSizeTuple(baseTest, sizeTest);

            Assert.AreEqual(baseTest, bs.Base);
        }
    }

    [TestClass]
    public class SequenceBaseSize_Test
    {
        [TestMethod]
        public void TestSequenceBaseSizeGenerate()
        {
            var sbs = new SequenceBaseSize();
            CollectionAssert.AllItemsAreNotNull(sbs.Generate().Take(5).ToList());
        }
        
        [TestMethod]
        public void TestSequenceBaseSizeSequence()
        {
            int expected_size = 10;
            var sbs = new SequenceBaseSize();
            Assert.AreEqual(sbs.Size, expected_size);
        }
        
        [TestMethod]
        public void TestGeneratedSample()
        {
            var sbs = (new SequenceBaseSize()).Generate().Take(10).ToList();
            Assert.IsTrue(sbs[4].Base == 44 && sbs[4].Size == 10);
        }
    }

    [TestClass]
    public class VTuneToDWJSON_Test
    {
        [TestMethod]
        [DeploymentItem("zlib_example.csv")]
        public void TestParsing()
        {
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
            //int initial_count = dict.Count;
            //VTuneToDWJSON.AddLineNumbers(ref dict, "/home/rgesteve/projects/zlib-1.2.11/build");
            //Assert.AreEqual(initial_count, dict.Count);

            //var mfiles = VTuneToDWJSON.SourceFilesByModule(dict);
            //int expected_source_files = 3; // this assumes that files are installed in the machine where the tests are run, should mock it instead
            //Assert.AreEqual(expected_source_files, mfiles[known_module].Count);
            

            //var x = mfilesDWJSON["libz.so.1"].FindIndex(fi => fi.file == "/home/rgesteve/projects/zlib-1.2.11/build/../adler32.c");
            //var y = mfilesDWJSON["libz.so.1"][x];



            var modspec = VTuneToDWJSON.ModFunToTrace(dict).ToList();
            Assert.IsInstanceOfType(modspec[0], typeof(ModuleSpec));
            // SequenceBaseSize

            foreach (var r in VTuneToDWJSON.ModFunToTrace(dict)) {
                Trace.WriteLine($"**** Got module {r.name}, assigned id: [{r.id}]");
            }
        }
    }
    


}
