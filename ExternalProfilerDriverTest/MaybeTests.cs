using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExternalProfilerDriver;

namespace ExternalProfilerDriverTest
{
    [TestClass]
    public class MaybeTest
    {
        [TestMethod]
        public void TestMaybeNone()
        {
            Maybe<string> testNothing = Maybe<string>.None;

            Assert.IsFalse(testNothing.HasValue);
        }
    }
}
