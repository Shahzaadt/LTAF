using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{ 
	[TestClass()]
	public class NodeTest
	{
		[TestMethod()]
		public void TagSetGet()
		{
			Node target = CreateNode();
			target.Tag = "Test";
			Assert.AreEqual("Test", target.Tag);
		}

		[TestMethod()]
		public void StartPositionGet()
		{
			Node target = CreateNode();
			target.SetPosition(0, 1);
			Assert.AreEqual(0, target.StartPosition);
		}

		[TestMethod()]
		public void EndPositionGet()
		{
			Node target = CreateNode();
			target.SetPosition(0, 1);
			Assert.AreEqual(1, target.EndPosition);
		}

		[TestMethod()]
		public void SetNameGetPrefixLocalName()
		{
			Node target = CreateNode();
			target.SetName("prefix", "localName");
			Assert.AreEqual("prefix", target.Prefix);
			Assert.AreEqual("localName", target.LocalName);
		}

		[TestMethod()]
		public void ParentSetGet()
		{
			Node target = CreateNode();
			Element parent = new Element();
			target.SetParent(parent);
			Assert.AreEqual(parent, target.Parent);
		}

		[TestMethod()]
		public void SetNameWithPrefix()
		{
			Node target = CreateNode();
			target.SetName("prefix", "localName");
			Assert.AreEqual("prefix", target.Prefix);
			Assert.AreEqual("localName", target.LocalName);
		}

		[TestMethod()]
		public void SetNameWithNoPrefix()
		{
			Node target = CreateNode();
			target.SetName(null, "localName");
			Assert.IsNull(target.Prefix);
			Assert.AreEqual("localName", target.LocalName);
		}

		[TestMethod()]
		public void NameGet()
		{
			Node target = CreateNode();
			target.SetName("prefix", "localName");
			Assert.AreEqual("prefix:localName", target.Name);
		}

		[TestMethod()]
		public void SetParent()
		{
			Node target = CreateNode();
			Assert.IsNull(target.Document);
			TagDocument doc = new TagDocument("This is a test");
			target.SetParent(doc);
			Assert.AreEqual(doc, target.Parent);
		}

		[TestMethod()]
		public void DocumentGet()
		{
			Node target = CreateNode();
			Assert.IsNull(target.Document);
			TagDocument doc = new TagDocument("This is a test");
			target.SetParent(doc);
			Assert.AreEqual("This is a test", target.Document.Text);
		}

		[TestMethod()]
		public void GlobalIndexGetSet()
		{
			Node target = CreateNode();
			target._globalIndex = 5;
			Assert.AreEqual(5, target.GlobalIndex);
		}

		[TestMethod()]
		public void GetTexShouldReturnEmptyStringtWhenNoPositionSet()
		{
			Node target = CreateNode();
			Assert.AreEqual(string.Empty, target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnEmptyStringWhenNoParentSet()
		{
			Node target = CreateNode();
			target.SetPosition(1, 5);
			Assert.AreEqual(string.Empty, target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnStringWtihValidParent()
		{
			Node target = CreateNode();
			target.SetPosition(1, 5);
			target.SetParent(new TagDocument("0123456789"));
			Assert.AreEqual("12345", target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnEntireDocumentWhenEndPositionToFar()
		{
			Node target = CreateNode();
			target.SetParent(new TagDocument("0123456789"));
			target.SetPosition(0, 10);
			Assert.AreEqual("0123456789", target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnEmptyStringWhenEndPositionToShort()
		{
			Node target = CreateNode();
			target.SetParent(new TagDocument("0123456789"));
			target.SetPosition(0, -1);
			Assert.AreEqual(string.Empty, target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnEmptyStringWhenStartPositionToFar()
		{
			Node target = CreateNode();
			target.SetParent(new TagDocument("0123456789"));
			target.SetPosition(10, 11);
			Assert.AreEqual(string.Empty, target.GetText());
		}

		[TestMethod()]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void GetTextShouldThrowExceptionWhenStartPostionToShort()
		{
			Node target = CreateNode();
			target.SetParent(new TagDocument("0123456789"));
			target.SetPosition(-1, 11);
			string value = target.GetText();
		}

		[TestMethod()]
		public void GetTextShouldReturnEmptyStringWhenDocumentDoesNotHaveText()
		{
			Node target = CreateNode();
			target.SetPosition(1, 5);
			target.SetParent(new TagDocument(string.Empty));
			Assert.AreEqual(string.Empty, target.GetText());
		}

		[TestMethod()]
		public void GetTextShouldReturnEmptyStringWhenEndPositionToShortWithStringParam()
		{
			Node target = CreateNode();
			target.SetPosition(1, 0);
			Assert.AreEqual(string.Empty, target.GetText("Foo"));
		}

		[TestMethod()]
		public void GetTextShouldReturnStringWhenPositionSetWithStringParam()
		{
			Node target = CreateNode();
			target.SetPosition(0, 9);
			Assert.AreEqual("0123456789", target.GetText("0123456789"));
		}
		
		[TestMethod()]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void GetTextShouldThrowExceptionWhenEndPostionToLongWithStringParam()
		{
			Node target = CreateNode();
			target.SetPosition(0, 20);
			Assert.AreEqual("0123456789", target.GetText("0123456789"));
		}

		internal virtual Node CreateNode()
		{
			Node target = new MockNode();
			return target;
		}

	}
}
