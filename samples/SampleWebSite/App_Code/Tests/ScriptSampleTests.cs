using System;
using LTAF;

[WebTestClass]
public class ScriptSampleTests
{
    [WebTestMethod]
    public void HandleConfirmPopUp()
    {
        HtmlPage page = new HtmlPage("ScriptSamples.aspx");

        // click on the button and cancel the confirm
        page.Elements.Find("ButtonWithConfirm").Click(new CommandParameters(WaitFor.None,PopupAction.ConfirmCancel));
        Assert.AreEqual("", page.Elements.Find("Label1").GetInnerText());

        // click on the button and accept the confirm
        page.Elements.Find("ButtonWithConfirm").Click(new CommandParameters(WaitFor.Postback, PopupAction.ConfirmOK));
        Assert.AreEqual("The Button was clicked.", page.Elements.Find("Label1").GetInnerText());
    }

    [WebTestMethod]
    public void WaitUntilSpanIsUpdated()
    {
        HtmlPage page = new HtmlPage("ScriptSamples.aspx");

        // click on the button that takes 2 seconds to update a span
        page.Elements.Find("ButtonUpdatesSpan").Click();

        // wait for span
        page.Elements.Find("Span1").WaitForInnerText("Operation Complete on Span1.", 5);

        // continue with test
        Assert.AreEqual("Operation Complete on Span1.", page.Elements.Find("Span1").GetInnerText());
    }

    [WebTestMethod]
    public void ExecuteScriptOnPage()
    {
        HtmlPage page = new HtmlPage("ScriptSamples.aspx");

        // Execute script in the context of the ScriptSamples.aspx page
        page.ExecuteScript("document.getElementById('Span2').innerHTML = 'Operation Complete on Span2.';");

        // Verify span
        Assert.AreEqual("Operation Complete on Span2.", page.Elements.Find("Span2").GetInnerText());
    }

    [WebTestMethod]
    public void ExecuteScriptOnPageAndGetReturnValue()
    {
        HtmlPage page = new HtmlPage("ScriptSamples.aspx");

        // Call script that exists in the ScriptSamples.aspx page and get return value
        string value = (string) page.ExecuteScript("GetValueFromScript()");

        // Verify return value from script
        Assert.AreEqual("Value from script", value);
    }

    
}