using System;
using System.Drawing;
using LTAF;

[WebTestClass]
public class MiscellaneousTests
{
    [WebTestMethod]
    public void WaitForCustomFunction()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("HTMLPage1.htm");

        page.Elements.Find("Button1").Click();

        string expression = @"
        function foo()
        {
            if(document.getElementById('Messages').innerHTML == 'MyCodeMarker')
            {
                 return true;
            }
            else
            {
                return false;
            }
        }

        foo();";
        page.WaitForScript(expression, 10);

        Assert.AreEqual("MyCodeMarker", page.Elements.Find("Messages").GetInnerText());

    }

    [WebTestMethod]
    public void WaitForCustomFunction_WithLoad()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("HTMLPage1.htm");

        page.Elements.Find("Button1").Click();

        string expression = @"
        function foo()
        {
            if(document.getElementById('Messages').innerHTML == 'MyCodeMarker')
            {
                 return true;
            }
            else
            {
                return false;
            }
        }";

        page.ExecuteScript(expression);

        page.WaitForScript("foo()", 10);

        Assert.AreEqual("MyCodeMarker", page.Elements.Find("Messages").GetInnerText());

    }

    [WebTestMethod]
    public void MouseOverTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("HTMLPage1.htm");

        HtmlElement link = page.Elements.Find("Link1");
        link.MouseOver();

        Assert.AreEqual(Color.Green, link.GetAttributes().Style.BackgroundColor);

    }

    [WebTestMethod]
    public void LinkHrefTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("HTMLPage1.htm");

        page.Elements.Find("a", "HRef", 0).Click();

        Assert.AreEqual("href", page.Elements.Find("Messages").GetInnerText());
    }

    [WebTestMethod]
    public void LinkNavigationTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("HTMLPage1.htm");

        Assert.AreEqual("HTMLPage1", page.Elements.Find("title", 0).GetInnerText());

        HtmlElement anchor = page.Elements.Find("a", 0);
        anchor.Click();

        anchor.WaitUntilNotFound(5);

        Assert.AreEqual("HTMLPage2", page.Elements.Find("title", 0).GetInnerText());
    }

    [WebTestMethod]
    public void BasicActionsTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        page.Elements.Find("Button1").Click(WaitFor.Postback);
        page.Elements.Find("LinkButton1").Click(WaitFor.Postback);
        page.Elements.Find("CheckBox1").Click(WaitFor.Postback);
        page.Elements.Find("RadioButton1").Click(WaitFor.Postback);

        page.Elements.Find("TextBox1").SetText("foobar");
        page.Elements.Find("TextBoxLabel").WaitForInnerText("[foobar]", 5);

        page.Elements.Find("DropDownList1").SetSelectedIndex(1);
        page.Elements.Find("DropDownLabel").WaitForInnerText("[b]", 5);

        Assert.AreEqual("[ButtonClick]", page.Elements.Find("ButtonLabel").GetInnerText().Trim());
        Assert.AreEqual("[LinkClicked]", page.Elements.Find("LinkButtonLabel").GetInnerText().Trim());
        Assert.AreEqual("[CheckBoxClick]", page.Elements.Find("CheckBoxLabel").GetInnerText().Trim());
        Assert.AreEqual("[RadioClick]", page.Elements.Find("RadioButtonLabel").GetInnerText().Trim());

    }

    [WebTestMethod]
    public void UpdatePanelTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("UpdatePanelPage.aspx");

        HtmlElement waitSpan = page.Elements.Find("MiniScenarioRefresh");

        page.Elements.Find("NameTextBox").SetText("TestUser");
        waitSpan.WaitForInnerText("[Async Refresh #1]", 10);

        page.Elements.Find("GenderRadioButtonList_0").Click();
        waitSpan.WaitForInnerText("[Async Refresh #2]", 10);

        page.Elements.Find("StartNextButton").Click();
        waitSpan.WaitForInnerText("[Async Refresh #3]", 10);

        page.Elements.Refresh();
        page.Elements.Find("CountryDropDownList").SetSelectedIndex(1);
        waitSpan.WaitForInnerText("[Async Refresh #4]", 10);

        page.Elements.Find("StateListBox").SetSelectedIndex(1);
        waitSpan.WaitForInnerText("[Async Refresh #5]", 10);

        page.Elements.Find("a", "Next", 0).Click();
        waitSpan.WaitForInnerText("[Async Refresh #6]", 10);

        page.Elements.Refresh();
        page.Elements.Find("NewsCheckBox").Click();
        waitSpan.WaitForInnerText("[Async Refresh #7]", 10);

        page.Elements.Find("ULARadioButton").Click();
        waitSpan.WaitForInnerText("[Async Refresh #8]", 10);

        page.Elements.Find("GamesCheckBoxList_0").Click();
        waitSpan.WaitForInnerText("[Async Refresh #9]", 10);

        page.Elements.Find("a", "Step 4", 0).Click();
        waitSpan.WaitForInnerText("[Async Refresh #10]", 10);

        page.Elements.Refresh();
        page.Elements.Find("FinishPreviousButton").Click();
        waitSpan.WaitForInnerText("[Async Refresh #11]", 10);

        page.Elements.Refresh();
        page.Elements.Find("a", "Next", 0).Click();
        waitSpan.WaitForInnerText("[Async Refresh #12]", 10);

        page.Elements.Refresh();
        page.Elements.Find("a", "NextMonth", 0).Click();
        waitSpan.WaitForInnerText("[Async Refresh #13]", 10);

        page.Elements.Find("FinishImageButton").Click();
        waitSpan.WaitForInnerText("[Async Refresh #14]", 10);

        Assert.AreEqual("[Async Refresh #14]", waitSpan.GetInnerText());
    }

    [WebTestMethod]
    public void VerifyGettingTextWithApos()
    {
        HtmlPage page = new HtmlPage("Miscellaneous.aspx");
        HtmlInputElement input = (HtmlInputElement) page.Elements.Find("TextBoxWithApos");
        Assert.AreEqual("I'm here", input.GetAttributes().Value);
    }

    [WebTestMethod]
    public void VerifyIsVisibleProperty()
    {
        HtmlPage testPage = new HtmlPage();
        testPage.Navigate("Miscellaneous.aspx");

        Assert.IsFalse(testPage.Elements.Find("RequiredFieldValidator1").IsVisible());

        testPage.Elements.Find("Button1").Click();
        testPage.Elements.Refresh();

        Assert.IsTrue(testPage.Elements.Find("RequiredFieldValidator1").IsVisible());

    }

    [WebTestMethod]
    public void VerifyGetCurrentUrl()
    {
        HtmlPage p = new HtmlPage();
        p.Navigate("HTMLPage1.htm");

        Assert.StringContains(p.GetCurrentUrl(), "HTMLPage1.htm");

        p.Elements.Find("a", "Navigate to Page2", 0).Click(LTAF.WaitFor.Postback);
        Assert.StringContains(p.GetCurrentUrl(), "HTMLPage2.htm");

    }

    [WebTestMethod]
    public void VerifyHrefNotFollowedIfOnclickIsCancelled()
    {
        HtmlPage p = new HtmlPage("Miscellaneous.aspx");
        p.Elements.Find("anchor").Click();
        p.Elements.Find("anchorResult").WaitForInnerText("anchor has been clicked.", 5);

        Assert.StringEndsWith(p.GetCurrentUrl(), "Miscellaneous.aspx");
    }

    [WebTestMethod]
    public void FindAllByParams()
    {
        HtmlPage p = new HtmlPage("Miscellaneous.aspx");
        p.Navigate("Miscellaneous.aspx");

        HtmlElementFindParams f = new HtmlElementFindParams();
        f.TagName = "input";
        f.Attributes.Add("value", "TestButton");
        HtmlElement e = p.Elements.Find(f);
        Assert.AreEqual("Button2", e.Id);
        


    }
}
