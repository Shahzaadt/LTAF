using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Describes a type that can handle the result of an Assert call
    /// </summary>
    public interface IAssertResultHandler
    {
        /// <summary>AssertFailed</summary>
        void AssertFailed(string message);
        /// <summary>AssertPassed</summary>
        void AssertPassed(string message);
    }
}
