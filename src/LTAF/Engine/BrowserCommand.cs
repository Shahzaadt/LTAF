using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LTAF.Engine
{
    /// <summary>
    /// Class that represents the class that is transfered between the testcase 
    /// and the browser
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    [XmlInclude(typeof(BrowserCommandHandler))]
    [XmlInclude(typeof(BrowserCommandTarget))]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class BrowserCommand
    {
        private string[] _traces;
        private string _description;
        private BrowserCommandTarget _target;
        private BrowserCommandHandler _handler;

        /// <summary>
        /// Class that contains constant strings for common function names
        /// </summary>
        public class FunctionNames
        {
            /// <summary>NavigateToUrl</summary>
            public const string NavigateToUrl = "NavigateToUrl";
            /// <summary>SetTextBox</summary>
            public const string SetTextBox = "SetTextBox";
            /// <summary>ClickElement</summary>
            public const string ClickElement = "ClickElement";
            /// <summary>SetSelectBoxIndex</summary>
            public const string SetSelectBoxIndex = "SetSelectBoxIndex";
            /// <summary>DispatchHtmlEvent</summary>
            public const string DispatchHtmlEvent = "DispatchHtmlEvent";
            /// <summary>DispatchMouseEvent</summary>
            public const string DispatchMouseEvent = "DispatchMouseEvent";
            /// <summary>DispatchKeyEvent</summary>
            public const string DispatchKeyEvent = "DispatchKeyEvent";
            /// <summary>ExecuteScript</summary>
            public const string ExecuteScript = "ExecuteScript";

            /// <summary>GetCurrentUrl</summary>
            public const string GetCurrentUrl = "GetCurrentUrl";
            /// <summary>GetPageDom</summary>
            public const string GetPageDom = "GetPageDom";
            /// <summary>GetElementDom</summary>
            public const string GetElementDom = "GetElementDom";
            /// <summary>GetElementAttributes</summary>
            public const string GetElementAttributes = "GetElementAttributes";
            /// <summary>GetElemenInnerText</summary>
            public const string GetElementInnerText = "GetElementInnerText";
            /// <summary>GetElementInnerTestRecursive</summary>
            public const string GetElementInnerTextRecursive = "GetElementInnerTextRecursive";
            /// <summary>GetElementInnerHtml</summary>
            public const string GetElementInnerHtml = "GetElementInnerHtml";

            /// <summary>WaitUntilDisappears</summary>
            public const string WaitUntilDissapears = "WaitUntilDissapears";
            /// <summary>WaitForDomChange</summary>
            public const string WaitForDomChange = "WaitForDomChange";
            /// <summary>WaitForScript</summary>
            public const string WaitForScript = "WaitForScript";
            /// <summary>TestcaseExecuting</summary>
            public const string TestcaseExecuting = "TestcaseExecuting";
            /// <summary>TestcaseExecuted</summary>
            public const string TestcaseExecuted = "TestcaseExecuted";
            /// <summary>TestRunStarted</summary>
            public const string TestRunStarted = "TestRunStarted";
            /// <summary>TestRunFinished</summary>
            public const string TestRunFinished = "TestRunFinished";

            /// <summary>FormSubmit</summary>
            public const string FormSubmit = "FormSubmit";
        }

        /// <summary>
        ///	Constructor that initialized the Handler and the Target objects
        /// </summary>
        public BrowserCommand()
        {
            this.Target = new BrowserCommandTarget();
            this.Handler = new BrowserCommandHandler();
        }

        /// <summary>
        ///	Constructor that receives the id of the target object and the client 
        /// function name to handle the command execution
        /// </summary>
        /// <param name="clientFunctionName">The name of the client function that will handle the command</param>
        public BrowserCommand(string clientFunctionName)
            : this()
        {
            this.Handler.ClientFunctionName = clientFunctionName;
        }

        /// <summary>
        /// Object that describes the client side handler that will execute this command
        /// </summary>
        public BrowserCommandHandler Handler
        {
            get
            {
                return _handler;
            }
            set
            {
                _handler = value;
            }
        }

        /// <summary>
        /// Object that describes the target DOM element upon which the command will be executed
        /// </summary>
        public BrowserCommandTarget Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        /// <summary>
        /// A description of this command. Will be printed in the driver frame
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set 
            {
                _description = value;
            }
        }

        /// <summary>
        /// A list of traces to be displayed in the driver frame
        /// </summary>
        public string[] Traces
        {
            get 
            {
                return _traces;
            }
            set
            {
                _traces = value;
            }
        }
    }
}
