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
    internal class MockHttpWebRequest : HttpWebRequest
    {
        static public SerializationInfo GetInfo()
        {
            SerializationInfo info = new SerializationInfo(typeof(string), new FormatterConverter());
            info.AddValue("_HttpRequestHeaders", new WebHeaderCollection());
            info.AddValue("_Proxy", null);
            info.AddValue("_KeepAlive", false);
            info.AddValue("_Pipelined", true);
            info.AddValue("_AllowAutoRedirect", true);
            info.AddValue("_AllowWriteStreamBuffering", true);
            info.AddValue("_HttpWriteMode", 0);
            info.AddValue("_MaximumAllowedRedirections", 10);
            info.AddValue("_AutoRedirects", 0);
            info.AddValue("_Timeout", 30000);
            info.AddValue("_OriginVerb", "");
            info.AddValue("_ConnectionGroupName", null);
            info.AddValue("_ContentLength", 0);
            info.AddValue("_MediaType", ""); 
            info.AddValue("_Version", new Version(1, 0));
            info.AddValue("_OriginUri", new Uri("http://localhost"));

            return info;
        }

        private Uri _uri = null;
        private MemoryStream _stream = null;

        #pragma warning disable 0618

        public MockHttpWebRequest(string url)
            : base(GetInfo(), new StreamingContext())
        {
            this._uri = new Uri(url);
        }

        #pragma warning restore 0618

        public override Uri RequestUri
        {
            get
            {
                return this._uri;
            }
        }

        public override Stream GetRequestStream()
        {
            this._stream = new MemoryStream();
            return this._stream;
        }

        public string GetRequestData()
        {
            string result = "";

            if (this._stream != null)
            {
                byte[] w = this._stream.GetBuffer();
                result = Encoding.UTF8.GetString(w);
            }

            return result;
        }
    }
}
