using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class InvalidNodeTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			InvalidNode target = new InvalidNode();
			Assert.AreEqual(NodeType.Invalid, target.Type);
		}
	}
}
