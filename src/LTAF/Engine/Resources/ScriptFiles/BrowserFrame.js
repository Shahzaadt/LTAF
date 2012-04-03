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

LTAF.BrowserFrame.registerClass('LTAF.BrowserFrame');