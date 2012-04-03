LTAF.NavigationVerification = function() {
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
