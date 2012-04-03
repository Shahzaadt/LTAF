using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
    
	[TestClass()]
	public class ProcessingInstructionTest
	{
		[TestMethod]
		public void TypeGet()
		{
			ProcessingInstruction target = new ProcessingInstruction();
			Assert.AreEqual(NodeType.ProcessingInstruction, target.Type);
		}
	}
}
