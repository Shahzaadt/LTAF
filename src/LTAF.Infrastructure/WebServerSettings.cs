using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Infrastructure
{
    public class WebServerSettings
    {
        public int StartupTimeout { get; set; } // milliseconds
        public Architecture Architecture { get; set; }
        public string RootPhysicalPath { get; set; }
        public string HostName { get; set; }

        public WebServerSettings()
        {
            StartupTimeout = 5000;
            Architecture = Infrastructure.Architecture.Default;
            RootPhysicalPath = "";
            HostName = "localhost";
        }
    }
}
