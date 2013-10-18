/// --------------------------------------
/// Script Concatenated by a Tool
/// Time Stamp:10/18/2013 3:04:31 AM
/// --------------------------------------

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
LTAF.TestExecutor = function()
{
    LTAF.TestExecutor.initializeBase(this);
    
    this._threadId = null; 
}

LTAF.TestExecutor.prototype = {    
    /// <summary>
    /// Method that sets the server thread id that is processing this client
    /// </summary>
    set_threadId : function(threadId)
    {
        this._threadId = threadId;
    },
    
    hideSpinner : function()
    {
        LTAF.TestExecutor.callBaseMethod(this, "hideSpinner");        
        
        var threadId = document.getElementById('ThreadId');
        if(threadId) threadId.innerHTML = '';
    },
    
    invokeGetCommand: function(browserInfo, successDelegate, errorDelegate)
    {  
        PageMethods.GetCommand(this._threadId, browserInfo, successDelegate, errorDelegate);
    }   
}

LTAF.TestExecutor.registerClass('LTAF.TestExecutor', LTAF.TestExecutorBase); 


// initialize static var
TestExecutor = new LTAF.TestExecutor();
/// <summary>
/// This class represents a frame of a window.
/// </summary>
/// <param name="frame">A javascript window/frame object</param>
/// <change date="11/07/2006">Created</change>
LTAF.BrowserFrame = function(frame)
{
    /// <summary> The javascript frame contained in this object </summary>
    this._jsFrame = frame;
    
    // Save a reference to the original open functions for this frameame
    frame._originalOpen = frame.open;
    
    /// <summary> Reference to the original alert function of this frame</summary>    
    this._originalAlert = frame.alert;

    /// <summary> Reference to the original confirm function of this frame</summary>    
    this._originalConfirm = frame.confirm;
    
    this._childTagIndex = null;
}

LTAF.BrowserFrame.prototype = {
    get_jsFrame : function()
    {
        return this._jsFrame;
    },
    
    /// <summary>
    /// Method that restores the original alert function for this frame. 
    /// </summary>
    /// <change date="11/07/2006">Created</change>    
    restoreOriginalAlert : function()
    {
        this._jsFrame.alert = this._originalAlert;
    },
    
    /// <summary>
    /// Method that restores the original confirm function for this frame. 
    /// </summary>
    /// <change date="1/07/2008">Created</change>    
    restoreOriginalConfirm : function()
    {
        this._jsFrame.confirm = this._originalConfirm;
    },
    
    /// <summary>
    /// Method that locates an element in the DOM by its id/name
    /// </summary>
    /// <param name="id">The id/name of the dom element (string) </param>
    /// <param name="index">Zero-based index of the element (int) </param>
    /// <change date="10/12/2006">Created</change>
    findObjectById : function(id, index)
    {       
        var obj = this._jsFrame.document.getElementById(id);
        
        if(obj == null)
        {
            //if we fail, try to locate it by name
            var elements = this._jsFrame.document.getElementsByName(id);
            obj = elements[index];            
        }

        if(obj == null) 
        {       
            throw "[AjaxBrowser] ElementNotFoundException: Could not locate an element with id/name="+ id +", index=" + index;
        }
        
        return obj;
    },
    
    /// <summary>
    /// Method that locates an element in the DOM by its tagName
    /// </summary>
    /// <param name="tagName">The tag name of the dom element (string) </param>
    /// <param name="index">Zero-based index of the element (int) </param>
    /// <change date="02/07/2006">Created</change>
    findObjectByTagName : function(tagName, index, textBetweenTags)
    {
        var elements = this._jsFrame.document.getElementsByTagName(tagName);
        if(elements == null || elements.length == 0)
        {
            throw "[AjaxBrowser] ElementNotFoundException: No element with tagname='"+ tagName +"' was found in the document.";
        }
                
        if(textBetweenTags == null)
        {
            //just look by the index
            if(elements.length - 1 >= index)
            {
                return elements[index];
            }
            else
            {
                throw "[AjaxBrowser] IndexOutOfRangeException: Could not find element with tagname='"+ tagName +"' and index=" + index;            
            }
        }
        else
        {
            //Locate with TextBetweenTags (code path only for full LTAF)
            var count = -1;
            textBetweenTags = textBetweenTags.toLowerCase();
            for(i = 0; i < elements.length; i++)
            {
                if(elements[i].innerHTML.toLowerCase() == textBetweenTags)
                {
                    count++;
                    if(count == index)
                    {
                        return elements[i];
                    }
                }
            }
           
            throw "[AjaxBrowser] ElementNotFoundException: Could not find element with tagname='"+ tagName +"', textbetweentags='"+ textBetweenTags +"' and index=" + index;

        }
    },
    
    findChildObjectByTagName : function(parentElement, tagName)
    {
        for(var i=0; i<parentElement.childNodes.length; i++)
        {
            var childElement = parentElement.childNodes[i];
            if(childElement.nodeName.toLowerCase() == tagName)
            {
                if(this._childTagIndex == 0)
                {
                    // we found it, return it
                    return childElement;
                }
                else
                {
                    this._childTagIndex--;
                }
            }
            
            var grandChild = this.findChildObjectByTagName(childElement, tagName);
            if(grandChild != null)
            {
                return grandChild;
            }
        }
        return null;
    },
    
    /// <summary>
    /// Method that locates an element in the DOM within a window/frame
    /// </summary>
    /// <param name="target">Object of type LTAF.Target</param>
    /// <change date="02/07/2006">Created</change>
    /// <change date="05/16/2006">Change to call FindFrame passing the array of frame hierarchy</change>
    findObject : function(target)
    {   
        var targetElement = null;
        // now that we have the target frame, try to locate the object
        if(target.Id != null && target.Id.length > 0) 
        {
            //first try to locate the object by id
            targetElement = this.findObjectById(target.Id, target.Index);              
        }
        else if(target.TagName != null && target.TagName.length > 0)
        {
            //try to locate by tagname
            var textBetweenTags = null;
            if(target.TextBetweenTags)
            {
                textBetweenTags = target.TextBetweenTags;
            }
            targetElement = this.findObjectByTagName(target.TagName, target.Index, textBetweenTags);
        }
        else
        {
            throw "[AjaxBrowser] ElementNotFoundException: In order to locate an element either BrowserCommandTarget.Id or BrowserCommandTarget.TagName must be set";
        }

        if(target.ChildTagName != null)
        {          
            this._childTagIndex = target.ChildTagIndex;
            targetElement = this.findChildObjectByTagName(targetElement, target.ChildTagName.toLowerCase());
            if(targetElement == null)
            {
                throw "[AjaxBrowser] ElementNotFoundException: Could not locate element with tagname='"+ target.ChildTagName +"' and index='"+ target.ChildTagIndex +"' that is a child of element with tagname='"+ targetElement.nodeName +"' and id='"+ targetElement.id +"'.";
            }            
        }
        
        return targetElement;
    },
    
    /// <summary>
    /// Method that calculates the current DOM state for this frame
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    /// <change date="11/10/2006">Changed to calculate the DOM for the whole documentElement</change>
    calculateCurrentDom : function(attributesToInclude)
    {
        return DomSupport.calculateDOMForElement(this._jsFrame.document.documentElement, attributesToInclude);
    }
}

