using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace LTAF
{
    internal interface IWebApplication
    {
        ICollection<string> GetReferencedAssemblyFullNames();

        string GetResourceString(string assemblyName, string resourceName, string resourceKey);
        string GetResourceString(Assembly assembly, string resourceName, string resourceKey);
    }
}
