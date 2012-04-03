using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;

namespace LTAF.Runner
{
    public interface IFileSystem
    {
        bool FileExists(string filePath);

        bool DirectoryExists(string directoryPath);

        IEnumerable<string> GetSubDirectories(string directoryPath);

        void CopyFile(string sourcePath, string destinationPath);

        string CurrentDirectory { get; }

        IEnumerable<string> GetFilesInDirectory(string directoryPath);

        void DeleteFile(string filePath);

        string ReadFile(string filePath);

        void CreateDirectory(string directoryPath);

        void DeleteDirectory(string directoryPath);

        void WriteFile(string filePath, string content);

        FileAttributes GetFileAttributes(string filePath);

        void SetFileAttributes(string filePath, FileAttributes fileAttributes);

        void AddDirectoryAccessRule(string directoryPath, FileSystemAccessRule accessRule);

        string GetProgramFilesPath();

        string EnvironmentExpandVariable(string environmentVariable);
    }
}
