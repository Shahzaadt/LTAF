using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LTAF.Emulators
{
    /// <summary>
    /// Parameters for RequestSendingEvent
    /// </summary>
    public class RequestSendingEventArgs : EventArgs
    {
        /// <summary>
        /// Public ctor
        /// </summary>
        /// <param name="request">Request data that is going to be sent to server</param>
        /// <param name="requestData">Request data</param>
        public RequestSendingEventArgs(HttpWebRequest request, string requestData)
        {
            Request = request;
            RequestData = requestData;
        }

        /// <summary>Request data that is going to be sent to server</summary>
        public HttpWebRequest Request
        {
            get;
            set;
        }

        /// <summary>Request sending to the server </summary>
        public string RequestData
        {
            get;
            set;
        }
    }
}
