using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace LTAF.UnitTests
{
    [TestClass]
    public class WebTestConsoleTest
    {
        [TestMethod]
        public void WriteAddsMessageToList()
        {
            int originalCount = WebTestConsole.GetTraces().Length;

            WebTestConsole.Write("foo");
            WebTestConsole.Write("bar");
            Assert.AreEqual(originalCount + 2, WebTestConsole.GetTraces().Length);
            Assert.AreEqual("foo", WebTestConsole.GetTraces()[originalCount]);
            Assert.AreEqual("bar", WebTestConsole.GetTraces()[originalCount + 1]);
        }

        [TestMethod]
        public void ClearRemovesTraces()
        {
            WebTestConsole.Write("foo");
            WebTestConsole.Write("bar");

            Assert.IsTrue(WebTestConsole.GetTraces().Length > 0);

            WebTestConsole.Clear();
            Assert.AreEqual(0, WebTestConsole.GetTraces().Length);
        }
    }
}
