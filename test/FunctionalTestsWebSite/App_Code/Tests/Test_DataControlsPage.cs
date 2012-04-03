using System;
using System.Data;
using System.Configuration;
using LTAF;

using LTAF.CompositeControls;

public class Test_DataControlsPage : HtmlPage
{
    private TestGridView _gridView;

    public Test_DataControlsPage()
        : base()
    {

    }

    public void Navigate()
    {
        base.Navigate("DataControls.aspx");
    }

    public TestGridView GridView1
    {
        get
        {
            if (_gridView == null)
            {
                _gridView = new TestGridView(this.Elements.Find("CoursesGridView"));
            }
            return _gridView;
        }
    }
}
