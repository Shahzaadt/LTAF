using System;
using System.Collections.Generic;
using System.Text;


namespace LTAF
{
    /// <summary>PopupAction</summary>
    public enum PopupAction
    {
        /// <summary>None</summary>
        None,
        /// <summary>AlertOK</summary>
        AlertOK,
        /// <summary>ConfigmOK</summary>
        ConfirmOK,
        /// <summary>ConfirmCancel</summary>
        ConfirmCancel
    }
}

namespace LTAF.Engine
{
    /// <summary>
    /// Class that describes the client side handler that will execute this command
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class BrowserCommandHandler
    {
        private PopupAction _popupAction;
        private bool _requiresElementFound;
        private string _clientFunctionName;
        private object[] _arguments;

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public BrowserCommandHandler()
        {
            _requiresElementFound = false;
            _popupAction = PopupAction.None;
        }

        /// <summary>
        /// Whether the engine should wait for an alert to show and capture its text
        /// </summary>
        public PopupAction PopupAction
        {
            get
            {
                return _popupAction;
            }
            set
            {
                _popupAction = value;
            }
        }

        /// <summary>
        /// Whether the command handler requires an HtmlElement to be found in order to operate
        /// </summary>
        public bool RequiresElementFound
        {
            get { return _requiresElementFound; }
            set { _requiresElementFound = value; }
        }

        /// <summary>
        /// The name of the javascript function included in LTAF AjaxDriver to execute.
        /// This is the client side handler function for this command
        /// </summary>
        public string ClientFunctionName
        {
            get
            {
                return _clientFunctionName;
            }
            set
            {
                _clientFunctionName = value;
            }
        }

        /// <summary>
        /// Additional parameters passed to the handler function
        /// </summary>
        public object[] Arguments
        {
            get
            {
                return _arguments;
            }
            set
            {
                _arguments = value;
            }
        }

        /// <summary>
        /// Method to facilitate setting the Arguments object array
        /// </summary>
        /// <param name="args">Object array containing the arguments to pass to the command handler</param>
        public void SetArguments(params object[] args)
        {
            _arguments = args;
        }
    }
}
