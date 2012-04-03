using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Web.Testing.UI;

namespace Microsoft.Web.Testing.UnitTests.UI
{
    [TestFixture]
    public class HtmlInputElementReaderTest
    {
        [Test]
        public void CanReadCheckedTrue()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("checked", "true");
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.IsTrue(reader.Checked);
        }

        [Test]
        public void CanReadCheckedFalse()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("checked", "false");
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.IsFalse(reader.Checked);
        }

        [Test]
        public void CanReadCheckedNotSpecified()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.IsFalse(reader.Checked);
        }

        [Test]
        public void CanReadCheckedEqualsChecked()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("checked", "checked");
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.IsTrue(reader.Checked);
        }

        [Test]
        public void CanReadSize()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("size", "10");
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.AreEqual(10, reader.Size);
        }

        [Test]
        public void SizeIsNullWhenNotPresent()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            HtmlElement htmlElement = new HtmlInputElement(attributes, null, null);
            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(htmlElement);
            Assert.IsNull(reader.Size);
        }
    }
}