LTAF.BrowserFrame.registerClass('LTAF.BrowserFrame');LTAF.NavigationVerification = function() {
    /// <summary>
    /// How we determine when navigation to a new page has completed
    /// </summary>
    /// <field name="Default" type="Number" integer="true" />
    /// <field name="AspNetAjax" type="Number" integer="true" />
    throw Error.invalidOperation();
}
LTAF.NavigationVerification.prototype = {
    Default : 0,
    AspNetAjax : 1
}
LTAF.NavigationVerification.registerEnum("LTAF.NavigationVerification", false);

/// <summary>
/// This class represents a top level window of the browser.
/// </summary>
/// <param name="windo">A javascript window object</param>
/// <change date="02/06/2006">Created</change>
LTAF.BrowserWindow = function(window)
{
    /// <summary> The javascript window contained in this object </summary>
    this._window = window;
    
    /// <summary> The current selected frame inside this window</summary>
    this._currentFrame = new LTAF.BrowserFrame(window);
    
    /// <summary>Current NavigationVerification mode</summary>
    this._navigationVerificationMode = LTAF.NavigationVerification.Default;
}    

LTAF.BrowserWindow.prototype = {
    /// <summary>
    /// Method that returns the url of the page loaded in this window
    /// </summary>
    getCurrentUrl: function()
    {
        return this._window.location.href;
    },

    /// <summary>
    /// Method that checks whether the browser has finished navigating by
    /// inspecting if the <span> that LTAF added is still there.
    /// </summary>
    isNavigationComplete : function(waitingTime)
    {
        return TestExecutor.get_activeWindow()._isNavigationCompleteInternal(waitingTime);    
    },
    
    /// <summary>
    /// Method that checks whether the browser has finished navigating by
    /// inspecting if the <span> that LTAF added is still there.
    /// </summary>
    _isNavigationCompleteInternal : function(waitingTime)
    {    
        // Wait for
        //      1. last page to be cleared
        //      2. the document to have finished loading
        //      3. Optionally wait for the ASP.NET AJAX libraries to have loaded
        //      4. Optionally wait for any custom conditions
        
        var postBackTimeOut = TestExecutor.get_contextObject();
        if(postBackTimeOut){
			if(waitingTime > postBackTimeOut){
				TestExecutor.reportFailure('[AjaxBrowser] Navigation was not detected after ' + postBackTimeOut/1000 + " seconds.");
				return true;
			}
        }
        
        var frame = this._currentFrame.get_jsFrame();
        return (frame.document && frame.document.getElementById("Testing_WaitSpan") == null) &&
            ((!frame.document.readyState) || (frame.document.readyState == 'complete')) &&
            ((this._navigationVerificationMode != LTAF.NavigationVerification.AspNetAjax)  ||
                (frame.Sys && frame.Sys.Application && !frame.Sys.Application._initializing && frame.Sys.Application._initialized)) &&
            (!LTAF.BrowserWindow.isLoaded || LTAF.BrowserWindow.isLoaded(frame));
    },
    
    /// <summary>
    /// Method that navigates the window to a target url. It sets the location
    /// property of this window/frame
    /// </summary>
    /// <param name="url">The url string to navigate to</param>
    /// <param name="navigationVerificationMode" type="LTAF.NavigationVerification" optional="true">NavigationVerification mode</param>
    navigate : function(url, navigationVerificationMode)
    {
        this.prepareWaitForNavigationComplete();
        
        // [11/04/2007] Removed the functionality below which is needed by a toolkit testcase. Need to think how to accomodate for this
        // Add a dynamic querystring value to force a refresh of the page
        // do to a navigation quirk in Safari
        //if (DomSupport.get_navigatorName() == "safari" && url)
        //{
        //    url += ((url.indexOf('?') >= 0) ? '&' : '?') + 'forceRefresh=' + new Date();
        //}
        
        this._window.location = url;
        this._navigationVerificationMode = navigationVerificationMode || LTAF.NavigationVerification.Default;
    },
    
    
    
    prepareWaitForNavigationComplete : function()
    {
        //  since we are navigating out of here... we are going to inject a place holder
        //  on the testpage and wait until the place holder is gone. That will indicate
        //  that we have finished navigating.
    
        var newSpan = this._currentFrame.get_jsFrame().document.createElement("span");
        newSpan.id = 'Testing_WaitSpan';
        newSpan.innerHTML = '[Waiting for navigation to complete...]';
        this._currentFrame.get_jsFrame().document.body.appendChild(newSpan);
        
        TestExecutor.set_checkCommandCompleteFunction(this.isNavigationComplete);
    },

    /// <summary>
    /// Method that returns the most recent accesed frame in this window
    /// </summary>
    get_activeFrame : function()
    {
        return this._currentFrame;
    },
    
    /// <summary>
    /// Method that locates a child frame within this window
    /// </summary>
    /// <param name="frameHierarchy">Array that contains the id/name of the hierarchy of frames to locate the target frame</param>
    findFrame : function(frameHierarchy)
    {
        var jsFrame = this._window;
        if(frameHierarchy != null && frameHierarchy.length > 0)
        {
            for(i = 0; i < frameHierarchy.length; i++)
            {
                var frameId = frameHierarchy[i];
                jsFrame = jsFrame.frames[frameId];  
                if(jsFrame == null)
                {
                    throw "[AjaxBrowser] FrameNotFoundException: Could not find frame with index/name=" + frameId;
                }                  
            }
        }     
 
        if(jsFrame != this._currentFrame.get_jsFrame())
        {
            this._currentFrame = new LTAF.BrowserFrame(jsFrame);
        }
                
        return this._currentFrame; 
    }
}

// LTAF.BrowserWindow.isLoaded can be defined to perform additional
// verification that the window has completed loading.  As an example, the following
// function would be used to ensure that the Microsoft AJAX Library has fully loaded
// the scripts for a page.
//      LTAF.BrowserWindow.isLoaded = function(frame) {
//          return frame.Sys && frame.Sys.Application && !frame.Sys.Application._initializing && frame.Sys.Application._initialized;
//      };

