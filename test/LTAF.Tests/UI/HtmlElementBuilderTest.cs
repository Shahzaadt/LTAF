using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using System.Text.RegularExpressions;

namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlElementBuilderTest
    {
        private HtmlElementBuilder _builder;

        [TestInitialize]
        public void SetUp()
        {
            ServiceLocator.ApplicationPathFinder = new ApplicationPathFinder("http://test");
            _builder = new HtmlElementBuilder(null);
        }


        [TestMethod]
        public void CreateElementOnlyOneTagWithInnerText()
        {
            string html = @"
                <html>
                    foo
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);

            Assert.AreEqual("html", element.TagName);
            Assert.AreEqual(0, element.ChildElements.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void CreateElementThrowsIfMoreThanOneRootTags()
        {
            // 2 root tags is not valid
            string html = @"
                <foo>
                </foo>
                <html>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
        }

        [TestMethod]
        public void CreateElementPopulatesAttributesDictionary()
        {
            string html = @"
                <html a=a b=b c=c d=d>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(4, element.GetAttributes().Dictionary.Count);
        }

        [TestMethod]
        public void CreateElementWithMultipleChildTagsWithDifferentSpacing()
        {
            string html = @"
                <html   >
                    <tag1    />
                    <tag2   ></tag2>
                    <tag3 />
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(3, element.ChildElements.Count);
            foreach (HtmlElement child in element.ChildElements)
            {
                Assert.AreEqual(0, child.ChildElements.Count);
                Assert.AreEqual(0, child.GetAttributes().Dictionary.Count);
            }
        }

        [TestMethod]
        public void CreateElementDeepTagHierarchy()
        {
            string html = @"
                <html   >
                    <tag1>
                        <tag2   >
                           <tag3   >
                                <tag4 />
                            </tag3>
                        </tag2>
                    </tag1>
                    foo
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(1, element.ChildElements.Count);
            Assert.AreEqual(1, element.ChildElements[0].ChildElements[0].ChildElements.Count);
        }

        [TestMethod]
        public void CreateElementDoesNotTreatOperatorsAsTags()
        {
            string html = @"
                <html   >
                    <script>
                        for(i=0; i<5;i++);
                    </script>
                    <script>
                        if(c>t || c > t || c> t || c >t || c<>t || c< >t);
                    </script>
                    <script>
                        <!-- <foo></bar> -->
                    </script>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(3, element.ChildElements.Count);
            Assert.AreEqual("for(i=0; i<5;i++);", element.ChildElements[0].CachedInnerText.Trim());
            Assert.AreEqual("if(c>t || c > t || c> t || c >t || c<>t || c< >t);", element.ChildElements[1].CachedInnerText.Trim());
            Assert.AreEqual("<!-- <foo></bar> -->", element.ChildElements[2].CachedInnerText.Trim());
        }

        [TestMethod]
        public void CreateElementSelftClosingTags()
        {
            string html = @"
                <html>
                    <foo>
                        <bar />                    
                    </foo>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);

            Assert.IsNull(element.ParentElement);
            Assert.AreEqual(element, element.ChildElements[0].ParentElement);
            Assert.AreEqual(element.ChildElements[0], element.ChildElements[0].ChildElements[0].ParentElement);
        }

        [TestMethod]
        public void CreateElementWithComments()
        {
            string html = @"
                <html   >
                    <div>
                        for(i=0; i<5;i++);
                    </div>
                    <div>
                        if(c>t || c > t || c> t || c >t || c<>t || c< >t);
                    </div>
                    <div>
                        <!-- <foo></bar> -->
                    </div>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(3, element.ChildElements.Count);
            Assert.AreEqual("for(i=0; i<5;i++);", element.ChildElements[0].CachedInnerText.Trim());
            Assert.AreEqual("if(c>t || c > t || c> t || c >t || c<>t || c< >t);", element.ChildElements[1].CachedInnerText.Trim());
            Assert.AreEqual("<!-- <foo></bar> -->", element.ChildElements[2].CachedInnerText.Trim());
        }

        [TestMethod]
        public void CreateElementScriptIsConsideredAsLiteral()
        {
            string html = @"
                <html   >
                    <script>
                        <foo>bar</foo>
                    </script>
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(0, element.ChildElements[0].ChildElements.Count);
            Assert.AreEqual("<foo>bar</foo>", element.ChildElements[0].CachedInnerText.Trim());
        }

        [TestMethod]
        public void CreateElementTagsWithDifferentCasing()
        {
            string html = @"
                <html>
                    <FOO>
                        hola
                    </foo>
                </HTML>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual("hola", element.ChildElements[0].CachedInnerText.Trim());
        }

        [TestMethod]
        public void CreateElementAttributesWithNoQuotes()
        {
            string html = @"
                <html>
                    <FOO a=a c=c>
                        <bar b=b />
                    </foo>
                </HTML>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual("a", element.ChildElements[0].GetAttributes().Dictionary["a"]);
            Assert.AreEqual("c", element.ChildElements[0].GetAttributes().Dictionary["c"]);
            Assert.AreEqual("b", element.ChildElements[0].ChildElements[0].GetAttributes().Dictionary["b"]);
        }

        [TestMethod]
        public void CreateElementCorrectsMalFormedHtml()
        {
            string html = @"
                <html>
                    <bar />
                </foo>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(1, element.ChildElements.Count);
            Assert.AreEqual("bar", element.ChildElements[0].TagName);
        }

        [TestMethod]
        [ExpectedException(typeof(WebTestException))]
        public void CreateElementThrowsIfEnsureValidMarkup()
        {
            string html = @"
                <html>
                    <bar />
                </foo>
                ";

            _builder.EnsureValidMarkup = true;
            HtmlElement element = _builder.CreateElement(html);
        }


        [TestMethod]
        public void CreateElementUpperCaseID()
        {
            string html = @"
                <html>
                    <bar ID='bar1' />
                </html>
                ";

            _builder.EnsureValidMarkup = true;
            HtmlElement element = _builder.CreateElement(html);
            Assert.IsNull(element.Id);
            Assert.AreEqual("bar1", element.ChildElements[0].Id);
        }

        [TestMethod]
        public void CreateElementStronglyTypedElements()
        {
            string html = @"
                <html>
                    <input ID='control1' />
                    <select ID='control2' />
                    <a ID='control3' />
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.IsTrue(element.ChildElements[0] is HtmlInputElement);
            Assert.IsTrue(element.ChildElements[1] is HtmlSelectElement);
            Assert.IsTrue(element.ChildElements[2] is HtmlAnchorElement);

        }

        [TestMethod]
        public void CreateElementCheckTagNameIndex()
        {
            string html = @"
                <html>
                    <input ID='control1' />
                    <input ID='control2' />
                    <input Id='control3' />
                </html>
                ";

            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual(0, element.ChildElements[0].TagNameIndex);
            Assert.AreEqual(1, element.ChildElements[1].TagNameIndex);
            Assert.AreEqual(2, element.ChildElements[2].TagNameIndex);
        }

        [TestMethod]
        public void IgnoreDuplicateAttributesWhenBuildingDictionary()
        {
            string html = @"<a foo='blah' foo='bar' />";
            HtmlElement element = _builder.CreateElement(html);
            Assert.AreEqual("blah", element.AttributeDictionary["foo"]);
        }

        [TestMethod]
        public void VerifyFormElementIsCreated()
        {
            string html = @"<html><form id='form1' /></html>";
            HtmlElement element = _builder.CreateElement(html);
            Assert.IsInstanceOfType(element.ChildElements.Find("form1"), typeof(HtmlFormElement));
            Assert.AreEqual("form", element.ChildElements.Find("form1").TagName);

        }

        [TestMethod]
        public void WhenGenerateHtmlElement_ShouldStoreStartAndEndIndeces()
        {
            // Arrange
            string html = @"
                <html>
                    <FOO a=a c=c>
                        <bar b=b />
                    </foo>
                </HTML>
                ";

            // Act
            HtmlElement element = _builder.CreateElement(html);

            // Assert
            Assert.AreEqual(46, element.ChildElements[0].StartPosition);
            Assert.AreEqual(123, element.ChildElements[0].EndPosition);
        }
    }
}
