using System;
using System.Collections.Generic;
using System.Text;
using LTAF.Engine;
using System.Threading;

namespace LTAF
{
    /// <summary>
    /// What event to wait for after a click has been made
    /// </summary>
	/// <remarks>Postback will wait a maxium of 30 seconds.</remarks>
    public enum WaitFor
    {
        /// <summary>None</summary>
        None,
        /// <summary>AsyncPostback</summary>
        AsyncPostback,
        /// <summary>Postback</summary>
        Postback
    }

    //http://www.w3.org/TR/DOM-Level-2-HTML/html.html#ID-58190037
    /// <summary>
    /// Class that represents a Html Dom element.
    /// </summary>
    public class HtmlElement
    {
		//After 30 seconds waitfor.postback will timeout.
		private const int WAIT_FOR_POSTBACK_TIMEOUT = 30000;

        private HtmlAttributeDictionary _attributeDictionary;

        /// <summary>AttributeReader</summary>
        protected HtmlElementAttributeReader _attributeReader;

        private HtmlElementCollection _childElements;
        private HtmlElement _parentElement;
        private HtmlElement _nextSibling;
        private HtmlElement _previousSibling;
        private HtmlPage _parentPage;

        private int _startPosition;
        private int _endPosition;
        private string _innerText;
        private string _tagName;
        private int _tagNameIndex;
        private bool _canUseTagIndexToLocate;

        #region Constructors
        internal HtmlElement(string tagName,
            IDictionary<string, string> attributes,
            HtmlElement parentElement,
            HtmlPage parentPage)
        {
            _innerText = String.Empty;
            _tagName = tagName;
            _parentElement = parentElement;
            _parentPage = parentPage;
            _canUseTagIndexToLocate = false;
            _startPosition = -1;
            _endPosition = -1;
            _attributeDictionary = new HtmlAttributeDictionary(attributes);  
        }
        
        #endregion

        #region Properties
        internal bool CanUseTagIndexToLocate
        {
            get { return _canUseTagIndexToLocate; }
            set { _canUseTagIndexToLocate = value; }
        }

        internal int TagNameIndex
        {
            get { return _tagNameIndex; }
            set { _tagNameIndex = value; }
        }

        internal int StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        internal int EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }

        /// <summary>
        /// Returns whether this element is visible on the browser
        /// </summary>
        public bool IsVisible()
        {
            HtmlElementAttributeReader attributes = this.GetAttributes();
            return attributes.Style.Display != Display.None && attributes.Style.Visibility != Visibility.Hidden;
        }

        /// <summary>
        /// The tag name of this element.
        /// </summary>
        public string TagName
        {
            get { return _tagName; }
        }

