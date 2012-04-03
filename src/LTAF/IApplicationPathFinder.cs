using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTAF
{
    /// <summary>
    /// Describes a type that can find the Application Path to use by HtmlPage
    /// </summary>
    public interface IApplicationPathFinder
    {
        /// <summary>
        /// When implemented returns physical path to an application
        /// </summary>
        /// <returns>physical path to an application</returns>
        string GetApplicationPath();
    }
}
