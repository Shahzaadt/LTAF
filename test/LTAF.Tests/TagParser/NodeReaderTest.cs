using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{
	[TestClass()]
	public class NodeReaderTest
	{
		[TestMethod()]
		public void ReadNextShouldReturnTextNode()
		{
			NodeReader reader = new NodeReader("Just Text", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.Text, n.Type);
			Assert.AreEqual(9, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnClosingTagNode()
		{
			NodeReader reader = new NodeReader("</Test>", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.CloseTag, n.Type);
			Assert.AreEqual("Test", n.LocalName);
			Assert.AreEqual(null, n.Prefix);
			Assert.AreEqual(7, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnClosingTagNodeWithPrefix()
		{
			NodeReader reader = new NodeReader("</asp:Test>", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.CloseTag, n.Type);
			Assert.AreEqual("Test", n.LocalName);
			Assert.AreEqual("asp", n.Prefix);
			Assert.AreEqual(11, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnTextNodeWhenClosingTagNotFormatedProperly()
		{
			NodeReader reader = new NodeReader("</asp:Test", 0);
			Node n = reader.ReadNext();
			//Becuase of it lacks '>' it is turn into a text node.
			Assert.AreEqual(NodeType.Text, n.Type);
			Assert.AreEqual(10, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnTextNodeWhenClosingTagNotFormatedProperlyWithTrailingSpace()
		{
			NodeReader reader = new NodeReader("</asp:Test   ", 0);
			Node n = reader.ReadNext();
			//Becuase of it lacks '>' it is turn into a text node.
			Assert.AreEqual(NodeType.Text, n.Type);
			Assert.AreEqual(13, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnClosingTagNodeWhenSpaceIncluded()
		{
			NodeReader reader = new NodeReader("</Testing   >", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.CloseTag, n.Type);
			Assert.AreEqual("Testing", n.LocalName);
			Assert.AreEqual(13, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnClosingTagNodeWhenTabIncluded()
		{
			NodeReader reader = new NodeReader("</Testing\t>", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.CloseTag, n.Type);
			Assert.AreEqual("Testing", n.LocalName);
			Assert.AreEqual(11, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnCommentNode()
		{
			NodeReader reader = new NodeReader("<!-- This is a Test -->", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.Comment, n.Type);
			Assert.AreEqual(23, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnTextNodeWhenCommentNotFormatedProperly()
		{
			NodeReader reader = new NodeReader("<!- This is a Test -->", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.Text, n.Type);
			Assert.AreEqual(22, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnProcessingInstructionNodeWithDocType()
		{
			NodeReader reader = new NodeReader("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN/\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.ProcessingInstruction, n.Type);
			Assert.AreEqual("DOCTYPE", n.Name);
		}

		[TestMethod()]
		public void ReadNextShouldReturnProcessingInstructionNode()
		{
			NodeReader reader = new NodeReader("<?xml version=\"1.0\" ?>", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.ProcessingInstruction, n.Type);
			Assert.AreEqual("xml", n.Name);
			Assert.AreEqual(22, reader.Pos);
		}

		[TestMethod()]
		public void ReadNextShouldReturnTextNodeWhenProcessingInstructionNotFormatedProperly()
		{
			NodeReader reader = new NodeReader("<?xml version=\"1.0\" ?", 0);
			Node n = reader.ReadNext();
			Assert.AreEqual(NodeType.Text, n.Type);
			Assert.AreEqual(21, reader.Pos);
		}
	}
}