        /// <summary>
        /// The id of this element as exposed by the dom.
        /// </summary>
        public string Id
        {
            get
            {
                if (_attributeDictionary.ContainsKey("id"))
                {
                    return _attributeDictionary["id"];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The name of this element as exposed by the dom.
        /// </summary>
        public string Name
        {
            get
            {
                if (_attributeDictionary.ContainsKey("name"))
                {
                    return _attributeDictionary["name"];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The parent of this element (null if this is root element).
        /// </summary>
        public HtmlElement ParentElement
        {
            get 
            { 
                return _parentElement; 
            }
            internal set
            {
                _parentElement = value;
            }
        }

        /// <summary>
        /// NextSibling
        /// </summary>
        public HtmlElement NextSibling
        {
            get
            {
                return _nextSibling;
            }
            internal set
            {
                _nextSibling = value;
            }
        }

        /// <summary>
        /// PreviousSibling
        /// </summary>
        public HtmlElement PreviousSibling
        {
            get
            {
                return _previousSibling;
            }
            internal set
            {
                _previousSibling = value;
            }
        }

        /// <summary>
        /// Web page that contains this element.
        /// </summary>
        public HtmlPage ParentPage
        {
            get { return _parentPage; }
        }

        /// <summary>
        /// The children that belong to this element
        /// </summary>
        public HtmlElementCollection ChildElements
        {
            get { return _childElements; }
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString()
        {
            return "<" + this.TagName + " id='"+ this.Id +"'>";
        }

        /// <summary>
        /// Property that returns the inmemory attribute reader for this element, containing only the attributes that were retrieved on last GetAttributes. 
        /// Meant to be used by infraestructure utilities.
        /// </summary>
        public HtmlElementAttributeReader CachedAttributes
        {
            get
            {
                if (_attributeReader == null)
                {
                    _attributeReader = new HtmlElementAttributeReader(this);
                }

                return _attributeReader;
            }
        }

        internal HtmlAttributeDictionary AttributeDictionary
        {
            get
            {
                return _attributeDictionary;
            }
        }
        #endregion

        #region SetChildElements
        internal virtual void SetChildElements(IList<HtmlElement> childElements)
        {
            _childElements = new HtmlElementCollection(childElements, _parentPage, this);
        }
        #endregion

        #region RefreshAttributesDictionary
        internal void RefreshAttributesDictionary()
        {
            if (_parentPage == null)
            {
                return;
            }
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementAttributes);
            command.Description = "GetElementAttributes";
            command.Target = this.BuildBrowserCommandTarget();
            command.Handler.RequiresElementFound = true;
            string data = _parentPage.ExecuteCommand(this, command).Data;
            HtmlElement tempElement = HtmlElement.Create(data, _parentPage, false);

            this._attributeDictionary = new HtmlAttributeDictionary(tempElement._attributeDictionary);
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Creates an HtmlElement given an html markup string (ie. "&lt;span id=span1&gt;Info&lt;/span&gt;)
        /// </summary>
        /// <param name="htmlMarkup">Well formed html markup string.</param>
        /// <returns>An HtmlElement with children and attributes fully populated.</returns>
        public static HtmlElement Create(string htmlMarkup)
        {
            return Create(htmlMarkup, null, false);
        }

        internal static HtmlElement Create(string htmlMarkup, HtmlPage parentPage, bool ensureValidMarkup)
        {
            HtmlElementBuilder builder = new HtmlElementBuilder(parentPage);
            builder.EnsureValidMarkup = ensureValidMarkup;
            HtmlElement newElement = builder.CreateElement(htmlMarkup);
            return newElement;
        }
        #endregion

        #region GetInnerHtml

        /// <summary>
        /// GetInnerHtml
        /// </summary>
        public string GetInnerHtml()
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementInnerHtml);
            command.Description = "GetInnerHtml";
            command.Target = this.BuildBrowserCommandTarget();
            command.Handler.RequiresElementFound = true;
            return (_parentPage.ExecuteCommand(this, command).Data ?? "").Trim();
        }
        #endregion

        #region InnerText
        /// <summary>
        /// Property that returns the inmemory inner text for this element. Meant to be used by infraestructure utilities
        /// </summary>
        public string CachedInnerText
        {
            get
            {
                return _innerText;
            }
            internal set
            {
                _innerText = value;
            }
        }

        /// <summary>
        /// Property that returns union the inmemory inner text of this element and all of its child elements. Meant to be used by infraestructure utilities.
        /// </summary>
        public string CachedInnerTextRecursive
        {
            get
            {
                StringBuilder cachedInnerTextBuilder = new StringBuilder();
                AppendCachedInnerTextRecursive(cachedInnerTextBuilder);
                return cachedInnerTextBuilder.ToString();
            }
        }

        private void AppendCachedInnerTextRecursive(StringBuilder cachedInnerTextBuilder)
        {
            cachedInnerTextBuilder.Append(_innerText);
            foreach (HtmlElement child in this.ChildElements)
            {
                cachedInnerTextBuilder.Append(" " + child.CachedInnerTextRecursive);
            }
        }

        /// <summary>
        /// Retrieves the innerText of this element from the web page.
        /// </summary>
        /// <returns>The innerText as exposed by the Dom at this moment.</returns>
        public string GetInnerText()
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementInnerText);
            command.Description = "GetInnerText";
            command.Target = this.BuildBrowserCommandTarget();
            command.Handler.RequiresElementFound = true;
            _innerText = (_parentPage.ExecuteCommand(this, command).Data ?? "").Trim();
            return _innerText;
        }

        /// <summary>
        /// Recursively builds the innerText of this element and all its child elements.
        /// </summary>
        /// <returns>The innerText as exposed by the Dom at this moment.</returns>
        public string GetInnerTextRecursively()
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementInnerTextRecursive);
            command.Description = "GetInnerTextRecursively";
            command.Target = this.BuildBrowserCommandTarget();
            command.Handler.RequiresElementFound = true;
            return (_parentPage.ExecuteCommand(this, command).Data ?? "").Trim();
        }

        /// <summary>
        /// Recursively builds the innerText of this element and all its child elements.
        /// </summary>
        private string GetInnerTextRecursively(bool showHidden)
        {
            if (showHidden == false)
            {
                HtmlElementAttributeReader attributes = GetAttributes();
                if (attributes.Style.Display == Display.None ||
                    attributes.Style.Visibility == Visibility.Hidden)
                {
                    return "";
                }
            }

            StringBuilder childrensInnerText = new StringBuilder(_innerText);
            for (int i = 0; i < this.ChildElements.Count; i++)
            {
                childrensInnerText.Append(" " + this.ChildElements[i].GetInnerTextRecursively(showHidden));
            }
            return childrensInnerText.ToString().Trim();
        }

        #endregion

        #region GetAttributes
        /// <summary>
        /// Method that refreshes the latest attribute dictionary for this element
        /// </summary>
        public HtmlElementAttributeReader GetAttributes()
        {
            this.RefreshAttributesDictionary();
            _attributeReader = new HtmlElementAttributeReader(this);
            return _attributeReader;
        }
        #endregion

        #region AppendInnerText
        internal void AppendInnerText(string text)
        {
            _innerText += text;
        }
        #endregion

        #region GetOuterHtml
        /// <summary>
        /// Retrieve the outer html for this element. 
        /// </summary>
        /// <returns>The full outer html as exposed by the Dom at this moment.</returns>
        internal string GetOuterHtml()
        {
            return GetOuterHtml(true);
        }

        internal string GetOuterHtml(bool reload)
        {
            if (reload)
            {
                BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetElementDom);
                command.Description = "GetElementDom";
                command.Handler.RequiresElementFound = true;
                command.Target = this.BuildBrowserCommandTarget();
                return _parentPage.ExecuteCommand(this, command).Data;
            }
            else
            {
                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<" + this.TagName);

                foreach (string attr in this._attributeDictionary.Keys)
                {
                    htmlBuilder.Append(" " + attr + "=");
                    htmlBuilder.Append("'" + this._attributeDictionary[attr] + "'");
                }

                htmlBuilder.Append(">");
                htmlBuilder.Append(this.CachedInnerText);


                foreach (HtmlElement element in this.ChildElements)
                {
                    htmlBuilder.Append(element.GetOuterHtml(false));
                }

                htmlBuilder.Append("</" + this.TagName + ">");
                return htmlBuilder.ToString();

            }
        }
        #endregion

        #region BuildBrowserCommandTarget
        internal BrowserCommandTarget BuildBrowserCommandTarget()
        {
            BrowserCommandTarget commandTarget = new BrowserCommandTarget();

            if (!String.IsNullOrEmpty(this.Id))
            {
                commandTarget.Id = this.Id;
                return commandTarget;
            }
            else if (!String.IsNullOrEmpty(this.Name))
            {
                commandTarget.Id = this.Name;
                return commandTarget;
            }
            else if(this._canUseTagIndexToLocate)
            {
                commandTarget.TagName = this.TagName;
                commandTarget.Index = this.TagNameIndex;
                return commandTarget;
            }

            HtmlElement nearestParent = FindNearestLocalizableElement();
            commandTarget = nearestParent.BuildBrowserCommandTarget();

            int occurrence;
            if (FindElementOccurrenceFromParent(nearestParent, out occurrence))
            {
                commandTarget.ChildTagName = this.TagName;
                commandTarget.ChildTagIndex = occurrence - 1;
                return commandTarget;
            }
            
            throw new WebTestException("Could not build a command that locates this element.");
        }
        #endregion

        #region FindNearestLocalizableElement
        private HtmlElement FindNearestLocalizableElement()
        {
            HtmlElement parent = this._parentElement;
            while (parent != null
                && String.IsNullOrEmpty(parent.Id)
                && String.IsNullOrEmpty(parent.Name)
                && !CanUseTagIndexToLocate)
            {
                parent = parent.ParentElement;
            }

            if (parent != null)
            {
                return parent;
            }
            else
            {
                throw new WebTestException("Could not build a command that locates this element.");
            }
        }
        #endregion

        #region FindElementOccurrenceFromParent
        private bool FindElementOccurrenceFromParent(HtmlElement parent, out int count)
        {
            count = 0;

            foreach (HtmlElement child in parent.ChildElements)
            {
                if (child.TagName == this.TagName)
                {
                    count++;
                }
                if (child == this)
                {
                    return true;
                }

                int countInChildren;
                bool found = FindElementOccurrenceFromParent(child, out countInChildren);
                count += countInChildren;

                if (found)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Wait Methods
        /// <summary>
        /// Wait until an attribute of this element is set to the expected value.
        /// </summary>
        /// <param name="attributeName">The attribute name to wait upon.</param>
        /// <param name="expectedAttributeValue">The attribute value to wait upon.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds for the attribute to have the expected value.</param>
        public void WaitForAttributeValue(string attributeName, string expectedAttributeValue, int timeoutInSeconds)
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.WaitForDomChange);
            command.Description = "WaitForDomChange";
            command.Target = this.BuildBrowserCommandTarget();

            // even though we truly require element to be found, this command needs special logic that is implemented in the command handler.
            command.Handler.RequiresElementFound = false;
            command.Handler.SetArguments(attributeName, expectedAttributeValue, timeoutInSeconds * 1000);
            _parentPage.ExecuteCommand(command);
        }

        /// <summary>
        /// Wait until the inner text of this element is set to the expected value.
        /// </summary>
        /// <param name="expectedInnerText">The inner text value to wait upon.</param>
        /// <param name="timeoutInSeconds">Timeout in seconds for the inner text to have the expected value.</param>
        public void WaitForInnerText(string expectedInnerText, int timeoutInSeconds)
        {
            WaitForAttributeValue("innerHTML", expectedInnerText, timeoutInSeconds);

            //if we succeed, we can assume that the inner text for this element has changed.
            _innerText = expectedInnerText;
        }

        /// <summary>
        /// Wait until this element can no longer be found on the web page.
        /// </summary>
        /// <param name="timeoutInSeconds">Timeout in seconds for this element to disappear.</param>
        public void WaitUntilNotFound(int timeoutInSeconds)
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.WaitUntilDissapears);
            command.Description = "WaitUntilNotFound";
            command.Target = this.BuildBrowserCommandTarget();

