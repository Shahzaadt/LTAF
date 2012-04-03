using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LTAF.UnitTests.Mock
{
    public class MockFileSystem : IFileSystem
    {
        public MockFileSystem()
        {
            Files = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Files
        {
            get;
            set;
        }

        #region IFileSystem Members
        public void WriteAllText(string filePath, string fileContent)
        {
            Files.Add(filePath, fileContent);
        }

        public bool FileExists(string filePath)
        {
            return false;
        }

        public Stream OpenFileStream(string fileFullName, FileMode fileMode, FileAccess fileAccess)
        {
            string content = "";

            if (Files.TryGetValue(fileFullName, out content))
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(content));
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
