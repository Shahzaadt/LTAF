using LTAF.TagParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTAF.UnitTests.TagParser
{   
    
    /// <summary>
    ///This is a test class for AttributeCollectionTest and is intended
    ///to contain all AttributeCollectionTest Unit Tests
    ///</summary>
	[TestClass()]
	public class AttributeCollectionTest
	{
		[TestMethod()]
		public void ParentGet()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			Assert.AreEqual(parent, target.Parent);
		}

		[TestMethod]
		public void GetByIndexShouldReturnElement()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsNotNull(target[0]);
			Assert.AreEqual("Test", target[0].LocalName);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void GetByIndexShouldThrowError()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			Attribute result = target[1];
		}

		[TestMethod()]
		public void IndexOfShoulReturnNegativeOneWhenCaseSensative()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.AreEqual(-1, target.IndexOf("test", false));
		}

		[TestMethod()]
		public void IndexOfShoulReturnZerohenNonCaseSensative()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.AreEqual(0, target.IndexOf("test", true));
		}

		[TestMethod()]
		public void GetAttributeShouldReturnStringCaseSensativeParamWrongCase()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsNotNull(target.GetAttribute("test"));
		}

		[TestMethod()]
		public void GetAttributeShouldReturnStringCaseSensativeParamRightCase()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsNotNull(target.GetAttribute("Test"));
		}

		[TestMethod()]
		public void GetAttributeShouldReturnNullParamAttributeDoesNotExist()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsNull(target.GetAttribute("notThere"));
		}

		[TestMethod()]
		public void GetAttributeShouldReturnNullWhenNoAttributes()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			Assert.IsNull(target.GetAttribute("Test"));
		}

		[TestMethod()]
		public void ContainsShouldReturnFalseCaseSensativeParamWrongCase()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsFalse(target.Contains("test",false));
		}

		[TestMethod()]
		public void ContainsShouldReturnTrueCaseSensativeParamRightCase()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsTrue(target.Contains("Test", false));
		}

		[TestMethod()]
		public void ContainsShouldReturnTrueNonCaseSensative()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			target.Add(new Attribute(null, "Test", "Value"));
			Assert.IsTrue(target.Contains("test", true));
		}

		[TestMethod()]
		public void ContainsShouldReturnFalse()
		{
			Element parent = new Element();
			parent.SetName("prefix", "localname");
			AttributeCollection target = new AttributeCollection(parent);
			Assert.IsFalse(target.Contains("test", true));
		}
	}
}
