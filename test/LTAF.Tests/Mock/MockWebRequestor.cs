using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using LTAF.Emulators;

namespace LTAF.UnitTests
{

    internal class MockWebRequestor: IWebRequestor
    {
        private Dictionary<string, MockHttpWebResponse> _responses;

        public List<MockHttpWebRequest> RequestHistory
        {
            get; 
            set; 
        }

        public MockWebRequestor()
        {
            _responses = new Dictionary<string, MockHttpWebResponse>();
            RequestHistory = new List<MockHttpWebRequest>();
        }

        public void SetResponseForUrl(string url, MockHttpWebResponse response)
        {
            _responses.Add(url.ToLowerInvariant(), response);
        }

        #region IWebRequestor Members

        public HttpWebRequest CreateRequest(string url)
        {
            return new MockHttpWebRequest(url);
        }

        public HttpWebResponse ExecuteRequest(HttpWebRequest request)
        {   
            RequestHistory.Add((MockHttpWebRequest)request);

            string url = request.RequestUri.AbsoluteUri.ToLowerInvariant();

            if (_responses.ContainsKey(url))
            {
                HttpWebResponse response = (HttpWebResponse)_responses[url];

                if (response.Cookies != null && request.CookieContainer != null)
                {
                    request.CookieContainer.Add(response.Cookies);
                }

                return response;
            }
            else
            {
                throw new InvalidOperationException(String.Format("Mock does not contain a response for url '{0}'", url));
            }
        }

        #endregion
    }
}
