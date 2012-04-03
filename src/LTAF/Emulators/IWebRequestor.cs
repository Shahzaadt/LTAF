using System;
using System.Net;

namespace LTAF.Emulators
{
    internal interface IWebRequestor
    {
        HttpWebRequest CreateRequest(string url);
        HttpWebResponse ExecuteRequest(HttpWebRequest request);
    }
}
