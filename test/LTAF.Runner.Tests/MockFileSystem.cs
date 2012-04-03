using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF.Runner;
using System.IO;
using System.Security.AccessControl;

namespace LTAF.Runner.Tests
{
    public class SetFileAttributeInfo
    {

        public string FilePath
        {
            get;
            set;
        }

        public FileAttributes FileAttributes
        {
            get;
            set;
        }
    }

    public class CopyFileInfo
    {
        public string SourcePath
        {
            get;
            set;
        }

        public string DestinationPath
        {
            get;
            set;
        }
    }

    public class MockFileSystem: IFileSystem
    {
        

        public MockFileSystem()
        {
            Files = new Dictionary<string, string>();
            Directories = new List<string>();
            CopiedFiles = new List<CopyFileInfo>();
            FileAttributes = new Dictionary<string, FileAttributes>();
            CurrentDirectory = "D:\\LTAFRunner";
            PolledFiles = new Dictionary<string, int>();
            DirectoryAccessRules = new Dictionary<string, FileSystemAccessRule>();
        }

        public string CurrentDirectory
        {
            get;
            set;
        }

        public Dictionary<string, string> Files
        {
            get;
            set;
        }

        public Dictionary<string, int> PolledFiles
        {
            get;
            set;

        }

        public List<string> Directories
        {
            get;
            set;
        }

        public List<CopyFileInfo> CopiedFiles
        {
            get;
            set;
        }

        public Dictionary<string, FileAttributes> FileAttributes
        {
            get;
            set;
        }

        public Dictionary<string,FileSystemAccessRule> DirectoryAccessRules
        {
            get;
            set;
        }

    
        public bool FileExists(string file)
        {
            if (PolledFiles.ContainsKey(file))
            {
                PolledFiles[file]++;
            }
            else
            {
                PolledFiles.Add(file, 1);
            }
            return Files.ContainsKey(file);       
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directories.Contains(directoryPath);
        }

        public IEnumerable<string> GetSubDirectories(string dirPath)
        {
            List<string> subDirs = new List<string>() ;

            foreach (string dir in Directories)
            {
                if (IsSubItem(dirPath, dir))
                {
                    subDirs.Add(dir);
                }
            }
            return subDirs;
        }

        private bool IsSubItem(string parent, string child)
        {
            if (child.Contains(parent))
            {
                string subItem = child.Replace(parent, "").Trim(new char[1] { '\\' });
                if (!String.IsNullOrEmpty(subItem) && !subItem.Contains("\\"))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyFile(string sourcePath, string destinationPath)
        {
            this.CopiedFiles.Add(new CopyFileInfo() { SourcePath = sourcePath, DestinationPath = destinationPath });
        }

        public void DeleteFile(string logPath)
        {
            Files.Remove(logPath);
        }

        public string GetProgramFilesPath()
        {
            return @"D:\Program Files";
        }

        public string ReadFile(string logPath)
        {
            return Files[logPath];
        }

        public void CreateDirectory(string destinationPath)
        {
            Directories.Add(destinationPath);
        }

        public void DeleteDirectory(string subDir)
        {
            Directories.Remove(subDir);
        }

        public void WriteFile(string p, string log)
        {
            Files.Add(p, log);
        }

        public FileAttributes GetFileAttributes(string filePath)
        {
            return this.FileAttributes[filePath];
        }

        public void SetFileAttributes(string filePath, FileAttributes fileAttributes)
        {
            if (this.FileAttributes.ContainsKey(filePath))
            {
                this.FileAttributes[filePath] = fileAttributes;
            }
            else
            {
                this.FileAttributes.Add(filePath, fileAttributes);
            }
        }

        public IEnumerable<string> GetFilesInDirectory(string sourcePath)
        {
            List<string> subFiles = new List<string>();

            foreach (string file in this.Files.Keys)
            {
                if (IsSubItem(sourcePath, file))
                {
                    subFiles.Add(file);
                }
                
            }

            return subFiles;
        }

        public void AddDirectoryAccessRule(string directoryPath, FileSystemAccessRule accessRule)
        {
            DirectoryAccessRules.Add(directoryPath, accessRule);
        }

        public string EnvironmentExpandVariable(string environment)
        {
            return "invalid";
        }

    }
}
