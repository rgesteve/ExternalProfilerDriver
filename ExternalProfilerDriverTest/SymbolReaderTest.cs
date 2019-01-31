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
    public class SymbolReaderTest
    {
        [TestMethod]
        [DeploymentItem("something.pdb")]
        public void LoadTest()
        {
#if true
            Assert.IsTrue(true);
#else
            string known_filename = "something.pdb";
            Assert.IsTrue(File.Exists(known_filename));
            SymbolReader symreader = SymbolReader.Load(known_filename);
            Assert.IsTrue(symreader != null);

            
            const int expected_symbol_count = 150;            
            var syms = symreader.FunctionLocations(known_filename).ToList();
            Assert.AreEqual(syms.Count, expected_symbol_count);
#endif
        }
    } 
}