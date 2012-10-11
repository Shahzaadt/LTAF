using System;
using System.IO;
using System.Text;
using LTAF.Infrastructure.Abstractions;

namespace LTAF.Infrastructure.Test.Abstractions
{
    public abstract class FileSystemBase : IFileSystem
    {
        public abstract bool DirectoryExists(string directoryPath);
        public abstract string[] DirectoryGetSubDirs(string directoryPath, string searchPattern, SearchOption searchOption);
        public abstract void DirectoryCreate(string path);
        public abstract void DirectoryDelete(string directoryPath, bool recursive);
        public abstract string[] DirectoryGetFiles(string directoryPath, string searchPattern, SearchOption searchOption);
        public abstract void DirectoryCopy(string sourcePath, string destinationPath);
        public abstract void DirectorySetAttribute(string dirpath, FileAttributes attributes);

        public abstract bool FileExists(string filePath);
        public abstract void FileCopy(string sourceFileName, string destFileName, bool overwrite);
        public abstract string FileRead(string fileName);
        public abstract void FileWrite(string filename, string content, Encoding encoding);
        public abstract void FileWrite(string fileName, string content);
        public abstract void FileDelete(string fileName);
        public abstract void FileMove(string sourceFileName, string destFileName);
        public abstract Version FileGetVersion(string fileName);
    }
}
