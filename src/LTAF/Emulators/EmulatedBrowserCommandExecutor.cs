using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LTAF.Engine;
using System.IO;
using System.Net;

namespace LTAF.Emulators
{
    /// <summary>
    /// CommandExecutor to use when a BrowserEmulator is used.
    /// </summary>
    internal class EmulatedBrowserCommandExecutor : IBrowserCommandExecutor
    {
        private const string EXECUTECOMMAND_REQUIRESOURCEANDTYPE_FORMATSTRING = "The '{0}' command requires the HtmlElement source to be an instance of '{1}'.";
        private const string EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING = "The '{0}' command requires the HtmlElement source to be set.";
        private const string EXECUTECOMMAND_NOTSUPPORTEDONELEMENT_FORMATSTRING = "The '{0}' command is not supported on the element '{1}'";
        private const string EXECUTECOMMAND_NEGATIVEINDEXISOUTOFRANGE = "The command '{0}', negative index out of range: '{1}'. Use -1 to unselect all options for select html element.";
        private const string CLICKSUBMITBUTTON_FORMNOTFOUND = "Unable to locate parent form to submit.";
        private const string CLICKANCHOR_HREFNOTFOUND = "Anchor element does not contain an Href attribute.";

        /// <summary>Generic event occuring before every command was executed </summary>
        public event EventHandler<BrowserCommandEventArgs> BrowserCommandExecuting;
        /// <summary>Specifies format for every browser command handler </summary>
        private delegate BrowserInfo BrowserCommandHandler(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout);
        /// <summary>Dictionary with all supported browser commands and their handlers </summary>
        private Dictionary<string, BrowserCommandHandler> _browserCommandHandlerFactory;

        /// <summary>Stores last response (everything between html and /html tags) </summary>
        private string _currentBody;

        /// <summary>Interface for the object implementing the actual request</summary>
        private BrowserEmulator _emulator;

