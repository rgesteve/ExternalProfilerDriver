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

/*
dotnet run -f netcoreapp2.0 -p ExternalProfilerDriver\ExternalProfilerDriver.csproj -- -j -- c:\users\perf\appdata\local\continuum\anaconda3\python.exe c:\users\perf\projects\examples\pybind\test\test.py
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExternalProfilerDriver
{
    /* 
    {
        "cpu": ["84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87"]
    }
    */
    class CPUTrace
    {
        public List<string> cpu;
    }

    /// <summary>
    /// Looks like this is a straightforward version of PerformanceSample
    /// </summary>       
    class FunctionSummarySpec
    {
        public string function { get; set; }
        public string cpu_time { get; set; }
        public string module { get; set; }
        public string function_full {get; set;}
        public string source_file { get; set; }
        public string start_address {get; set;}
    }

    class CallTreeSpec : FunctionSummarySpec
    {
        public List<FunctionSummarySpec> children {get; set;}
    }
       
    class CallTreeSpecList
    {
        public List<CallTreeSpec> frames {get; set;}
    }

    class ModuleAttrib {
        public string module { get; set; }
        public Single time { get; set; }
        public Single fraction {get; set; }
    }
    class ModuleBreakDown
    {
        public List< ModuleAttrib > module_attribution {get; set;}
    }

    class RunSummary
    {
        public CallTreeSpecList frames { get; set; }
        public List<string> cpu { get; set; }
        public List< ModuleAttrib > module_attribution {get; set;}
    }
}
