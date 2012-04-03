using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LTAF.TagParser;

namespace LTAF.UnitTests.TagParser
{
	[TestClass]
	public class NodeContainerTest
	{
		
		[TestMethod]
		public void AttributeGetShouldReturnNewAttributeCollectionWhenFirstCall()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.AreEqual(0, target.Attributes.Count);
		}

		[TestMethod]
		public void AttributeGetShouldReturnExistingAttributeCollectionWhenSecondCall()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.AreEqual(0, target.Attributes.Count);
			target.Attributes.Add(new LTAF.TagParser.Attribute("prefix", "localName", "value"));
			Assert.AreEqual(1, target.Attributes.Count);
		}

		[TestMethod()]
		public void HasAttributesGetShouldReturnFalseWhenNoCollection()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.IsFalse(target.HasAttributes);
		}

		[TestMethod()]
		public void HasAttributesGetShouldReturnFalseWhenCollectionHasNoElements()
		{
			NodeContainer target = CreateNodeContainer();
			AttributeCollection c = target.Attributes;
			Assert.IsFalse(target.HasAttributes);
		}

		[TestMethod()]
		public void HasAttributesGetShouldReturnTrueWhenCollectionHasElements()
		{
			NodeContainer target = CreateNodeContainer();
			target.Attributes.Add(new LTAF.TagParser.Attribute("prefix", "localName", "value"));
			Assert.IsTrue(target.HasAttributes);
		}

		[TestMethod]
		public void NodesGetShouldReturnNewNodesCollectionWhenFirstCall()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.AreEqual(0, target.Nodes.Count);
		}

		[TestMethod]
		public void NodesGetShouldReturnExistingNodesCollectionWhenSecondCall()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.AreEqual(0, target.Nodes.Count);
			target.Nodes.Add(new MockNode());
			Assert.AreEqual(1, target.Nodes.Count);
		}

		[TestMethod()]
		public void HasChildNodesGetShouldReturnFalseWhenNoCollection()
		{
			NodeContainer target = CreateNodeContainer();
			Assert.IsFalse(target.HasChildNodes);
		}

		[TestMethod()]
		public void HasChildNodesGetShouldReturnFalseWhenCollectionHasNoElements()
		{
			NodeContainer target = CreateNodeContainer();
			NodeCollection c = target.Nodes;
			Assert.IsFalse(target.HasAttributes);
		}

		[TestMethod()]
		public void HasChildNodesGetShouldReturnTrueWhenCollectionHasElements()
		{
			NodeContainer target = CreateNodeContainer();
			target.Nodes.Add(new MockNode());
			Assert.IsTrue(target.HasChildNodes);
		}

		internal virtual NodeContainer CreateNodeContainer()
		{
			NodeContainer target = new MockNodeContainer();
			return target;
		}
	}
}