        /// <summary>
        /// ctor 
        /// </summary>
        /// <param name="emulator">reference BrowserEmulator</param>
        internal EmulatedBrowserCommandExecutor(BrowserEmulator emulator)
        {
            this._emulator = emulator;

            // register all browser command handlers
            _browserCommandHandlerFactory = new Dictionary<string, BrowserCommandHandler>();
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.NavigateToUrl, ExecuteCommandNavigateToUrl);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetPageDom, ExecuteCommandGetPageDom);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.SetTextBox, ExecuteCommandSetText);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.ClickElement, ExecuteClickElement);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetElementInnerHtml, ExecuteCommandGetElementInnerHtml);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetElementDom, ExecuteCommandGetElementDom);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.FormSubmit, ExecuteFormSubmit);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.SetSelectBoxIndex, ExecuteCommandSetSelectBoxIndex);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetElementInnerText, ExecuteCommandGetElementInnerText);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetElementInnerTextRecursive, ExecuteCommandGetElementInnerTextRecursive);
            RegisterBrowserCommandHandler(BrowserCommand.FunctionNames.GetElementAttributes, ExecuteCommandGetElementAttributes);
        }

        /// <summary>
        /// Returns current body obtained after last request
        /// </summary>
        public string CurrentBody
        {
            get
            {
                return this._currentBody;
            }
        }

        #region IBrowserCommandExecutor

        /// <summary>
        /// Execute a command on the target browser
        /// </summary>
        /// <param name="threadId">Id used to distinguish between multiple tests running at the same time</param>
        /// <param name="source">HtmlElement that initiated this command, null if none</param>
        /// <param name="command">Command to execute</param>
        /// <param name="secondsTimeout">Timeout in seconds that browser shoud wait for this command</param>
        /// <returns>BrowserInfo object that contains command results</returns>
        public BrowserInfo ExecuteCommand(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source != null)
            {
                // add id to description, to make logs more informative
                command.Description += " id=" + source.Id;
            }

            // fire event before any command execution is started
            OnBrowserCommandExecuting(new BrowserCommandEventArgs(threadId, command, secondsTimeout));

            BrowserInfo browserInfo = null;

            BrowserCommandHandler commandHandler = null;

            if (this._browserCommandHandlerFactory.TryGetValue(command.Handler.ClientFunctionName, out commandHandler))
            {
                // just call a handler and if there any exceptions, let them go
                browserInfo = commandHandler(threadId, source, command, secondsTimeout);
            }
            else
            {
                throw new NotSupportedException(String.Format("Command '{0}' is not supported", command.Handler.ClientFunctionName));
            }

            return browserInfo;
        }

        #endregion


        /// <summary>
        /// Adds Browser Command handlers to the dictionary
        /// </summary>
        /// <param name="commandName">Command name</param>
        /// <param name="commandHandler">Corresponding handler</param>
        private void RegisterBrowserCommandHandler(string commandName, BrowserCommandHandler commandHandler)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException("commandName", "Browser command name can not be null or empty.");
            }

            if (commandHandler == null)
            {
                throw new ArgumentNullException("commandHandler", "Handler for browser command can not be null.");
            }

            this._browserCommandHandlerFactory[commandName] = commandHandler;
        }

        /// <summary>
        /// Handler for NavigateToUrl browser command
        /// </summary>
        private BrowserInfo ExecuteCommandNavigateToUrl(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            return PerformRequest((string)command.Handler.Arguments[0]);
        }

        /// <summary>
        /// Handler for GetPageDom browser command
        /// </summary>
        private BrowserInfo ExecuteCommandGetPageDom(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            BrowserInfo browserInfo = new BrowserInfo();

            browserInfo.Data = this._currentBody;

            return browserInfo;
        }

        /// <summary>
        /// Handler for SetText browser command
        /// </summary>
        private BrowserInfo ExecuteCommandSetText(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source is HtmlInputElement)
            {
                ((HtmlInputElement)source).CachedAttributes.Value = (string)command.Handler.Arguments[0];
            }
            else if (source is HtmlTextAreaElement)
            {
                ((HtmlTextAreaElement)source).CachedInnerText = ((string)command.Handler.Arguments[0]);
            }
            else
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCEANDTYPE_FORMATSTRING, "SetText",
                        typeof(HtmlInputElement).ToString() + " or " + typeof(HtmlTextAreaElement).ToString()));
            }

            return new BrowserInfo();
        }

        /// <summary>
        /// Handler for GetElementDom browser command
        /// </summary>
        private BrowserInfo ExecuteCommandSetSelectBoxIndex(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "SetSelectBoxIndex"));
            }

            HtmlSelectElement select = source as HtmlSelectElement;
            if (select != null)
            {
                var options = select.ChildElements.FindAll("option");

                if (options != null)
                {
                    // remove old selected attributes
                    foreach (HtmlElement option in options)
                    {
                        ((HtmlOptionElement)option).CachedAttributes.Selected = false;
                    }

                    // get requested index
                    int index = Int32.Parse(command.Handler.Arguments[0].ToString());
                    if (index >= 0)
                    {
                        var newSelectedOption = select.ChildElements.Find("option", index);

                        ((HtmlOptionElement)newSelectedOption).CachedAttributes.Selected = true;
                    }
                    else if (index < -1)
                    {
                        throw new InvalidOperationException(String.Format(EXECUTECOMMAND_NEGATIVEINDEXISOUTOFRANGE, "SetSelectBoxIndex", index));
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCEANDTYPE_FORMATSTRING, "SetSelectBoxIndex",
                        typeof(HtmlSelectElement).ToString()));
            }

            return new BrowserInfo();
        }

        /// <summary>
        /// Handler for GetElementDom browser command
        /// </summary>
        private BrowserInfo ExecuteCommandGetElementDom(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "GetElementDom"));
            }

            BrowserInfo browserInfo = new BrowserInfo();

            browserInfo.Data = this._currentBody.Substring(source.StartPosition, source.EndPosition - source.StartPosition + 1);

            return browserInfo;
        }

        /// <summary>
        /// Handler for GetElementInnerHtml browser command
        /// </summary>
        private BrowserInfo ExecuteCommandGetElementInnerHtml(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "GetElementInnerHtml"));
            }

            BrowserInfo browserInfo = ExecuteCommandGetElementDom(threadId, source, command, secondsTimeout);

            // now remove the tag itself
            if (!string.IsNullOrEmpty(browserInfo.Data))
            {
                // find starting tag and remove it
                int tagEndIndex = browserInfo.Data.IndexOf('>');

                // if tag is not ending with />, retrieve inner html
                if ((tagEndIndex + 1) < browserInfo.Data.Length)
                {
                    browserInfo.Data = browserInfo.Data.Substring(tagEndIndex + 1);

                    // find ending tag and remove it
                    tagEndIndex = browserInfo.Data.LastIndexOf('<');

                    if (tagEndIndex >= 0)
                    {
                        browserInfo.Data = browserInfo.Data.Substring(0, tagEndIndex).Trim();
                    }
                }
                else
                {
                    browserInfo.Data = "";
                }
            }

            return browserInfo;
        }

        /// <summary>
        /// Handler for GetElementInnerText browser command
        /// </summary>
        private BrowserInfo ExecuteCommandGetElementInnerText(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "GetElementInnerText"));
            }

            BrowserInfo browserInfo = new BrowserInfo();

            browserInfo.Data = source.CachedInnerText;

            return browserInfo;
        }

        /// <summary>
        /// Handler for GetElementInnerTextRecursive browser command
        /// </summary>
        private BrowserInfo ExecuteCommandGetElementInnerTextRecursive(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "GetElementInnerTextRecursive"));
            }

            BrowserInfo browserInfo = ExecuteCommandGetElementInnerHtml(threadId, source, command, secondsTimeout);

            // now get all text that is not between first < symbol and last >
            if (!string.IsNullOrEmpty(browserInfo.Data))
            {
                string tempText = browserInfo.Data;

                // find starting <
                int tagIndex = browserInfo.Data.IndexOf('<');

                if (tagIndex >= 0)
                {
                    tempText = browserInfo.Data.Substring(0, tagIndex);
                    
                    tagIndex = browserInfo.Data.IndexOf('>', tagIndex + 1);

                    while (tagIndex >= 0)
                    {
                        int stopIndex = browserInfo.Data.IndexOf('<', tagIndex + 1);
                        if (stopIndex >= 0)
                        {
                            tempText = tempText + browserInfo.Data.Substring(tagIndex + 1, stopIndex - tagIndex - 1);

                            tagIndex = browserInfo.Data.IndexOf('>', tagIndex + 1);
                        }
                        else
                        {
                            tempText = tempText + browserInfo.Data.Substring(tagIndex + 1);
                            tagIndex = -1;
                        }
                    }
                }

                browserInfo.Data = tempText;
            }

            return browserInfo;
        }

        private BrowserInfo ExecuteCommandGetElementAttributes(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "GetElementAttributes"));
            }

            return ExecuteCommandGetElementDom(threadId, source, command, secondsTimeout);
        }
        
        /// <summary>
        /// Handler for Form Submit browser command
        /// </summary>
        private BrowserInfo ExecuteFormSubmit(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {  
            var form = source as HtmlFormElement;
            if (form == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCEANDTYPE_FORMATSTRING, "FormSubmit",
                        typeof(HtmlFormElement).ToString()));
            }

            return ExecuteFormSubmit(form);
        }

        /// <summary>
        /// Executes a form submit
        /// </summary>
        private BrowserInfo ExecuteFormSubmit(HtmlFormElement form, HtmlInputElement submitInputElement = null)
        {
            Uri url = new Uri(this._emulator.CurrentUri, form.CachedAttributes.Get("action", this._emulator.CurrentUri));

            PostDataCollection postdataCollection = form.GetPostDataCollection(submitInputElement);

            // execute request
            return PerformRequest(url.AbsoluteUri, // url
                        form.CachedAttributes.Get("method", "post"), // method
                        "application/x-www-form-urlencoded", // contentType
                        postdataCollection); // postData     
        }

        /// <summary>
        ///  Handler for ClickElement browser command
        /// </summary>
        private BrowserInfo ExecuteClickElement(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            if (source == null)
            {
                throw new InvalidOperationException(String.Format(EXECUTECOMMAND_REQUIRESOURCE_FORMATSTRING, "ClickElement"));
            }

            BrowserInfo browserInfo = null;
            if (TryHandleSubmitButtonClick(source, out browserInfo)
                || TryHandleAnchorClick(source, out browserInfo)
                || TryHandleCheckBoxClick(source, out browserInfo)
                || TryHandleRadioClick(source, out browserInfo))
            {
                return browserInfo;
            }

            throw new NotSupportedException(String.Format(EXECUTECOMMAND_NOTSUPPORTEDONELEMENT_FORMATSTRING, "ClickElement", source));
        }

        /// <summary>
        /// Checks if source element is an anchor and handles the click if it is.
        /// </summary>
        private bool TryHandleAnchorClick(HtmlElement source, out BrowserInfo browserInfo)
        {
            browserInfo = null;
            HtmlAnchorElement anchor = source as HtmlAnchorElement;
            if (anchor != null)
            {
                if (!anchor.CachedAttributes.Dictionary.ContainsKey("href"))
                {
                    throw new InvalidOperationException(CLICKANCHOR_HREFNOTFOUND);
                }

                Uri url = new Uri(this._emulator.CurrentUri, anchor.CachedAttributes.HRef);

                browserInfo = PerformRequest(url.AbsoluteUri);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if source element is an input type=submit and handles the click if it is
        /// </summary>
        private bool TryHandleSubmitButtonClick(HtmlElement source, out BrowserInfo browserInfo)
        {
            browserInfo = null;
            HtmlInputElement button = source as HtmlInputElement;

            if (button != null && button.CachedAttributes.Type == HtmlInputElementType.Submit)
            {
                // retrieve the form
                HtmlFormElement form = button.FindParentForm();
                if (form == null)
                {
                    throw new InvalidOperationException(CLICKSUBMITBUTTON_FORMNOTFOUND);
                }

                browserInfo = ExecuteFormSubmit(form, button);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if source element is a checkbox and handles the click if it is.
        /// </summary>
        private bool TryHandleCheckBoxClick(HtmlElement source, out BrowserInfo browserInfo)
        {
            browserInfo = null;

            HtmlInputElement checkbox = source as HtmlInputElement;
            if (checkbox != null && checkbox.CachedAttributes.Type == HtmlInputElementType.Checkbox)
            {
                checkbox.CachedAttributes.Checked = !checkbox.CachedAttributes.Checked;

                browserInfo = new BrowserInfo();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if source element is a radio and handles the click if it is.
        /// </summary>
        private bool TryHandleRadioClick(HtmlElement source, out BrowserInfo browserInfo)
        {
            browserInfo = null;

            HtmlInputElement radio = source as HtmlInputElement;
            if (radio != null && radio.CachedAttributes.Type == HtmlInputElementType.Radio)
            {
                // get all radios with the given name and uncheck them
                ReadOnlyCollection<HtmlElement> allRadios = source.ParentPage.Elements.FindAll(new { type = "radio", name = radio.Name });
                foreach (HtmlElement r in allRadios)
                {
                    if (r is HtmlInputElement)
                    {
                        ((HtmlInputElement)r).CachedAttributes.Checked = false;
                    }
                }

                // check current radio
                radio.CachedAttributes.Checked = true;

                browserInfo = new BrowserInfo();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Performes an actual request by calling requestor's ExecuteRequest
        /// </summary>
        private BrowserInfo PerformRequest(string url)
        {
            return PerformRequest(url, "get", "", null);
        }

        /// <summary>
        /// Performes an actual request by calling requestor's ExecuteRequest
        /// </summary>
        /// <param name="url">Url to be requested</param>
        /// <param name="method">Method: get/post</param>
        /// <param name="contentType">content type</param>
        /// <param name="postData">name/value collection of post data parameters</param>
        /// <returns>BrowserInfo returned by requestor</returns>
        private BrowserInfo PerformRequest(string url, string method, string contentType,
                        PostDataCollection postData)
        {
            // do a request
            BrowserInfo browserInfo = this._emulator.ExecuteRequest(url, method, contentType, postData);

            // successfull store current page's body
            if (browserInfo != null)
            {
                this._currentBody = browserInfo.Data;
            }

            return browserInfo;
        }

        #region Raise Events

        /// <summary>
        /// Fires BrowserCommandExecuting event
        /// </summary>
        protected virtual void OnBrowserCommandExecuting(BrowserCommandEventArgs e)
        {
            if (BrowserCommandExecuting != null)
            {
                BrowserCommandExecuting(this, e);
            }
        }

        #endregion
    }
}
