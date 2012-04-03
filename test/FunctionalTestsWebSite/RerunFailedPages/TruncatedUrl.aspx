<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Test JavaScript to Rerun Failed Tests | Truncated Url</title>
    <script type="text/javascript">
		function pageLoad(){
			TestExecutor._activeWindow = new LTAF.BrowserWindow(this)
		}
		function RunTest(){
			//mock failed tests
			for(i=0; i<200; i++){
				 failedTestIds.push("This.is.a.long.test.name.test" + i);
			}
			RerunFailedTests();
			document.getElementById("urlIsTruncated").innerHTML = urlTruncated;
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
		<h1>Test JavaScript to Rerun Failed Tests | Truncated Url</h1>
		<div id="divTestUI">
			<input type="button" id="btnRunTests" onclick="RunTest()" value="Run Test" />
			<br />
			# Failed tests: <span id="numFailedTests">0</span>
			<br />
			Url Truncated: <span id="urlIsTruncated">false</span>
		</div>
    </div>
    </form>
</body>
</html>