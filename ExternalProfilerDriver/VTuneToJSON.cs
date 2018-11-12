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
        static CallTreeSpecList CSReportToJson(string filename, string outfname)
        {

            if (!File.Exists(filename)) {
                throw new ArgumentException($"Specified file {filename} does not exist!");
            }
            var samples = VTuneStackParserForCPP.ParseFromFile(filename);

            Console.WriteLine($"Parsing file: {filename} int output {outfname}.");
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
                    // this is where I should be adding lstInChain
                }
                if (lstInChain.Count > 0) {
                    top.children = lstInChain;
                }
                //Console.WriteLine($"The first sample has {s.Stacks.Take(1).ToList().Count} stacks.");
                lstframes.Add(top);
            }

            CallTreeSpecList test = new CallTreeSpecList { frames = lstframes };
            return test;
#if false
            string json = JsonConvert.SerializeObject(test, Formatting.Indented, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore});
            Console.WriteLine($"The serialized model looks like {json}.");
#endif

#if false
            var trace = new Trace {
                name = Dns.GetHostName() ?? "machine-name",
                processor = new ProcessorSpec {
                    logicalCount = 4,
                    speedInMHz = 2670,
                    pointerSizeInBytes = 4,
                    highestUserAddress = new LongInt(0, 2147418111)
                },
                processes = processes,
                modules = mods.ToList()
            };

            string json = JsonConvert.SerializeObject(trace, Formatting.Indented);
            StreamWriter writer = File.CreateText(outfname);
            writer.WriteLine(json);
            writer.Close();
#endif

            //return total;
            //return 0;
        }

        public static void CPUReportToJson(string filename, string outfname, double timeTotal = 0.0)
        {

            if (!File.Exists(filename)) {
                throw new ArgumentException($"Cannot find specified CPU utilization report {filename}");
            }

#if false
            if (timeTotal <= 0) {
                throw new Exception("Invalid running time specification in CPU utilization report");
            }
#endif

#if false
            LongInt durationli = TraceUtils.ToNanoseconds(timeTotal);
#endif

            var cpuRecords = Utils.ReadFromFile(filename)
                                  .Skip(2)
                                  .ParseCPURecords();

            CPUTrace cpu = new CPUTrace { cpu = cpuRecords.Select(x => ((int)(x.CPUUtil)).ToString()).ToList() };
            string json = JsonConvert.SerializeObject(cpu, Formatting.Indented);
            Console.WriteLine($"The serialized model looks like {json}.");
#if false
            // var fs = new FileStream(@"C:\users\perf\Sample2.counters", FileMode.Create);
            var fs = new FileStream(outfname, FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fs, Encoding.Unicode)) { // encoding in Unicode here is key
                writer.WriteLine(json);
            }
#endif
        }
    
        public static void ReportToJson(string stacks_filename, string cpu_filename, string outfname) {
            Console.WriteLine($"Should be parsing stacks from {stacks_filename} and utilization from {cpu_filename}, test.");

            CallTreeSpecList ctree = VTuneToJSON.CSReportToJson(stacks_filename, "<placeholder>");
            string json = JsonConvert.SerializeObject(ctree, Formatting.Indented, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore});
            Console.WriteLine($"The serialized model for callstacks {json}.");

            VTuneToJSON.CPUReportToJson(cpu_filename, "<placeholder>" /* , runtime*/);
        }
    }


}

