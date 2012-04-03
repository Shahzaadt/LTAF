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
    public class HtmlImageElementAttributeReaderTest
    {
        [TestMethod]
        public void AlternateTextGet()
        {
            string html = "<img id='control1' alt='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlImageElementAttributeReader reader = new HtmlImageElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.AlternateText);
        }

        [TestMethod]
        public void SourceGet()
        {
            string html = "<img id='control1' src='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlImageElementAttributeReader reader = new HtmlImageElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Source);
        }

        [TestMethod]
        public void SourceGetHref()
        {
            string html = "<img id='control1' href='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlImageElementAttributeReader reader = new HtmlImageElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Source);
        }
    }
}
