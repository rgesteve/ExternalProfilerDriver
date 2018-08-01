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
    public class SymbolsSourceTest
    {
        [TestMethod]
        [DeploymentItem("objdump_output.txt")]
        public void TestSymbolizerFromPreGen()
        {
            string filename = "objdump_output.txt";
            Assert.IsTrue(File.Exists(filename));
    
            SymbolReaderLinux reader = SymbolReaderLinux.Load(filename);
            Assert.IsTrue(reader != null);

            const int expected_symbol_count = 150;            
            var syms = reader.FunctionLocations(filename).ToList();
            Assert.AreEqual(syms.Count, expected_symbol_count);
        }

#if false
        // Trying on a real ELF file, right now not checked into git
        [TestMethod]
        public void TestSymbolizerFromELF()
        {
            const int expected_symbol_count = 150;
            string filename = "/home/rgesteve/projects/zlib-1.2.11/build/libz.so.1.2.11";
            Assert.IsTrue(File.Exists(filename));

            SymbolReaderLinux reader = SymbolReaderLinux.Load(filename);
            var syms = reader.FunctionLocations(/* empty */).ToList();
            Assert.AreEqual(syms.Count, expected_symbol_count);
        }
#endif
    }
}
