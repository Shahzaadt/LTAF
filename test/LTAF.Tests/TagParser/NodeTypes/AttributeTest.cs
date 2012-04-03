using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class AttributeTest
	{
		[TestMethod()]
		public void ValueGet()
		{
			Attribute target = new Attribute("prefix", "localName", "value");
			Assert.AreEqual("value", target.Value);
		}

		[TestMethod()]
		public void TypeGet()
		{
			Attribute target = new Attribute("prefix", "localName", "value");
			Assert.AreEqual(NodeType.Attribute, target.Type);
		}
	}
}
