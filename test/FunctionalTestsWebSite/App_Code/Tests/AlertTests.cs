using System;
using LTAF;


namespace NexusLightTest
{
    [WebTestClass]
    public class AlertTest
    {
        [WebTestMethod]
        public void Alert()
        {
            HtmlPage page = new HtmlPage();
            page.Navigate("AlertPage.aspx");
            page.Elements.Find("AlertButton").Click(new CommandParameters(WaitFor.None, PopupAction.AlertOK));
            Assert.AreEqual("hi", page.Elements.Find("log").GetInnerText());
        }

        [WebTestMethod]
        public void GetTextFromAlert()
        {
            HtmlPage page = new HtmlPage();
            page.Navigate("AlertPage.aspx");
            CommandParameters parameters = new CommandParameters(WaitFor.None, PopupAction.AlertOK);
            page.Elements.Find("AlertButton").Click(parameters);
            Assert.AreEqual("hi", parameters.PopupText);
        }

        [WebTestMethod]
        public void ExpectAlertWhenNoActualAlert()
        {
            HtmlPage page = new HtmlPage();
            page.Navigate("AlertPage.aspx");
            var clickOptions = new CommandParameters(WaitFor.None, PopupAction.AlertOK);
            page.Elements.Find("ShowText").Click(clickOptions);
            Assert.IsNull(clickOptions.PopupText);
        }

        [WebTestMethod]
        public void ConfirmOK()
        {
            HtmlPage page = new HtmlPage();
            page.Navigate("AlertPage.aspx");
            var options = new CommandParameters(WaitFor.None, PopupAction.ConfirmOK);
            page.Elements.Find("ConfirmButton").Click(options);
            Assert.AreEqual("You said OK", page.Elements.Find("log").GetInnerText());
            Assert.AreEqual("are you sure?", options.PopupText);
        }

        [WebTestMethod]
        public void ConfirmCancel()
        {
            HtmlPage page = new HtmlPage();
            page.Navigate("AlertPage.aspx");
            var options = new CommandParameters(WaitFor.None, PopupAction.ConfirmCancel);
            page.Elements.Find("ConfirmButton").Click(options);
            Assert.AreEqual("You said Cancel", page.Elements.Find("log").GetInnerText());
            Assert.AreEqual("are you sure?", options.PopupText);
        }
    }
}