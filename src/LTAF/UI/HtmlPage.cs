using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using LTAF.Engine;
using LTAF.Emulators;
using System.Threading;
using System.Text.RegularExpressions;

namespace LTAF
{
    /// <summary>
    /// Class that represents a web page as seen by the browser than can be automated.
    /// </summary>
    public class HtmlPage
    {
        private const int EXECUTE_COMMAND_TIMEOUT = 60;
        private const int REFRESH_TIMEOUT_DEFAULT = 5;
        
        private const string ERROR_APPLICATIONPATH_NULL = "'ApplicationPath' is required when creating an HtmlPage. "+
            "Pass it through the constructor or make sure a valid ApplicationPathFinder has been registered with the ServiceLocator.";

        private static string _aspnetErrorRegexPattern;
        private static EmulatedBrowserCommandExecutorFactory _defaultFactory = new EmulatedBrowserCommandExecutorFactory();

        private readonly int _threadId;
        private HtmlElementCollection _elements;
        private string _applicationPath = null;

        /// <summary>The index of this window used to locate it in the client</summary>
        internal int WindowIndex { get; set; }

        /// <summary>
        /// The frame hierarchy to use to reach the page under test
        /// </summary>
        internal List<string> FrameHierachy { get; set; }

        /// <summary>
        /// Constructs a new HtmlPage to interact with the page loaded in the browser
        /// </summary>
        public HtmlPage()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor that automatically navigates to url specified
        /// </summary>
        /// <param name="navigateUrl">The Url to navigate to</param>
        public HtmlPage(string navigateUrl)
            : this(null, navigateUrl)
        {
        }


        /// <summary>
        /// Constructs a new HtmlPage with an application path to be used to resolve relative paths
        /// </summary>
        /// <param name="applicationPath">Application path used to resolve relative paths</param>
        public HtmlPage(Uri applicationPath)
            : this(applicationPath.AbsoluteUri, null)
        {
        }

        /// <summary>
        /// Constructs a new HtmlPage with an application path to be used to resolve relative paths and navigates to target page
        /// </summary>
        /// <param name="applicationPath">The application path used to resolve relative paths</param>
        /// <param name="navigateUrl">The Url to navigate to</param>
        private HtmlPage(string applicationPath, string navigateUrl)
        {
            if (applicationPath ==  null)
            {
                applicationPath = ServiceLocator.ApplicationPathFinder.GetApplicationPath();

                if (applicationPath == null)
                {
                    throw new InvalidOperationException(ERROR_APPLICATIONPATH_NULL);
                }
            }

            // ensure that DNS name of the host is punycoded (when it is necessary for 
            applicationPath = EnsureDnsSafePath(applicationPath);

            _threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            _elements = new HtmlElementCollection(this);
            _applicationPath = applicationPath;
            FrameHierachy = new List<string>();

            if (ServiceLocator.BrowserCommandExecutorFactory != null)
            {
                this.BrowserCommandExecutor = ServiceLocator.BrowserCommandExecutorFactory.CreateBrowserCommandExecutor(applicationPath, this);
            }
            else
            {
                this.BrowserCommandExecutor = _defaultFactory.CreateBrowserCommandExecutor(applicationPath, this);
            }

            if (navigateUrl != null)
            {
                this.Navigate(navigateUrl);
            }
        }

