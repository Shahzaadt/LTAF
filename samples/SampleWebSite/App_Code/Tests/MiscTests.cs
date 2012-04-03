using System;
using LTAF;

[WebTestClass]
public class MiscTests
{
    [WebTestMethod]
    public void VerifyTheAspNetErrorPage()
    {
        HtmlPage p = new HtmlPage("PageWithError.aspx");
        bool pageFail = p.IsServerError();
        Assert.IsTrue(pageFail);
    }
}
