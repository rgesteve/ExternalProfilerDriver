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
    public class Overall_Test
    {
        [TestMethod]
        public void TestOverall()
        {
            string vtuneExec = VTuneInvoker.VTunePath();
            Assert.IsTrue(File.Exists(vtuneExec));
           
            VTuneCollectHotspotsSpec spec = new VTuneCollectHotspotsSpec() {
                                            WorkloadSpec = String.Join(" ", "C:\\Users\\clairiky\\Work\\delete\\main.exe") };
            string vtuneCollectArgs = spec.FullCLI();
            Trace.WriteLine($"**** Got these args for collection {vtuneCollectArgs}");
            
            VTuneReportCallstacksSpec repspec = new VTuneReportCallstacksSpec();
            string vtuneReportArgs = repspec.FullCLI();
            Trace.WriteLine($"**** Got these args for report {vtuneReportArgs}");
            
            VTuneCPUUtilizationSpec reptimespec = new VTuneCPUUtilizationSpec();
            string vtuneReportTimeArgs = reptimespec.FullCLI();
            Trace.WriteLine($"**** Got these args for report {vtuneReportTimeArgs}");
            
            ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneCollectArgs);
            ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportArgs);
            ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportTimeArgs);
            
            string tmpPath = Path.GetTempPath();
            string timeStamp = DateTime.Now.ToString("MMddHHmmss");
            string dwjsonPath = Path.Combine(tmpPath, timeStamp + "_Sample.dwjson");
            string counterPath = Path.Combine(tmpPath, timeStamp + "_Session.counters");
            string jsonPath = Path.Combine(tmpPath, timeStamp + "_Sample.json");


            double runtime = VTuneToDWJSON.CSReportToDWJson(repspec.ReportOutputFile, dwjsonPath);
            VTuneToDWJSON.CPUReportToDWJson(reptimespec.ReportOutputFile, counterPath, runtime);
            VTuneToJSON.ReportToJson(repspec.ReportOutputFile, reptimespec.ReportOutputFile, jsonPath);
            
            Assert.IsTrue(File.Exists(dwjsonPath));
            Assert.IsTrue(File.Exists(counterPath));
            Assert.IsTrue(File.Exists(jsonPath));
            
        }
    }
}