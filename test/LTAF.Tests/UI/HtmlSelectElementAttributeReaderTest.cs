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
    public class HtmlSelectElementAttributeReaderTest
    {
        [TestMethod]
        public void TabIndexGet()
        {
            string html = "<select id='control1' tabindex='5' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(5, reader.TabIndex);
        }

        [TestMethod]
        public void DisabledGet()
        {
            string html = "<select id='control1' disabled='disabled' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Disabled);
        }

        [TestMethod]
        public void DisabledGetFalse()
        {
            string html = "<select id='control1' disabled='false' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(false, reader.Disabled);
        }

        [TestMethod]
        public void MultipleGet()
        {
            string html = "<select id='control1' multiple='true' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Multiple);
        }

        [TestMethod]
        public void SizeGet()
        {
            string html = "<select id='control1' size='5' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(5, reader.Size);
        }

        [TestMethod]
        public void LengthGet()
        {
            string html = "<select id='control1' length='4' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(4, reader.Length);
        }

        [TestMethod]
        public void ValueGet()
        {
            string html = "<select id='control1' value='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Value);
        }

        [TestMethod]
        public void SelectedIndexGet()
        {
            string html = "<select id='control1' selectedIndex='4' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlSelectElementAttributeReader reader = new HtmlSelectElementAttributeReader(element);
            UnitTestAssert.AreEqual(4, reader.SelectedIndex);
        }
    }
}
