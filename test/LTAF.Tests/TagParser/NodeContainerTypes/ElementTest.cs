using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTAF.TagParser;

namespace LTAF.UnitTests.TagParser
{
	[TestClass]
	public class ElementTest
	{
		[TestMethod]
		public void TypeGet()
		{
			Element target = new Element();
			Assert.AreEqual(NodeType.Element, target.Type);
		}

		[TestMethod]
		public void SetElementPosition()
		{
			Element target = new Element();
			target.SetElementPosition(1, 2, 3, 4);
			Assert.AreEqual(1, target.StartPosition);
			Assert.AreEqual(2, target.EndPositionOpeningTag);
			Assert.AreEqual(3, target.StartPositionClosingTag);
			Assert.AreEqual(4, target.EndPosition);
		}

		[TestMethod]
		public void IsClosedShouldReturnFalseWhenSetIsClosedWithParamFalse()
		{
			Element target = new Element();
			target.SetIsClosed(false);
			Assert.IsFalse(target.IsClosed);
		}

		[TestMethod]
		public void IsClosedShouldReturnTrueDefault()
		{
			Element target = new Element();
			Assert.IsTrue(target.IsClosed);
		}

		[TestMethod]
		public void IsInlineClosedReturnTrueWhenSetIsInlineClosedWithParamTrue()
		{
			Element target = new Element();
			target.SetIsInlineClosed(true);
			Assert.IsTrue(target.IsInlineClosed);
		}

		[TestMethod]
		public void IsInlineClosedShouldReturnFalseDefault()
		{
			Element target = new Element();
			Assert.IsFalse(target.IsInlineClosed);
		}

		[TestMethod]
		public void GetCloseTagShouldReturnEmptyStringWhenIsInlineClosedTrue()
		{
			Element target = new Element();
			target.SetIsInlineClosed(true);
			Assert.AreEqual(string.Empty, target.GetCloseTag());
		}

		[TestMethod]
		public void GetCloseTagShouldReturnEmptyStringWhenIsClosedFalse()
		{
			Element target = new Element();
			target.SetIsClosed(false);
			Assert.AreEqual(string.Empty, target.GetCloseTag());
		}

		[TestMethod]
		public void GetCloseTagShouldReturnEmptyStringWhenStartPositionClosingTagIsAfterEndPosition()
		{
			Element target = new Element();
			target.SetElementPosition(0, 2, 4, 3);
			Assert.AreEqual(string.Empty, target.GetCloseTag());
		}

		[TestMethod]
		public void GetCloseTagShouldReturnString()
		{
			Element target = new Element();
			target.SetParent(new TagDocument("<span>Test</span>"));
			target.SetElementPosition(0, 5, 10, 16);
			Assert.AreEqual("</span>", target.GetCloseTag());
		}

		[TestMethod]
		public void GetOpenTagShouldReturnStringWhenIsInlineClosedTrue()
		{
			Element target = new Element();
			target.SetParent(new TagDocument("<span/>"));
			target.SetIsInlineClosed(true);
			target.SetElementPosition(0, 6, -1, 6);
			Assert.AreEqual("<span/>", target.GetOpenTag());
		}

		[TestMethod]
		public void GetOpenTagShouldReturnEmptyStringWhenStartPositionIsAfterEndPositionOpenTag()
		{
			Element target = new Element();
			target.SetElementPosition(1, 0, 2, 3);
			Assert.AreEqual(string.Empty, target.GetOpenTag());
		}

		[TestMethod]
		public void GetOpenTagShouldReturnString()
		{
			Element target = new Element();
			target.SetParent(new TagDocument("<span>Test</span>"));
			target.SetElementPosition(0, 5, 10, 16);
			Assert.AreEqual("<span>", target.GetOpenTag());
		}
	}
}
