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
public class RerunFailedTestsTests
{
	[WebTestMethod]
	public void RerunFailedTestsWithNoFailedTest()
	{
		HtmlPage page = new HtmlPage("RerunFailedPages/NoFailedTests.aspx");
		CommandParameters cp = new CommandParameters(WaitFor.None, PopupAction.AlertOK);
		page.Elements.Find("btnRunTests").Click(cp);
		Assert.IsFalse(string.IsNullOrEmpty(cp.PopupText),"There was no popup.");
		Assert.AreEqual("Good News, there are no Failed Tests!", cp.PopupText,"Popup message was not correct.");
		Assert.AreEqual("0", page.Elements.Find("numFailedTests").GetInnerHtml());
	}
	
	[WebTestMethod]
	public void RerunFailedTestsWithTrincatedUrlTest()
	{
		HtmlPage page = new HtmlPage("RerunFailedPages/TruncatedUrl.aspx");
		CommandParameters cp = new CommandParameters(WaitFor.None, PopupAction.ConfirmCancel);
		page.Elements.Find("btnRunTests").Click(cp);
		string expectedPopupText = "I'm sorry, there are too many failed tests, some of the tests will not be run. Would you still like to continue?";
		Assert.StringIsNotNullOrEmpty(cp.PopupText, "There was no popup.");
		Assert.AreEqual(expectedPopupText, cp.PopupText, "Popup message was not correct.");
		Assert.AreEqual("true", page.Elements.Find("urlIsTruncated").GetInnerHtml());
	}

	[WebTestMethod]
	public void GetFailedTestsUrlTest()
	{
		HtmlPage page = new HtmlPage("RerunFailedPages/MultipleFailedTests.aspx");
		page.Elements.Find("btnRunTests").Click();
		string expectedUrl = "?tag=Test1@Test2@Test3&amp;Filter=true&amp;Run=true";
		Assert.StringContains(page.Elements.Find("TestUrl").GetInnerHtml(),expectedUrl);
		Assert.AreEqual("3", page.Elements.Find("numFailedTests").GetInnerHtml());
	}
}
