using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using LTAF.Emulators;

namespace LTAF.UnitTests
{
    internal class MockHttpWebResponse: HttpWebResponse
    {
        static public SerializationInfo GetInfo()
        {
            SerializationInfo info = new SerializationInfo(typeof(string), new FormatterConverter());
            info.AddValue("m_HttpResponseHeaders", new WebHeaderCollection());
            info.AddValue("m_Uri", new Uri("http://localhost"));
            info.AddValue("m_Version", new Version());
            info.AddValue("m_StatusCode", 200);
            info.AddValue("m_ContentLength", 0);
            info.AddValue("m_Verb", "get");
            info.AddValue("m_StatusDescription", "");
            info.AddValue("m_MediaType", "");
            info.AddValue("m_Certificate", null);

            return info;
        }

        private string _body;
        private Uri _responseUri;

        public MockHttpWebResponse(string url, string body)
            : this(url, body, null)
        {
        }

        #pragma warning disable 0618
        
        public MockHttpWebResponse(string url, string body, CookieCollection cookies)
            : base(GetInfo(), new StreamingContext())
        {
            this._responseUri = new Uri(url);
            this._body = body;
            this.Cookies = cookies;
        }

        #pragma warning restore 0618

        public override Stream GetResponseStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(this._body));
        }

        public override Uri ResponseUri 
        {
            get
            {
                return _responseUri;
            }
        }
    }
}
