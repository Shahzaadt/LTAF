using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.Engine;
using System.Threading;

namespace LTAF.UnitTests.Engine
{
    [TestClass]
    public class BrowserInfoTest
    {
        [TestMethod]
        public void InfoMessagesGetSet()
        {
            BrowserInfo info = new BrowserInfo();
            Assert.IsNull(info.InfoMessages);

            info.InfoMessages = "foo";
            Assert.AreEqual("foo", info.InfoMessages);
        }

        [TestMethod]
        public void DataGetSet()
        {
            BrowserInfo info = new BrowserInfo();
            Assert.IsNull(info.Data);

            info.Data = "foo";
            Assert.AreEqual("foo", info.Data);
        }

        [TestMethod]
        public void JavascriptErrorMessagesGetSet()
        {
            BrowserInfo info = new BrowserInfo();
            Assert.IsNull(info.JavascriptErrorMessages);

            info.JavascriptErrorMessages = "foo";
            Assert.AreEqual("foo", info.JavascriptErrorMessages);
        }

        [TestMethod]
        public void ErrorMessagesGetSet()
        {
            BrowserInfo info = new BrowserInfo();
            Assert.IsNull(info.ErrorMessages);

            info.ErrorMessages = "foo";
            Assert.AreEqual("foo", info.ErrorMessages);
        }

    }
}
