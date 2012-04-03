<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {
        TextBoxLabel.Text = "[" + TextBox1.Text + "]";
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        ButtonLabel.Text = "[ButtonClick]";
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        LinkButtonLabel.Text = "[LinkClicked]";
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownLabel.Text = "["+ DropDownList1.SelectedValue +"]";
    }

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        CheckBoxLabel.Text = "[CheckBoxClick]";
    }

    protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        RadioButtonLabel.Text = "[RadioClick]";
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript">
        function DoClick()
        {
            var button3 = document.getElementById("Button3");
            button3.className = "Fuchi";
            button3.value = "New Button";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <strong>
        Test Page</strong><br />
        <br />
        <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="True" OnTextChanged="TextBox1_TextChanged" ToolTip="ToolTip" CssClass="myClass"></asp:TextBox>
        <asp:Label ID="TextBoxLabel" runat="server"></asp:Label><br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" AccessKey="a" CssClass="ButtonClass" TabIndex="2" ToolTip="ButtonToolTip" />
        <asp:Label ID="ButtonLabel" runat="server"></asp:Label><br />
        <br />
        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" AccessKey="b" CssClass="LinkClass" TabIndex="4" ToolTip="LinkToolTip">LinkButton</asp:LinkButton>
        <asp:Label ID="LinkButtonLabel" runat="server"></asp:Label><br />
        <br />
        <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem>a</asp:ListItem>
            <asp:ListItem>b</asp:ListItem>
            <asp:ListItem Value="c"></asp:ListItem>
        </asp:DropDownList>
        <asp:Label ID="DropDownLabel" runat="server"></asp:Label><br />
        <br />
        <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged" />
        <asp:Label ID="CheckBoxLabel" runat="server"></asp:Label><br />
        <br />
        <asp:RadioButton ID="RadioButton1" runat="server" AutoPostBack="True" OnCheckedChanged="RadioButton1_CheckedChanged" />
        <asp:Label ID="RadioButtonLabel" runat="server"></asp:Label><br />
        <br /><asp:Button ID="Button2" runat="server" Text="Button" OnClick="Button1_Click" AccessKey="a" CssClass="ButtonClass" Enabled="False" TabIndex="2" ToolTip="ButtonToolTip" /><br />
        <br />
        <input id="Button3" type="button" value="Old Button" onclick="javascript:DoClick();" /><br />
        <br />
        <div>
            </div>
    </div>
    </form>
</body>
</html>
