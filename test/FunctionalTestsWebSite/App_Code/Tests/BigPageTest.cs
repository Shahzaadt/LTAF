using System;
using System.Data;
using System.Configuration;
using LTAF;


[WebTestClass]
public class BigPageTest
{
    [WebTestMethod]
    public void BigPageTest1()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("BigPage.aspx");

        page.Elements.Find("UserName").SetText("foo");
        page.Elements.Find("Password").SetText("bar");
        page.Elements.Find("LoginButton").Click(WaitFor.Postback);

        Assert.AreEqual("Logout", page.Elements.Find("LoginStatus1").GetInnerText());
    }
}
