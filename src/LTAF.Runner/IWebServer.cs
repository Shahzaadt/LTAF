using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF.Runner
{
    public interface IWebServer
    {
        void CreateVirtualDirectory(string virtualDirectoryName, string path, string serverName);
    }
}
