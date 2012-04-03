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
    public class BrowserCommandTargetTest
    {
        [TestMethod]
        public void TextBetweenTagsGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.TextBetweenTags);

            info.TextBetweenTags = "foo";
            Assert.AreEqual("foo", info.TextBetweenTags);
        }

        [TestMethod]
        public void WindowCaptionGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.WindowCaption);

            info.WindowCaption = "foo";
            Assert.AreEqual("foo", info.WindowCaption);
        }

        [TestMethod]
        public void WindowIndexGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.AreEqual(0, info.WindowIndex);

            info.WindowIndex = 1;
            Assert.AreEqual(1, info.WindowIndex);
        }

        [TestMethod]
        public void FrameHierarchyGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.FrameHierarchy);

            info.FrameHierarchy = new object[1] {"foo"};
            Assert.IsNotNull(info.FrameHierarchy);
        }

        [TestMethod]
        public void IdGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.Id);

            info.Id = "foo";
            Assert.AreEqual("foo", info.Id);
        }

        [TestMethod]
        public void TagNameGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.TagName);

            info.TagName = "foo";
            Assert.AreEqual("foo", info.TagName);
        }

        [TestMethod]
        public void IndexGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.AreEqual(0, info.Index);

            info.Index = 2;
            Assert.AreEqual(2, info.Index);
        }

        [TestMethod]
        public void ChildTagNameGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.IsNull(info.ChildTagName);

            info.ChildTagName = "foo";
            Assert.AreEqual("foo", info.ChildTagName);
        }

        [TestMethod]
        public void ChildTagIndexGetSet()
        {
            BrowserCommandTarget info = new BrowserCommandTarget();
            Assert.AreEqual(0, info.ChildTagIndex);

            info.ChildTagIndex = 2;
            Assert.AreEqual(2, info.ChildTagIndex);
        }
    }
}
