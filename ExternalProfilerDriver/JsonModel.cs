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

#if false
    class ProcessSpec
    {
        public string name { get; set; }
        public int id { get; set; }
        public LongInt begin { get; set; }
        public LongInt end { get; set; }
        public bool isTarget { get; set; }
        public bool isUser { get; set; }
        public IList<int> moduleIDs { get; set; }
        public IList<ThreadSpec> threads { get; set; }
    }

    class FrameInfo
    {
        public LongInt timestamp { get; set; }
        public IList<LongInt> frameIPs { get; set; }
    }

    public class FunctionSpec
    {
        public string name { get; set; }
        [JsonProperty("base")]
        public LongInt @base { get; set; }
        public LongInt size { get; set; }
        public IList<LineSpec> lines { get; set; }

        public FunctionSpec(string _name, long _base, long _size)
        {
            name = _name;
            @base = new LongInt(0, _base);
            size = new LongInt(0, _size);
        }
    }

    public class ModuleSpec
    {
        public string name { get; set; }
        public int id { get; set; }
        public LongInt begin { get; set; }
        public LongInt end { get; set; }
        [JsonProperty("base")]
        public LongInt @base { get; set; }
        public LongInt size { get; set; }
        public IList<FunctionSpec> ranges { get; set; }
        public IList<FileIDMapSpec> fileIdMapping { get; set; }

        public ModuleSpec()
        {
            /* empty */
        }
        public ModuleSpec(string _name, int _id, IEnumerable<FunctionSpec> _ranges)
        {
            name = _name;
            id = _id;
            ranges = _ranges.ToList();
        }

        public ModuleSpec(string _name, IEnumerable<FunctionSpec> _ranges)
        {
            name = _name;
            ranges = _ranges.ToList();
        }
    }

    public class LineSpec
    {
      public int fileId      { get; set; }
      public int offset      { get; set; } 
      public int lineBegin   {get; set; } 
      public int lineEnd     {get; set; } 
      public int columnBegin {get; set; } 
      public int columnEnd   {get; set; } 
    }

    public class FileIDMapSpec
    {
        public int id { get; set; }
        public string file { get; set; }
    }

    class ThreadSpec
    {
        public int id { get; set; }
        public LongInt begin { get; set; }
        public LongInt end { get; set; }
        public IList<FrameInfo> stacks { get; set; }
    }

    class Trace
    {
        public TimeSpec totalTimeRange { get; set; }
        public string name { get; set; }
        public ProcessorSpec processor { get; set; }
        public IList<ProcessSpec> processes { get; set; }

        public IList<ModuleSpec> modules { get; set; }
    }

    class CPUUtilTrace
    {
        public LongInt beginTime = null;
        public LongInt duration = null;
        public IList<ValueTrace> counters = null;
    }

    class ValueTrace
    {
        public string id;
        public IList<CPUSample> p;
        public ValueTrace(IEnumerable<CPUSample> samples)
        {
            id = "DiagnosticsHub.Counters.Process.CPU";
            p = samples.ToList();
        }
    }

    class CPUSample
    {
        public LongInt t;
        public float v;
        public CPUSample(LongInt _t, float _v)
        {
            t = _t;
            v = _v;
        }
    }
#endif
}
