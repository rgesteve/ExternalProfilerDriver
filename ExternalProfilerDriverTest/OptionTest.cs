using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void TestNoneOption()
        {
            Option<int> x = Option.None<int>();
            Assert.IsFalse(x.HasValue);
        }

        [TestMethod]
        public void TestSomeOption()
        {
            const int expected_value = 42;
            Option<int> x = Option.Some(expected_value);
            Assert.IsTrue(x.HasValue);
            Assert.AreEqual(expected_value, x.GetOrElse(0));
        }
        
        [TestMethod]
        public void TestMatch()
        {
            const int expected_value = 42;
            Option<int> opt = Option.Some(expected_value);
            Assert.AreEqual(expected_value, opt.Match<int>(some : x => x, none: () => 0));
        }
    }
}
