using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class ElementCloseTagTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			ElementCloseTag target = new ElementCloseTag();
			Assert.AreEqual(NodeType.CloseTag, target.Type);
		}
	}
}
