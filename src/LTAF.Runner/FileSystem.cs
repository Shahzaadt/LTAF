using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Security.AccessControl;

namespace LTAF.Runner
{
    public class FileSystem: IFileSystem
    {
        private string _currentDirectory;

        public string GetProgramFilesPath()
        {
            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public IEnumerable<string> GetSubDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath);
        }

        public void CopyFile(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath, true);
        }

        public string CurrentDirectory
        {
            get 
            {
                if(_currentDirectory == null)
                {
                    _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                return _currentDirectory;
            }
        }

        public IEnumerable<string> GetFilesInDirectory(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public string ReadFile(string filePath)
        {
            Thread.Sleep(2000);
            return File.ReadAllText(filePath);
        }

        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        public void DeleteDirectory(string directoryPath)
        {
            Directory.Delete(directoryPath, true);
        }

        public void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public FileAttributes GetFileAttributes(string filePath)
        {
            return File.GetAttributes(filePath);
        }

        public void SetFileAttributes(string filePath, FileAttributes fileAttributes)
        {
            File.SetAttributes(filePath, fileAttributes);
        }

        public void AddDirectoryAccessRule(string directoryPath, FileSystemAccessRule accessRule)
        {
            DirectoryInfo dir = new DirectoryInfo(directoryPath);
            DirectorySecurity dSec = dir.GetAccessControl();
            dSec.AddAccessRule(accessRule);
            dir.SetAccessControl(dSec);
        }

        public string EnvironmentExpandVariable(string environmentVariable)
        {
            return Environment.GetEnvironmentVariable(environmentVariable);
        }

    }
}
