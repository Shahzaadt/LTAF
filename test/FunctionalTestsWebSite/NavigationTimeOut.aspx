<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test isNavigationComplete Function</title>
    <script type="text/javascript">
		function pageLoad(){
			TestExecutor._activeWindow = new LTAF.BrowserWindow(this);
			TestExecutor.set_browserInfo(new MockBrowserInfo());
		}
		function SetupMockUI()
		{
			var span = document.createElement("span");
			span.setAttribute("id", "Testing_WaitSpan");
			span.innerHTML = "[waiting for navigation...]";
			document.getElementById("divMockUI").appendChild(span);
		}
		function RunTest(){
			//Set Items that would be set by command Handler
			TestExecutor.set_contextObject(30000);
			// Call isNavigationComplete function		
			var waitingTime = document.getElementById("txtWaitTime").value;
			var complete = TestExecutor.get_activeWindow()._isNavigationCompleteInternal(parseInt(waitingTime));
			// Set results for verification
			document.getElementById("navComplete").innerHTML = complete;
			if(TestExecutor._browserInfo.ErrorMessages)
			{
			    document.getElementById("errorMessage").innerHTML = TestExecutor._browserInfo.ErrorMessages;
			}
		}
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:ScriptManager runat="server" ID="DriverPageScriptManager" EnablePageMethods="true" EnablePartialRendering="true">
			<Scripts>
				<asp:ScriptReference Name="LTAF.Engine.Resources.TestcaseExecutor.js" Assembly="LTAF" />
				<asp:ScriptReference Path="~/MockBrowserInfo.js" />
			</Scripts>
		</asp:ScriptManager>
		<h1>Test isNavigationComplete Function</h1>
		<div id="divMockUI">
		</div>
		<div id="divTest">
			Time Spent Waiting for Navigation (in milliseconds): <input type="text" id="txtWaitTime" value="30000" />
			<input type="button" id="btnSetupMockUI" onclick="SetupMockUI()" value="Setup Mock UI" />
			<input type="button" id="btnRunTest" onclick="RunTest()" value="Run Test" />
			<br />
			Is Navigation Complete: <span id="navComplete"></span>
			<br />
			Error Message: <span id="errorMessage"></span>
		</div>
    </div>
    </form>
</body>
</html>
