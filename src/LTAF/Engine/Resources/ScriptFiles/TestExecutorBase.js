Type.registerNamespace('LTAF');

// Static test executor
var TestExecutor = null;

/// <summary>
/// Class that's in charge of polling the MWT WebService for commands and invoking their handlers.
/// It also keeps track of the windows that have been opened.
/// </summary>
/// <change date="02/06/2006">Created</change>
LTAF.TestExecutorBase = function()
{
    ///<summary>The active window that will execute commands</summary>
    this._activeWindow = null;

    ///<summary>Whether the engine is running</summary>
    this._running = false;

    ///<summary>Last command received by the Engine</summary>
    this._lastCommand = null;
    
    ///<summary>The BrowserInfo object to hold log info and verification strings</summary>
    this._browserInfo = null;
    
    ///<summary>Collection of windows that are accessible by Engine.</summary>
    this._windowCollection = new Array();
    
    ///<summary>Delegate function to check if command is complete</summary>
    this._checkCommandCompleteFunction = null;
    
    ///<summary>Detail of the logging</summary>
    this._logDetail = LTAF.WebTestLogDetail.Default;
    
    ///<summary>Number of test cases in the test run</summary>
    this._numberOfTestCases = 0;
    
    ///<summary>Number of test cases already run</summary>
    this._numberOfTestCasesRun = 0;
    
    ///<summary>Number of failures in the test run</summary>
    this._numberOfTestCaseFailures = 0;
    
    ///<summary>Time that the current test run was started</summary>
    this._testRunStarted = null;
    
    ///<summary>Time that the current test case was started</summary>
    this._testCaseStarted = null;
    
    ///<summary>Number of requests for the next command that have retrieved no command</summary>
    this._noCommandCount = 0;
    
    /// <summary>Field to store context information that can be used by handler functions</summary>
    this._contextObject = null;
}

