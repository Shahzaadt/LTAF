using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web.Compilation;
using System.Reflection;
using System.Resources;
using System.Globalization;


namespace LTAF
{
    internal class WebApplication : IWebApplication
    {
        private static ICollection<string> _referencedAssemblies = null;
        private static object _lock = new object();

        public ICollection<string> GetReferencedAssemblyFullNames()
        {
            lock (_lock)
            {
                if (_referencedAssemblies == null)
                {
                    ICollection assemblies = BuildManager.GetReferencedAssemblies();
                    _referencedAssemblies = new List<string>();
                    foreach (Assembly asm in assemblies)
                    {
                        _referencedAssemblies.Add(asm.FullName);
                    }
                }
                return _referencedAssemblies;
            }
        }

        public string GetResourceString(string fullAssemblyName, string resourceName, string resourceKey)
        {
            return GetResourceString(Assembly.Load(fullAssemblyName), resourceName, resourceKey);
        }

        public string GetResourceString(Assembly assembly, string resourceName, string resourceKey)
        {
            ResourceManager rm = new ResourceManager(resourceName, assembly);
            // This method loads the resources for the specified culture
            ResourceSet resourceSet = rm.GetResourceSet(CultureInfo.CurrentUICulture, true /*createIfNotExists*/, true /*tryParents*/);
            return rm.GetString(resourceKey);
        }
    }
}
