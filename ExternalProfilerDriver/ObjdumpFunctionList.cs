using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.DotNet.PlatformAbstractions;

namespace ExternalProfilerDriver {

    /// <summary>
    /// A symbol reader based on the output of `objdump -d -C -l --no-show-raw-insn`
    /// This is largely a port of the symbolizer in google/pprof
    /// </summary>
    public class SymbolReaderLinux
    {
        string _sourceFile;
        
        private Regex objdumpAsmOutputRE    = new Regex(@"^\s*([0-9a-dA-D]+):\s+(.*)");
        private Regex objdumpOutputFileLine = new Regex(@"^(.*):([0-9]+)");
        private Regex objdumpOutputFunction = new Regex(@"^(\S.*)\(\):");

        private SymbolReaderLinux()
        {
            /* empty */
        }

        public static SymbolReaderLinux Load(string pdbpath)
        {
            try {
                if (!File.Exists(pdbpath)) throw new SymbolReaderException($"Cannot find specified file: [{pdbpath}]");
                var loader = new SymbolReaderLinux();
                loader._sourceFile = pdbpath;
                return loader;
            } catch (Exception ex) {
                throw;
            }
        }

        public IEnumerable<FunctionSourceLocation> FunctionLocations(string preParsedFile = null)
        {
            FunctionSourceLocation fsl = null;
            IEnumerable<string> gen;

            if (preParsedFile != null) {
                gen = Utils.ReadFromFile(preParsedFile);
            } else {
                gen = Utils.QuickExecute("objdump", $"-d -C -l --no-show-raw-insn {_sourceFile}");
            }

            // System.Diagnostics.Trace.WriteLine($"**** Going to try to parse {_sourceFile}");
            foreach (var line in gen) {
                if (line != null) {
                    Match mf = objdumpOutputFunction.Match(line);
                    if (mf.Success) {
                        fsl = new FunctionSourceLocation() {
                            Function = mf.Groups[1].ToString()
                        };
                    } else {
                        Match ml = objdumpOutputFileLine.Match(line);
                        if (!ml.Success) {
                            continue;
                        }
                        if (fsl == null) {
                            continue;
                        } else {
                            fsl.SourceFile = ml.Groups[1].ToString();
                            fsl.LineNumber = Int32.Parse(ml.Groups[2].ToString());
                            yield return fsl;
                            fsl = null;
                        }
                    }
                }
            }
        }

    }
}