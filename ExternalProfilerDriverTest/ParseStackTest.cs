using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    class FuncInfo
    {
        public string FunctionName { get; set; }
        public string SourceFile { get; set; }
        public long? Base { get; set; }
        public long? Size { get; set; }

        public FuncInfo(string _functionname, string _sourceFile = "")
        {
            FunctionName = _functionname;
            SourceFile = _sourceFile;
            Base = null;
            Size = null;
        }
    }
    
    class FuncInfoComparer : IEqualityComparer<FuncInfo>
    {
        public bool Equals(FuncInfo x, FuncInfo y)
        {
            return x.FunctionName == y.FunctionName && x.SourceFile == y.SourceFile;
        }
 
        public int GetHashCode(FuncInfo obj)
        {
            return (obj.FunctionName + obj.SourceFile).GetHashCode();
        }
    }


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

            var modFunDictionary = samples.SelectMany(sm => sm.AllSamples())
                                              .Select(p => new { Module = p.Module, Function = p.Function, SourceFile = p.SourceFile })
                                              .GroupBy(t => t.Module)
                                              .Select(g => new { Module = g.Key, 
                                                                 Functions = g.Select(gg => new FuncInfo(gg.Function, gg.SourceFile)).Distinct(new FuncInfoComparer()),
                                                               });
            // Two-level dictionary
            var mfdd = modFunDictionary.ToDictionary(r => r.Module, 
                                                     r => r.Functions.ToDictionary(
                                                         rr => rr.FunctionName,
                                                         rr => rr
                                                     ));

#if false
            // Two-level dictionary module -> (function -> (base, size))
            var mfdd = modFunDictionary.Select(x => new { Module = x.Module,
                                                          Functions = x.Functions.Zip((new SequenceBaseSize()).Generate(), 
                                                                                       (f, b) => new { Function = f, BaseSize = b }
                                                                                     ).ToDictionary(t => t.Function, t => new { BaseSize = t.BaseSize, DefinedIn = 1})
                                                        }
                                              ).ToDictionary(od => od.Module, od => od.Functions);
#endif
            foreach (var k in mfdd.Keys) {
                Trace.WriteLine($"#### testing...{k}:");
                foreach (var f in mfdd[k]) {
                    Trace.WriteLine($"#### \t{f.Key}");
                } 
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
