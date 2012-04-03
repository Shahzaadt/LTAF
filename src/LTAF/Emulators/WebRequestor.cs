using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using LTAF.Engine;

namespace LTAF.Emulators
{
    /// <summary>
    /// Class that processes actual requests
    /// </summary>
    internal class WebRequestor : IWebRequestor
    {
        public HttpWebRequest CreateRequest(string url)
        {
            return (HttpWebRequest)WebRequest.Create(url);            
        }

        public HttpWebResponse ExecuteRequest(HttpWebRequest request)
        {
            HttpWebResponse response = null;

            if (request != null)
            {
                response = (HttpWebResponse)request.GetResponse();
            }

            return response;
        }
    }
}
