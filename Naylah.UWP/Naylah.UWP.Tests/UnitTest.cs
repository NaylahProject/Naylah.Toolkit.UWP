using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Naylah.UWP.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var s = new Architecture.Sandbox();

            Assert.AreNotEqual(s.N(), s.N2());
        }
    }
}
