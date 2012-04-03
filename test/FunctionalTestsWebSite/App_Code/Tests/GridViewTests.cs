using System;
using System.Data;
using System.Configuration;
using LTAF;
using LTAF.CompositeControls;

[WebTestClass]
public class GridViewTests
{
    [WebTestMethod]
    public void GridViewSortTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("DataControls.aspx");

        TestGridView gridView = new TestGridView(page.Elements.Find("CoursesGridView"));

        //sort by name
        gridView.Sort("Name");

        // verify sort operation (we are going to just go and get the inner text)
        Assert.AreEqual("Course Name #17", gridView.Rows[10].Cells[4].GetInnerText());
    }

    [WebTestMethod]
    public void GridViewPagingTest()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("GridViewControl.aspx");

        // GV1 page forward
        TestGridView gridView = new TestGridView(page.Elements.Find("GridView1"));
        gridView.Page("...");
        Assert.AreEqual("Course Name #12", gridView.Rows[1].Cells[4].GetInnerText());

        // GV1 page back
        gridView.Page("4");
        Assert.AreEqual("Course Name #9", gridView.Rows[1].Cells[4].GetInnerText());

        // GV2 page forward
        gridView = new TestGridView(page.Elements.Find("GridView2"));
        gridView.Page(">");
        Assert.AreEqual("Course Name #3", gridView.Rows[1].Cells[4].GetInnerText());

        // GV2 page back
        gridView.Page("<");
        Assert.AreEqual("Course Name #0", gridView.Rows[1].Cells[4].GetInnerText());

        // GV3 page forward
        gridView = new TestGridView(page.Elements.Find("GridView3"));
        gridView.Page(">>");
        Assert.AreEqual("Course Name #18", gridView.Rows[1].Cells[4].GetInnerText());

        // GV3 page back
        gridView.Page("<<");
        Assert.AreEqual("Course Name #0", gridView.Rows[1].Cells[4].GetInnerText());

        // GV4 page forward
        gridView = new TestGridView(page.Elements.Find("GridView4"));
        gridView.Page(">>");
        Assert.AreEqual("Course Name #18", gridView.Rows[1].Cells[4].GetInnerText());

        // GV3 page back
        gridView.Page("5");
        Assert.AreEqual("Course Name #12", gridView.Rows[1].Cells[4].GetInnerText());
    }

    [WebTestMethod]
    public void GridViewEditCancelTest()
    {
        Test_DataControlsPage page = new Test_DataControlsPage();
        page.Navigate();

        page.GridView1.ClickEditRow(9);
        page.GridView1.ClickCancel();

        Assert.StringContains(page.GridView1.Rows[10].GetInnerTextRecursively(), "Edit");
        
    }
}
