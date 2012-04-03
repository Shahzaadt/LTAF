using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF;


namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlAnchorElementAttributeReaderTest
    {
        [TestMethod]
        public void AccessKeyGet()
        {
            string html = "<a id='control1' accesskey='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlAnchorElementAttributeReader reader = new HtmlAnchorElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.AccessKey);
        }

        [TestMethod]
        public void TabIndexGet()
        {
            string html = "<a id='control1' tabindex='5' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlAnchorElementAttributeReader reader = new HtmlAnchorElementAttributeReader(element);
            UnitTestAssert.AreEqual(5, reader.TabIndex);
        }

        [TestMethod]
        public void HRefGet()
        {
            string html = "<a id='control1' href='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlAnchorElementAttributeReader reader = new HtmlAnchorElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.HRef);
        }

        [TestMethod]
        public void TargetGet()
        {
            string html = "<a id='control1' target='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlAnchorElementAttributeReader reader = new HtmlAnchorElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Target);
        }
    }
}
