// script that is required by the main test driver page

var errorLogs = new Array();
var pageSources = new Array();
var failedTestIds = new Array();
var maxUrlLength = 2000;
var urlTruncated = false;

function LoadPassLog()
{
    parent.testFrame.location = '<%=WebResource("LTAF.Engine.Resources.LogSuccess.htm")%>';
}

function LoadErrorLog(index)
{
    parent.testFrame.location = '<%=WebResource("LTAF.Engine.Resources.LogErrorFrameSet.htm")%>';
    window.setTimeout('WriteLog('+ (index) +')', 250);
}

function WriteLog(index)
{
    var logSpan = parent.testFrame.stackTrace.document.getElementById('Log');
    logSpan.innerHTML = errorLogs[index];
    logSpan.style.color = "red";
    
    var htmlSource = pageSources[index];
    parent.testFrame.pageSource.document.write(htmlSource);
    parent.testFrame.pageSource.document.close();
}

function TreeView_TestcaseExecuted(testId, passed, stackTrace)
{
    testId = testId.replace(/\.(?!.*\.)/,'\\\\');
    
    var anchorCollection = document.getElementsByTagName('a');
    for(i=0; i < anchorCollection.length; i++)
    {
        var anchor = anchorCollection[i];
        if(anchor.href.indexOf(testId + "'") !== -1)
        {
            if(passed)
            {
				anchor.className ="testPassed";
                anchor.href = "javascript:LoadPassLog();"
            }
            else
            {
                var pageDom = TestExecutor.get_activeWindow().get_activeFrame().calculateCurrentDom("all_attributes");
                pageSources.push(pageDom);                   
            
                errorLogs.push(stackTrace);
                failedTestIds.push(testId.replace('\\\\','.'));
                
                anchor.className = "testFailed";
                var logViewerHref = "javascript:LoadErrorLog("+ (errorLogs.length - 1) +");"
                anchor.href = logViewerHref;
            }
			return;
        }
    }
}

function GetFailedTestsURL()
{
	var baseUrl = parent.location.href.split('?')[0];
	var queryString = "?tag=";
	for(i=0;i<failedTestIds.length; i++)
	{
		if(queryString.length + failedTestIds[i].length > maxUrlLength)
		{
			urlTruncated = true;
			break;
		}
		queryString += failedTestIds[i];
		if(i < failedTestIds.length -1)
		{
			queryString += "@";
		}
	}
	return baseUrl + queryString + "&Filter=true&Run=true";
}

function RerunFailedTests()
{
	urlTruncated = false;
	var truncatedOkay = true;	
	if(failedTestIds.length>0)
	{
		var url = GetFailedTestsURL();
		if(urlTruncated)
		{
			truncatedOkay = confirm("I'm sorry, there are too many failed tests, some of the tests will not be run. Would you still like to continue?")
		}
		if(truncatedOkay)
		{
			window.open(url);
		}		
	}
	else
	{
		alert("Good News, there are no Failed Tests!");
	}
}

//Remember Treview Scroll Position
var testTreviewXScrollPos, testTreviewYScrollPos;
var prm = Sys.WebForms.PageRequestManager.getInstance();
prm.add_beginRequest(BeginRequestHandlerTreeviewScroll);
prm.add_endRequest(EndRequestHandlerTreeviewScroll);
function BeginRequestHandlerTreeviewScroll(sender, args) {
	testTreviewXScrollPos = $get('Tests').scrollLeft;
	testTreviewYScrollPos = $get('Tests').scrollTop;
}
function EndRequestHandlerTreeviewScroll(sender, args) {
	$get('Tests').scrollLeft = testTreviewXScrollPos;
	$get('Tests').scrollTop = testTreviewYScrollPos;
}