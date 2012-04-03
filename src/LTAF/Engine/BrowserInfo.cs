using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF.Engine
{
    /// <summary>
    /// Class that represents the information that is transferred between the browser
    /// and the testcase
    /// </summary>
    /// <change date="02/06/2006">Created</change>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class BrowserInfo
    {
        private string _data;
        private string _errorMessages;
        private string _javascriptErrorMessages;
        private string _infoMessages;

        /// <summary>
        /// The custom return value set by the BrowserCommand handler
        /// </summary>
        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Error messages logged by testcase or engine through javascript.
        /// </summary>
        public string ErrorMessages
        {
            get
            {
                return _errorMessages;
            }
            set
            {
                _errorMessages = value;
            }
        }

        /// <summary>
        /// Javascript errors logged by the browser.
        /// </summary>
        public string JavascriptErrorMessages
        {
            get
            {
                return _javascriptErrorMessages;
            }
            set
            {
                _javascriptErrorMessages = value;
            }
        }

        /// <summary>
        /// Trace messages logged by testcase or engine through javascript.
        /// </summary>
        public string InfoMessages
        {
            get
            {
                return _infoMessages;
            }
            set
            {
                _infoMessages = value;
            }
        }
    }
}
