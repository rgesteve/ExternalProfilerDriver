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
    public class VTuneInvoker_Test
    {
        [TestMethod]
        public void TestVTunePath()
        {
            Assert.IsTrue(File.Exists(VTuneInvoker.VTunePath()));
        }
    }

    [TestClass]
    public class VTuneCollectHotspotsSpec_Test
    {
        [TestMethod]
        public void TestFullCLI()
        {
            string known_fullCLI = "-collect hotspots -user-data-dir=" + Path.GetTempPath();
            string workloadSpec = "test";
            VTuneCollectHotspotsSpec spec = new VTuneCollectHotspotsSpec() { WorkloadSpec = workloadSpec };
            string known_collectSpec = spec.FullCLI();
            
            Assert.IsTrue(known_collectSpec.Contains(known_fullCLI));
            Assert.IsTrue(known_collectSpec.Contains(workloadSpec));

        }

    }

    [TestClass]
    public class VTuneReportCallstacksSpec_Test
    {
        [TestMethod]
        public void TestFullCLI()
        {
            string known_reportName = "-report callstacks -call-stack-mode user-plus-one -user-data-dir=" + Path.GetTempPath();
            string known_reportOutput = "-report-output=" + Path.GetTempPath();
            VTuneReportCallstacksSpec callstacksSpec = new VTuneReportCallstacksSpec();
            string vtuneReportArgs = callstacksSpec.FullCLI();
            
            Assert.IsTrue(vtuneReportArgs.Contains(known_reportName));
            Assert.IsTrue(vtuneReportArgs.Contains(known_reportOutput));
        }
    }

    [TestClass]
    public class VTuneCPUUtilizationSpec_Test
    {
        [TestMethod]
        public void TestFullCLI()
        {
            string known_knobs = "-r-k column-by=CPUTime -r-k query-type=overtime -r-k bin_count=15";
            string known_reportOutput = "-report-output=" + Path.GetTempPath();
            string known_reportName = "-report time";
            string known_userDir = "-user-data-dir="  + Path.GetTempPath();
            VTuneCPUUtilizationSpec cputimespec = new VTuneCPUUtilizationSpec();
            string vtuneReportTimeArgs = cputimespec.FullCLI();

            Assert.IsTrue(vtuneReportTimeArgs.Contains(known_knobs));
            Assert.IsTrue(vtuneReportTimeArgs.Contains(known_reportOutput));
            Assert.IsTrue(vtuneReportTimeArgs.Contains(known_reportName));
            Assert.IsTrue(vtuneReportTimeArgs.Contains(known_userDir));
        } 
    }

}

                            
        
