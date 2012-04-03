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
    public class HtmlInputElementAttributeReaderTest
    {
        [TestMethod]
        public void AccessKeyGet()
        {
            string html = "<input id='control1' accesskey='a' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual("a", reader.AccessKey);
        }

        [TestMethod]
        public void AltGet()
        {
            string html = "<input id='control1' alt='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Alt);
        }

        [TestMethod]
        public void TabIndexGet()
        {
            string html = "<input id='control1' tabindex='1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(1, reader.TabIndex);
        }

        [TestMethod]
        public void MaxLengthGet()
        {
            string html = "<input id='control1' maxlength='1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(1, reader.MaxLength);
        }

        [TestMethod]
        public void SizeGet()
        {
            string html = "<input id='control1' size='5' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(5, reader.Size);
        }

        [TestMethod]
        public void ValueGet()
        {
            string html = "<input id='control1' value='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.Value);
        }

        [TestMethod]
        public void ValueSet()
        {
            string html = "<input id='control1' value='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            reader.Value = "bar";
            UnitTestAssert.AreEqual("bar", reader.Value);
        }

        [TestMethod]
        public void ReadOnlyGet()
        {
            string html = "<input id='control1' readonly='true' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.ReadOnly);
        }

        [TestMethod]
        public void DefaultValueGet()
        {
            string html = "<input id='control1' defaultvalue='foo' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual("foo", reader.DefaultValue);
        }

        [TestMethod]
        public void DefaultCheckedGet()
        {
            string html = "<input id='control1' defaultchecked='false' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(false, reader.DefaultChecked);
        }

        [TestMethod]
        public void TypeGet()
        {
            string html = "<input id='control1' type='password' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(HtmlInputElementType.Password, reader.Type);
        }

        [TestMethod]
        public void CheckedWillReturnTrueWhenAttributeHasNoValue()
        {
            string html = "<input id='control1' checked />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Checked);
        }

        [TestMethod]
        public void CheckedGet()
        {
            string html = "<input id='control1' checked='checked' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Checked);
        }

        [TestMethod]
        public void CheckedSet()
        {
            string html = "<input id='control1' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            reader.Checked = true;
            UnitTestAssert.AreEqual(true, reader.Checked);
        }

        [TestMethod]
        public void CheckedGetFalse()
        {
            string html = "<input id='control1' checked='false' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(false, reader.Checked);
        }

        [TestMethod]
        public void DisabledGet()
        {
            string html = "<input id='control1' disabled='disabled' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Disabled);
        }

        [TestMethod]
        public void DisabledGetFalse()
        {
            string html = "<input id='control1' disabled='false' />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(false, reader.Disabled);
        }

        [TestMethod]
        public void DisabledGetShouldReturnTrueWhenDisabledHasNoValue()
        {
            string html = "<input id='control1' disabled />";

            HtmlElement element = HtmlElement.Create(html);

            HtmlInputElementAttributeReader reader = new HtmlInputElementAttributeReader(element);
            UnitTestAssert.AreEqual(true, reader.Disabled);
        }

        
    }
}
