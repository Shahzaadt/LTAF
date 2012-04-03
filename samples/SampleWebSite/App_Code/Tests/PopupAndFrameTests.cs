using System;
using LTAF;

[WebTestClass]
public class PopupAndFrameTests
{
    [WebTestMethod]
    public void PopupPageWithIndex()
    {
        // This test also shows the use of the WebTestConsole to add traces to the driver page

        // Navigate to the frameset
        HtmlPage page = new HtmlPage("TestFrameSet.htm");

        //get the top frame
        WebTestConsole.Write("Get the top frame");
        HtmlPage frame = page.GetFramePage("topFrame");

        // click on the button that opens a new page
        WebTestConsole.Write("Click the openwindow button");
        frame.Elements.Find("OpenWindow").Click();

        // get popup window
        WebTestConsole.Write("Get the popup page");
        HtmlPage popup = frame.GetPopupPage(1);

        // verify title of popup
        Assert.AreEqual("This is the Popup Page", popup.Elements.Find("h1", 0).GetInnerText());

        // click on a button in popup window
        WebTestConsole.Write("Click on button inside popup");
        popup.Elements.Find("ChangeSpanButton").Click();
        Assert.AreEqual("Button has been clicked", popup.Elements.Find("TheSpan").GetInnerText());

        //close the popup window
        WebTestConsole.Write("Click the button to close the popup");
        popup.Elements.Find("CloseWindowButton").Click();
    }

    [WebTestMethod]
    public void Frame()
    {
        HtmlPage page = new HtmlPage("TestFrameSet.htm");

        // get top frame
        HtmlPage frame = page.GetFramePage("topFrame");

        // verify title of frame
        Assert.AreEqual("This is the top frame", frame.Elements.Find("h1", 0).GetInnerText());

        // click on a button in frame
        frame.Elements.Find("ChangeSpanButton").Click();
        Assert.AreEqual("Button has been clicked", frame.Elements.Find("TheSpan").GetInnerText());
    }

    [WebTestMethod]
    public void PageWithIFrame()
    {
        HtmlPage page = new HtmlPage("TestFrameSet.htm");

        // get bottom frame
        HtmlPage bottomFrame = page.GetFramePage("bottomFrame");

        // verify title of frame
        Assert.AreEqual("This is the bottom frame with an iFrame", bottomFrame.Elements.Find("h1", 0).GetInnerText());

        // get the iFrame
        HtmlPage iFrame = bottomFrame.GetFramePage("theIFrame");

        //click on a button in frame
        iFrame.Elements.Find("ChangeSpanButton").Click();
        Assert.AreEqual("Button has been clicked", iFrame.Elements.Find("TheSpan").GetInnerText());
    }
}