LTAF.TestExecutorBase.prototype = {
    /// <summary>
    /// Gets/Sets the context information that can be used by handler functions
    /// </summary>
    get_contextObject : function()
    {
        return this._contextObject;
    },  
    set_contextObject : function(value)
    {
        this._contextObject = value;
    },

    /// <summary>
    /// Abstract method that gets the next command from the server
    /// </summary>
    invokeGetCommand: Function.abstractMethod,
    
    /// <summary>
    /// Method that gets the latest command that was recieved by executor.
    /// </summary>
    get_lastCommand: function()
    {
        return this._lastCommand;
    },

    /// <summary>
    /// Method that returns the most recent accesed window
    /// </summary>
    get_activeWindow : function()
    {
        return this._activeWindow;
    },
    
    /// <summary>
    /// Method that returns the BrowserInfo object for the current command
    /// </summary>
    get_browserInfo : function()
    {
        return this._browserInfo;
    },
    
    /// <summary>
    /// Method that sets the BrowserInfo object for the current command
    /// </summary>
    set_browserInfo : function(browserInfo)
    {
        this._browserInfo = browserInfo;
    },
    
    /// <summary>
    /// Method that sets the callback function to use to check if current command has finished execution.
    /// </summary>
    set_checkCommandCompleteFunction : function(callback)
    {
        this._checkCommandCompleteFunction = callback;
    },
    
    /// <summary>
    /// Gets/Sets the number of requests have retrieved no command
    /// </summary>
    get_noCommandCount : function()
    {
      return this._noCommandCount;
    },
    set_noCommandCount: function(noCommandCount)
    {
      this._noCommandCount = noCommandCount;
    },
    
    /// <summary>
    /// Level of detail in the test logging
    /// </summary>
    get_logDetail : function()
    {
        return this._logDetail;
    },
    set_logDetail : function(value)
    {
        this._logDetail = value;
        this.verbose = (value == LTAF.WebTestLogDetail.Verbose);
    },
    
    /// <summary>
    /// Simple spinner show/hide functions to visualize when polling occurs
    // </summary>
    showSpinner : function()
    {
        var spinner = document.getElementById('spinner');
        if( spinner ) spinner.style.visibility = 'visible';
    },
    
    hideSpinner : function()
    {
        var spinner = document.getElementById('spinner');
        if( spinner ) spinner.style.visibility = 'hidden';    
    },
    
    /// <summary>
    /// Method that kicks of the engine
    /// </summary>
    start : function()
    {
        this._running = true;
        this.showSpinner();
        this.clearLog();
        
        // Initialize the BrowserInfo
        this._browserInfo = new LTAF.Engine.BrowserInfo();
        
        // Get the test frame
        var frame = parent.testFrame;
        if (!frame)
        {
            throw "Could not find 'testFrame' on page!";
        }
        
        // Prepare the main window
        this._activeWindow = new LTAF.BrowserWindow(frame);
        this._windowCollection.push(this._activeWindow);
                        
        // Start getting commands loop
        this.getNextCommand();
    },
    
    /// <summary>
    /// Method that stops the engine
    /// </summary>
    stop : function()
    {
        this.hideSpinner();
        this._running = false;
        
        // Display any error messages that we were not able to send to the server
        if (this._browserInfo && this._browserInfo.ErrorMessages)
        {
            alert(this._browserInfo.ErrorMessages);
        }
    },
    
    /// <summary>
    /// Method that makes an async call to MWT WebService to get the next command
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    getNextCommand : function()
    {
        if(!this._running)
        {
            return;
        }
        
        this.invokeGetCommand(null, this._onGetCommandComplete, this.onGetCommandError);
    },
    
    /// <summary>
    /// Method that is executed by atlas if the polling fails
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    _onGetCommandError : function(result)
    {
        TestExecutor.reportFailure("[TestExecutorException] Failed to poll for command!  (Details: " + TestExecutor.serializeAsJson(error) + ")");
        TestExecutor.stop();
    
        TestExecutor.printFatalError(result);
    },
    
    /// <summary>
    /// Method that is called when GetNextCommand webservice returns
    /// </summary>
    /// <change date="04/02/07">Created</change>    
    _onGetCommandComplete : function(command)
    {
        if(command != null)
        {
            TestExecutor.set_noCommandCount(0);
            TestExecutor.executeCommand(command)
        }
        else
        {
            var noCommandCount = TestExecutor.get_noCommandCount();
            noCommandCount++;
            if (noCommandCount > 3) {
                TestExecutor.reportFailure("Test run aborted!  The server is not responding with more commands!");
                TestExecutor.stop();
            } else {
                TestExecutor.set_noCommandCount(noCommandCount);
                TestExecutor.getNextCommand();
            }
        }
    },
    
    /// <summary>
    /// Method that is executed when the async poll for command is completed successfully
    /// </summary>
    /// <param name="command">An object of type LTAF.Command</param>
    executeCommand : function(command)
    {
        // set the last command
        this._lastCommand = command;
    
        // clear the command complete function
        this._checkCommandCompleteFunction = null;
    
        // Create a new BrowserInfo object to respond to this command
        this._browserInfo = new LTAF.Engine.BrowserInfo();
            
        // [11/04/2007] The following lines are to update the visual display. Think of moving them to another method
        var handlerName = command.Handler.ClientFunctionName;
        if (handlerName == "TestRunStarted")
        {
            var numberOfTests = command.Handler.Arguments[0];
            this._testRunStarted = new Date();
            this._numberOfTestCases = numberOfTests;
            this._numberOfTestCasesRun = 0;
            this._numberOfTestCaseFailures = 0;
            this.logTestRunStarted(numberOfTests);
        }
        else if (handlerName == "TestRunFinished")
        {
            this.logTestRunFinished();
            this.get_browserInfo().Data = this.getLogContent();
        }
        else if (handlerName == "TestcaseExecuting")
        {
            this._testCaseStarted = new Date();
            var testId = command.Handler.Arguments[0];
            this.logTestCaseStarted(testId);
        }
        else if (handlerName == "TestcaseExecuted")
        {
            var testId = command.Handler.Arguments[0];
            var passed = command.Handler.Arguments[1];
            var errorMessage = command.Handler.Arguments[2];
            this._numberOfTestCasesRun++;
            if (!passed) { this._numberOfTestCaseFailures++; }
            this.logTestCaseFinished(testId, passed, errorMessage);
        }
        else if (this._logDetail > LTAF.WebTestLogDetail.Concise)
        {
            this.logTestCommand(command);
        }
        // [11/04/2007] End of lines to update the visual display

        // Process the CommandTarget
        var callbackDelegate = Function.createDelegate(this, this.commandFinished);
        this.processCommandTarget(command, callbackDelegate);
        
    },
    
    /// <summary>
    /// Method that process the Target of this command.
    /// </summary>
    /// <param name="command">An object of type LTAF.Command</param>
    /// <change date="02/06/2006">Created</change>
    /// <change date="04/02/2007">Changed for synchrounous</change>
    /// <change date="4/20/2007">Added callback argument to allow dynamic execution flows</change>
    processCommandTarget : function(command, commandFinishedCallback) 
    {    
        try
        {
            // find on what window to execute this command. This will set the ActiveWindow of the Engine object
            var activeWindow = this.findWindow(command.Target);

            //find the frame within the window
            var activeFrame = activeWindow.findFrame(command.Target.FrameHierarchy);
            
            // Need to override the open function of this frame so that it can be captured by the Engine
            var jsFrame = activeFrame.get_jsFrame();
            if(!jsFrame._originalOpen)
            {
                jsFrame._originalOpen = jsFrame.open;
            }
            jsFrame.open = TestExecutor.openWindowAndRegister;
            
            // Hook up the popup handlers if necessary.            
            this.anticipatePopup(command.Handler.PopupAction, activeFrame);
            
            // find the object within the frame upon which to execute this command (if any)
            if(command.Handler.RequiresElementFound)
            {
                this.findTargetObject(0, commandFinishedCallback);
            }
            else
            {
                this.processCommandHandler(command, null, commandFinishedCallback);
            }
        }
        catch(e)
        {
            this.reportFailure(e);
            commandFinishedCallback();
        }
    },
    
    anticipatePopup : function(popupAction, activeFrame) {
        // If we need to anticipate alert, override the alert function
        //      of the current frame. And set an CommandCompleteDelegate that will
        //      wait until an alert has been caught.
        
        if(popupAction == LTAF.PopupAction.None) {
            return;
        }
        
        this._checkCommandCompleteFunction = TestExecutor.isPopupComplete;
        
        if(popupAction == LTAF.PopupAction.AlertOK)
        {
            activeFrame.get_jsFrame().alert = function(e)
            {
                TestExecutor.get_browserInfo().Data = e;
                return true;
            }
        } 
        else if(popupAction == LTAF.PopupAction.ConfirmOK)
        {
            activeFrame.get_jsFrame().confirm = function(e)
            {
                TestExecutor.get_browserInfo().Data = e;
                return true;
            }
        }
        else if(popupAction == LTAF.PopupAction.ConfirmCancel)
        {
            activeFrame.get_jsFrame().confirm = function(e)
            {
                TestExecutor.get_browserInfo().Data = e;
                return false;
            }
        }
    },
    
    /// <summary>
    /// Reentry method that keeps attempting to find target object. If succeeds, it calls to ProcessCommandHandler
    /// </summary>
    /// <change date="04/02/07">Created</change>    
    /// <change date="4/20/2007">Added callback argument to allow dynamic execution flows</change>
    findTargetObject : function(attempts, commandFinishedCallback)
    {
        var activeFrame = TestExecutor.get_activeWindow().get_activeFrame();
        var command = TestExecutor.get_lastCommand();
        var targetObject = null;
        
        try
        {
            targetObject = activeFrame.findObject(command.Target);
        }
        catch(e)
        {
            if(attempts < 10)
            {
                attempts++;
                window.setTimeout(function() { TestExecutor.findTargetObject(attempts, commandFinishedCallback); }, 200);
            }
            else
            {
                TestExecutor.reportFailure(e);
                commandFinishedCallback();
            }
        }
        
        if(targetObject != null)
        {
            TestExecutor.processCommandHandler(command, targetObject, commandFinishedCallback);
        }
    },
    
    /// <summary>
    /// Method that calls the handler of this command
    /// </summary>
    /// <change date="04/02/07">Created</change>    
    /// <change date="4/20/2007">Added callback argument to allow dynamic execution flows</change>
    processCommandHandler : function(command, targetObject, commandFinishedCallback)
    {
        try
        {
            //Invoke the command handler
            window[command.Handler.ClientFunctionName](targetObject, command.Handler.Arguments);
        }
        catch(e)
        {
            this.reportFailure('[TestExecutorException]: Unhandled JS exception while executing command handler function '+ command.Handler.ClientFunctionName + '.  (Details: ' + this.serializeAsJson(e) + ')');
        }
              
        if(this._checkCommandCompleteFunction != null)
        {
            // Javascript is single threaded, so it's ok for the MWT client-side framework to
            // continue setting Engine.CheckCommandCompleteDelegate instead of setting
            // the delegate on the command object itself.  (That's really where it should be.)
            // But the second we give up execution, all bets are off regarding the validity of
            // Engine.CheckCommandCompleteDelegate when we start executing again.
            // Fortunately, this is the one and only place where we first give up execution
            // after other parts of the framework set Engine.CheckCommandCompleteDelegate.
            // So we can save the delegate on the command object to preserve it for future
            // use.
            
            var cmdCheckState = new CheckCommandCompleteState();
            cmdCheckState.CheckCommandCompleteDelegate = this._checkCommandCompleteFunction;
            this._checkCommandCompleteFunction = null;

            //gonna poll the complete delegate
            cmdCheckState.CheckCommandCompleteAttemps = 0;
            cmdCheckState.WaitForCommandCompletePoller = window.setInterval(function() { TestExecutor.checkCommandComplete(cmdCheckState, commandFinishedCallback); }, cmdCheckState.CheckCommandCompleteDelay);
        }
        else
        {
            commandFinishedCallback();
        }
    },
    
    /// <summary>
    /// Method that is executed to check if a command has finished
    /// </summary>
    /// <change date="05/20/2006">Created</change>
    /// <change date="01/10/2007">Calculate and send the waiting time</change>
    /// <change date="4/20/2007">Added callback argument to allow dynamic execution flows</change>
    checkCommandComplete : function(cmdCheckState, commandFinishedCallback)
    {
        cmdCheckState.CheckCommandCompleteAttemps++;
        
        var isCommandComplete = false;
        
        try
        {
            isCommandComplete = cmdCheckState.CheckCommandCompleteDelegate(cmdCheckState.CheckCommandCompleteAttemps * cmdCheckState.CheckCommandCompleteDelay);            
        }
        catch(e)
        {
            isCommandComplete = true;   
            this.reportFailure('[TestExecutorException]: Unhandled JS exception while waiting for command to complete.  (Details: ' + this.serializeAsJson(e) + ')');
        }
                    
        if(isCommandComplete)
        {
            cmdCheckState.CheckCommandCompleteDelegate = null;
            window.clearInterval(cmdCheckState.WaitForCommandCompletePoller);
            commandFinishedCallback();
        }
    },
    
    /// <summary>
    /// Serialize a JavaScript object as JSON
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <change date="05/20/2006">Created</change>
    serializeAsJson : function(obj)
    {
        var result = null;
        if (obj)
        {
            try { result = Sys.Serialization.JavaScriptSerializer.serialize(obj); }
            catch (ex)
            {
                var error = ex ? ex.toString() : '';
                try { error = Sys.Serialization.JavaScriptSerializer.serialize(ex); } catch(ex) { }
                this.reportFailure('[TestExecutorException] Failed to serialize object as JSON!  (Details: ' + error + ')');
            }
        }
        return result;
    },
    
    /// <summary>
    /// Method that reports back to the webserver that the current command is finished.
    /// Then it waits for some time to ask for another command
    /// </summary>
    commandFinished : function()
    {
        //clear out the context object so that it is not used in subsequent commands
        TestExecutor.set_contextObject(null);    
        this.invokeGetCommand(this._browserInfo, this._onGetCommandComplete, this.onGetCommandError);
    },
    
    /// <summary>
    /// Method that stops the polling, and reports error on the driver frame
    /// </summary>
    /// <change date="05/08/2007">Created</change> 
    printFatalError : function(result)
    {
        this.hideSpinner();
    
        //The message pump is broken. Browser is unable to communicate with WebService
        //We need to log this error from an alternative route
        var errorMessage = "EngineExecutionException: Error while communicating with WebService. "
        
        if ( result != null ) 
        {
            errorMessage += "[StatusCode=" + result.get_statusCode() + "]";
            errorMessage += "[ExceptionType=" + result.get_exceptionType() + "]";
            errorMessage += "[Message=" + result.get_message() + "]";
            errorMessage += "[StackTrace=" + result.get_stackTrace() + "]";
        }
        else 
        {
            errorMessage += "[Unknown Error]";
        }
  
        window.location = "MWT_TestPage.aspx?errorMessage=" + errorMessage;
    },
    
    /// <summary>
    /// Method that given a Target object, finds a window in the collection
    /// </summary>
    /// <param name="command">An object of type LTAF.Target</param>
    findWindow : function(target)
    {
        this._activeWindow = null;
        if(target.WindowCaption != null && target.WindowCaption.length > 0)
        {
            var windowCount = this._windowCollection.length;
            for(i = 0; i < windowCount; i++)
            {
                var currentWindow = this._windowCollection[i].Window;
                if(currentWindow.document.title == target.WindowCaption
                    || currentWindow.document.url == target.WindowCaption)
                {
                    this._activeWindow = this._windowCollection[i];
                    break;
                }
            }
            if(this._activeWindow == null)
            {
                throw "[TestExecutorException]: Could not find window with Caption=" + target.WindowCaption;
            }
        }
        else
        {
            if(this._windowCollection.length - 1 < target.WindowIndex)
            {
                throw "[TestExecutorException]: The WindowCollection does not have an object on index=" + target.WindowIndex;
            }
           
            this._activeWindow = this._windowCollection[target.WindowIndex];
        }
        return this._activeWindow;
    },
    
    /// <summary>
    /// Method that will add an error message to the BrowserInfo object to send
    /// </summary>
    /// <param name="message">The string to log as failure</param>
    reportFailure : function(message)
    {
        var error = message ? message.toString() : '';
        try 
        { 
            error = Sys.Serialization.JavaScriptSerializer.serialize(message); 
        } catch(ex) { }
    
        if(this._browserInfo.ErrorMessages == null)
        {
            this._browserInfo.ErrorMessages = error;
        }
        else
        {
            this._browserInfo.ErrorMessages += '|' + error;
        }
    },
    
     /// <summary>
    /// Method that opens a new browser window and registers it in the WindowCollection property
    /// </summary>
    /// <param name="url">String specifying the location of the web page to be displayed in the new window</param>
    /// <param name="name">String specifying the name of the new window</param>
    /// <param name="feature">Optional string parameter specifying the features of the new window</param>
    /// <param name="replace">Optional bollean parameter that specifies if the new location will replace the current page in the browser's history</param>
    openWindowAndRegister : function(url,name,features,replace)
    {
        var newWindow = TestExecutor.get_activeWindow().get_activeFrame().get_jsFrame()._originalOpen(url, name, features, replace);
        TestExecutor._windowCollection.push(new LTAF.BrowserWindow(newWindow));
        return newWindow;
    },
    
    /// <summary>
    /// Method that checks whether an alert from the current frame has been consumed by the Engine.
    /// If it is, then restore the original alert on the frame.
    /// </summary>
    isPopupComplete : function()
    {
        if(TestExecutor.get_browserInfo().Data == "")
        {
            return false;
        }
        else
        {
            TestExecutor.get_activeWindow().get_activeFrame().restoreOriginalAlert();
            TestExecutor.get_activeWindow().get_activeFrame().restoreOriginalConfirm();
            return true;
        }
    },
    
    
    
    htmlEncode : function(value)
    {
        /// <summary>
        /// Utility function to HTML encode a string
        /// </summary>
        /// <param name="value" type="String">String to encode</param>
        /// <returns type="String">HTML encoded message</returns>
        
        return new String(value).replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;"); //"// Prevent bad lexers from messing up more than this line...
    },
    
    formatElement : function(element)
    {
        /// <summary>
        /// Format a DOM element for display
        /// </summary>
        /// <param name="element" type="Sys.UI.DomElement" isDomElement="true">Element to format</param>
        /// <returns type="String">Element as a string</returns>
        
        if (!element)
        {
            return 'null';
        }
        
        var result = element.tagName;
        
        if (element.tagName && element.tagName.toLowerCase() == 'input')
        {
            result += ' type="' + element.type + '"';
        }
        
        if (element.id && element.id.length > 0) {
            result += ' id="' + element.id + '"';
        }
        
        if (element.name && element.name.length > 0) {
            result += ' name="' + element.name + '"';
        }
        
        if (element.className && element.className.length > 0) {
            result += ' class="' + element.className + '"';
        }
        
        return result;
    },
    
    formatElapsedTime : function(start)
    {
        /// <summary>
        /// Create a text representation of the time elapsed since the provided value
        /// </summary>
        /// <param name="start" type="Date">Start of elapsed time</param>
        /// <returns type="String">Elapsed time as a string</returns>
        
        // Get the number of milliseconds since we started
        var now = new Date();
        var diff = now - start;
        
        // Roll back the time to the start of the day
        now.setHours(0, 0, 0, 0);
        
        // Make the time of day the elapsed time (i.e. if it took 2 hours and
        // 43 minutes, then make the time 02:43am)
        now.setTime(now.getTime() + diff);
        
        // Format the elapsed time using the existing date/time formatting
        return now.format('HH:mm:ss.ffffff');
    },
    
    clearLog : function()
    {
        /// <summary>
        /// Clear the test log before another pass is started
        /// </summary>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "";
        }
    },
    
    logTestRunStarted : function(numberOfTests)
    {
        /// <summary>
        /// Log the start of a test run
        /// </summary>
        /// <param name="numberOfTests" type="Number">Number of tests in the rest run</param>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "<hr/><div class='LogRun'>Test run started with " + numberOfTests + " test(s) at " + TestExecutor._testRunStarted.format("T") + "</div>" + tranceConsole.innerHTML;
        }
    },
    
    logTestRunFinished : function()
    {
        /// <summary>
        /// Log the end of a test run
        /// </summary>
        /// <param name="numberOfTests" type="Number">Number of tests in the rest run</param>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "<div class='LogRun'>Test run finished with " + this._numberOfTestCaseFailures + " failure(s) in " + TestExecutor.formatElapsedTime(TestExecutor._testRunStarted) + "</div><hr/>" + tranceConsole.innerHTML;
        }
    },
    
    logTestCaseStarted : function(testId)
    {
        /// <summary>
        /// Log the start of a test case
        /// </summary>
        /// <param name="testId" type="String">ID of the test case</param>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "<div class='LogTestcase'>Test '" + testId + "' started at " + TestExecutor._testCaseStarted.format("T") + "</div>" + tranceConsole.innerHTML;
        }
    },
    
    logTestCaseFinished : function(testId, passed, errorMessage)
    {
        /// <summary>
        /// Log the end of a test case
        /// </summary>
        /// <param name="testId" type="String">ID of the test case</param>
        /// <param name="passed" type="Boolean">Whether the test passed or failed</param>
        /// <param name="errorMessage" type="String">Any error associated with the test</param>

        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "<div class='LogTestcase'>Test '" + testId + "' " + (passed ? "passed" : "failed") + " in " + TestExecutor.formatElapsedTime(TestExecutor._testCaseStarted) + "</div>" + tranceConsole.innerHTML;
        } 
    },
    
    logTestCommand : function(command)
    {
        /// <summary>
        /// Log the execution of a test command
        /// </summary>
        /// <param name="command" type="LTAF.Engine.BrowserCommand">Command being executed</param>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole && command)
        {
            var traces = "";
            if(command.Traces != null && command.Traces.length > 0)
            {
                for(i = command.Traces.length - 1; i >= 0; i--)
                {
                    traces += "<div class='LogTrace'>" + command.Traces[i] + "</div>" ;
                }
            }
            tranceConsole.innerHTML = "<div class='LogCommand'>" + command.Description + "</div>" + traces + tranceConsole.innerHTML;
        }
    },
    
    logTestDetail : function(detail)
    {
        /// <summary>
        /// Log a detail associated with the command being executed
        /// </summary>
        /// <param name="detail" type="String">Detail of the command being executed</param>
        
        var tranceConsole = document.getElementById("TraceConsole");
        if (tranceConsole)
        {
            tranceConsole.innerHTML = "<div class='LogDetail'>" + detail + "</div>" + tranceConsole.innerHTML;
        }
    },
    
    getLogContent : function()
    {
        /// <summary>
        /// Get the contents of the log to transfer to the server
        /// </summary>
        /// <returns type="String">Contents of the log</returns>
       
        return null;
    }
}
LTAF.TestExecutorBase.registerClass('LTAF.TestExecutorBase');


LTAF.WebTestLogDetail = function() {
    /// <summary>
    /// The WebTestLogDetail enumeration describes the amount
    /// detail included in the test log
    /// </summary>
    /// <field name="Concise" type="Number" integer="true" />
    /// <field name="Default" type="Number" integer="true" />
    /// <field name="Verbose" type="Number" integer="true" />
    throw Error.invalidOperation();
}
LTAF.WebTestLogDetail.prototype = {
    Concise : 0,
    Default : 1,
    Verbose : 2
}
LTAF.WebTestLogDetail.registerEnum("LTAF.WebTestLogDetail", false);

function CheckCommandCompleteState()
{
    ///<summary>Holds the poller for wait for command complete</summary>    
    this.WaitForCommandCompletePoller = null;

    ///<summary>Delegate function to check if command is complete</summary>    
    this.CheckCommandCompleteDelegate = null;

    ///<summary>The delay in milliseconds to wait before polling to check if a command is complete.</summary>        
    this.CheckCommandCompleteDelay = 200;
    
    ///<summary>Keeps track of how many times the Engine has called the CheckComamndComplete function.</summary>            
    this.CheckCommandCompleteAttemps = 0;
}