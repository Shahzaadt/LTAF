using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Describes a type that can retrieve resource strings
    /// </summary>
    public interface IWebResourceReader
    {
        /// <summary>ReadString</summary>
        string ReadString(string assemblyName, string resourceName, string resourceKey);
    }
}
