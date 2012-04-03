using System;
using System.Data;
using System.Configuration;
using LTAF;

using LTAF.CompositeControls;


[WebTestClass]
public class DataTest
{
    [WebTestMethod]
    public void EditDataTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("DataControls.aspx");

        HtmlElement refreshLabel = page.Elements.Find("span", 1);

        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");
        string originalString = gridView.TBody.ChildElements[3].ChildElements[6].GetInnerText();
        Assert.AreEqual("0", originalString.Trim());

        // click on edit link       
        gridView.ChildElements.Find("a", "Edit", 2).Click();
        refreshLabel.WaitForInnerText("UpdatePanel Refresh #:1", 10);

        // refresh only the gridview elements
        gridView.ChildElements.Refresh();

        // click on the checkbox
        gridView.ChildElements.Find("ctl04.*CheckBox1", MatchMethod.Regex).Click();

        // fill the textboxes (we are going to do it by traversing the dom just for fun)
        gridView.TBody.ChildElements[3].ChildElements[6].ChildElements.Find("input", 0).SetText("112233") ;

        // click update link
        gridView.ChildElements.Find("a", "ReadyToUpdateRow", 0).Click();
        refreshLabel.WaitForInnerText("UpdatePanel Refresh #:2", 10);

        // verify gridview updated find table -> tobdy -> 4th row -> 7th cell
        gridView.ChildElements.Refresh();
        string updatedString = gridView.TBody.ChildElements[3].ChildElements[6].GetInnerText();
        Assert.AreEqual("112233", updatedString.Trim());

		// revert data back to original state
		gridView.ChildElements.Find("a", "Edit", 2).Click();
		refreshLabel.WaitForInnerText("UpdatePanel Refresh #:3", 10);
		gridView.TBody.ChildElements[3].ChildElements[6].ChildElements.Find("input", 0).SetText("0");
		gridView.ChildElements.Find("a", "ReadyToUpdateRow", 0).Click();
		refreshLabel.WaitForInnerText("UpdatePanel Refresh #:4", 10);
		Assert.AreEqual("0", gridView.TBody.ChildElements[3].ChildElements[6].GetInnerText());

    }

    

    [WebTestMethod]
    public void SortDataTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("DataControls.aspx");

        HtmlElement refreshLabel = page.Elements.Find("UpdatePanelRefreshLabel");

        // verify initial state
        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");
        HtmlElement elementToVerify = gridView.TBody.ChildElements[10].ChildElements[4];
        Assert.AreEqual("Course Name #9", elementToVerify.GetInnerText().Trim());

        //sort by name
        gridView.ChildElements.Find("a", "Name", 0).Click();
        refreshLabel.WaitForInnerText("UpdatePanel Refresh #:1", 10);

        // verify sort operation (we are going to just go and get the inner text)
        Assert.AreEqual("Course Name #17", elementToVerify.GetInnerText());
    }

    [WebTestMethod]
    public void PagingTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("DataControls.aspx");

        HtmlElement refreshLabel = page.Elements.Find("UpdatePanelRefreshLabel");

        // verify initial state
        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");
        HtmlElement elementToVerify = gridView.TBody.ChildElements[1].ChildElements[4];
        Assert.AreEqual("Course Name #0", elementToVerify.GetInnerText().Trim());

        //page
        gridView.ChildElements.Find("a", "2", 0).Click();
        refreshLabel.WaitForInnerText("UpdatePanel Refresh #:1", 10);

        // verify sort operation (we are going to just go and get the inner text)
        Assert.AreEqual("Course Name #10", elementToVerify.GetInnerText());
    }

    [WebTestMethod]
    public void ClickOnTemplateButton()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("DataControls.aspx");

        HtmlElement refreshLabel = page.Elements.Find("UpdatePanelRefreshLabel");

        // verify initial state (note that we don't need to refresh the innertext)
        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");
        HtmlElement templateField = gridView.TBody.ChildElements[1].ChildElements[9];
        Assert.AreEqual("0", templateField.GetInnerTextRecursively().Trim());

        //page
        templateField.ChildElements.Find("input", 0).Click();
        refreshLabel.WaitForInnerText("UpdatePanel Refresh #:1", 10);

        // verify sort operation (we are going to just go and get the inner text)
        Assert.AreEqual("1000", templateField.GetInnerTextRecursively().Trim());

		// revert data back to original state
		templateField.ChildElements.Find("input", 1).Click(WaitFor.Postback);
		Assert.AreEqual("0", templateField.GetInnerTextRecursively().Trim());
    }
}
