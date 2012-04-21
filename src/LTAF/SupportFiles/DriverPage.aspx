<%@ Page Language="C#" Inherits="LTAF.Engine.TestDriverPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>LTAF Javascript Driver</title>
    <link rel="stylesheet" type="text/css" media="screen" href="<%= Page.ClientScript.GetWebResourceUrl(typeof(LTAF.Engine.TestDriverPage), "LTAF.Engine.Resources.driver.css") %>" />
    <script type="text/javascript">
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="DriverPageScriptManager" EnablePageMethods="true" EnablePartialRendering="true">
            <Scripts>
                <asp:ScriptReference Name="LTAF.Engine.Resources.TestcaseExecutor.js" Assembly="LTAF" />
            </Scripts>
        </asp:ScriptManager>
        <asp:UpdatePanel runat="server" ID="DriverPageUpdatePanel">
            <ContentTemplate>
                <asp:PlaceHolder ID="DriverPageContentPlaceHolder" runat="server">
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>        
    </form>
</body>
</html>
