using System;
using System.Data;
using System.Configuration;
using LTAF;


[WebTestClass]
public class AttributeTests
{
    [WebTestMethod]
    public void HtmlElement()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        HtmlElementAttributeReader attributes = (HtmlElementAttributeReader) page.Elements.Find("TextBox1").GetAttributes();
        Assert.AreEqual("TextBox1", attributes.Name);
        Assert.AreEqual("TextBox1", attributes["name"]);
        Assert.AreEqual("ToolTip", attributes.Title);
        Assert.AreEqual("myClass", attributes.Class);
        Assert.IsNull(attributes.Get<string>("foobar", null));
    }

    [WebTestMethod]
    public void HtmlTextElement()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        HtmlInputElementAttributeReader attributes = ((HtmlInputElement)page.Elements.Find("TextBox1")).GetAttributes();
        Assert.AreEqual("TextBox1", attributes.Name);
        Assert.AreEqual("ToolTip", attributes.Title);
        Assert.AreEqual("myClass", attributes.Class);
        Assert.AreEqual(HtmlInputElementType.Text, attributes.Type);
        Assert.AreEqual(HtmlInputElementType.Text, attributes.Type);

    }

    [WebTestMethod]
    public void HtmlButtonElement()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        HtmlInputElementAttributeReader attributes = ((HtmlInputElement) page.Elements.Find("Button1")).GetAttributes();
        Assert.AreEqual("Button1", attributes.Name);
        Assert.AreEqual("ButtonToolTip", attributes.Title);
        Assert.AreEqual("ButtonClass", attributes.Class);
        Assert.AreEqual(HtmlInputElementType.Submit, attributes.Type);
        Assert.AreEqual("a", attributes.AccessKey);
        Assert.AreEqual("Button", attributes.Value);
        Assert.AreEqual(2, attributes.TabIndex);
        Assert.IsFalse(attributes.Disabled);

        Assert.IsTrue(((HtmlInputElement)page.Elements.Find("Button2")).GetAttributes().Disabled);
    }

    [WebTestMethod]
    public void HtmlAnchorElement()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        HtmlAnchorElementAttributeReader attributes = ((HtmlAnchorElement) page.Elements.Find("LinkButton1")).GetAttributes();

        Assert.IsNull(attributes.Name);
        Assert.AreEqual("b", attributes.AccessKey);
        Assert.AreEqual(4, attributes.TabIndex);                
        Assert.AreEqual("LinkToolTip", attributes.Title);
        Assert.AreEqual("LinkClass", attributes.Class);
        Assert.IsTrue(attributes.HRef.Contains("javascript"));
    }

    [WebTestMethod]
    public void RefreshAttributes()
    {
        HtmlPage page = new HtmlPage();
        page.Navigate("CoreControls.aspx");

        HtmlInputElement button = (HtmlInputElement) page.Elements.Find("Button3");

        HtmlInputElementAttributeReader attributes = button.GetAttributes();

        Assert.AreEqual("Old Button", attributes.Value);
        Assert.IsNull(attributes.Class);

        button.Click();

        attributes = button.GetAttributes();
        Assert.AreEqual("New Button", attributes.Value);
        Assert.AreEqual("Fuchi", attributes.Class);

    }

	[WebTestMethod]
	public void GetAttributeWithNoEncodingWhenDoubleQuotes()
	{
		HtmlPage page = new HtmlPage("miscellaneous.aspx");
		HtmlInputElementAttributeReader attributes = ((HtmlInputElement)page.Elements.Find("txtDoubleQuotes")).GetAttributes();
		Assert.AreEqual("foo\"bar", attributes.Value);
	}

	[WebTestMethod]
	public void GetAttributeWithNoEncodingWhenSingleQuotes()
	{
		HtmlPage page = new HtmlPage("miscellaneous.aspx");
		HtmlInputElementAttributeReader attributes = ((HtmlInputElement)page.Elements.Find("txtSingleQuotes")).GetAttributes();
		Assert.AreEqual("foo\'bar", attributes.Value);
	}

	[WebTestMethod]
	public void GetAttributeWithEncodedSingleQuotes()
	{
		HtmlPage page = new HtmlPage("miscellaneous.aspx");
		HtmlInputElementAttributeReader attributes = ((HtmlInputElement)page.Elements.Find("txtBothQuotes")).GetAttributes();
		Assert.AreEqual("foo\"&apos;bar", attributes.Value);

	}
}