            // even though we truly require element to be found, this command needs special logic that is implemented in the command handler.
            command.Handler.RequiresElementFound = false;

            string innerText = null;
            if (this.ChildElements.Count == 0 && !String.IsNullOrEmpty(_innerText))
            {
                //if this element has inner text, we also want to use it in the wait until dissappaears
                innerText = _innerText;
            }
            command.Handler.SetArguments(timeoutInSeconds * 1000, innerText);

            _parentPage.ExecuteCommand(command);
        }
        #endregion

        #region DispatchEvent Methods

        /// <summary>
        /// DispatchEvent
        /// </summary>
        public void DispatchEvent(HtmlEventName eventName)
        {
            HtmlEvent evt = HtmlEvent.Create(eventName);
            DispatchEvent(evt);
        }

        /// <summary>
        /// DispatchEvent
        /// </summary>
        public void DispatchEvent(HtmlEvent htmlEvent)
        {
            // [01/26/2009] Missing support for CanBuble and Cancelable. 
            //      They're never set as arguments for the events even though HtmlEvent exposes them.
            BrowserCommand command = new BrowserCommand();
            command.Handler.RequiresElementFound = true;

            HtmlKeyEvent keyEvent = htmlEvent as HtmlKeyEvent;
            if (keyEvent != null)
            {
                command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.DispatchKeyEvent;
                command.Handler.SetArguments(keyEvent.Name,
                        keyEvent.CtrlKey, keyEvent.AltKey, keyEvent.ShiftKey, keyEvent.MetaKey,
                        keyEvent.KeyCode, keyEvent.CharCode);
            }
            else if (htmlEvent is HtmlMouseEvent)
            {
                command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.DispatchMouseEvent;
                command.Handler.SetArguments(htmlEvent.Name);
            }
            else
            {
                command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.DispatchHtmlEvent;
                command.Handler.SetArguments(htmlEvent.Name);
            }

            command.Target = this.BuildBrowserCommandTarget();
            command.Description = "DispatchEvent:" + htmlEvent.Name;
            
            this.ParentPage.ExecuteCommand(command);
        }
        #endregion

        #region Action Methods
        /// <summary>
        /// Dispatch a focus event to this element.
        /// </summary>
        public void Focus()
        {
            this.DispatchEvent(HtmlEventName.Focus);
        }

        /// <summary>
        /// Dispatch a blur event to this element.
        /// </summary>
        public void Blur()
        {
            this.DispatchEvent(HtmlEventName.Blur);
        }

        /// <summary>
        /// Click on this element.
        /// </summary>
        public void Click()
        {
            this.Click(WaitFor.None);
        }

        /// <summary>
        /// Click on this element and wait
        /// </summary>
        public void Click(WaitFor waitFor)
        {
            CommandParameters parameters = new CommandParameters();
            parameters.WaitFor = waitFor;
            parameters.PopupAction = PopupAction.None;
            this.Click(parameters);
        }

        /// <summary>
        /// Click on this element with parameters
        /// </summary>
        public void Click(CommandParameters parameters)
        {
            bool waitForPostback = (parameters.WaitFor == WaitFor.Postback);

            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.ClickElement);
            command.Handler.PopupAction = parameters.PopupAction;
            command.Target = this.BuildBrowserCommandTarget();
            command.Description = "Click";
            command.Handler.SetArguments(waitForPostback, WAIT_FOR_POSTBACK_TIMEOUT);
            command.Handler.RequiresElementFound = true;
            BrowserInfo browserInfo = this.ParentPage.ExecuteCommand(this, command);
            if (browserInfo != null)
            {
                parameters.PopupText = browserInfo.Data;
            }

            if (parameters.WaitFor == WaitFor.AsyncPostback)
            {
                this._parentPage.WaitForAsyncPostComplete();
            }
        }

        /// <summary>
        /// Dispatch a mouseover event to this element.
        /// </summary>
        public void MouseOver()
        {
            this.DispatchEvent(HtmlEventName.MouseOver);
        }

        /// <summary>
        /// Whether the elements supports the set text command
        /// </summary>
        protected virtual bool CanSetText
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Set the text of this element.
        /// </summary>
        /// <param name="textValue">Value to set.</param>
        public void SetText(string textValue)
        {
            SetText(textValue, true);
        }

        /// <summary>
        /// Set the text of this element.
        /// </summary>
        /// <param name="textValue">Value to set.</param>
        /// <param name="focusAndBlur">Wheter to dispatch focus and blur events to the textbox as part of setting the value.</param>
        public void SetText(string textValue, bool focusAndBlur)
        {
            if (CanSetText)
            {
                BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.SetTextBox);
                command.Target = this.BuildBrowserCommandTarget();
                command.Description = "FillTextBox";
                command.Handler.SetArguments(textValue, focusAndBlur);
                command.Handler.RequiresElementFound = true;
                this.ParentPage.ExecuteCommand(this, command);
            }
            else
            {
                throw new WebTestException("This HtmlElement does not support setting text.");
            }
        }

        /// <summary>
        /// By default no element supports selecting index
        /// </summary>
        protected virtual bool CanSelectIndex
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Select an item of this list.
        /// </summary>
        /// <param name="selectBoxIndex">The zero-based index to select.</param>
        public void SetSelectedIndex(int selectBoxIndex)
        {
            if (CanSelectIndex)
            {
                BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.SetSelectBoxIndex);
                command.Target = this.BuildBrowserCommandTarget();
                command.Description = "SetSelectBox";
                command.Handler.SetArguments(selectBoxIndex);
                command.Handler.RequiresElementFound = true;
                this.ParentPage.ExecuteCommand(this, command);
            }
            else
            {
                throw new WebTestException("This HtmlElement does not support setting the selected index.");
            }
        }
        #endregion

        /// <summary>
        /// Locates the nearest parent form that contains this element
        /// </summary>
        /// <returns>HtmlFormElement that contains this elememnt, null if no form is found</returns>
        public HtmlFormElement FindParentForm()
        {
            HtmlElement element = this.ParentElement;

            while (element != null)
            {
                if (element is HtmlFormElement)
                {
                    return (HtmlFormElement)element;
                }
                element = element.ParentElement;
            }

            return null;
        }
    }
}
