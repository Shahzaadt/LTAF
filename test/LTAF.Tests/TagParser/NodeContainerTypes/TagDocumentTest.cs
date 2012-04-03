using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class TagDocumentTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			TagDocument target = new TagDocument(string.Empty);
			Assert.AreEqual(NodeType.Document, target.Type);
		}

		[TestMethod()]
		public void TextGet()
		{
			TagDocument target = new TagDocument("This is a test");
			Assert.AreEqual("This is a test", target.Text);
		}

		[TestMethod()]
		public void NodeCountGet()
		{
			TagDocument target = new TagDocument("This is a test");
			target._nodeCount = 5;
			Assert.AreEqual(5, target.NodeCount);
		}

		[TestMethod()]
		public void DocumentGet()
		{
			TagDocument target = new TagDocument("This is a test");
			Assert.AreEqual(target, target.Document);
		}
	}
}
