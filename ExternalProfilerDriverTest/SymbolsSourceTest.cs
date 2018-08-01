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
        public void TestDeploymentItem()
        {
            string filename = "objdump_output.txt";
            Assert.IsTrue(File.Exists(filename));
    
            SymbolReaderLinux reader = SymbolReaderLinux.Load(filename);
            Assert.IsTrue(reader != null);

            const int expected_symbol_count = 150;            
            var syms = reader.FunctionLocations().ToList();
            Assert.AreEqual(syms.Count, expected_symbol_count);
        }
    }
}
