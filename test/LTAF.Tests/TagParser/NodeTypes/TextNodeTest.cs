using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class TextNodeTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			TextNode target = new TextNode();
			Assert.AreEqual(NodeType.Text, target.Type);
		}
	}
}
