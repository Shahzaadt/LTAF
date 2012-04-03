<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
	protected void Page_Load(object sender, EventArgs e)
	{
		txtBothQuotes.Text = "foo\"\'bar";
		txtDoubleQuotes.Text = "foo\"bar";
		txtSingleQuotes.Text = "foo\'bar";
	}
   

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
        ControlToValidate="TextBox1" ErrorMessage="RequiredFieldValidator"></asp:RequiredFieldValidator>
    <asp:Button ID="Button1" runat="server" Text="Button" />
    <asp:Button ID="Button2" runat="server" Text="TestButton" />
    <span id="anchorResult"></span>
    <a href="HTMLPage1.htm" id="anchor" onclick="javascript:getElementById('anchorResult').innerHTML='anchor has been clicked.'; return false;">Anchor1</a>
    <asp:TextBox runat="server" ID="TextBoxWithApos">I'm here</asp:TextBox>
    <h2>Encoding Tests</h2>
    <asp:TextBox ID="txtDoubleQuotes" runat="server" />
    <asp:TextBox ID="txtSingleQuotes" runat="server" />
    <asp:TextBox ID="txtBothQuotes" runat="server" />
  </form>
</body>
</html>
