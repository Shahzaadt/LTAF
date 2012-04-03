using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Exception thrown when an HtmlElement is not found within a collection
    /// </summary>
    public class ElementNotFoundException: WebTestException
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ElementNotFoundException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ElementNotFoundException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
