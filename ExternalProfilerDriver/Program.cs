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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace ExternalProfilerDriver
{
    
    class ProgramOptions {
        [Option('p', "path", HelpText = "Report VTune path")]
        public bool ReportVTunePath { get; set; }

        [Option('n', "dry-run", HelpText = "Whether I should execute or just pretend to execute")]
        public bool DryRunRequested { get; set; }

        [Option('c', "callstack", HelpText = "Specify the pre-generated callstack report to process")]
        public string CallStackFNameToParse { get; set; }
        
        [Option('s', "sympath", HelpText = "Specify the path(s) to search symbols in")]
        public string SymbolPath { get; set; }

        [Option('k', "console", HelpText = "Drop into console once the profiler data is collected and processed")]
        public bool ConsoleRequested { get; set; }

        [Option('d', "dwjsondir", HelpText = "Specify the directory in which to dump resulting dwjson (contents are overwritten)")]
        public string DWJsonOutDir { get; set; }

        [Value(0)]
        public IEnumerable<string> Rest { get; set; }
    }

    class Program {

        static void Main(string[] args) {
            
            string dwjsonDir = "";

            var parser = new Parser(config => {
                config.EnableDashDash = true;
            });


            var res = parser.ParseArguments<ProgramOptions>(args)
                            .WithParsed<ProgramOptions>(opts => {

                                if (opts.CallStackFNameToParse != null) {
                                    try {
                                        // TODO: test /tmp/results_20180314/r_stacks_0004.csv
                                        ParseStackReport(opts.CallStackFNameToParse);
                                    } catch (Exception ex) {
                                        Console.WriteLine($"Couldn't parse callstack file with error: {ex.Message}");
                                        Environment.Exit(1);
                                    }

                                    if (opts.ConsoleRequested) {
                                        Console.WriteLine("Should be opening console");
                                    } else {
                                        Console.WriteLine("Should be duping");
                                    }

                                    Environment.Exit(0);
                                }

                                string vtuneExec = "";

                                try {
                                    vtuneExec = VTuneInvoker.VTunePath();
                                } catch (VTuneNotInstalledException ex) {
                                    Console.WriteLine($"VTune not found in expected path: {ex.Message}");
                                    Environment.Exit(1);
                                }

                                if (opts.ReportVTunePath)
                                {
                                    Console.WriteLine($"The path of VTune is: {vtuneExec}");
                                    Environment.Exit(0);
                                }
                                
                                if (opts.SymbolPath != null) {
                                    Console.WriteLine($"Somehow the thing thinks I have a symbolpath, and it is [{opts.SymbolPath}]");
                                }
                                
                                var RestArgs = opts.Rest.ToList();
                                VTuneCollectHotspotsSpec spec = new VTuneCollectHotspotsSpec()
                                {
                                    WorkloadSpec = String.Join(" ", RestArgs)
                                };
                                
                                if (opts.SymbolPath != string.Empty) {
                                    spec.SymbolPath = opts.SymbolPath;
                                }
                                
                                string vtuneCollectArgs = spec.FullCLI();

                                VTuneReportCallstacksSpec repspec = new VTuneReportCallstacksSpec();
                                string vtuneReportArgs = repspec.FullCLI();

                                VTuneCPUUtilizationSpec reptimespec = new VTuneCPUUtilizationSpec();
                                string vtuneReportTimeArgs = reptimespec.FullCLI();

                                // If output directory requested and it does not exist, create it

                                if (!opts.DryRunRequested) {
                                    if (opts.DWJsonOutDir == null) {
                                        Console.WriteLine($"Need an output directory unless in dry run.");
                                        Environment.Exit(1);
                                    } else {
                                        if (!Directory.Exists(opts.DWJsonOutDir)) {
                                            try {
                                                Directory.CreateDirectory(opts.DWJsonOutDir);
                                            } catch (Exception ex) {
                                                Console.WriteLine($"Couldn't create specified directory [{opts.DWJsonOutDir}]: {ex.Message}");
                                                Environment.Exit(1);
                                            }
                                        }
                                        dwjsonDir = opts.DWJsonOutDir;
                                    }
                                }

                                if (!opts.DryRunRequested) {
                                    Console.WriteLine($"Collect command line is: [ {vtuneExec} {vtuneCollectArgs} ]");
                                    ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneCollectArgs);

                                    Console.WriteLine($"Report callstacks line: [ {vtuneExec} {vtuneReportArgs} ]");
                                    ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportArgs);

                                    Console.WriteLine($"Report timing line: [ {vtuneExec} {vtuneReportTimeArgs} ]");
                                    ProcessAsyncRunner.RunWrapper(vtuneExec, vtuneReportTimeArgs);
                                } else {
                                    Console.WriteLine($"Collect command line is: [ {vtuneExec} {vtuneCollectArgs} ]");
                                    Console.WriteLine("Report command lines");
                                    Console.WriteLine($"[ {vtuneExec} {vtuneReportArgs} ]");
                                    Console.WriteLine($"[ {vtuneExec} {vtuneReportTimeArgs} ]");

                                    Environment.Exit(0);
                                }

                                /*
                                Console.WriteLine($"Please check for generated file: [{repspec.ReportOutputFile}]");
                                Console.WriteLine($"\tand also [{reptimespec.ReportOutputFile}]");
                                Console.WriteLine($"(Which I should process and dump at directory [{dwjsonDir}]");
                                */
#if false
                                double runtime = VTuneToDWJSON.CSReportToDWJson(repspec.ReportOutputFile, Path.Combine(dwjsonDir,"Sample.dwjson"));
                                VTuneToDWJSON.CPUReportToDWJson(reptimespec.ReportOutputFile, Path.Combine(dwjsonDir, "Session.counters"), runtime);

                                Console.WriteLine($"Which I dumped at directory [{dwjsonDir}]");

#if false
                                var stackReportFName = repspec.ReportOutputFile;
                                if (!File.Exists(stackReportFName)) {
                                    Console.WriteLine("Cannot find the VTune report, something went wrong with the profiler process.");
                                    Environment.Exit(1);
                                }
#endif
#endif // this is the general one
                            })
                            .WithNotParsed(errors => {
                                Console.WriteLine("Incorrect command line.");
                                Environment.Exit(1);
                            });

            Environment.Exit(0);
        }

        public class ParseStackException : Exception
        {
            public ParseStackException() {
                /* empty */
            }

            public ParseStackException(string message) : base(message)
            {
                /* empty */
            }

            public ParseStackException(string message, Exception inner) : base(message, inner)
            {
                /* empty */
            }
        }

        private static void ParseStackReport(string fname)
        {

            string possibleFn = fname;
            if (!File.Exists(possibleFn)) {
                // The [old] argument parsing library chokes on absolute Linux paths (it gets confused apparently by leading '/')
                possibleFn = Path.DirectorySeparatorChar + possibleFn;
                if (!File.Exists(possibleFn)) {
                    throw new ParseStackException($"Cannot find callstack file {fname}");
                }
            }
            try {
                var samples = VTuneToDWJSON.ParseFromFile(possibleFn);

#if true
                foreach(var s in samples) {
                    Console.WriteLine($"The top of the stack: {s.TOSFrame.ToString()}");
                    int branchCount = 0;
                    foreach (var ss in s.Stacks) {
                        branchCount++;
                        Console.WriteLine($"\tbranch: {branchCount}");
                        foreach (var frame in ss) {
                            //if (!(frame.FunctionFull.StartsWith('[') || frame.FunctionFull.StartsWith('<'))) {
                                //Console.WriteLine($"\t\t{frame.ToString()}");
                                Console.WriteLine($"\t\t({frame.FunctionFull}, {frame.SourceFile})");
                            //}
                        }
                    }
                }
#else
                int sample_counter = 1;
                foreach (var s in samples.Take(5))
                {
                    int current_top = sample_counter;
                    //Console.WriteLine("{0}, {1}", s.TOSFrame.Function, s.TOSFrame.CPUTime);
                    Console.WriteLine($"<tr data-tt-id=\"{current_top}\"><td>{s.TOSFrame.Function}</td><td>{s.TOSFrame.CPUTime}</td><td>{s.TOSFrame.Module}</td><td>{s.TOSFrame.FunctionFull}</td><td>{s.TOSFrame.SourceFile}</td><td>{s.TOSFrame.StartAddress}</td></tr>");
                    foreach (var ss in s.Stacks.Take(1))
                    {
                        foreach (var p in ss.Take(5))
                        {
                            sample_counter += 1;
                            //Console.WriteLine($"\t{p.Function}");
                            Console.WriteLine($"<tr data-tt-id=\"{sample_counter}\" data-tt-parent-id=\"{current_top}\"><td>{p.Function}</td><td>{p.CPUTime}</td><td>{p.Module}</td><td>{p.FunctionFull}</td><td>{p.SourceFile}</td><td>{p.StartAddress}</td></tr>");
                        }
                    }
                }
                Console.WriteLine($"Got {samples.Count()} samples.");
#endif
            } catch (Exception ex) {
                Console.WriteLine($"Caught an error, with message: [{ex.StackTrace}]");
            }
        }
    }
}
