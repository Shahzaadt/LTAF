// static dom support
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
