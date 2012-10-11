using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NuGet;

namespace LTAF.Infrastructure.Abstractions
{
    internal class NuGetCore : INuGetCore
    {
        public IEnumerable<string> InstallPackage(string sitePhysicalPath, string packageName, string source, string version, Version targetFramework = null)
        {
            IPackage package = null;
            var packageManager = TryToLocatePackage(sitePhysicalPath, packageName, source, version, targetFramework, out package);

            if (packageManager == null || package == null)
            {
                throw new Exception(string.Format("No package named {0}{1} found at location {2}",
                    packageName, (version ?? ""), source));
            }

            return packageManager.InstallPackage(package);
        }

        private NuGetWebProjectManager TryToLocatePackage(string sitePhysicalPath, string packageName, string source,
                                                     string version, Version targetFramework, out IPackage package)
        {
            System.Version safeVersion = null;
            System.Version.TryParse(version, out safeVersion);

            SemanticVersion semanticVersion = (safeVersion == null) ? null : new SemanticVersion(safeVersion);

            string[] sources = source.Split(';');
            package = null;

            NuGetWebProjectManager packageManager = new NuGetWebProjectManager(sources, sitePhysicalPath, targetFramework);

            package = packageManager.SourceRepository.FindPackage(packageName, semanticVersion);

            if (package == null)
            {
                return null;
            }

            return packageManager;
        }
    }
}
