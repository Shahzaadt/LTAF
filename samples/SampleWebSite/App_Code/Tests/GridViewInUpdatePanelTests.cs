using System;
using System.Data;
using System.Configuration;
using LTAF;

[WebTestClass]
public class GridViewInUpdatePanelTests
{
    [WebTestMethod]
    public void GridViewSortTest()
    {
        HtmlPage page = new HtmlPage();
        LoginAndGoToHome(page);

        // Get the gridview table
        HtmlTableElement gridView = (HtmlTableElement) page.Elements.Find("CoursesGridView");

        // Sort by name
        gridView.Rows[0].Cells[4].ChildElements[0].Click(WaitFor.AsyncPostback);

        // Verify sort operation 
        Assert.AreEqual("Course Name #17", gridView.Rows[10].Cells[4].GetInnerText());
    }

    private void LoginAndGoToHome(HtmlPage page)
    {
        // Navigate to Home page
        page.Navigate("Home.aspx");

        if (page.Elements.Exists(new HtmlElementFindParams("tab-login"), 0))
        {
            // If the login tab exists, then we were unauthenticated and we need to login
            page.Elements.Find("UserName").SetText("ValidUser");
            page.Elements.Find("Password").SetText("foo");
            page.Elements.Find("LoginButton").Click(WaitFor.Postback);
        }
    }

    [WebTestMethod]
    public void GridViewPagingTest()
    {
        HtmlPage page = new HtmlPage();
        LoginAndGoToHome(page);

        // Get the gridview table
        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");

        // Verify current last entry
        Assert.AreEqual("Course Name #9", gridView.Rows[10].Cells[4].GetInnerText());

        // Go to page two
        gridView.Rows[11].ChildElements.Find("a", 0).Click(WaitFor.AsyncPostback);

        // Verify current last entry
        Assert.AreEqual("Course Name #19", gridView.Rows[10].Cells[4].GetInnerText());
    }

    [WebTestMethod]
    public void GridViewEditTest()
    {
        HtmlPage page = new HtmlPage();
        LoginAndGoToHome(page);

        // Build the expected grades that are going to be set
        Random rand = new Random(DateTime.Now.Second);
        string grade1 = rand.Next(100).ToString();
        string grade2 = rand.Next(100).ToString();
        string grade3 = rand.Next(100).ToString();

        // Get the gridview table
        HtmlTableElement gridView = (HtmlTableElement)page.Elements.Find("CoursesGridView");

        // Go to edit mode
        gridView.Rows[1].Cells[1].ChildElements[0].Click(WaitFor.AsyncPostback);

        // Fill the textboxes
        gridView.ChildElements.Refresh();
        gridView.Rows[1].Cells[6].ChildElements[0].SetText(grade1);
        gridView.Rows[1].Cells[7].ChildElements[0].SetText(grade2);
        gridView.Rows[1].Cells[8].ChildElements[0].SetText(grade3);

        // Click update button
        gridView.Rows[1].Cells[1].ChildElements[0].Click(WaitFor.AsyncPostback);

        // Verify new values
        gridView.ChildElements.Refresh();
        Assert.AreEqual(grade1, gridView.Rows[1].Cells[6].CachedInnerText);
        Assert.AreEqual(grade2, gridView.Rows[1].Cells[7].CachedInnerText);
        Assert.AreEqual(grade3, gridView.Rows[1].Cells[8].CachedInnerText);
    }
}
