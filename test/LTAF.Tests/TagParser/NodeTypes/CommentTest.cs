using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class CommentTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			Comment target = new Comment();
			Assert.AreEqual(NodeType.Comment, target.Type);
		}

	}
}
