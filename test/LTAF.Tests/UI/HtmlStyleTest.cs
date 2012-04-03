using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF;
using System.Drawing;
using System.Web.UI.WebControls;


namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlStyleTest
    {
        private HtmlStyle _style;

        [TestInitialize]
        public void TestInitialize()
        {
            _style = new HtmlStyle();

            StringBuilder descriptors = new StringBuilder();
            descriptors.Append("visibility:hidden;position:fixed;color:green;backgroundcolor:blue;backgroundimage:fooimage;");
            descriptors.Append("bottom:1px;left:2px;right:3px;top:4px;borderspacing:5px;height:6px;width:7px;");
            descriptors.Append("paddingtop:8px;paddingleft:9px;paddingright:10px;paddingbottom:11px;size:12px;");
            descriptors.Append("textindent:13px;align:justify;verticalalign:middle;whitespace:nowrap;display:inline;overflow:scroll;");
            _style.LoadDescriptors(descriptors.ToString());
        }

        [TestMethod]
        public void OverflowGet()
        {
            UnitTestAssert.AreEqual(Overflow.Scroll, _style.Overflow);
        }

        [TestMethod]
        public void DisplayGet()
        {
            UnitTestAssert.AreEqual(Display.Inline, _style.Display);
        }

        [TestMethod]
        public void WhiteSpaceGet()
        {
            UnitTestAssert.AreEqual(WhiteSpace.NoWrap, _style.WhiteSpace);
        }

        [TestMethod]
        public void VerticalAlignGet()
        {
            UnitTestAssert.AreEqual(VerticalAlign.Middle, _style.VerticalAlign);
        }

        [TestMethod]
        public void HorizontalAlignGet()
        {
            UnitTestAssert.AreEqual(HorizontalAlign.Justify, _style.Align);
        }

        [TestMethod]
        public void TextIndentGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(13), _style.TextIndent);
        }

        [TestMethod]
        public void SizeGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(12), _style.Size);
        }

        [TestMethod]
        public void PaddingBottomGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(11), _style.PaddingBottom);
        }

        [TestMethod]
        public void PaddingRightGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(10), _style.PaddingRight);
        }

        [TestMethod]
        public void PaddingLeftGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(9), _style.PaddingLeft);
        }

        [TestMethod]
        public void PaddingTopGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(8), _style.PaddingTop);
        }

        [TestMethod]
        public void WidthGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(7), _style.Width);
        }

        [TestMethod]
        public void HeightGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(6), _style.Height);
        }

        [TestMethod]
        public void BorderSpacingGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(5), _style.BorderSpacing);
        }

        [TestMethod]
        public void TopGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(4), _style.Top);
        }

        [TestMethod]
        public void RightGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(3), _style.Right);
        }

        [TestMethod]
        public void LeftGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(2), _style.Left);
        }

        [TestMethod]
        public void BottomGet()
        {
            UnitTestAssert.AreEqual(Unit.Pixel(1), _style.Bottom);
        }


        [TestMethod]
        public void BackgroundImageGet()
        {
            UnitTestAssert.AreEqual("fooimage", _style.BackgroundImage);
        }


        [TestMethod]
        public void BackgroundColorGet()
        {
            UnitTestAssert.AreEqual(Color.Blue, _style.BackgroundColor);
        }


        [TestMethod]
        public void ColorGet()
        {
            UnitTestAssert.AreEqual(Color.Green, _style.Color);
        }

        [TestMethod]
        public void PositionGet()
        {
            UnitTestAssert.AreEqual(Position.Fixed, _style.Position);
        }

        [TestMethod]
        public void VisibilityGet()
        {
            UnitTestAssert.AreEqual(Visibility.Hidden, _style.Visibility);
        }

        [TestMethod]
        public void RawStyleGet_EmptyStyle()
        {
            HtmlStyle style = new HtmlStyle();
            UnitTestAssert.IsNull(style.RawStyle);
        }

        [TestMethod]
        public void RawStyleGet()
        {
            HtmlStyle style = new HtmlStyle();
            style.LoadDescriptors("foo:foo1;bar:bar1;");
            UnitTestAssert.AreEqual("foo:foo1;bar:bar1;", style.RawStyle);
        }
    }
}
