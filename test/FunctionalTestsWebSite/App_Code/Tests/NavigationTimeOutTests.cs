using LTAF;

[WebTestClass]
public class NavigationTimeOutTests
{
	[WebTestMethod]
	public void NavigationNotCompleteCausesNoTimeOutTest()
	{
		HtmlPage page = new HtmlPage("NavigationTimeOut.aspx");
		page.Elements.Find("btnSetupMockUI").Click();
		page.Elements.Find("btnRunTest").Click();
		Assert.IsFalse(bool.Parse(page.Elements.Find("navComplete").GetInnerText()));
		Assert.StringIsNullOrEmpty(page.Elements.Find("errorMessage").GetInnerText());
	}

	[WebTestMethod]
	public void NavigationNotCompleteCausesTimeOutTest()
	{
		HtmlPage page = new HtmlPage("NavigationTimeOut.aspx");
		page.Elements.Find("btnSetupMockUI").Click();
		page.Elements.Find("txtWaitTime").SetText("100000");
		page.Elements.Find("btnRunTest").Click();
		Assert.IsTrue(bool.Parse(page.Elements.Find("navComplete").GetInnerText()));
		Assert.AreEqual("\"[AjaxBrowser] Navigation was not detected after 30 seconds.\"", page.Elements.Find("errorMessage").GetInnerText());
	}

	[WebTestMethod]
	public void NavigationComplete()
	{
		HtmlPage page = new HtmlPage("NavigationTimeOut.aspx");
		page.Elements.Find("btnRunTest").Click();
		Assert.IsTrue(bool.Parse(page.Elements.Find("navComplete").GetInnerText()));
        Assert.StringIsNullOrEmpty(page.Elements.Find("errorMessage").GetInnerText());
	}
}
