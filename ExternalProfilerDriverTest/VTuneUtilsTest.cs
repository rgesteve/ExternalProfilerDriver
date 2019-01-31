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
    public class Option_Test
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
		
	    [TestMethod]
		public void TestEquals()
		{
			const int expected_value_x = 42;
			Option<int> x = Option.Some(expected_value_x);
			const int expected_value_y = 43;
			Option<int> y = Option.Some(expected_value_y);
			Assert.IsFalse(x.Equals(y));
		}
    }
}
