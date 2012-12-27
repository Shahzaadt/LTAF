using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Web.Administration;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure
{
    public class WebServerIISExpress : WebServer
    {
        private const string COMMAND_LINE_ARGS_FORMAT = "/config:\"{0}\"";
        public const string PATH_TO_ISSEXPRESS = @"%PROGRAMFILES%\IIS Express\iisexpress.exe";

        private Architecture _architecture = Architecture.Default;
        private int _httpPort = 0;
        private int _startupTimeout = 0;
        private string _serverExecutablePath = "";
        private string _applicationHostConfigPath = "";
        private Process _serverProcess = null;

        private IFileSystem _fileSystem = null;
        private IEnvironmentSystem _environmentSystem = null;
        private IProcessRunner _processRunner = null;

        private bool IsRunning
        {
            get
            {
                return (this._serverProcess != null);
            }
        }

        public WebServerIISExpress()
            : this(new WebServerSettings())
        {
        }

        public WebServerIISExpress(WebServerSettings settings)
            : this(settings, Dependencies.FileSystem, Dependencies.EnvironmentSystem, Dependencies.ProcessRunner)
        {
        }

        internal WebServerIISExpress(WebServerSettings settings, 
                                     IFileSystem fileSystem, 
                                     IEnvironmentSystem environmentSystem, 
                                     IProcessRunner processRunner)
        {
            this._architecture = settings.Architecture;
            this._fileSystem = fileSystem;
            this._environmentSystem = environmentSystem;
            this._processRunner = processRunner;

            this._hostName = settings.HostName;
            this._startupTimeout = settings.StartupTimeout;
            this._rootPhysicalPath = string.IsNullOrEmpty(settings.RootPhysicalPath)
                ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                : settings.RootPhysicalPath;
            this._httpPort = this._environmentSystem.GetNextAvailablePort();
            this._serverExecutablePath = LocateIISExpress();

            this._applicationHostConfigPath = CreateApplicationHostConfigFromTemplate();
            this._serverManager = new ServerManager(_applicationHostConfigPath);

            InitializeApplicationHostConfig();

            Start();
        }

        protected override void DisposeWebServer(bool disposing)
        {
            if (IsRunning)
            {
                 // dispose managed resources
                 Stop();
            }           
        }

        public override void Start()
        {
            // prepare command line params
            string arguments = String.Format(COMMAND_LINE_ARGS_FORMAT, this._applicationHostConfigPath);

            // prepare process parameters
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = this._serverExecutablePath;
            psi.WorkingDirectory = this._rootPhysicalPath;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.Arguments = arguments;

            this._serverProcess = new Process();
            this._serverProcess.StartInfo = psi;

            // try to start process
            if (!this._processRunner.Start(this._serverProcess))
            {
                throw new Exception(string.Format("Failed to start process '{0} {1}'.", this._serverExecutablePath, arguments));
            }

            // wait for process some time - it also gives us some delay before we start requesting 
            if (this._processRunner.WaitForExit(this._serverProcess, this._startupTimeout))
            {
                throw new Exception(string.Format("Process '{0} {1}' exited unexpctedly.", this._serverExecutablePath, arguments));
            }
        }

        public override void Stop()
        {
            if (IsRunning)
            {
                this._processRunner.Stop(_serverProcess);

                this._serverProcess = null;
            }
        }

        public override void Reset()
        {
            Stop();
            Start();
        }

        private string LocateIISExpress()
        {
            string iisExpressPath = "";

            if (this._architecture == Architecture.Default)
            {
                // try to find IISExpress depending on OS arch
                iisExpressPath = this._environmentSystem.ExpandEnvironmentVariables(PATH_TO_ISSEXPRESS);

                if (!this._fileSystem.FileExists(iisExpressPath))
                {
                    iisExpressPath = this._environmentSystem.ExpandEnvironmentVariables(PATH_TO_ISSEXPRESS.Replace("%PROGRAMFILES%", "%PROGRAMFILES(x86)%"));
                }
            }
            else if (this._architecture == Architecture.x86)
            {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        iisExpressPath = this._environmentSystem.ExpandEnvironmentVariables(PATH_TO_ISSEXPRESS.Replace("%PROGRAMFILES%", "%PROGRAMFILES(x86)%"));
                    }
                    else
                    {
                        iisExpressPath = this._environmentSystem.ExpandEnvironmentVariables(PATH_TO_ISSEXPRESS);
                    }
            }
            else if (this._architecture == Architecture.Amd64)
            {
                iisExpressPath = this._environmentSystem.ExpandEnvironmentVariables(PATH_TO_ISSEXPRESS);
            }        

            if (string.IsNullOrEmpty(iisExpressPath) || !this._fileSystem.FileExists(iisExpressPath))
            {
                throw new Exception("IISExpress was not found. Check if it was installed or architecture was specified correctly.");
            }

            this._version = this._fileSystem.FileGetVersion(iisExpressPath);

            return iisExpressPath;
        }

        private string CreateApplicationHostConfigFromTemplate()
        {
            string templatePath = Path.Combine(
                Path.GetDirectoryName(this._serverExecutablePath), @"config\templates\PersonalWebServer\applicationhost.config");

            if (!this._fileSystem.FileExists(templatePath))
            {
                throw new Exception(string.Format("Template file for IIS Express does not exist at '{0}'.", templatePath));
            }

            string newApplicationHostConfigPath = Path.Combine(this._rootPhysicalPath, "applicationhost.config");
            this._fileSystem.FileCopy(templatePath, newApplicationHostConfigPath, true);

            return newApplicationHostConfigPath;
        }

        private void InitializeApplicationHostConfig()
        {
            // remove existing DefaultAppPool
            var existingAppPools = this._serverManager.ApplicationPools
                .Where(a => string.Compare(a.Name, DEFAULT_APPPOOL_NAME, StringComparison.InvariantCultureIgnoreCase) == 0);

            if (existingAppPools.Count() > 0)
            {
                foreach (var pool in existingAppPools)
                {
                    this._serverManager.ApplicationPools.Remove(pool);
                }
            }
          
            // add new DefaultAppPool
            var defaultAppPool = this._serverManager.ApplicationPools.Add(DEFAULT_APPPOOL_NAME);
            defaultAppPool.ManagedRuntimeVersion = "v4.0";
            defaultAppPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
            defaultAppPool.ProcessModel.IdentityType = ProcessModelIdentityType.ApplicationPoolIdentity;
            this._serverManager.ApplicationDefaults.ApplicationPoolName = DEFAULT_APPPOOL_NAME;

            this._serverManager.Sites.Clear();

            var defaultWebSite = this._serverManager.Sites.Add(
                    DEFAULT_WEBSITE_NAME, "http", string.Format("*:{0}:", this._httpPort), this._rootPhysicalPath);
            
            // TODO: What to do with certificates for https? Should test setup add its own certificates?
            // defaultWebSite.Bindings.Add(string.Format("*:{0}:", this._httpsPort), "https");

            this._serverManager.CommitChanges();
        }
    }
}
