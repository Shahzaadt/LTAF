<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections.Generic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    
    protected void Page_Load(object sender, EventArgs e)
    {
        string startupScript = @"
        Sys.Application.add_load(OnAppLoaded);
        
        function OnAppLoaded() 
        {
            TestExecutor.start();        
        }";
        
        Page.ClientScript.RegisterStartupScript(typeof(Page), "LTAFStartup", startupScript, true);
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Microsoft.Web.Testing Javascript Driver</title>
   
    <link rel="stylesheet" type="text/css" media="screen" href="driver.css" />
</head>
<body>
    <form id="form1" runat="server">
     <div id="Container">
        <asp:ScriptManager runat="server" ID="scriptManager">
            <Scripts>
                <asp:ScriptReference Path="ScriptLibrary.js" />
            </Scripts>
            <Services>
                <asp:ServiceReference Path="WebService.asmx" />
            </Services>
        </asp:ScriptManager>

        <asp:Menu ID="__HiddenMenu1" runat="server">
            <Items>
                <asp:MenuItem Text="" Value=""></asp:MenuItem>
            </Items>
        </asp:Menu>
        <div id="Header">
            <div id="Logo"><b>LTAF</b></div>
            
        <frame name="testFrame" src="">
                <img id="spinner" alt="." width="12" height="12" src="<%= Page.ClientScript.GetWebResourceUrl(typeof(LTAF.Engine.TestDriverPage), "LTAF.Engine.Resources.spinner.gif") %>" style="visibility:hidden" /> 
                <b><asp:Label ID="DriverName" Text="LTAF Javascript Driver" runat="server" /></b>
            </div>
        <div id="Content">
            <span id="Commands">Commands: </span>
            <span id="Messages"></span>
        </div>
        <br />
     </div>
    </form>
</body>
</html>