LTAF.BrowserWindow.registerClass('LTAF.BrowserWindow');
    /// <summary>
    /// Command handler that executes a redirect within the current window
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    /// <change date="05/21/2006">Attach delegate to check command complete</change>
    function NavigateToUrl(obj, args)
    {
        var url = args[0];
        var navigationVerificationMode = args[1];
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Url: " + url);
            TestExecutor.logTestDetail("Navigation Verification: " + LTAF.NavigationVerification.toString(navigationVerificationMode));
        }
        TestExecutor.get_activeWindow().navigate(url, navigationVerificationMode);
    }
    
    /// <summary>
    /// Command handler that dispatches an html event to the given object
    /// </summary>
    /// <change date="03/20/2007">Created</change>
    function DispatchHtmlEvent(obj, args)
    {
        var evt = args[0];
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Event: " + evt);
        }
        DomSupport.invokeHtmlEventInteral(obj, evt);
    }
    
    /// <summary>
    /// Command handler that dispatches a mouse event to the given object
    /// </summary>
    /// <change date="03/20/2007">Created</change>
    function DispatchMouseEvent(obj, args)
    {
        var evt = new String(args[0]);
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Event: " + evt);
        }
        DomSupport.invokeMouseEventInternal(obj, evt);
    }
    
    /// <summary>
    /// Command handler that dispatches a key event to the given object
    /// </summary>
    function DispatchKeyEvent(obj, args)
    {
        var eventName = args[0];
        var ctrlKey = args[1];
        var altKey = args[2];
        var shiftKey = args[3];
        var metaKey = args[4];
        var keyCode = args[5];
        var charCode = args[6];
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Event: " + eventName);
            TestExecutor.logTestDetail("Char Code: " + charCode);
            TestExecutor.logTestDetail("Key Code: " + charCode);
            TestExecutor.logTestDetail("Ctrl Key: " + ctrlKey);
            TestExecutor.logTestDetail("Alt Key: " + altKey);
            TestExecutor.logTestDetail("Shift Key: " + shiftKey);
            TestExecutor.logTestDetail("MetaKey: " + metaKey);
        }
        DomSupport.invokeKeyEventInternal(obj, eventName, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode);
    }
    
    /// <summary>
    /// Command handler that sets a textbox to a specific value
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    function SetTextBox(obj,args)
    {
        var value = args[0];
        var focusAndBlur = args[1];
        var currentNavigator = DomSupport.get_navigatorName();
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Value: " + value);
            TestExecutor.logTestDetail("Focus and Blur: " + focusAndBlur);
        }
        
        if (focusAndBlur)
        {
            DomSupport.invokeHtmlEventInteral(obj, "focus");
        }
        
        obj.value = value;
        DomSupport.invokeHtmlEventInteral(obj, "change");
        
        if (focusAndBlur)
        {
            DomSupport.invokeHtmlEventInteral(obj, "blur");
        }
    }

    /// <summary>
    /// Command handler that clicks an HTML element tag in the DOM (span, div)
    /// </summary>
    /// <change date="02/13/2006">Created</change>
    function ClickElement(obj, args)
    {
        if (obj == null)
        {
            TestExecutor.reportFailure("[AjaxBrowser] Target of the click not found.");
            return;
        }
        
        var wait = args[0];
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Wait For Postback: " + wait);
        }
        
        if (wait)
        {
            // this action will cause a post we are going to prepare the window for navigate
            TestExecutor.get_activeWindow().prepareWaitForNavigationComplete();
            if(args[1])
            {
                TestExecutor.set_contextObject(args[1]);
            }    
        }        
        DomSupport.clickObject(obj);
    }
    
    /// <summary>
    /// Command handler that clicks an HTML control in the DOM (button, checkbox, radiobutton, link)
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    function ClickControl(obj, args)
    {
        if (obj == null)
        {
            TestExecutor.reportFailure("[AjaxBrowser] Target of the click not found.");
            return;
        }
        
        var currentNavigator = DomSupport.get_navigatorName();
        if (currentNavigator == "safari")
        {
            //safari does not trigger focus/blur/change.
            ClickElement(obj, args);
        }
        else
        {
            if (TestExecutor.verbose)
            {
                TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            }
        
            DomSupport.invokeHtmlEventInteral(obj, "focus");
            // [01/19/2007] FireFox and Opera trigger the onchange event just by dispatching a click
            DomSupport.clickObject(obj);
            DomSupport.invokeHtmlEventInteral(obj, "blur");
        }
    }
    
    /// <summary>
    /// Command handler that selects an element in a list 
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    function SetSelectBoxIndex(obj, args)
    {
        var index = parseInt(args[0]);
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Index: " + index);
        }
        
        DomSupport.invokeHtmlEventInteral(obj, "focus");
        obj.selectedIndex = index;
        DomSupport.invokeHtmlEventInteral(obj, "change");
        DomSupport.invokeHtmlEventInteral(obj, "blur");
    }
    
    /// <summary>
    /// Command handler that calculates the page dom, and prepares to send it back on command finish
    /// </summary>
    /// <change date="02/07/2006">Created</change>
    function GetPageDom(obj, args)
    {
        var attributesToInclude = null;
        if (args && args[0])
        {
            attributesToInclude = args[0];
        }
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Attributes to include: " + attributesToInclude);
        }
        
        var pageDom = TestExecutor.get_activeWindow().get_activeFrame().calculateCurrentDom(attributesToInclude);
        TestExecutor.get_browserInfo().Data = pageDom;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Page DOM: " + pageDom);
        }
    }
    
    /// <summary>
    /// Command handler that calculates the dom for a particular element, and prepares to send it back on command finish.
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    function GetElementDom(obj, args)
    {
        var attributesToInclude = null;
        if (args && args[0])
        {
            attributesToInclude = args[0];
        }
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Attributes to include: " + attributesToInclude);
        }
        
        var elementDom = DomSupport.calculateDOMForElement(obj, attributesToInclude);
        TestExecutor.get_browserInfo().Data = elementDom;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Element DOM: " + elementDom);
        }
    }
    
    /// <summary>
    /// Command handler that calculates the attribute list for a particular element
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    function GetElementAttributes(obj, args)
    {
        var attributes = DomSupport.calculateAttributesForElement(obj);
        TestExecutor.get_browserInfo().Data = attributes;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Attributes: " + attributes);
        }
    }
    
    
    /// <summary>
    /// Command handler that calculates innerText for a particular element
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    function GetElementInnerText(obj, args)
    {
        var innerText = DomSupport.calculateInnerTextForElement(obj);
        TestExecutor.get_browserInfo().Data =  innerText;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("InnerText: " + innerText);
        }
    }
    
    /// <summary>
    /// Command handler that calculates innerText recursively for a particular element
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    function GetElementInnerTextRecursive(obj, args)
    {
        var innerText = DomSupport.calculateNativeInnerTextForElement(obj);
        TestExecutor.get_browserInfo().Data =  innerText;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("InnerTextRecursive: " + innerText);
        }
    }
    
    /// <summary>
    /// Command handler that calculates innerHTML for a particular element
    /// </summary>
    /// <change date="11/09/2007">Created</change>
    function GetElementInnerHtml(obj, args)
    {
        var innerHtml = obj.innerHTML;
        
        TestExecutor.get_browserInfo().Data =  innerHtml;
    
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("innerHtml: " + innerHtml);
        }
    }
    
    /// <summary>
    /// Command handler that waits until a property on a given DOM control has changed to expected value
    /// </summary>
    /// <remarks>
    /// This is a special command because the target object might be created dynamically
    /// </remarks>
    /// <change date="05/19/2006">Created</change>
    /// <change date="01/10/2007">Attempt to veify Dom value immediately. If fails, then poll</change>
    function WaitForDomChange(obj, args)
    {
        var attribute = args[0];
        var expectedValue = args[1];
        var timeout = args[2];
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Attribute: " + attribute);
            TestExecutor.logTestDetail("Expected Value: " + expectedValue);
            TestExecutor.logTestDetail("Timeout: " + timeout);
        }
        
        // store the arguments array in the dom's context object
        TestExecutor.set_contextObject(args);
        if (!DomSupport.verifyDomAttribute(0))
        {
            // if the attribute has not changed, then we are going to set the delegate and poll for it
            TestExecutor.set_checkCommandCompleteFunction(DomSupport.verifyDomAttribute);
        }
    }
    
    /// <summary>
    /// Command handler that waits until an element can no longer be found in dom
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    function WaitUntilDissapears(obj, args)
    {
        var timeout = args[0];
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Timeout: " + timeout);
        }
        
        // store the arguments array in the dom's context object
        TestExecutor.set_contextObject(args);
        if (!DomSupport.verifyElementIsNull(0))
        {
            // if the attribute has not changed, then we are going to set the delegate and poll for it
            TestExecutor.set_checkCommandCompleteFunction(DomSupport.verifyElementIsNull);
        }
    }
    
    /// <summary>
    /// Command handler that evaluates any custom script in the context of the test frame
    /// </summary>
    /// <change date="06/18/2007">Created</change>
    function ExecuteScript(obj, args)
    {
        var script = args[0];

        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Script: " + script);
        }
        
        var result = TestExecutor.serializeAsJson(DomSupport.evalInActiveFrame(script));
        TestExecutor.get_browserInfo().Data = result;
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Result: " + result);
        }
    }
    
    /// <summary>
    /// Command handler that waits until a custom script evaluated in the context of the test frame returns true
    /// </summary>
    /// <change date="06/18/2007">Created</change>
    function WaitForScript(obj, args)
    {
        var script = args[0];
        var timeout = args[1];
        
        if (TestExecutor.verbose)
        {
            TestExecutor.logTestDetail("Target: " + TestExecutor.formatElement(obj));
            TestExecutor.logTestDetail("Script: " + script);
            TestExecutor.logTestDetail("Timeout: " + timeout);
        }
        
        TestExecutor.set_contextObject(args);
        if (!DomSupport.isCustomScriptTrue(0))
        {
            // if the custom script does not return true, then we are going to set the delegate and poll for it
            TestExecutor.set_checkCommandCompleteFunction(DomSupport.isCustomScriptTrue);
        }
    }
    
    /// <summary>
    /// Command handler that returns the url of the page loaded in the test frame
    /// </summary>
    function GetCurrentUrl(obj, args)
    {
        var url = TestExecutor.get_activeWindow().getCurrentUrl();                
        TestExecutor.get_browserInfo().Data = url;        
    }
    
    function TestcaseExecuting(obj, args)
    {
        var testId = args[0];
        
        //call the callback handler of the page so that it can update treeview
        if (TestExecutor.TestcaseExecutingCallback != null)
        {
            TestExecutor.TestcaseExecutingCallback(testId);
        }
    }
    
    function TestcaseExecuted(obj, args)
    {
        var testId = args[0];
        var passed = args[1];
        var errorMessage = args[2];
        
        //call the callback handler of the page so that it can update treeview
        if(TestExecutor.TestcaseExecutedCallback != null)
        {
            TestExecutor.TestcaseExecutedCallback(testId, passed, errorMessage);
        }
    }
    
    function TestRunStarted(obj, args)
    {
        var numberOfTestCases = args[0];
        // hide buttons
        document.getElementById("failedTests").disabled = true;
        document.getElementById("WriteLogCheckBox").disabled = true;
        document.getElementById("ShowConsoleCheckBox").disabled = true;
        document.getElementById("RunTestcasesButton").disabled = true;
        //clear logs
        errorLogs = new Array();
		pageSources = new Array();
		failedTestIds = new Array();
    }
    
    function TestRunFinished(obj, args)
    {
        // stop the engine
        TestExecutor.stop();
        //make sure all the buttons are set back to enable
        document.getElementById("WriteLogCheckBox").disabled = false;
        document.getElementById("ShowConsoleCheckBox").disabled = false;
        document.getElementById("RunTestcasesButton").disabled = false;
        // if there are errors show the rerun button
        if(failedTestIds.length > 0){
			document.getElementById("failedTests").disabled = false;
        }
        
    }// static dom support
