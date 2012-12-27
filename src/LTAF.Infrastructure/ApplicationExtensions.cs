using System;
using System.IO;
using Microsoft.Web.Administration;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure
{
    public static class ApplicationExtensions
    {
        public static void Deploy(this Application application, string sourceDir)
        {
            Deploy(application, new DirectoryInfo(sourceDir));
        }

        public static void Deploy(this Application application, DirectoryInfo sourceDir)
        {
            Deploy(application, sourceDir.FullName, Dependencies.FileSystem);

        }

        public static void Deploy(this Application application, string relativeFilePath, string fileContents)
        {
            Deploy(application, relativeFilePath, fileContents, Dependencies.FileSystem);
        }

        internal static void Deploy(Application application, string sourceDir, IFileSystem fileSystem)
        {
            if (!fileSystem.DirectoryExists(sourceDir))
            {
                throw new Exception(string.Format("Failed to deploy files to application, source directory does not exist: '{0}'.", sourceDir));
            }

            if (application.VirtualDirectories.Count <= 0)
            {
                throw new Exception(string.Format("Application '{0}' does not have a virtual directory.", application.Path));
            }

            string targetDir = application.VirtualDirectories[0].PhysicalPath;
            if (!fileSystem.DirectoryExists(targetDir))
            {
                fileSystem.DirectoryCreate(targetDir);
            }

            fileSystem.DirectoryCopy(sourceDir, targetDir);
        }

        internal static void Deploy(Application application, string relativeFilePath, string fileContents, IFileSystem fileSystem)
        {
            if (application.VirtualDirectories.Count <= 0)
            {
                throw new Exception(string.Format("Application '{0}' does not have a virtual directory.", application.Path));
            }

            string targetFilePath = Path.Combine(application.VirtualDirectories[0].PhysicalPath, relativeFilePath);

            fileSystem.FileWrite(targetFilePath, fileContents);
        }

    }
}