        /// <summary>
        /// Ensure that DNS name of the host is punycoded (when it is necessary)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureDnsSafePath(string path)
        {
            string safePath = path;

            if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    path = "http://" + path;
                }
            }

            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                Uri uri = new Uri(path);
                safePath = uri.Scheme + "://" + uri.DnsSafeHost + (uri.Port != 80 && uri.Port > 0 ? ":" + uri.Port.ToString() : "") + uri.PathAndQuery;
            }

            return safePath;
        }


        #region Properties
        /// <summary>
        /// The command executor to use when issuing command for this page.
        /// </summary>
        internal IBrowserCommandExecutor BrowserCommandExecutor 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The Dom elements that are exposed by this web page.
        /// </summary>
        public HtmlElementCollection Elements
        {
            get
            {
                return _elements;
            }
        }

        /// <summary>
        /// The root Dom element exposed by this web page
        /// </summary>
        public HtmlElement RootElement
        {
            get 
            {
                if (this.Elements.Count == 0)
                {
                    throw new InvalidOperationException("Dom has not been loaded, please call Navigate() before trying to access the RootElement property.");
                }

                return this.Elements[0]; 
            }
        }
        #endregion

        #region ExecuteCommand

        /// <summary>
        /// Execute a command on this web page.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>BrowserInfo object containing results from the command execution</returns>
        public BrowserInfo ExecuteCommand(BrowserCommand command)
        {
            return ExecuteCommand(null, command);
        }
        

        /// <summary>
        /// Execute a command on this web page.
        /// </summary>
        /// <param name="source">HtmlElement that initiated this command, null if none</param>
        /// <param name="command">The command to execute</param>
        /// <returns>BrowserInfo object containing results from the command execution</returns>
        public virtual BrowserInfo ExecuteCommand(HtmlElement source, BrowserCommand command)
        {
            command.Target.WindowIndex = this.WindowIndex;
            if (this.WindowIndex > 0)
            {
                command.Description += String.Format(" (Popup window: {0})", this.WindowIndex);
            }
            if (this.FrameHierachy.Count > 0)
            {
                string[] frames = this.FrameHierachy.ToArray();   
                command.Description += String.Format(" (Frame: {0})", String.Join("-", frames));
                command.Target.FrameHierarchy = frames;
            }

            command.Traces = WebTestConsole.GetTraces();

            BrowserInfo browserInfo = null;

            try
            {

                browserInfo = this.BrowserCommandExecutor.ExecuteCommand(Thread.CurrentThread.ManagedThreadId, source, command, EXECUTE_COMMAND_TIMEOUT);
            }
            finally
            {
                WebTestConsole.Clear();
            }

            if (browserInfo != null)
            {
                if (string.IsNullOrEmpty(browserInfo.ErrorMessages))
                {
                    return browserInfo;
                }
                else
                {
                    throw new WebTestException("Exception was thrown by the client engine: " + browserInfo.ErrorMessages + " when running command: " +
                        command.Description + " on the target: <" + command.Target.TagName + " id=" + command.Target.Id + ", index: " + command.Target.Index + ">.");
                }
            }

            throw new TimeoutException(string.Format("BrowserCommand \"{0}\" timed out after {1} seconds(s)!", command.Description, EXECUTE_COMMAND_TIMEOUT));
        }
        #endregion

        #region Navigate
        /// <summary>
        /// Navigate to a url and load its DOM.
        /// </summary>
        /// <param name="url">The absolute or relative url (relative to app root) to navigate to.</param>
        public void Navigate(string url)
        {
            Navigate(url, NavigationVerification.Default);
        }

        /// <summary>
        /// Navigate to a url and load its DOM.
        /// </summary>
        /// <param name="url">The absolute or relative url (relative to app root) to navigate to.</param>
        /// <param name="navigationVerificationMode">NavigationVerification mode</param>
        public void Navigate(string url, NavigationVerification navigationVerificationMode)
        {
            Navigate(url, navigationVerificationMode, 0);
        }

        /// <summary>
        /// Returns an absolute url based on the current application path.
        /// </summary>
        /// <param name="url">The relative url.</param>
        /// <returns>The absolute url.</returns>
        protected internal string ResolveNavigateUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return url;
            }
            else if (url == "/" || String.IsNullOrEmpty(url))
            {
                return this._applicationPath;
            }
            else
            {
                string appPath = this._applicationPath.EndsWith("/") ? this._applicationPath : this._applicationPath + "/";
                return appPath + url.TrimStart('/');
            }
        }


        /// <summary>
        /// Navigate to this web page.
        /// </summary>
        /// <param name="url">The absolute or relative url (relative to app root) to navigate to.</param>
        /// <param name="navigationVerificationMode">NavigationVerification mode</param>
        /// <param name="millisecondsWaitToLoad">Time in milliseconds after navigate to wait for page to be fully loaded.</param>
        protected virtual void Navigate(string url, NavigationVerification navigationVerificationMode, int millisecondsWaitToLoad)
        {
            string absoluteUrl = ResolveNavigateUrl(url);

            BrowserCommand command = new BrowserCommand();
            command.Description = String.Format("Navigate - {0}", absoluteUrl);
            command.Handler.RequiresElementFound = false;
            command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.NavigateToUrl;
            command.Handler.SetArguments(absoluteUrl, navigationVerificationMode);
            this.ExecuteCommand(command);

            if (millisecondsWaitToLoad > 0)
            {
                System.Threading.Thread.Sleep(millisecondsWaitToLoad);
            }

            this.Elements.Refresh();
        }
        #endregion

        #region GetCurrentUrl
        /// <summary>
        /// Returns the url of the page currently loaded
        /// </summary>
        /// <returns>Url of the page currently loaded</returns>
        public string GetCurrentUrl()
        {
            BrowserCommand command = new BrowserCommand(BrowserCommand.FunctionNames.GetCurrentUrl);
            command.Description = "GetCurrentUrl";
            command.Handler.RequiresElementFound = false;
            return this.ExecuteCommand(command).Data;
        }
        #endregion

        #region ExecuteScript
        /// <summary>
        /// Evaluates a custom script expression in the context of the page under test
        /// </summary>
        /// <param name="scriptExpression">The javascript expression to evaluate</param>
        public object ExecuteScript(string scriptExpression)
        {
            BrowserCommand command = new BrowserCommand();
            command.Description = "ExecuteScript";
            command.Handler.RequiresElementFound = false;
            command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.ExecuteScript;
            command.Handler.SetArguments(scriptExpression);

            BrowserInfo result = this.ExecuteCommand(command);
            if(result == null || string.IsNullOrEmpty(result.Data))
            {
                return null;
            }
            else
            {
                return TestDriverPage.SystemWebExtensionsAbstractions.DeserializeJson(result.Data);
            }
        }
        #endregion

        #region WaitForAsyncPostComplete
        /// <summary>
        /// Method that waits until no asynchronous post is in execution
        /// </summary>
        public void WaitForAsyncPostComplete()
        {
            this.WaitForScript("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack() == false", 1000);
        }
        #endregion

        #region WaitForScript
        /// <summary>
        /// Method that waits until a custom script expression that is evaluated in the context of the 
        /// page under test returns true
        /// </summary>
        /// <param name="scriptExpression">The javascript expression to evaluate</param>
        /// <param name="timeoutInSeconds">The timeout in seconds to keep trying the expression before fail.</param>
        public void WaitForScript(string scriptExpression, int timeoutInSeconds)
        {
            BrowserCommand command = new BrowserCommand();
            command.Description = "WaitForScript";
            command.Handler.RequiresElementFound = false;
            command.Handler.ClientFunctionName = BrowserCommand.FunctionNames.WaitForScript;
            command.Handler.SetArguments(scriptExpression, timeoutInSeconds * 1000);

            this.ExecuteCommand(command);
        }
        #endregion 

        #region GetPopupWindow
        /// <summary>
        /// Method that gets a reference to an HTML popup page
        /// </summary>
        /// <param name="index">Zero based index of windows opened by driver</param>
        /// <param name="timeoutInSeconds">Timeout to wait for the popup window to finish loading</param>
        /// <param name="waitBetweenAttemptsInMilliseconds">Milliseconds to wait between attempting to get popup page</param>
        /// <returns>HtmlPage to interact with the supplied popup window</returns>
        public HtmlPage GetPopupPage(int index, int timeoutInSeconds = REFRESH_TIMEOUT_DEFAULT, int waitBetweenAttemptsInMilliseconds = 500)
        {
            HtmlPage popupPage = new HtmlPage();
            popupPage.WindowIndex = index;

            Exception exception = null;
            DateTime timeout = DateTime.Now.AddSeconds(timeoutInSeconds);

            // Wait until the popup page can be refreshed, or until timeout
            while (!TryPageRefresh(popupPage, out exception) && DateTime.Now < timeout)
            {
                Thread.Sleep(waitBetweenAttemptsInMilliseconds);
            }

            if (exception != null)
            {
                throw new WebTestException(
                    String.Format("Failed to retrieve the popup window after {0} seconds.", timeoutInSeconds),
                    exception);
            }

            return popupPage;
        }
        #endregion 
        
        #region GetFrameWindow
        /// <summary>
        /// Method that gets a reference to an HTML frame
        /// </summary>
        /// <param name="frameNames">One or more frame names reachable from the current page</param>
        /// <returns>HtmlPage to interact with the supplied frame</returns>
        public HtmlPage GetFramePage(params string[] frameNames)
        {
            return GetFramePage(frameNames, REFRESH_TIMEOUT_DEFAULT);
        }

        /// <summary>
        /// Method that gets a reference to an HTML frame
        /// </summary>
        /// <param name="frameNames">One or more frame names reachable from the current page</param>
        /// <param name="timeoutInSeconds">Timeout to wait for the target frame to finish loading</param>
        /// <param name="waitBetweenAttemptsInMilliseconds">Milliseconds to wait between attempting to get frame</param>
        /// <returns>HtmlPage to interact with the supplied frame</returns>
        public HtmlPage GetFramePage(string[] frameNames, int timeoutInSeconds = REFRESH_TIMEOUT_DEFAULT, int waitBetweenAttemptsInMilliseconds = 500)
        {
            HtmlPage framePage = new HtmlPage();
            framePage.FrameHierachy.AddRange(this.FrameHierachy);
            framePage.FrameHierachy.AddRange(frameNames);

            Exception exception = null;
            DateTime timeout = DateTime.Now.AddSeconds(timeoutInSeconds);

            // Wait until the frame page can be refreshed, or until timeout
            while (!TryPageRefresh(framePage, out exception) && DateTime.Now < timeout)
            {
                Thread.Sleep(waitBetweenAttemptsInMilliseconds);
            }

            if (exception != null)
            {
                throw new WebTestException(
                    String.Format("Failed to retrieve the frame after {0} seconds.", timeoutInSeconds),
                    exception);
            }

            return framePage;
        }
        #endregion

        /// <summary>
        /// Helper method that attempts to refresh the elements collection of a page
        /// </summary>
        private bool TryPageRefresh(HtmlPage page, out Exception exception)
        {
            exception = null;
            try
            {
                page.Elements.Refresh();
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns whether the current page is an Asp.Net server error page.
        /// </summary>
        /// <returns>True if current page is an Asp.Net server error page, false otherwise</returns>
        public virtual bool IsServerError()
        {
            HtmlElementFindParams findParams = new HtmlElementFindParams("h1", 0);
            if (this.Elements.Exists(findParams, 0))
            {
                if (_aspnetErrorRegexPattern == null)
                {
                    WebResourceReader reader = new WebResourceReader();
                    string aspnetErrorStringFormatter = reader.GetString("System.Web", "System.Web", "Error_Formatter_ASPNET_Error"); //Server Error in '{0}' Application.
                    _aspnetErrorRegexPattern = String.Format(aspnetErrorStringFormatter, ".+?");
                }
                
                HtmlElement h1 = this.Elements.Find(findParams, 0);
                return Regex.IsMatch(h1.CachedInnerText, _aspnetErrorRegexPattern , RegexOptions.Singleline);
            }
            return false;

        }
    }
}
