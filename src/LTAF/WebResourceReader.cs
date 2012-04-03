using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace LTAF
{
    /// <summary>
    /// Class capable of reading resources from the assemblies referenced by website
    /// </summary>
    /// <change date="12/17/2007">Created</change>
    public class WebResourceReader : IWebResourceReader
    {
        private IWebApplication _webApplication;
        private IWebResourceReader _resourceReader;

        /// <summary>
        /// ctor
        /// </summary>
        public WebResourceReader()
            : this(new WebApplication())
        {
        }

        internal WebResourceReader(IWebApplication webApplication)
        {
            _webApplication = webApplication;
            _resourceReader = ServiceLocator.WebResourceReader;
            if (_resourceReader == null)
            {
                _resourceReader = this;
            }
        }

        /// <summary>
        /// Gets a string from the resources of an assembly
        /// </summary>
        /// <param name="assemblyName">The partial name of the assembly</param>
        /// <param name="resourceName">The name of the embedded resource</param>
        /// <param name="resourceKey">The resource key</param>
        /// <returns>The resource string if found.</returns>
        public string GetString(string assemblyName, string resourceName, string resourceKey)
        {
            if (assemblyName.EndsWith(".dll"))
            {
                assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
            }

            string resource = _resourceReader.ReadString(assemblyName, resourceName, resourceKey);

            if (String.IsNullOrEmpty(resource))
            {
                throw new WebTestException(String.Format("Failed to load resource string. AssemblyName='{0}'. ResourceName='{1}'. ResourceKey='{2}'", assemblyName, resourceName, resourceKey));
            }

            return resource;
        }

        /// <summary>
        /// Gets a string from the resources of an assembly
        /// </summary>
        /// <param name="assembly">instance of assembly to load resource string from</param>
        /// <param name="resourceName">The name of the embedded resource</param>
        /// <param name="resourceKey">The resource key</param>
        /// <returns>The resource string if found.</returns>
        /// <change date="12/21/2007">Created</change>
        public string GetString(Assembly assembly, string resourceName, string resourceKey)
        {
            if (assembly == null)
            {
                throw new WebTestException("Failed to load resource string. Assembly provided was null.");
            }

            string resource = string.Empty;

            try
            {
                resource = _webApplication.GetResourceString(assembly, resourceName, resourceKey);
            }
            catch (Exception e)
            {
                throw new WebTestException(
                    String.Format("Error occurred while attempting to extract resource from assembly. Assembly: '{0}'. ResourceName: '{1}'. ResourceId: '{2}'",
                        assembly.FullName,
                        resourceName,
                        resourceKey), e);
            }

            if (String.IsNullOrEmpty(resource))
            {
                throw new WebTestException(String.Format("Failed to load resource string. AssemblyName='{0}'. ResourceName='{1}'. ResourceKey='{2}'", 
                        assembly.FullName, 
                        resourceName, 
                        resourceKey));
            }

            return resource;
        }

        #region IWebResourceReader Members
        /// <summary>
        /// Method that loads the asembly specified from all the assemblies referenced by website, and extracts the given resource string
        /// </summary>
        string IWebResourceReader.ReadString(string assemblyName, string resourceName, string resourceKey)
        {
            ICollection<string> assemblies;
            assemblies = _webApplication.GetReferencedAssemblyFullNames();

            string fullAssemblyName = null;
            foreach (string asm in assemblies)
            {
                string tempAssemblyName = assemblyName;
                if (!tempAssemblyName.Contains(","))
                {
                    tempAssemblyName += ",";
                }

                if (asm.StartsWith(tempAssemblyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    fullAssemblyName = asm;
                    break;
                }
            }

            if (fullAssemblyName == null)
            {
                throw new WebTestException(String.Format("Assembly '{0}' was not found on the web application's referenced assemblies", assemblyName));
            }

            try
            {
                return _webApplication.GetResourceString(fullAssemblyName, resourceName, resourceKey);
            }
            catch (Exception e)
            {
                throw new WebTestException(
                    String.Format("Error occurred while attempting to extract resource from assembly. Assembly: '{0}'. ResourceName: '{1}'. ResourceId: '{2}'",
                        fullAssemblyName,
                        resourceName,
                        resourceKey), e);
            }
        }

        #endregion
    }
}
