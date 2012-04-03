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
    public class HtmlElementAttributeReaderTest
    {
        [TestMethod]
        public void IdGet()
        {
            string html = "<a id='control1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("control1", reader.Id);
        }

        [TestMethod]
        public void NameGet()
        {
            string html = "<a name='control1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("control1", reader.Name);
        }

        [TestMethod]
        public void ClassGet()
        {
            string html = "<a id='control1' class='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Class);
        }

        [TestMethod]
        public void TitleGet()
        {
            string html = "<a id='control1' title='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Title);
        }

        [TestMethod]
        public void IndexGet()
        {
            string html = "<a id='control1' title='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader["title"]);
        }

        [TestMethod]
        public void Get()
        {
            string html = "<a id='control1' title='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Get<string>("title"));
        }

        [TestMethod]
        public void GetWithDefault()
        {
            string html = "<a id='control1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Get<string>("title", "foo"));
        }

        [TestMethod]
        public void DictionaryGet()
        {
            string html = "<a id='control1' title='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlElementAttributeReader reader = new HtmlElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Dictionary["title"]);
        }
    }
}
