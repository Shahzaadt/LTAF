using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF;
using LTAF.Engine;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LTAF.RemoteService
{
    /// <summary>
    /// The remote LTAF service that can broker commands between test and browser
    /// </summary>
    public class LTAFService : IBrowserCommandExecutor, IBrowserCommandExecutorFactory, IApplicationPathFinder
    {
        private const string DEFAULT_SERVICE_NAME = "LTAFService";
        private const string ERROR_SERVICE_INIT_FAILED = "Failed to initialize LTAF Service. Make sure application under test and webservice are on the same domain.";

        private string _appUrl;
        private string _hostName;
        private string _framesetUrl;
        private WebServiceProxy _webService;
        private Process _browserProcess;

        /// <summary>
        /// Initializes services with localhost and base url
        /// </summary>
        /// <param name="baseUrl">The base url of the application under test</param>
        public LTAFService(string baseUrl): this("localhost", baseUrl)
        {
        }


        /// <summary>
        /// Initializes services with hostname and base url
        /// </summary>
        /// <param name="hostName">The server that hosts the application</param>
        /// <param name="baseUrl">The base url of the application under test</param>
        public LTAFService(string hostName, string baseUrl)
        {
            _appUrl = baseUrl;
            _hostName = hostName;
            this.ServiceName = DEFAULT_SERVICE_NAME;
        }

        /// <summary>
        /// Starts the service and the browser
        /// </summary>
        public void Start()
        {
            ServiceLocator.ApplicationPathFinder = this;
            ServiceLocator.BrowserCommandExecutorFactory = this;

            _webService = new WebServiceProxy();
            _webService.Url = String.Format("http://{0}/{1}/WebService.asmx", _hostName, this.ServiceName);
            _framesetUrl = String.Format("http://{0}/{1}/FrameSet.aspx", _hostName, this.ServiceName);

            EnsureServiceDeployed(_hostName, this.ServiceName);

            InitializeService();

            LaunchBrowser();

            VerifyServiceCommunicationLoop();
        }

        /// <summary>
        /// Verify that LTAFService can send commands to browser and receive a response
        /// </summary>
        private void VerifyServiceCommunicationLoop()
        {
            BrowserCommand firstCommand = new BrowserCommand(BrowserCommand.FunctionNames.GetPageDom);
            firstCommand.Handler.RequiresElementFound = false;
            firstCommand.Description = "Startup";

            try
            {
                this.ExecuteCommand(0, null, firstCommand, 60 * 4 /*timeout*/);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(ERROR_SERVICE_INIT_FAILED, e);
            }
        }

        /// <summary>
        /// Lanches the browser specified
        /// </summary>
        private void LaunchBrowser()
        {
            string browser = "iexplore.exe";
            if (!String.IsNullOrEmpty(Browser))
            {
                browser = Browser + ".exe";
            }
            ProcessStartInfo browserInfo = new ProcessStartInfo(browser, _framesetUrl);
            _browserProcess = Process.Start(browserInfo);
        }

        /// <summary>
        /// Initializes the remote service
        /// </summary>
        private void InitializeService()
        {
            _webService.CreateBrowserQueue();
        }


        /// <summary>
        /// Ensure that the webservice is deployed
        /// </summary>
        private void EnsureServiceDeployed(string hostName, string appName)
        {
            string rootDrive = Path.GetPathRoot(Environment.SystemDirectory);

            string servicePath = Path.Combine(rootDrive, Path.Combine(@"inetpub\wwwroot", appName));

            if (!Directory.Exists(servicePath))
            {
                DeployService(hostName, appName, servicePath);
            }
        }

        /// <summary>
        /// Deployes the webservice.
        /// </summary>
        /// <param name="hostName">Name of the server that hosts the service</param>
        /// <param name="appName">Name of the application that hosts the service</param>
        /// <param name="physicalPath">The physical path to the webservice application</param>
        public void DeployService(string hostName,string appName, string physicalPath)
        {
            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }

            Assembly ltafAssembly = Assembly.GetAssembly(typeof(BrowserCommand));

            //deploy LTAF assembly
            DirectoryInfo binDir = Directory.CreateDirectory(Path.Combine(physicalPath, "bin"));
            string ltafPath = ltafAssembly.Location;
            File.Copy(ltafPath, Path.Combine(binDir.FullName, Path.GetFileName(ltafPath)));

            // deploy all files
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            File.WriteAllText(
                Path.Combine(physicalPath, "driver.css"),
                GetResourceContent(currentAssembly, "LTAF.Support.Resources.driver.css"));
            File.WriteAllText(
                Path.Combine(physicalPath, "DriverPage.aspx"),
                GetResourceContent(currentAssembly, "LTAF.Support.Resources.DriverPage.aspx"));
            File.WriteAllText(
                Path.Combine(physicalPath, "FrameSet.aspx"),
                GetResourceContent(currentAssembly, "LTAF.Support.Resources.FrameSet.aspx"));
            File.WriteAllText(
                Path.Combine(physicalPath, "WebService.asmx"),
                GetResourceContent(currentAssembly, "LTAF.Support.Resources.WebService.asmx"));
            File.WriteAllText(
                Path.Combine(physicalPath, "web.config"),
                GetResourceContent(currentAssembly, "LTAF.Support.Resources.web.config"));

            StringBuilder scriptBuilder = new StringBuilder();
            scriptBuilder.AppendLine(GetResourceContent(ltafAssembly, "LTAF.Engine.Resources.TestcaseExecutor.js"));
            scriptBuilder.AppendLine(GetResourceContent(currentAssembly, "LTAF.Support.Resources.RemoteTestcaseExecutor.js"));
            File.WriteAllText(
                Path.Combine(physicalPath, "ScriptLibrary.js"),
                scriptBuilder.ToString());

            //create Vdir
            IISHelper.CreateVDir(appName, physicalPath, hostName);

        }

        /// <summary>
        /// Extracts a string resource from an assembly
        /// </summary>
        private string GetResourceContent(Assembly containingAssembly, string resourceName)
        {
            using (StreamReader reader = new StreamReader(containingAssembly.GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// The browser to use when running tests
        /// </summary>
        public string Browser
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the remote web service
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Closed the browser window
        /// </summary>
        public void Stop()
        {
            if (_browserProcess != null && !_browserProcess.HasExited)
            {
                _browserProcess.CloseMainWindow();
            }
        }

        #region IBrowserCommandExecutor Members
        public BrowserInfo ExecuteCommand(int threadId, HtmlElement source, BrowserCommand command, int secondsTimeout)
        {
            return _webService.ExecuteCommand(command, secondsTimeout);
        }

        #endregion

        #region IBrowserCommandExecutorFactory Members

        public IBrowserCommandExecutor CreateBrowserCommandExecutor(string applicationPath, HtmlPage page)
        {
            HtmlElementCollection.FindElementTimeout = 5;
            return this;
        }

        #endregion
        #region IApplicationPathFinder Members
        public string GetApplicationPath()
        {
            return _appUrl;
        }
        #endregion
    }
}
