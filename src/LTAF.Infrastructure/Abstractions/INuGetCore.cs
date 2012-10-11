using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LTAF.Infrastructure.Abstractions
{
    internal interface INuGetCore
    {
        IEnumerable<string> InstallPackage(string sitePhysicalPath, string packageName, 
                                           string source, string version, Version targetFramework);
    }
}
