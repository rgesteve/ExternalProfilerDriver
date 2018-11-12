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
        public static double CSReportToJson(string filename, string outfname)
        {

            if (!File.Exists(filename)) {
                throw new ArgumentException($"Specified file {filename} does not exist!");
            }
            var samples = VTuneStackParserForCPP.ParseFromFile(filename);

#if true
            Console.WriteLine($"Parsing file: {filename} int output {outfname}.");
            foreach(var s in samples) {
                Console.WriteLine($"sample: ${s.ToString()}");
            }
#else
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
            return 0;
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
    }


}

