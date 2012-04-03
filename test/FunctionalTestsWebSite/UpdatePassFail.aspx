<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Test UI Update Pass/Fail of a Test</title>
    <script type="text/javascript">
		function pageLoad(){
			TestExecutor._activeWindow = new LTAF.BrowserWindow(this);
		}
		function RunTest(){
			TreeView_TestcaseExecuted("Test1", true, "Stack Trace");
			TreeView_TestcaseExecuted("Test2", false, "Stack Trace");
			TreeView_TestcaseExecuted("Test3", false, "Stack Trace");
			TreeView_TestcaseExecuted("Test4", true, "Stack Trace");
		}
		
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:ScriptManager runat="server" ID="DriverPageScriptManager" EnablePageMethods="true" EnablePartialRendering="true">
			<Scripts>
				<asp:ScriptReference Name="LTAF.Engine.Resources.TestcaseExecutor.js" Assembly="LTAF" />
			</Scripts>
		</asp:ScriptManager>
		<h1>Test UI Update Pass/Fail of a Test</h1>
		<div id="divMockDriverPageUI">
			<div id="MockTestTree">
				<a id="Test1" href="javascript:__doPostBack('All Test Cases\\classname\\Test1')">Test1 | Dummy Test</a><br />
				<a id="Test2" href="javascript:__doPostBack('All Test Cases\\classname\\Test2')'">Test2 | Dummy Test</a><br />
				<a id="Test3" href="javascript:__doPostBack('All Test Cases\\foo.bugRepro\\Test3')'">Test3 | Dummy Test</a><br />
				<a id="Test4" href="javascript:__doPostBack('All Test Cases\\foo.foo2.bugRepro\\Test4')'">Test4 | Dummy Test</a>
			</div>
		</div>
		<div id="divTestUI">
			<input type="button" id="btnRunTests" onclick="RunTest()" value="Run Test" />
		</div>
    </div>
    </form>
</body>
</html>

