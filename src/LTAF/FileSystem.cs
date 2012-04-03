using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LTAF
{
    /// <summary>
    /// Implementation of the Sys.IO file system
    /// </summary>
    internal class FileSystem : IFileSystem
    {
        public void WriteAllText(string filePath, string fileContent)
        {
            File.WriteAllText(filePath, fileContent);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public Stream OpenFileStream(string fileFullName, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(fileFullName, fileMode, fileAccess);
        }
    }
}
