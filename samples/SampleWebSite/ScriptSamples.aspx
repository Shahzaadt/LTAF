<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" Title="Untitled Page" %>

<script runat="server">

    protected void ButtonWithConfirm_Click(object sender, EventArgs e)
    {
        Label1.Text = "The Button was clicked.";
    }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentPlaceHolder" runat="server">
    <script type="text/javascript">
        function UpdateSpan1()
        {
            document.getElementById("Span1").innerHTML = "Operation Complete on Span1.";
        }
        
        function GetValueFromScript()
        {
            return "Value from script";
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PaperContentPlaceHolder" Runat="Server">
    <b>1. This sample demonstrates how to automate a javascript confirm pop up.</b>
    <br />
    <asp:Button ID="ButtonWithConfirm" runat="server" Text="Button With Confirm" 
        OnClientClick="return confirm('Are you sure you want to submit?');" onclick="ButtonWithConfirm_Click" />
    <asp:Label ID="Label1" runat="server"></asp:Label>
    <br />
    <br />
    <b>2. This sample demonstrates how to wait for an element on the page to be updated.</b>    
    <input id="ButtonUpdatesSpan" type="button" value="Button that updates Span after 2 seconds" onclick="window.setTimeout('UpdateSpan1()', 2000);" />
    <span id="Span1"></span>
    <br />
    <br />
    <b>3. This sample demonstrates how to execute script on the page from a testcase.</b>
    <br />
    (Test will call into a function to update this span)
    <span id="Span2"></span>
    <br />
    <br />
    <b>4. This sample demonstrates how to retrieve data from a script expression on the page.</b>
    <br />
    (Test will call into a function that will return: "Value from script")
    <br />
    <br />
    <b>5. This sample demonstrates how to automate the content of a popup window</b>
    <br />
    <input id="OpenWindow" type="button" value="Open New Window" onclick="javascript:window.open('PopupPage.htm', 'MyPopup');" />
</asp:Content>

