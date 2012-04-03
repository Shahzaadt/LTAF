<%@ Page Language="C#" %>

<html xmlns="http://www.w3.org/1999/xhtml">
    <frameset rows="100,100%">
        <frame name="AjaxDriver" src="DriverPage.aspx">
        <frame name="testFrame" src="<%= Page.ClientScript.GetWebResourceUrl(typeof(LTAF.Engine.TestDriverPage), "LTAF.Engine.Resources.StartUpPage.htm") %>">
    </frameset>
</html>