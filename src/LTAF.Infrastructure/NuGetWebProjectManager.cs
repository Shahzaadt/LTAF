using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Globalization;
using NuGet;

namespace LTAF.Infrastructure
{
    internal class NuGetWebProjectManager
    {
        private readonly IProjectManager _projectManager;

        public NuGetWebProjectManager(IEnumerable<string> remoteSources, string siteRoot, Version targetFramework)
        {
            var webProject = new NuGetWebProjectSystem(siteRoot);

            if (targetFramework != null)
            {
                webProject.TargetFramework = new FrameworkName(".NetFramework", targetFramework);
            }

            string webRepositoryDirectory = GetWebRepositoryDirectory(siteRoot);
            this._projectManager = new ProjectManager(sourceRepository: new AggregateRepository(PackageRepositoryFactory.Default, remoteSources, true),
                                       pathResolver: new DefaultPackagePathResolver(webRepositoryDirectory),
                                       localRepository: PackageRepositoryFactory.Default.CreateRepository(webRepositoryDirectory),
                                       project: webProject);
        }

        public IPackageRepository LocalRepository
        {
            get
            {
                return this._projectManager.LocalRepository;
            }
        }

        public IPackageRepository SourceRepository
        {
            get
            {
                return this._projectManager.SourceRepository;
            }
        }

        public virtual IQueryable<IPackage> GetRemotePackages(string searchTerms, bool filterPreferred)
        {
            var packages = GetPackages(SourceRepository, searchTerms);

            // Order by download count and Id to allow collapsing 
            return packages.OrderByDescending(p => p.DownloadCount).ThenBy(p => p.Id);
        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return GetPackages(LocalRepository, searchTerms);
        }

        public IEnumerable<IPackage> GetPackagesWithUpdates(string searchTerms, bool filterPreferredPackages)
        {
            var packagesToUpdate = GetPackages(LocalRepository, searchTerms);

            return SourceRepository.GetUpdates(packagesToUpdate, includePrerelease: true, includeAllVersions: true).AsQueryable();
        }

        public IEnumerable<string> InstallPackage(IPackage package)
        {
            return PerformLoggedAction(() =>
            {
                this._projectManager.AddPackageReference(package.Id, package.Version, ignoreDependencies: false, allowPrereleaseVersions: true);
            });
        }

        public IEnumerable<string> UpdatePackage(IPackage package)
        {
            return PerformLoggedAction(() =>
            {
                this._projectManager.UpdatePackageReference(package.Id, package.Version, updateDependencies: true, allowPrereleaseVersions: true);
            });
        }

        public IEnumerable<string> UninstallPackage(IPackage package, bool removeDependencies)
        {
            return PerformLoggedAction(() =>
            {
                this._projectManager.RemovePackageReference(package.Id, forceRemove: false, removeDependencies: removeDependencies);
            });
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return LocalRepository.Exists(package);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return SourceRepository.GetUpdates(new[] { package }, includePrerelease: true, includeAllVersions: true).SingleOrDefault();
        }

        private IEnumerable<string> PerformLoggedAction(Action action)
        {
            ErrorLogger logger = new ErrorLogger();
            this._projectManager.Logger = logger;

            try
            {
                action();
            }
            finally
            {
                this._projectManager.Logger = null;
            }
            return logger.Errors;
        }

        internal static IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return GetPackages(repository.GetPackages(), searchTerm);
        }

        internal static IQueryable<IPackage> GetPackages(IQueryable<IPackage> packages, string searchTerm)
        {
            if (!String.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                packages = packages.Find(searchTerm);
            }
            return packages;
        }

        internal static string GetWebRepositoryDirectory(string siteRoot)
        {
            return Path.Combine(siteRoot, "packages");
        }

        private class ErrorLogger : ILogger
        {
            private readonly IList<string> _errors = new List<string>();

            public IEnumerable<string> Errors
            {
                get
                {
                    return this._errors;
                }
            }

            public void Log(MessageLevel level, string message, params object[] args)
            {
                if (level == MessageLevel.Warning)
                {
                    this._errors.Add(String.Format(CultureInfo.CurrentCulture, message, args));
                }
            }
        }
    }
}
