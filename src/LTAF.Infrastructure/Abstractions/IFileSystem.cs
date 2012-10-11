using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LTAF.Infrastructure.Abstractions
{
    internal interface IFileSystem
    {
        bool DirectoryExists(string directoryPath);
        string[] DirectoryGetSubDirs(string directoryPath, string searchPattern, SearchOption searchOption);
        void DirectoryCreate(string path);
        void DirectoryDelete(string directoryPath, bool recursive);
        string[] DirectoryGetFiles(string directoryPath, string searchPattern, SearchOption searchOption);
        void DirectoryCopy(string sourcePath, string destinationPath);
        void DirectorySetAttribute(string dirpath, FileAttributes attributes);

        bool FileExists(string filePath);
        void FileCopy(string sourceFileName, string destFileName, bool overwrite);
        string FileRead(string fileName);
        void FileWrite(string filename, string content, Encoding encoding);
        void FileWrite(string fileName, string content);
        void FileDelete(string fileName);
        void FileMove(string sourceFileName, string destFileName);
        Version FileGetVersion(string fileName);
    }
}
