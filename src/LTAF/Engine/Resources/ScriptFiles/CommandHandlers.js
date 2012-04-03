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
        
    }