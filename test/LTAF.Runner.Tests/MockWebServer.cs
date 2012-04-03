using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Runner;

namespace LTAF.Runner.Tests
{
    public class MockWebServer: IWebServer
    {
        public List<string> AppNames
        {
            get;
            set;
        }

        public List<string> AppDirectories
        {
            get;
            set;
        }

        public List<string> ServerNames
        {
            get;
            set;
        }

        public MockWebServer()
        {
            AppDirectories = new List<string>();
            AppNames = new List<string>();
            ServerNames = new List<string>();
        }

        #region IWebServer Members

        public void CreateVirtualDirectory(string virtualDirectoryName, string path, string serverName)
        {
            this.AppNames.Add(virtualDirectoryName);
            this.ServerNames.Add(serverName);
            this.AppDirectories.Add(path);
        }

        #endregion
    }
}
