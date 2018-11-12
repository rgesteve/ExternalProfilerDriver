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
{
  "cpu": ["84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87","84","80","83","82","83","81","83","89","89","85","83","80","90","83","83","87","83","84","84","83","87","87"]
}
*/
/*
dotnet run -f netcoreapp2.0 -p ExternalProfilerDriver\ExternalProfilerDriver.csproj -- -j -- c:\users\perf\appdata\local\continuum\anaconda3\python.exe c:\users\perf\projects\examples\pybind\test\test.py
 */

using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExternalProfilerDriver
{
    class CPUTrace
    {
        public List<string> cpu;
    }

    /*
    var columnDefs = [
    {headerName: "Function", field: 'function', cellRenderer:'agGroupCellRenderer'},
    {headerName: "CPU Time", field: "cpu_time"},
    {headerName: "Module", field: "module"},
    {headerName: "Function (Full)", field: "function_full"},
    {headerName: "Source File", field: "source_file"},
    {headerName: "Start Address", field: "start_address"}
];
     */
     /* profile data:

     [{"function": "func@0x1e0897c0", "cpu_time": "12.703125", "module": "python36.dll", "function_full": "func@0x1e0897c0", "source_file": "Unknown", "start_address": "Unknown", 
    "children":[
        {"function":"_scrt_common_main_seh", "cpu_time": "12.703125", "module": "python.exe", "function_full": "_scrt_common_main_seh", "source_file": "exe_common.inl", "start_address": "0x1d001150"},
        {"function":"BaseThreadInitThunk", "cpu_time": "0", "module": "KERNEL32.DLL", "function_full": "BaseThreadInitThunk", "source_file": "[Unknown]", "start_address": "0x180012760"},
        {"function":"RtlUserThreadStart", "cpu_time": "0", "module": "ntdll.dll", "function_full": "RtlUserThreadStart", "source_file": "[Unknown]", "start_address": "0x180070d30"}
    ]},
    {"function": "jpeg_idct_16x16", "cpu_time": "1.538451", "module": "opencv_imgcodecs331.dll", "function_full": "peg_idct_16x16", "source_file": "[Unknown]", "start_address": "0x180131d90",
    "children": [
        {"function": "func@0x180123b90"         ,"cpu_time": "1.538451","module": "opencv_imgcodecs331.dll", "function_full": "func@0x180123b90",  "source_file": "[Unknown]", "start_address": "0x180123b90" },
        {"function": "func@0x1801233b0"         ,"cpu_time": "0",       "module": "opencv_imgcodecs331.dll", "function_full": "func@0x1801233b0",  "source_file": "[Unknown]", "start_address": "0x1801233b0" },
        {"function": "jpeg_read_scanlines"      ,"cpu_time": "0",       "module": "opencv_imgcodecs331.dll", "function_full": "jpeg_read_scanlines","source_file": "[Unknown]", "start_address": "0x18011c900"},
        {"function": "cv::JpegDecoder::readData","cpu_time": "0",       "module": "opencv_imgcodecs331.dll", "function_full": "cv::JpegDecoder::readData(class cv::Mat &)", "source_file": "grfmt_jpeg.cpp", "start_address": "0x1800277a0"},
        {"function": "cv::imdecode_"            ,"cpu_time": "0",       "module": "opencv_imgcodecs331.dll", "function_full": "cv::imdecode_",      "source_file": "loadsave.cpp", "start_address": "0x18000eb10"}

    ]},
    {"function": "jpeg_idct_islow" , "cpu_time": "1.214521", "module": "opencv_imgcodecs331.dll", "function_full": "jpeg_idct_16x16" , "source_file":"jpeg_idct_islow,[Unknown]", "start_address": "0x180135c20",
    "children": [
        {"function": "func@0x180123b90", "cpu_time": "1.203510", "module": "opencv_imgcodecs331.dll", "function_full": "func@0x180123b90", "source_file":"[Unknown]", "start_address": "0x180123b90"},
        {"function": "func@0x1801233b0", "cpu_time": "0"       , "module": "opencv_imgcodecs331.dll", "function_full": "func@0x1801233b0", "source_file":"[Unknown]", "start_address": "0x1801233b0"}
    
    ]}]
      */

      /*
              {"function":"_scrt_common_main_seh", 
              "cpu_time": "12.703125", 
              "module": "python.exe", 
              "function_full": "_scrt_common_main_seh", 
              "source_file": "exe_common.inl", 
              "start_address": "0x1d001150"},

       */

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
}
