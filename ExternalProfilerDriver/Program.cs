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

using Newtonsoft.Json;

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

        [Option('d', "dwjsondir", HelpText = "Specify the directory in which to dump resulting dwjson (contents are overwritten)")]
        public string DWJsonOutDir { get; set; }

        [Option('j', "json", HelpText = "Specify whether json output suitable for vscode is to be generated")]
	public bool JsonOutput { get; set; }

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

// whether we should run from pre-generated report
                                if (opts.CallStackFNameToParse != null) {
                                    try {
                                        // TODO: test /tmp/results_20180314/r_stacks_0004.csv
					//            /home/rgesteve/test_collection/report_callstack.csv
					Console.WriteLine($"I should be parsing the provided file: {opts.CallStackFNameToParse}");
                                        ParseStackReport(opts.CallStackFNameToParse, opts.SymbolPath);
                                    } catch (Exception ex) {
                                        Console.WriteLine($"Couldn't parse callstack file with error: {ex.Message}");
                                        Environment.Exit(1);
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

                                if (opts.ReportVTunePath) {
                                    Console.WriteLine($"The path of VTune is: {vtuneExec}");
                                    Environment.Exit(0);
                                }

				/// TODO: Remove this part
				Console.WriteLine($"--------- Finishing here ------");
				Environment.Exit(0);

#if false // disables syspath   
                                if (opts.SymbolPath != null) {
                                    Console.WriteLine($"Somehow the thing thinks I have a symbolpath, and it is [{opts.SymbolPath}]");
                                }
#endif
                                
                                var RestArgs = opts.Rest.ToList();
                                VTuneCollectHotspotsSpec spec = new VTuneCollectHotspotsSpec() {
                                    WorkloadSpec = String.Join(" ", RestArgs)
                                };

#if false
                                if (opts.SymbolPath != string.Empty) {
                                    spec.SymbolPath = opts.SymbolPath;
                                }
#endif
                                
                                string vtuneCollectArgs = spec.FullCLI();

                                VTuneReportCallstacksSpec repspec = new VTuneReportCallstacksSpec();
                                string vtuneReportArgs = repspec.FullCLI();

                                VTuneCPUUtilizationSpec reptimespec = new VTuneCPUUtilizationSpec();
                                string vtuneReportTimeArgs = reptimespec.FullCLI();

                                // If output directory requested and it does not exist, create it

                                if (!opts.DryRunRequested) {
                                    if (opts.DWJsonOutDir == null) {
                                        Console.WriteLine($"Need an output directory unless in dry run or JSON output requested.");
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
                                if ( opts.JsonOutput) {
                                    /*
                                    dotnet run -f netcoreapp2.0 -p ExternalProfilerDriver\ExternalProfilerDriver.csproj -- -d c:\temp -j -- c:\users\perf\appdata\local\continuum\anaconda3\python.exe c:\users\perf\projects\examples\pybind\test\test.py
                                    */
				    Console.WriteLine($"Going to process filename: {repspec.ReportOutputFile}.");

                                    // this is different from generating DWJSON in that a single JSON object is generated, not two files
                                    VTuneToJSON.ReportToJson(repspec.ReportOutputFile, reptimespec.ReportOutputFile, Path.Combine(dwjsonDir, "output.json"));
                                    Environment.Exit(0);
                                } else {
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
#endif
                                }

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

        private static void ParseStackReport(string fname, string symbolPath = null)
        {
	    Console.WriteLine($"I should be parsing the provided file: {fname}, with possible symbolpath [{symbolPath}]");
            string possibleFn = fname;
            if (!File.Exists(possibleFn)) {
                // The [old] argument parsing library chokes on absolute Linux paths (it gets confused apparently by leading '/')
                possibleFn = Path.DirectorySeparatorChar + possibleFn;
                if (!File.Exists(possibleFn)) {
                    throw new ParseStackException($"Cannot find callstack file {fname}");
                }
            }
    	    Console.WriteLine($"Apparently validated that file {possibleFn} exists");

            try {
                // should I not just call VTuneToDWJSON.CSReportToDWJson
            	var samples = VTuneStackParserForCPP.ParseFromFile(possibleFn);

		Console.WriteLine($"I just parsed the file, checking out... ({samples.Count()} samples)");

		foreach (var s in samples) {
		    //Console.WriteLine($"Found sample {s} (of type {s.GetType()})");
		    Console.WriteLine($"Found sample with {s.TOSFrame.Function} at the top, time ({s.TOSFrame.CPUTime})");
		}

		return;
                
                var modFunDictionary = samples.SelectMany(sm => sm.AllSamples())
                                              .Select(p => new { Module = p.Module, Function = p.Function, SourceFile = p.SourceFile })
                                              .GroupBy(t => t.Module)
                                              .Select(g => new { Module = g.Key, 
                                                                 Functions = g.Select(gg => new { Function = gg.Function, SourceFile = gg.SourceFile}).Distinct(),  
                                                               })
                                              ;

                // Two-level dictionary module -> (function -> (base, size))
                var mfdd = modFunDictionary.Select(x => new { Module = x.Module,
                                                              Functions = x.Functions.Zip((new SequenceBaseSize()).Generate(), 
                                                                                           (f, b) => new { Function = f, BaseSize = b }
                                                                                         ).ToDictionary(t => t.Function, t => new { BaseSize = t.BaseSize, DefinedIn = 1})
                                                            }
                                                          ).ToDictionary(od => od.Module, od => od.Functions);

                if (mfdd.Count <= 0) {
                    throw new Exception("Couldn't build the module/function dictionary, can't figure out why");
                }

                foreach (var m in mfdd) {
                    Console.WriteLine($"Got a module: {m.Key}");
                    if (symbolPath != null && Directory.Exists(symbolPath)) {
                        Console.WriteLine("Checking purported symbol path");
                        var candidates = Directory.EnumerateFiles(symbolPath, m.Key + ".pdb", SearchOption.AllDirectories).ToList();
                        Console.WriteLine($"The candidate list has {candidates.Count} elements.");
                    }
                }
#if false
                foreach(var s in modFunDictionary) {
                    var funcsInModule = s.Functions.Select(r => r.Function).ToList();
                    Console.WriteLine($"{s.Module}:");
                    foreach(var f in funcsInModule) {
                        Console.WriteLine($"\t{f}");
                    }
                }
#endif

#if false
                // second pass for source line mapping
                foreach(var s in modFunDictionary) {
                    string fnamet = s.Module;
                    if (fnamet != "libz.so.1") {
                        continue;
                    } else {
                       string rootDir = System.Environment.GetEnvironmentVariable("HOME");
                       string funcFile = Path.Combine(rootDir, "Downloads", "mock-funcsline.csv");
                       fnamet = funcFile;
                    }

                    try {
                        SymbolReader symreader = SymbolReader.Load(fnamet);
                        var funcFileLine = s.Functions.Join(symreader.FunctionLocations(),
                                                                 modfun => modfun.Function,
                                                                 funline => funline.Function,
                                                                 (modfun, funline) => new { FunctionName = modfun.Function, SourceFile = funline.SourceFile, LineNumber = funline.LineNumber}
                        );
                        var filesInMod = funcFileLine.Select(x => x.SourceFile).Distinct();
                        Dictionary<string, int> fileIdDict = new Dictionary<string, int>();
                        List<FileIDMapSpec> fileidmap = Enumerable.Range(1, int.MaxValue).Zip(filesInMod, (i, f) => new FileIDMapSpec { id = i, file = f } ).ToList();
                        foreach (var r in fileidmap) {
                            fileIdDict[r.file] = r.id;
                        }
                                
                        ModuleSpec modtest = new ModuleSpec() {
                           name = s.Module,
                           begin = new LongInt(0, 0), 
                           end = new LongInt(0, 10000),
                           @base = new LongInt(0, 1000),
                           size = new LongInt(0, 300),
                           // ranges = x.Value.Select(xx => new FunctionSpec(xx.Key, xx.Value.Base, xx.Value.Size)).ToList()
                           ranges = funcFileLine.Select((ffl) => 
                                                        new FunctionSpec(ffl.FunctionName, 0, 0) { 
                                                            lines = new List<LineSpec>(Utils.Emit<LineSpec>(new LineSpec { 
                                                                fileId = fileIdDict[ffl.SourceFile], 
                                                                lineBegin = Convert.ToInt32(ffl.LineNumber) }))}
                                                       ).ToList(),
                           fileIdMapping = fileIdDict.Select((kvp, idx) => new FileIDMapSpec { id = kvp.Value, file = kvp.Key}).ToList()
                        };

                            // modules = mods.ToList()
                        string json = JsonConvert.SerializeObject(modtest/*, Formatting.Indented*/);
                        Console.WriteLine($"The generated JSON would look like {json}");

                    } catch (Exception ex) {
                        Console.WriteLine($"Caught an exception while processing functions: [{ex.Message}]");
                    }
#if false
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
#endif
                }
#if false
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
#endif
            } catch (Exception ex) {
                Console.WriteLine($"Caught an error, with message: [{ex.StackTrace}]");
            }
        }
    }
}
