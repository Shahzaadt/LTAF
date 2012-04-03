using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTAF;

namespace LTAF.UnitTests
{
    public class MockResourceReader: IWebResourceReader
    {
        private Dictionary<string, string> _resourceStrings;

        public MockResourceReader()
        {
            _resourceStrings = new Dictionary<string, string>();
        }

        internal void SetResourceString(string assemblyName, string resourceName, string resourceKey, string expectedString)
        {
            _resourceStrings.Add(String.Format("{0}.{1}.{2}", assemblyName, resourceName, resourceKey), expectedString);
        }

        #region IResourceReader Members
        public string ReadString(string assemblyName, string resourceName, string resourceKey)
        {
            string key = String.Format("{0}.{1}.{2}", assemblyName, resourceName, resourceKey);

            if(_resourceStrings.ContainsKey(key))
            {
                return _resourceStrings[key];
            }
            else
            {
                return "my dummy string";
            }
        }
        #endregion
    }
}
