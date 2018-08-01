using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

#if (WITH_DIA)
using Dia2Lib;
#endif

using Microsoft.DotNet.PlatformAbstractions;

namespace ExternalProfilerDriver {

    class SymbolReader
    {
#if (WITH_DIA)
        IDiaDataSource _ds;
        IDiaSession _session;
#else
        string _sourceFile;
#endif

        private SymbolReader()
        {
            /* empty */
        }

        public static SymbolReader Load(string pdbpath)
        {
            try {
                if (!File.Exists(pdbpath)) throw new SymbolReaderException($"Cannot find specified file: [{pdbpath}]");
                var loader = new SymbolReader();
#if (WITH_DIA)
                loader._ds = CoCreateDiaDataSource();
                loader._ds.loadDataFromPdb(pdbpath);
                loader._ds.openSession(out loader._session);
#else
                loader._sourceFile = pdbpath;
#endif
                return loader;
            } catch (Exception) {
                throw;
            }
        }

#if (WITH_DIA)
        private static readonly Guid[] s_msdiaGuids = new[] {
            new Guid("e6756135-1e65-4d17-8576-610761398c3c"), // VS 2017 (msdia140.dll)
            new Guid("3BFCEA48-620F-4B6B-81F7-B9AF75454C7D"), // VS 2013 (msdia120.dll)
            new Guid("761D3BCD-1304-41D5-94E8-EAC54E4AC172"), // VS 2012 (msdia110.dll)
            new Guid("B86AE24D-BF2F-4AC9-B5A2-34B14E4CE11D"), // VS 2010 (msdia100.dll)
            new Guid("4C41678E-887B-4365-A09E-925D28DB33C2")  // VS 2008 (msdia90.dll)
        };

        public static IDiaDataSource CoCreateDiaDataSource()
        {
            uint i = 0;
            while (true) {
                try {
                    return (IDiaDataSource)Activator.CreateInstance(Type.GetTypeFromCLSID(s_msdiaGuids[i]));
                } catch (COMException ex) {
                    if (++i >= s_msdiaGuids.Length) throw;
                }
            }
        }
#endif

        public IEnumerable<FunctionSourceLocation> FunctionLocations()
        {
#if (WITH_DIA)
            IDiaEnumSymbols results;

            // findChildren(IDiaSymbol parent, SymTagEnum symTag, string name, uint compareFlags, out IDiaEnumSymbols ppResult);
            //_session.findChildren(_session.globalScope, SymTagEnum.SymTagCompiland, null, 0, out results); // can't find the symbolic name for nsnone
            _session.findChildren(_session.globalScope, SymTagEnum.SymTagFunction, null, 0, out results); // can't find the symbolic name for nsnone

            foreach (IDiaSymbol item in results) {
                IDiaEnumLineNumbers sourceLocs;
                _session.findLinesByRVA(item.relativeVirtualAddress, 0, out sourceLocs);
                foreach (IDiaLineNumber ln in sourceLocs) {
                    yield return new FunctionSourceLocation
                    {
                        Function = item.name,
                        SourceFile = ln.sourceFile.fileName,
                        LineNumber = ln.lineNumber
                    };
                }
            }
#else
            string line;
            using (var reader = File.OpenText(_sourceFile)) {
                uint lineCounter = 0;
                while ((line = reader.ReadLine()) != null) {
                    lineCounter++;
                    string [] functionrecord = line.Split(',');
                    if (functionrecord.Length != 3) {
                        // Cannot parse, hopefully an artifact of dumping the output of the PDB reader
                        continue;
                    }
                    yield return new FunctionSourceLocation() {
                        Function = functionrecord[0], 
                        SourceFile = functionrecord[1],
                        LineNumber = Int32.Parse(functionrecord[2])
                    };
                }
            }
#endif
        }
    }

    class SymbolReaderException : System.Exception
    {
        public SymbolReaderException()
        {
            /* empty */
        }

        public SymbolReaderException(string message): base(message)
        {
            /* empty */
        }
    }

    public class FunctionSourceLocation
    {
        public string Function { get; set; }
        public string SourceFile { get; set; }
        public long LineNumber { get; set; }
        
        override public string ToString()
        {
            return $"Function {Function} defined at {SourceFile}:{LineNumber}";
        }
    }
}
