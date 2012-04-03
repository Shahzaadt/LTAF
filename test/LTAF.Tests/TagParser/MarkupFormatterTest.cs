using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using LTAF.TagParser;

namespace LTAF.UnitTests.TagParser
{
    [TestClass]
    public class MarkupFormatterTest
    {
        [TestMethod]
        public void VerifyFormatElementWithLotsOfAttributes()
        {
            string originalText = @"<foo a='very big string1' b='very big string2' c='very big string3' d='very big string4'></foo>";
            MarkupFormatter formatter = new MarkupFormatter();
            string newText = formatter.Format(originalText);

            string expectedText = "<foo a='very big string1' b='very big string2' c='very big string3'\n\t d='very big string4'></foo>\n";            

            UnitTestAssert.AreEqual(expectedText, newText, newText);
        }

        [TestMethod]
        public void VerifyFormatElementWithLotsOfNestedElements()
        {
            string originalText = @"<a><b><c><d></d></c></b></a>";
            MarkupFormatter formatter = new MarkupFormatter();
            string newText = formatter.Format(originalText);

            string expectedText = @"<a>
	<b>
		<c>
			<d></d>
		</c>
	</b>
</a>
";

            UnitTestAssert.AreEqual(expectedText.Replace("\r", ""), newText, newText);
        }

        [TestMethod]
        public void VerifyFormatInvalidElement()
        {
            string originalText = @"<a><b><c></b></c></a>";
            MarkupFormatter formatter = new MarkupFormatter();
            string newText = formatter.Format(originalText);

            string expectedText = @"<a>
	<b>
		<c>
	</b>
	</c>
</a>
";

            UnitTestAssert.AreEqual(expectedText.Replace("\r", ""), newText, newText);
        }


        [TestMethod]
        public void VerifyFormatMultipleTopLevelElements()
        {
            string originalText = @"<a></a><b></b>";
            MarkupFormatter formatter = new MarkupFormatter();
            string newText = formatter.Format(originalText);

            string expectedText = @"<a></a>
<b></b>
";

            UnitTestAssert.AreEqual(expectedText.Replace("\r", ""), newText, newText);
        }

        [TestMethod]
        public void VerifyFormatElementWithInnerText()
        {
            string originalText = @"<a><b>Very Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long String</b></a>";
            MarkupFormatter formatter = new MarkupFormatter();
            string newText = formatter.Format(originalText);

            string expectedText = @"<a>
	<b>
		Very Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringVery Long StringTOKEN
	</b>
</a>
";
            expectedText = expectedText.Replace("\r", "").Replace("TOKEN" ,"\r");

            UnitTestAssert.AreEqual(expectedText, newText, newText);
        }
    }
}
