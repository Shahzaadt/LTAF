using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NuGet;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure
{
    public class NuGetManager
    {
        public const string NUGET_DEFAULT_SOURCE = @"https://nuget.org/api/v2/";
        private INuGetCore _nugetCore = null;
        private LTAF.Infrastructure.Abstractions.IFileSystem _fileSystem = null;

        public NuGetManager()
            : this(Dependencies.NuGetCore, Dependencies.FileSystem)
        {
        }

        internal NuGetManager(INuGetCore nugetCore, LTAF.Infrastructure.Abstractions.IFileSystem fileSystem)
        {
            this._nugetCore = nugetCore;
            this._fileSystem = fileSystem;
        }

        public IEnumerable<string> InstallPackages(string sitePhysicalPath, string packagesConfig, string source = "",
                            bool binariesOnly = true, bool latest = true, Version targetFramework = null)
        {
            if (string.IsNullOrEmpty(packagesConfig))
            {
                throw new ArgumentNullException("packagesConfig");
            }

            // parse config and get all packages
            if (!this._fileSystem.FileExists(packagesConfig))
            {
                throw new ArgumentException(string.Format("Packages config was not found at: '{0}'.", packagesConfig));
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(this._fileSystem.FileRead(packagesConfig));

            List<PackageName> packages = new List<PackageName>();
            XmlNodeList xmlPackages = doc.GetElementsByTagName("package");
            foreach (XmlNode p in xmlPackages)
            {
                PackageName package = new PackageName(p.Attributes["id"].Value, new SemanticVersion(p.Attributes["version"].Value));

                packages.Add(package);

                if (p.Attributes["source"] != null && !string.IsNullOrEmpty(p.Attributes["source"].Value))
                {
                    source += (";" + p.Attributes["source"].Value);
                }
            }

            return InstallPackages(sitePhysicalPath, packages, source.Trim(';'), binariesOnly, latest, targetFramework);
        }

        public IEnumerable<string> InstallPackages(string sitePhysicalPath, IEnumerable<PackageName> packages, string source = "",
                            bool binariesOnly = true, bool latest = true, Version targetFramework = null)
        {
            if (packages == null)
            {
                throw new ArgumentNullException("packages");
            }

            List<string> warnings = new List<string>();
            foreach (PackageName p in packages)
            {
                warnings.AddRange(InstallPackage(sitePhysicalPath, p.Name, source, 
                    (latest ? "" : p.Version.ToString()), binariesOnly, targetFramework));
            }

            return warnings;
        }

        public IEnumerable<string> InstallPackage(string appPhysicalPath, string packageName, string source = "",
                            string version = "", bool binariesOnly = true, Version targetFramework = null)
        {
            if (string.IsNullOrEmpty(appPhysicalPath))
            {
                throw new ArgumentNullException("appPhysicalPath");
            }

            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentNullException("packageName");
            }

            if (string.IsNullOrEmpty(source))
            {
                source = NUGET_DEFAULT_SOURCE;
            }
            else
            {
                source += (";" + NUGET_DEFAULT_SOURCE);
            }

            // make app folder writable
            this._fileSystem.DirectorySetAttribute(appPhysicalPath, FileAttributes.Normal);

            string webConfigPath = Path.Combine(appPhysicalPath, "web.config");
            string webConfigPathTemp = Path.Combine(appPhysicalPath, "web.config.temp");

            if (binariesOnly && this._fileSystem.FileExists(webConfigPath))
            {
                // backup web.config                
                this._fileSystem.FileCopy(webConfigPath, webConfigPathTemp, true);
            }

            IEnumerable<string> warnings = this._nugetCore.InstallPackage(appPhysicalPath, packageName, source, version, targetFramework);

            if (binariesOnly && this._fileSystem.FileExists(webConfigPathTemp))
            {
                // restore web.config
                this._fileSystem.FileCopy(webConfigPathTemp, webConfigPath, true);
                this._fileSystem.FileDelete(webConfigPathTemp);
            }

            return warnings;
        }
    }
}
