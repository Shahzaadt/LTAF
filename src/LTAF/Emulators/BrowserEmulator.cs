using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LTAF.Engine;
using System.IO;
using System.Net;
using System.Web;

namespace LTAF.Emulators
{
    /// <summary>
    /// Impliments limited browser functionality for running tests without UI
    /// </summary>
    public class BrowserEmulator
    {
        /// <summary>Generic event occuring before request was sent to the server </summary>
        public event EventHandler<RequestSendingEventArgs> RequestSending;
        /// <summary>Generic event occuring after response is received from the server </summary>
        public event EventHandler<ResponseReceivedEventArgs> ResponseReceived;


        /// <summary>Interface for the object implementing the actual request</summary>
        private IWebRequestor _webRequestor;
        /// <summary>Interface for the handling all interaction with file system </summary>
        private IFileSystem _fileSystem;
        /// <summary>We want to store all active cookies between requests</summary>
        private CookieContainer _currentCookies;
        /// <summary>Default logger object - writes to Console </summary>
        private IBrowserEmulatorLog _log;
        /// <summary>The last url (url in the browser address bar)</summary>
        private Uri _currentUri;

        /// <summary>
        /// Public ctor
        /// </summary>
        public BrowserEmulator(string baseUrl)
            : this(baseUrl, new WebRequestor(), new BrowserEmulatorLog(), new FileSystem())
        {
        }

        /// <summary>
        /// Constructor for unit tests
        /// </summary>
        internal BrowserEmulator(string baseUrl, IWebRequestor webRequestor, IBrowserEmulatorLog log, IFileSystem fileSystem)
        {
            // update given base url if it has no trailing /
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            // set initial Uri to be equal to root
            this._currentUri = new Uri(baseUrl);
            this._currentCookies = new CookieContainer();
            this._webRequestor = webRequestor;
            this._fileSystem = fileSystem;
            this._log = log;

            // we want to have output in the log
            SilentMode = false;

        }

        /// <summary>
        /// Returns current Uri
        /// </summary>
        public Uri CurrentUri
        {
            get
            {
                return this._currentUri;
            }
        }

        /// <summary>
        /// Specifies if BrowserEmulator should write any messages in default log
        /// </summary>
        public bool SilentMode
        {
            get
            {
                return this._log.SilentMode;
            }
            set
            {
                this._log.SilentMode = value;
            }
        }

        /// <summary>
        /// Creates a IBrowserCommandExecutor to receive commands from the HtmlPage
        /// </summary>
        public IBrowserCommandExecutor CreateCommandExecutor()
        {
            // create command handler instance and sign up for command executing event
            var commandExecutor = new EmulatedBrowserCommandExecutor(this);
            commandExecutor.BrowserCommandExecuting += (o, e) => _log.WriteLine(String.Format("[COMMAND STARTED] {0}, {1}.",
                    e.Command.Handler.ClientFunctionName,
                    e.Command.Description));

            return commandExecutor;
        }

        /// <summary>
        /// Initializes and executes request and handles response. 
        /// </summary>
        internal BrowserInfo ExecuteRequest(string url, string method, string contentType, PostDataCollection postData)
        {
            BrowserInfo browserInfo = new BrowserInfo();

            HttpWebRequest request = this._webRequestor.CreateRequest(url);
            request.Method = method.ToUpperInvariant();

            // Note: cookies should be set before ContentType (request reorganize headers and cookies after that)
            request.AddCookies(this._currentCookies);

            // Note: content type (and length) should be set before data sent to the request stream 
            if (!string.IsNullOrEmpty(contentType))
            {
                request.ContentType = contentType;
            }

            string requestData = "";

            if ((request.Method.Equals("post", StringComparison.OrdinalIgnoreCase)
                    || request.Method.Equals("put", StringComparison.OrdinalIgnoreCase))
                && postData != null)
            {
                if (postData.HasFilesToUpload)
                {
                    //request.KeepAlive = true;
                    // Note: AddMultipartRequestData sets corresponding ContentType 
                    requestData = request.AddMultipartRequestData(postData, this._fileSystem);
                }
                else
                {
                    requestData = request.AddStandardRequestData(postData.GetPostDataString());
                }
            }

            // fire event before request has been sent
            RequestSendingEventArgs args = new RequestSendingEventArgs(request, requestData);
            OnRequestSending(args);
            request = args.Request;

            HttpWebResponse response = null;
            try
            {
                // execute request
                response = this._webRequestor.ExecuteRequest(request);
            }
            catch (WebException e)
            {
                // We catch only WebException here. In this case there should be valid error page,
                // which we want to obtain from the Response. If there was different type of exception,
                // or we have another exception while reading response, then we let it go through.
                if (e.Response != null)
                {
                    response = (HttpWebResponse)e.Response;
                }
                else
                {
                    throw;
                }
            }

            // get raw body
            string body = GetStringFromStream(response.GetResponseStream(), Encoding.UTF8);

            // fire event after responce has been received
            OnResponseReceived(new ResponseReceivedEventArgs(response, body));

            if (!string.IsNullOrEmpty(body))
            {
                // store response body
                browserInfo.Data = "<BrowserEmulatorRoot>" + body + "</BrowserEmulatorRoot>";

                // remember current URL
                this._currentUri = response.ResponseUri;

                return browserInfo;
            }

            throw new InvalidOperationException("Response did not contain html markup.");
        }

        #region Raise Events
        /// <summary>
        /// Fires RequestSending event and output standard log to console
        /// </summary>
        protected virtual void OnRequestSending(RequestSendingEventArgs e)
        {
            if (RequestSending != null)
            {
                RequestSending(this, e);
            }

            this._log.WriteLine("-------------------------------- [REQUEST] ------------------------------");

            if (e.Request != null)
            {
                this._log.WriteLine("[URL]: " + e.Request.RequestUri.AbsoluteUri);
                this._log.WriteLine("[Method]: " + e.Request.Method);

                this._log.WriteLine("[Headers]: ");
                if (e.Request.Headers != null)
                {
                    foreach (string s in e.Request.Headers.AllKeys)
                    {
                        this._log.WriteLine(s + ": " + e.Request.Headers[s]);
                    }
                }

                if (e.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase)
                    || e.Request.Method.Equals("put", StringComparison.OrdinalIgnoreCase))
                {
                    this._log.WriteLine("[Request Data]:");
                    this._log.WriteLine((e.RequestData.Length > 1000) ? e.RequestData.Substring(0, 1000) + " \n [Too long request content....]"
                                                                      : e.RequestData);
                }
            }
        }

        /// <summary>
        /// Fires ResponseReceived event
        /// </summary>
        protected virtual void OnResponseReceived(ResponseReceivedEventArgs e)
        {
            if (ResponseReceived != null)
            {
                ResponseReceived(this, e);
            }

            this._log.WriteLine("-------------------------------- [RESPONSE] ------------------------------");

            if (e.Response != null)
            {
                this._log.WriteLine("[Headers]: ");
                if (e.Response.Headers != null)
                {
                    foreach (string s in e.Response.Headers.AllKeys)
                    {
                        this._log.WriteLine(s + ": " + e.Response.Headers[s]);
                    }
                }

                this._log.WriteLine("[Body]: ");
                this._log.WriteLine(e.Body);
            }
        }

        private string GetStringFromStream(Stream stream, Encoding encoding)
        {
            string result = "";

            if (stream != null && stream.CanRead == true)
            {
                StreamReader reader = (encoding == null) ? new StreamReader(stream) : new StreamReader(stream, encoding);
                result = reader.ReadToEnd();
                reader.Close();
            }

            return result;
        }

        #endregion
    }
}