var DomSupport = null;

/// <summary>
/// Class that contains several support functions geared towards DOM manipulation
/// </summary>
/// <change date="05/22/2006">Transformed from separate functions to a class</change>
LTAF.DomSupport = function()
{   
    /// <summary>Private variable to store the navigator name</summary>    
    this._navigatorName = null;

    /// <summary>List of excluded attributes.</summary>    
    this._listOfExcludeProps = null;

    /// <summary>String used to indicate to include all attribute when calculating the dom.</summary>    
    this._includeAllAttributesConst = "all_attributes";
}

LTAF.DomSupport.prototype = {
    /// <summary> 
    /// Method that returns the navigator name
    /// </summary>
    /// <change date="06/29/2006">Created</change>
    /// <change date="08/17/2006">Add support for IE</change>
    get_navigatorName : function()
    {
        if(this._navigatorName == null)
        {
            if(navigator.userAgent.indexOf("Firefox") >= 0)
            {
                this._navigatorName = "firefox";
            }
            else if(navigator.userAgent.indexOf("MSIE") >= 0)
            {
                this._navigatorName = "ie";
            }
            else if(navigator.userAgent.indexOf("Safari") >= 0)
            {
                this._navigatorName = "safari";
            }
            else if(navigator.userAgent.indexOf("Opera") >= 0)
            {
                this._navigatorName = "opera";
            }
        }
        return this._navigatorName;
    },
    
    /// <summary>
    /// Function that dispatches an HtmlEvent to a DOM Object
    /// </summary>
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <change date="02/07/2006">Created</change>
    invokeHtmlEventInteral : function(object, eventName)
    {
        this.invokeHtmlEventInteralWithArgs(object, eventName, false, true);    
    },
    
    /// <summary>
    /// Function that dispatches an HtmlEvent to a DOM Object
    /// </summary>
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <param name="canBubble">Wheter this event bubles up the DOM chain (bool)</param>
    /// <param name="cancelable">Wheter this event is cancelable by any element in DOM chain (bool)</param>
    /// <change date="02/07/2006">Created</change>
    invokeHtmlEventInteralWithArgs : function(object, eventName, canBubble, cancelable)
    {
	    if (object.fireEvent) 
        {
            object.fireEvent('on' + eventName);
        }
        else
        {
            //[10/16/2006] This works for FireFox and Safari
            var theEvent = document.createEvent('HTMLEvents');
            theEvent.initEvent(eventName, canBubble, cancelable);
            object.dispatchEvent(theEvent);
        }
    },
    
    /// <summary>
    /// Function that dispatches an KeyboardEvent to a DOM Object
    /// </summary>  
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <param name="ctrlKey">Whether the Control key is down when dispatching the event (bool)</param>
    /// <param name="altKey">Whether the Alt key is down when dispatching the event (bool)</param>
    /// <param name="shiftKey">Whether the Shift key is down when dispatching the event (bool)</param>
    /// <param name="metaKey">Whether the Meta key is down when dispatching the event (bool)</param>
    /// <param name="keyCode">The key code to send (int)</param>
    /// <param name="charCode">The char code to send (int)</param>
    /// <change date="02/07/2006">Created</change>
    invokeKeyEventInternal : function(object, eventName, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode)
    {
        this.invokeKeyEventInternalWithArgs(object, eventName, true, true, document.defaultView, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode)   
    },
    
    /// <summary>
    /// Function that dispatches an KeyboardEvent to a DOM Object
    /// </summary>  
    /// <remarks>
    /// You can use the following code to create a charCode: "(new String('f')).charCodeAt(0)"
    /// </remarks>
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <param name="canBubble">Whether this event bubles up the DOM chain (bool)</param>
    /// <param name="cancelable">Whether this event is cancelable by any element in DOM chain (bool)</param>
    /// <param name="view">The view (window) upon which to trigger the event (AbstractView)</param>
    /// <param name="ctrlKey">Whether the Control key is down when dispatching the event (bool)</param>
    /// <param name="altKey">Whether the Alt key is down when dispatching the event (bool)</param>
    /// <param name="shiftKey">Whether the Shift key is down when dispatching the event (bool)</param>
    /// <param name="metaKey">Whether the Meta key is down when dispatching the event (bool)</param>
    /// <param name="keyCode">The key code to send (int)</param>
    /// <param name="charCode">The char code to send (int)</param>
    /// <change date="02/07/2006">Created</change>
    invokeKeyEventInternalWithArgs : function(object, eventName, canBubble, cancelable, view, 
        ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode)
    {
	    if (document.createEvent) 
        {
            var theEvent = document.createEvent('KeyboardEvent');
            theEvent.initKeyEvent(eventName, canBubble, cancelable, view, ctrlKey, altKey, shiftKey, metaKey, keyCode, charCode);
            object.dispatchEvent(theEvent);
        }
        else if (document.createEventObject)
        {
            var name = 'on' + eventName;
            var theEvent = parent.testFrame.document.createEventObject();
            // Ignore cancelable, view, metaKey, and keyCode
            theEvent.cancelBubble = !canBubble;
            theEvent.ctrlKey = ctrlKey;
            theEvent.altKey = altKey;
            theEvent.shiftKey = shiftKey;
            theEvent.keyCode = charCode || keyCode;
            theEvent.type = name;
            object.fireEvent(name, theEvent);
        }
        else
        {
            //[10/16/2006] Safari currently does not support additional properties when dispatching key event
            if (object['on' + eventName])
            {
                object['on' + eventName]();
            }
        }
    },
    
    /// <summary>
    /// Function that dispatches a MouseEvent to a DOM Object
    /// </summary>
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <change date="02/07/2006">Created</change>
    invokeMouseEventInternal : function(object, eventName)
    {
        return this.invokeMouseEventInternalWithArgs(object, eventName, true, true, document.defaultView, 1, 0, 0, 0, 0, false, false, false, false, 0, null);    
    },
    
    /// <summary>
    /// Function that dispatches a MouseEvent to a DOM Object
    /// </summary>
    /// <param name="object">The dom element that will receive the event</param>
    /// <param name="eventName">The name of the event to dispatch (string)</param>
    /// <param name="canBubble">Wheter this event bubles up the DOM chain (bool)</param>
    /// <param name="cancelable">Wheter this event is cancelable by any element in DOM chain (bool)</param>
    /// <param name="view">The view (window) upon which to trigger the event (AbstractView)</param>
    /// <param name="detail">Specifies the Event's mouse click count(long)</param>
    /// <param name="screenX">Specifies the event's screen x coordinate (long)</param>
    /// <param name="screenY">Specifies the event's screen y coordinate (long)</param>
    /// <param name="clientX">Specifies the event's client x coordinate (long)</param>
    /// <param name="clientY">Specifies the event's client y coordinate (long)</param>
    /// <param name="ctrlKey">Whether the Control key is down when dispatching the event (bool)</param>
    /// <param name="altKey">Whether the Alt key is down when dispatching the event (bool)</param>
    /// <param name="shiftKey">Whether the Shift key is down when dispatching the event (bool)</param>
    /// <param name="metKey">Whether the Meta key is down when dispatching the event (bool)</param>
    /// <param name="button">Specifies which of the mouse buttons was pressed (short)</param>
    /// <param name="relatedTarget">Specifies the event's related EventTarget (EventTarget)</param>
    /// <change date="02/07/2006">Created</change>
    /// <change date="01/18/2006">Ported code from AtlasUnit framework</change>
    invokeMouseEventInternalWithArgs : function(object, eventName, canBubble, cancelable, view, detail, 
        screenX, screenY, clientX, clientY, ctrlKey, altKey, shiftKey, metKey, button, relatedTarget)
    {
        var evt;
        if (object.fireEvent) 
        {
            // IE supports fireEvent()
            evt = parent.testFrame.document.createEventObject();
            evt.cancelBubble = false;
            evt.offsetX = 0;
            evt.offsetY = 0;
            evt.screenX = screenX;
            evt.screenY = screenY;
            evt.clientX = clientX;
            evt.clientY = clientY;
            evt.ctrlKey = ctrlKey;
            evt.altKey = altKey;
            evt.shiftKey = shiftKey;
            evt.metaKey = metKey;
            evt.button = button;
            
            object.fireEvent("on" + eventName, evt);
        }
        else if (object.dispatchEvent) 
        {
            // Firefox, Opera and Safari support dispatchEvent()
            evt = document.createEvent("MouseEvents");
            if (evt.initMouseEvent) 
            {
                // Firefox and Opera support initMouseEvent()
                evt.initMouseEvent(
                    eventName, 
                    canBubble, 
                    cancelable, 
                    view, 
                    detail, 
                    screenX, 
                    screenY, 
                    clientX, 
                    clientY, 
                    ctrlKey, 
                    altKey, 
                    shiftKey, 
                    metKey, 
                    button, 
                    relatedTarget
                );
            }
            else if (evt.initEvent) 
            {
                // Safari supports initEvent(), but not initMouseEvent()
                evt.initEvent(
                    eventName, 
                    canBubble,
                    cancelable 
                );
            }
            else
            {
                TestExecutor.reportFailure('[AjaxBrowser] target object does not support event firing.');
            }
            
            /*
                [05/23/2007] Return value of dispatchEvent (from http://www.w3.org/TR/DOM-Level-2-Events/events.html)
                The return value of dispatchEvent indicates whether any of the listeners which handled the event called preventDefault. 
                If preventDefault was called the value is false, else the value is true.
            */
            var canceled = !object.dispatchEvent(evt);
            return canceled;
        }
        else
        {
            TestExecutor.reportFailure('[AjaxBrowser] target object does not support event firing.');
        }
    },
    
    /// <summary>
    /// Method that handles the action of clicking on any HTML Object
    /// </summary>
    /// <change date="06/29/2006">Created</change>
    /// <change date="10/13/2006">Safari dispatch of events do not work properly, calling click() directly</change>
    clickObject : function(obj)
    {
	    if(obj.click && (this.get_navigatorName() == "ie" || this.get_navigatorName() == "safari" || this.get_navigatorName() == "opera"))
	    {
            /* [05/23/2007] If the object supports the click method, we are just going to call it directly
                This includes:
                    - All objects of IE
                    - All objects of Opera
                    - All objects of Safari EXCEPT anchors.
                    - No objects for FireFox (its correct method is to dispatch events).
            */
            obj.click();
            return;
        }
        
        var canceled = DomSupport.invokeMouseEventInternal(obj,"click");    
        /*
            [05/23/2007] Read this for the official way that event cancelation works: http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-flow-cancelation
                Essentially, we dispatch a click event to a link, if none of the listeners canceled the event
                then we have to run the browser's default action, which is to follow the href.
                
                We don't need to do this for Safari, because that browser automatically triggers the HREF (the default browser behaviour) when 
                dispatching the click event. Note that this only applies for version 2.0.4 (419.3) or higher.
        */  
        
        if(!canceled && obj.href && this.get_navigatorName() == "firefox")
        {   
            if(obj.href.indexOf("javascript") > -1)
            {
                TestExecutor.get_activeWindow().get_activeFrame().get_jsFrame().location = obj.href;                
            }
            else
            {
                // [01/18/2007] href is meant to perform a navigate. We need to
                //      call our Navigate method so that we can wait for it to complete
                if(obj.target)
                {
                   var frameName = new Array(obj.target);
                   var targetFrame = TestExecutor.get_activeWindow().findFrame(frameName).get_jsFrame();
    	           targetFrame.location = obj.href;
                }
                else
                {
                    TestExecutor.get_activeWindow().navigate(obj.href);                 
                }               
            }
        }
    },
    
    /// <summary>
    /// Function that checks if a custom script evaluates to true
    /// </summary>
    /// <remarks>This is meant to be used as a delegate, it uses the ContextObject to retrieve information</remarks>
    /// <param name="waitingTime">The amount of time in milliseconds that the Engine has been waiting for this function to return true</param>
    /// <change date="06/18/2007">Created</change>
    isCustomScriptTrue : function(waitingTime)
    {   
        if(waitingTime == null)
        {
            TestExecutor.reportFailure("[AjaxBrowser] DomSupport.isCustomScriptTrue was called with null argument");
            return true;
        }
        
        var scriptExpression = TestExecutor.get_contextObject()[0];
        var timeout = TestExecutor.get_contextObject()[1];    
        
        var result = DomSupport.evalInActiveFrame(scriptExpression);
        
        if(result)
        {
            return true;
        }
        
        //[01/10/2007] An important assumption here is that the 'timeout' is in milliseconds
        //      and that this delegate is called by the engine with the amount of milliseconds that it has been waiting.
        if(waitingTime < timeout)
        {
            return false;
        }
        else
        {
            //we have timed out, return true to break loop but log error
            var errorMessage = "[AjaxBrowser] Timed out waiting for custom script expression to return true.  timeout='" + timeout + "ms'.";            
            TestExecutor.reportFailure(errorMessage);
            return true;
        }       
    },
    
    /// <summary>
    /// Evaluate an expression in the context of the actively running test frame
    /// </summary>
    /// <param name="expression">Expression to evaluate</param>
    evalInActiveFrame : function(expression) {
        // Frame containing the running test (where the script must be evaluated)
        var frame = TestExecutor.get_activeWindow().get_activeFrame().get_jsFrame();
        
        // Safari doesn't support changing the context of the eval function,
        // so it has to be done specially
        if ("safari" != DomSupport.get_navigatorName())
        {
           return frame.eval(expression);
        }
        else
        {
            // Inject a custom function into the test frame that will evaluate
            // the script in the correct context.
            if (!frame.__safariEval)
            {
                var scriptElement = frame.document.createElement("script");
                scriptElement.type = 'text/javascript';
                scriptElement.innerHTML = "function __safariEval(s) { return eval(s); }";
                frame.document.getElementsByTagName('HEAD')[0].appendChild(scriptElement);
            }
            return frame.__safariEval(expression);
        }
    },
    
    /// <summary>
    /// Function that checks if the element no longer exists on the dom
    /// </summary>
    /// <remarks>This is meant to be used as a delegate, it uses the ContextObject to retrieve information</remarks>
    /// <param name="waitingTime">The amount of time in milliseconds that the Engine has been waiting for this function to return true</param>
    /// <change date="04/04/2007">Created</change>
    verifyElementIsNull : function(waitingTime)
    {
        if(waitingTime == null)
        {
            TestExecutor.reportFailure("[AjaxBrowser] DomSupport.verifyElementIsNull was called with null argument");
            return true;
        }
        
        var timeout = TestExecutor.get_contextObject()[0];
        var innerText = TestExecutor.get_contextObject()[1];
        
        var command = TestExecutor.get_lastCommand();
        var currentFrame = TestExecutor.get_activeWindow().get_activeFrame();
        var element = null;
        
        try
        {
            element = currentFrame.findObject(command.Target)
        }
        catch(e)
        {
            return true;
        }
        
        if(innerText && element.innerHTML.trim() != innerText.trim())
        {
            // if the element is found, but it has the wrong innerText, then the wait is over
            return true;
        }
        
        
        //[01/10/2007] An important assumption here is that the 'timeout' is in milliseconds
        //      and that this delegate is called by the engine with the amount of milliseconds that it has been waiting.
        if(waitingTime < timeout)
        {
            return false;
        }
        else
        {
            //we have timed out, return true to break loop but log error
            var errorMessage = "[AjaxBrowser] Timed out waiting for Element to dissapear Change.  timeout='" + timeout + "ms'.";            
            TestExecutor.reportFailure(errorMessage);
            return true;
        }          
    },
    
    /// <summary>
    /// Function that checks the value of an element's attribute 
    /// </summary>
    /// <remarks>This is meant to be used as a delegate, it uses the ContextObject to retrieve information</remarks>
    /// <param name="waitingTime">The amount of time in milliseconds that the Engine has been waiting for this function to return true</param>
    /// <change date="05/22/2006">Created</change>
    verifyDomAttribute : function(waitingTime)
    {
        if(waitingTime == null)
        {
            TestExecutor.reportFailure("[AjaxBrowser] DomSupport.verifyDomAttribute was called with null argument");
            return true;
        }
        
        var attrName = TestExecutor.get_contextObject()[0];
        var expectedAttrValue = TestExecutor.get_contextObject()[1];
        var timeout = TestExecutor.get_contextObject()[2];
        
        var command = TestExecutor.get_lastCommand();
        var currentFrame = TestExecutor.get_activeWindow().get_activeFrame();
        var element = null;
        var actualAttrValue = null
        
        try
        {
            element = currentFrame.findObject(command.Target)
        }
        catch(e)
        {
            element = null;
        }
    
        if(element != null)
        {
            actualAttrValue = element[attrName];
            if(actualAttrValue != null && actualAttrValue == expectedAttrValue)
            {
                return true;
            }
        }
        
        //[01/10/2007] An important assumption here is that the 'timeout' is in milliseconds
        //      and that this delegate is called by the engine with the amount of milliseconds that it has been waiting.
        if(waitingTime < timeout)
        {
            return false;
        }
        else
        {
            //we have timed out, return true to break loop but log error
            var errorMessage = "[AjaxBrowser] Timed out waiting for DOM Change. Attribute='"+ attrName +"'; ExpectedValue='"+ expectedAttrValue +"'; timeout='" + timeout + "ms'.";
            if(element == null)
            {
                errorMessage += "The requested element was not found on the page.";
            }
            else
            {
                errorMessage += "Requested element was found on the page but had its property set to '" + actualAttrValue + "'.";
            }
            
            TestExecutor.reportFailure(errorMessage);
            return true;
        }
    },
    
    /// <summary>
    /// Function that generates the innertext of this element
    /// </summary>
    /// <param name="obj">The element for which the innertext needs to be generated</param>
    /// <change date="11/09/2007">Created</change>
    calculateInnerTextForElement: function(obj)
    {
        var textBuilder = new Sys.StringBuilder();
        var numberOfChildNodes = obj.childNodes.length;
        if(numberOfChildNodes == 0)
        {
            return this.calculateNativeInnerTextForElement(obj);
        }
        
        for(var i = 0; i < numberOfChildNodes; i++) 
        {
            var childNode = obj.childNodes[i];
            if (childNode.nodeName.toLowerCase() == '#text') 
            {
                textBuilder.append(childNode.data);
            }
        }
        return textBuilder.toString();
    },
    
    /// <summary>
    /// Function that generates the innertext recursively for this element
    /// </summary>
    /// <change date="04/04/2007">Created</change>
    calculateNativeInnerTextForElement: function(obj)
    {
        var innerText = obj.innerText;
        if (this.get_navigatorName() == "firefox")
        {
            innerText = obj.textContent;
        }
        else if (innerText == "" && this.get_navigatorName() == "safari")
        {
            innerText = obj.text;
        }
        
        return innerText;
    },
    
    /// <summary>
    /// Recursive function that generates a string representation of the current DOM for an element
    /// </summary>
    /// <param name="obj">The element for which DOM need to be found out</param>
    /// <change date="02/07/2006">Created</change>
    /// <change date="07/22/2006">Fix to always write the closing tags (instead of selfclosing).
    ///         Otherwise the log won't show the rendering of the page.</change>
    calculateDOMForElementWithBuilder : function(obj, domBuilder, attributesToInclude) {   
        if(obj == null) 
        {
            domBuilder.append('ERROR: OBJECT UNDEFINED');
        }
        else if (obj.nodeName.toLowerCase() == '#text') 
        {
            domBuilder.append(obj.data);
        }
        else if(obj.nodeName.toLowerCase() == 'br') 
        {
            domBuilder.append("<BR />");
        }
        else if(obj.nodeName.toLowerCase() == 'script'
            || obj.nodeName.toLowerCase() == '#comment')
        {
            // [01/26/2009] Scripts and comments are stripped out of the DOM
            return;
        }
        else 
        {
            domBuilder.append('<');
            domBuilder.append(obj.nodeName);

            if(attributesToInclude == this._includeAllAttributesConst)
            {
                // write out all attributes for object
                this.getAttributesForObject(obj, domBuilder, this._includeAllAttributesConst);                
            }
            else
            {
                // always write out the id or name attributes at least
                if(obj.id)
                {
                    domBuilder.append(" id=\""+ obj.id +"\"");
                }
                else if(obj.name)
                {
                    domBuilder.append(" name=\""+ obj.name +"\"");                
                }
                
                // if there is any additional attributes to include write them out
                if(attributesToInclude)
                {
                    this.getAttributesForObject(obj, domBuilder, attributesToInclude);
                }
            }
            

            domBuilder.append(">");
    
            //add its child nodes
            var numberOfChildNodes = obj.childNodes.length;
            for(var i = 0; i < numberOfChildNodes; i++) 
            {
                this.calculateDOMForElementWithBuilder(obj.childNodes[i], domBuilder, attributesToInclude);
            }
            
            domBuilder.append("</");
            domBuilder.append(obj.nodeName);
            domBuilder.append(">");
        }
    },
    
    
    /// <summary>
    /// Recursive function that generates a string representation of the current DOM for an element
    /// </summary>
    /// <param name="obj">The element for which DOM need to be found out</param>
    /// <change date="02/07/2006">Created</change>
    /// <change date="07/22/2006">Fix to always write the closing tags (instead of selfclosing).
    ///         Otherwise the log won't show the rendering of the page.</change>
    calculateDOMForElement : function(obj, attributesToInclude) {
        var domBuilder = new Sys.StringBuilder();
        
        if (this._listOfExcludeProps == null ) {
            this._listOfExcludeProps = new LTAF.ExcludePropertyList();
        }
        
        this.calculateDOMForElementWithBuilder(obj, domBuilder, attributesToInclude);
        return domBuilder.toString();
    },
    
    
    /// <summary>
    /// Function that generates a string representation of the dom element with all its attributes (does not include children)
    /// </summary>
    calculateAttributesForElement : function(obj) {
        var domBuilder = new Sys.StringBuilder();
        
        if (this._listOfExcludeProps == null ) {
            this._listOfExcludeProps = new LTAF.ExcludePropertyList();
        }
        
        domBuilder.append('<');
        domBuilder.append(obj.nodeName);
        this.getAttributesForObject(obj, domBuilder, this._includeAllAttributesConst);
        domBuilder.append(" />");
        
        return domBuilder.toString();
    },
    
    /// <summary>
    /// Function that returns whether the attribute needs to be retrieved in a special manner
    /// </summary>
    isSpecialDomAttribute : function(attributeName) 
    {
        var specialDomAttributes = ["style", "selectedindex", "value", "checked"];
        for(var i = 0; i < specialDomAttributes.length; i++) {
            if(specialDomAttributes[i] == attributeName.toLowerCase()) {
                return true;
            }
        }
        return false;
    },
    
    
    /// <summary>
    /// Function that adds specific attributes that need to be retrieved in a special manner
    /// </summary>
    addSpecialDomAttributes : function(obj, domBuilder, attributesToInclude) 
    {
        // style
        if(attributesToInclude == this._includeAllAttributesConst || attributesToInclude.indexOf("style") > 0)
        {
            // if attributesToInclude is _includeAllAttributes then the caller wants all attributes
            //   otherwise, caller must have specifically requested the "style" attribute
            if(obj.style != null && obj.style.cssText != null) {
                this.appendAttribute(domBuilder, "style", obj.style.cssText);
            }
        }
        
        // selectedIndex
        if(attributesToInclude == this._includeAllAttributesConst || attributesToInclude.indexOf("selectedindex") > 0)
        {
            if(obj.selectedIndex != null) {
                this.appendAttribute(domBuilder, "selectedIndex", obj.selectedIndex);
            }
        }
        
        // [07/282008] When calling obj.attribtes; FireFox fails to get the value attribute from an 
        //              element if it was set via javascript
        if(attributesToInclude == this._includeAllAttributesConst || attributesToInclude.indexOf("value") >= 0)
        {
            if(obj.value != null) {
                this.appendAttribute(domBuilder, "value", obj.value);
            }
        }
        
        if(attributesToInclude == this._includeAllAttributesConst || attributesToInclude.indexOf("checked") >= 0)
        {
            if(obj.checked != null) {
                this.appendAttribute(domBuilder, "checked", obj.checked);
            }
        }
    },
    
    /// <summary>
    /// Function that generates a string containing all the attribute/value pairs for a given DOM element
    /// </summary>
    /// <param name="obj">The dom element whoes attributes needed</param>
    /// <change date="02/07/2006">Created</change>
    /// <change date="09/08/2006">Exclude properties with value -1 (needs revision)</change>
    getAttributesForObject : function(obj, domBuilder, attributesToInclude)
    {
        this.addSpecialDomAttributes(obj, domBuilder, attributesToInclude);

        if(obj.attributes.length > 0)
        {
            for(var i = 0; i < obj.attributes.length; i++)
            { 
                var attr = obj.attributes[i];
                
                if(this.isSpecialDomAttribute(attr.nodeName)) {
                    continue;
                }
                
                if(attributesToInclude
                    && attributesToInclude != this._includeAllAttributesConst
                    && attributesToInclude.indexOf(attr.nodeName.toLowerCase()) < 0)
                {
                    continue;
                }
                
                if(attr.nodeValue != "" 
                    && attr.nodeValue != null)
                {
                    this.appendAttribute(domBuilder, attr.nodeName, attr.nodeValue);
                }
            }
        }
    },
    
    /// <summary>
    /// Function that appends the attribute/value pair to the builder.
    /// </summary>
    appendAttribute : function(domBuilder, attributeName, attributeValue) 
    {
		var stringToAppend = "";
		var stringValue = new String(attributeValue)
		var hasDoubleQuotes = stringValue.indexOf("\"");
		var hasSingleQuotes = stringValue.indexOf("\'");
		
		if( hasDoubleQuotes >= 0 && hasSingleQuotes >= 0){
			stringToAppend = " " + attributeName + "=\'" + stringValue.replace(/'/g,"&apos;") +"\'";
		}
		else if(hasDoubleQuotes >= 0){
			stringToAppend = " " + attributeName + "=\'" + attributeValue + "\'";
		}
		else{
			stringToAppend = " " + attributeName + "=\"" + attributeValue +"\"";
		}
		
		domBuilder.append(stringToAppend); 
    }
    
}

LTAF.DomSupport.registerClass('LTAF.DomSupport');
  
DomSupport = new LTAF.DomSupport();
    
    
/// <summary>
/// Function that contains a string of all attribute that are not needed in DOM
/// </summary>
/// <change date="03/20/2006">Created</change>
/// <change date="06/09/2006">Excluded more properties to increase perf.</change>
/// <change date="06/09/2006">Excluded "XML" attribute which is added by FireFox, HUGE perf hit</change>
/// <change date="10/31/2006">Excluded "ContentEditable" attribute which is added by Safari</change>
LTAF.ExcludePropertyList = function()
{
    ///<summary>string containing delimited list of property name which should not be included in dom</summary>
    var _excludePropertyList = null;
    
    //initialize the _excludePropertyList
    //To add new property to be excluded then make sure its listed in UPPERCASE and prefix by '-'
    
    this._excludePropertyList += "-" + "ALLOW_KEYBOARD_INPUT";
    this._excludePropertyList += "-" + "SPELLCHECK";
    this._excludePropertyList += "-" + "DATASET";
    this._excludePropertyList += "-" +"NODENAME";
    this._excludePropertyList += "-" +"NODETYPE";
    this._excludePropertyList += "-" +"LOCALNAME";
    this._excludePropertyList += "-" +"ELEMENT_NODE";
    this._excludePropertyList += "-" +"ATTRIBUTE_NODE";
    this._excludePropertyList += "-" +"TEXT_NODE";
    this._excludePropertyList += "-" +"CDATA_SECTION_NODE";
    this._excludePropertyList += "-" +"ENTITY_REFERENCE_NODE";
    this._excludePropertyList += "-" +"ENTITY_NODE";
    this._excludePropertyList += "-" +"PROCESSING_INSTRUCTION_NODE";
    this._excludePropertyList += "-" +"COMMENT_NODE";
    this._excludePropertyList += "-" +"DOCUMENT_NODE";
    this._excludePropertyList += "-" +"DOCUMENT_TYPE_NODE";
    this._excludePropertyList += "-" +"DOCUMENT_FRAGMENT_NODE";
    this._excludePropertyList += "-" +"NOTATION_NODE";
    this._excludePropertyList += "-" +"TAGNAME";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_DISCONNECTED";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_PRECEDING";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_FOLLOWING";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_CONTAINS";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_CONTAINED_BY";
    this._excludePropertyList += "-" +"DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC";
    this._excludePropertyList += "-" +"BASENAME";
    this._excludePropertyList += "-" +"OUTERHTML";
    this._excludePropertyList += "-" +"INNERHTML";
    this._excludePropertyList += "-" +"INNERTEXT";
    this._excludePropertyList += "-" +"OUTERTEXT";
    this._excludePropertyList += "-" +"TEXT";
    this._excludePropertyList += "-" +"TEXTCONTENT";
    
    //Safari specific properties
    this._excludePropertyList += "-" +"CONTENTEDITABLE";    
    this._excludePropertyList += "-" +"NAMESPACEURI";    
    
    //Opera specific properties
    this._excludePropertyList += "-" +"SCROLLLEFT";    
    this._excludePropertyList += "-" +"SCROLLTOP";    
    this._excludePropertyList += "-" +"SOURCEINDEX";    

    this._excludePropertyList += "-" +"OFFSETTOP";
    this._excludePropertyList += "-" +"OFFSETLEFT";
    this._excludePropertyList += "-" +"OFFSETWIDTH";
    this._excludePropertyList += "-" +"OFFSETHEIGHT";
    this._excludePropertyList += "-" +"SCROLLHEIGHT";
    this._excludePropertyList += "-" +"SCROLLWIDTH";
    this._excludePropertyList += "-" +"CLIENTHEIGHT";
    this._excludePropertyList += "-" +"CLIENTWIDTH";
    this._excludePropertyList += "-" +"BASEURI";
    this._excludePropertyList += "-" +"CH";
    this._excludePropertyList += "-" +"XML";
    
    //Firefox-specific properties
    this._excludePropertyList += "-" + "SELECTIONSTART";
    this._excludePropertyList += "-" + "SELECTIONEND";
    
    //keep this line last
    this._excludePropertyList += "-" 
}
    
LTAF.ExcludePropertyList.prototype = {
    /// <summary>
    /// Method that returns if a property exists in the excluded list of properties
    /// </summary>
    /// <param name="command">A property that needed to be checked</param>
    /// <change date="03/16/2006">Created</change> 
    contain : function(propertyName)
    {
        propertyName = "-"+propertyName+"-"        
        if(this._excludePropertyList.indexOf(propertyName.toUpperCase()) < 0)
        {
            return false;        
        }        
        else
        {
            return true;
        }
    }   

}
        
LTAF.ExcludePropertyList.registerClass('LTAF.ExcludePropertyList');
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