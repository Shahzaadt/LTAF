using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LTAF.Emulators
{
    /// <summary>
    /// Parameters for ResponseReceivedEvent
    /// </summary>
    public class ResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Public ctor
        /// </summary>
        /// <param name="response">Response parameters that were returned from the server</param>
        /// <param name="body">markup returned in ResponseStream</param>
        public ResponseReceivedEventArgs(HttpWebResponse response, string body)
        {
            Response = response;
            Body = body;
        }

        /// <summary>Response parameters that were returned from the server </summary>
        public HttpWebResponse Response
        {
            get;
            set;
        }

        /// <summary>Response returned from the server </summary>
        public string Body
        {
            get;
            set;
        }
    }
}
