using System;
using System.Collections.Generic;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Exception thrown by the automation framework during a test execution
    /// </summary>
    public class WebTestException: Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        public WebTestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WebTestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
