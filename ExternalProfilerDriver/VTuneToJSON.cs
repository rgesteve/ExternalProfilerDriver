// Python Tools for Visual Studio
// Copyright(c) 2018 Intel Corporation.  All rights reserved.
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;

using Newtonsoft.Json;

namespace ExternalProfilerDriver
{
    public class VTuneToJSON
    {
        /// <summary>
        /// <paramref name="filename"/>
        /// </summary>
        /// <param name="filename">The filename with the callstack report</param>
        static CallTreeSpecList CSReportToJson(string filename)
        {
            if (!File.Exists(filename)) {
                throw new ArgumentException($"Specified file {filename} does not exist!");
            }
            var samples = VTuneStackParserForCPP.ParseFromFile(filename);

            List<CallTreeSpec> lstframes = new List<CallTreeSpec>();
            foreach(var s in samples) {
                var t = s.TOSFrame;
                CallTreeSpec top = new CallTreeSpec {
                    function = t.Function,
                    cpu_time = t.CPUTime.ToString(),
                    module = t.Module,
                    function_full = t.FunctionFull,
                    source_file = t.SourceFile,
                    start_address = t.StartAddress
                };

                List<FunctionSummarySpec> lstInChain = new List<FunctionSummarySpec>();
                foreach (var tails in s.Stacks) {
                    foreach(var stack in tails) {
                        lstInChain.Add(new FunctionSummarySpec {
                            function = stack.Function,
                            cpu_time = stack.CPUTime.ToString(),
                            module = stack.Module,
                            function_full = stack.FunctionFull,
                            source_file = stack.SourceFile,
                            start_address = stack.StartAddress
                        });
                    }
                    // TODO: this is where I should be adding lstInChain
                }
                if (lstInChain.Count > 0) {
                    top.children = lstInChain;
                }

                lstframes.Add(top);
            }

            CallTreeSpecList ret = new CallTreeSpecList { frames = lstframes };
            return ret;
        }

        static CPUTrace CPUReportToJson(string filename)
        {

            if (!File.Exists(filename)) {
                throw new ArgumentException($"Cannot find specified CPU utilization report {filename}");
            }

            var cpuRecords = Utils.ReadFromFile(filename)
                                  .Skip(2)
                                  .ParseCPURecords();

            CPUTrace cpu = new CPUTrace { cpu = cpuRecords.Select(x => ((int)(x.CPUUtil)).ToString()).ToList() };
            return cpu;
        }

        static ModuleBreakDown CalculateComposition(string filename) {
            if (!File.Exists(filename)) {
                throw new ArgumentException($"Specified file {filename} does not exist!");
            }
            var samples = VTuneStackParserForCPP.ParseFromFile(filename);

            var relevant = samples.Select(x => x.TOSFrame).Select(x => new {Module = x.Module, CPUTime = x.CPUTime});

            var totalTime = relevant.Sum(x => x.CPUTime);
	
	        var perModule = relevant.GroupBy(x => x.Module, (key, values) => new { Module = key, Time = values.Sum(x => x.CPUTime)});
	        var perModuleNormalized = perModule.Select(x => new { Module = x.Module, Time = x.Time, Portion = (x.Time / totalTime) * 100}  );

            foreach(var r in perModuleNormalized) {
                Console.WriteLine($"Module: {r.Module} contributes {r.Portion} % of a total time of {totalTime}.");
            }

            ModuleBreakDown mod = new ModuleBreakDown { 
                module_attribution = perModuleNormalized.Select(
                    x => new ModuleAttrib {module = x.Module, 
                                           time = x.Time,
                                           fraction = x.Portion}).ToList()
            };

            return mod;
        }
    
        public static void ReportToJson(string stacks_filename, string cpu_filename, string outfname) {
            CallTreeSpecList ctree = CSReportToJson(stacks_filename);
            CPUTrace cpu = CPUReportToJson(cpu_filename);
            ModuleBreakDown modules = CalculateComposition(stacks_filename);

            RunSummary summary = new RunSummary {
                frames = ctree,
                cpu = cpu.cpu,
                module_attribution = modules.module_attribution
            };

            string json = JsonConvert.SerializeObject(summary, Formatting.Indented, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore});

            using (StreamWriter writer = File.CreateText(outfname)) {
                writer.WriteLine(json);
            }
        }
    }


}

