
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
    public class VTuneStackParser_Test
    {
        [TestMethod]
        //[DeploymentItem("r_stacks_0004.csv")]
        [DeploymentItem("zlib_example.csv")]
        public void TestParseFromFile()
        {
            string filename = "zlib_example.csv";
            Assert.IsTrue(File.Exists(filename));
            int expected_sample_count = 5;
            var samples = VTuneStackParser.ParseFromFile(filename).ToList();
            Assert.AreEqual(samples.Count, expected_sample_count);
        } 
    }
}
    
