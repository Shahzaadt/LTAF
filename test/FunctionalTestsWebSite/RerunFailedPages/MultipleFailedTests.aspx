<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Test JavaScript to Rerun Failed Tests | Multiple Failed Tests</title>
    <script type="text/javascript">
		function pageLoad(){
			TestExecutor._activeWindow = new LTAF.BrowserWindow(this)
		}
		function RunTest(){
			TreeView_TestcaseExecuted("Test1", false, "Stack Trace");
			TreeView_TestcaseExecuted("Test2", false, "Stack Trace");
			TreeView_TestcaseExecuted("Test3", false, "Stack Trace");
			document.getElementById("TestUrl").innerHTML = GetFailedTestsURL();
			document.getElementById("numFailedTests").innerHTML = failedTestIds.length;
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
		<h1>Test JavaScript to Rerun Failed Tests | Multiple Failed Tests</h1>
		<div id="divMockDriverPageUI">
			<div id="MockTestTree">
				<a href="javascript:__doPostBack('Test1')">Test1 | Dummy Test</a><br />
				<a href="javascript:__doPostBack('Test2')'">Test2 | Dummy Test</a><br />
				<a href="javascript:__doPostBack('Test3')">Test3 | Dummy Test</a><br />
				<a href="javascript:__doPostBack('Test4')">Test4 | Dummy Test</a>
			</div>
		</div>
		<div id="divTestUI">
			<input type="button" id="btnRunTests" onclick="RunTest()" value="Run Test" />
			<br />
			# Failed tests: <span id="numFailedTests">0</span>
			<br />
			Test Url: <span id="TestUrl"></span>
		</div>
    </div>
    </form>
</body>
</html>
