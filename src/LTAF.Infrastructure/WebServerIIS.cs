using System;
using System.IO;
using Microsoft.Web.Administration;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure
{
    public class WebServerIIS : WebServer
    {
        private IEnvironmentSystem _environmentSystem = null;
        private IFileSystem _fileSystem = null;

        public WebServerIIS() 
            : this(new WebServerSettings(), Dependencies.FileSystem, Dependencies.EnvironmentSystem)
        {
        }

        public WebServerIIS(WebServerSettings settings)
            : this(settings, Dependencies.FileSystem, Dependencies.EnvironmentSystem)
        {
        }

        internal WebServerIIS(WebServerSettings settings, IFileSystem fileSystem, IEnvironmentSystem environmentSystem)
        {
            this._environmentSystem = environmentSystem;
            this._fileSystem = fileSystem;

            this._hostName = settings.HostName;
            this._rootPhysicalPath = string.IsNullOrEmpty(settings.RootPhysicalPath)
                ? this._environmentSystem.ExpandEnvironmentVariables(@"%SystemDrive%\inetpub\wwwroot")
                : settings.RootPhysicalPath;

            this._version = GetIISVersion();

            this._serverManager = new ServerManager();
        }

        private Version GetIISVersion()
        {
            // Note: best way to determine version of IIS is check its file version, but if unit 
            // test using this assembly are executed as x86 process on x64 machine, file may not be
            // found. So we have a back up way to determine IIS version based on OS version.

            Version version;

            string exePath = _environmentSystem.ExpandEnvironmentVariables(@"%windir%\Sysnative\inetsrv\inetinfo.exe");

            if (_fileSystem.FileExists(exePath))
            {
                version = _fileSystem.FileGetVersion(exePath);    
            } 
            else
            {
                var osVersion = _environmentSystem.OSVersion.ToString(2);
                if (osVersion == "6.3")
                {
                    version = new Version(8, 5);
                } 
                else if (osVersion == "6.2")
                {
                    version = new Version(8, 0);
                }
                else if (osVersion == "6.1")
                {
                    version = new Version(7, 5);
                }
                else if (osVersion == "6.0")
                {
                    version = new Version(7, 0);
                }
                else
                {
                    throw new Exception(@"Unable to dtermine version of IIS. Check if IIS is installed and if tests are executed as a process with native OS architecture.");
                }
            }

            return version;
        }
    }
}
