using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Internal.Test;

namespace LTAF.Runner
{
    public class WebServer: IWebServer
    {
        public void CreateVirtualDirectory(string virtualDirectoryName, string path, string serverName)
        {
            IISHelper.CreateVDir(virtualDirectoryName, path, serverName);
        }
    }
}
