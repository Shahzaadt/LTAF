using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LTAF
{
    /// <summary>
    /// FileSystem abstraction for unit testing
    /// </summary>
    internal interface IFileSystem
    {
        void WriteAllText(string filePath, string fileContent);
        bool FileExists(string filePath);
        Stream OpenFileStream(string fileFullName, FileMode fileMode, FileAccess fileAccess);
    }
}
