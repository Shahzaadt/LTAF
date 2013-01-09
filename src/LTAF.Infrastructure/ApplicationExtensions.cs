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

        public static void Deploy(this Application application, string[] filePaths)
        {
            Deploy(application, filePaths, "");
        }

        public static void Deploy(this Application application, string[] filePaths, string relativePathUnderVDir)
        {
            Deploy(application, filePaths, relativePathUnderVDir, Dependencies.FileSystem);
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

        internal static void Deploy(Application application, string[] filePaths, string relativePathUnderVDir, IFileSystem fileSystem)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException("filePaths");
            }

            if (application.VirtualDirectories.Count <= 0)
            {
                throw new Exception(string.Format("Application '{0}' does not have a virtual directory.", application.Path));
            }

            string physicalPath = application.VirtualDirectories[0].PhysicalPath;
            if (!fileSystem.DirectoryExists(physicalPath))
            {
                fileSystem.DirectoryCreate(physicalPath);
            }

            string relativeDirectoryPath = Path.Combine(physicalPath, relativePathUnderVDir);
            if (!fileSystem.DirectoryExists(relativeDirectoryPath))
            {
                fileSystem.DirectoryCreate(relativeDirectoryPath);
            }

            foreach (string sourceFilePath in filePaths)
            {
                if (fileSystem.FileExists(sourceFilePath))
                {
                    string destinationFileName = Path.Combine(relativeDirectoryPath, Path.GetFileName(sourceFilePath));
                    fileSystem.FileCopy(sourceFilePath, destinationFileName, true);
                }
            }
        }
    }
}
