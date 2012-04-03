using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace LTAF.UnitTests.UI
{
    [TestClass]
    public class HtmlElementFindParamsTests
    {
        [TestMethod]
        public void ConvertObjectToDictionaryReturnsNullWhenPassedNull()
        {
            //Arrange & Act
            Dictionary<string, object> d = HtmlElementFindParams.ConvertObjectToDictionary(null);
            //Assert
            UnitTestAssert.IsNull(d);
        }

        [TestMethod]
        public void ConvertObjectToDictionaryReturnsEmptyDictionaryWhenPassedEmptyObject()
        {
            //Arrange & Act
            Dictionary<string, object> d = HtmlElementFindParams.ConvertObjectToDictionary(new { });
            //Assert
            UnitTestAssert.AreEqual(0, d.Count);
        }

        [TestMethod]
        public void ConvertObjectToDictionaryReturnsDictionaryWhenPassedAnObject()
        {
            //Arrange & Act
            Dictionary<string, object> d = HtmlElementFindParams.ConvertObjectToDictionary(new { Id = 3, Name = "Matthew", isAwesome = true });
            //Assert
            UnitTestAssert.AreEqual(3, d.Count);
            UnitTestAssert.AreEqual(3, d["Id"]);
            UnitTestAssert.AreEqual("Matthew", d["Name"]);
            UnitTestAssert.AreEqual(true, d["IsAwesome"]);
        }

        [TestMethod]
        public void CreateWithEmptyObjectHasNoParams()
        {
            //Arrange & Act
            HtmlElementFindParams fp = new HtmlElementFindParams(new { });
            //Assert
            ValidateBaseHtmlElementFindParams(fp, null, null, 0, 0, null, MatchMethod.EndsWith); 
        }

        [TestMethod]
        public void CreateWithBasicPropertiesHasProperParamsPopulated()
        {
            //Arrange & Act
            HtmlElementFindParams fp = new HtmlElementFindParams(new { id = "myID", idMatchMethod = MatchMethod.Literal, tagName = "Div", index = 7, innerText = "InnerText" });
            //Assert
            ValidateBaseHtmlElementFindParams(fp, "Div", "InnerText", 7, 0, "myID", MatchMethod.Literal);
        }

        [TestMethod]
        public void CreateWithCustomAttributesObjectProperParamsPopulated()
        {
            //Arrange & Act
            HtmlElementFindParams fp = new HtmlElementFindParams(new { @class="myClass", @classMatchMethod = MatchMethod.Contains, attribute1="value", attribute1MatchMethod = MatchMethod.Regex, attribute2 = "value2" });
            //Assert
            ValidateBaseHtmlElementFindParams(fp, null, null, 0, 3, null, MatchMethod.EndsWith);
            //  Class Attribute
            UnitTestAssert.AreEqual("class", fp.Attributes[0].Name);
            UnitTestAssert.AreEqual("myClass", fp.Attributes[0].Value);
            UnitTestAssert.AreEqual(MatchMethod.Contains, fp.Attributes[0].ValueMatchMethod);
            //  Attribute 1
            UnitTestAssert.AreEqual("attribute1", fp.Attributes[1].Name);
            UnitTestAssert.AreEqual("value", fp.Attributes[1].Value);
            UnitTestAssert.AreEqual(MatchMethod.Regex, fp.Attributes[1].ValueMatchMethod);
            //  Attribute 2
            UnitTestAssert.AreEqual("attribute2", fp.Attributes[2].Name);
            UnitTestAssert.AreEqual("value2", fp.Attributes[2].Value);
            UnitTestAssert.AreEqual(MatchMethod.Literal, fp.Attributes[2].ValueMatchMethod);

        }

        private void ValidateBaseHtmlElementFindParams(HtmlElementFindParams fp, string tagName, string innerText, int index, int attributeCount, string id, MatchMethod idMatchMethod)
        {
            //Base Properties
            UnitTestAssert.AreEqual(tagName, fp.TagName);
            UnitTestAssert.AreEqual(innerText, fp.InnerText);
            UnitTestAssert.AreEqual(index, fp.Index);
            UnitTestAssert.AreEqual(attributeCount, fp.Attributes.Count);

            //ID Properties
            UnitTestAssert.AreEqual(id, fp.IdAttributeFindParams.Value);
            UnitTestAssert.AreEqual(idMatchMethod, fp.IdAttributeFindParams.ValueMatchMethod);
        }
    }
}
