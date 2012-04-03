using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using LTAF;

[WebTestClass]
public class UpdateUIForPassFailTests
{
	[WebTestMethod]
	public void GetFailedTestsUrlTest()
	{
		HtmlPage page = new HtmlPage("UpdatePassFail.aspx");
		page.Elements.Find("btnRunTests").Click();
		Assert.AreEqual("testPassed", page.Elements.Find("Test1").GetAttributes().Class);
		Assert.AreEqual("testFailed", page.Elements.Find("Test2").GetAttributes().Class);
		Assert.AreEqual("testFailed", page.Elements.Find("Test3").GetAttributes().Class);
		Assert.AreEqual("testPassed", page.Elements.Find("Test4").GetAttributes().Class);
	}
}
