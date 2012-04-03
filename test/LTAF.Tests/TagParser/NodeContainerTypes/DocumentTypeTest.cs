using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{   
	[TestClass()]
	public class DocumentTypeTest
	{
		[TestMethod()]
		public void TypeGet()
		{
			DocumentType target = new DocumentType();
			Assert.AreEqual(NodeType.ProcessingInstruction, target.Type);
		}
	}
}
